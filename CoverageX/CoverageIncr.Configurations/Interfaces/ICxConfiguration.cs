using CoverageIncr.Shared;
using CoverageIncr.Shared.Pipelines;

namespace CoverageIncr.Configurations.Interfaces;

public interface ICxConfiguration
{
    object? GetPipelineComponent(string componentName, ComponentType componentType, Type optionType);

    IDictionary<string, PipelineScope> GetPipelines();
}