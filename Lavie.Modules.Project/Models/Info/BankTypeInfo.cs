using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class BankTypeInfo
    {
        ///<summary>
        /// 银行类型ID
        ///</summary>
        public int? BankTypeID { get; set; } // BankTypeID (Primary key)

        ///<summary>
        /// 银行类型
        ///</summary>
        public string Name { get; set; } // Name (length: 50)
    }
}
