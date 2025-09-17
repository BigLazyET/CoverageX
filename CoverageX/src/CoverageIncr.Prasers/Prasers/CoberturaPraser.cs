using System.Xml.Linq;
using CoverageIncr.Prasers.PreProcessors;
using CoverageIncr.Shared;

namespace CoverageIncr.Prasers;

public class CoberturaPraser : PraserBase
{
    private readonly CoberturaReportPreprocessor _preprocessor = new();

    private IEnumerable<string> _fileFIlters = [];
    private IEnumerable<string> _classFilters = [];
    public override CoverageFormat Format => CoverageFormat.Cobertura;

    public override IEnumerable<ParserResult> Parse(string filePath, IList<ReportGeneratorFilter> filters)
    {
        _fileFIlters = filters.Select(x => $"+{x.FilePath}");
        _classFilters = filters.SelectMany(f => f.ClassFilters.Select(c => $"+{c}"));
        
        var elements = GetXElements(filePath, "coverage");
        foreach (var element in elements)
        {
            _preprocessor.ExecuteAsync(element);
            yield return Parse(element);
        }
    }
    
    /// <summary>
        /// Parses the given XML report.
        /// </summary>
        /// <param name="report">The XML report.</param>
        /// <returns>The parser result.</returns>
        public ParserResult Parse(XContainer report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            var assemblies = new List<Assembly>();

            var assemblyElementGrouping = report.Descendants("package")
                .GroupBy(m => m.Attribute("name").Value)
                .Where(a => this.AssemblyFilter.IsElementIncludedInReport(a.Key))
                .ToArray();

            foreach (var elements in assemblyElementGrouping)
            {
                // var assembly = this.ProcessAssembly(modules, assemblyName);
                var assembly = this.ProcessAssembly(elements.ToArray(), elements.Key);
                if (assembly.Classes.Any())
                {
                    assemblies.Add(assembly);
                }
            }

            var result = new ParserResult(assemblies.OrderBy(a => a.Name).ToList(), true, this.ToString());

            foreach (var sourceElement in report.Elements("sources").Elements("source"))
            {
                result.AddSourceDirectory(sourceElement.Value);
            }

            try
            {
                if (report.Element("sources")?.Parent?.Attribute("timestamp") != null)
                {
                    DateTime timeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    timeStamp = timeStamp
                        .AddSeconds(double.Parse(report.Element("sources").Parent.Attribute("timestamp").Value))
                        .ToLocalTime();

                    result.MinimumTimeStamp = timeStamp;
                    result.MaximumTimeStamp = timeStamp;
                }
            }
            catch (Exception)
            {
                // Ignore since timestamp is not relevant. If timestamp is missing or in wrong format the information is just missing in the report(s)
            }

            return result;
        }

        /// <summary>
        /// Processes the given assembly.
        /// </summary>
        /// <param name="modules">The modules.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>The <see cref="Assembly"/>.</returns>
        private Assembly ProcessAssembly(XElement[] modules, string assemblyName)
        {
            Logger.DebugFormat(Resources.CurrentAssembly, assemblyName);

            var classes = modules
                .Elements("classes")
                .Elements("class")
                .ToArray();

            var classNames = classes
                .Select(c => ClassNameParser.ParseClassName(c.Attribute("name").Value, this.RawMode))
                .Where(c => c.Include)
                .Distinct()
                .Where(c => this.ClassFilter.IsElementIncludedInReport(c.Name))
                .OrderBy(c => c.Name)
                .ToArray();

            var assembly = new Assembly(assemblyName);

            Parallel.ForEach(classNames, c => this.ProcessClass(classes, assembly, c));

            return assembly;
        }

        /// <summary>
        /// Processes the given class.
        /// </summary>
        /// <param name="allClasses">All class elements.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="classNameParserResult">Name of the class.</param>
        private void ProcessClass(XElement[] allClasses, Assembly assembly, ClassNameParserResult classNameParserResult)
        {
            bool FilterClass(XElement element)
            {
                var name = element.Attribute("name").Value;

                return name.Equals(classNameParserResult.Name)
                       || (!this.RawMode
                           && name.StartsWith(classNameParserResult.Name, StringComparison.Ordinal)
                           && (name[classNameParserResult.Name.Length] == '$'
                               || name[classNameParserResult.Name.Length] == '/'
                               || name[classNameParserResult.Name.Length] == '.'));
            }

            var classes = allClasses
                .Where(FilterClass)
                .ToArray();

            var files = classes
                .Select(c => c.Attribute("filename").Value)
                .Distinct()
                .ToArray();

            var filteredFiles = files
                .Where(f => this.FileFilter.IsElementIncludedInReport(f))
                .ToArray();

            // If all files are removed by filters, then the whole class is omitted
            if ((files.Length == 0 && !this.FileFilter.HasCustomFilters) || filteredFiles.Length > 0)
            {
                var @class = new Class(classNameParserResult.DisplayName, classNameParserResult.RawName, assembly);

                foreach (var file in filteredFiles)
                {
                    var fileClasses = classes
                        .Where(c => c.Attribute("filename").Value.Equals(file))
                        .ToArray();
                    var codeFile = this.ProcessFile(fileClasses, @class, classNameParserResult.Name, file);
                    if (codeFile != null)
                    {
                        @class.AddFile(codeFile);
                    }
                }

                if (@class.Files.Any())
                {
                    assembly.AddClass(@class);
                }
            }
        }

        /// <summary>
        /// Processes the file.
        /// </summary>
        /// <param name="classElements">The class elements for the file.</param>
        /// <param name="class">The class.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="CodeFile"/>.</returns>
        private CodeFile ProcessFile(XElement[] classElements, Class @class, string className, string filePath)
        {
            try
            {
                var lines = classElements.Elements("lines")
                    .Elements("line")
                    .ToArray();

                var lineNumbers = lines
                    .Select(l => l.Attribute("number").Value)
                    .ToHashSet();

                var methodsOfFile = classElements
                    .Elements("methods")
                    .Elements("method")
                    .ToArray();

                var additionalLinesInMethodElement = methodsOfFile
                    .Elements("lines")
                    .Elements("line")
                    .Where(l => !lineNumbers.Contains(l.Attribute("number").Value))
                    .ToArray();

                var linesOfFile = lines.Concat(additionalLinesInMethodElement)
                    .Select(line => new
                    {
                        LineNumber = int.Parse(line.Attribute("number").Value, CultureInfo.InvariantCulture),
                        Visits = line.Attribute("hits").Value.ParseLargeInteger()
                    })
                    .OrderBy(seqpnt => seqpnt.LineNumber)
                    .ToArray();

                var branches = GetBranches(lines);

                if (linesOfFile.Length == 0)
                {
                    Log.Information($"文件 {filePath} 中的 类 {className} 不存在行");
                    return null;
                }

                int[] coverage = new int[] { };
                LineVisitStatus[] lineVisitStatus = new LineVisitStatus[] { };

                if (CoverageLine.IsIncremental == false)
                {
                    if (linesOfFile.Length > 0)
                    {
                        coverage = new int[linesOfFile[linesOfFile.LongLength - 1].LineNumber + 1];
                        lineVisitStatus = new LineVisitStatus[linesOfFile[linesOfFile.LongLength - 1].LineNumber + 1];

                        for (int i = 0; i < coverage.Length; i++)
                        {
                            coverage[i] = -1;
                        }

                        foreach (var line in linesOfFile)
                        {
                            coverage[line.LineNumber] = line.Visits;

                            bool partiallyCovered = false;

                            if (branches.TryGetValue(line.LineNumber, out ICollection<Branch> branchesOfLine))
                            {
                                partiallyCovered = branchesOfLine.Any(b => b.BranchVisits == 0);
                            }

                            LineVisitStatus statusOfLine = line.Visits > 0 ? (partiallyCovered ? LineVisitStatus.PartiallyCovered : LineVisitStatus.Covered) : LineVisitStatus.NotCovered;
                            lineVisitStatus[line.LineNumber] = statusOfLine;
                        }
                    }

                    var fullOrAutoCodeFile = new CodeFile(filePath, coverage, lineVisitStatus, branches);

                    SetMethodMetrics(fullOrAutoCodeFile, methodsOfFile);
                    SetCodeElements(fullOrAutoCodeFile, methodsOfFile);

                    return fullOrAutoCodeFile;
                }

                // key: filePath
                // value: changed line numbers
                var getLineContents = CoverageLine.ChangeLines.TryGetValue(filePath, out List<int> changedLines);
                if (!getLineContents || changedLines == null)
                {
                    Log.Error($"文件 {filePath} 没有找到变更行");
                    return null;
                }

                var classLines = linesOfFile.Select(line => line.LineNumber).Distinct(); // 有可能有重复值！所以我们做一下distinct
                var filteredChangeLines = changedLines.Where(number => number > 0 && classLines.Contains(number));
                var changedLineNumbers = filteredChangeLines as int[] ?? filteredChangeLines.ToArray();
                if (!changedLineNumbers.Any())
                {
                    // 当存在一个.cs文件中存在多个类的情况
                    // 最终显示的维度是单个类，所以我们要区分修改的行是否标记在具体的类中；解决输出多个类的bug
                    Serilog.Log.Warning($"文件 {filePath} 变更的行不存在于 类 {className}中, 忽略它");
                    return null;
                }

                var classLineNumbers = classLines as int[] ?? classLines.ToArray();
                var minLineNumber = classLineNumbers.Min();
                var maxLineNumber = classLineNumbers.Max();
                Serilog.Log.Information(
                    $"文件{filePath} 变更的行 {string.Join(",", changedLineNumbers.Select(i => i.ToString()))} 且类 {className} 行是从 {minLineNumber} 到 {maxLineNumber}");

                coverage = new int[linesOfFile[linesOfFile.LongLength - 1].LineNumber + 1];
                lineVisitStatus =
                    new LineVisitStatus[linesOfFile[linesOfFile.LongLength - 1].LineNumber + 1];

                for (int i = 0; i < coverage.Length; i++)
                {
                    coverage[i] = -1;
                }

                var markedLines = new Dictionary<int, int>();
                foreach (var line in linesOfFile)
                {
                    if (!changedLineNumbers.Contains(line.LineNumber))
                    {
                        coverage[line.LineNumber] = -1;
                        lineVisitStatus[line.LineNumber] = LineVisitStatus.NotCoverable;
                    }
                    else
                    {
                        if (markedLines.TryGetValue(line.LineNumber, out var visits))
                        {
                            markedLines[line.LineNumber] = line.Visits < 0 ? visits : visits + line.Visits;
                        }
                        else
                        {
                            markedLines.Add(line.LineNumber, line.Visits < 0 ? 0 : line.Visits);
                        }
                    }
                }

                foreach (var markedLine in markedLines)
                {
                    // -1: Not coverable 0: Not visited >0: Number of visits
                    coverage[markedLine.Key] = markedLine.Value;

                    bool partiallyCovered = false;

                    if (branches.TryGetValue(markedLine.Key, out ICollection<Branch> branchesOfLine))
                    {
                        partiallyCovered = branchesOfLine.Any(b => b.BranchVisits == 0);
                    }

                    LineVisitStatus statusOfLine = markedLine.Value > 0
                        ? (partiallyCovered ? LineVisitStatus.PartiallyCovered : LineVisitStatus.Covered)
                        : LineVisitStatus.NotCovered;
                    lineVisitStatus[markedLine.Key] = statusOfLine;
                }

                var marked = markedLines.GroupBy(m => m.Value)
                    .ToDictionary(m => m.Key, m => m.Select(l => l.Key));
                foreach (var mark in marked)
                {
                    Log.Information(
                        $"文件{filePath}中, The visits is {mark.Key} of line numbers are {string.Join(",", mark.Value)}");
                }

                if (changedLineNumbers.Any())
                {
                    branches = branches.Where(b => changedLineNumbers.Contains(b.Key))
                        .ToDictionary(kv => kv.Key, kv => kv.Value);
                }

                var codeFile = new CodeFile(filePath, coverage, lineVisitStatus, branches);

                SetMethodMetrics(codeFile, methodsOfFile);
                SetCodeElements(codeFile, methodsOfFile);

                return codeFile;
            }
            catch (Exception e)
            {
                Log.Error(e, $"CoberturaParser failed withe error message: {e.Message}");
                return null;
            }
        }
}