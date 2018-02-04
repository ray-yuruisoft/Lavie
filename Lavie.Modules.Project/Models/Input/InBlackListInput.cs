using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class InBlackListInput
    {
        [Required(ErrorMessage = "请输入员工ID")]
        public Int32 StaffID { get; set; }

        [Required(ErrorMessage = "请选择是否进入黑名单")]
        public bool IsInBlackList { get; set; }

        public String InBlackListRemark { get; set; }

    }
}
