using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class ExceptionReportInfo : IEquatable<ExceptionReportInfo>
    {

        //  骑手ID 站点ID 站点名称 姓名 在职状态 骑手职位  骑手职位类型 饿了么骑手ID 饿了么团队ID 饿了么团队名称

        [DisplayName("骑手ID")]
        public int? StaffID { get; set; }

        [DisplayName("站点ID")]
        public Guid? GroupID { get; set; }

        [DisplayName("站点名称")]
        public string StationName { get; set; }

        [DisplayName("姓名")]
        public string RiderName { get; set; }

        [DisplayName("手机号")]
        public string StaffMobile { get; set; }

        [DisplayName("在职状态")]
        public string StaffStatus { get; set; }

        [DisplayName("骑手职位")]
        public string RiderPosition { get; set; }

        [DisplayName("骑手职位类型")]
        public string RiderJobType { get; set; }

        [DisplayName("饿了么骑手ID")]
        public int RiderEleID { get; set; }

        [DisplayName("饿了么团队ID")]
        public int TeamID { get; set; }

        [DisplayName("饿了么团队名称")]
        public string TeamName { get; set; }

        [DisplayName("异常原因")]
        public string ExceptionReason { get; set; }

        [DisplayName("备注")]
        public string Remark { get; set; }

        public bool Equals(ExceptionReportInfo other)
        {
            return (this.RiderEleID == other.RiderEleID)
                && (this.ExceptionReason == other.ExceptionReason)
                && (this.Remark == other.Remark)
                ;
        }
        public override int GetHashCode()
        {
            return RiderEleID.GetHashCode()
                 ^ ExceptionReason.GetHashCode()
                 ^ Remark.GetHashCode()
                 ;
        }

    }
}

