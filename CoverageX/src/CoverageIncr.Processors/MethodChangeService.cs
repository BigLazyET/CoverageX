using System.Text;
using CoverageIncr.Processors.Extensions;
using CoverageIncr.Shared;
using LibGit2Sharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoverageIncr.Processors;

public class MethodChangeService : IMethodChangeService
{
    public ReportGeneratorFilter GetDiffMethodDescriptions(Blob targetBlob, Blob sourceBlob, string sourceFilePath)
    {
        var reportGeneratorFilter = new ReportGeneratorFilter() { FilePath = sourceFilePath };

        var sourceClasses = GetClasses(sourceBlob);

        // 如果新类为空，则没必要比较
        if (sourceClasses == null || sourceClasses.Count() == 0)
            return reportGeneratorFilter;
        // 如果旧类为空，则所有为新增
        if (targetBlob == null)
        {
            reportGeneratorFilter.ClassFilters.AddRange(sourceClasses.Keys);
            //reportGeneratorFilter.MethodFilters.AddRange(sourceClassesMethods.Values.SelectMany(x => x));
            return reportGeneratorFilter;
        }

        var targetClasses = GetClasses(targetBlob);

        // 如果旧类没有任何方法，则所有为新增
        if (targetClasses == null || targetClasses.Count() == 0)
        {
            reportGeneratorFilter.ClassFilters.AddRange(sourceClasses.Keys);
            //reportGeneratorFilter.MethodFilters.AddRange(sourceClassesMethods.Values.SelectMany(x => x));
            return reportGeneratorFilter;
        }

        // 如果两者都存在，则对比出结果
        foreach (var sourceClass in sourceClasses)
        {
            if (targetClasses.ContainsKey(sourceClass.Key))
            {

                if (!targetClasses.Values.Any(p => ((ReadOnlySpan<byte>)p).SequenceEqual((ReadOnlySpan<byte>)sourceClass.Value)))
                {
                    reportGeneratorFilter.ClassFilters.Add(sourceClass.Key);
                }
            }
            else
            {
                reportGeneratorFilter.ClassFilters.Add(sourceClass.Key);
            }
        }

        return reportGeneratorFilter;
    }
    
    private IDictionary<string, byte[]> GetClasses(Blob blob)
    {
        if (blob == null)
            return null;

        var content = blob.GetContentText(Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(content);
        var classes = syntaxTree.GetRoot().DescendantNodes().Where(x => x.GetType() == typeof(ClassDeclarationSyntax) || x.GetType() == typeof(RecordDeclarationSyntax));
        if (classes == null || classes.Count() == 0)
            return null;

        var compilation = CSharpCompilation.Create(null).AddSyntaxTrees(syntaxTree);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var classMethods = new Dictionary<string, byte[]>();
        foreach (TypeDeclarationSyntax typeDeclaration in classes)
        {
            // 获取类的标识符
            var identifier = typeDeclaration.Identifier;
            // 获取类的类型参数列表
            var typeParameterList = typeDeclaration.TypeParameterList;

            var classDisplayName = semanticModel.GetDeclaredSymbol(typeDeclaration).GetFullMetadataName();

            if (typeParameterList != null)
            {
                // 构建泛型类名
                var genericClassName = $"{identifier.Text}<{string.Join(", ", typeParameterList.Parameters.Select(p => p.Identifier.Text))}>";
                var lastDotIndex = classDisplayName.LastIndexOf('.');
                classDisplayName = classDisplayName.Substring(0, lastDotIndex) + '.' + genericClassName;
            }

            classMethods[classDisplayName] = Encoding.UTF8.GetBytes(typeDeclaration.ToString());
        }

        return classMethods;
    }

    public HashSet<int> GetMethodLineNumbers(Blob blob)
    {
        var methodLineNumbers = new HashSet<int>();
        if (blob == null)
            return methodLineNumbers;

        var content = blob.GetContentText(Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(content);
        var classes = syntaxTree.GetRoot().DescendantNodes().Where(x => x.GetType() == typeof(ClassDeclarationSyntax) || x.GetType() == typeof(RecordDeclarationSyntax));
        if (classes == null || classes.Count() == 0)
            return methodLineNumbers;
            
        foreach (var typeDeclaration in classes)
        {
            var methods = typeDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                var lineSpan = method.GetLocation().GetLineSpan();
                var lineNumber = lineSpan.StartLinePosition.Line + 1; // 行号是从0开始的，所以加1

                methodLineNumbers.Add(lineNumber);
            }
        }

        return methodLineNumbers;
    }
}