namespace CoverageIncr.Prasers;

/// <summary>
/// 优先级接口（可选实现）
/// </summary>
public interface IPrioritizable
{
    int Priority { get; } // 越大优先
}