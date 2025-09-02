using CoverageIncr.Shared;

namespace CoverageIncr.Pipelines;

/// <summary>
/// 执行器
/// </summary>
public static class PipelineExecutor
{
    public static async Task ExecuteAsync(string pipelineName, IEnumerable<Func<IPipelineStep>> steps)
    {
        try
        {
            object ctx = null;
            foreach (var stepFunc in steps)
            {
                var step = stepFunc();
                await step.StartAsync();
                ctx = await step.ExecuteAsync(ctx);
                await step.StopAsync();
            }
        }
        finally
        {
            
        }
    }
}