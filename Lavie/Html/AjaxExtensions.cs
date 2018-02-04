using System;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using System.Collections.Generic;
using System.Globalization;
using Lavie.Infrastructure.FastReflectionLib;
using System.Reflection;
using System.Text;
using Lavie.Utilities.Exceptions;

namespace Lavie.Html
{
    public static class AjaxExtensions
    {
        #region Static Constructor

        private static IMethodInvoker s_FormHelperInvoker;
        private static IMethodInvoker s_ToJavascriptStringInvoker;

        static AjaxExtensions()
        {
            MethodInfo formHelperMethodInfo = typeof(System.Web.Mvc.Ajax.AjaxExtensions).GetMethod("FormHelper", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            s_FormHelperInvoker = new MethodInvoker(formHelperMethodInfo);
            MethodInfo toJavascriptStringMethodInfo = typeof(System.Web.Mvc.Ajax.AjaxOptions).GetMethod("ToJavascriptString", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            s_ToJavascriptStringInvoker = new MethodInvoker(toJavascriptStringMethodInfo);
        }

        #endregion

        private const string LinkOnClickFormat = "Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), {0});";
        private const string FormOnClickValue = "Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));";
        private const string FormOnSubmitFormat = "Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), {0});";
        private const string _globalizationScript = @"<script type=""text/javascript"" src=""{0}""></script>";

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, AjaxOptions ajaxOptions)
        {
            return ActionLinkEx(ajaxHelper, linkText, actionName, (string)null /* controllerName */, ajaxOptions);
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLinkEx(ajaxHelper, linkText, actionName, (string)null /* controllerName */, routeValues, ajaxOptions);
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ActionLinkEx(ajaxHelper, linkText, actionName, (string)null /* controllerName */, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLinkEx(ajaxHelper, linkText, actionName, (string)null /* controllerName */, routeValues, ajaxOptions);
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ActionLinkEx(ajaxHelper, linkText, actionName, (string)null /* controllerName */, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions)
        {
            return ActionLinkEx(ajaxHelper, linkText, actionName, controllerName, null /* values */, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLinkEx(ajaxHelper, linkText, actionName, controllerName, routeValues, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var newValues = new RouteValueDictionary(routeValues);
            var newAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return ActionLinkEx(ajaxHelper, linkText, actionName, controllerName, newValues, ajaxOptions, newAttributes);
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLinkEx(ajaxHelper, linkText, actionName, controllerName, routeValues, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            Guard.ArgumentNotNull(linkText, "linkText");

            string targetUrl = EnsureUrl(UrlHelper.GenerateUrl(null, actionName, controllerName, routeValues, ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true /* includeImplicitMvcValues */));

            return MvcHtmlString.Create(GenerateLink(ajaxHelper, linkText, targetUrl, GetAjaxOptions(ajaxOptions), htmlAttributes));
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var newValues = new RouteValueDictionary(routeValues);
            var newAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return ActionLinkEx(ajaxHelper, linkText, actionName, controllerName, protocol, hostName, fragment, newValues, ajaxOptions, newAttributes);
        }

        public static MvcHtmlString ActionLinkEx(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            Guard.ArgumentNotNull(linkText, "linkText");

            string targetUrl = EnsureUrl(UrlHelper.GenerateUrl(null /* routeName */, actionName, controllerName, protocol, hostName, fragment, routeValues, ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true /* includeImplicitMvcValues */));

            return MvcHtmlString.Create(GenerateLink(ajaxHelper, linkText, targetUrl, ajaxOptions, htmlAttributes));
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, AjaxOptions ajaxOptions)
        {
            string formAction = ajaxHelper.ViewContext.HttpContext.Request.RawUrl;
            return FormHelper(ajaxHelper, formAction, ajaxOptions, new RouteValueDictionary());
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, string actionName, AjaxOptions ajaxOptions)
        {
            return BeginFormEx(ajaxHelper, actionName, (string)null /* controllerName */, ajaxOptions);
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            return BeginFormEx(ajaxHelper, actionName, (string)null /* controllerName */, routeValues, ajaxOptions);
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return BeginFormEx(ajaxHelper, actionName, (string)null /* controllerName */, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return BeginFormEx(ajaxHelper, actionName, (string)null /* controllerName */, routeValues, ajaxOptions);
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return BeginFormEx(ajaxHelper, actionName, (string)null /* controllerName */, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, string actionName, string controllerName, AjaxOptions ajaxOptions)
        {
            return BeginFormEx(ajaxHelper, actionName, controllerName, null /* values */, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions)
        {
            return BeginFormEx(ajaxHelper, actionName, controllerName, routeValues, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var newValues = new RouteValueDictionary(routeValues);
            var newAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return BeginFormEx(ajaxHelper, actionName, controllerName, newValues, ajaxOptions, newAttributes);
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return BeginFormEx(ajaxHelper, actionName, controllerName, routeValues, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcForm BeginFormEx(this AjaxHelper ajaxHelper, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            // get target URL
            string formAction = EnsureUrl(UrlHelper.GenerateUrl(null, actionName, controllerName, routeValues ?? new RouteValueDictionary(), ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true /* includeImplicitMvcValues */));
            return FormHelper(ajaxHelper, formAction, ajaxOptions, htmlAttributes);
        }

        public static MvcForm BeginRouteFormEx(this AjaxHelper ajaxHelper, string routeName, AjaxOptions ajaxOptions)
        {
            return BeginRouteFormEx(ajaxHelper, routeName, null /* routeValues */, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcForm BeginRouteFormEx(this AjaxHelper ajaxHelper, string routeName, object routeValues, AjaxOptions ajaxOptions)
        {
            return BeginRouteFormEx(ajaxHelper, routeName, (object)routeValues, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcForm BeginRouteFormEx(this AjaxHelper ajaxHelper, string routeName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            RouteValueDictionary newAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return BeginRouteFormEx(ajaxHelper, routeName, new RouteValueDictionary(routeValues), ajaxOptions, newAttributes);
        }

        public static MvcForm BeginRouteFormEx(this AjaxHelper ajaxHelper, string routeName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return BeginRouteFormEx(ajaxHelper, routeName, routeValues, ajaxOptions, null /* htmlAttributes */);
        }

        public static MvcForm BeginRouteFormEx(this AjaxHelper ajaxHelper, string routeName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            string formAction = EnsureUrl(UrlHelper.GenerateUrl(routeName, null /* actionName */, null /* controllerName */, routeValues ?? new RouteValueDictionary(), ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, false /* includeImplicitMvcValues */));
            return FormHelper(ajaxHelper, formAction, ajaxOptions, htmlAttributes);
        }

        private static MvcForm FormHelper(this AjaxHelper ajaxHelper, string formAction, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return (MvcForm)s_FormHelperInvoker.Invoke(null, ajaxHelper, formAction, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString GlobalizationScript(this AjaxHelper ajaxHelper)
        {
            return GlobalizationScript(ajaxHelper, CultureInfo.CurrentCulture);
        }

        public static MvcHtmlString GlobalizationScript(this AjaxHelper ajaxHelper, CultureInfo cultureInfo)
        {
            return GlobalizationScriptHelper(AjaxHelper.GlobalizationScriptPath, cultureInfo);
        }

        private static MvcHtmlString GlobalizationScriptHelper(string scriptPath, CultureInfo cultureInfo)
        {
            Guard.ArgumentNotNull(cultureInfo, "cultureInfo");

            string src = VirtualPathUtility.AppendTrailingSlash(scriptPath) + cultureInfo.Name + ".js";
            string scriptWithCorrectNewLines = _globalizationScript.Replace("\r\n", System.Environment.NewLine);
            string formatted = String.Format(CultureInfo.InvariantCulture, scriptWithCorrectNewLines, src);

            return MvcHtmlString.Create(formatted);
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, object routeValues, AjaxOptions ajaxOptions)
        {
            return RouteLinkEx(ajaxHelper, linkText, null /* routeName */, new RouteValueDictionary(routeValues), ajaxOptions,
                             new Dictionary<string, object>());
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return RouteLinkEx(ajaxHelper, linkText, null /* routeName */, new RouteValueDictionary(routeValues), ajaxOptions,
                             HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return RouteLinkEx(ajaxHelper, linkText, null /* routeName */, routeValues, ajaxOptions,
                             new Dictionary<string, object>());
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return RouteLinkEx(ajaxHelper, linkText, null /* routeName */, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, string routeName, AjaxOptions ajaxOptions)
        {
            return RouteLinkEx(ajaxHelper, linkText, routeName, new RouteValueDictionary(), ajaxOptions,
                             new Dictionary<string, object>());
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, string routeName, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return RouteLinkEx(ajaxHelper, linkText, routeName, new RouteValueDictionary(), ajaxOptions, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, string routeName, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return RouteLinkEx(ajaxHelper, linkText, routeName, new RouteValueDictionary(), ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, string routeName, object routeValues, AjaxOptions ajaxOptions)
        {
            return RouteLinkEx(ajaxHelper, linkText, routeName, new RouteValueDictionary(routeValues), ajaxOptions,
                             new Dictionary<string, object>());
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, string routeName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return RouteLinkEx(ajaxHelper, linkText, routeName, new RouteValueDictionary(routeValues), ajaxOptions,
                             HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, string routeName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return RouteLinkEx(ajaxHelper, linkText, routeName, routeValues, ajaxOptions, new Dictionary<string, object>());
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, string routeName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            Guard.ArgumentNotNull(linkText, "linkText");

            string targetUrl = EnsureUrl(UrlHelper.GenerateUrl(routeName, null /* actionName */, null /* controllerName */, routeValues ?? new RouteValueDictionary(), ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, false /* includeImplicitMvcValues */));

            return MvcHtmlString.Create(GenerateLink(ajaxHelper, linkText, targetUrl, GetAjaxOptions(ajaxOptions), htmlAttributes));
        }

        public static MvcHtmlString RouteLinkEx(this AjaxHelper ajaxHelper, string linkText, string routeName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            Guard.ArgumentNotNull(linkText, "linkText");

            string targetUrl = EnsureUrl(UrlHelper.GenerateUrl(routeName, null /* actionName */, null /* controllerName */, protocol, hostName, fragment, routeValues ?? new RouteValueDictionary(), ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, false /* includeImplicitMvcValues */));

            return MvcHtmlString.Create(GenerateLink(ajaxHelper, linkText, targetUrl, GetAjaxOptions(ajaxOptions), htmlAttributes));
        }

        private static string GenerateLink(AjaxHelper ajaxHelper, string linkText, string targetUrl, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            var tag = new TagBuilder("a")
            {
                InnerHtml = HttpUtility.HtmlEncode(linkText)
            };

            tag.MergeAttributes(htmlAttributes);
            tag.MergeAttribute("href", targetUrl);

            if (ajaxHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
            {
                tag.MergeAttributes(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            }
            else
            {
                tag.MergeAttribute("onclick", GenerateAjaxScript(ajaxOptions, LinkOnClickFormat));
            }

            return tag.ToString(TagRenderMode.Normal);
        }

        private static string GenerateAjaxScript(AjaxOptions ajaxOptions, string scriptFormat)
        {
            string optionsString = s_ToJavascriptStringInvoker.Invoke(ajaxOptions) as String;
            return String.Format(CultureInfo.InvariantCulture, scriptFormat, optionsString);
        }

        private static AjaxOptions GetAjaxOptions(AjaxOptions ajaxOptions)
        {
            return ajaxOptions ?? new AjaxOptions();
        }

        #region Private Methods

        private static string EnsureUrl(string url)
        {
            return UrlHelperExtensions.EnsureUrl(url);

        }

        #endregion
    }
}
