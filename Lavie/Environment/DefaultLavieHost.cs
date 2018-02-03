using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Infrastructure.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lavie.Environment
{
    public class DefaultLavieHost : ILavieHost
    {
        #region IHost 成员

        void ILavieHost.Initialize(HttpApplication application)
        {
            Initialize(application);
        }

        void ILavieHost.Disponse(HttpApplication application)
        {
            Disponse(application);
        }

        void ILavieHost.BeginRequest(HttpApplication application)
        {
            BeginRequest(application);
        }

        void ILavieHost.PostMapRequestHandler(HttpApplication application)
        {
            PostMapRequestHandler(application);
        }

        void ILavieHost.EndRequest(HttpApplication application)
        {
            EndRequest(application);
        }

        #endregion

        protected virtual void Initialize(HttpApplication application)
        {
            // 启动所有引导程序
            foreach (IBootStrapperTask task in DependencyResolver.Current.GetServices<IBootStrapperTask>())
            {
                task.Execute();
            }
        }
        protected virtual void Disponse(HttpApplication application)
        {
            foreach (IBootStrapperTask task in DependencyResolver.Current.GetServices<IBootStrapperTask>())
            {
                task.Cleanup();
            }
        }
        protected virtual void BeginRequest(HttpApplication application)
        {

        }
        protected virtual void PostMapRequestHandler(HttpApplication application)
        {
            //当前请求的 System.Web.Routing.RequestContext 实例。对于非路由请求，返回的 RequestContext对象为空。
            RequestContext requestContext = application.Request.RequestContext;
            if (requestContext == null) return;

            // 获取已加载模块(Module)
            var moduleRegistry = DependencyResolver.Current.GetService<IModuleRegistry>();

            if (moduleRegistry != null)
            {
                IUser user;
                // 当前模块
                IModule currentModule = moduleRegistry.GetModule(requestContext.RouteData.DataTokens["ModuleName"] as string);
                if (currentModule == null) return;
                // 当前模块的认证模块(可以是当前模块，也可以是其他模块)
                string authenticationModuleName = currentModule.Settings.GetString("AuthenticationModuleName", null);
                IAuthenticationModule authenticationModule = !authenticationModuleName.IsNullOrWhiteSpace() ?
                    moduleRegistry.GetModules<IAuthenticationModule>()
                    .FirstOrDefault(m => m.ModuleName == authenticationModuleName) : null;

                if (authenticationModule != null)
                    user = authenticationModule.GetUser(requestContext);
                else
                    user = new UserAnonymous();

                requestContext.HttpContext.Items[typeof(IUser).FullName] = user;
                requestContext.HttpContext.Items[typeof(IModule).FullName] = currentModule;
                requestContext.HttpContext.Items[typeof(IAuthenticationModule).FullName] = authenticationModule;
                requestContext.HttpContext.Items[typeof(RequestContext).FullName] = requestContext;
            }
        }
        protected virtual void EndRequest(HttpApplication application)
        {
        }

    }
}
