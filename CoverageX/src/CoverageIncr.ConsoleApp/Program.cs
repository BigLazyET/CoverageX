using CoverageIncr.Configurations.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CoverageIncr.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var services = new ServiceCollection();

        // 全局配置load
        
        
        // 根据全局配置按需注册Receivers, Processors, Exporters
    }
}