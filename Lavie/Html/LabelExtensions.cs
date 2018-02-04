using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using Lavie.Infrastructure.FastReflectionLib;

namespace Lavie.Html
{
    public static class LabelExtensions
    {
        #region Static Constructor

        private static readonly IMethodInvoker LabelHelperInvoker;
        static LabelExtensions()
        {
            MethodInfo labelHelperMethodInfo = typeof(System.Web.Mvc.Html.LabelExtensions).GetMethod("LabelHelper", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            LabelHelperInvoker = new MethodInvoker(labelHelperMethodInfo);
        }

        #endregion

        #region Label

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper html, TModel model, Expression<Func<TModel, TValue>> expression)
        {
            return LabelFor<TModel, TValue>(html, model, expression, labelText: null);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper html, TModel model, Expression<Func<TModel, TValue>> expression, string labelText)
        {
            return LabelFor(html, model,expression, labelText, htmlAttributes: null, metadataProvider: null);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper html, TModel model, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return LabelFor(html, model, expression, labelText: null, htmlAttributes: htmlAttributes, metadataProvider: null);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper html, TModel model, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            return LabelFor(html, model, expression, labelText: null, htmlAttributes: htmlAttributes, metadataProvider: null);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper html, TModel model, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes)
        {
            return LabelFor(html, model, expression, labelText, htmlAttributes, metadataProvider: null);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper html, TModel model, Expression<Func<TModel, TValue>> expression, string labelText, IDictionary<string, object> htmlAttributes)
        {
            return LabelFor(html, model, expression, labelText, htmlAttributes, metadataProvider: null);
        }
        internal static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper html, TModel model, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes, ModelMetadataProvider metadataProvider)
        {
            return LabelFor(html,
                            model,
                            expression,
                            labelText,
                            HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes),
                            metadataProvider);
        }

        internal static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper html, TModel model, Expression<Func<TModel, TValue>> expression, string labelText, IDictionary<string, object> htmlAttributes, ModelMetadataProvider metadataProvider)
        {
            var viewData = new ViewDataDictionary<TModel>(model);
            return (MvcHtmlString)LabelHelperInvoker.Invoke(
                null
                , html
                , ModelMetadata.FromLambdaExpression(expression, viewData)
                , ExpressionHelper.GetExpressionText(expression)
                , labelText, htmlAttributes);
        }

        #endregion

    }
}
