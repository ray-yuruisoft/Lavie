using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Lavie.ModelValidation.Attributes
{
    public class HourMinuteListRegularExpressionAttribute : ModelClientValidationRegularExpressionAttribute
    {
        // 匹配：
        // 单个时段：08:30-09:10
        // 多个时段：08:30-09:10 18:00-18:40
        // 任意空格：08:30-09:10    18:00-18:40   23:10-23:30
        public HourMinuteListRegularExpressionAttribute() :
            base(@"^([01]\d|2[0-3]):([0-5][0-9])-([01]\d|2[0-3]):([0-5][0-9])(( +)([01]\d|2[0-3]):([0-5][0-9])-([01]\d|2[0-3]):([0-5][0-9]))*$") { }
       
    }
}
