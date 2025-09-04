using CoverageIncr.Shared;

namespace CoverageIncr.Receivers;

public abstract class ReceiverBase<TIn, TOut> : IReceiver<TIn, TOut>
{
    protected TIn _option;

    public ReceiverBase(TIn option)
    {
        _option = option;
    }

    public abstract Task<PipelineContext<TOut>> ReceiveAsync();

    public Task StartAsync() => Task.CompletedTask;

    public Task StopAsync() => Task.CompletedTask;
}