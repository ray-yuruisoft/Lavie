using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Project.Extentions;

namespace Lavie.Modules.Project.Models
{
    public class TurnoverInput
    {
        [Required(ErrorMessage = "请输入员工ID")]
        public Int32 StaffID { get; set; }

        [Required(ErrorMessage = "请输入调整类型")]
        [EnumRange(new int[] { 4, 5, 6, 7 }, ErrorMessage = "可用范围：“平调、晋升、降级 、骑手职位类型调整”")]
        public Int32 StaffTurnoverTypeID { get; set; }

        [Required(ErrorMessage = "请输入用户组ID")]
        public Guid GroupID { get; set; }

        [Required(ErrorMessage = "请输入职位ID")]
        public Guid RoleID { get; set; }

        public String RequestRemark { get; set; }

    }
}
