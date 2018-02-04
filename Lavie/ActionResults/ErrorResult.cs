using System.Web.Mvc;

namespace Lavie.ActionResults
{
    public class ErrorResult : ViewResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            ViewName = "Error";

            ViewData = context.Controller.ViewData;
            TempData = context.Controller.TempData;

            base.ExecuteResult(context);

            context.HttpContext.Response.StatusDescription = "Internal Error";
            context.HttpContext.Response.StatusCode = 500;
        }
    }
}
