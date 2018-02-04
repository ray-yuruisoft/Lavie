using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Lavie.Infrastructure.FastReflectionLib;
using Lavie.Modules.Skinning.ViewEngines;

namespace Lavie.Modules.Skinning.Extensions
{
    public static class PartialExtensions
    {
        #region Static Constructor

        private static IMethodInvoker s_RenderPartialInternal;

        static PartialExtensions()
        {
            MethodInfo renderPartialInternaMethodInfo = typeof(System.Web.Mvc.HtmlHelper).GetMethod("RenderPartialInternal", BindingFlags.Instance | BindingFlags.NonPublic);
            s_RenderPartialInternal = new MethodInvoker(renderPartialInternaMethodInfo);
        }

        #endregion

        #region RenderPartialFromSkin

        public static void RenderPartialFromSkin(this HtmlHelper htmlHelper, string partialViewName)
        {
            htmlHelper.RenderPartialFromSkin(partialViewName, null, htmlHelper.ViewData);
        }

        public static void RenderPartialFromSkin(this HtmlHelper htmlHelper, string partialViewName, ViewDataDictionary viewData)
        {
            htmlHelper.RenderPartialFromSkin(partialViewName, null, viewData);
        }

        public static void RenderPartialFromSkin(this HtmlHelper htmlHelper, string partialViewName, object model)
        {
            htmlHelper.RenderPartialFromSkin(partialViewName, model, htmlHelper.ViewData);
        }

        public static void RenderPartialFromSkin(this HtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData)
        {
            htmlHelper.RenderPartialFromSkin(partialViewName, model, viewData, "LavieViewEngines");
        }

        #endregion

        #region PartialFromSkin

        public static MvcHtmlString PartialFromSkin(this HtmlHelper htmlHelper, string partialViewName)
        {
            return htmlHelper.PartialFromSkin(partialViewName, null, htmlHelper.ViewData);
        }

        public static MvcHtmlString PartialFromSkin(this HtmlHelper htmlHelper, string partialViewName, ViewDataDictionary viewData)
        {
            return htmlHelper.PartialFromSkin(partialViewName, null, viewData);
        }

        public static MvcHtmlString PartialFromSkin(this HtmlHelper htmlHelper, string partialViewName, object model)
        {
            return htmlHelper.PartialFromSkin(partialViewName, model, htmlHelper.ViewData);
        }

        public static MvcHtmlString PartialFromSkin(this HtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData)
        {
            return htmlHelper.PartialFromSkin(partialViewName, model, viewData, "LavieViewEngines");
        }

        #endregion

        #region Private Static Methods

        private static void RenderPartialFromSkin(this HtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData, string viewEngineCollectionName)
        {
            s_RenderPartialInternal.Invoke(
                htmlHelper,
                partialViewName,
                viewData,
                model,
                htmlHelper.ViewContext.Writer,
                GetViewEngines(htmlHelper, model, viewData, viewEngineCollectionName)
                );
        }

        private static MvcHtmlString PartialFromSkin(this HtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData, string viewEngineCollectionName)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                s_RenderPartialInternal.Invoke(
                    htmlHelper,
                    partialViewName,
                    viewData,
                    model,
                    writer,
                    GetViewEngines(htmlHelper, model, viewData, viewEngineCollectionName)
                    );

                return MvcHtmlString.Create(writer.ToString());
            }
        }

        private static ViewEngineCollection GetViewEngines(HtmlHelper htmlHelper,object model, ViewDataDictionary viewData, string viewEngineCollectionName)
        {
            ViewDataDictionary newViewData;
            if (model == null)
                newViewData = viewData == null ? new ViewDataDictionary(htmlHelper.ViewData) : new ViewDataDictionary(viewData);
            else
                newViewData = viewData == null ? new ViewDataDictionary(model) : new ViewDataDictionary(viewData) { Model = model };


            IEnumerable<ILavieViewEngine> ve = newViewData[viewEngineCollectionName] as IEnumerable<ILavieViewEngine>;
            if (ve != null)
                return new ViewEngineCollection(ve.Cast<IViewEngine>().ToList());
            else
                return System.Web.Mvc.ViewEngines.Engines;
        }

        #endregion
    }
}
