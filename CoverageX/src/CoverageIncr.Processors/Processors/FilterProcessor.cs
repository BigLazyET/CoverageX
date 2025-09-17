using CoverageIncr.Processors.Options;
using CoverageIncr.Shared;
using LibGit2Sharp;

namespace CoverageIncr.Processors.Processors;

public class FilterProcessor(FilterOption option, IMethodChangeService methodChangeService) : ProcessorBase<FilterOption>(option)
{
    public override Task<PipelineContext> ProcessAsync(PipelineContext ctx)
    {
        using var repo = new Repository(ctx.Config.Repository);
        var changeLines = ctx.ChangeLines;
        var filters = new List<ReportGeneratorFilter>();
        
        foreach (var patchChange in ctx.PatchChanges)
        {
            var deployFilePath = Path.Combine(ctx.Config.DeployPath, patchChange.Path);
            if (!changeLines.ContainsKey(deployFilePath)) continue;
            
            var baseBlob = repo.Lookup<Blob>(ctx.BaseCommit + ":" + patchChange.Path);
            var featureBlob = repo.Lookup<Blob>(ctx.FeatureCommit + ":" + patchChange.Path);
            var deployPath = Path.Combine(ctx.Config.DeployPath, patchChange.Path);
            var filter = methodChangeService.GetDiffMethodDescriptions(baseBlob, featureBlob, deployPath);
            if (filter.ClassFilters.Count == 0) continue;
            filters.Add(filter);
        }
        ctx.Filters = filters;
        return Task.FromResult(ctx);
    }
}