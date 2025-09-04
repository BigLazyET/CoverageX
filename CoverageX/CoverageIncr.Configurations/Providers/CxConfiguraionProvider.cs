using CoverageIncr.Configurations.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace CoverageIncr.Configurations.Providers;

public abstract class CxConfiguraionProvider : ICxConfigurationProvider, IDisposable
{
    private ConfigurationReloadToken _reloadToken = new ();
    protected CxTakenConfiguration Data { get; set; }
    
    public IChangeToken GetReloadToken() => _reloadToken;

    public abstract object? GetReceiver(string receiverName, Type returnType);

    public abstract void Load();

    protected void OnReload()
    {
        Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken()).OnReload();
    }

    public virtual void Dispose()
    {
        
    }
}