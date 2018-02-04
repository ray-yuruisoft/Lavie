using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Lavie.Infrastructure.FastReflectionLib;
using System.Globalization;

namespace Lavie.Html
{
    public static class TextAreaExtensions
    {
        #region Static Constructor

        private const int TextAreaRows = 2;
        private const int TextAreaColumns = 20;
        private static readonly Dictionary<string, object> implicitRowsAndColumns = new Dictionary<string, object> {
            { "rows", TextAreaRows.ToString(CultureInfo.InvariantCulture) },
            { "cols", TextAreaColumns.ToString(CultureInfo.InvariantCulture) },
        };

        private static readonly IMethodInvoker TextAreaHelperInvoker;
        static TextAreaExtensions()
        {
            MethodInfo textAreaHelperMethodInfo = typeof(System.Web.Mvc.Html.TextAreaExtensions).GetMethod("TextAreaHelper", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            TextAreaHelperInvoker = new MethodInvoker(textAreaHelperMethodInfo);
        }

        #endregion

        #region TextArea

        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper htmlHelper,TModel model, Expression<Func<TModel, TProperty>> expression)
        {
            return TextAreaFor(htmlHelper,model, expression, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return TextAreaFor(htmlHelper, model, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var viewData = new ViewDataDictionary<TModel>(model);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, viewData);

            return (MvcHtmlString)TextAreaHelperInvoker.Invoke(
                null
                , htmlHelper
                , metadata
                , ExpressionHelper.GetExpressionText(expression)
                , implicitRowsAndColumns
                , htmlAttributes, null/*innerHtmlPrefix*/);
        }

        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, int rows, int columns, object htmlAttributes)
        {
            return TextAreaFor(htmlHelper, model, expression, rows, columns, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, int rows, int columns, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            var viewData = new ViewDataDictionary<TModel>(model);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, viewData);

            return (MvcHtmlString)TextAreaHelperInvoker.Invoke(
                null
                , htmlHelper
                , metadata
                , ExpressionHelper.GetExpressionText(expression)
                , GetRowsAndColumnsDictionary(rows, columns)
                , htmlAttributes, null/*innerHtmlPrefix*/);

        }

        #endregion

        #region Private Methods

        private static Dictionary<string, object> GetRowsAndColumnsDictionary(int rows, int columns)
        {
            if (rows < 0)
            {
                throw new ArgumentOutOfRangeException("rows");
            }
            if (columns < 0)
            {
                throw new ArgumentOutOfRangeException("columns");
            }
            var dictionary = new Dictionary<string, object>();
            if (rows > 0)
            {
                dictionary.Add("rows", rows.ToString(CultureInfo.InvariantCulture));
            }
            if (columns > 0)
            {
                dictionary.Add("cols", columns.ToString(CultureInfo.InvariantCulture));
            }
            return dictionary;
        }

        #endregion
    }

}
