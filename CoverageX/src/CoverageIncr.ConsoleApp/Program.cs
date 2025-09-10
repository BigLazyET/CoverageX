using CoverageIncr.Configurations.Extensions;
using CoverageIncr.Pipelines.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CoverageIncr.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var services = new ServiceCollection();

        // 全局配置load
        services.AddCxConfiguration(builder => builder.ConfigureYml(source =>
        {
            source.FilePath = "";
            source.ReloadOnChange = true;
        }));

        // 根据全局配置按需注册Receivers, Processors, Exporters
        services.AddCxPipelines();
    }
}