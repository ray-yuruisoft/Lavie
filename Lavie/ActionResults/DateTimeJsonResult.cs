using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Lavie.ActionResults
{
    public class DateTimeJsonResult : JsonResult
    {
        private const String DefaultDateTimeFormateString = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string DateTimeFormateString
        {
            get;
            set;
        }

        /// <summary>
        /// 重写执行视图
        /// </summary>
        /// <param name="context">上下文</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("JsonRequestBehavior.DenyGet");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (this.Data != null)
            {
                // 方法1：IsoDateTimeConverter
                /*
                var dateTimeConverter = new IsoDateTimeConverter
                {
                    DateTimeFormat = DefaultDateTimeFormateString //"yyyy'-'MM'-'dd' 'HH':'mm':'ss"
                };
                var result = JsonConvert.SerializeObject(Data, Formatting.Indented, dateTimeConverter);
                response.Write(result);
                */
                // 方法2：JsonSerializerSettings
                var serializersettings = new JsonSerializerSettings
                {
                    DateFormatString = DateTimeFormateString ?? DefaultDateTimeFormateString,
                    Formatting = Formatting.Indented
                };
                var result = JsonConvert.SerializeObject(Data, Formatting.Indented, serializersettings);
                response.Write(result);

                /*
                var serializer = new JavaScriptSerializer();
                if (MaxJsonLength.HasValue)
                {
                    serializer.MaxJsonLength = MaxJsonLength.Value;
                }
                if (RecursionLimit.HasValue)
                {
                    serializer.RecursionLimit = RecursionLimit.Value;
                }

                string jsonString = serializer.Serialize(Data);
                const string p = @"\\/Date\((\d+)\)\\/";
                var matchEvaluator = new MatchEvaluator(this.ConvertJsonDateToDateString);
                var reg = new Regex(p);
                jsonString = reg.Replace(jsonString, matchEvaluator);
                response.Write(jsonString);
                */
            }
        }

        /// <summary>  
        /// 将Json序列化的时间由/Date(1294499956278)转为字符串 .
        /// </summary>  
        /// <param name="m">正则匹配</param>
        /// <returns>格式化后的字符串</returns>
        private string ConvertJsonDateToDateString(Match m)
        {
            var dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            string result = dt.ToString(DateTimeFormateString ?? DefaultDateTimeFormateString);
            return result;
        }
    }
    public static class ContollerDateTimeJsonExtensions
    {
        public static DateTimeJsonResult DateTimeJson(this Controller controller, object data, JsonRequestBehavior jsonRequestBehavior, string dateTimeFormateString = null)
        {
            var result = new DateTimeJsonResult()
            {
                Data = data,
                JsonRequestBehavior = jsonRequestBehavior,
                DateTimeFormateString = dateTimeFormateString,
            };

            return result;
        }
        public static DateTimeJsonResult DateTimeJson(this Controller controller, object data, string dateTimeFormateString = null)
        {
            return DateTimeJson(controller, data, JsonRequestBehavior.AllowGet, dateTimeFormateString);
        }
    }

}
