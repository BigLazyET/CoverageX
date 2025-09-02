namespace CoverageIncr.Shared;

public class PipelineContext
{
    public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    public string TraceId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PipelineContext<T> : PipelineContext
{
    public T Data { get; set; }
}