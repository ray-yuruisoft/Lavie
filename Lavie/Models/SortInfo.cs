using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Models
{
    public class SortInfo
    {
        [JsonProperty(PropertyName = "sortDir")]
        public SortDir SortDir { get; set; }

        [JsonProperty(PropertyName = "sort")]
        public String Sort { get; set; }
    }

    public enum SortDir
    {
        ASC,
        DESC
    }
}
