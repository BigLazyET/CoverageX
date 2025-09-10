using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;
using LibGit2Sharp;

namespace CoverageIncr.Processors.Processors;

[Processor(Name = "change", OptionType = typeof(string))]
public class ChangeProcessor(string option, IMethodChangeService methodChangeService) : ProcessorBase<string>(option)
{
    public override Task<PipelineContext> ProcessAsync(PipelineContext ctx)
    {
        using var repo = new Repository(ctx.Config.Repository);

        foreach (var patchChange in ctx.PatchChanges)
        {
            var coveragePath = Path.Combine(ctx.Config.FeaturePath, patchChange.Path);
            var deployFilePath = Path.Combine(ctx.Config.DeployPath, patchChange.Path);
            
            var linePatches = patchChange.AddedLines
                .Where(x => !string.IsNullOrWhiteSpace(x.Content.Trim()) && x.Content.Trim() != "{" &&
                            x.Content.Trim() != "}" &&
                            !x.Content.Trim().StartsWith("//"))
                .ToDictionary(x => x.LineNumber, x => x.Content.TrimEnd());
            
            var sourceBlob = repo.Lookup<Blob>(ctx.FeatureCommit + ":" + patchChange.Path);
            var methodLineNumbers = methodChangeService.GetMethodLineNumbers(sourceBlob);
            linePatches = linePatches.Where(x => !methodLineNumbers.Contains(x.Key)).ToDictionary();

            if (linePatches.Count == 0) continue;

            ctx.ChangeLines[deployFilePath] = [];

            var keyLines = File.ReadAllLines(deployFilePath);
            foreach (var linePatch in linePatches)
            {
                var lineNumber = linePatch.Key;
                var lineContent = linePatch.Value;
                var keyLineContent = keyLines[lineNumber - 1]?.TrimEnd();
                if (lineContent == keyLineContent)
                {
                    ctx.ChangeLines[deployFilePath].Add(lineNumber);
                }
                else
                {
                    // Log.Warning($"文件{coveragePath}和文件{sourcePath}的相同行号{lineNumber}的内容不一致，我们需要进一步处理");
                    // 当xjl-baz和uat相同行号的内容却不一样，代表uat分支可能被其他分支合并修改了，这个时候我们再找uat中相同内容的行号，更正最终uat的内容对应的行号信息
                    var indexes = keyLines.Select((k, i) => k?.TrimEnd() == lineContent ? i : -1).Where(k => k != -1)
                        .ToArray();
                    if (indexes.Length == 0) continue;
                    
                    var closest = FindClosest(indexes, lineNumber - 1);
                    ctx.ChangeLines[deployFilePath].Add(closest + 1);
                }
            }

            if (ctx.ChangeLines[deployFilePath].Count == 0)
            {
                ctx.ChangeLines.Remove(deployFilePath);
            }
        }
        return Task.FromResult(ctx);
    }
    
    private int FindClosest(int[] arr, int target)
    {
        var left = 0;
        var right = arr.Length - 1;

        while (left < right)
        {
            var mid = left + (right - left) / 2;

            if (arr[mid] == target)
                return arr[mid];

            if (arr[mid] < target)
                left = mid + 1;
            else
                right = mid;
        }

        // Compare the two closest values
        if (left == 0)
            return arr[0];
    
        if (left == arr.Length)
            return arr[^1];

        return (Math.Abs(arr[left] - target) < Math.Abs(arr[left - 1] - target))
            ? arr[left]
            : arr[left - 1];
    }
}