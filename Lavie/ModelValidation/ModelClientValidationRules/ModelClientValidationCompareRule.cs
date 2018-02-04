using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Lavie.ModelValidation.ModelClientValidationRules
{
    public class ModelClientValidationCompareRule : ModelClientValidationRule
    {
        public ModelClientValidationCompareRule(string errorMessage, string otherProperty, string valueToCompare, string dataType, string @operator)
        {
            base.ErrorMessage = errorMessage;
            base.ValidationType = "compareto";
            if (!String.IsNullOrEmpty(otherProperty))
                base.ValidationParameters["other"] = otherProperty;
            if (!String.IsNullOrEmpty(valueToCompare))
                base.ValidationParameters["value"] = valueToCompare;
            base.ValidationParameters["datatype"] = dataType;
            base.ValidationParameters["operator"] = @operator;
        }
    }
}
