using System.Web.Mvc;

namespace Lavie.ActionFilters.Authorization
{
    public class DebugAuthorizationFilter : IAuthorizationFilter
    {
        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            bool debug = false;

#if DEBUG
            debug = true;
#endif

            string queryStringDebugValue = filterContext.HttpContext.Request.QueryString["Debug"];
            if (!string.IsNullOrEmpty(queryStringDebugValue))
            {
                bool queryStringDebug;
                if (bool.TryParse(queryStringDebugValue, out queryStringDebug))
                    debug = queryStringDebug;
            }

            filterContext.Controller.ViewData["Debug"] = debug;
        }

        #endregion
    }
}
