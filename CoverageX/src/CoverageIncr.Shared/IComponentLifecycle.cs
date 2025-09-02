namespace CoverageIncr.Shared;

public interface IComponentLifecycle
{
    Task StartAsync();
    Task StopAsync();
}