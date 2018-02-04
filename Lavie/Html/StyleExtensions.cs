using System;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Html;

namespace Lavie.Extensions
{
    public static class StyleExtensions
    {
        #region RenderStyleTag

        public static MvcHtmlString RenderStyleTag(this HtmlHelper htmlHelper, string src)
        {
            return htmlHelper.HeadLink("stylesheet", src, "text/css");
        }
        public static MvcHtmlString RenderStyleTag(this HtmlHelper htmlHelper, string type, string src)
        {
            return htmlHelper.HeadLink(type, src, "text/css");
        }

        #endregion
    }
}
