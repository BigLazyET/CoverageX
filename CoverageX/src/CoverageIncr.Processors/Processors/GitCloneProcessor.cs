using CoverageIncr.Processors.Options;
using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;
using LibGit2Sharp;

namespace CoverageIncr.Processors.Processors;

[Processor(Name = "gitClone", OptionType = typeof(GitCloneProcOption))]
public class GitCloneProcessor(GitCloneProcOption option) : ProcessorBase<GitCloneProcOption>(option)
{
    public override async Task<PipelineContext> ProcessAsync(PipelineContext ctx)
    {
        await CloneAsync();
        return ctx;
    }
    
    private async Task CloneAsync()
    {
        var deployDir = new DirectoryInfo(Option.DeployPath);
        if (deployDir.Exists)
            deployDir.Delete(true);

        var featureDir = new DirectoryInfo(Option.FeaturePath);
        if (featureDir.Exists)
            featureDir.Delete(true);

        var deployTask = Task.Run(() => 
            CloneBranch(Option.Repository, Option.DeployPath, Option.DeployBranch, Option.UserName, Option.Password));
        var featureTask = Task.Run(() => 
            CloneBranch(Option.Repository, Option.FeaturePath, Option.FeatureBranch, Option.UserName, Option.Password));

        await Task.WhenAll(deployTask, featureTask);
    }
    
    private static void CloneBranch(string repoUrl, string localPath, string branchName, string userName, string password)
    {
        var cloneOptions = new CloneOptions { BranchName = branchName };
        cloneOptions.FetchOptions.CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
        {
            Username = userName,
            Password = password
        };

        try
        {
            Repository.Clone(repoUrl, localPath, cloneOptions);
        }
        catch (Exception ex)
        {
            
        }
    }
}