using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lavie.Modules.Admin.Models
{
    public class GroupInfo
    {
        [JsonProperty(PropertyName = "groupID")]
        public Guid GroupID { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    [Serializable]
    public class GroupBase
    {
        public Guid GroupID { get; set; }
        public string Name { get; set; }
        public bool IsIncludeUser { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsSystem { get; set; }
    }

    [Serializable]
    public class Group : GroupBase
    {
        public Group() {
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
