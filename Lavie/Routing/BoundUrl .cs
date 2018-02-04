using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Lavie.Extensions;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Reflection;
using Lavie.Infrastructure.FastReflectionLib;

namespace Lavie.Routing
{
    internal class BoundUrl
    {
        private static PropertyAccessor s_urlAccessor;

        static BoundUrl()
        {
            var type = typeof(Route).Assembly.GetType("System.Web.Routing.BoundUrl");
            var property = type.GetProperty("Url", BindingFlags.Instance | BindingFlags.Public);
            s_urlAccessor = new PropertyAccessor(property);
        }

        private object m_instance;

        public BoundUrl(object instance)
        {
            this.m_instance = instance;
        }

        public string Url
        {
            get
            {
                string url = (string)s_urlAccessor.GetValue(this.m_instance);
                //端口处理
                if (!url.IsNullOrWhiteSpace())
                    url = url.Replace("%3a", ":");
                return url;
            }
        }

    }
}
