using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Html;
using Lavie.Extensions;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Skinning.Extensions
{
    public static class ScriptExtensions
    {
        #region RenderScriptTagFromSkin

        public static MvcHtmlString RenderScriptTagFromSkin(this HtmlHelper htmlHelper, string src, string releasePath=null)
        {
            Guard.ArgumentNotNull(htmlHelper, "htmlHelper");
            Guard.ArgumentNotNull(src, "src");

#if !DEBUG
            if (!string.IsNullOrEmpty(releasePath))
                src = releasePath;
#endif

            if (!(src.StartsWith("http://") || src.StartsWith("https://")))
            {
                var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

                src = !src.StartsWith("/")
                    ? urlHelper.FilePath(src, htmlHelper.ViewContext)
                    : urlHelper.AppPath(src);
            }
            return htmlHelper.RenderScriptTag(src);
        }

        #endregion

    }
}
