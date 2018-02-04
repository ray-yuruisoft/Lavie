using System;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Lavie.Models;
using Lavie.Infrastructure;
using Lavie.Extensions;

namespace Lavie.ActionFilters.Action
{
    public class NoCacheActionFilter : IResultFilter
    {

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
            //filterContext.HttpContext.Response.Cache.SetMaxAge(TimeSpan.Zero);
        }
    }
}
