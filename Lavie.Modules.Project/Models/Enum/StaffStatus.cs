using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public enum StaffStatus
    {
        [Display(Name = "未入职")]
        NoEntry = 1,
        [Display(Name = "在职")]
        OnJob = 2,
        [Display(Name = "离职")]
        Resign = 3,
        [Display(Name = "自离")]
        AutoResign = 4
    }




}
