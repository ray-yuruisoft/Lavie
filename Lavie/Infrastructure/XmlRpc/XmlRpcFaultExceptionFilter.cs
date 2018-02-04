using System.Web.Mvc;

namespace Lavie.Infrastructure.XmlRpc
{
    public class XmlRpcFaultExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnException(ExceptionContext filterContext)
        {
            filterContext.Result = new XmlRpcFaultResult(0,filterContext.Exception.Message);
        }
    }
}
