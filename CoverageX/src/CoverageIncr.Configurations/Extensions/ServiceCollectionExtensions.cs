using CoverageIncr.Configurations.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CoverageIncr.Configurations.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCxConfiguration(this IServiceCollection services, Action<CxConfiguraionBuilder> configure)
    {
        var builder = new CxConfiguraionBuilder();
        configure?.Invoke(builder);

        var cxConfiguration = builder.Build();
        services.AddSingleton<ICxConfiguration>(cxConfiguration);

        return services;
    }
}