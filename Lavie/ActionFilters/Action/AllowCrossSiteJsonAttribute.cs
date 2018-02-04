using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lavie.ActionFilters.Action
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 如果 ApiAdvancedAuthorizationFilter 添加了 CROS 头则不需要再添加
            if (filterContext.RequestContext.HttpContext.Response.Headers.Get("Access-Control-Allow-Origin") == null)
            {
                var requestOrigin = filterContext.RequestContext.HttpContext.Request.Headers["Origin"] ?? "*";
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", requestOrigin);
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Credentials", "true");
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "X-Requested-With, Content-Type");
            }

            if (filterContext.HttpContext.Request.HttpMethod == "OPTIONS")
            {
                filterContext.Result = new EmptyResult();
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
