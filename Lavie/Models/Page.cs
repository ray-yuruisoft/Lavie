using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Models
{
    public class Page<T>
    {
        [JsonProperty(PropertyName = "list")]
        public List<T> List { get; set; }

        [JsonProperty(PropertyName = "totalItemCount")]
        public int TotalItemCount { get; set; }

        [JsonProperty(PropertyName = "totalPageCount")]
        public int TotalPageCount { get; set; }
    }
}
