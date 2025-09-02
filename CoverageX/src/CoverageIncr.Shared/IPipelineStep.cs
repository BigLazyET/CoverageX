namespace CoverageIncr.Shared;

public interface IPipelineStep : IComponentLifecycle
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    Task<object> ExecuteAsync(object ctx);
}