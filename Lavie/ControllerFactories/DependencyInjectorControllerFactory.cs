using System;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Infrastructure.InversionOfControl;
using Lavie.Utilities.Exceptions;

namespace Lavie.ControllerFactories
{
    /// <summary>
    /// 主要作用:为Controller重新设置ActionInvoker
    /// </summary>
    public class DependencyInjectorControllerFactory : DefaultControllerFactory
    {
        private readonly IDependencyResolver _dependencyResolver;

        public DependencyInjectorControllerFactory(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        /// <summary>
        /// 获取Controller实例
        /// </summary>
        /// <param name="requestContext">Http请求上下文</param>
        /// <param name="controllerType">Controller类型</param>
        /// <returns>Controller实例</returns>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            Guard.ArgumentNotNull(controllerType, "controllerType"); // 请检查路由配置的 controller 参数

            IController iController = _dependencyResolver.GetService(controllerType) as IController;

            if (typeof(Controller).IsAssignableFrom(controllerType))
            {
                Controller controller = iController as Controller;

                //设置ActionInvoker
                if (controller != null)
                {
                    IActionInvoker actionInvoker = _dependencyResolver.GetService<IActionInvoker>();
                    if(actionInvoker != null)
                        controller.ActionInvoker = actionInvoker;
                }
                return iController;
            }

            return iController;
        }
    }
}
