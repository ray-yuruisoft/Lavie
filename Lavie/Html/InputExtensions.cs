using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Lavie.Extensions;
using Lavie.Infrastructure.FastReflectionLib;
using Lavie.Utilities.Exceptions;

namespace Lavie.Html
{
    public static class InputExtensions
    {
        #region Static Constructor

        private static readonly IMethodInvoker InputHelperInvoker;

        static InputExtensions()
        {
            MethodInfo inputHelperMethodInfo = typeof(System.Web.Mvc.Html.InputExtensions).GetMethod("InputHelper", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            InputHelperInvoker = new MethodInvoker(inputHelperMethodInfo);
        }

        #endregion

        #region TextBox

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression)
        {
            return TextBoxFor(htmlHelper, model, expression, format: null);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, string format)
        {
            return TextBoxFor(htmlHelper, model, expression, format, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return TextBoxFor(htmlHelper, model, expression, format: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        {
            return TextBoxFor(htmlHelper, model, expression, format: format, htmlAttributes: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return TextBoxFor(htmlHelper, model, expression, format: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, string format, IDictionary<string, object> htmlAttributes)
        {
            var viewData = new ViewDataDictionary<TModel>(model);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, viewData);
            return TextBoxHelper(htmlHelper,
                                 metadata,
                                 metadata.Model,
                                 ExpressionHelper.GetExpressionText(expression),
                                 format,
                                 htmlAttributes);

        }

        private static MvcHtmlString TextBoxHelper(this HtmlHelper htmlHelper, ModelMetadata metadata, object model, string expression, string format, IDictionary<string, object> htmlAttributes)
        {
            var result = (MvcHtmlString)InputHelperInvoker.Invoke(null,
                htmlHelper,
                InputType.Text,
                metadata,
                expression,
                model,
                /*useViewData: */false,
                /*isChecked: */false,
                /*setId： */true,
                /*isExplicitValue: */true,
                /*format: */format,
                /*htmlAttributes: */htmlAttributes);
            return result;

        }

        #endregion

        #region Hidden

        public static MvcHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper htmlHelper,TModel model, Expression<Func<TModel, TProperty>> expression)
        {
            return HiddenFor(htmlHelper, model, expression, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return HiddenFor(htmlHelper,model, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var viewData = new ViewDataDictionary<TModel>(model);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, viewData); 
            return HiddenHelper(htmlHelper,
                                metadata,
                                metadata.Model,
                                false,
                                ExpressionHelper.GetExpressionText(expression),
                                htmlAttributes);
        }

        private static MvcHtmlString HiddenHelper(HtmlHelper htmlHelper, ModelMetadata metadata, object value, bool useViewData, string expression, IDictionary<string, object> htmlAttributes)
        {
            var binaryValue = value as Binary;
            if (binaryValue != null)
            {
                value = binaryValue.ToArray();
            }

            var byteArrayValue = value as byte[];
            if (byteArrayValue != null)
            {
                value = Convert.ToBase64String(byteArrayValue);
            }

            return (MvcHtmlString)InputHelperInvoker.Invoke(null,
                htmlHelper,
                InputType.Hidden,
                metadata,
                expression,
                value,
                /*useViewData: */false,
                /*isChecked: */false,
                /*setId： */true,
                /*isExplicitValue: */true,
                /*format: */null,
                /*htmlAttributes: */htmlAttributes);
        }

        #endregion

        #region Password

        public static MvcHtmlString PasswordFor<TModel, TProperty>(this HtmlHelper htmlHelper,TModel model, Expression<Func<TModel, TProperty>> expression)
        {
            return PasswordFor(htmlHelper,model, expression, htmlAttributes: null);
        }

        public static MvcHtmlString PasswordFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return PasswordFor(htmlHelper, model, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString PasswordFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var viewData = new ViewDataDictionary<TModel>(model);
            return PasswordHelper(htmlHelper,
                                  ModelMetadata.FromLambdaExpression(expression, viewData),
                                  ExpressionHelper.GetExpressionText(expression),
                                  value: null,
                                  htmlAttributes: htmlAttributes);
        }

        private static MvcHtmlString PasswordHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            return (MvcHtmlString)InputHelperInvoker.Invoke(null,
                htmlHelper,
                InputType.Password,
                metadata,
                name,
                value,
                /*useViewData: */false,
                /*isChecked: */false,
                /*setId： */true,
                /*isExplicitValue: */true,
                /*format: */null,
                /*htmlAttributes: */htmlAttributes);
        }

        #endregion

        #region RadioButtonFor

        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper htmlHelper,TModel model, Expression<Func<TModel, TProperty>> expression, object value)
        {
            return RadioButtonFor(htmlHelper, model, expression, value, htmlAttributes: null);
        }

        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, object value, object htmlAttributes)
        {
            return RadioButtonFor(htmlHelper, model, expression, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, object value, IDictionary<string, object> htmlAttributes)
        {
            var viewData = new ViewDataDictionary<TModel>(model);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, viewData);
            return RadioButtonHelper(htmlHelper,
                                     metadata,
                                     metadata.Model,
                                     ExpressionHelper.GetExpressionText(expression),
                                     value,
                                     null /* isChecked */,
                                     htmlAttributes);
        }

        private static MvcHtmlString RadioButtonHelper(HtmlHelper htmlHelper, ModelMetadata metadata, object model, string name, object value, bool? isChecked, IDictionary<string, object> htmlAttributes)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);

            bool explicitValue = isChecked.HasValue;
            if (explicitValue)
            {
                attributes.Remove("checked"); // Explicit value must override dictionary
            }
            else
            {
                string valueString = Convert.ToString(value, CultureInfo.CurrentCulture);
                isChecked = model != null &&
                            !String.IsNullOrEmpty(name) &&
                            String.Equals(model.ToString(), valueString, StringComparison.OrdinalIgnoreCase);
            }

            return (MvcHtmlString)InputHelperInvoker.Invoke(null,
                htmlHelper,
                InputType.Radio,
                metadata,
                name,
                value,
                /*useViewData: */false,
                /*isChecked: */isChecked ?? false,
                /*setId： */true,
                /*isExplicitValue: */true,
                /*format: */null,
                /*htmlAttributes: */attributes);
        }

        #endregion

        #region RadioButtonListWithValueFor

        public static MvcHtmlString RadioButtonListWithValueFor<TModel, TProperty>(this HtmlHelper htmlHelper
            , TModel model
            , Expression<Func<TModel, TProperty>> expression
            , IEnumerable<SelectListItem> selectList
            )
        {
            return htmlHelper.RadioButtonListWithValueFor(model, expression, selectList, null, null);
        }

        public static MvcHtmlString RadioButtonListWithValueFor<TModel, TProperty>(this HtmlHelper htmlHelper
            , TModel model
            , Expression<Func<TModel, TProperty>> expression
            , IEnumerable<SelectListItem> selectList
            , object defaultValue
            )
        {
            return htmlHelper.RadioButtonListWithValueFor(model, expression, selectList, defaultValue, null);
        }

        public static MvcHtmlString RadioButtonListWithValueFor<TModel, TProperty>(this HtmlHelper htmlHelper
            , TModel model
            , Expression<Func<TModel, TProperty>> expression
            , IEnumerable<SelectListItem> selectList
            , object defaultValue
            , object htmlAttributes
            )
        {
            return htmlHelper.RadioButtonListWithValueFor(model, expression, selectList, defaultValue, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RadioButtonListWithValueFor<TModel, TProperty>(this HtmlHelper htmlHelper
            , TModel model
            , Expression<Func<TModel, TProperty>> expression
            , IEnumerable<SelectListItem> selectList
            , object defaultValue
            , IDictionary<string, object> htmlAttributes
            )
        {
            if (selectList == null)
                return MvcHtmlString.Empty;
            //属性名称
            string propertyName = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyName);

            //获取属性值
            var viewData = new ViewDataDictionary<TModel>(model);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, viewData);
            string propertyValue = metadata.Model != null ? metadata.Model.ToString() : null;

            var divTag = new TagBuilder("ul");
            divTag.MergeAttributes(htmlAttributes, true);

            var sl = selectList as SelectList;
            if (sl != null)
            {
                selectList = GetSelectListWithDefaultValue(selectList, sl.SelectedValue, false);

            }
            var selectListItems = selectList as SelectListItem[] ?? selectList.ToArray();
            var listItems = new StringBuilder(selectListItems.Count());
            foreach (var item in selectListItems)
            {
                listItems.AppendFormat("<li>"
                    + "<input type=\"radio\" name=\"{0}\" id=\"{0}_{1}\" value=\"{1}\"{2}/>"
                    + "<label for=\"{0}_{1}\">{3}</label>"
                    + "</li>",
                    fullHtmlFieldName,
                    item.Value,
                    RadioIsChecked(propertyValue, defaultValue, item) ? " checked=\"checked\"" : String.Empty,
                    item.Text);
            }
            divTag.InnerHtml = listItems.ToString();
            return MvcHtmlString.Create(divTag.ToString(TagRenderMode.Normal));
        }

        private static bool RadioIsChecked(string propertyValue, object defaultValue, SelectListItem selectListItem)
        {
            if (propertyValue != null)
                return propertyValue == selectListItem.Value ||
                       (defaultValue != null && propertyValue == defaultValue.ToString());

            return selectListItem.Selected;
        }

        #endregion

        #region RadioButtonListWithValue
        public static MvcHtmlString RadioButtonListWithValue(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
        {
            return RadioButtonListWithValue(htmlHelper, name, selectList, null);
        }
        public static MvcHtmlString RadioButtonListWithValue(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return RadioButtonListWithValue(htmlHelper, name, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        public static MvcHtmlString RadioButtonListWithValue(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            if (selectList == null) return MvcHtmlString.Empty;
            var divTag = new TagBuilder("ul");
            divTag.MergeAttributes(htmlAttributes, true);

            var sl = selectList as SelectList;
            if (sl != null)
            {
                selectList = GetSelectListWithDefaultValue(selectList, sl.SelectedValue, false);

            }
            var selectListItems = selectList as SelectListItem[] ?? selectList.ToArray();
            var listItems = new StringBuilder(selectListItems.Length);
            foreach (var item in selectListItems)
            {
                listItems.AppendFormat("<li>"
                                       + "<input type=\"radio\" name=\"{0}\" id=\"{0}_{1}\" value=\"{1}\"{2}/>"
                                       + "<label for=\"{0}_{1}\">{3}</label>"
                                       + "</li>",
                                        name,
                                        item.Value,
                                        item.Selected ? " checked=\"checked\"" : String.Empty,
                                        item.Text);
            }
            divTag.InnerHtml = listItems.ToString();
            return MvcHtmlString.Create(divTag.ToString(TagRenderMode.Normal));
        }

        #endregion

        #region CheckBoxFor

        public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, bool>> expression)
        {
            return CheckBoxFor(htmlHelper, model, expression, htmlAttributes: null);
        }

        public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            return CheckBoxFor(htmlHelper, model, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, bool>> expression, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var viewData = new ViewDataDictionary<TModel>(model);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, viewData);
            bool? isChecked = null;
            if (metadata.Model != null)
            {
                bool modelChecked;
                if (Boolean.TryParse(metadata.Model.ToString(), out modelChecked))
                {
                    isChecked = modelChecked;
                }
            }

            return CheckBoxHelper(htmlHelper, metadata, ExpressionHelper.GetExpressionText(expression), isChecked, htmlAttributes);
        }

        private static MvcHtmlString CheckBoxHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string name, bool? isChecked, IDictionary<string, object> htmlAttributes)
        {
            RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);

            bool explicitValue = isChecked.HasValue;
            if (explicitValue)
            {
                attributes.Remove("checked"); // Explicit value must override dictionary
            }

            return (MvcHtmlString)InputHelperInvoker.Invoke(null,
                htmlHelper,
                InputType.CheckBox,
                metadata,
                name,
                 "true",
                /*useViewData: */!explicitValue,
                /*isChecked: */isChecked ?? false,
                /*setId： */true,
                /*isExplicitValue: */false,
                /*format: */null,
                /*htmlAttributes: */attributes);
        }

        #endregion

        #region CheckBoxWithValue

        public static MvcHtmlString CheckBoxWithValue(this HtmlHelper htmlHelper, string name, string value, bool isChecked)
        {
            return CheckBoxWithValue(htmlHelper, name, value, isChecked, true);
        }
        public static MvcHtmlString CheckBoxWithValue(this HtmlHelper htmlHelper, string name, string value, bool isChecked, bool enabled)
        {
            return CheckBoxWithValue(htmlHelper, name, value, isChecked, enabled, null);
        }
        public static MvcHtmlString CheckBoxWithValue(this HtmlHelper htmlHelper, string name, string value, bool isChecked, object htmlAttributes)
        {
            return CheckBoxWithValue(htmlHelper, name, value, isChecked, true, htmlAttributes);
        }
        public static MvcHtmlString CheckBoxWithValue(this HtmlHelper htmlHelper, string name, string value, bool isChecked, bool enabled, object htmlAttributes)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);

            if (!enabled)
                attributes.Add("disabled", "disabled");

            return value != null ? CheckBoxWithValue(name, value, isChecked, attributes) : htmlHelper.CheckBox(name, isChecked, attributes);

        }
        public static MvcHtmlString CheckBoxWithValue(string name, string value, bool isChecked, IDictionary<string, object> htmlAttributes)
        {
            return MvcHtmlString.Create(string.Format("<input id=\"{0}_{1}\" name=\"{0}\" value=\"{1}\" {2} type=\"checkbox\" {3}/>", name, value, isChecked ? "checked=\"checked\"" : String.Empty, htmlAttributes.ToAttributeList()));
        }

        #endregion

        #region CheckBoxListWithValue
        public static MvcHtmlString CheckBoxListWithValue(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxListWithValue(htmlHelper, name, selectList, null);
        }
        public static MvcHtmlString CheckBoxListWithValue(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return CheckBoxListWithValue(htmlHelper,name, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        public static MvcHtmlString CheckBoxListWithValue(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            if (selectList == null) return MvcHtmlString.Empty;
            var divTag = new TagBuilder("ul");
            divTag.MergeAttributes(htmlAttributes, true);

            var sl = selectList as SelectList;
            if (sl != null)
            {
                selectList = GetSelectListWithDefaultValue(selectList, sl.SelectedValue, true);

            }
            var selectListItems = selectList as SelectListItem[] ?? selectList.ToArray();
            var listItems = new StringBuilder(selectListItems.Length);
            foreach (var item in selectListItems)
            {
                listItems.AppendFormat("<li>"
                                       + "<input type=\"checkbox\" name=\"{0}\" id=\"{0}_{1}\" value=\"{1}\"{2}/>"
                                       + "<label for=\"{0}_{1}\">{3}</label>"
                                       + "</li>",
                                        name,
                                        item.Value,
                                        item.Selected ? " checked=\"checked\"" : String.Empty,
                                        item.Text);
            }
            divTag.InnerHtml = listItems.ToString();
            return MvcHtmlString.Create(divTag.ToString(TagRenderMode.Normal));
        }

        #endregion

        #region CheckBoxListWithValueFor

        public static MvcHtmlString CheckBoxListWithValueFor<TModel, TProperty>(this HtmlHelper htmlHelper
            , TModel model
            , Expression<Func<TModel, IEnumerable<TProperty>>> expression
            , IEnumerable<SelectListItem> selectList
            ) where TModel : class
        {
            return htmlHelper.CheckBoxListWithValueFor(model, expression, selectList, null);
        }

        public static MvcHtmlString CheckBoxListWithValueFor<TModel, TProperty>(this HtmlHelper htmlHelper
            , TModel model
            , Expression<Func<TModel, IEnumerable<TProperty>>> expression
            , IEnumerable<SelectListItem> selectList
            , object htmlAttributes
            ) where TModel : class
        {
            return htmlHelper.CheckBoxListWithValueFor(model, expression, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxListWithValueFor<TModel, TProperty>(this HtmlHelper htmlHelper
            , TModel model
            , Expression<Func<TModel, IEnumerable<TProperty>>> expression
            , IEnumerable<SelectListItem> selectList
            , IDictionary<string, object> htmlAttributes
            ) where TModel:class 
        {
            if (selectList == null)
                return MvcHtmlString.Empty;

            string propertyName = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyName);

            IEnumerable<TProperty> selectedValues = model != null ? expression.Compile().Invoke(model) : Enumerable.Empty<TProperty>();

            var divTag = new TagBuilder("ul");
            divTag.MergeAttributes(htmlAttributes, true);

            var selectListItems = selectList as SelectListItem[] ?? selectList.ToArray();
            var listItems = new StringBuilder(selectListItems.Length);
            foreach (var item in selectListItems)
            {
                listItems.AppendFormat("<li>"
                    + "<input type=\"checkbox\" name=\"{0}\" id=\"{0}_{1}\" value=\"{1}\"{2}/>"
                    + "<label for=\"{0}_{1}\">{3}</label>"
                    + "</li>",
                    fullHtmlFieldName,
                    item.Value,
                    selectedValues.Any(m => m.ToString() == item.Value) || item.Selected ? " checked=\"checked\"" : String.Empty,
                    item.Text);
            }
            divTag.InnerHtml = listItems.ToString();
            return MvcHtmlString.Create(divTag.ToString(TagRenderMode.Normal));
        }

        #endregion

        #region Private Methods

        private static RouteValueDictionary ToRouteValueDictionary(IDictionary<string, object> dictionary)
        {
            return dictionary == null ? new RouteValueDictionary() : new RouteValueDictionary(dictionary);
        }
        private static MvcHtmlString ToAttributeList(this IEnumerable<KeyValuePair<string, object>> htmlAttributes)
        {
            var sb = new StringBuilder();
            if (htmlAttributes != null)
            {
                const string resultFormat = " {0}=\"{1}\"";
                foreach (var a in htmlAttributes)
                {
                    sb.AppendFormat(resultFormat, a.Key, a.Value);
                }
            }
            return MvcHtmlString.Create(sb.ToString());

        }
        private static IEnumerable<SelectListItem> GetSelectListWithDefaultValue(IEnumerable<SelectListItem> selectList, object defaultValue, bool allowMultiple)
        {
            IEnumerable defaultValues;

            if (allowMultiple)
            {
                defaultValues = defaultValue as IEnumerable;
                if (defaultValues == null || defaultValues is string)
                {
                    defaultValues = new[] {(defaultValue as string) ?? String.Empty};

                    /*
                    throw new InvalidOperationException(
                        String.Format(
                            CultureInfo.CurrentCulture,
                        / *MvcResources.HtmlHelper_SelectExpressionNotEnumerable* /"The parameter '{0}' must evaluate to an IEnumerable when multiple selection is allowed.",
                            "expression"));
                    */
                }
                
            }
            else
            {
                defaultValues = new[] { defaultValue };
            }

            IEnumerable<string> values = from object value in defaultValues
                                         select Convert.ToString(value, CultureInfo.CurrentCulture);

            // ToString() by default returns an enum value's name.  But selectList may use numeric values.
            IEnumerable<string> enumValues = from Enum value in defaultValues.OfType<Enum>()
                                             select value.ToString("d");
            values = values.Concat(enumValues);

            HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
            List<SelectListItem> newSelectList = new List<SelectListItem>();

            foreach (SelectListItem item in selectList)
            {
                item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                newSelectList.Add(item);
            }
            return newSelectList;
        }

        #endregion

    }
}
