using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class ChangeRiderJobTypeInput
    {
        [Required(ErrorMessage = "请输入员工ID")]
        public Int32 StaffID { get; set; }

        [Required(ErrorMessage = "请输入员工职位类型")]
        public Int32 RiderJobTypeID { get; set; }

        public String RequestRemark { get; set; }

    }
}
