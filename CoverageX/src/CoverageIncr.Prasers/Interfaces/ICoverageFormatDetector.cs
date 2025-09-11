namespace CoverageIncr.Prasers;

/// <summary>
/// 基础检测器接口
/// </summary>
public interface ICoverageFormatDetector
{
    CoverageFormat Format { get; }
    int DetectConfidence(string filePath);
}