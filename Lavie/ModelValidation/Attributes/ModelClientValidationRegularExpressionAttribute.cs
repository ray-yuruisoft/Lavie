using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Lavie.ModelValidation.Attributes
{
    public class ModelClientValidationRegularExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {

        public ModelClientValidationRegularExpressionAttribute(string pattern) : base(pattern) { }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationRegexRule(
                    FormatErrorMessage(metadata.GetDisplayName())
                    ,Pattern)
            };
        }
    }
}
