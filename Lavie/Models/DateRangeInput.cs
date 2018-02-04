using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.ModelValidation.Attributes;

namespace Lavie.Models
{
    public class DateRangeInput
    {
        [Required(ErrorMessage = "请选择开始日期")]
        [DisplayName("开始日期")]
        public DateTime BeginDate { get; set; }

        [Required(ErrorMessage = "请选择结束日期")]
        [Lavie.ModelValidation.Attributes.Compare("BeginDate", ValidationCompareOperator.GreaterThanEqual, ValidationDataType.Date, ErrorMessage = "结束日期不能小于开始日期")]
        [DisplayName("结束日期")]
        public DateTime EndDate { get; set; }
    }
}
