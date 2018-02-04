using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Models;

namespace Lavie.Modules.Project.Models
{
    public class JobNatureListResut : ApiResult
    {

        [JsonProperty(PropertyName = "jobnatures")]
        public object Jobnatures { get; set; }
    }
}
