using CoverageIncr.Shared;
using LibGit2Sharp;

namespace CoverageIncr.Processors;

public interface IMethodChangeService
{
    ReportGeneratorFilter GetDiffMethodDescriptions(Blob targetBlob, Blob sourceBlob, string sourceFilePath);

    HashSet<int> GetMethodLineNumbers(Blob blob);
}