namespace CoverageIncr.Configurations.Interfaces;

public interface ICxConfigurationSource
{
    ICxConfigurationProvider Build(ICxConfigurationBuilder builder);
}