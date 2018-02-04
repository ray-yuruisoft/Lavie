using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Models;

namespace Lavie.Modules.Admin.Models.Api
{
    public class GroupTreeNode
    {
        [JsonProperty(PropertyName = "id")]
        public Guid ID { get; set; }

        [JsonProperty(PropertyName = "parentID", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? ParentID { get; set; }

        [JsonProperty(PropertyName = "parentIDPath", NullValueHandling = NullValueHandling.Ignore)]
        public List<Guid> ParentIDPath { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "children", NullValueHandling = NullValueHandling.Ignore)]
        public List<GroupTreeNode> Children { get; set; }

        [JsonProperty(PropertyName = "level")]
        public int Level { get; set; }

        [JsonProperty(PropertyName = "displayOrder")]
        public int DisplayOrder { get; set; }

        [JsonProperty(PropertyName = "isIncludeUser")]
        public bool IsIncludeUser { get; set; }

        [JsonProperty(PropertyName = "isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty(PropertyName = "isSystem")]
        public bool IsSystem { get; set; }

        [JsonProperty(PropertyName = "roles", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<RoleBase> Roles { get; set; }

        [JsonProperty(PropertyName = "limitRoles", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<RoleBase> LimitRoles { get; set; }

        [JsonProperty(PropertyName = "permissions", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<PermissionBase> Permissions { get; set; }
    }
}
