using CKSource.CKFinder.Connector.Core.Logs;
using CKSource.CKFinder.Connector.Logs.NLog;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Configuration;
using System.Reflection;
using Lavie.CKFinder;

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
                AuthenticationType = ".LAVIE",     // "ApplicationCookie"
                AuthenticationMode = AuthenticationMode.Active
            });
            */

            var route = "/ckfinder/connector";      // ConfigurationManager.AppSettings["ckfinderRoute"]
            builder.Map(route, x => ConnectorConfig.SetupConnector(x));
        }
    }
}
