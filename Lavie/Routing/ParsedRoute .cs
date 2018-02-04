using System.Web.Routing;
using System.Reflection;
using Lavie.Extensions;
using Lavie.Infrastructure.FastReflectionLib;

namespace Lavie.Routing
{
    internal class ParsedRoute
    {
        private static MethodInvoker s_matchInvoker;
        private static MethodInvoker s_bindInvoker;

        static ParsedRoute()
        {
            var routeType = typeof(Route).Assembly.GetType("System.Web.Routing.ParsedRoute");

            var matchMethod = routeType.GetMethod("Match", BindingFlags.Instance | BindingFlags.Public);
            s_matchInvoker = new MethodInvoker(matchMethod);

            var bindMethod = routeType.GetMethod("Bind", BindingFlags.Instance | BindingFlags.Public);
            s_bindInvoker = new MethodInvoker(bindMethod);
        }

        private object m_instance;

        public ParsedRoute(object instance)
        {
            this.m_instance = instance;
        }

        public RouteValueDictionary Match(
            string virtualPath,
            RouteValueDictionary defaultValues)
        {
            return (RouteValueDictionary)s_matchInvoker.Invoke(this.m_instance, virtualPath, defaultValues);
        }

        public BoundUrl Bind(
            RouteValueDictionary currentValues,
            RouteValueDictionary values,
            RouteValueDictionary defaultValues,
            RouteValueDictionary constraints)
        {
            object boundUrl = s_bindInvoker.Invoke(this.m_instance, currentValues, values, defaultValues, constraints);
            return boundUrl == null ? null : new BoundUrl(boundUrl);
        }

    }

}
