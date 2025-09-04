namespace CoverageIncr.Shared.Pipelines;

public class PipelineScope
{
    public string Receiver { get; set; }
    public List<string> Processors { get; set; } = new();
    public string Exporter { get; set; }
}