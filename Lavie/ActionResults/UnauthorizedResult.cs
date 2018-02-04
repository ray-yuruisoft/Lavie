using System.Web.Mvc;
using Lavie.ActionFilters;
using System.Web;
using Lavie.Models;
using Lavie.Infrastructure;
using Lavie.Extensions;
using System.Collections.Generic;
using System.Web.Routing;

namespace Lavie.ActionResults
{
    public class UnauthorizedResult : ViewResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            base.ViewName = "Unauthorized";

            base.ViewData = context.Controller.ViewData;
            base.TempData = context.Controller.TempData;
                
            base.ExecuteResult(context);
            
            context.HttpContext.Response.StatusDescription = "Forbidden";
            context.HttpContext.Response.StatusCode = 403;
        }


    }
}
