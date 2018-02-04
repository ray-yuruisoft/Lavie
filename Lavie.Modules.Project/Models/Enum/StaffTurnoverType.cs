using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{

    public enum StaffTurnoverType
    {

        [Description("入职")]
        Entry = 1,

        [Description("离职")]
        Resign = 2,

        [Description("平调")]
        FlatMove = 3,

        [Description("晋升")]
        HigherMove = 4,

        [Description("降级")]
        LowerMove = 5,

        [Description("部门调整")]
        GroupMove = 6,

        [Description("骑手职位类型调整")]
        RiderTypeMove = 7,

        [Description("骑手自离")]
        RiderAutoLeave = 8

    }








}
