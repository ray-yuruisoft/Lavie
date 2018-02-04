using System.Web.Mvc;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.ViewModels;

namespace Lavie.Modules.Admin.ActionFilters.Action
{
    public class SiteActionFilter : IActionFilter, IExceptionFilter
    {
        private readonly Site _site;
        public SiteActionFilter(Site site)
        {
            this._site = site;
        }

        #region IActionFilter Members

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            SetModel(filterContext.Controller.ViewData.Model as LavieViewModel);
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        #endregion

        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            SetModel(filterContext.Controller.ViewData.Model as LavieViewModel);
        }

        #endregion

        private void SetModel(LavieViewModel model)
        {
            if (model != null)
                model.Site = _site;
        }
    }
}
