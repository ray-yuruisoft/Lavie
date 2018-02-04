using System;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Models;
using Lavie.Modules.Admin.Models.ViewModels;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Admin.Extensions
{
    public static class PromptViewModelExtensions
    {
        public static void AddModelErrors(this PromptViewModel prompt, ModelErrorCollection errors)
        {
            Guard.ArgumentNotNull(prompt, "prompt");
            Guard.ArgumentNotNull(errors, "errors");

            foreach (var error in errors)
            {
                if (!error.ErrorMessage.IsNullOrWhiteSpace())
                    prompt.AddMessage(error.ErrorMessage);
                if (error.Exception != null)
                    prompt.AddMessage(error.Exception.Message);
            }
        }
        public static void AddModelErrors(this PromptViewModel prompt, ModelStateDictionary modelStates)
        {
            Guard.ArgumentNotNull(prompt, "prompt");
            Guard.ArgumentNotNull(modelStates, "modelStates");

            if (modelStates.Values == null)
                throw new ArgumentException("modelStates.Values is null.", "modelStates");

            foreach (var modelState in modelStates.Values)
            {
                prompt.AddModelErrors(modelState.Errors);
            }
        }
        public static void AddMessages(this PromptViewModel prompt,ModelResult modelResult)
        {
            Guard.ArgumentNotNull(prompt, "prompt");
            Guard.ArgumentNotNull(modelResult, "modelResult");

            prompt.AddModelErrors(modelResult.Errors);

            if (modelResult.HasInfo)
                prompt.AddMessages(modelResult.Infos);
        }
    }
}
