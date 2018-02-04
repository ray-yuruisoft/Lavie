using System;
using System.Collections.Generic;
using Lavie.Configuration;

namespace Lavie.Infrastructure
{
    public interface IBackgroundServiceRegistry
    {
        void Clear();
        void Add(LavieConfigurationSection config, LavieBackgroundServiceConfigurationElement backgroundService);
       
        IEnumerable<IBackgroundServiceExecutor> GetBackgroundServices();
    }
}