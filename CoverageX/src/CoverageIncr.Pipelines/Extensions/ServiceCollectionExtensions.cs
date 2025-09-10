using CoverageIncr.Exporters.Extensions;
using CoverageIncr.Processors.Extensions;
using CoverageIncr.Receivers.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CoverageIncr.Pipelines.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCxPipelines(this IServiceCollection services)
    {
        services.RegistReceivers();
        services.RegistProcessors();
        services.RegistExporters();

        services.AddSingleton<IPipelineFactory, PipelineFactory>();
        services.AddSingleton<IPipelineExecutor, PipelineExecutor>();
        
        return services;
    }
}