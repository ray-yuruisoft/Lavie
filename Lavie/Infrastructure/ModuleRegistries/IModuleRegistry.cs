using Lavie.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
