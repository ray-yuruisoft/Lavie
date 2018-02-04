using System.Web.Mvc;
using System.Web.Routing;

namespace Lavie.Html
{
    public static class HeadExtensions
    {
        #region HeadLink

        public static MvcHtmlString HeadLink(this HtmlHelper htmlHelper, string rel, string href, string type, string title=null, object htmlAttributes=null)
        {
            TagBuilder tagBuilder = new TagBuilder("link");

            tagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            if (!string.IsNullOrEmpty(rel))
            {
                tagBuilder.MergeAttribute("rel", rel);
            }
            if (!string.IsNullOrEmpty(href))
            {
                tagBuilder.MergeAttribute("href", href);
            }
            if (!string.IsNullOrEmpty(type))
            {
                tagBuilder.MergeAttribute("type", type);
            }
            if (!string.IsNullOrEmpty(title))
            {
                tagBuilder.MergeAttribute("title", title);
            }

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        #endregion
    }
}
