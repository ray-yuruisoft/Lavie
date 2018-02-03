using Lavie.Configuration;
using Lavie.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lavie.BootStrapperTask
{
    public class LoadBackgroundServices : IBootStrapperTask
    {
        private readonly IDependencyResolver _dependencyResolver;

        public LoadBackgroundServices()
            : this(DependencyResolver.Current)
        {
        }

        public LoadBackgroundServices(IDependencyResolver dependencyResolver)
        {
            this._dependencyResolver = dependencyResolver;
        }

        #region IBootStrapperTask Members

        /// <summary>
        /// 执行引导程序
        /// </summary>
        public void Execute()
        {
            IBackgroundServiceRegistry backgroundServicesRegistry = this._dependencyResolver.GetService<IBackgroundServiceRegistry>();
            LavieConfigurationSection moduleConfig = this._dependencyResolver.GetService<LavieConfigurationSection>();

            foreach (LavieBackgroundServiceConfigurationElement backgroundService in moduleConfig.BackgroundServices)
            {
                backgroundServicesRegistry.Add(moduleConfig, backgroundService);
            }
            foreach (IBackgroundServiceExecutor executor in backgroundServicesRegistry.GetBackgroundServices())
                executor.Start();

        }

        /// <summary>
        /// 引导程序清理
        /// </summary>
        public void Cleanup()
        {
            IBackgroundServiceRegistry backgroundServicesRegistry = this._dependencyResolver.GetService<IBackgroundServiceRegistry>();

            foreach (IBackgroundServiceExecutor executor in backgroundServicesRegistry.GetBackgroundServices())
                executor.Stop();
        }

        #endregion
    }
}
