using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;

namespace CoverageIncr.Receivers;

[Receiver(Name = "file", OptionType = typeof(string))]
public class FileReceiver(string option) : ReceiverBase<string>(option)
{
    public override Task<PipelineContext> ReceiveAsync(PipelineContext ctx)
    {
        ctx.CoverageFiles = Option.Split(';');
        return Task.FromResult(ctx);
    }
}