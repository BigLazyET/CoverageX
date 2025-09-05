using CoverageIncr.Receivers.Options;
using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;

namespace CoverageIncr.Receivers;

[Receiver(Name = "file", OptionType = typeof(string), OutType = typeof(IEnumerable<string>))]
public class FileReceiver : ReceiverBase<string, IEnumerable<string>>
{
    public FileReceiver(string option) : base(option)
    {
    }

    public override Task<PipelineContext<IEnumerable<string>>> ReceiveAsync()
    {
        var pipelineContext = new PipelineContext<IEnumerable<string>> { Data = _option.Split(';') };
        return Task.FromResult(pipelineContext);
    }
}