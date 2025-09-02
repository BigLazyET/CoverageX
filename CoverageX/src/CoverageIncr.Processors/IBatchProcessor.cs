using CoverageIncr.Shared;

namespace CoverageIncr.Processors;

public interface IBatchProcessor : IComponentLifecycle
{
}

public interface IBatchProcessor<TIn, TOut> : IBatchProcessor
{
    Task<PipelineContext<IEnumerable<TOut>>> ProcessAsync(PipelineContext<IEnumerable<TIn>> ctx);
}