using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Lavie.Extensions;
using Lavie.FilterProviders;
using Lavie.Infrastructure.InversionOfControl;
using Lavie.Utilities.Exceptions;

namespace Lavie.Infrastructure
{
    public abstract class Module : IModule
    {
        private readonly RouteCollection _routes;
        protected readonly IDependencyInjector DependencyInjector;

        protected Module(IDependencyInjector dependencyInjector)
        {
            Guard.ArgumentNotNull(dependencyInjector, "dependencyInjector");

            this.DependencyInjector = dependencyInjector;
            this._routes = DependencyInjector.GetService<RouteCollection>();
        }

        protected Module()
            : this(DependencyResolver.Current.GetService<IDependencyInjector>())
        {
        }

        #region MapRoute

        protected void MapRoute(string name, string url)
        {
            MapRoute(name, url, null, null, null);
        }
        protected void MapRoute(string name, string url, object defaults)
        {
            MapRoute(name, url, defaults, null, null);
        }
        protected void MapRoute(string name, string url, string[] namespaces)
        {
            MapRoute(name, url, null, null, namespaces);
        }
        protected void MapRoute(string name, string url, object defaults, object constraints)
        {
            MapRoute(name, url, defaults, constraints, null);
        }
        protected void MapRoute(string name, string url, object defaults, string[] namespaces)
        {
            MapRoute(name, url, defaults, null, namespaces);
        }
        protected void MapRoute(string name, string url, object defaults, object constraints, string[] namespaces)
        {
            _routes.MapRoute(name, url, defaults, constraints, namespaces, Settings.GetString("UrlPrefix", null), Settings.GetString("SubDomain", null), ModuleName);
        }
        protected void MapDomainRoute(string name, string url, object defaults, object constraints, string[] namespaces)
        {
            _routes.MapRoute(name, url, defaults, constraints, namespaces, null, Settings.GetString("SubDomain", null), ModuleName);
        }
        protected void MapPrefixRoute(string name, string url, object defaults, object constraints, string[] namespaces)
        {
            _routes.MapRoute(name, url, defaults, constraints, namespaces, Settings.GetString("UrlPrefix", null), null, ModuleName);
        }
        // 忽略Url前缀设置和域名设置
        protected void MapRawRoute(string name, string url, object defaults, object constraints, string[] namespaces)
        {
            _routes.MapRoute(name, url, defaults, constraints, namespaces, null, null, ModuleName);
        }

        #endregion 

        /// <summary>
        /// 模块名称
        /// </summary>
        public abstract string ModuleName { get; }
        /// <summary>
        /// 模块设置
        /// </summary>
        public AppSettingsHelper Settings { get; set; }
        /// <summary>
        /// 初始化模块
        /// </summary>
        public virtual void Initialize() { }
        public virtual void WebApiConfigRegister(HttpConfiguration config) { }

        /// <summary>
        /// 注册路由配置
        /// </summary>
        public virtual void RegisterRoutes() { }
        /// <summary>
        /// 注册资源包
        /// </summary>
        public virtual void RegisterBundles(BundleCollection bundles)
        {
        }

        /// <summary>
        /// 注册过滤器
        /// </summary>
        /// <param name="filterRegistry"></param>
        /// <param name="globalFilters"></param>
        public virtual void RegisterFilters(FilterRegistryFilterProvider filterRegistry, GlobalFilterCollection globalFilters) { }
        /// <summary>
        /// 注册模型绑定器
        /// </summary>
        /// <param name="modelBinders"></param>
        public virtual void RegisterModelBinders(ModelBinderDictionary modelBinders) { }
        /// <summary>
        /// 卸载模块
        /// </summary>
        public virtual void Unload() { }
    }
}
