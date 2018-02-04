using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class StaffStatusInfoBase
    {
        ///<summary>
        /// 员工状态ID
        ///</summary>
        public int StaffStatusID { get; set; } // StaffStatusID (Primary key)

        ///<summary>
        /// 员工状态名称
        ///</summary>
        public string Name { get; set; } // Name (length: 20)
    }
}
