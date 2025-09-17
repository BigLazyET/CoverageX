using CoverageIncr.Shared;

namespace CoverageIncr.Prasers;

/// <summary>
/// 基础解析器接口
/// </summary>
public interface ICoverageParser
{
    CoverageFormat Format { get; }
    IEnumerable<ParserResult> Parse(string filePath, IList<ReportGeneratorFilter> filters);
}