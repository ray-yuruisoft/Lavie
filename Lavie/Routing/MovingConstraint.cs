using System;
using System.Web;
using System.Web.Routing;
using Lavie.Extensions;
using Lavie.Models;

namespace Lavie.Routing
{
    public class IsMovingTarget : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object parameterValue = values[parameterName];

            //区分大小写
            return parameterValue != null&&Enum.IsDefined(typeof(MovingTarget), parameterValue);
        }
    }
    public class IsMovingLocation : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object parameterValue = values[parameterName];

            //区分大小写
            return parameterValue != null && Enum.IsDefined(typeof(MovingLocation), parameterValue);
        }
    }

}
