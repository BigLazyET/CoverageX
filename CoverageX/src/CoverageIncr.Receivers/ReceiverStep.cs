using CoverageIncr.Shared;

namespace CoverageIncr.Receivers;

public class ReceiverStep<TOut> : IPipelineStep
{
    private readonly IReceiver<TOut> _receiver;

    public ReceiverStep(IReceiver<TOut> receiver) => _receiver = receiver;
    
    public async Task<object> ExecuteAsync(object ctx) => await _receiver.ReceiveAsync(ctx);
    
    public Task StartAsync() => _receiver.StartAsync();
    public Task StopAsync() => _receiver.StopAsync();
}