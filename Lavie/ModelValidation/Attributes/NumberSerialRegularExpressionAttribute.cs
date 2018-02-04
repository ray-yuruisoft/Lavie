using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Lavie.ModelValidation.Attributes
{
    public class NumberSerialAttribute : ModelClientValidationRegularExpressionAttribute
    {
        /// <summary>
        /// 纯数字，可以是0开头
        /// </summary>
        public NumberSerialAttribute(int length) : base(@"^\d{"+ length  + @"}$") { }

    }
}
