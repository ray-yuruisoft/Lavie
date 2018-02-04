using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public enum StaffLeaveAuditStatus
    {

        [Description("未审")]
        UnAudited = 0,

        [Description("一级审核通过")]
        OneLevelAudited = 1,

        [Description("一级审核拒绝")]
        OneLevelDeny = 2,

        [Description("二级审核通过")]
        TwoLevelAudited = 3,

        [Description("二级审核拒绝")]
        TwoLevelDeny = 4,

        [Description("三级审核通过")]
        ThreeLevelAudited = 5,

        [Description("三级审核拒绝")]
        ThreeLevelDeny = 6,

        [Description("四级审核通过")]
        FourLevelAudited = 7,

        [Description("四级审核拒绝")]
        FourLevelDeny = 8,

    }
}
