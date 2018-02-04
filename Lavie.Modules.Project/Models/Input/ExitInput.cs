using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class ExitInput
    {

        [Required(ErrorMessage = "请输入员工ID")]
        public Int32 StaffID { get; set; }

        [Required(ErrorMessage = "请输入离职时间")]
        public DateTime ExitDate { get; set; }

        [Required(ErrorMessage = "请输入离职原因")]
        public Int32 ExitReasonID { get; set; }

        [StringLength(100)]
        public String ExitRemark { get; set; }


    }
}
