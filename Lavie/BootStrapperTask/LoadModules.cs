using Lavie.Configuration;
using Lavie.Extensions;
using Lavie.FilterProviders;
using Lavie.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Lavie.BootStrapperTask
{
    public class LoadModules : IBootStrapperTask
    {
        private readonly IDependencyResolver _dependencyResolver;

        public LoadModules()
            : this(DependencyResolver.Current)
        {
        }
        public LoadModules(IDependencyResolver dependencyResolver)
        {
            this._dependencyResolver = dependencyResolver;
        }

        #region IBootStrapperTask Members

        /// <summary>
        /// 执行引导程序
        /// </summary>
        public void Execute()
        {
            //已经存在的单例
            IModuleRegistry moduleRegistry = this._dependencyResolver.GetService<IModuleRegistry>();
            ModelBinderDictionary modelBinders = this._dependencyResolver.GetService<ModelBinderDictionary>();
            LavieConfigurationSection moduleConfig = this._dependencyResolver.GetService<LavieConfigurationSection>();
            BundleCollection bundles = this._dependencyResolver.GetService<BundleCollection>();
            RouteCollection routes = this._dependencyResolver.GetService<RouteCollection>();

            routes.Clear();
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //已经存在的单例，ActionFilter注册表
            GlobalFilterCollection globalFilters = this._dependencyResolver.GetService<GlobalFilterCollection>();
            FilterRegistryFilterProvider filterRegistry = this._dependencyResolver.GetService<FilterRegistryFilterProvider>();
            globalFilters.Clear();
            filterRegistry.Clear();

            //遍历配置文件中的modules节点，获取根据module配置信息创建实例并执行相关方法
            foreach (LavieModuleConfigurationElement module in moduleConfig.Modules)
            {
                IModule moduleInstance = moduleRegistry.Load(moduleConfig, module);
                if (moduleInstance != null)
                {
                    moduleInstance.Initialize();
                    moduleInstance.RegisterModelBinders(modelBinders);
                    GlobalConfiguration.Configure(moduleInstance.WebApiConfigRegister);
                    moduleInstance.RegisterRoutes();
                    moduleInstance.RegisterBundles(bundles);
                }
            }

            //由于ILavieModule.RegisterFilters方法内部可能会用到注册的路由信息(如RouteFilterCriteria)，
            //所以该方法在路由注册(RegisterRoutes)之后调用
            moduleRegistry.RegisterFilters(filterRegistry, globalFilters);
        }

        /// <summary>
        /// 引导程序清理
        /// </summary>
        public void Cleanup()
        {
            _dependencyResolver.GetService<IModuleRegistry>().UnloadModules();
        }

        #endregion
    }
}
