using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace Lavie.Routing
{
    public class IsPageNumber : IRouteConstraint
    {
        private readonly Regex pageNumberRegex = new Regex(@"(?:(?<=^|/)Page(?<number>\d+)(?=$|/))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object pageNumber = values[parameterName];

            if (pageNumber!=null)
                return pageNumberRegex.Match(pageNumber.ToString()).Groups["number"].Success;

            return true;
        }
    }
}
