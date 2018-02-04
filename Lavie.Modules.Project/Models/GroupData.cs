using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Project.Models
{

    [Serializable]
    public class GroupBase
    {
        public Guid GroupID { get; set; }
        public string Name { get; set; }
        public bool IsIncludeUser { get; set; }
        public bool IsSystem { get; set; }
    }

    [Serializable]
    public class GroupData: GroupBase
    {
        public GroupData()
        {
            Roles = Enumerable.Empty<RoleBase>();
            LimitRoles = Enumerable.Empty<RoleBase>();
            Permissions = Enumerable.Empty<PermissionBase>();
        }
        public Guid? ParentID { set; get; }
        public int Level { get; set; }
        public int DisplayOrder { get; set; }
        public virtual IEnumerable<RoleBase> Roles { get; set; }
        public virtual IEnumerable<RoleBase> LimitRoles { get; set; }
        public virtual IEnumerable<PermissionBase> Permissions { get; set; }
    }
}
