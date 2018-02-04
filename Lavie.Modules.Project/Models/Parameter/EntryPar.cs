using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class EntryPar
    {
        [Range(1, Int32.MaxValue, ErrorMessage = "StaffID数据格式不正确")]
        public Int32 StaffID { get; set; }

        [StringLength(100)]
        public string EntryRemark { get; set; }

        [StringLength(50)]
        public string WorkNO { get; set; }
      
        public Guid GroupID { get; set; }

        public Guid RoleID { get; set; }

        public Int32? RiderJobTypeID { get; set; }

        public Int32? RecruitChannelID { get; set; }

        public Int32? RiderReferrerStaffID { get; set; }

        public DateTime? RiderReferrerDate { get; set; }

        [StringLength(200)]
        public string RiderReferrerAttachmentURL { get; set; }

        public Int32 JobNatureID { get; set; }

        [StringLength(50)]
        public string ProtocolNO { get; set; }

        public Int32 ProtocolTimeID { get; set; }

        public Int32 ProtocolTypeID { get; set; }

        public DateTime ProtocolSignedDate { get; set; }

        public DateTime ProtocolBeginDate { get; set; }

        public Int32? InsuranceNatureID { get; set; }

        public DateTime? InsuranceStartBuyDate { get; set; }

        public Int32? BankTypeID { get; set; }

        [StringLength(50)]
        public string BankNO { get; set; }

    }

}
