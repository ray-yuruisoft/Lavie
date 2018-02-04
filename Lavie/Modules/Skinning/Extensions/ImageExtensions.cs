using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Extensions;

namespace Lavie.Modules.Skinning.Extensions
{
    public static class ImageExtensions
    {
        #region SkinImageButton

        public static MvcHtmlString SkinImageButton<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string src)
        {
            return htmlHelper.SkinImageButton(id, null, src, null);
        }
        public static MvcHtmlString SkinImageButton<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string name, string src)
        {

            return htmlHelper.SkinImageButton(id, name, src, null);
        }
        public static MvcHtmlString SkinImageButton<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string name, string src, object htmlAttributes)
        {

            return htmlHelper.SkinImageButton(id, name, src, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString SkinImageButton<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string src, object htmlAttributes)
        {
            return htmlHelper.SkinImageButton(id, null, src, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString SkinImageButton<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string name, string src, IDictionary<string, object> htmlAttributes)
        {

            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            src = urlHelper.FilePath(src, htmlHelper.ViewContext);

            return htmlHelper.ImageButton(id, name, src, htmlAttributes);
        }

        #endregion

        #region SkinImage

        public static MvcHtmlString SkinImage<TModel>(this HtmlHelper<TModel> htmlHelper, string src)
        {
            return SkinImage(htmlHelper, src, null, null);
        }
        public static MvcHtmlString SkinImage<TModel>(this HtmlHelper<TModel> htmlHelper, string src, string alt)
        {
            return SkinImage(htmlHelper, src, alt, null);
        }
        public static MvcHtmlString SkinImage<TModel>(this HtmlHelper<TModel> htmlHelper, string src, object htmlAttributes)
        {
            return SkinImage(htmlHelper, src, null, htmlAttributes);
        }
        public static MvcHtmlString SkinImage<TModel>(this HtmlHelper<TModel> htmlHelper, string src, string alt, object htmlAttributes)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            src = urlHelper.FilePath(src, htmlHelper.ViewContext);

            return htmlHelper.Image(src, alt, new RouteValueDictionary(htmlAttributes));
        }

        #endregion
    }
}
