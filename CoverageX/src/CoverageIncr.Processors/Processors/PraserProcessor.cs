using CoverageIncr.Processors.Options;
using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;

namespace CoverageIncr.Processors.Processors;

[Processor(Name = "praser", OptionType = typeof(PraserProcessorOption))]
public class PraserProcessor(PraserProcessorOption option) : ProcessorBase<PraserProcessorOption>(option)
{
    public override Task<PipelineContext> ProcessAsync(PipelineContext ctx)
    {
        var coverageFiles = ctx.CoverageFiles;
        return Task.FromResult(ctx);
    }
}