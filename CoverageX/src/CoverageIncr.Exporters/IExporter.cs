using CoverageIncr.Shared;

namespace CoverageIncr.Exporters;

public interface IExporter : IComponentLifecycle
{
    
}

public interface IExporter<TIn> : IExporter
{
    Task ReceiveAsync(PipelineContext<TIn> ctx);
}
