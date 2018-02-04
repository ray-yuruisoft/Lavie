using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class RecruitChannelInfo
    {
        ///<summary>
        /// 招聘渠道ID
        ///</summary>
        public int RecruitChannelID { get; set; } // RecruitChannelID (Primary key)

        ///<summary>
        /// 招聘渠道
        ///</summary>
        public string Name { get; set; } // Name (length: 20)
    }
}
