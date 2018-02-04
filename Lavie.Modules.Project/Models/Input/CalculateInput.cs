using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class CalculateInput
    {

        [Required(ErrorMessage = "请提供城市月账单路径")]
        public string CityMonthBillCsvPath { get; set; }


        [Required(ErrorMessage = "请提供评价单路径")]
        public string EvaluateCsvPath { get; set; }

        [Required(ErrorMessage = "请选择所在城市")]
        public Guid GroupID { get; set; }

        [Required(ErrorMessage = "请选择所在年份")]
        public int Year { get; set; }

        [Required(ErrorMessage = "请选择所在月份")]
        public int Month { get; set; }

        [Required(ErrorMessage = "请输入应出勤天数")]
        [Range(1, 31, ErrorMessage = "应出勤天数应大于0或小于31")]
        public int AttendanceDaysDesigned { get; set; }

    }
}
