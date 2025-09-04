namespace CoverageIncr.Configurations;

using System.Text.Json;

public static class YamlDynamicBinder
{
    // public static T Bind<T>(object yamlSection)
    // {
    //     if (yamlSection == null)
    //         throw new ArgumentNullException(nameof(yamlSection));
    //
    //     // 先把 Dictionary<object, object> 转成 Dictionary<string, object>
    //     var normalized = NormalizeObject(yamlSection);
    //
    //     // 再转成 JSON 字符串
    //     var json = JsonSerializer.Serialize(normalized, new JsonSerializerOptions
    //     {
    //         WriteIndented = false,
    //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //     });
    //
    //     // 反序列化成目标类型
    //     return JsonSerializer.Deserialize<T>(json)
    //            ?? throw new InvalidOperationException($"无法将 YAML 绑定到类型 {typeof(T).Name}");
    // }
    
    /// <summary>
    /// 将从YamlDotNet反序列化得到的动态字典绑定到强类型对象
    /// </summary>
    public static object? Bind(object yamlSection, Type returnType)
    {
        if (yamlSection == null)
            throw new ArgumentNullException(nameof(yamlSection));

        // 先把 Dictionary<object, object> 转成 Dictionary<string, object>
        var normalized = NormalizeObject(yamlSection);

        // 再转成 JSON 字符串
        var json = JsonSerializer.Serialize(normalized, new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // 反序列化成目标类型
        return JsonSerializer.Deserialize(json, returnType)
               ?? throw new InvalidOperationException($"无法将 YAML 绑定到类型 {returnType.Name}");
    }

    /// <summary>
    /// 递归将 Dictionary<object, object> 全转成 Dictionary<string, object>
    /// </summary>
    private static object? NormalizeObject(object? input)
    {
        if (input is IDictionary<object, object> dictObj)
        {
            var newDict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in dictObj)
            {
                var key = kv.Key?.ToString() ?? string.Empty;
                newDict[key] = NormalizeObject(kv.Value);
            }
            return newDict;
        }
        else if (input is IDictionary<string, object> dictStr)
        {
            var newDict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in dictStr)
            {
                newDict[kv.Key] = NormalizeObject(kv.Value);
            }
            return newDict;
        }
        else if (input is IList<object> list)
        {
            return list.Select(NormalizeObject).ToList();
        }
        else
        {
            // 基础类型直接返回
            return input;
        }
    }
}