using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Project.Models
{
    public class StaffBaseList : StaffInfoBaseList
    {
        public UserBase User { get; set; }

        public EducationInfo Education { get; set; }

        public RiderJobTypeInfo RiderJobType { get; set; }

        [StringLength(100)]
        public string EntryRemark { get; set; }

        [StringLength(50)]
        public string WorkNO { get; set; }

        public RecruitChannelBaseInfo RecruitChannel { get; set; }

      //  public Int32? RiderReferrerStaffID { get; set; }

        public StaffInfoBase RiderReferrerStaff { get; set; }

        public DateTime? RiderReferrerDate { get; set; }

        [StringLength(200)]
        public string RiderReferrerAttachmentURL { get; set; }

        public JobNatureInfo JobNature { get; set; }

        [StringLength(50)]
        public string ProtocolNO { get; set; }

        public ProtocolTimeBaseInfo ProtocolTime { get; set; }

        public ProtocolTypeInfo ProtocolType { get; set; }

        public DateTime? ProtocolSignedDate { get; set; }

        public DateTime? ProtocolBeginDate { get; set; }

        public DateTime? ProtocolEndDate { get; set; }

        public InsuranceNatureBaseInfo InsuranceNature { get; set; }

        public DateTime? InsuranceStartBuyDate { get; set; }

        public DateTime? InsuranceStopBuyDate { get; set; }


        public BankTypeInfo BankType { get; set; }

        [StringLength(50)]
        public string BankNO { get; set; }

        public ExitReasonInfoBase ExitReason { get; set; }

        public bool IsInBlackList { get; set; }

        public new int? RiderEleID { get; set; }

        public string ExitRemark { get; set; }

        public string RiderReferrerRemark { get; set; }

    }

}
