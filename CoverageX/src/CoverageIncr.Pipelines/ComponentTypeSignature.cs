namespace CoverageIncr.Pipelines;

/// <summary>
/// 组件类型签名
/// </summary>
/// <param name="InputType"></param>
/// <param name="OutputType"></param>
/// <param name="IsBatch"></param>
public record ComponentTypeSignature(Type InputType, Type OutputType, bool IsBatch = false);