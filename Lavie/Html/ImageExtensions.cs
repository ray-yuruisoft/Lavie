using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lavie.Extensions
{
    public static class ImageExtensions
    {
        #region Image

        public static MvcHtmlString Image(this HtmlHelper helper, string src)
        {
            return helper.Image(src, null, null);
        }
        public static MvcHtmlString Image(this HtmlHelper helper, string src, string alt)
        {
            return helper.Image(src, alt, null);
        }
        public static MvcHtmlString Image(this HtmlHelper helper, string src, object htmlAttributes)
        {
            return helper.Image(src, null, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString Image(this HtmlHelper helper, string src, string alt, object htmlAttributes)
        {
            return helper.Image(src, alt, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString Image(this HtmlHelper helper, string src, string alt, IDictionary<string, object> htmlAttributes)
        {
            UrlHelper url = new UrlHelper(helper.ViewContext.RequestContext);
            string imageUrl = url.Content(src);
            TagBuilder imageTag = new TagBuilder("img");

            if (!string.IsNullOrEmpty(imageUrl))
            {
                imageTag.MergeAttribute("src", imageUrl);
            }

            if (!string.IsNullOrEmpty(alt))
            {
                imageTag.MergeAttribute("alt", alt);
            }

            imageTag.MergeAttributes(htmlAttributes, true);

            if (imageTag.Attributes.ContainsKey("alt") && !imageTag.Attributes.ContainsKey("title"))
            {
                imageTag.MergeAttribute("title", imageTag.Attributes["alt"] ?? String.Empty);
            }

            return MvcHtmlString.Create(imageTag.ToString(TagRenderMode.SelfClosing));
        }

        #endregion

        #region ImageButton

        public static MvcHtmlString ImageButton<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string src)
        {
            return htmlHelper.ImageButton(id, null, src, null);
        }
        public static MvcHtmlString ImageButton<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string name, string src)
        {

            return htmlHelper.ImageButton(id, name, src, null);
        }
        public static MvcHtmlString ImageButton<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string name, string src, object htmlAttributes)
        {

            return htmlHelper.ImageButton(id, name, src, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString ImageButton<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string src, object htmlAttributes)
        {
            return htmlHelper.ImageButton(id, null, src, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString ImageButton<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string name, string src, IDictionary<string, object> htmlAttributes)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttribute("type", "image");

            if (!string.IsNullOrEmpty(src))
            {
                tagBuilder.MergeAttribute("src", src);
            }
            if (!string.IsNullOrEmpty(id))
            {
                tagBuilder.MergeAttribute("id", id);
            }
            if (!string.IsNullOrEmpty(name))
            {
                tagBuilder.MergeAttribute("name", name);
            }
            else
            {
                tagBuilder.MergeAttribute("name", id);
            }

            tagBuilder.MergeAttributes(htmlAttributes);

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        #endregion

    }
}
