using CoverageIncr.Shared;

namespace CoverageIncr.Processors;

public abstract class ProcessorBase<TOption>(TOption option) : IProcessor
{
    protected TOption Option = option;

    public abstract Task<PipelineContext> ProcessAsync(PipelineContext ctx);

    public Task StartAsync() => Task.CompletedTask;

    public Task StopAsync() => Task.CompletedTask;
}