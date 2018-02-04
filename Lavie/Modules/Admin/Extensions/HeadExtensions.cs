using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Extensions;
using Lavie.Html;
using Lavie.Modules.Admin.Models.ViewModels;

namespace Lavie.Modules.Admin.Extensions
{
    public static class HeadExtensions
    {
        #region Title

        public static string SiteTitle<TModel>(this HtmlHelper<TModel> htmlHelper, params string[] items) where TModel : LavieViewModel
        {
            LavieViewModel model = htmlHelper.ViewData.Model;

            if (items == null || !items.Any())
                return model.Site.Title.CleanHtmlTags();

            StringBuilder sb = new StringBuilder(50);
            List<string> itemList = new List<string>(items);

            itemList.Insert(0, model.Site.Title);
            itemList.RemoveAll(s => string.IsNullOrEmpty(s));

            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                sb.Append(itemList[i]);

                if (i > 0)
                    sb.Append(model.Site.PageTitleSeparator);
            }

            return sb.ToString().CleanHtmlTags();
        }

        #endregion

        #region Keywords

        public static MvcHtmlString SiteKeywords<TModel>(this HtmlHelper<TModel> htmlHelper, params string[] items) where TModel : LavieViewModel
        {
            LavieViewModel model = htmlHelper.ViewData.Model;
            string keywords = model.Site.Keywords;
            string format = "<meta name=\"keywords\" content=\"{0}\" />";

            if (items == null || items.Length == 0)
                return MvcHtmlString.Create(string.Format(format, keywords.CleanHtmlTags().CleanHref()));

            StringBuilder sb = new StringBuilder(50);
            List<string> itemList = new List<string>(items);
            itemList.RemoveAll(s => s.IsNullOrWhiteSpace());
            for (int i = 0; i < itemList.Count; i++)
            {
                sb.Append(itemList[i]);

                if (i != itemList.Count - 1)
                    sb.Append(",");
            }
            if (!keywords.IsNullOrWhiteSpace())
                sb.Append("," + model.Site.Keywords);

            return MvcHtmlString.Create(string.Format(format, sb.ToString().CleanHtmlTags().CleanHref()));
        }

        #endregion

        #region Description

        public static MvcHtmlString SiteDescription<TModel>(this HtmlHelper<TModel> htmlHelper) where TModel : LavieViewModel
        {
            return htmlHelper.SiteDescription(null);
        }

        public static MvcHtmlString SiteDescription<TModel>(this HtmlHelper<TModel> htmlHelper, string description) where TModel : LavieViewModel
        {
            if (description == null)
                description = htmlHelper.Encode(htmlHelper.ViewData.Model.Site.Description.CleanHtmlTags());

            return MvcHtmlString.Create(string.Format("<meta name=\"description\" content=\"{0}\" />", description.CleanHtmlTags()));
        }

        #endregion

        #region RenderFavIcon

        public static MvcHtmlString RenderFavIcon<TModel>(this HtmlHelper<TModel> htmlHelper) where TModel : LavieViewModel
        {
            LavieViewModel model = htmlHelper.ViewData.Model;

            if (!string.IsNullOrEmpty(model.Site.FavIconURL))
            {
                var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

                return htmlHelper.HeadLink("shortcut icon", urlHelper.AppPath(model.Site.FavIconURL), null, null);
            }
            return MvcHtmlString.Empty;
        }

        #endregion
    }
}
