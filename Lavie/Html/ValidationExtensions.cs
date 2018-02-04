using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Infrastructure.FastReflectionLib;
using System.Globalization;
using System.Web;
using System.Linq;

namespace Lavie.Html
{
    public static class ValidationExtensions
    {
        #region Static Constructor

        /*
        private static MethodInvoker s_ValidationMessageHelperInvoker;
        static ValidationExtensions()
        {
            MethodInfo validationMessageMethodInfo = typeof(ValidationExtensions).GetMethod("ValidationMessageHelper", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            s_ValidationMessageHelperInvoker = new MethodInvoker(validationMessageMethodInfo);
        }
        */

        #endregion

        #region ValidationMessage

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression)
        {
            return ValidationMessageFor(htmlHelper, model, expression, validationMessage: null, htmlAttributes: new RouteValueDictionary());
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, string validationMessage)
        {
            return ValidationMessageFor(htmlHelper, model, expression, validationMessage, new RouteValueDictionary());
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, string validationMessage, object htmlAttributes)
        {
            return ValidationMessageFor(htmlHelper, model, expression, validationMessage, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model, Expression<Func<TModel, TProperty>> expression, string validationMessage, IDictionary<string, object> htmlAttributes)
        {
            return ValidationMessageFor(htmlHelper,
                model,
                                        expression,
                                        validationMessage,
                                        htmlAttributes,
                                        tag: null);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model,
                                                                            Expression<Func<TModel, TProperty>> expression,
                                                                            string validationMessage,
                                                                            string tag)
        {
            return ValidationMessageFor(htmlHelper,
                model,
                                        expression,
                                        validationMessage,
                                        htmlAttributes: null,
                                        tag: tag);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model,
                                                                            Expression<Func<TModel, TProperty>> expression,
                                                                            string validationMessage,
                                                                            object htmlAttributes,
                                                                            string tag)
        {
            return ValidationMessageFor(htmlHelper,
                model,
                                        expression,
                                        validationMessage,
                                        HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes),
                                        tag);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper htmlHelper, TModel model,
                                                                            Expression<Func<TModel, TProperty>> expression,
                                                                            string validationMessage,
                                                                            IDictionary<string, object> htmlAttributes,
                                                                            string tag)
        {
            var viewData = new ViewDataDictionary<TModel>(model);
            return ValidationMessageHelper(htmlHelper,
                                           ModelMetadata.FromLambdaExpression(expression, viewData),
                                           ExpressionHelper.GetExpressionText(expression),
                                           validationMessage,
                                           htmlAttributes,
                                           tag);
        }

        private static MvcHtmlString ValidationMessageHelper(this HtmlHelper htmlHelper, ModelMetadata modelMetadata, string expression, string validationMessage, IDictionary<string, object> htmlAttributes, string tag)
        {
            string modelName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expression);
            FormContext formContext = htmlHelper.ViewContext.GetFormContextForClientValidation();

            if (!htmlHelper.ViewData.ModelState.ContainsKey(modelName) && formContext == null)
            {
                return null;
            }

            ModelState modelState = htmlHelper.ViewData.ModelState[modelName];
            ModelErrorCollection modelErrors = (modelState == null) ? null : modelState.Errors;
            ModelError modelError = (((modelErrors == null) || (modelErrors.Count == 0)) ? null : modelErrors.FirstOrDefault(m => !String.IsNullOrEmpty(m.ErrorMessage)) ?? modelErrors[0]);

            if (modelError == null && formContext == null)
            {
                return null;
            }

            if (String.IsNullOrEmpty(tag))
            {
                tag = htmlHelper.ViewContext.ValidationMessageElement;
            }

            TagBuilder builder = new TagBuilder(tag);
            builder.MergeAttributes(htmlAttributes);
            builder.AddCssClass((modelError != null) ? HtmlHelper.ValidationMessageCssClassName : HtmlHelper.ValidationMessageValidCssClassName);

            if (!String.IsNullOrEmpty(validationMessage))
            {
                builder.SetInnerText(validationMessage);
            }
            else if (modelError != null)
            {
                builder.SetInnerText(GetUserErrorMessageOrDefault(htmlHelper.ViewContext.HttpContext, modelError, modelState));
            }

            if (formContext != null)
            {
                bool replaceValidationMessageContents = String.IsNullOrEmpty(validationMessage);

                if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                {
                    builder.MergeAttribute("data-valmsg-for", modelName);
                    builder.MergeAttribute("data-valmsg-replace", replaceValidationMessageContents.ToString().ToLowerInvariant());
                }
                else
                {
                    FieldValidationMetadata fieldMetadata = ApplyFieldValidationMetadata(htmlHelper, modelMetadata, modelName);
                    // rules will already have been written to the metadata object
                    fieldMetadata.ReplaceValidationMessageContents = replaceValidationMessageContents; // only replace contents if no explicit message was specified

                    // client validation always requires an ID
                    builder.GenerateId(modelName + "_validationMessage");
                    fieldMetadata.ValidationMessageId = builder.Attributes["id"];
                }
            }
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
            //return builder.ToMvcHtmlString(TagRenderMode.Normal);
        }


        #endregion

        #region Private Methods

        private static FormContext GetFormContextForClientValidation(this ViewContext viewContext)
        {
            if (!viewContext.ClientValidationEnabled)
            {
                return null;
            }
            return viewContext.FormContext;
        }
        private static string GetUserErrorMessageOrDefault(HttpContextBase httpContext, ModelError error, ModelState modelState)
        {
            if (!String.IsNullOrEmpty(error.ErrorMessage))
            {
                return error.ErrorMessage;
            }
            if (modelState == null)
            {
                return null;
            }

            string attemptedValue = (modelState.Value != null) ? modelState.Value.AttemptedValue : null;
            //Edited
            //return String.Format(CultureInfo.CurrentCulture, GetInvalidPropertyValueResource(httpContext), attemptedValue);
            return String.Format(CultureInfo.CurrentCulture, "The value '{0}' is invalid.", attemptedValue);
        }
        private static FieldValidationMetadata ApplyFieldValidationMetadata(HtmlHelper htmlHelper, ModelMetadata modelMetadata, string modelName)
        {
            FormContext formContext = htmlHelper.ViewContext.FormContext;
            FieldValidationMetadata fieldMetadata = formContext.GetValidationMetadataForField(modelName, true /* createIfNotFound */);

            // write rules to context object
            IEnumerable<ModelValidator> validators = ModelValidatorProviders.Providers.GetValidators(modelMetadata, htmlHelper.ViewContext);
            foreach (ModelClientValidationRule rule in validators.SelectMany(v => v.GetClientValidationRules()))
            {
                fieldMetadata.ValidationRules.Add(rule);
            }

            return fieldMetadata;
        }

        #endregion
    }
}
