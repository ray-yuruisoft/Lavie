using System;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Routing;
using Lavie.Utilities.Exceptions;

namespace Lavie.Extensions
{
    public static class RouteCollectionExtensions
    {
        #region MapRoute

        public static Route MapRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces, string urlPrefix, string subDomain, string moduleName)
        {
            Guard.ArgumentNotNull(routes, "routes");
            Guard.ArgumentNotNull(url, "url");

            Route route;

            if (!urlPrefix.IsNullOrWhiteSpace())
                url = urlPrefix + "/" + url;

            //如果是子域名
            if (!subDomain.IsNullOrWhiteSpace())
            {
                route = routes.MapDomainRoute(name, url, defaults, constraints, namespaces, subDomain);
            }
            else
            {
                route = routes.MapRouteEx(name, url, defaults, constraints, namespaces);
            }

            if (!moduleName.IsNullOrWhiteSpace())
            {
                route.DataTokens["ModuleName"] = moduleName;
            }

            return route;
        }

        #endregion

        #region MapDomainRoute

        public static Route MapDomainRoute(this RouteCollection routes, string name, string url, string subDomain)
        {
            return routes.MapDomainRoute(name, url, null, null, subDomain);
        }
        public static Route MapDomainRoute(this RouteCollection routes, string name, string url, object defaults, string subDomain)
        {
            return routes.MapDomainRoute(name, url, defaults, null, subDomain);
        }
        public static Route MapDomainRoute(this RouteCollection routes, string name, string url, string[] namespaces, string subDomain)
        {
            return routes.MapDomainRoute(name, url, null, null, namespaces, subDomain);
        }
        public static Route MapDomainRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, string subDomain)
        {
            return routes.MapDomainRoute(name, url, defaults, constraints, null, subDomain);
        }
        public static Route MapDomainRoute(this RouteCollection routes, string name, string url, object defaults, string[] namespaces, string subDomain)
        {
            return routes.MapDomainRoute(name, url, defaults, null, namespaces, subDomain);
        }
        public static Route MapRouteEx(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            Guard.ArgumentNotNull(routes, "routes");
            Guard.ArgumentNotNull(url, "url");

            Route route = new RouteEx(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary()
            };

            //ConstraintValidation.Validate(route);
            if (namespaces != null && namespaces.Length > 0)
            {
                route.DataTokens["Namespaces"] = namespaces;
            }
            routes.Add(name, route);
            return route;
        }

        /*
         * 
         subDomain参数可以为：
         1、单个合法单词，如：blog,admin等(对应：blog.yourdomain.com,admin.yourdomain.com)
         2、点号分隔的的合法单词,如：hello.baby等(对应：hello.baby.yourdomain.com)
         3、规则模式，如：{user},{user}.blog等(对应：{user}.yourdomain.com,{user}.blog.yourdomain.com)
         */
        public static Route MapDomainRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces, string subDomain)
        {
            Guard.ArgumentNotNull(routes, "routes");
            Guard.ArgumentNotNull(url, "url");

            Route route = new DomainRoute("{protocol}" + Uri.SchemeDelimiter + subDomain + ".{*domain}", url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary()
            };
            if (namespaces != null && namespaces.Length > 0)
            {
                route.DataTokens["Namespaces"] = namespaces;
            }
            routes.Add(name, route);
            return route;
        }

        /*
         * 
         网站域名的命名规则： 
         
            1． 英文域名： 
            1） 26个英文字母 
            2） “0”到“9”的数字 
            3） “-”英文中的连词不得用于开头及结尾处

            2． 中文域名： 
            1） 两到十五个汉字之间的字词或词组 
            2） 26个英文字母 
            3） “0”到“9”的数字 

            在域名中字符的组合也有一些限制： 
            1． 在域名中是不区分英文字母的大小写。
            2． 中文域名不区分简繁体。 
            3． 空格及符号如“？/\；：@#$%^~_=+，8． 。<>”等都不能用在域名命名
            4． 英文域名命名长度限制介于2到46个字符之间
         
         */

        #endregion
    }
}
