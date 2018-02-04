using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class RequestStationLeaveInput
    {
        [Range(1, Int32.MaxValue, ErrorMessage = "Frontend: 请输入请假类型")]
        public Int32 StaffLeaveTypeID { get; set; }

        public Int32? RequestStaffID { get; set; } // 目前内部赋值

        [Range(1, Int32.MaxValue, ErrorMessage = "Frontend: 请输入请假人ID")]
        public Int32 TargetStaffID { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "Frontend: 请输入请假半天数")]
        public Int32 HalfDays { get; set; }        // 冗余字段，可通过 BeginDate 和 EndDate 计算出

        //  public float Days { get; set; }

        public DateTime BeginDate { get; set; }

        public Int32 BeginDatePartial { get; set; }

        public DateTime EndDate { get; set; }

        public Int32 EndDatePartial { get; set; }

        [StringLength(1000)]
        public string RequestRemark { get; set; }

        [StringLength(200)]
        public string RequestAttachmentURL1 { get; set; }

        [StringLength(200)]
        public string RequestAttachmentURL2 { get; set; }

    }
}
