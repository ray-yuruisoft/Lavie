using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System;

namespace Lavie.Routing
{
    public class IsLong : IRouteConstraint
    {
        public IsLong() { }

        public IsLong(Int64 minValue, Int64 maxValue)
            : this()
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public IsLong(bool isOptional)
            : this()
        {
            IsOptional = isOptional;
        }

        public IsLong(int minValue, int maxValue, bool isOptional)
            : this(minValue, maxValue)
        {
            IsOptional = isOptional;
        }

        protected Int64 MinValue { get; set; }
        protected Int64 MaxValue { get; set; }
        protected bool IsOptional { get; set; }

        #region IRouteConstraint Members

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
                          RouteDirection routeDirection)
        {
            object parameterValue = values[parameterName];

            Int64 tryLong;

            return (
                       parameterValue != null
                       && Int64.TryParse(parameterValue.ToString(), out tryLong)
                       && ((MinValue < 1 && MaxValue < 1) || (MinValue <= tryLong && tryLong <= MaxValue))
                   )
                   || (IsOptional && parameterValue == UrlParameter.Optional);
        }

        #endregion
    }
}