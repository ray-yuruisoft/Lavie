using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lavie.Html
{
    public static class ScriptExtensions
    {
        #region RenderScriptVariable

        public static void RenderScriptVariable(this HtmlHelper htmlHelper, string name, object value)
        {
            const string scriptVariableFormat = "window.{0} = {1};";
            string script;

            if (value != null)
            {
                var dcjs = new DataContractJsonSerializer(value.GetType());
                using (var ms = new MemoryStream())
                {
                    dcjs.WriteObject(ms, value);
                    script = string.Format(scriptVariableFormat, name, Encoding.Default.GetString(ms.ToArray()));
                }
            }
            else
            {
                script = string.Format(scriptVariableFormat, name, "null");
            }

            htmlHelper.ViewContext.Writer.Write(script);
        }

        #endregion

        #region ScriptTag

        public static MvcHtmlString RenderScriptTag(this HtmlHelper htmlHelper, string src, string type = "text/javascript", object htmlAttributes = null)
        {
            var tagBuilder = new TagBuilder("script");

            tagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            if (!string.IsNullOrEmpty(type))
            {
                tagBuilder.MergeAttribute("type", type);
            }
            if (!string.IsNullOrEmpty(src))
            {
                tagBuilder.MergeAttribute("src", src);
            }

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        #endregion

    }
}
