using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Project.Extentions;

namespace Lavie.Modules.Project.Models
{
    public class StaffLeaveInfo
    {
        ///<summary>
        /// 请假ID
        ///</summary>
        public int StaffLeaveID { get; set; } // StaffLeaveID (Primary key)

        public StaffLeaveType StaffLeaveType { get; set; }

        public StaffInfoBase RequestStaff { get; set; }

        public StaffInfoBase TargetStaff { get; set; }


        ///<summary>
        /// 请假天数(冗余)
        ///</summary>
        public int HalfDays { get; set; } // Days


        public int BeginDatePartial { get; set; }

        public int EndDatePartial { get; set; }
        ///<summary>
        /// 请假开始日期
        ///</summary>
        public System.DateTime BeginDate { get; set; } // BeginDate

        ///<summary>
        /// 请假结束日期
        ///</summary>
        public System.DateTime EndDate { get; set; } // EndDate

        ///<summary>
        /// 申请备注
        ///</summary>
        public string RequestRemark { get; set; } // RequestRemark (length: 1000)

        ///<summary>
        /// 申请附件URL1
        ///</summary>
        public string RequestAttachmentURL1 { get; set; } // RequestAttachmentURL1 (length: 200)

        ///<summary>
        /// 申请附件URL2
        ///</summary>
        public string RequestAttachmentURL2 { get; set; } // RequestAttachmentURL2 (length: 200)

        ///<summary>
        /// 附件1上传时间
        ///</summary>
        public System.DateTime? RequestAttachment1UploadTime { get; set; } // RequestAttachment1UploadTime

        ///<summary>
        /// 附件2上传时间
        ///</summary>
        public System.DateTime? RequestAttachment2UploadTime { get; set; } // RequestAttachment2UploadTime

        ///<summary>
        /// 创建日期（申请时间）
        ///</summary>
        public System.DateTime CreationDate { get; set; } // CreationDate

        public AuditStatus AuditStatus { get; set; }

        ///<summary>
        /// 当前审核层级
        ///</summary>
        public int AuditStaffLevelCurrent { get; set; } // AuditStaffLevelCurrent

        ///<summary>
        /// 最大审批层级
        ///</summary>
        public int? AuditStaffLevelMax { get; set; } // AuditStaffLevelMax

        ///<summary>
        /// 一级审核员ID(员工ID,用户ID)
        ///</summary>
        public StaffInfoBase AuditStaff1 { get; set; } // AuditStaffID1




        ///<summary>
        /// 二级审核员ID(员工ID,用户ID)
        ///</summary>
        public StaffInfoBase AuditStaff2 { get; set; } // AuditStaffID2

        ///<summary>
        /// 三级审核员ID(员工ID,用户ID)
        ///</summary>
        public StaffInfoBase AuditStaff3 { get; set; } // AuditStaffID3

        ///<summary>
        /// 四级审核员ID(员工ID,用户ID)
        ///</summary>
        public StaffInfoBase AuditStaff4 { get; set; } // AuditStaffID4

        ///<summary>
        /// 一级审核时间
        ///</summary>
        public System.DateTime? AuditDate1 { get; set; } // AuditDate1

        ///<summary>
        /// 二级审核时间
        ///</summary>
        public System.DateTime? AuditDate2 { get; set; } // AuditDate2

        ///<summary>
        /// 三级审核时间
        ///</summary>
        public System.DateTime? AuditDate3 { get; set; } // AuditDate3

        ///<summary>
        /// 四级审核时间
        ///</summary>
        public System.DateTime? AuditDate4 { get; set; } // AuditDate4

        ///<summary>
        /// 一级审核备注
        ///</summary>
        public string AuditRemark1 { get; set; } // AuditRemark1 (length: 1000)

        ///<summary>
        /// 二级审核时间
        ///</summary>
        public string AuditRemark2 { get; set; } // AuditRemark2 (length: 1000)

        ///<summary>
        /// 三级审核时间
        ///</summary>
        public string AuditRemark3 { get; set; } // AuditRemark3 (length: 1000)

        ///<summary>
        /// 四级审核时间
        ///</summary>
        public string AuditRemark4 { get; set; } // AuditRemark4 (length: 1000)


    }

    public class StaffLeaveInfoExtention : StaffLeaveInfo
    {

        public int CurrentUserAudituditStaffLevel { get; set; }

    }

    public class AuditStatus
    {
        public StaffLeaveAuditStatus AuditStatusID { get; set; }

        [JsonConverter(typeof(EnumJsonConvert<StaffLeaveAuditStatus>))]
        public StaffLeaveAuditStatus Name { get; set; }
    }

    public class StaffLeaveType
    {
        ///<summary>
        /// 请假类型ID
        ///</summary>
        public int StaffLeaveTypeID { get; set; } // StaffLeaveTypeID


        public string Name { get; set; }

    }

    public class StaffLeaveBase
    {
        ///<summary>
        /// 请假ID
        ///</summary>
        public int StaffLeaveID { get; set; } // StaffLeaveID (Primary key)

        ///<summary>
        /// 请假类型ID
        ///</summary>
        public int StaffLeaveTypeID { get; set; } // StaffLeaveTypeID

        ///<summary>
        /// 申请者员工ID(员工ID,用户ID)
        ///</summary>
        public int RequestStaffID { get; set; } // RequestStaffID

        ///<summary>
        /// 目标员工ID(员工ID,用户ID)
        ///</summary>
        public int TargetStaffID { get; set; } // TargetStaffID

        ///<summary>
        /// 请假天数(冗余)
        ///</summary>
        public int HalfDays { get; set; } // HalfDays

        ///<summary>
        /// 请假开始日期
        ///</summary>
        public System.DateTime BeginDate { get; set; } // BeginDate
        public int BeginDatePartial { get; set; } // BeginDatePartial

        ///<summary>
        /// 请假结束日期
        ///</summary>
        public System.DateTime EndDate { get; set; } // EndDate
        public int EndDatePartial { get; set; } // EndDatePartial

        ///<summary>
        /// 申请备注
        ///</summary>
        public string RequestRemark { get; set; } // RequestRemark (length: 1000)

        ///<summary>
        /// 申请附件URL1
        ///</summary>
        public string RequestAttachmentURL1 { get; set; } // RequestAttachmentURL1 (length: 200)

        ///<summary>
        /// 申请附件URL2
        ///</summary>
        public string RequestAttachmentURL2 { get; set; } // RequestAttachmentURL2 (length: 200)

        ///<summary>
        /// 附件1上传时间
        ///</summary>
        public System.DateTime? RequestAttachment1UploadTime { get; set; } // RequestAttachment1UploadTime

        ///<summary>
        /// 附件2上传时间
        ///</summary>
        public System.DateTime? RequestAttachment2UploadTime { get; set; } // RequestAttachment2UploadTime

        ///<summary>
        /// 创建日期（申请时间）
        ///</summary>
        public System.DateTime CreationDate { get; set; } // CreationDate
        public System.DateTime EffectiveDate { get; set; } // EffectiveDate

        ///<summary>
        /// 审核状态(0 未审 1 一级审核通过 2 一级审核拒绝 3 二级审核通过 4 二级审核拒绝 ... )
        ///</summary>
        public StaffLeaveAuditStatus AuditStatus { get; set; } // AuditStatus

        ///<summary>
        /// 当前审核层级
        ///</summary>
        public int AuditStaffLevelCurrent { get; set; } // AuditStaffLevelCurrent

        ///<summary>
        /// 最大审批层级
        ///</summary>
        public int? AuditStaffLevelMax { get; set; } // AuditStaffLevelMax

        ///<summary>
        /// 一级审核员ID(员工ID,用户ID)
        ///</summary>
        public int? AuditStaffID1 { get; set; } // AuditStaffID1

        ///<summary>
        /// 二级审核员ID(员工ID,用户ID)
        ///</summary>
        public int? AuditStaffID2 { get; set; } // AuditStaffID2

        ///<summary>
        /// 三级审核员ID(员工ID,用户ID)
        ///</summary>
        public int? AuditStaffID3 { get; set; } // AuditStaffID3

        ///<summary>
        /// 四级审核员ID(员工ID,用户ID)
        ///</summary>
        public int? AuditStaffID4 { get; set; } // AuditStaffID4

        ///<summary>
        /// 一级审核时间
        ///</summary>
        public System.DateTime? AuditDate1 { get; set; } // AuditDate1

        ///<summary>
        /// 二级审核时间
        ///</summary>
        public System.DateTime? AuditDate2 { get; set; } // AuditDate2

        ///<summary>
        /// 三级审核时间
        ///</summary>
        public System.DateTime? AuditDate3 { get; set; } // AuditDate3

        ///<summary>
        /// 四级审核时间
        ///</summary>
        public System.DateTime? AuditDate4 { get; set; } // AuditDate4

        ///<summary>
        /// 一级审核备注
        ///</summary>
        public string AuditRemark1 { get; set; } // AuditRemark1 (length: 1000)

        ///<summary>
        /// 二级审核时间
        ///</summary>
        public string AuditRemark2 { get; set; } // AuditRemark2 (length: 1000)

        ///<summary>
        /// 三级审核时间
        ///</summary>
        public string AuditRemark3 { get; set; } // AuditRemark3 (length: 1000)

        ///<summary>
        /// 四级审核时间
        ///</summary>
        public string AuditRemark4 { get; set; } // AuditRemark4 (length: 1000)


    }

    public class AuditStationLeaveInput
    {

        public Int32 StaffLeaveID { get; set; }

        [EnumRange(new int[] { 1, 2 },ErrorMessage ="参数错误")]
        public Int32 AuditStatus { get; set; }

        public string AuditRemark { get; set; }

    }

}
