using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public enum MaritalStatus
    {

        [Description("未婚")]
        UnMarried = 1,

        [Description("已婚")]
        Married = 2,

        [Description("离异")]
        Divorce = 3

    }
}
