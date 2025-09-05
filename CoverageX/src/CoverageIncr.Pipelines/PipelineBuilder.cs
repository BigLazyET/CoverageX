using CoverageIncr.Configurations.Interfaces;
using CoverageIncr.Receivers;
using CoverageIncr.Shared;

namespace CoverageIncr.Pipelines;

public class PipelineBuilder
{
    private readonly ComponentsRegistry _componentsRegistry;
    
    private readonly ICxConfiguration _cxConfiguration;
    public PipelineBuilder(ICxConfiguration cxConfiguration, ComponentsRegistry componentsRegistry)
    {
        _cxConfiguration = cxConfiguration;
        _componentsRegistry = componentsRegistry;
    }

    public IDictionary<string, IEnumerable<IPipelineStep>> Build()
    {
        var pipelines = _cxConfiguration.GetPipelines();

        foreach (var pipeline in pipelines)
        {
            var pipelineScope = pipeline.Value;
            var pipelineSteps = new List<IPipelineStep>();
            
            var receiverName = pipelineScope.Receiver;
            if (!ComponentsGallery.TryGetReceiver(receiverName, out var componentInfo)) continue;
            var optionInstance = _cxConfiguration.GetPipelineComponent(receiverName, ComponentType.Receiver, componentInfo.OptionsType);
            var reciver = Activator.CreateInstance(componentInfo.ImplType, optionInstance)!;
            var stepType = typeof(ReceiverStep<>).MakeGenericType(componentInfo.outType);
            var stepInstance = (IPipelineStep)Activator.CreateInstance(stepType, reciver)!;
            pipelineSteps.Add(stepInstance);
            
            var processorNames = pipelineScope.Processors;
            foreach (var processorName in processorNames)
            {
                
            }
        }

        return null;
    }
}