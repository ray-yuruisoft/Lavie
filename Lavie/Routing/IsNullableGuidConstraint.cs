using System;
using System.Web;
using System.Web.Routing;
using Lavie.Extensions;
using System.Web.Mvc;

namespace Lavie.Routing
{
    public class IsNullableGuid : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object parameterValue = values[parameterName];

            if (parameterValue == null
                || parameterValue == UrlParameter.Optional
                || parameterValue.ToString().IsNullOrWhiteSpace())
                return true;

            Guid result;
            return parameterValue.ToString().GuidTryParse(out result);
        }
    }
}
