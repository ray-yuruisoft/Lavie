using System;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Models;

namespace Lavie.Extensions
{
    public static class PagingExtensions
    {
        #region Paging extensions

        public static MvcHtmlString Paging<T>(this HtmlHelper htmlHelper, IPagedList<T> page)
        {
            return Paging(htmlHelper, page, null,null);
        }

        public static MvcHtmlString Paging<T>(this HtmlHelper htmlHelper, IPagedList<T> page, string actionName)
        {
            return Paging(htmlHelper, page, actionName, null);
        }

        public static MvcHtmlString Paging<T>(this HtmlHelper htmlHelper, IPagedList<T> page, object values)
        {
            return Paging(htmlHelper, page, null, new RouteValueDictionary(values));
        }
   
        public static MvcHtmlString Paging<T>(this HtmlHelper htmlHelper, IPagedList<T> page, RouteValueDictionary valuesDictionary)
        {
            return Paging(htmlHelper, page, null, valuesDictionary);
        }

        public static MvcHtmlString Paging<T>(this HtmlHelper htmlHelper, IPagedList<T> page, string actionName, RouteValueDictionary valuesDictionary)
        {
            return Paging(htmlHelper, page.PageSize, page.PageNumber, page.TotalItemCount,actionName, valuesDictionary);
        }

        public static MvcHtmlString Paging(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount)
        {
            return Paging(htmlHelper, pageSize, currentPage, totalItemCount, null, null);
        }

        public static MvcHtmlString Paging(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName)
        {
            return Paging(htmlHelper, pageSize, currentPage, totalItemCount, actionName, null);
        }

        public static MvcHtmlString Paging(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, object values)
        {
            return Paging(htmlHelper, pageSize, currentPage, totalItemCount, null, new RouteValueDictionary(values));
        }

        public static MvcHtmlString Paging(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary)
        {
            return Paging(htmlHelper, pageSize, currentPage, totalItemCount, null, valuesDictionary);
        }

        public static MvcHtmlString Paging(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName, object values)
        {
            return Paging(htmlHelper, pageSize, currentPage, totalItemCount, actionName, new RouteValueDictionary(values));
        }

        public static MvcHtmlString Paging(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName, RouteValueDictionary valuesDictionary)
        {
            bool dictHasAction = valuesDictionary != null && valuesDictionary.ContainsKey("action");

            if (actionName.IsNullOrWhiteSpace() && !dictHasAction)
                throw new ArgumentException("The Action is not set.");
            else if (!actionName.IsNullOrWhiteSpace() && dictHasAction)
                throw new ArgumentException("The valuesDictionary already contains an action.", "actionName");

            if (valuesDictionary == null)
            {
                valuesDictionary = new RouteValueDictionary();
            }
            if (actionName != null)
            {
                valuesDictionary.Add("action", actionName);
            }

            var pager = new Pager(htmlHelper.ViewContext, pageSize, currentPage, totalItemCount, valuesDictionary);
            return pager.RenderHtml();
        }

        #endregion
    }

}
