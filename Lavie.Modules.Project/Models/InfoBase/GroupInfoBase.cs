using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class GroupInfoBase
    {
        public Guid GroupID { get; set; }
        public Guid? ParentID { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public bool IsIncludeUser { get; set; }
        public List<RoleInfoBase> Roles { get; set; }
    }


    public class RoleInfoBase
    {
        public Guid RoleID { get; set; }
        public string Name { get; set; }
    }

}
