using System;
using System.Web;
using System.Web.Routing;
using Lavie.Extensions;
using System.Web.Mvc;

namespace Lavie.Routing
{
    public class IsGuid : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object parameterValue = values[parameterName];
            Guid result;
            return parameterValue != null && parameterValue.ToString().GuidTryParse(out result);

        }
    }
}
