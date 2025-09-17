using System.Collections.Concurrent;
using CoverageIncr.Shared;

namespace CoverageIncr.Prasers;

public class CoverageProcessDirector
{
    private readonly List<ICoverageFormatDetector> _detectors;
    private readonly List<ICoverageParser> _parsers;

    // 缓存格式 → Parser 映射
    private readonly ConcurrentDictionary<CoverageFormat, ICoverageParser> _parserCache = new();

    public CoverageProcessDirector(IEnumerable<ICoverageFormatDetector> detectors,
        IEnumerable<ICoverageParser> parsers)
    {
        _detectors = detectors.ToList();
        _parsers = parsers.ToList();

        BuildParserCache();
    }

    private void BuildParserCache()
    {
        // 组装缓存: 按 Format 分组，选 Priority 最大的 Parser
        var grouped = _parsers.GroupBy(p => p.Format);

        foreach (var grp in grouped)
        {
            var best = grp.OrderByDescending(p =>
            {
                if (p is IPrioritizable pr) return pr.Priority;
                return 0; // 默认0优先级
            }).First();

            _parserCache[grp.Key] = best;
        }

        Console.WriteLine("=== Parser Mapping ===");
        foreach (var kv in _parserCache)
        {
            var name = kv.Value.GetType().Name;
            var pri = kv.Value is IPrioritizable pr ? pr.Priority : 0;
            Console.WriteLine($"{kv.Key} => {name} (priority {pri})");
        }
    }

    public async Task<List<ParserResult>> ProcessAllAsync(PipelineContext context)
    {
        var results = new List<ParserResult>();
        var tasks = context.CoverageFiles.Select(filePath => Task.Run(() =>
        {
            var detectedFormat = DetectFormat(filePath);
            if (!_parserCache.TryGetValue(detectedFormat, out var parser))
                throw new NotSupportedException($"No parser found for {detectedFormat}");
            results.AddRange(parser.Parse(filePath));
        }));

        await Task.WhenAll(tasks);

        return results;
    }

    private CoverageFormat DetectFormat(string filePath)
    {
        var confidences = _detectors
            .Select(d => (detector: d, score: SafeDetect(d, filePath), pri: PriorityOf(d)))
            .Where(x => x.score >= 0)
            .OrderByDescending(x => x.score)
            .ThenByDescending(x => x.pri)
            .ToList();

        if (!confidences.Any())
            throw new NotSupportedException($"Unknown coverage format for file {filePath}");

        var chosen = confidences.First();
        Console.WriteLine(
            $"[Detect] {filePath} detected as {chosen.detector.Format} (score {chosen.score}, pri {chosen.pri})");
        return chosen.detector.Format;
    }

    private int SafeDetect(ICoverageFormatDetector d, string file)
    {
        try
        {
            return d.DetectConfidence(file);
        }
        catch
        {
            return -1;
        }
    }

    private int PriorityOf(object obj)
    {
        return obj is IPrioritizable pr ? pr.Priority : 0;
    }
}