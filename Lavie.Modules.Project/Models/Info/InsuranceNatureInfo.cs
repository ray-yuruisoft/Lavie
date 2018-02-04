using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class InsuranceNatureInfo
    {
        ///<summary>
        /// 保险性质ID
        ///</summary>
        public int InsuranceNatureID { get; set; } // InsuranceNatureID (Primary key)

        ///<summary>
        /// 保险性质
        ///</summary>
        public string Name { get; set; } // Name (length: 20)
    }
}
