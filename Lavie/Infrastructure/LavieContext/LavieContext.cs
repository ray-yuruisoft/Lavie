using System;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Routing;
using Lavie.Models;

namespace Lavie.Infrastructure
{
    /// <summary>
    /// 请求上下文
    /// </summary>
    /// <remarks>
    /// <para>该类封装RequestContext、RouteCollection、Site和IUser。</para>
    /// <para>一般作为其他类的构造函数的参数传入。使用时自动被依赖注入容器实例化。</para>
    /// </remarks>
    public class LavieContext
    {
        public LavieContext(RequestContext requestContext, RouteCollection routes, IUser user)
        {
            HttpContext = requestContext.HttpContext;
            RequestContext = requestContext;
            Routes = routes;
            User = user;

            var dataFormat = RequestContext.RouteData.Values["dataFormat"] as string;

            if (String.Compare(dataFormat, "RSS", StringComparison.OrdinalIgnoreCase) == 0)
                RequestDataFormat = RequestDataFormat.RSS;
            else if (String.Compare(dataFormat, "ATOM", StringComparison.OrdinalIgnoreCase) == 0)
                RequestDataFormat = RequestDataFormat.ATOM;
            else
                RequestDataFormat = RequestDataFormat.Web;
        }

        public LavieContext(LavieContext context)
            : this(context.RequestContext, context.Routes, context.User)
        {
        }

        public HttpContextBase HttpContext { get; private set; }
        public RequestContext RequestContext { get; private set; }
        public RequestDataFormat RequestDataFormat { get; private set; }
        public RouteCollection Routes { get; private set; }
        public IUser User { get; private set; }
    }
}
