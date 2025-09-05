using CoverageIncr.Shared;
using CoverageIncr.Shared.Pipelines;
using Microsoft.Extensions.Primitives;

namespace CoverageIncr.Configurations.Interfaces;

public interface ICxConfigurationProvider
{
    IChangeToken GetReloadToken();

    object? GetReceiver(string componentName, ComponentType componentType, Type optionType);

    IDictionary<string, PipelineScope> GetPipelines();
    
    void Load();
}