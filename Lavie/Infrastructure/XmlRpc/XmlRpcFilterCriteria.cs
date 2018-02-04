using System.Web.Mvc;
using Lavie.FilterProviders.FilterCriterion;

namespace Lavie.Infrastructure.XmlRpc
{
    public class XmlRpcFilterCriteria : IFilterCriteria
    {
        public bool Match(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            return controllerContext.RouteData.DataTokens.ContainsKey("IsXmlRpc") && ((bool)controllerContext.RouteData.DataTokens["IsXmlRpc"]);
        }
    }
}
