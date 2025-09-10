using CoverageIncr.Shared;

namespace CoverageIncr.Receivers;

public interface IReceiver : IComponentLifecycle
{
    Task<PipelineContext> ReceiveAsync(PipelineContext ctx);
}