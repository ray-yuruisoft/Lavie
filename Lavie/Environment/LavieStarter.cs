using Lavie.ActionInvokers;
using Lavie.ControllerFactories;
using Lavie.FilterProviders;
using Lavie.Infrastructure.InversionOfControl;
using Lavie.InversionOfControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Lavie.Environment
{
    public static class LavieStarter
    {
        public static ILavieHost CreateHost()
        {
            Initialize();
            return DependencyResolver.Current.GetService<ILavieHost>();
        }

        private static void Initialize()
        {
            IDependencyInjector dependencyInjector = new DependencyInjectorFactory().CreateDependencyInjector();
            if (dependencyInjector != null)
            {
                // 设置 IoC/DI 容器
                SetResolver(dependencyInjector);

                // 注册FilterProviders
                RegisterFilterProvider(dependencyInjector);

                // 注册ActionInvoker
                RegisterActionInvoker(dependencyInjector);

                // 设置控制器工厂
                SetControllerFactory(dependencyInjector);

            }
        }

        /// <summary>
        /// 设置 IoC/DI 容器
        /// </summary>
        /// <param name="dependencyResolver">IoC/DI 容器</param>
        private static void SetResolver(IDependencyResolver dependencyResolver)
        {
            // 用于替换ASP.NET MVC内置的依赖注入容器DefaultDependencyResolver
            DependencyResolver.SetResolver(dependencyResolver);
        }

        /// <summary>
        /// 设置控制器工厂
        /// <remarks>主要目的是在ControllerFactory的GetControllerInstance方法中获取Controller实例后，重新设置其ActionInvoker</remarks>
        /// </summary>
        /// <param name="dependencyInjector">IoC/DI 容器</param>
        private static void SetControllerFactory(IDependencyResolver dependencyInjector)
        {
            IControllerFactory controllerFactory = dependencyInjector.GetService<DependencyInjectorControllerFactory>();
            if (controllerFactory != null)
            {
                ControllerBuilder.Current.SetControllerFactory(controllerFactory);

                #region 设置控制器工厂的几种方式

                //方式1：
                //实现一个IControllerFactory并RegisterType注册，不能再使用SetControllerFactory设置
                //dependencyInjector.RegisterType<IControllerFactory, DependencyInjectorControllerFactory>();
                //方式2：
                //实现一个IControllerFactory创建对象，再使用SetControllerFactory设置
                //ControllerBuilder.Current.SetControllerFactory(dependencyInjector.GetService<DependencyInjectorControllerFactory>());
                //方式3：
                //实现一个IControllerActivator，如：NewControllerActivator,并RegisterType注册
                //DependencyInjector.RegisterType<IControllerActivator, NewControllerActivator>();
                //方式4：
                //实现一个IControllerActivator，如：NewControllerActivator,并创建对象
                //再使用DefaultControllerFactory(IControllerActivator controllerActivator)构造
                //最后使用SetControllerFactory设置
                //IControllerActivator activator = new NewControllerActivator();
                //DefaultControllerFactory controllerFactory = new DefaultControllerFactory(activator);
                //ControllerBuilder.Current.SetControllerFactory(controllerFactory);

                #endregion
            }
        }

        /// <summary>
        /// 注册ActionInvoker
        /// </summary>
        /// <param name="dependencyInjector">IoC/DI 容器</param>
        private static void RegisterActionInvoker(IDependencyInjector dependencyInjector)
        {
            dependencyInjector
                //.RegisterType<IActionInvoker, XoohooControllerActionInvoker>()
                .RegisterType<IAsyncActionInvoker, LavieAsyncControllerActionInvoker>();
        }

        /// <summary>
        /// 注册FilterProviders
        /// </summary>
        /// <param name="dependencyResolver">IoC/DI 容器</param>
        private static void RegisterFilterProvider(IDependencyResolver dependencyResolver)
        {
            //FilterProviderCollection filterProviders = dependencyResolver.GetService<FilterProviderCollection>() ?? System.Web.Mvc.FilterProviders.Providers;
            //filterProviders.Add(dependencyResolver.GetService<FilterRegistryFilterProvider>());
            System.Web.Mvc.FilterProviders.Providers.Add(FilterRegistry.Filters);

        }
    }
}
