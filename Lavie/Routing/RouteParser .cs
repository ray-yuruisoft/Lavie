using System.Web.Routing;
using System.Reflection;
using Lavie.Infrastructure.FastReflectionLib;

namespace Lavie.Routing
{
    internal static class RouteParser
    {
        private static MethodInvoker s_parseInvoker;

        static RouteParser()
        {
            var parserType = typeof(Route).Assembly.GetType("System.Web.Routing.RouteParser");
            var parseMethod = parserType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public);
            s_parseInvoker = new MethodInvoker(parseMethod);
        }

        public static ParsedRoute Parse(string routeUrl)
        {
            return new ParsedRoute(s_parseInvoker.Invoke(null, routeUrl));
        }
    }


}
