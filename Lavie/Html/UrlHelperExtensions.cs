using System;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using Lavie.Extensions;

namespace Lavie.Html
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// 构建绝对路径
        /// </summary>
        /// <param name="urlHelper">UrlHelper</param>
        /// <param name="relativeUrl">相对路径</param>
        /// <returns></returns>
        public static string AbsolutePath(this UrlHelper urlHelper, string relativeUrl)
        {
            Uri url = urlHelper.RequestContext.HttpContext.Request.Url;
            var uriBuilder = new UriBuilder(url.Scheme, url.Host, url.Port) { Path = relativeUrl };

            string path = uriBuilder.Uri.ToString();

            if (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);

            //TODO: (erikpo) Instead of this workaround, chop off the hash before feeding the url to UrlBuilder, then tack it back on before returning the final url
            path = path.Replace("%23", "#");
            return path;
        }

        /// <summary>
        /// 构建应用程序路径
        /// </summary>
        /// <param name="urlHelper">UrlHelper</param>
        /// <param name="relativeUrl">相对路径(或绝对路径)</param>
        /// <returns></returns>
        public static string AppPath(this UrlHelper urlHelper, string relativePath)
        {
            if (relativePath == null) return null;

            if (relativePath.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) 
                || relativePath.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                return relativePath;

            if (!relativePath.StartsWith("~/"))
            {
                if (!relativePath.StartsWith("/"))
                    relativePath = "/" + relativePath;
                if (!relativePath.StartsWith("~"))
                    relativePath = "~" + relativePath;
            }

            return VirtualPathUtility.ToAbsolute(relativePath, urlHelper.RequestContext.HttpContext.Request.ApplicationPath);
        }

        /// <summary>
        /// 站点首页
        /// </summary>
        /// <param name="urlHelper">UrlHelper</param>
        /// <returns></returns>
        public static string Home(this UrlHelper urlHelper)
        {
            return urlHelper.AppPath("~/");
        }

        /// <summary>
        /// 缩略网址
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="absoluteUrlEncoded"></param>
        /// <returns></returns>
        public static string CompressUrl(this UrlHelper urlHelper, string absoluteUrlEncoded)
        {
            string cacheKey = "tinyurl:" + absoluteUrlEncoded.ToLower();
            Cache cache = urlHelper.RequestContext.HttpContext.Cache;
            string url = (string)cache[cacheKey];

            if (string.IsNullOrEmpty(url))
            {
                try
                {
                    const string urlToSendTo = "http://is.gd/api.php?longurl={0}";
                    var wc = new WebClient();

                    url = wc.DownloadString(string.Format(urlToSendTo, absoluteUrlEncoded));

                    cache.Add(cacheKey, url, null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                }
                catch
                {
                    url = absoluteUrlEncoded;
                }
            }

            return url;
        }

        #region Action

        public static string ActionEx(this UrlHelper helper, string actionName)
        {
            return EnsureUrl(helper.Action(actionName));
        }

        public static string ActionEx(this UrlHelper helper, string actionName, object routeValues)
        {
            return EnsureUrl(helper.Action(actionName, routeValues));
        }

        public static string ActionEx(this UrlHelper helper, string actionName, RouteValueDictionary routeValues)
        {
            return EnsureUrl(helper.Action(actionName, routeValues));
        }

        public static string ActionEx(this UrlHelper helper, string actionName, string controllerName)
        {
            return EnsureUrl(helper.Action(actionName, controllerName));
        }

        public static string ActionEx(this UrlHelper helper, string actionName, string controllerName, object routeValues)
        {
            return EnsureUrl(helper.Action(actionName, controllerName, routeValues));
        }

        public static string ActionEx(this UrlHelper helper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return EnsureUrl(helper.Action(actionName, controllerName, routeValues));
        }

        public static string ActionEx(this UrlHelper helper, string actionName, string controllerName, object routeValues, string protocol)
        {
            return EnsureUrl(helper.Action(actionName, controllerName, routeValues, protocol));
        }

        public static string ActionEx(this UrlHelper helper, string actionName, string controllerName, RouteValueDictionary routeValues, string protocol, string hostName)
        {
            return EnsureUrl(helper.Action(actionName, controllerName, routeValues, protocol, hostName));
        }

        #endregion

        #region RouteUrl

        public static string RouteUrlEx(this UrlHelper helper, object routeValues)
        {
            return helper.RouteUrlEx(null /* routeName */, routeValues);
        }

        public static string RouteUrlEx(this UrlHelper helper, RouteValueDictionary routeValues)
        {
            return helper.RouteUrlEx(null /* routeName */, routeValues);
        }

        public static string RouteUrlEx(this UrlHelper helper, string routeName)
        {
            return helper.RouteUrlEx(routeName, (object)null /* routeValues */);
        }

        public static string RouteUrlEx(this UrlHelper helper, string routeName, object routeValues)
        {
            return helper.RouteUrlEx(routeName, routeValues, null /* protocol */);
        }

        public static string RouteUrlEx(this UrlHelper helper, string routeName, RouteValueDictionary routeValues)
        {
            return helper.RouteUrlEx(routeName, routeValues, null /* protocol */, null /* hostName */);
        }

        public static string RouteUrlEx(this UrlHelper helper, string routeName, object routeValues, string protocol)
        {
            return EnsureUrl(helper.RouteUrl(routeName, routeValues, protocol));
        }

        public static string RouteUrlEx(this UrlHelper helper, string routeName, RouteValueDictionary routeValues, string protocol, string hostName)
        {
            return EnsureUrl(helper.RouteUrl(routeName, routeValues, protocol, hostName));
        }

        #endregion

        #region Private Methods

        internal static string EnsureUrl(string url)
        {
            if (!url.IsNullOrWhiteSpace())
            {
                if (url.StartsWith("/http:/", StringComparison.InvariantCultureIgnoreCase))
                {
                    url = url.Replace("/http:/", "http://");
                }
                else if (url.StartsWith("/https:/", StringComparison.InvariantCultureIgnoreCase))
                {
                    url = url.Replace("/https:/", "https://");
                }
            }
            return url;

        }
        #endregion

    }
}
