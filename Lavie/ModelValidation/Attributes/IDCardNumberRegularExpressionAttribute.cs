using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Lavie.ModelValidation.Attributes
{
    public class IDCardNumberAttribute : ModelClientValidationRegularExpressionAttribute
    {
        /// <summary>
        /// 身份证号码
        /// </summary>
        public IDCardNumberAttribute() : base(@"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$") { }

    }
}
