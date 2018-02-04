using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Extensions;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Project.Repositories;

namespace Lavie.Modules.Project.Models
{
    // TODO: 人事管理重复代码优化
    public class Staff
    {
        public int StaffID { get; set; }
        public string DisplayName { get; set; }
        public string RealName { get; set; }
        public string FullDisplayName
        {
            get
            {
                if (!DisplayName.IsNullOrWhiteSpace() && !RealName.IsNullOrWhiteSpace())
                {
                    return "{0}({1})".FormatWith(DisplayName, RealName);
                }
                else if (!DisplayName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else if (!RealName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string ShortDisplayName
        {
            get
            {
                if (!DisplayName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else if (!RealName.IsNullOrWhiteSpace())
                {
                    return RealName;
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        public string ShortRealName
        {
            get
            {
                if (!RealName.IsNullOrWhiteSpace())
                {
                    return RealName;
                }
                else if (!DisplayName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }

    public class StaffUserInfo : UserInfo
    {

    }

    public class StaffInput
    {
        [DisplayName("员工ID")]
        public int StaffID { get; set; }
        public RiderJobType RiderJobType { get; set; }
    }

    public class PermitInput {

        public System.Guid? ParentID { get; set; } // ParentID     
        public System.Guid PermissionID { get; set; } // PermissionID (Primary key)       
        public string ModuleName { get; set; } // ModuleName (length: 50)     
        public string Name { get; set; } // Name (length: 50)      
        public int Level { get; set; } // Level
        public int DisplayOrder { get; set; } // DisplayOrder

    }

    public class StaffSearchCriteria
    {
        public Guid[] GroupIDs { get; set; }
        public Guid[] RoleIDs { get; set; }
        public String Keyword { get; set; }
        public UserStatus[] Status { get; set; }

    }

}
