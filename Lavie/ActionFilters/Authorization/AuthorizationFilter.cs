using System;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Utilities.Exceptions;

namespace Lavie.ActionFilters.Authorization
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly Func<IUser, bool> _checkUser;

        public AuthorizationFilter()
            : this(DependencyResolver.Current, null) { }

        public AuthorizationFilter(Func<IUser, bool> checkUser)
            : this(DependencyResolver.Current, checkUser) { }

        public AuthorizationFilter(IDependencyResolver dependencyResolver, Func<IUser, bool> checkUser)
        {
            Guard.ArgumentNotNull(dependencyResolver, "dependencyResolver");
            Guard.ArgumentNotNull(checkUser, "checkUser");

            this._dependencyResolver = dependencyResolver;
            this._checkUser = checkUser;
        }

        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //当前用户
            IUser user = _dependencyResolver.GetService<IUser>();

            // 如果用户尚未登录，或者没有通过验证
            if (user == null || !user.IsAuthenticated || (_checkUser != null && !_checkUser(user)))
            {
                //当前模块的认证模块
                var authenticationModule = _dependencyResolver.GetService<IAuthenticationModule>();
                if (authenticationModule == null)
                    throw new NullReferenceException("AuthenticationModule of current module is null.");
                string loginUrl = authenticationModule.GetLoginUrl(filterContext.RequestContext);
                filterContext.SetActionResult(loginUrl);
            }

        }

        #endregion
    }
}
