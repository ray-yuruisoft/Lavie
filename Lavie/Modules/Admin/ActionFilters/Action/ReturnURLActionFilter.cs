using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Modules.Admin.Models.ViewModels;

namespace Lavie.Modules.Admin.ActionFilters.Action
{
    public class ReturnURLActionFilter : IActionFilter, IExceptionFilter
    {
        #region IActionFilter Members

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            setModel(filterContext);
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }

        #endregion

        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            setModel(filterContext);
        }

        #endregion

        private void setModel(ControllerContext controllerContext)
        {
            var model = controllerContext.Controller.ViewData.Model as LavieViewModel;
            if (model == null) return ;

            string returnUrlValue = controllerContext.HttpContext.Request["ReturnURL"];
            //if (returnUrlValue.IsNullOrWhiteSpace())
            //{
            //    if (controllerContext.HttpContext.Request.UrlReferrer != null)
            //        returnUrlValue = controllerContext.HttpContext.Request.UrlReferrer.AbsolutePath;
            //}

            if (!returnUrlValue.IsNullOrWhiteSpace())
            {
                model.ReturnURL = returnUrlValue;
            }
        }
    }
}
