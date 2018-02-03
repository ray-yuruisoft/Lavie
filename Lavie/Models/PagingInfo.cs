using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Models
{
    public class PagingInfo
    {

        [JsonIgnore]
        public int PageIndex { get { return PageNumber >= 1 ? PageNumber - 1 : 0; } }

        [JsonProperty(PropertyName = "pageNumber")]
        [Range(1, Int32.MaxValue, ErrorMessage = "请输入 PageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty(PropertyName = "pageSize")]
        [Range(1, Int32.MaxValue, ErrorMessage = "请输入 PageSize")]
        public int PageSize { get; set; }

        [JsonProperty(PropertyName = "skipTop")]
        public int SkipTop { get; set; }

        [JsonProperty(PropertyName = "sortInfo")]
        public SortInfo SortInfo { get; set; }

        [JsonProperty(PropertyName = "isExcludeMetaData")]
        public bool IsExcludeMetaData { get; set; }

    }
}
