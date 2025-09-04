using CoverageIncr.Receivers.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CoverageIncr.Pipelines.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPipelines(this IServiceCollection services)
    {
        services.RegistReceivers();

        services.AddSingleton<ComponentsRegistry>();
        
        return services;
    }
}