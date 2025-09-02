using CoverageIncr.Shared;

namespace CoverageIncr.Processors;

public class ProcessorStep<TIn, TOut> : IPipelineStep
{
    private readonly IProcessor<TIn, TOut> _processor;

    public ProcessorStep(IProcessor<TIn, TOut> processor) => _processor = processor;
    
    public Task StartAsync() => _processor.StartAsync();

    public Task StopAsync() => _processor.StopAsync();

    public async Task<object> ExecuteAsync(object ctx)
    {
        var pipelineCtx = ctx as PipelineContext<TIn>;
        var result = await _processor.ProcessAsync(pipelineCtx);
        return result;
    }
}