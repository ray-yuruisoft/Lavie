using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lavie.ActionResults
{
    public class NotFoundResult : ViewResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            base.ViewName = "NotFound";

            base.ViewData = context.Controller.ViewData;
            base.TempData = context.Controller.TempData;

            base.ExecuteResult(context);

            context.HttpContext.Response.StatusDescription = "File Not Found";
            context.HttpContext.Response.StatusCode = 404;
        }
    }
}
