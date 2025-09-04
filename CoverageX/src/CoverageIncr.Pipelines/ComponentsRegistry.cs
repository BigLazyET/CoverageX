using CoverageIncr.Configurations;
using CoverageIncr.Configurations.Interfaces;

namespace CoverageIncr.Pipelines;

public class ComponentsRegistry
{
    public ICxConfiguration CxConfiguration { get; }

    public ComponentsRegistry(ICxConfiguration cxConfiguration)
    {
        CxConfiguration = cxConfiguration;
    }
}