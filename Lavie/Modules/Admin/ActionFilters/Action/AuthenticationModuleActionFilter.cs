using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Modules.Admin.Models.ViewModels;

namespace Lavie.Modules.Admin.ActionFilters.Action
{
    public class AuthenticationModuleActionFilter : IActionFilter
    {
        private readonly IModuleRegistry _moduleRegistry;

        public AuthenticationModuleActionFilter(IModuleRegistry moduleRegistry)
        {
            this._moduleRegistry = moduleRegistry;
        }

        #region IActionFilter Members

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var model = filterContext.Controller.ViewData.Model as LavieViewModel;

            if (model != null)
            {
                IAuthenticationModule authenticationModule = null;

                if (filterContext.HttpContext.Items.Contains(typeof(IAuthenticationModule).FullName))
                {
                    authenticationModule = filterContext.HttpContext.Items[typeof(IAuthenticationModule).FullName] as IAuthenticationModule;
                }
                model.AuthenticationModule = authenticationModule;
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        #endregion
    }
}
