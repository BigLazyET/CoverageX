namespace CoverageIncr.Configurations.Interfaces;

public interface ICxConfigurationBuilder
{
    IList<ICxConfigurationSource> Sources { get; }
    
    CxConfigurationRoot Build();
    
    ICxConfigurationBuilder Add(ICxConfigurationSource source);
}