using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class JobNatureInfo
    {
        ///<summary>
        /// 用工性质ID
        ///</summary>
        public int? JobNatureID { get; set; } // JobNatureID (Primary key)

        ///<summary>
        /// 用工性质
        ///</summary>
        public string Name { get; set; } // Name (length: 20)
    }
}
