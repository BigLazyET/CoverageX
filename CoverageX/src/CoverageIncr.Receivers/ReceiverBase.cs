using CoverageIncr.Shared;

namespace CoverageIncr.Receivers;

public abstract class ReceiverBase<TOption>(TOption option) : IReceiver
{
    protected TOption Option = option;

    public abstract Task<PipelineContext> ReceiveAsync(PipelineContext ctx);

    public Task StartAsync() => Task.CompletedTask;

    public Task StopAsync() => Task.CompletedTask;
}