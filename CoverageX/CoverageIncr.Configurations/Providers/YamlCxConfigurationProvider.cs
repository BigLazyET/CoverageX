using CoverageIncr.Configurations.Sources;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CoverageIncr.Configurations.Providers;

public class YamlCxConfigurationProvider : CxConfiguraionProvider
{
    private readonly YamlCxConfigurationSource _source;
    
    public YamlCxConfigurationProvider(YamlCxConfigurationSource source)
    {
        _source = source;
        
        if (source.ReloadOnChange == false) return;
        var watcher = new FileSystemWatcher(Path.GetDirectoryName(source.FilePath)!)
        {
            Filter = Path.GetFileName(source.FilePath),
            NotifyFilter = NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };
        watcher.Changed += WatcherOnChanged;
    }

    public override void Load()
    {
        Data = GenerateCxConfiguration();
    }

    private void WatcherOnChanged(object sender, FileSystemEventArgs e)
    {
        Data = GenerateCxConfiguration();
        OnReload();
    }
    
    private CxTakenConfiguration GenerateCxConfiguration()
    {
        var yaml = File.ReadAllText(_source.FilePath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<CxTakenConfiguration>(yaml);
    }

    public override object? GetReceiver(string receiverName, Type returnType)
    {
        if (!Data.Receivers.TryGetValue(receiverName, out var ymlSection))
            return default;
        return YamlDynamicBinder.Bind(ymlSection, returnType);
    }
}