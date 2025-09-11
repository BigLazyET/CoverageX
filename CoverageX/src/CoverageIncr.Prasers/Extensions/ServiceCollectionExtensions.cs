using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CoverageIncr.Prasers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoverageSourceFromAssembly(
        this IServiceCollection services,
        string assemblyName,
        CoverageRegistrationOptions? options = null)
    {
        var asm = Assembly.Load(assemblyName);
        return services.ScanAndRegisterCoverageTypes(asm, options ?? new CoverageRegistrationOptions());
    }

    public static IServiceCollection AddCoverageSourceFromEntryAssembly(
        this IServiceCollection services,
        CoverageRegistrationOptions? options = null)
    {
        var asm = Assembly.GetEntryAssembly();
        if (asm == null) throw new InvalidOperationException("Entry Assembly not found");
        return services.ScanAndRegisterCoverageTypes(asm, options ?? new CoverageRegistrationOptions());
    }

    public static IServiceCollection AddCoverageSourcesByConvention(
        this IServiceCollection services,
        CoverageRegistrationOptions? options = null)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.Contains("CoveragePlugins"));
        foreach (var asm in assemblies)
        {
            services.ScanAndRegisterCoverageTypes(asm, options ?? new CoverageRegistrationOptions());
        }

        return services;
    }

    public static IServiceCollection AddCoverageSource<TDetector, TParser>(
        this IServiceCollection services,
        CoverageRegistrationOptions? options = null)
        where TDetector : class, ICoverageFormatDetector
        where TParser : class, ICoverageParser
    {
        options ??= new CoverageRegistrationOptions();
        var detector = Activator.CreateInstance<TDetector>();
        var parser = Activator.CreateInstance<TParser>();

        if (options.DisabledFormats.Contains(detector.Format) ||
            options.DisabledFormats.Contains(parser.Format))
        {
            Console.WriteLine($"[CoverageReg] Skipped {detector.Format} (disabled)");
            return services;
        }

        services.AddSingleton<ICoverageFormatDetector, TDetector>();
        services.AddSingleton<ICoverageParser, TParser>();
        Console.WriteLine($"[CoverageReg] Registered {typeof(TDetector).Name} + {typeof(TParser).Name}");
        return services;
    }

    private static IServiceCollection ScanAndRegisterCoverageTypes(
        this IServiceCollection services,
        Assembly asm,
        CoverageRegistrationOptions options)
    {
        var detectors = asm.GetTypes()
            .Where(t => typeof(ICoverageFormatDetector).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        foreach (var t in detectors)
        {
            var temp = (ICoverageFormatDetector)Activator.CreateInstance(t)!;
            if (options.DisabledFormats.Contains(temp.Format))
            {
                Console.WriteLine($"[CoverageReg] Skipped Detector {t.Name} (disabled)");
                continue;
            }

            if (!options.OverrideIfExists && services.Any(sd => sd.ServiceType == typeof(ICoverageFormatDetector)))
            {
                Console.WriteLine($"[CoverageReg] Skip Detector {t.Name} (exists)");
                continue;
            }

            services.AddSingleton(typeof(ICoverageFormatDetector), t);
            Console.WriteLine($"[CoverageReg] Registered Detector: {t.Name}");
        }

        var parsers = asm.GetTypes()
            .Where(t => typeof(ICoverageParser).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        foreach (var t in parsers)
        {
            var temp = (ICoverageParser)Activator.CreateInstance(t)!;
            if (options.DisabledFormats.Contains(temp.Format))
            {
                Console.WriteLine($"[CoverageReg] Skipped Parser {t.Name} (disabled)");
                continue;
            }

            if (!options.OverrideIfExists && services.Any(sd => sd.ServiceType == typeof(ICoverageParser)))
            {
                Console.WriteLine($"[CoverageReg] Skip Parser {t.Name} (exists)");
                continue;
            }

            services.AddSingleton(typeof(ICoverageParser), t);
            Console.WriteLine($"[CoverageReg] Registered Parser: {t.Name}");
        }

        return services;
    }
}