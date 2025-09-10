using CoverageIncr.Shared;

namespace CoverageIncr.Processors;

public class ProcessorStep(IProcessor processor) : IPipelineStep
{
    public Task StartAsync() => processor.StartAsync();

    public Task StopAsync() => processor.StopAsync();

    public async Task<PipelineContext> ExecuteAsync(PipelineContext ctx)
    {
        var result = await processor.ProcessAsync(ctx);
        return result;
    }
}