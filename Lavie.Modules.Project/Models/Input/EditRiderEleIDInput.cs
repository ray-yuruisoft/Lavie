using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class EditRiderEleIDInput
    {
        [Required(ErrorMessage = "请输入员工ID")]
        public Int32 StaffID { get; set; }
        [Required(ErrorMessage = "骑手饿了么ID")]
        public Int32 RiderEleID { get; set; }

    }
}
