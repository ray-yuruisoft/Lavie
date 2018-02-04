using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using Lavie.Configuration;
using Lavie.Configuration.Extensions;
using Lavie.Extensions;
using Lavie.Infrastructure.InversionOfControl;
using System.Web.Mvc;
using Lavie.Utilities.Exceptions;

namespace Lavie.Infrastructure
{
    public class BackgroundServiceRegistry : IBackgroundServiceRegistry
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly List<IBackgroundServiceExecutor> _executors;

        public BackgroundServiceRegistry(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
            _executors = new List<IBackgroundServiceExecutor>();
        }

        #region IBackgroundServiceRegistry Members

        public void Clear()
        {
            _executors.Clear();
        }

        public void Add(LavieConfigurationSection config, LavieBackgroundServiceConfigurationElement backgroundService)
        {
            Guard.ArgumentNotNull(config, "config");
            Guard.ArgumentNotNull(backgroundService, "backgroundService");

            if (!backgroundService.Enabled) return;

            Type serviceType = GetType(backgroundService.Type);
            var service = _dependencyResolver.GetService(serviceType) as IBackgroundService;
            if (service == null)
                throw new NullReferenceException("{0} does not implement the IBackgroundService interface.".FormatWith(serviceType));

            service.ServiceName = backgroundService.Name;
            service.Settings = backgroundService.Settings.ToAppSettingsHelper();
            service.Interval = TimeSpan.FromMilliseconds(backgroundService.Interval);

            Type executorType = GetType(backgroundService.ExecutorType);
            var executor = (IBackgroundServiceExecutor)Activator.CreateInstance(executorType, _dependencyResolver, service);
            if (executor == null)
                throw new NullReferenceException("{0} does not implement the IBackgroundServiceExecutor interface.".FormatWith(executorType));

            _executors.Add(executor);
        }

        public IEnumerable<IBackgroundServiceExecutor> GetBackgroundServices()
        {
            return _executors;
        }

        #endregion

        private Type GetType(string type)
        {
            Type instance = Type.GetType(type);

            // 如果在引用程序集中无法获取类型，则尝试从App_Code目录生成的程序集中获取
            if (instance == null && BuildManager.CodeAssemblies != null)
            {
                foreach (var assembly in BuildManager.CodeAssemblies.OfType<Assembly>())
                {
                    instance = assembly.GetExportedTypes().FirstOrDefault(t => t.FullName == type);

                    if (instance != null) break;
                }
                // 如果从以上两种途径都无法获取类型，抛出异常
                if (instance == null)
                    throw new TypeLoadException("Could not load type '{0}'.".FormatWith(type));
            }
            return instance;

        }
    }
}
