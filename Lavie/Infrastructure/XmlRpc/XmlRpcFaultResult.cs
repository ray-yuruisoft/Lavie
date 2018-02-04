using System.Web.Mvc;
using System.Xml.Linq;

namespace Lavie.Infrastructure.XmlRpc
{
    public class XmlRpcFaultResult : ActionResult
    {
        private readonly int _faultCode;
        private readonly string _faultString;

        public XmlRpcFaultResult(int faultCode, string faultString)
        {
            this._faultCode = faultCode;
            this._faultString = faultString;
        }

        public XmlRpcFaultResult()
        {
            _faultCode = 0;
            _faultString = "Unspecified Error";
        }

        public string FaultString
        {
            get { return _faultString; }
        }

        public int FaultCode
        {
            get { return _faultCode; }
        }

        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from <see cref="T:System.Web.Mvc.ActionResult"/>.
        /// </summary>
        /// <param name="context">The context within which the result is executed.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            XDocument response =
                new XDocument(
                    new XElement("methodResponse",
                        new XElement("fault",
                            new XElement("value",
                                new XElement("struct",
                                    new XElement("member",
                                        new XElement("name", "faultCode"),
                                        new XElement("value",
                                            new XElement("int", FaultCode)
                                            )
                                        ),
                                        new XElement("member",
                                            new XElement("name", "faultString"),
                                            new XElement("value",
                                                new XElement("string", _faultString)
                                                )
                                            )
                                    )
                                )
                            )
                        )
                    );

            response.Save(context.HttpContext.Response.Output);
        }
    }
}
