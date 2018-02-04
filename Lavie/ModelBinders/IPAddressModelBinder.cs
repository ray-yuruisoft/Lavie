using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using Lavie.Extensions;
using Lavie.Extensions.IP;

namespace Lavie.ModelBinders
{
    /// <summary>
    /// 获取客户端IP地址
    /// </summary>
    public class IPAddressModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return controllerContext.RequestContext.HttpContext.Request.GetIPAddress();
        }
    }

}