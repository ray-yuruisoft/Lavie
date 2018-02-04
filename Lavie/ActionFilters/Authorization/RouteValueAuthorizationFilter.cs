using System;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Utilities.Exceptions;

namespace Lavie.ActionFilters.Authorization
{
    public class RouteValueAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IDependencyResolver _dependencyResolver;

        private readonly string _routeKey;
        private readonly Func<IUser, string, bool> _checkUser;

        public RouteValueAuthorizationFilter(string routeKey, Func<IUser, string, bool> checkUser)
            : this(DependencyResolver.Current, routeKey,checkUser) { }

        public RouteValueAuthorizationFilter(IDependencyResolver dependencyResolver, string routeKey, Func<IUser, string, bool> checkUser)
        {
            Guard.ArgumentNotNull(dependencyResolver, "dependencyResolver");
            Guard.ArgumentNotNull(routeKey, "routeKey");
            Guard.ArgumentNotNull(checkUser, "checkUser");

            this._dependencyResolver = dependencyResolver;
            this._routeKey = routeKey;
            this._checkUser = checkUser;
        }

        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //当前用户
            IUser user = _dependencyResolver.GetService<IUser>();

            bool isAuthorized = false;

            // 如果用户尚未登录
            if (user != null && user.IsAuthenticated)
            {
                string authValue = filterContext.RouteData.Values[_routeKey] as string;
                if (authValue.IsNullOrWhiteSpace()) return;
                var authValueArray = authValue.Split('|');

                // 如果用户已经登录，但是没有通过权限校验
                foreach (var v in authValueArray)
                {
                    if (isAuthorized = _checkUser(user, v)) break;
                }

            }
            if (!isAuthorized)
            {
                //当前模块的认证模块
                IAuthenticationModule authenticationModule = _dependencyResolver.GetService<IAuthenticationModule>();
                if (authenticationModule == null)
                    throw new NullReferenceException("AuthenticationModule of current module is null.");
                string loginUrl = authenticationModule.GetLoginUrl(filterContext.RequestContext);
                filterContext.SetActionResult(loginUrl);
            }

        }

        #endregion
    }
}
