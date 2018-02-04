using System.Web.Mvc;

namespace Lavie.ActionFilters.Authorization
{
    public class RouteValueGroupAuthorizationFilter : RouteValueAuthorizationFilter
    {
        public RouteValueGroupAuthorizationFilter(IDependencyResolver dependencyResolver)
            : base(dependencyResolver, "groups", (user, authValue) => user.IsInGroup(authValue))
        {
        }
    }
}
