namespace CoverageIncr.Pipelines;

public class PipelineConfig
{
    public string Name { get; set; }
    public string Receiver { get; set; }
    public List<string> Processors { get; set; } = new();
    public string Exporter { get; set; }
}

public class MultiPipelineConfig
{
    public IEnumerable<PipelineConfig> Pipelines { get; set; }
}