using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.Utility;
using Xoohoo.BootStrapperTasks;
using Xoohoo.Configuration;
using Xoohoo.FilterProviders;
using Xoohoo.Repositories;
using Xoohoo.Repositories.SqlServerEntLibDAB;
using Xoohoo.Services;
using XM = Xoohoo.Models;

namespace Xoohoo.Infrastructure
{
    public class UnityDependencyInjector : IDependencyInjector
    {
        private readonly IUnityContainer _innerContainer;

        public UnityDependencyInjector()
        {
            this._innerContainer = CreateContainer();
            // 将自身注册为单例
            this._innerContainer.RegisterInstance<IDependencyInjector>(this);
        }

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
                .RegisterInstance((XoohooConfigurationSection)ConfigurationManager.GetSection("xoohoo"))
                // AppSettingsHelper对web.config的appSettings节点进行封装
                .RegisterInstance(new AppSettingsHelper(ConfigurationManager.AppSettings))
                // IDependencyResolver
                //.RegisterInstance(DependencyResolver.Current)
                // RouteCollection，全局的静态的路由表
                .RegisterInstance(RouteTable.Routes)
                // ModelBinderDictionary，通过请求数据(QueryString、Form、RouteValue等)生成实体
                .RegisterInstance(System.Web.Mvc.ModelBinders.Binders)
                // ViewEngineCollection，视图引擎集合
                .RegisterInstance(System.Web.Mvc.ViewEngines.Engines)
                // GlobalFilterCollection，全局拦截器集合，本身也是拦截器提供器
                .RegisterInstance(GlobalFilters.Filters)
                // FilterProviderCollection 拦截器提供器集合
                .RegisterInstance(System.Web.Mvc.FilterProviders.Providers)
                // VirtualPathProvider 虚拟路径提供器
                .RegisterInstance(HostingEnvironment.VirtualPathProvider);

            // Web.config中的数据库链接字符串
            // 注意，默认情况下应该有一个名称为ApplicationServices的数据库链接字符串
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
                parentContainer.RegisterInstance(connectionString.Name, connectionString.ConnectionString);

            // Xoohoo.config中的数据库链接字符串
            foreach (ConnectionStringSettings connectionString in parentContainer.Resolve<XoohooConfigurationSection>().ConnectionStrings)
                parentContainer.RegisterInstance(connectionString.Name, connectionString.ConnectionString);

            // ModulesLoaded 相当于一个容器类，用于加载指定模块(Module)，并用一个集合保存之
            parentContainer
                .RegisterInstance<IModulesLoaded>(new ModulesLoaded(this));

            // FilterRegistryFilterProvider Filter注册表Filter提供器
            parentContainer
                .RegisterInstance(new FilterRegistryFilterProvider(this));

            // 日志
            parentContainer
                .RegisterInstance<ILog>(LogManager.GetLogger("File"));

            // LoadModules引导程序，根据Xoohoo.config中modules节点的配置，对模块进行注册
            // LoadBackgroundServices引导程序，加载后台服务
            parentContainer
                .RegisterInstance<IBootStrapperTask>("LoadModules", new LoadModules(this))
                .RegisterInstance<IBootStrapperTask>("LoadBackgroundServices", new LoadBackgroundServices(this));

            // RouteUrlModifier 用于给Route路径加前缀(目的是为了兼容默认配置下的IIS5.1以及IIS6)
            parentContainer
                .RegisterType<IRouteUrlModifier, RouteUrlModifier>();

            // Site、IUser、RequestContext和RouteTable.Routes（另外还有个RequestDataFormat枚举对象）将会组合成XoohooContext对象
            parentContainer
                // 系统(站点)配置信息处理逻辑
                .RegisterType<ISiteService, SiteService>()
                // 系统公告，用于系统后台的通知公告
                .RegisterType<IBulletinService, BulletinService>()
                // 用户信息、用户组、用户角色、权限信息的处理逻辑
                .RegisterType<IUserService, UserService>()
                .RegisterType<IUserGroupService, UserGroupService>()
                .RegisterType<IRoleService, RoleService>()
                .RegisterType<IPermissionService, PermissionService>()
                //.RegisterType<IUserExtendedPropertyService, UserExtendedPropertyService>()
                // 延迟获取数据
                // Application_AcquireRequestState方法中将以下三个对象存入当前请求的HttpContextState中
                // 自定义Site生命周期。当需要注入Site对象时，首先尝试从当前HttpContextState中获取，如果为null则从其他途径获取(缓存或数据库等)
                .RegisterType<XM.Site>(new FactoryMethodLifetimeManager(() => HttpContext.Current.Items[typeof(XM.Site).FullName] as XM.Site ?? childContainer.Resolve<ISiteService>().GetItem()))
                .RegisterType<IUser>(new FactoryMethodLifetimeManager(() => HttpContext.Current.Items[typeof(IUser).FullName] as IUser ?? new UserAnonymous()))
                .RegisterType<IAuthenticationModule>(new FactoryMethodLifetimeManager(() => HttpContext.Current.Items[typeof(IAuthenticationModule).FullName] as IAuthenticationModule ?? null))
                .RegisterType<RequestContext>(new FactoryMethodLifetimeManager(() => HttpContext.Current.Items[typeof(RequestContext).FullName] as RequestContext ?? new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData())));

            // 默认数据库访问方式，主要是为核心的几个Module提供各种需要的数据。如果要更改，在web.config中的unity节点中进行覆盖
            // 注意,如果使用系统默认的数据访问方式，name 为 ApplicationServices 的数据库连接字符串必须提供
            // （当然，如果外部unity配置了其他的数据访问方式，则无此限定）
            InjectionMember connectionStringName = new InjectionConstructor("ApplicationServices");
            parentContainer
                .RegisterType<ISiteRepository, SiteRepository>(connectionStringName)
                .RegisterType<IBulletinRepository, BulletinRepository>(connectionStringName)
                .RegisterType<IUserRepository, UserRepository>(connectionStringName)
                .RegisterType<IUserGroupRepository, UserGroupRepository>(connectionStringName)
                .RegisterType<IRoleRepository, RoleRepository>(connectionStringName)
                .RegisterType<IPermissionRepository, PermissionRepository>(connectionStringName);

            // 根据web.config中的unity配置节点对子容器进行配置。
            // web.config中的unity配置节点的配置（即位于子容器中的配置），优先于父容器的配置。
            UnityConfigurationSection config = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            if (config.Containers.Default != null)
            {
                config.Configure(childContainer);
            }
            // *将子容器自身注册为单例*
            //childContainer.RegisterInstance(childContainer);

            // 返回子容器
            return childContainer;
        }

        public object GetService(Type type)
        {
            Guard.ArgumentNotNull(type, "serviceType");

            return CanResolve(type) ? _innerContainer.Resolve(type) : null;
        }

        public IEnumerable<object> GetServices(Type type)
        {
            Guard.ArgumentNotNull(type, "serviceType");

            //return _container.IsRegistered(type) ? _container.ResolveAll(type) : Enumerable.Empty<object>();
            return _innerContainer.ResolveAll(type) ?? Enumerable.Empty<object>();
        }

        public IDependencyInjector RegisterInstance<T>(T instance)
        {
            Guard.ArgumentNotNull(instance, "instance");

            _innerContainer.RegisterInstance(instance);
            return this;
        }

        public IDependencyInjector RegisterInstance<TFrom, TTo>() where TTo : TFrom
        {
            return RegisterInstance(typeof(TFrom), typeof(TTo));
        }

        public IDependencyInjector RegisterInstance(Type from, Type to)
        {
            _innerContainer.RegisterType(from, to, new ContainerControlledLifetimeManager());
            return this;
        }

        public IDependencyInjector RegisterType<TFrom, TTo>() where TTo : TFrom
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

    }

}
