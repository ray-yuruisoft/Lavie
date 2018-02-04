using System.Collections.Generic;
using Lavie.Configuration;

namespace Lavie.Infrastructure
{
    public interface IModuleRegistry
    {
        IModule Load(LavieConfigurationSection config, LavieModuleConfigurationElement module);
        IEnumerable<IModule> GetModules();
        IEnumerable<T> GetModules<T>() where T : IModule;
        void UnloadModules();
    }
}
