using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Models;

namespace Lavie.Modules.Admin.Models.Api
{
    public class ProfileResult : ApiResult
    {
        [JsonProperty(PropertyName = "profile")]
        public UserInfoWarpper Profile { get; set; }
    }
    public class GroupTreeResult : ApiResult
    {
        [JsonProperty(PropertyName = "tree")]
        public List<GroupTreeNode> Tree { get; set; }
    }
}
