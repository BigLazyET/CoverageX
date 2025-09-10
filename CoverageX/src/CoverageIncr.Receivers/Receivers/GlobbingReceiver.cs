using CoverageIncr.Receivers.Options;
using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace CoverageIncr.Receivers;

[Receiver(Name = "globbing", OptionType = typeof(GlobbingOption), InType = typeof(string))]
public class GlobbingReceiver(GlobbingOption option) : ReceiverBase<GlobbingOption>(option)
{
    public override Task<PipelineContext> ReceiveAsync(PipelineContext ctx)
    {
        var files = new List<string>();
        foreach (var segment in Option.Segments)
        {
            var matcher = new Matcher();
            matcher.AddInclude(segment.Glob);
            
            var matchResult = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(segment.BaseDir)));
            files.AddRange(matchResult.Files.Select(match => Path.Combine(segment.BaseDir, match.Path)));
        }
        
        ctx.CoverageFiles = files;
        return Task.FromResult(ctx);
    }
}