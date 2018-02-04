using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Lavie.Routing
{
    /// <summary>
    /// RouteEx
    /// 目的：
    /// 1、将集合类的值转换为以逗号分隔的字符串
    /// 2、清除空值
    /// </summary>
    public class RouteEx : Route
    {
        public RouteEx(string url, IRouteHandler routeHandler)
            : base(url, null, null, null, routeHandler)
        { }
        public RouteEx(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, null, null, routeHandler)
        { }

        public RouteEx(string url,
            RouteValueDictionary defaults,
            RouteValueDictionary constraints,
            IRouteHandler routeHandler)
            : base(url, defaults, constraints, null, routeHandler)
        { }
        public RouteEx(string url,
            RouteValueDictionary defaults,
            RouteValueDictionary constraints,
            RouteValueDictionary dataTokens,
            IRouteHandler routeHandler)
            : base(url, defaults, constraints, dataTokens, routeHandler)
        {
        }

        public override VirtualPathData GetVirtualPath(
            RequestContext requestContext,
            RouteValueDictionary values)
        {
            if (values == null) return base.GetVirtualPath(requestContext, values);

            var innerValues = GetNewRouteValus(values);
            return base.GetVirtualPath(requestContext, innerValues);
        }

        private RouteValueDictionary GetNewRouteValus(RouteValueDictionary values)
        {
            var pageLinkValueDictionary = new RouteValueDictionary();
            foreach (KeyValuePair<string, object> value in values)
            {
                if (value.Value == null) continue;

                // 字符串
                if (value.Value is String)
                {
                    var stringValue = value.Value as String;
                    if (stringValue != null && stringValue.Length > 0)
                    {
                        pageLinkValueDictionary.Add(value.Key, value.Value);
                    }
                    continue;
                }

                var list = value.Value as System.Collections.IEnumerable;
                if (list == null)
                {
                    pageLinkValueDictionary.Add(value.Key, value.Value);
                    continue;
                }

                var sb = new StringBuilder();
                foreach (var item in list)
                {
                    var part = Convert.ToString(item, CultureInfo.InvariantCulture);
                    //var part = Uri.EscapeDataString(Convert.ToString(item, CultureInfo.InvariantCulture));
                    if (sb.Length == 0)
                        sb.Append(part);
                    else
                        sb.Append("," + part);
                }

                if (sb.Length > 0)
                    pageLinkValueDictionary.Add(value.Key, sb.ToString());

            }

            return pageLinkValueDictionary;
        }
    }
}
