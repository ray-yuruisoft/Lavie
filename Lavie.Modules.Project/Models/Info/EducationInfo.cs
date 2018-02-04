using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class EducationInfo
    {
        ///<summary>
        /// 学历ID
        ///</summary>
        public int? EducationID { get; set; } // EducationID (Primary key)

        ///<summary>
        /// 学历
        ///</summary>
        public string Name { get; set; } // Name (length: 20)
    }
}
