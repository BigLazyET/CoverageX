using CoverageIncr.Configurations.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CoverageIncr.Configurations;

public class CxConfiguraionBuilder : ICxConfigurationBuilder
{
    public IList<ICxConfigurationSource> Sources { get; }

    public CxConfigurationRoot Configuration { get; }

    public ICxConfigurationBuilder Add(ICxConfigurationSource source)
    {
        Sources.Add(source);
        return this;
    }
    
    public CxConfigurationRoot Build()
    {
        var providers = new List<ICxConfigurationProvider>();
        foreach (var source in Sources)
        {
            var provider = source.Build(this);
            providers.Add(provider);
        }
        return new CxConfigurationRoot(providers);
    }
}