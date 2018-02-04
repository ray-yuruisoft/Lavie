using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class RecruitChannelInput
    {
        [Required(ErrorMessage = "StaffID不能为空")]
        public Int32 StaffID { get; set; }

        [Required(ErrorMessage = "JobNatureID不能为空")]
        public Int32 JobNatureID { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "ProtocolNO不能为空")]
        public string ProtocolNO { get; set; }

        [Required(ErrorMessage = "ProtocolTimeID不能为空")]
        public Int32 ProtocolTimeID { get; set; }

        [Required(ErrorMessage = "ProtocolTypeID不能为空")]
        public Int32 ProtocolTypeID { get; set; }

        [Required(ErrorMessage = "ProtocolSignedDate不能为空")]
        public DateTime ProtocolSignedDate { get; set; }

        [Required(ErrorMessage = "ProtocolBeginDate不能为空")]
        public DateTime ProtocolBeginDate { get; set; }

        [Required(ErrorMessage = "ProtocolEndDate不能为空")]
        public DateTime ProtocolEndDate { get; set; }

    }
}
