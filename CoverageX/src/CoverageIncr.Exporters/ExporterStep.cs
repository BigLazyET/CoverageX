using CoverageIncr.Shared;

namespace CoverageIncr.Exporters;

public class ExporterStep<TIn> : IPipelineStep
{
    private readonly IExporter<TIn> _exporter;

    public ExporterStep(IExporter<TIn> exporter) => _exporter = exporter;
    
    public Task StartAsync() => _exporter.StartAsync();

    public Task StopAsync() => _exporter.StopAsync();

    public async Task<object> ExecuteAsync(object ctx)
    {
        var typedCtx = ctx as PipelineContext<TIn>;
        await _exporter.ReceiveAsync(typedCtx);
        return null;
    }
}