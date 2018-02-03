using Lavie.FilterProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;

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
