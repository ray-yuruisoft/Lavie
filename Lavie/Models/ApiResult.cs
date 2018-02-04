﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Models
{
    public enum ApiResultCode : Int32
    {
        Success = 200,
        DefaultError = 400
    }

    public class ApiResult
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "url", NullValueHandling = NullValueHandling.Ignore)]
        public string URL { get; set; }
    }

    public class ApiListResult : ApiResult
    {
        [JsonProperty(PropertyName = "list", NullValueHandling = NullValueHandling.Ignore)]
        public object List { get; set; }
    }
    public class ApiPageResult : ApiResult
    {
        [JsonProperty(PropertyName = "page", NullValueHandling = NullValueHandling.Ignore)]
        public object Page { get; set; }
    }

    public class ApiTreeResult : ApiResult
    {
        [JsonProperty(PropertyName = "tree")]
        public List<TreeNode> Tree { get; set; }
    }

    public class ApiItemResult : ApiResult
    {
        // [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "IsShow", "00000000-0000-0000-0000-000000000000")]
        [JsonProperty(PropertyName = "item", NullValueHandling = NullValueHandling.Ignore)]
        public object Item { get; set; }
    }

    public class ApiHTMLResult : ApiResult
    {
        [JsonProperty(PropertyName = "html", NullValueHandling = NullValueHandling.Ignore)]
        public string HTML { get; set; }
    }

    [Serializable]
    public class TreeNode
    {
        [JsonProperty(PropertyName = "id")]
        public Guid ID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "children", NullValueHandling = NullValueHandling.Ignore)]
        public List<TreeNode> Children { get; set; }
    }

}
