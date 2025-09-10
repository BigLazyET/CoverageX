using CoverageIncr.Exporters.Options;
using CoverageIncr.Shared;

namespace CoverageIncr.Exporters.Exporters;

public class MqExporter(MqExporterOption option) : ExporterBase<MqExporterOption>(option)
{
    public override Task<PipelineContext> ExportAsync(PipelineContext ctx)
    {
        return Task.FromResult(ctx);
    }
}