using CoverageIncr.Shared;

namespace CoverageIncr.Pipelines;

public class PipelineExecutor : IPipelineExecutor
{
    private readonly IPipelineFactory _pipelineFactory;

    public PipelineExecutor(IPipelineFactory pipelineFactory)
    {
        _pipelineFactory = pipelineFactory;
    }

    public async Task Run()
    {
        // var pipelines = AppDomain.CurrentDomain.GetAssemblies()
        var pipelines = _pipelineFactory.GetPipelines();
        foreach (var pipeline in pipelines)
        {
            Console.WriteLine($"pipeline {pipeline.Key} start run");

            var pipelineSteps = pipeline.Value;
            
            PipelineContext context = null;
            foreach (var pipelineStep in pipelineSteps)
            {
                await pipelineStep.StartAsync();
                context = await pipelineStep.ExecuteAsync(context);
                await pipelineStep.StopAsync();
            }
        }
    }
}