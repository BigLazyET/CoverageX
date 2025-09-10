using CoverageIncr.Shared;

namespace CoverageIncr.Receivers;

public class ReceiverStep(IReceiver receiver) : IPipelineStep
{
    public Task StartAsync() => receiver.StartAsync();

    public Task StopAsync() => receiver.StopAsync();

    public async Task<PipelineContext> ExecuteAsync(PipelineContext ctx)
    {
        var result = await receiver.ReceiveAsync(ctx);
        return result;
    }
}