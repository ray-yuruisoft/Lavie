using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Optimization;
using Lavie.FilterProviders;
using System.Collections.Specialized;
using System.Web.Routing;
using System.Web.Http;

namespace Lavie.Infrastructure
{
    public interface IModule
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        string ModuleName { get; }
        /// <summary>
        /// 模块设置
        /// </summary>
        AppSettingsHelper Settings { get; set; }
        /// <summary>
        /// 初始化模块
        /// </summary>
        void Initialize();
        void WebApiConfigRegister(HttpConfiguration config);
        /// <summary>
        /// 注册路由配置
        /// </summary>
        void RegisterRoutes();
        /// <summary>
        /// 注册资源包
        /// </summary>
        void RegisterBundles(BundleCollection bundles);
        /// <summary>
        /// 注册过滤器
        /// </summary>
        /// <param name="filterRegistry"></param>
        /// <param name="globalFilters"></param>
        void RegisterFilters(FilterRegistryFilterProvider filterRegistry, GlobalFilterCollection globalFilters);
        /// <summary>
        /// 注册模型绑定器
        /// </summary>
        /// <param name="modelBinders"></param>
        void RegisterModelBinders(ModelBinderDictionary modelBinders);
        /// <summary>
        /// 卸载模块
        /// </summary>
        void Unload();
    }
}
