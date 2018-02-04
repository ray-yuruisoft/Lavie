using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lavie.Modules.Admin.Models
{
    [Serializable]
    public class RoleInfo
    {
        [JsonProperty(PropertyName = "roleID")]
        public Guid RoleID { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    [Serializable]
    public class RoleBase : RoleInfo
    {
        [JsonProperty(PropertyName = "isSystem")]
        public bool IsSystem { get; set; }
        [JsonProperty(PropertyName = "displayOrder")]
        public int DisplayOrder { get; set; }
    }

    [Serializable]
    public class Role : RoleBase
    {
        public Role()
        {
            Permissions = Enumerable.Empty<PermissionBase>();
        }
        [JsonProperty(PropertyName = "permissions", NullValueHandling = NullValueHandling.Ignore)]
        public virtual IEnumerable<PermissionBase> Permissions { get; set; }
    }
}
