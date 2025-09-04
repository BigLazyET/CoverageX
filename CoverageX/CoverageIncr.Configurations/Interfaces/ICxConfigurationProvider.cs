using Microsoft.Extensions.Primitives;

namespace CoverageIncr.Configurations.Interfaces;

public interface ICxConfigurationProvider
{
    IChangeToken GetReloadToken();

    object? GetReceiver(string receiverName, Type returnType);
    
    void Load();
}