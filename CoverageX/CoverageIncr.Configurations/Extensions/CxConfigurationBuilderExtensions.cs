using CoverageIncr.Configurations.Interfaces;
using CoverageIncr.Configurations.Providers;
using CoverageIncr.Configurations.Sources;

namespace CoverageIncr.Configurations.Extensions;

public static class CxConfigurationBuilderExtensions
{
    public static ICxConfigurationBuilder Addyaml(this ICxConfigurationBuilder builder, Action<YamlCxConfigurationSource> configure)
    {
        var source = new YamlCxConfigurationSource();
        configure?.Invoke(source);
        return builder.Add(source);
    }
}