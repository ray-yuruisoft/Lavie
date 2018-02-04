using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.ActionResults;
using XMA = Lavie.Modules.Admin.Models.Api;

namespace Lavie.Modules.Project.Models
{
    public class GroupTreeNode : XMA.GroupTreeNode
    {

        [JsonProperty(PropertyName = "children", NullValueHandling = NullValueHandling.Ignore)]
        new public List<GroupTreeNode> Children { get; set; }

        [JsonProperty(PropertyName = "staffLeaveAuditFlow", NullValueHandling = NullValueHandling.Ignore)]
        public StaffLeaveAuditFlowInfo StaffLeaveAuditFlow { get; set; }
    }

    public class StaffLeaveAuditFlowBase
    {
        [JsonProperty(PropertyName = "staffLeaveAuditFlowID", NullValueHandling = NullValueHandling.Ignore)]
        public System.Guid StaffLeaveAuditFlowID { get; set; } // StaffLeaveAuditFlowID (Primary key)

        ///<summary>
        /// 申请角色ID
        ///</summary>
        [JsonProperty(PropertyName = "requestRoleID", NullValueHandling = NullValueHandling.Ignore)]
        public System.Guid RequestRoleID { get; set; } // RequestRoleID

        ///<summary>
        /// 1级审核组ID
        ///</summary>
        [JsonProperty(PropertyName = "auditGroupID1", NullValueHandling = NullValueHandling.Ignore)]
        public System.Guid? AuditGroupID1 { get; set; } // AuditGroupID1

        ///<summary>
        /// 2级审核组ID
        ///</summary>
        [JsonProperty(PropertyName = "auditGroupID2", NullValueHandling = NullValueHandling.Ignore)]
        public System.Guid? AuditGroupID2 { get; set; } // AuditGroupID2

        ///<summary>
        /// 3级审核组ID
        ///</summary>
        [JsonProperty(PropertyName = "auditGroupID3", NullValueHandling = NullValueHandling.Ignore)]
        public System.Guid? AuditGroupID3 { get; set; } // AuditGroupID3

        ///<summary>
        /// 4级审核组ID
        ///</summary>
        [JsonProperty(PropertyName = "auditGroupID4", NullValueHandling = NullValueHandling.Ignore)]
        public System.Guid? AuditGroupID4 { get; set; } // AuditGroupID4

        ///<summary>
        /// 1级审核角色ID
        ///</summary>
        [JsonProperty(PropertyName = "auditRoleID1", NullValueHandling = NullValueHandling.Ignore)]
        public System.Guid? AuditRoleID1 { get; set; } // AuditRoleID1

        ///<summary>
        /// 2级审核角色ID
        ///</summary>
        [JsonProperty(PropertyName = "auditRoleID2", NullValueHandling = NullValueHandling.Ignore)]
        public System.Guid? AuditRoleID2 { get; set; } // AuditRoleID2

        ///<summary>
        /// 3级审核角色ID
        ///</summary>
        [JsonProperty(PropertyName = "auditRoleID3", NullValueHandling = NullValueHandling.Ignore)]
        public System.Guid? AuditRoleID3 { get; set; } // AuditRoleID3

        ///<summary>
        /// 4级审核角色ID
        ///</summary>
        [JsonProperty(PropertyName = "auditRoleID4", NullValueHandling = NullValueHandling.Ignore)]
        public System.Guid? AuditRoleID4 { get; set; } // AuditRoleID4

        ///<summary>
        /// 1级审核请假天数
        ///</summary>
        [JsonProperty(PropertyName = "auditStaffLevelMaxDays1", NullValueHandling = NullValueHandling.Ignore)]
        public int? AuditStaffLevelMaxDays1 { get; set; } // AuditStaffLevelMaxDays1

        ///<summary>
        /// 2级审核请假天数
        ///</summary>
        [JsonProperty(PropertyName = "auditStaffLevelMaxDays2", NullValueHandling = NullValueHandling.Ignore)]
        public int? AuditStaffLevelMaxDays2 { get; set; } // AuditStaffLevelMaxDays2

        ///<summary>
        /// 3级审核请假天数
        ///</summary>
        [JsonProperty(PropertyName = "auditStaffLevelMaxDays3", NullValueHandling = NullValueHandling.Ignore)]
        public int? AuditStaffLevelMaxDays3 { get; set; } // AuditStaffLevelMaxDays3

        ///<summary>
        /// 4级审核请假天数
        ///</summary>
        [JsonProperty(PropertyName = "auditStaffLevelMaxDays4", NullValueHandling = NullValueHandling.Ignore)]
        public int? AuditStaffLevelMaxDays4 { get; set; } // AuditStaffLevelMaxDays4

    }

    public class StaffLeaveAuditFlowInfo
    {

        public GroupBaseInfo StaffLeaveAuditFlowID { get; set; }

        public RoleBaseInfo RequestRoleID { get; set; } // RequestRoleID

        ///<summary>
        /// 1级审核组ID
        ///</summary>
        [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "GroupID", "00000000-0000-0000-0000-000000000000")]
        public GroupBaseInfo AuditGroupID1 { get; set; } // AuditGroupID1

        ///<summary>
        /// 2级审核组ID
        ///</summary>
        [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "GroupID", "00000000-0000-0000-0000-000000000000")]
        public GroupBaseInfo AuditGroupID2 { get; set; } // AuditGroupID2

        ///<summary>
        /// 3级审核组ID
        ///</summary>
        [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "GroupID", "00000000-0000-0000-0000-000000000000")]
        public GroupBaseInfo AuditGroupID3 { get; set; } // AuditGroupID3

        ///<summary>
        /// 4级审核组ID
        ///</summary>
        [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "GroupID", "00000000-0000-0000-0000-000000000000")]
        public GroupBaseInfo AuditGroupID4 { get; set; } // AuditGroupID4

        ///<summary>
        /// 1级审核角色ID
        ///</summary>
        [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "RoleID", "00000000-0000-0000-0000-000000000000")]
        public RoleBaseInfo AuditRoleID1 { get; set; } // AuditRoleID1

        ///<summary>
        /// 2级审核角色ID
        ///</summary>
        [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "RoleID", "00000000-0000-0000-0000-000000000000")]
        public RoleBaseInfo AuditRoleID2 { get; set; } // AuditRoleID2

        ///<summary>
        /// 3级审核角色ID
        ///</summary>
        [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "RoleID", "00000000-0000-0000-0000-000000000000")]
        public RoleBaseInfo AuditRoleID3 { get; set; } // AuditRoleID3

        ///<summary>
        /// 4级审核角色ID
        ///</summary>
        [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "RoleID", "00000000-0000-0000-0000-000000000000")]
        public RoleBaseInfo AuditRoleID4 { get; set; } // AuditRoleID4

        ///<summary>
        /// 1级审核请假天数
        ///</summary>         
        public int? AuditStaffLevelMaxDays1 { get; set; } // AuditStaffLevelMaxDays1

        ///<summary>
        /// 2级审核请假天数
        ///</summary>
        public int? AuditStaffLevelMaxDays2 { get; set; } // AuditStaffLevelMaxDays2

        ///<summary>
        /// 3级审核请假天数
        ///</summary>
        public int? AuditStaffLevelMaxDays3 { get; set; } // AuditStaffLevelMaxDays3

        ///<summary>
        /// 4级审核请假天数
        ///</summary>
        public int? AuditStaffLevelMaxDays4 { get; set; } // AuditStaffLevelMaxDays4



    }





}
