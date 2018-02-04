using System.Web.Mvc;
using Lavie.ActionResults;
using Lavie.Models;

namespace Lavie.Extensions
{
    public static class AuthorizationContextExtensions
    {
        public static void SetActionResult(this AuthorizationContext filterContext, string signInUrl)
        {
            ActionResult result;
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                if (!signInUrl.IsNullOrWhiteSpace())
                    result = new RedirectResult(signInUrl);
                else
                    result = new NotFoundResult();
            else
                    //格式:{"Url":"/SignIn"}
                    result = new JsonResult { Data = new AjaxRedirect(signInUrl) };

            filterContext.Result = result;
        }
    }
}
