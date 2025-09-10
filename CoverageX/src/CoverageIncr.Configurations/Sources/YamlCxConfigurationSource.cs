using CoverageIncr.Configurations.Interfaces;
using CoverageIncr.Configurations.Providers;

namespace CoverageIncr.Configurations.Sources;

public class YamlCxConfigurationSource : ICxConfigurationSource
{
    public string FilePath { get; set; }

    public bool ReloadOnChange { get; set; }
    
    public ICxConfigurationProvider Build(ICxConfigurationBuilder builder)
    {
        return new YamlCxConfigurationProvider(this);
    }
}