using System;
using System.Web.Mvc;
using Lavie.ActionResults;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Models;
using Lavie.Utilities.Exceptions;

namespace Lavie.ActionFilters.Authorization
{
    public class ApiAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly Func<IUser, bool> _checkUser;

        public ApiAuthorizationFilter()
            : this(DependencyResolver.Current, null) { }

        public ApiAuthorizationFilter(Func<IUser, bool> checkUser)
            : this(DependencyResolver.Current, checkUser) { }

        public ApiAuthorizationFilter(IDependencyResolver dependencyResolver, Func<IUser, bool> checkUser)
        {
            Guard.ArgumentNotNull(dependencyResolver, "dependencyResolver");
            Guard.ArgumentNotNull(checkUser, "checkUser");

            this._dependencyResolver = dependencyResolver;
            this._checkUser = checkUser;
        }

        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var requestOrigin = filterContext.RequestContext.HttpContext.Request.Headers["Origin"] ?? "*";
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", requestOrigin);
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "X-Requested-With, Content-Type");

            if (filterContext.HttpContext.Request.HttpMethod == "OPTIONS")
            {
                filterContext.Result = new EmptyResult();
                return;
            }

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
                // TODO: 这里使用了魔值 "400"
                var data = new ApiResult { Code = 400, Message = "登录已超时或无权限", URL = loginUrl };
                filterContext.Result = new DateTimeJsonResult()
                {
                    Data = data,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet, // TODO: 调试模式允许 Get
                };
            }
        }

        #endregion
    }
}
