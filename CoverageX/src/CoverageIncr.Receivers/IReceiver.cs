using CoverageIncr.Shared;

namespace CoverageIncr.Receivers;

public interface IReceiver<TIn, TOut> : IComponentLifecycle
{
    Task<PipelineContext<TOut>> ReceiveAsync();
}