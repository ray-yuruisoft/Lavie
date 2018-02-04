using Lavie.Configuration;
using Lavie.Infrastructure.InversionOfControl;

namespace Lavie.Infrastructure
{
    public interface IDataProvider
    {
        void ConfigureProvider(LavieConfigurationSection config, LavieDataProviderConfigurationElement dataProviderConfig, IDependencyInjector dependencyInjector);
    }
}