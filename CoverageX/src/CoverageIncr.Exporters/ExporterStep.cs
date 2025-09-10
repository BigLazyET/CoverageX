using CoverageIncr.Shared;

namespace CoverageIncr.Exporters;

public class ExporterStep(IExporter exporter) : IPipelineStep
{
    public Task StartAsync() => exporter.StartAsync();

    public Task StopAsync() => exporter.StopAsync();
    
    public async Task<PipelineContext> ExecuteAsync(PipelineContext ctx)
    {
        var result = await exporter.ExportAsync(ctx);
        return result;
    }
}