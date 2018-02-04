using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.ControllerFactories;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Infrastructure.InversionOfControl;
using Lavie.InversionOfControl;
using Lavie.Models;
using Lavie.FilterProviders;
using Lavie.ActionInvokers;
using Lavie.WarmupStarter;
using Lavie.Environment;
using System.Text;

namespace Lavie
{
    public class MvcApplication : HttpApplication
    {
        private static Starter<ILavieHost> _starter;

        protected void Application_Start()
        {
            _starter = new Starter<ILavieHost>(HostInitialization, HostDisponse, HostBeginRequest, HostPostMapRequestHandler, HostEndRequest);
            _starter.OnApplicationStart(this);
        }

        protected void Application_End()
        {
            _starter.OnApplicationEnd(this);
        }

        protected void Application_BeginRequest()
        {
            _starter.OnBeginRequest(this);
        }

        protected void Application_PostMapRequestHandler()
        {
            _starter.OnPostMapRequestHandler(this);
        }

        protected void Application_EndRequest()
        {
            _starter.OnEndRequest(this);
        }

        #region Private Static Methods

        private static ILavieHost HostInitialization(HttpApplication application)
        {
            var host = LavieStarter.CreateHost();
            host.Initialize(application);
            return host;
        }

        private static void HostDisponse(HttpApplication application, ILavieHost host)
        {
            host.Disponse(application);
        }

        private static void HostBeginRequest(HttpApplication application, ILavieHost host)
        {
            host.BeginRequest(application);
        }

        private static void HostPostMapRequestHandler(HttpApplication application, ILavieHost host)
        {
            host.PostMapRequestHandler(application);
        }

        private static void HostEndRequest(HttpApplication application, ILavieHost host)
        {
            host.EndRequest(application);
        }

        #endregion
    }
}
