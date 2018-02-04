using System;
using System.Web;
using System.Web.Routing;
using Lavie.Extensions;
using System.Web.Mvc;
using System.Linq;

namespace Lavie.Routing
{
    public class IsNullableGuidList : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object parameterValue = values[parameterName];

            if (parameterValue == null 
                || parameterValue == UrlParameter.Optional
                || parameterValue.ToString().IsNullOrWhiteSpace())
                return true;

            Guid result;
            string[] arr = parameterValue.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return arr.All(g => g.GuidTryParse(out result));

        }
    }
}
