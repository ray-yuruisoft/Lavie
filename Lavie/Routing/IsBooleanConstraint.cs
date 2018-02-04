using System;
using System.Web;
using System.Web.Routing;
using Lavie.Extensions;
using System.Web.Mvc;

namespace Lavie.Routing
{
    public class IsBoolean : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object parameterValue = values[parameterName];
            bool result;
            return parameterValue != null && bool.TryParse(parameterValue.ToString(), out result);

        }
    }
}
