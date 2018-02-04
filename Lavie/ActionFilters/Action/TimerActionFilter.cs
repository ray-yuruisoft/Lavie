using System.Web.Mvc;
using System.Diagnostics;
using Lavie.Models;
using Lavie.Infrastructure;
using Lavie.Extensions;

namespace Lavie.ActionFilters.Action
{
    public class TimerActionFilter : IActionFilter, IResultFilter
    {
        #region IActionFilter Members

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller;
            if (controller != null)
            {
                var stopwatch = new Stopwatch();
                controller.ViewData["StopWatch"] = stopwatch;
                stopwatch.Start();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller;
            if (controller != null)
            {
                var stopwatch = (Stopwatch)controller.ViewData["StopWatch"];
                stopwatch.Stop();
                controller.ViewData["Duration"] = stopwatch.Elapsed.TotalMilliseconds;
            }
        }

        #endregion

        #region IResultFilter Members

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                ResponseFilter responseFilter = new ResponseFilter(filterContext.HttpContext.Response.Filter, filterContext.HttpContext);
                responseFilter.Inserts.Add(
                    new ResponseInsert(i => "<div class=\"timer\">执行时间：{0}毫秒</div>".FormatWith(filterContext.Controller.ViewData["Duration"]),
                        ResponseInsertMode.AppendTo, "body"));
                filterContext.HttpContext.Response.Filter = responseFilter;
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        #endregion
    }
}
