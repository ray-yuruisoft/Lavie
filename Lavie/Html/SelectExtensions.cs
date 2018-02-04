using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Lavie.Extensions;
using Lavie.Utilities.Exceptions;

namespace Lavie.Html
{
    public static class SelectExtensions
    {
        #region Static Constructor

        /*
        private static IMethodInvoker s_DropDownListHelperInvoker;
        static SelectExtensions()
        {
            MethodInfo dropDownListHelperMethodInfo = typeof(System.Web.Mvc.Html.SelectExtensions).GetMethod("DropDownListHelper", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            s_DropDownListHelperInvoker = new MethodInvoker(dropDownListHelperMethodInfo);
        }
        */

        #endregion

        #region DropDownList

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return DropDownListFor(htmlHelper, model, expression, selectList, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return DropDownListFor(htmlHelper, model, expression, selectList, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return DropDownListFor(htmlHelper, model, expression, selectList, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return DropDownListFor(htmlHelper, model, expression, selectList, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return DropDownListFor(htmlHelper, model, expression, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var viewData = new ViewDataDictionary<TModel>(model);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, viewData);

            return DropDownListHelper(htmlHelper, metadata, ExpressionHelper.GetExpressionText(expression), selectList, optionLabel, htmlAttributes);
        }

        #endregion

        #region ListBox

        public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return ListBoxFor(htmlHelper, model, expression, selectList, null /* htmlAttributes */);
        }

        public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return ListBoxFor(htmlHelper, model, expression, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper  htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var viewData = new ViewDataDictionary<TModel>(model);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, viewData);

            return ListBoxHelper(htmlHelper,
                                 metadata,
                                 ExpressionHelper.GetExpressionText(expression),
                                 selectList,
                                 htmlAttributes);
        }



        #endregion

        #region EnumDropDownList

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper htmlHelper, TModel model,
            Expression<Func<TModel, TEnum>> expression)
        {
            return EnumDropDownListFor(htmlHelper, model, expression, optionLabel: null);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper htmlHelper, TModel model,
            Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            return EnumDropDownListFor(htmlHelper, model, expression, optionLabel: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper htmlHelper, TModel model,
            Expression<Func<TModel, TEnum>> expression, IDictionary<string, object> htmlAttributes)
        {
            return EnumDropDownListFor(htmlHelper, model, expression, optionLabel: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper htmlHelper, TModel model,
            Expression<Func<TModel, TEnum>> expression, string optionLabel)
        {
            return EnumDropDownListFor(htmlHelper, model, expression, optionLabel, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper htmlHelper, TModel model,
            Expression<Func<TModel, TEnum>> expression, string optionLabel, object htmlAttributes)
        {
            return EnumDropDownListFor(htmlHelper, model, expression, optionLabel,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper htmlHelper, TModel model,
            Expression<Func<TModel, TEnum>> expression, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
                //throw Error.ArgumentNull("expression");
            }

            var viewData = new ViewDataDictionary<TModel>(model);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, viewData); if (metadata == null)
            {
                throw new ArgumentException("Unable to determine ModelMetadata for expression '{0}'.".FormatWith(expression.ToString()), "expression");
                //throw Error.Argument("expression", "Unable to determine ModelMetadata for expression '{0}'",expression.ToString());
            }

            if (metadata.ModelType == null)
            {
                throw new ArgumentException("Unable to determine type of expression '{0}'.".FormatWith(expression.ToString()), "expression");
                //throw Error.Argument("expression", MvcResources.SelectExtensions_InvalidExpressionParameterNoModelType,expression.ToString());
            }

            if (!EnumHelper.IsValidForEnumHelper(metadata.ModelType))
            {
                string formatString;
                if (/*EnumHelper.*/HasFlags(metadata.ModelType))
                {
                    formatString = "Return type '{0}' is not supported. Type must not have a '{1}' attribute.";
                    //formatString = MvcResources.SelectExtensions_InvalidExpressionParameterTypeHasFlags;
                }
                else
                {
                    formatString = "Return type '{0}' is not supported.";
                    //formatString = MvcResources.SelectExtensions_InvalidExpressionParameterType;
                }

                throw new ArgumentException(formatString.FormatWith(metadata.ModelType.FullName, "Flags"), "expression");
                //throw Error.Argument("expression", formatString, metadata.ModelType.FullName, "Flags");
            }

            // Run through same processing as SelectInternal() to determine selected value and ensure it is included
            // in the select list.
            string expressionName = ExpressionHelper.GetExpressionText(expression);
            string expressionFullName =
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionName);
            Enum currentValue = null;
            if (!String.IsNullOrEmpty(expressionFullName))
            {
                currentValue = GetModelStateValue(htmlHelper,expressionFullName, metadata.ModelType) as Enum;
            }

            if (currentValue == null && !String.IsNullOrEmpty(expressionName))
            {
                // Ignore any select list (enumerable with this name) in the view data
                currentValue = htmlHelper.ViewData.Eval(expressionName) as Enum;
            }

            if (currentValue == null)
            {
                currentValue = metadata.Model as Enum;
            }

            IList<SelectListItem> selectList = EnumHelper.GetSelectList(metadata.ModelType, currentValue);
            if (!String.IsNullOrEmpty(optionLabel) && selectList.Count != 0 && String.IsNullOrEmpty(selectList[0].Text))
            {
                // Were given an optionLabel and the select list has a blank initial slot.  Combine.
                selectList[0].Text = optionLabel;

                // Use the option label just once; don't pass it down the lower-level helpers.
                optionLabel = null;
            }

            return DropDownListHelper(htmlHelper, metadata, expressionName, selectList, optionLabel, htmlAttributes);
        }

        #endregion

        #region Private Methods
        // EnumHelper提取的方法
        private static bool HasFlags(Type type)
        {

            Type checkedType = Nullable.GetUnderlyingType(type) ?? type;
            return HasFlagsInternal(checkedType);
        }

        private static bool HasFlagsInternal(Type type)
        {
            var attribute = type.GetCustomAttribute<FlagsAttribute>(inherit: false);
            return attribute != null;
        }

        // HtmlHelper提取的方法
        private static object GetModelStateValue(HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState state;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out state) && (state.Value != null))
            {
                return state.Value.ConvertTo(destinationType, null);
            }
            return null;
        }

        private static MvcHtmlString DropDownListHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return SelectInternal(htmlHelper, metadata, optionLabel, expression, selectList, allowMultiple: false, htmlAttributes: htmlAttributes);
        }
        private static MvcHtmlString ListBoxHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return SelectInternal(htmlHelper, metadata, optionLabel: null, name: name, selectList: selectList, allowMultiple: true, htmlAttributes: htmlAttributes);
        }

        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, ModelMetadata metadata,
            string optionLabel, string name, IEnumerable<SelectListItem> selectList, bool allowMultiple,
            IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "name");
            }

            bool usedViewData = false;

            // If we got a null selectList, try to use ViewData to get the list of items.
            if (selectList == null)
            {
                selectList = htmlHelper.GetSelectData(name);
                usedViewData = true;
            }

            object defaultValue = (allowMultiple) ? GetModelStateValue(htmlHelper,fullName, typeof(string[])) : GetModelStateValue(htmlHelper,fullName, typeof(string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (defaultValue == null && !String.IsNullOrEmpty(name))
            {
                if (!usedViewData)
                {
                    defaultValue = htmlHelper.ViewData.Eval(name);
                }
                else if (metadata != null)
                {
                    defaultValue = metadata.Model;
                }
            }

            if (defaultValue != null)
            {
                selectList = GetSelectListWithDefaultValue(selectList, defaultValue, allowMultiple);
            }

            // Convert each ListItem to an <option> tag and wrap them with <optgroup> if requested.
            StringBuilder listItemBuilder = BuildItems(optionLabel, selectList);

            TagBuilder tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
            tagBuilder.GenerateId(fullName);
            if (allowMultiple)
            {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

            //return tagBuilder.ToMvcHtmlString(TagRenderMode.Normal);
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        private static StringBuilder BuildItems(string optionLabel, IEnumerable<SelectListItem> selectList)
        {
            StringBuilder listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
            {
                listItemBuilder.AppendLine(ListItemToOption(new SelectListItem()
                {
                    Text = optionLabel,
                    Value = String.Empty,
                    Selected = false
                }));
            }

            // Group items in the SelectList if requested.
            // Treat each item with Group == null as a member of a unique group
            // so they are added according to the original order.
            IEnumerable<IGrouping<int, SelectListItem>> groupedSelectList = selectList.GroupBy<SelectListItem, int>(
                i => (i.Group == null) ? i.GetHashCode() : i.Group.GetHashCode());
            foreach (IGrouping<int, SelectListItem> group in groupedSelectList)
            {
                SelectListGroup optGroup = group.First().Group;

                // Wrap if requested.
                TagBuilder groupBuilder = null;
                if (optGroup != null)
                {
                    groupBuilder = new TagBuilder("optgroup");
                    if (optGroup.Name != null)
                    {
                        groupBuilder.MergeAttribute("label", optGroup.Name);
                    }
                    if (optGroup.Disabled)
                    {
                        groupBuilder.MergeAttribute("disabled", "disabled");
                    }
                    listItemBuilder.AppendLine(groupBuilder.ToString(TagRenderMode.StartTag));
                }

                foreach (SelectListItem item in group)
                {
                    listItemBuilder.AppendLine(ListItemToOption(item));
                }

                if (optGroup != null)
                {
                    listItemBuilder.AppendLine(groupBuilder.ToString(TagRenderMode.EndTag));
                }
            }

            return listItemBuilder;
        }

        private static IEnumerable<SelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name)
        {
            object o = null;
            if (htmlHelper.ViewData != null)
            {
                o = htmlHelper.ViewData.Eval(name);
            }
            if (o == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                    /*MvcResources.HtmlHelper_MissingSelectData*/"There is no ViewData item of type '{1}' that has the key '{0}'.",
                        name,
                        "IEnumerable<SelectListItem>"));
            }
            var selectList = o as IEnumerable<SelectListItem>;
            if (selectList == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                    /*MvcResources.HtmlHelper_WrongSelectDataType*/"The ViewData item that has the key '{0}' is of type '{1}' but must be of type '{2}'.",
                        name,
                        o.GetType().FullName,
                        "IEnumerable<SelectListItem>"));
            }
            return selectList;
        }

        internal static string ListItemToOption(SelectListItem item)
        {
            TagBuilder builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            if (item.Disabled)
            {
                builder.Attributes["disabled"] = "disabled";
            }
            return builder.ToString(TagRenderMode.Normal);
        }

        private static IEnumerable<SelectListItem> GetSelectListWithDefaultValue(IEnumerable<SelectListItem> selectList, object defaultValue, bool allowMultiple)
        {
            IEnumerable defaultValues;

            if (allowMultiple)
            {
                defaultValues = defaultValue as IEnumerable;
                if (defaultValues == null || defaultValues is string)
                {
                    throw new InvalidOperationException(
                        String.Format(
                            CultureInfo.CurrentCulture,
                        /*MvcResources.HtmlHelper_SelectExpressionNotEnumerable*/"The parameter '{0}' must evaluate to an IEnumerable when multiple selection is allowed.",
                            "expression"));
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
