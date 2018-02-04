using System.Web.Mvc;

namespace Lavie.ActionFilters.Authorization
{
    public class RouteValueRoleAuthorizationFilter : RouteValueAuthorizationFilter
    {
        public RouteValueRoleAuthorizationFilter(IDependencyResolver dependencyResolver)
            : base(dependencyResolver, "roles", (user, authValue) => user.IsInRole(authValue))
        {
        }
    }
}
