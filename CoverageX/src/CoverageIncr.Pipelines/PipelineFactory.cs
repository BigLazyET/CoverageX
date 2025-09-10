using CoverageIncr.Configurations.Interfaces;
using CoverageIncr.Exporters;
using CoverageIncr.Processors;
using CoverageIncr.Receivers;
using CoverageIncr.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace CoverageIncr.Pipelines;

public class PipelineFactory : IPipelineFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICxConfiguration _cxConfiguration;
    private readonly Dictionary<string, IEnumerable<IPipelineStep>> _pipelines = new();
    
    public PipelineFactory(IServiceProvider serviceProvider, ICxConfiguration cxConfiguration)
    {
        _serviceProvider = serviceProvider;
        _cxConfiguration = cxConfiguration;
        _pipelines = Build();
    }

    private Dictionary<string, IEnumerable<IPipelineStep>> Build()
    {
        var pipes = new Dictionary<string, IEnumerable<IPipelineStep>>();
        var pipelines = _cxConfiguration.GetPipelines();

        foreach (var pipeline in pipelines)
        {
            var pipelineScope = pipeline.Value;
            var pipelineSteps = new List<IPipelineStep>();
            
            var receiverName = pipelineScope.Receiver;
            if (!ComponentsGallery.TryGetReceiver(receiverName, out var receiverCompInfo)) continue;
            var receiverOptionInstance = _cxConfiguration.GetPipelineComponent(receiverName, ComponentType.Receiver, receiverCompInfo.OptionsType);
            var receiver = Activator.CreateInstance(receiverCompInfo.ImplType, receiverOptionInstance)!;
            var receiverStepInstance = (IPipelineStep)Activator.CreateInstance(typeof(ReceiverStep), receiver)!;
            pipelineSteps.Add(receiverStepInstance);
            
            var processorNames = pipelineScope.Processors;
            foreach (var processorName in processorNames)
            {
                if (!ComponentsGallery.TryGetProcessor(processorName, out var processorCompInfo))
                    throw new KeyNotFoundException($"Processor {processorName} could not be found");
                var processorOptionInstance = _cxConfiguration.GetPipelineComponent(processorName, ComponentType.Processor, processorCompInfo.OptionsType);
                // var processor = Activator.CreateInstance(processorCompInfo.ImplType, processorOptionInstance)!;
                var processor = ActivatorUtilities.CreateInstance(_serviceProvider, processorCompInfo.ImplType, processorOptionInstance!);
                var processStepInstance = (IPipelineStep)Activator.CreateInstance(typeof(ProcessorStep), processor)!;
                pipelineSteps.Add(processStepInstance);
            }
            
            var exporterName = pipelineScope.Exporter;
            if (!ComponentsGallery.TryGetExporter(exporterName, out var exporterCompInfo))
                throw new KeyNotFoundException($"Exporter {exporterName} could not be found");
            var exporterOptionInstance = _cxConfiguration.GetPipelineComponent(exporterName, ComponentType.Exporter, exporterCompInfo.OptionsType);
            var exporter = Activator.CreateInstance(exporterCompInfo.ImplType, exporterOptionInstance)!;
            var exporterStepInstance = (IPipelineStep)Activator.CreateInstance(typeof(ExporterStep), exporter)!;
            pipelineSteps.Add(exporterStepInstance);
            
            pipes.Add(pipeline.Key, pipelineSteps);
        }

        return pipes;
    }

    public IEnumerable<IPipelineStep> GetPipelines(string pipelineName)
    {
        if (!_pipelines.TryGetValue(pipelineName, out var pipeline))
            throw new KeyNotFoundException($"Pipeline {pipelineName} could not be found");
        return pipeline;
    }
    
    public Dictionary<string, IEnumerable<IPipelineStep>> GetPipelines() => _pipelines;
}