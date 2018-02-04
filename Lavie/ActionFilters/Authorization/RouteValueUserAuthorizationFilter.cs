using System.Web.Mvc;

namespace Lavie.ActionFilters.Authorization
{
    public class RouteValueUserAuthorizationFilter : RouteValueAuthorizationFilter
    {
        public RouteValueUserAuthorizationFilter(IDependencyResolver dependencyResolver)
            : base(dependencyResolver, "users", (user, authValue) => user.Name != null && user.Name.Equals(authValue, System.StringComparison.OrdinalIgnoreCase))
        {
        }
    }
}
