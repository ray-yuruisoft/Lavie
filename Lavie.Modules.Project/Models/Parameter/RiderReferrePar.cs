using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class RiderReferrePar
    {
        public Int32 StaffID { get; set; }
        public Int32 RecruitChannelID { get; set; }
        public Int32? RiderReferrerStaffID { get; set; }
        public DateTime? RiderReferrerDate { get; set; }
        [StringLength(100)]
        public string RiderReffererRemark { get; set; }
        [StringLength(200)]
        public string RiderReferrerAttachmentURL { get; set; }
    }
}
