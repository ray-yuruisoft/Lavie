using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.ModelValidation.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false)]
    public class DateTimeRangeAttribute : ValidationAttribute
    {

        public object Maximum { get; private set; }

        public object Minimum { get; private set; }

        public DateTimeRangeAttribute(DateTime minimum, DateTime maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Empty;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            return false;

        }

    }
}
