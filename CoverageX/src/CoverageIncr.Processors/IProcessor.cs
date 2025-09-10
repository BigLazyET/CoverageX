using CoverageIncr.Shared;

namespace CoverageIncr.Processors;

public interface IProcessor : IComponentLifecycle
{
    Task<PipelineContext> ProcessAsync(PipelineContext ctx);
}
