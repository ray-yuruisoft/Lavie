﻿using Lavie.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Infrastructure
{
    public interface IBackgroundServiceRegistry
    {
        void Clear();
        void Add(LavieConfigurationSection config, LavieBackgroundServiceConfigurationElement backgroundService);

        IEnumerable<IBackgroundServiceExecutor> GetBackgroundServices();
    }
}
