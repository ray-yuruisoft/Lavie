using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.ModelValidation.ModelClientValidationRules;
using Lavie.Utilities.Exceptions;

namespace Lavie.ModelValidation.Attributes
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class CompareAttribute : ValidationAttribute, IClientValidatable
    {
        public string OtherProperty { get; private set; }
        public object ValueToCompare { get; private set; }
        public ValidationCompareOperator Operator { get; private set; }
        public ValidationDataType DataType { get; private set; }

        private const string DefaultErrorMessage = "对{0}和{1}属性或值进行{2}型的{3}比较失败";

        public CompareAttribute(string originalProperty, ValidationCompareOperator compareOperator, ValidationDataType dataType)
            : this(originalProperty,null,compareOperator,dataType)
        {
        }
        public CompareAttribute(string originalProperty, object valueToCompare, ValidationCompareOperator compareOperator, ValidationDataType dataType)
            : base(DefaultErrorMessage)
        {
            OtherProperty = originalProperty;
            ValueToCompare = valueToCompare;
            Operator = compareOperator;
            DataType = dataType;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                name, OtherProperty,DataType.ToString(), Operator.ToString());
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = null;
            if (!String.IsNullOrEmpty(OtherProperty))
            {
                PropertyInfo originalProperty = validationContext.ObjectType.GetProperty(this.OtherProperty);
                object otherValue = originalProperty.GetValue(validationContext.ObjectInstance, null);
                if (!Compare(this.Operator, this.DataType, value, otherValue))                   
                    validationResult = new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }

            if (validationResult==null&&ValueToCompare!=null&&!Compare(this.Operator, this.DataType, value, ValueToCompare))
                validationResult = new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));

            if (validationResult != null)
                return validationResult;

            return ValidationResult.Success;
        }

        private static bool Compare(ValidationCompareOperator compareOperator, ValidationDataType dataType, object value, object otherValue)
        {
            int num = 0;
            try
            {
                switch (dataType)
                {
                    case ValidationDataType.String:
                        num = string.Compare(value != null ? value.ToString() : String.Empty, otherValue!=null?otherValue.ToString():String.Empty, false, CultureInfo.CurrentCulture);
                        break;

                    case ValidationDataType.Integer:
                        num = ((int)value).CompareTo(otherValue);
                        break;

                    case ValidationDataType.Double:
                        num = ((double)value).CompareTo(otherValue);
                        break;

                    case ValidationDataType.Date:
                        num = ((DateTime)value).CompareTo(otherValue);
                        break;

                    case ValidationDataType.Currency:
                        num = ((decimal)value).CompareTo(otherValue);
                        break;
                }
            }
            catch {
                return false;
            }

            switch (compareOperator)
            {
                case ValidationCompareOperator.Equal:
                    return (num == 0);

                case ValidationCompareOperator.NotEqual:
                    return (num != 0);

                case ValidationCompareOperator.GreaterThan:
                    return (num > 0);

                case ValidationCompareOperator.GreaterThanEqual:
                    return (num >= 0);

                case ValidationCompareOperator.LessThan:
                    return (num < 0);

                case ValidationCompareOperator.LessThanEqual:
                    return (num <= 0);
            }
            return true;

        }

        public static string FormatPropertyForClientValidation(string property)
        {
            Guard.ArgumentNotNullOrEmpty(property, "property");

            return ("*." + property);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new ModelClientValidationRule[]{      
                new ModelClientValidationCompareRule(FormatErrorMessage(metadata.GetDisplayName())
                    , !String.IsNullOrEmpty(OtherProperty)?FormatPropertyForClientValidation(OtherProperty):null
                    , ValueToCompare.ToNullableString()
                    , DataType.ToString()
                    , Operator.ToString()
                    )
            };
        }
    }
}
