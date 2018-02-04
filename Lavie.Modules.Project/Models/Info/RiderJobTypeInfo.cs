using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class RiderJobTypeInfo
    {
        ///<summary>
        /// 骑手工作类型ID
        ///</summary>
        public int? RiderJobTypeID { get; set; } // RiderJobTypeID (Primary key)

        ///<summary>
        /// 骑手工作类型名称
        ///</summary>
        public string Name { get; set; } // Name (length: 20)
    }
}
