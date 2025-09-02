using CoverageIncr.Shared;

namespace CoverageIncr.Processors;

public class BatchProcessorStep<TIn, TOut> : IPipelineStep
{
    private readonly IBatchProcessor<TIn, TOut> _batchProcessor;

    public BatchProcessorStep(IBatchProcessor<TIn, TOut> batchProcessor) => _batchProcessor = batchProcessor;
    
    public Task StartAsync() => _batchProcessor.StartAsync();

    public Task StopAsync() => _batchProcessor.StopAsync();

    public async Task<object> ExecuteAsync(object ctx)
    {
        var typedCtx = ctx as PipelineContext<IEnumerable<TIn>>;
        var result = await _batchProcessor.ProcessAsync(typedCtx);
        return result;
    }
}