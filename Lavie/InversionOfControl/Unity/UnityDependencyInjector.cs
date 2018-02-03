using Lavie.Configuration;
using Lavie.Environment;
using Lavie.FilterProviders;
using Lavie.Infrastructure;
using Lavie.Infrastructure.InversionOfControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
using Lavie.Utilities.Exceptions;
using Lavie.BootStrapperTask;
using System.Web;
using Lavie.Infrastructure.Modules;
using Unity.Lifetime;
using Lavie.Models;
using Microsoft.Practices.Unity.Configuration;

namespace Lavie.InversionOfControl.Unity
{
    public class UnityDependencyInjector : IDependencyInjector
    {
        private readonly IUnityContainer _innerContainer;

        public UnityDependencyInjector()
        {
            this._innerContainer = CreateContainer();
            // 将自身注册为IDependencyInjector和IDependencyResolver的单例
            this._innerContainer.RegisterInstance<IDependencyInjector>(this);
            this._innerContainer.RegisterInstance<IDependencyResolver>(this);
        }

        #region IDependencyInjector 成员

        /// <summary>
        /// 配置依赖注入容器
        /// </summary>
        /// <remarks>
        /// <para>将相关对象注册为单例，并将接口(或父类)与类的映射注册到容器中。</para>
        /// <para>备注：该方法为virtual，意味着可以继承UnityDependencyInjector类进行重写</para>
        /// </remarks>
        /// <returns>返回IUnityContainer接口对象</returns>
        public virtual IUnityContainer CreateContainer()
        {
            // 创建(父)容器，将相对固定的映射放入该容器
            IUnityContainer parentContainer = new UnityContainer();
            // 创建子容器，作用：
            // 注册unity.config中的配置的映射（子容器的配置优先于其父容器的配置）；
            IUnityContainer childContainer = parentContainer.CreateChildContainer();

            // 注册已有对象实例为单例
            parentContainer
                // XoohooConfigurationSection为自定义配置节点，用于配置Module信息
                .RegisterInstance((LavieConfigurationSection)ConfigurationManager.GetSection("lavie"))
                // AppSettingsHelper对web.config的appSettings节点进行封装
                .RegisterInstance(new AppSettingsHelper(ConfigurationManager.AppSettings))
                // IDependencyResolver
                //.RegisterInstance(DependencyResolver.Current)
                // RouteCollection，全局的静态的路由表
                .RegisterInstance(RouteTable.Routes)
                .RegisterInstance(BundleTable.Bundles)
                // ModelBinderDictionary，通过请求数据(QueryString、Form、RouteValue等)生成实体
                .RegisterInstance(System.Web.Mvc.ModelBinders.Binders)
                // ViewEngineCollection，视图引擎集合
                .RegisterInstance(System.Web.Mvc.ViewEngines.Engines)
                // GlobalFilterCollection，全局Filter提供器
                .RegisterInstance(GlobalFilters.Filters)
                // FilterRegistryFilterProvider, 基于注册的Filter提供器
                .RegisterInstance(FilterRegistry.Filters)
                // FilterProviderCollection 拦截器提供器集合
                .RegisterInstance(System.Web.Mvc.FilterProviders.Providers);

            parentContainer
                .RegisterType<ILavieHost, DefaultLavieHost>();

            // ModulesLoaded 相当于一个容器类，用于加载指定模块(Module)，并用一个集合保存之
            parentContainer
                .RegisterInstance<IModuleRegistry>(new ModuleRegistry(this));

            parentContainer
                .RegisterInstance<IBackgroundServiceRegistry>(new BackgroundServiceRegistry(this));

            // LoadModules引导程序，根据Xoohoo.config中modules节点的配置，对模块进行加载
            // LoadBackgroundServices引导程序，根据Xoohoo.config中backgroundServices节点的配置,对后台服务进行加载
            parentContainer
                .RegisterInstance<IBootStrapperTask>("LoadModules", new LoadModules(this))
                .RegisterInstance<IBootStrapperTask>("LoadBackgroundServices", new LoadBackgroundServices(this));

            // Web.config中的数据库链接字符串
            // 注意，默认情况下应该有一个名称为ApplicationServices的数据库链接字符串
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
                parentContainer.RegisterInstance(connectionString.Name, connectionString.ConnectionString);

            // Xoohoo.config中的数据库链接字符串
            foreach (ConnectionStringSettings connectionString in parentContainer.Resolve<LavieConfigurationSection>().ConnectionStrings)
                parentContainer.RegisterInstance(connectionString.Name, connectionString.ConnectionString);

            // IUser、RequestContext和RouteTable.Routes（另外还有个RequestDataFormat枚举对象）将会组合成XoohooContext对象
            parentContainer
                //.RegisterType<IUserExtendedPropertyService, UserExtendedPropertyService>()
                // 延迟获取数据
                // Application_AcquireRequestState方法中将以下4个对象存入当前请求的HttpContextState中
                // 自定义IUser等的生命周期。当需要注入对象时，首先尝试从当前HttpContextState中获取，如果为null则从其他途径获取
                .RegisterType<IUser>(new FactoryMethodLifetimeManager(() => HttpContext.Current.Items[typeof(IUser).FullName] as IUser ?? new UserAnonymous()))
                .RegisterType<IModule>(new FactoryMethodLifetimeManager(() => HttpContext.Current.Items[typeof(IModule).FullName] as IModule ?? null))
                .RegisterType<IAuthenticationModule>(new FactoryMethodLifetimeManager(() => HttpContext.Current.Items[typeof(IAuthenticationModule).FullName] as IAuthenticationModule ?? null))
                .RegisterType<RequestContext>(new FactoryMethodLifetimeManager(() => HttpContext.Current.Items[typeof(RequestContext).FullName] as RequestContext ?? new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData())));

            //分页样式
            parentContainer
                .RegisterType<IPageSkin, PageSkin>();

            // 根据web.config中的unity配置节点对子容器进行配置。
            // web.config中的unity配置节点的配置（即位于子容器中的配置），优先于父容器的配置。
            var config = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            if (config.Containers.Default != null)
            {
                config.Configure(childContainer);
            }
            // *将子容器自身注册为单例*
            //childContainer.RegisterInstance(childContainer);

            // 返回子容器
            return childContainer;
        }

        public IDependencyInjector RegisterInstance<T>(T instance) where T : class
        {
            Guard.ArgumentNotNull(instance, "instance");

            _innerContainer.RegisterInstance(instance);
            return this;
        }

        public IDependencyInjector RegisterInstance<TFrom, TTo>() where TTo : class, TFrom
        {
            return RegisterInstance(typeof(TFrom), typeof(TTo));
        }

        public IDependencyInjector RegisterInstance(Type from, Type to)
        {
            Guard.ArgumentNotNull(from, "from");
            Guard.ArgumentNotNull(to, "to");

            _innerContainer.RegisterType(from, to, new ContainerControlledLifetimeManager());
            return this;
        }

        public IDependencyInjector RegisterType<TFrom, TTo>() where TTo : class, TFrom
        {
            _innerContainer.RegisterType<TFrom, TTo>();
            return this;
        }

        public IDependencyInjector RegisterType(Type from, Type to)
        {
            Guard.ArgumentNotNull(from, "from");
            Guard.ArgumentNotNull(to, "to");

            _innerContainer.RegisterType(from, to);
            return this;
        }

        /// <summary>
        /// 注入已存在的对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="existing"></param>
        /// <returns></returns>
        public T Inject<T>(T existing)
        {
            Guard.ArgumentNotNull(existing, "existing");

            return _innerContainer.BuildUp(existing);
        }

        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof(T));
        }

        public bool IsRegistered(Type type)
        {
            return _innerContainer.IsRegistered(type);
        }

        /// <summary>
        /// Determines whether this type can be resolved as the default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///     <c>true</c> if this instance can resolve; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve<T>()
        {
            return CanResolve(typeof(T));
        }

        /// <summary>
        /// Determines whether this type can be resolved as the default.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <c>true</c> if this instance can resolve; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve(Type type)
        {
            if (isResolvableClass(type))
                return true;
            return IsRegistered(type);
        }

        private bool isResolvableClass(Type type)
        {
            return type.IsClass && !type.IsAbstract;
        }

        #endregion

        #region IDependencyResolver 成员

        public object GetService(Type serviceType)
        {
            Guard.ArgumentNotNull(serviceType, "serviceType");

            return CanResolve(serviceType) ? _innerContainer.Resolve(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            Guard.ArgumentNotNull(serviceType, "serviceType");

            //return _container.IsRegistered(serviceType) ? _container.ResolveAll(serviceType) : Enumerable.Empty<object>();
            return _innerContainer.ResolveAll(serviceType) ?? Enumerable.Empty<object>();
        }

        #endregion

    }

}
