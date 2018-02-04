using System.Collections.Generic;
using System.Web.Mvc;
using Lavie.Infrastructure;
using Lavie.Infrastructure.InversionOfControl;
using Lavie.Configuration;
using System;

namespace Lavie.BootStrapperTasks
{
    /// <summary>
    /// �����������ڼ��غ�̨����
    /// </summary>
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
        /// ִ����������
        /// </summary>
        public void Execute()
        {
            IBackgroundServiceRegistry backgroundServicesRegistry = this._dependencyResolver.GetService<IBackgroundServiceRegistry>();
            LavieConfigurationSection moduleConfig = this._dependencyResolver.GetService<LavieConfigurationSection>();

            foreach (LavieBackgroundServiceConfigurationElement backgroundService in moduleConfig.BackgroundServices)
            {
                backgroundServicesRegistry.Add(moduleConfig,backgroundService);
            }
            foreach (IBackgroundServiceExecutor executor in backgroundServicesRegistry.GetBackgroundServices())
                executor.Start();

        }

        /// <summary>
        /// ������������
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