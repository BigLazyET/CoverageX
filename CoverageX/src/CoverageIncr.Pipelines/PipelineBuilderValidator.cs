using CoverageIncr.Shared;

namespace CoverageIncr.Pipelines;

public static class PipelineBuilderValidator
{
    public static IEnumerable<Func<IPipelineStep>> BuildAndValidate(PipelineConfig config)
    {
        var steps = new List<Func<IPipelineStep>>();
        
        var receiver = ComponentRegistry.Resolve(config.Receiver);
        var currentType = receiver.signature.OutputType;
        steps.Add(receiver.factory);

        foreach (var procName in config.Processors)
        {
            var processor = ComponentRegistry.Resolve(procName);
            var inType = processor.signature.InputType;
            if (inType != currentType)
                throw new Exception($"Processor '{procName}' 输入类型 {inType.Name} 不匹配当前类型 {currentType.Name}");
            currentType = processor.signature.OutputType;
            steps.Add(processor.factory);
        }
        
        var exporter = ComponentRegistry.Resolve(config.Exporter);
        if (exporter.signature.InputType != currentType)
            throw new Exception($"Exporter '{config.Exporter}' 输入类型 {exporter.signature.InputType.Name} 不匹配当前类型 {currentType.Name}");
        steps.Add(exporter.factory);

        return steps;
    }
}