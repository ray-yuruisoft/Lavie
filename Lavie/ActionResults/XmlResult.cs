using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Lavie.Utilities.Exceptions;

namespace Lavie.ActionResults
{
    public class XmlResult : ActionResult
    {
        public XmlResult() { }
        public XmlResult(object data) { this.Data = data; }

        public string ContentType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public object Data { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
                response.ContentType = this.ContentType;
            else
                response.ContentType = "text/xml";

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data != null)
            {
                if (this.Data is XmlNode)
                    response.Write(((XmlNode)this.Data).OuterXml);
                else if (this.Data is XNode)
                    response.Write(((XNode)this.Data).ToString());
                else
                {
                    var dataType = this.Data.GetType();
                    if (dataType.GetCustomAttributes(typeof(DataContractAttribute), true).FirstOrDefault() != null)
                    {
                        var dSer = new DataContractSerializer(dataType);
                        dSer.WriteObject(response.OutputStream, this.Data);
                    }
                    else
                    {
                        var xSer = new XmlSerializer(dataType);
                        xSer.Serialize(response.OutputStream, this.Data);
                    }
                }
            }
        }
    }

}