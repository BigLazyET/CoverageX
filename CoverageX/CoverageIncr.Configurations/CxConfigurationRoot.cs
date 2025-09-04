using CoverageIncr.Configurations.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace CoverageIncr.Configurations;

public class CxConfigurationRoot : ICxConfiguration, IDisposable
{
    private ConfigurationReloadToken _changeToken = new ();
    
    public ConfigurationReloadToken GReloadToken() => this._changeToken;
    
    private readonly IList<IDisposable> _changeTokenRegistrations;
    
    private IEnumerable<ICxConfigurationProvider> _providers;

    public CxConfigurationRoot(IEnumerable<ICxConfigurationProvider> providers)
    {
        _providers = providers;
        
        _changeTokenRegistrations = new List<IDisposable>(_providers.Count());
        foreach (var provider in providers)
        {
            _changeTokenRegistrations.Add(ChangeToken.OnChange(new Func<IChangeToken?>(provider.GetReloadToken), new Action(RaiseChanged)));
        }
    }

    public object? GetReceiver(string receiverName, Type returnType)
    {
        for (var i = _providers.Count() - 1; i >= 0; --i)
        {
            var provider = _providers.ElementAt(i);
             return provider.GetReceiver(receiverName, returnType);
        }

        return default;
    }
    
    private void RaiseChanged()
    {
        Interlocked.Exchange<ConfigurationReloadToken>(ref this._changeToken, new ConfigurationReloadToken()).OnReload();
    }
    
    public void Dispose()
    {
        foreach (IDisposable tokenRegistration in this._changeTokenRegistrations)
            tokenRegistration.Dispose();
        foreach (ICxConfigurationProvider provider in (IEnumerable<ICxConfigurationProvider>) this._providers)
        {
            if (provider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}