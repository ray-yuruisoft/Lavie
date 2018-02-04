using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Lavie.ModelValidation.Attributes
{
    public class PeriodAttribute : ModelClientValidationRegularExpressionAttribute
    {
        public PeriodAttribute() : base(@"^([1-9][0-9]*((,[1-9][0-9]*)*|(-[1-9][0-9]*)*|(,[1-9][0-9]*-[1-9][0-9]*)*)*)$") { }
    }

    // 支持："数字"、"数字,数字"或"数字-数字"的其中一种
    // ^(([1-9][0-9]*)|([1-9][0-9]*-[1-9][0-9]*)|([1-9][0-9]*(,[1-9][0-9]*)+))*$

    // 支持: "数字"、"数字-数字"的以逗号分隔的任意组合
    // ^([1-9][0-9]*((,[1-9][0-9]*)*|(-[1-9][0-9]*)*|(,[1-9][0-9]*-[1-9][0-9]*)*)*)$

    // 支持固定位数: "数字"、"数字-数字"的以逗号分隔的任意组合
    // ^(\d{8}((,\d{8})*|(-\d{8})*|(,\d{8}*-\d{8})*)*)$

    public class NumberSerialPeriodAttribute : ModelClientValidationRegularExpressionAttribute
    {
        public NumberSerialPeriodAttribute(int length) : base(@"^(\d{"+ length + @"}((,\d{" + length + @"})*|(-\d{" + length + @"})*|(,\d{" + length + @"}-\d{" + length + @"})*)*)$") { }
    }

    // 支持："00:00"以逗号分隔的任意组合
    // ^((([0-1][0-9])|([2][0-3])):([0-5][0-9])-(([0-1][0-9])|([2][0-3])):([0-5][0-9]))((,(([0-1][0-9])|([2][0-3])):([0-5][0-9])-(([0-1][0-9])|([2][0-3])):([0-5][0-9]))*)$

    public class HourMinutePeriodAttribute : ModelClientValidationRegularExpressionAttribute
    {
        public HourMinutePeriodAttribute() : base(@"^((([0-1][0-9])|([2][0-3])):([0-5][0-9])-(([0-1][0-9])|([2][0-3])):([0-5][0-9]))((,(([0-1][0-9])|([2][0-3])):([0-5][0-9])-(([0-1][0-9])|([2][0-3])):([0-5][0-9]))*)$") { }
    }

}
