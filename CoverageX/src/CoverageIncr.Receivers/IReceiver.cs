using CoverageIncr.Shared;

namespace CoverageIncr.Receivers;

public interface IReceiver : IComponentLifecycle
{
    
}

public interface IReceiver<TOut> : IReceiver
{
    Task<PipelineContext<TOut>> ReceiveAsync(object ctx);
}