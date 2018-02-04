using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Lavie.Extensions;

namespace Lavie.Routing
{
    public class NotIn : IRouteConstraint
    {
        private readonly string[] Values;
        public NotIn(params string[] values) { 
            Values = values;
        }

        #region IRouteConstraint Members

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
                          RouteDirection routeDirection)
        {
            string parameterValue = values[parameterName] as string;
            if (parameterValue.IsNullOrWhiteSpace()) return true;
            return !Values.Any(m => m.Equals(parameterValue, StringComparison.CurrentCultureIgnoreCase));
        }

        #endregion
    }
}