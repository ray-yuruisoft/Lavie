using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.ActionResults;
using Lavie.ModelValidation.Attributes;

namespace Lavie.Modules.Admin.Models
{
    public class NotificationBase
    {
        [JsonProperty(PropertyName = "notificationID")]
        public int NotificationID { get; set; }

        [JsonProperty(PropertyName = "fromUser")]
        public UserInfoWarpper FromUser { get; set; }

        [JsonProperty(PropertyName = "toUser")]
        [JsonConverter(typeof(DependencyJsonConverter<int>), "UserID", 0)]
        public UserInfoWarpper ToUser { get; set; }

        [JsonProperty(PropertyName = "creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string URL { get; set; }
    }
    public class Notification : NotificationBase
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

    public class NotificationUser: Notification
    {

        [JsonProperty(PropertyName = "readTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ReadTime { get; set; }

        [JsonProperty(PropertyName = "deleteTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DeleteTime { get; set; }
    }

    public class NotificationIDInput
    {
        [Required(ErrorMessage = "请输入通知ID")]
        public int NotificationID { get; set; }
    }

    public class NotificationIDListInput
    {
        [CollectionElementRange(1, Int32.MaxValue, ErrorMessage = "请输入合法的通知ID集")]
        public int[] NotificationIDs { get; set; }
    }

    public class NotificationInput
    {
        [Range(1, Int32.MaxValue, ErrorMessage = "请输入通知ID")]
        public int? NotificationID { get; set; }

        public int? FromUserID { get; set; }    // 内部赋值

        public int? ToUserID { get; set; }

        [Required(ErrorMessage = "请输入通知标题")]
        [StringLength(100, ErrorMessage = "通知标题请保持在100个字符以内")]
        public string Title { get; set; }

        [Required(ErrorMessage = "请输入通知内容")]
        [StringLength(1000, ErrorMessage = "通知内容请保持在1000个字符以内")]
        public string Message { get; set; }

        [StringLength(200, ErrorMessage = "URL保持在200个字符以内")]
        public string URL { get; set; }
    }

    public class NotificationSearchCriteria
    {
        public bool? IsReaded { get; set; }

        public int? FromUserID { get; set; }

        public int? ToUserID { get; set; }

        public string Keyword { get; set; }

        public DateTime? CreationDateBegin { get; set; }

        public DateTime? CreationDateEnd { get; set; }

    }
}
