using System.Text.RegularExpressions;
using CoverageIncr.Receivers.Options;
using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace CoverageIncr.Receivers;

[Receiver(Name = "globbing", OptionType = typeof(GlobbingOption), OutType = typeof(IEnumerable<string>))]
public class GlobbingReceiver : ReceiverBase<GlobbingOption, IEnumerable<string>>
{
    public GlobbingReceiver(GlobbingOption option) : base(option)
    {
    }

    public override Task<PipelineContext<IEnumerable<string>>> ReceiveAsync()
    {
        var files = new List<string>();
        foreach (var segment in _option.Segments)
        {
            var matcher = new Matcher();
            matcher.AddInclude(segment.Glob);
            
            var matchResult = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(segment.BaseDir)));
            files.AddRange(matchResult.Files.Select(match => Path.Combine(segment.BaseDir, match.Path)));
        }
        
        var pipelineContext = new PipelineContext<IEnumerable<string>> { Data = files };
        return Task.FromResult(pipelineContext);
    }
}