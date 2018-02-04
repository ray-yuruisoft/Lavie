using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Modules.Admin.Models.ViewModels;

namespace Lavie.Modules.Admin.ActionFilters.Action
{
    public class UserActionFilter : IActionFilter
    {
        private readonly IModuleRegistry _moduleRegistry;

        public UserActionFilter(IModuleRegistry moduleRegistry)
        {
            this._moduleRegistry = moduleRegistry;
        }

        #region IActionFilter Members

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var model = filterContext.Controller.ViewData.Model as LavieViewModel;

            if (model != null)
            {
                IUser user = null;

                if (filterContext.HttpContext.Items.Contains(typeof(IUser).FullName))
                {
                    user = filterContext.HttpContext.Items[typeof(IUser).FullName] as IUser;
                }

                if (user != null && user.IsAuthenticated)
                    model.User = user;
                else
                    model.User = filterContext.HttpContext.Request.Cookies.GetAnonymousUser();
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        #endregion
    }
}
