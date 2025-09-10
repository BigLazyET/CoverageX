using CoverageIncr.Shared.Pipelines;
using LibGit2Sharp;

namespace CoverageIncr.Shared;

public class PipelineContext
{
    public PipelineConfig Config { get; set; }
    
    public string TraceId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // 中间结果
    public Dictionary<string, IList<int>> ChangeLines { get; set; } = [];
    public List<ReportGeneratorFilter> Filters { get; set; } = [];
    public IEnumerable<PatchEntryChanges> PatchChanges { get; set; }
    public Commit BaseCommit { get; set; }
    public Commit FeatureCommit { get; set; }
    public IEnumerable<string> CoverageFiles { get; set; }
}

public class ReportGeneratorFilter
{
    public string FilePath { get; set; }

    public List<string> ClassFilters { get; set; } = new List<string>();
}