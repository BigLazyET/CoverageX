using CoverageIncr.Exporters;
using CoverageIncr.Processors;
using CoverageIncr.Receivers;
using CoverageIncr.Shared;

namespace CoverageIncr.Pipelines;

public static class ComponentRegistry
{
    public static Dictionary<string, (Func<IPipelineStep> factory, ComponentTypeSignature signature)> registry = [];

    public static void RegistReceiver<TOut>(string name, Func<IReceiver<TOut>> receiver)
    {
        registry.Add(name, (() => new ReceiverStep<TOut>(receiver()), new ComponentTypeSignature(null, typeof(TOut))));
    }

    public static void RegistExporter<TIn>(string name, Func<IExporter<TIn>> exporter)
    {
        registry.Add(name, (() => new ExporterStep<TIn>(exporter()), new ComponentTypeSignature(typeof(TIn), null)));
    }

    public static void RegistProcessor<TIn, TOut>(string name, Func<IProcessor<TIn, TOut>> processor)
    {
        registry.Add(name, (() => new ProcessorStep<TIn, TOut>(processor()), new ComponentTypeSignature(typeof(TIn), typeof(TOut))));
    }

    public static void RegistBatchProcessor<TIn, TOut>(string name, Func<IBatchProcessor<TIn, TOut>> processor)
    {
        registry.Add(name, (() => new BatchProcessorStep<TIn, TOut>(processor()), new ComponentTypeSignature(typeof(IEnumerable<TIn>), typeof(IEnumerable<TOut>))));
    }

    public static (Func<IPipelineStep> factory, ComponentTypeSignature signature) Resolve(string name)
    {
        if (!registry.TryGetValue(name, out var result))
            return (result.factory, result.signature);
        throw new KeyNotFoundException($"Component {name} not found");
    }
}