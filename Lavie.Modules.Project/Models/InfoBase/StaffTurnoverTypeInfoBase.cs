using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class StaffTurnoverTypeInfoBase
    {
        ///<summary>
        /// 员工职位流转日志类型ID
        ///</summary>
        public int StaffTurnoverTypeID { get; set; } // StaffTurnoverTypeID

        public string Name { get; set; }

    }

    public class StaffLeaveTypeInfoBase
    {

        public int StaffLeaveTypeID { get; set; } 

        public string Name { get; set; }

    }

}
