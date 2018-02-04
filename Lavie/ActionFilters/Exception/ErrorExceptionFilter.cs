using System.Linq;
using System.Web.Mvc;
using Lavie.ActionResults;
using Lavie.Infrastructure;

namespace Lavie.ActionFilters.Exception
{
    public class ErrorExceptionFilter : IExceptionFilter
    {
        private readonly IModuleRegistry _moduleRegistry;

        public ErrorExceptionFilter(IModuleRegistry moduleRegistry)
        {
            var routeData = new System.Web.Routing.RouteData();
            _moduleRegistry = moduleRegistry;
        }

        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            filterContext.Controller.ViewData.Model = filterContext.Exception;

            var debugViewData = filterContext.Controller.ViewData["Debug"];
            //DEBUG模式显示红黄页
            if (debugViewData != null && debugViewData is bool && (bool)debugViewData)
            {
                filterContext.HttpContext.AddError(filterContext.Exception);
            }

            ILoggingModule loggingModule = _moduleRegistry.GetModules<ILoggingModule>().LastOrDefault();
            if (loggingModule != null)
            {
                loggingModule.Fatal(filterContext.Exception);
                if (filterContext.Exception.InnerException != null)
                {
                    loggingModule.Fatal(filterContext.Exception.InnerException);
                }
            }

            filterContext.ExceptionHandled = true;

            filterContext.Result = new ErrorResult();
        }

        #endregion
    }
}
