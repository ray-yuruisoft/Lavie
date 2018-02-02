using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.CKFinder;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Lavie.Startup))]
namespace Lavie
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            // Any connection or hub wire up and configuration should go here
            builder.MapSignalR();

            // CKFinder
            // LoggerManager.LoggerAdapterFactory = new NLogLoggerAdapterFactory();
            // ConnectorConfig.RegisterFileSystems();

            /*
            builder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = ".XOOHOO",     // "ApplicationCookie"
                AuthenticationMode = AuthenticationMode.Active
            });
            */

            var route = "/ckfinder/connector";      // ConfigurationManager.AppSettings["ckfinderRoute"]
            builder.Map(route, x => ConnectorConfig.SetupConnector(x));
        }
    }
}

