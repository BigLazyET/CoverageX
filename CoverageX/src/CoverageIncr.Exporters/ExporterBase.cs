using CoverageIncr.Shared;

namespace CoverageIncr.Exporters;

public abstract class ExporterBase<TOption>(TOption option) : IExporter
{
    protected TOption Option = option;

    public abstract Task<PipelineContext> ExportAsync(PipelineContext ctx);

    public Task StartAsync() => Task.CompletedTask;

    public Task StopAsync() => Task.CompletedTask;
}