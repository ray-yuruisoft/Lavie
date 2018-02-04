using System;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using System.Collections.Generic;
using Lavie.Infrastructure.FastReflectionLib;
using System.Reflection;

namespace Lavie.Html
{
    public static class FormExtensions
    {
        #region Static Constructor

        private static readonly IMethodInvoker FormHelperInvoker;

        static FormExtensions()
        {
            MethodInfo formHelperMethodInfo = typeof(System.Web.Mvc.Html.FormExtensions).GetMethod("FormHelper", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            FormHelperInvoker = new MethodInvoker(formHelperMethodInfo);
        }

        #endregion

        #region BeginFrom

        public static MvcForm BeginForm(this HtmlHelper htmlHelper)
        {
            // generates <form action="{current url}" method="post">...</form>
            string formAction = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
            return FormHelper(htmlHelper, formAction, FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, object routeValues)
        {
            return BeginForm(htmlHelper, null, null, new RouteValueDictionary(routeValues), FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, RouteValueDictionary routeValues)
        {
            return BeginForm(htmlHelper, null, null, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            return BeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues)
        {
            return BeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return BeginForm(htmlHelper, actionName, controllerName, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method)
        {
            return BeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, new RouteValueDictionary());
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method)
        {
            return BeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), method, new RouteValueDictionary());
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method)
        {
            return BeginForm(htmlHelper, actionName, controllerName, routeValues, method, new RouteValueDictionary());
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, object htmlAttributes)
        {
            return BeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            return BeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, htmlAttributes);
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method, object htmlAttributes)
        {
            return BeginForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            string formAction = UrlHelper.GenerateUrl(null /* routeName */, actionName, controllerName, routeValues, htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true /* includeImplicitMvcValues */);
            return FormHelper(htmlHelper, formAction, method, htmlAttributes);
        }

        #endregion

        #region BeginRouteForm
        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, object routeValues)
        {
            return BeginRouteFormEx(htmlHelper, null /* routeName */, new RouteValueDictionary(routeValues), FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, RouteValueDictionary routeValues)
        {
            return BeginRouteFormEx(htmlHelper, null /* routeName */, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, string routeName)
        {
            return BeginRouteFormEx(htmlHelper, routeName, new RouteValueDictionary(), FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, string routeName, object routeValues)
        {
            return BeginRouteFormEx(htmlHelper, routeName, new RouteValueDictionary(routeValues), FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues)
        {
            return BeginRouteFormEx(htmlHelper, routeName, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, string routeName, FormMethod method)
        {
            return BeginRouteFormEx(htmlHelper, routeName, new RouteValueDictionary(), method, new RouteValueDictionary());
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, string routeName, object routeValues, FormMethod method)
        {
            return BeginRouteFormEx(htmlHelper, routeName, new RouteValueDictionary(routeValues), method, new RouteValueDictionary());
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues, FormMethod method)
        {
            return BeginRouteFormEx(htmlHelper, routeName, routeValues, method, new RouteValueDictionary());
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, string routeName, FormMethod method, object htmlAttributes)
        {
            return BeginRouteFormEx(htmlHelper, routeName, new RouteValueDictionary(), method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, string routeName, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            return BeginRouteFormEx(htmlHelper, routeName, new RouteValueDictionary(), method, htmlAttributes);
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, string routeName, object routeValues, FormMethod method, object htmlAttributes)
        {
            return BeginRouteFormEx(htmlHelper, routeName, new RouteValueDictionary(routeValues), method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginRouteFormEx(this HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            string formAction = EnsureUrl(UrlHelper.GenerateUrl(routeName, null, null, routeValues, htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, false /* includeImplicitMvcValues */));
            return FormHelper(htmlHelper, formAction, method, htmlAttributes);
        }

        #endregion

        #region Private Methods

        private static string EnsureUrl(string url)
        {
            return UrlHelperExtensions.EnsureUrl(url);

        }
        private static MvcForm FormHelper(this HtmlHelper htmlHelper, string formAction, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            return (MvcForm)FormHelperInvoker.Invoke(null, htmlHelper, formAction, method, htmlAttributes);
        }
        
        #endregion
    }
}
