using CoverageIncr.Shared;
using CoverageIncr.Shared.Pipelines;

namespace CoverageIncr.Pipelines;

public class PipelineBuilder
{
    private readonly ComponentsRegistry _componentsRegistry;
    
    public PipelineBuilder(ComponentsRegistry componentsRegistry)
    {
        _componentsRegistry = componentsRegistry;
    }

    public IEnumerable<IPipelineStep> Build()
    {
        

        return null;
    }
}