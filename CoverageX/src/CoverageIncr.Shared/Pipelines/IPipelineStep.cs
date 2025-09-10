namespace CoverageIncr.Shared;

public interface IPipelineStep : IComponentLifecycle
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    Task<PipelineContext> ExecuteAsync(PipelineContext ctx);
}