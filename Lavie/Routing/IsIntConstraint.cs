using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace Lavie.Routing
{
    public class IsInt : IRouteConstraint
    {
        public IsInt() { }

        public IsInt(int minValue, int maxValue)
            : this()
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public IsInt(bool isOptional)
            : this()
        {
            IsOptional = isOptional;
        }

        public IsInt(int minValue, int maxValue, bool isOptional)
            : this(minValue, maxValue)
        {
            IsOptional = isOptional;
        }

        protected int MinValue { get; set; }
        protected int MaxValue { get; set; }
        protected bool IsOptional { get; set; }

        #region IRouteConstraint Members

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
                          RouteDirection routeDirection)
        {
            object parameterValue = values[parameterName];

            int tryInt;

            return (
                       parameterValue != null
                       && int.TryParse(parameterValue.ToString(), out tryInt)
                       && ((MinValue < 1 && MaxValue < 1) || (MinValue <= tryInt && tryInt <= MaxValue))
                   )
                   || (IsOptional && parameterValue == UrlParameter.Optional);
        }

        #endregion
    }
}