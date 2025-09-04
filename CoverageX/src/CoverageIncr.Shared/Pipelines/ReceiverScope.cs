namespace CoverageIncr.Shared.Pipelines;

public class ReceiverScope
{
    public string Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}