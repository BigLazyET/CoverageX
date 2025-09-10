using System.Reflection;
using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace CoverageIncr.Processors.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegistProcessors(this IServiceCollection services)
    {
        services.AddSingleton<IMethodChangeService, MethodChangeService>();
        
        var types = from implType in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
            let attr = implType.GetCustomAttribute<ProcessorAttribute>()
            where attr != null
            select (implType, attr);

        foreach (var type in types)
        {
            ComponentsGallery.RegisterProcessor(type.attr.Name, type.implType, type.attr.OptionType);
        }
        
        return services;
    }
}