using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Lavie.ModelValidation.ModelClientValidationRules
{
    public class ModelClientValidationByteLengthRule : ModelClientValidationRule
    {
        public ModelClientValidationByteLengthRule(string errorMessage, int minimumLength, int maximumLength)
        {
            base.ErrorMessage = errorMessage;
            base.ValidationType = "byteLength";
            if (minimumLength != 0)
            {
                base.ValidationParameters["min"] = minimumLength;
            }
            if (maximumLength != Int32.MaxValue)
            {
                base.ValidationParameters["max"] = maximumLength;
            }
        }
    }
}
