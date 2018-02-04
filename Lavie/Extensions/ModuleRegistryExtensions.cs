using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lavie.FilterProviders;
using Lavie.Infrastructure;
using System.Web.Routing;
using Lavie.Routing;

namespace Lavie.Extensions
{
    public static class ModuleRegistryExtensions
    {
        /// <summary>
        /// 注册模块的路由配置
        /// <remarks>
        /// 后加入的模块的路由配置将排在前面，目的是提供一种"覆盖"机制
        /// </remarks>
        /// </summary>
        /// <param name="moduleRegistry"></param>
        public static void RegisterRoutes(this IModuleRegistry moduleRegistry)
        {
            moduleRegistry.GetModules().Reverse().ForEach(m => m.RegisterRoutes());
        }
        public static void RegisterFilters(this IModuleRegistry moduleRegistry, FilterRegistryFilterProvider filterRegistry, GlobalFilterCollection globalFilters)
        {
            moduleRegistry.GetModules().ForEach(m => m.RegisterFilters(filterRegistry, globalFilters));
        }
        public static IModule GetModule(this IModuleRegistry moduleRegistry, string moduleName)
        {
            if (moduleRegistry == null || moduleName.IsNullOrWhiteSpace())
                return null;

            //当前模块
            return moduleRegistry
                .GetModules<IModule>().FirstOrDefault(m => m.ModuleName == moduleName);
        }
        /// <summary>
        /// 获取当前模块所属于的认证模块
        /// </summary>
        /// <param name="moduleRegistry"></param>
        /// <param name="currentModuleName"></param>
        /// <returns></returns>
        public static IAuthenticationModule GetAuthenticationModule(this IModuleRegistry moduleRegistry, string currentModuleName)
        {
            //当前模块
            IModule currentModule = moduleRegistry.GetModule(currentModuleName);
            if (currentModule == null) return null;

            //当前模块的认证模块
            string authenticationModuleName = currentModule.Settings.GetString("AuthenticationModuleName");
            return !authenticationModuleName.IsNullOrWhiteSpace() ? moduleRegistry
                .GetModules<IAuthenticationModule>()
                .FirstOrDefault(m => m.ModuleName == authenticationModuleName) : null;
        }
    }
}
