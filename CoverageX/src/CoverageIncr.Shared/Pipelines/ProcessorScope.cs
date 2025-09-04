namespace CoverageIncr.Shared.Pipelines;

public class ProcessorScope
{
    public string Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}