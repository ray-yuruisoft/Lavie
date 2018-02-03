using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lavie.Infrastructure.XmlRpc
{
    public static class XmlRpcParameterMapper
    {
        public static IDictionary<string, object> Map(ParameterDescriptor[] methodParameters, IList<XmlRpcParameter> rpcParameters)
        {
            Dictionary<string, object> mappedValues = new Dictionary<string, object>();

            for (int i = 0; i < rpcParameters.Count; i++)
                mappedValues.Add(methodParameters[i].ParameterName, rpcParameters[i].AsType(methodParameters[i].ParameterType));

            return mappedValues;
        }
    }
}
