using System.Web.Mvc;

namespace Lavie.ActionFilters.Authorization
{
    public class RouteValuePermissionAuthorizationFilter : RouteValueAuthorizationFilter
    {
        public RouteValuePermissionAuthorizationFilter(IDependencyResolver dependencyResolver)
            : base(dependencyResolver, "permissions", (user, authValue) => user.HasPermission(authValue))
        {
        }
    }
}
