using CoverageIncr.Processors.Options;
using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;
using LibGit2Sharp;

namespace CoverageIncr.Processors.Processors;

[Processor(Name = "diff", OptionType = typeof(DiffProcOption))]
public class DiffProcessor(DiffProcOption option) : ProcessorBase<DiffProcOption>(option)
{
    public override Task<PipelineContext> ProcessAsync(PipelineContext ctx)
    {
        var featureBranch = ctx.Config.FeatureBranch;
        var featurePath = ctx.Config.FeaturePath;
        var baseBranch = ctx.Config.BaseBranch;

        using var repo = new Repository(featurePath);

        if (!IsValidGitNode(repo, baseBranch, out var commit1)) // master
            throw new InvalidOperationException($"{baseBranch}没有找到对应的branch, tag或者commitid");
        if (!IsValidGitNode(repo, featureBranch, out var commit2)) // xjl-baz
            throw new InvalidOperationException($"{featureBranch}没有找到对应的branch, tag或者commitid");

        if (commit1 == null || commit2 == null)
            throw new InvalidOperationException($"没有找到对应的branch, tag或者commitid");

        var mergeResult = repo.ObjectDatabase.MergeCommits(commit2, commit1,
            new MergeTreeOptions { MergeFileFavor = MergeFileFavor.Ours });

        if (mergeResult.Status == MergeTreeStatus.Conflicts)
            throw new InvalidOperationException($"无法处理{baseBranch}和{featureBranch}的merge request，请查看是否存在冲突");

        var patch = repo.Diff.Compare<Patch>(commit1.Tree, mergeResult.Tree);
        if (patch == null || !patch.Any())
            throw new InvalidOperationException($"{baseBranch}和{featureBranch}不存在变更，无法生成增量");

        var diffFiles = patch.Where(x => x.Status is ChangeKind.Added or ChangeKind.Modified)
            .Where(x => Path.GetExtension(x.Path) == ".cs");

        var patchChanges = diffFiles as PatchEntryChanges[] ?? diffFiles.ToArray();
        if (patchChanges.Length == 0)
            throw new InvalidOperationException($"{baseBranch}和{featureBranch}不存在变更，无法生成增量");
        
        ctx.PatchChanges = patchChanges;
        ctx.BaseCommit = commit1;
        ctx.FeatureCommit = commit2;

        return Task.FromResult(ctx);
    }

    private static bool IsValidGitNode(Repository repo, string gitNode, out Commit? commit)
    {
        var branch = repo.Branches.FirstOrDefault(x => x.FriendlyName == $"origin/{gitNode}");
        if (branch != null)
        {
            commit = branch.Tip;
            return true;
        }

        var tag = repo.Tags.FirstOrDefault(x => x.FriendlyName == gitNode);
        if (tag != null)
        {
            commit = tag.PeeledTarget as Commit;
            return true;
        }

        var gitCommit = repo.Commits.FirstOrDefault(x => x.Sha.StartsWith(gitNode));
        if (gitCommit != null)
        {
            commit = gitCommit;
            return true;
        }

        commit = null;
        return false;
    }
}