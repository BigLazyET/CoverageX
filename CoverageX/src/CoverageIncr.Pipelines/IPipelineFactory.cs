using CoverageIncr.Shared;

namespace CoverageIncr.Pipelines;

public interface IPipelineFactory
{
    IEnumerable<IPipelineStep> GetPipelines(string pipelineName);

    Dictionary<string, IEnumerable<IPipelineStep>> GetPipelines();
}