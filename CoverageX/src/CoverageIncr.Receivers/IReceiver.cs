using CoverageIncr.Shared;

namespace CoverageIncr.Receivers;

public interface IReceiver<TOut> : IComponentLifecycle
{
    Task<PipelineContext<TOut>> ReceiveAsync();
}