using System.Reflection;
using CoverageIncr.Shared;
using CoverageIncr.Shared.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace CoverageIncr.Receivers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegistReceivers(this IServiceCollection services)
    {
        var types = from implType in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
                    let attr = implType.GetCustomAttribute<ReceiverAttribute>()
                        where attr != null
                        select (implType, attr);

        foreach (var type in types)
        {
            ComponentsGallery.RegisterReceiver(type.attr.Name, type.implType, type.attr.OptionType);
        }
        
        return services;
    }
}
