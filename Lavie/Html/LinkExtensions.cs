using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Extensions;
using System;
using System.Web;
using Lavie.Utilities.Exceptions;

namespace Lavie.Html
{
    public static class LinkExtensions
    {
        #region Link

        public static MvcHtmlString Link(this HtmlHelper htmlHelper, string linkText, string href)
        {
            return htmlHelper.Link(linkText, href, null,null);
        }
        public static MvcHtmlString Link(this HtmlHelper htmlHelper, string linkText, string href,string title)
        {
            return htmlHelper.Link(linkText, href, title, null);
        }
        public static MvcHtmlString Link(this HtmlHelper htmlHelper, string linkText, string href, object htmlAttributes)
        {
            return htmlHelper.Link(linkText, href,null, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString Link(this HtmlHelper htmlHelper, string linkText, string href, string title, object htmlAttributes)
        {
            return htmlHelper.Link(linkText, href,title, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString Link(this HtmlHelper htmlHelper, string linkText, string href, string title, IDictionary<string, object> htmlAttributes)
        {
            var tagBuilder = new TagBuilder("a")
            {
                InnerHtml = linkText
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("href", href);
            if (!title.IsNullOrEmpty())
                tagBuilder.MergeAttribute("title", title);

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        #endregion

        #region ActionLinkEx

        public static MvcHtmlString ActionLinkEx(this HtmlHelper htmlHelper, string linkText, string actionName)
        {
            return ActionLinkEx(htmlHelper, linkText, actionName, null /* controllerName */, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLinkEx(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues)
        {
            return ActionLinkEx(htmlHelper, linkText, actionName, null /* controllerName */, new RouteValueDictionary(routeValues), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLinkEx(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes)
        {
            return ActionLinkEx(htmlHelper, linkText, actionName, null /* controllerName */, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ActionLinkEx(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return ActionLinkEx(htmlHelper, linkText, actionName, null /* controllerName */, routeValues, new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLinkEx(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return ActionLinkEx(htmlHelper, linkText, actionName, null /* controllerName */, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionLinkEx(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
        {
            return ActionLinkEx(htmlHelper, linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLinkEx(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return ActionLinkEx(htmlHelper, linkText, actionName, controllerName, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ActionLinkEx(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            Guard.ArgumentNotNull(linkText, "linkText");

            return MvcHtmlString.Create(GenerateLink(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, linkText, null/* routeName */, actionName, controllerName, routeValues, htmlAttributes));
        }

        public static MvcHtmlString ActionLinkEx(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes)
        {
            return ActionLinkEx(htmlHelper, linkText, actionName, controllerName, protocol, hostName, fragment, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ActionLinkEx(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            Guard.ArgumentNotNull(linkText, "linkText");

            return MvcHtmlString.Create(GenerateLink(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, linkText, null /* routeName */, actionName, controllerName, protocol, hostName, fragment, routeValues, htmlAttributes));
        }

        #endregion

        #region RouteLinkEx

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, object routeValues)
        {
            return RouteLinkEx(htmlHelper, linkText, new RouteValueDictionary(routeValues));
        }

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, RouteValueDictionary routeValues)
        {
            return RouteLinkEx(htmlHelper, linkText, routeValues, new RouteValueDictionary());
        }

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, string routeName)
        {
            return RouteLinkEx(htmlHelper, linkText, routeName, (object)null /* routeValues */ );
        }

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, string routeName, object routeValues)
        {
            return RouteLinkEx(htmlHelper, linkText, routeName, new RouteValueDictionary(routeValues));
        }

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, string routeName, RouteValueDictionary routeValues)
        {
            return RouteLinkEx(htmlHelper, linkText, routeName, routeValues, new RouteValueDictionary());
        }

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, object routeValues, object htmlAttributes)
        {
            return RouteLinkEx(htmlHelper, linkText, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return RouteLinkEx(htmlHelper, linkText, null /* routeName */, routeValues, htmlAttributes);
        }

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, string routeName, object routeValues, object htmlAttributes)
        {
            return RouteLinkEx(htmlHelper, linkText, routeName, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            Guard.ArgumentNotNull(linkText, "linkText");

            return MvcHtmlString.Create(GenerateRouteLink(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, linkText, routeName, routeValues, htmlAttributes));
        }

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, string routeName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes)
        {
            return RouteLinkEx(htmlHelper, linkText, routeName, protocol, hostName, fragment, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RouteLinkEx(this HtmlHelper htmlHelper, string linkText, string routeName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            Guard.ArgumentNotNull(linkText, "linkText");

            return MvcHtmlString.Create(GenerateRouteLink(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, linkText, routeName, protocol, hostName, fragment, routeValues, htmlAttributes));
        }

        #endregion

        #region Private Methods

        private static string EnsureUrl(string url)
        {
            return UrlHelperExtensions.EnsureUrl(url);
        }
        private static string GenerateLink(RequestContext requestContext, RouteCollection routeCollection, string linkText, string routeName, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return GenerateLink(requestContext, routeCollection, linkText, routeName, actionName, controllerName, null, null, null, routeValues, htmlAttributes);
        }
        private static string GenerateLink(RequestContext requestContext, RouteCollection routeCollection, string linkText, string routeName, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return GenerateLinkInternal(requestContext, routeCollection, linkText, routeName, actionName, controllerName, protocol, hostName, fragment, routeValues, htmlAttributes, true);
        }
        private static string GenerateRouteLink(RequestContext requestContext, RouteCollection routeCollection, string linkText, string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return GenerateRouteLink(requestContext, routeCollection, linkText, routeName, null, null, null, routeValues, htmlAttributes);
        }
        private static string GenerateRouteLink(RequestContext requestContext, RouteCollection routeCollection, string linkText, string routeName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return GenerateLinkInternal(requestContext, routeCollection, linkText, routeName, null, null, protocol, hostName, fragment, routeValues, htmlAttributes, false);
        }
        private static string GenerateLinkInternal(RequestContext requestContext, RouteCollection routeCollection, string linkText, string routeName, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool includeImplicitMvcValues)
        {
            string url = EnsureUrl(UrlHelper.GenerateUrl(routeName, actionName, controllerName, protocol, hostName, fragment, routeValues, routeCollection, requestContext, includeImplicitMvcValues));
            var tagBuilder = new TagBuilder("a")
            {
                InnerHtml = (!String.IsNullOrEmpty(linkText)) ? HttpUtility.HtmlEncode(linkText) : String.Empty
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("href", url);
            return tagBuilder.ToString(TagRenderMode.Normal);
        }

        #endregion

    }
}
