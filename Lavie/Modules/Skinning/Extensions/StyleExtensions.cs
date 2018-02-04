using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Html;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Skinning.Extensions
{
    public static class StyleExtensions
    {
        #region RenderStyleTagFromSkin

        public static MvcHtmlString RenderStyleTagFromSkin(this HtmlHelper htmlHelper, string src, string releasePath=null)
        {
            Guard.ArgumentNotNull(htmlHelper, "htmlHelper");
            Guard.ArgumentNotNull(src, "src");

#if !DEBUG
            if (!string.IsNullOrEmpty(releasePath))
                src = releasePath;
#endif

            if (!(src.StartsWith("http://") || src.StartsWith("https://")))
            {
                UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

                src = !src.StartsWith("/")
                    ? urlHelper.FilePath(src, htmlHelper.ViewContext)
                    : urlHelper.AppPath(src);
            }

            return htmlHelper.RenderStyleTag(src);
        }

        #endregion
    }
}
