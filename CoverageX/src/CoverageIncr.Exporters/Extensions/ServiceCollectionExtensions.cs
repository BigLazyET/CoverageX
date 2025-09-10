using System.Reflection;
using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace CoverageIncr.Exporters.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegistExporters(this IServiceCollection services)
    {
        var types = from implType in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
            let attr = implType.GetCustomAttribute<ExporterAttribute>()
            where attr != null
            select (implType, attr);

        foreach (var type in types)
        {
            ComponentsGallery.RegisterExporter(type.attr.Name, type.implType, type.attr.OptionType);
        }
        
        return services;
    }
}