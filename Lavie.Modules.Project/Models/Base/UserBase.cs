using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Project.Models
{
    public class UserBase : UserEditPar
    {

        [DisplayName("所属用户组")]
        [JsonIgnore]
        new public Guid? GroupID { get; set; }

        [JsonIgnore]
        public IEnumerable<Guid> Roles { get; set; }

        [JsonIgnore]
        public IEnumerable<Guid> Permissions { get; set; }

    }
}
