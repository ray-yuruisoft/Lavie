using System.Collections.Generic;
using Xoohoo.Configuration;

namespace Xoohoo.Infrastructure
{
    public interface IModulesLoaded
    {
        IModule Load(XoohooConfigurationSection config, XoohooModuleConfigurationElement module);
        IEnumerable<IModule> GetModules();
        IEnumerable<T> GetModules<T>() where T : IModule;
        void UnloadModules();
    }
}
