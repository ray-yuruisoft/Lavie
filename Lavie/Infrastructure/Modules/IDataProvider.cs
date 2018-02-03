using Lavie.Configuration;
using Lavie.Infrastructure.InversionOfControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Infrastructure
{
    public interface IDataProvider
    {
        void ConfigureProvider(LavieConfigurationSection config, LavieDataProviderConfigurationElement dataProviderConfig, IDependencyInjector dependencyInjector);
    }
}
