namespace CoverageIncr.Shared;

public static class ComponentsGallery
{
    public record ComponentInfo(Type ImplType, Type OptionsType, Type outType);

    private static readonly Dictionary<string, ComponentInfo> _receivers = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, ComponentInfo> _processors = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, ComponentInfo> _exporters = new(StringComparer.OrdinalIgnoreCase);
    
    public static void RegisterReceiver(string name, Type implType, Type optionsType, Type outType)
        => _receivers[name] = new(implType, optionsType, outType);

    public static bool TryGetReceiver(string name, out ComponentInfo info) => _receivers.TryGetValue(name, out info);
    public static bool TryGetProcessor(string name, out ComponentInfo info) => _processors.TryGetValue(name, out info);
    public static bool TryGetExporter(string name, out ComponentInfo info) => _exporters.TryGetValue(name, out info);
}