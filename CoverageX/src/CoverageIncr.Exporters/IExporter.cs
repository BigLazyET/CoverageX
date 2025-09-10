using CoverageIncr.Shared;

namespace CoverageIncr.Exporters;

public interface IExporter : IComponentLifecycle
{
    Task<PipelineContext> ExportAsync(PipelineContext ctx);
}
