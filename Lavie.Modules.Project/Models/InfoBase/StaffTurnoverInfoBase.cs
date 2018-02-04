using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class StaffTurnoverInfoBase
    {
        ///<summary>
        /// 员工职位流转日志ID
        ///</summary>
        public int StaffTurnoverID { get; set; } // StaffTurnoverID (Primary key)

        public StaffTurnoverTypeInfoBase StaffTurnoverType { get; set; }

        ///<summary>
        /// 申请者员工ID(员工ID,用户ID)
        ///</summary>
        public int RequestStaffID { get; set; } // RequestStaffID
        public StaffInfoBase RequestStaff { get; set; }
        public StaffInfoBase TargetStaff { get; set; }
        public GroupBaseInfo FromGroup { get; set; }
        public RoleBaseInfo FromRole { get; set; }
        public RiderJobTypeInfo FromRiderJobType { get; set; }
        public GroupBaseInfo ToGroup { get; set; }
        public RoleBaseInfo ToRole { get; set; }
        public RiderJobTypeInfo ToRiderJobType { get; set; }

        ///<summary>
        /// 申请备注
        ///</summary>
        public string RequestRemark { get; set; } // RequestRemark (length: 1000)

        ///<summary>
        /// 创建时间
        ///</summary>
        public System.DateTime CreationDate { get; set; } // CreationDate

        public System.DateTime EffectiveDate { get; set; } // EffectiveDate

        ///<summary>
        /// 审核状态(0 未审 1 一级审核通过 2 一级审核拒绝 3 二级审核通过 4 二级审核拒绝 ... )
        ///</summary>
        public int AuditStatus { get; set; } // AuditStatus

        ///<summary>
        /// 当前审核层级
        ///</summary>
        public int AuditStaffLevelCurrent { get; set; } // AuditStaffLevelCurrent

        ///<summary>
        /// 最大审批层级
        ///</summary>
        public int AuditStaffLevelMax { get; set; } // AuditStaffLevelMax

        public StaffInfoBase AuditStaff1 { get; set; }
        public StaffInfoBase AuditStaff2 { get; set; }
        public StaffInfoBase AuditStaff3 { get; set; }
        public StaffInfoBase AuditStaff4 { get; set; }

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
}
