using CoverageIncr.Shared;

namespace CoverageIncr.Processors;

public interface IProcessor : IComponentLifecycle
{
    
}

public interface IProcessor<TIn, TOut> : IProcessor
{
    Task<PipelineContext<TOut>> ProcessAsync(PipelineContext<TIn> ctx);
}
