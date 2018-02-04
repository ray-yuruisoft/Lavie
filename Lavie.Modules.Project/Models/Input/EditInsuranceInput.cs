using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class EditInsuranceInput
    {
        [Required(ErrorMessage = "请输入员工ID")]
        public Int32 StaffID { get; set; }

        public Int32? InsuranceNatureID { get; set; }

        public DateTime? InsuranceStartBuyDate { get; set; }

        public DateTime? InsuranceStopBuyDate { get; set; }

    }
}
