using System;
using System.Web;
using System.Web.Routing;
using Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Routing
{
    public class IsUserStatus : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object parameterValue = values[parameterName];
            
            //区分大小写
            return parameterValue != null && Enum.IsDefined(typeof(UserStatus), parameterValue);
        }
    }
}
