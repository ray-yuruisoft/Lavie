using System;
using System.Web;
using System.Web.Routing;
using Lavie.Extensions;
using System.Web.Mvc;

namespace Lavie.Routing
{
    public class DomainRoute : RouteEx
    {
        private readonly DomainParser _domainParser;

        public string Pattern { get; private set; }

        #region Constructor

        public DomainRoute(string pattern, string url, IRouteHandler routeHandler)
            : this(pattern, url, null, null, null, routeHandler)
        { }
 
        public DomainRoute(string pattern, string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : this(pattern, url, defaults, null, null, routeHandler)
        { }

        public DomainRoute(string pattern,
            string url,
            RouteValueDictionary defaults,
            RouteValueDictionary constraints,
            IRouteHandler routeHandler)
            : this(pattern, url, defaults, constraints, null, routeHandler)
        { }
 
        public DomainRoute(string pattern,
            string url, 
            RouteValueDictionary defaults, 
            RouteValueDictionary constraints, 
            RouteValueDictionary dataTokens,
            IRouteHandler routeHandler)
            : base(url,defaults,constraints,dataTokens,routeHandler)
        {
            this.Url = url;
            this.Defaults = defaults;
            this.Constraints = constraints;
            this.DataTokens = dataTokens;
            this.RouteHandler = routeHandler;

            this.Pattern = pattern;
            this._domainParser = new DomainParser(pattern);
        }

        #endregion

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // match domain
            var domainValues = this._domainParser.Match(httpContext.Request.Url);
            if (domainValues == null) return null;

            // match path
            var routeData = base.GetRouteData(httpContext);
            if (routeData == null) return null;

            // merge
            routeData.Values.Concat(domainValues);
            routeData.Route = this;

            return routeData;

        }

        public override VirtualPathData GetVirtualPath(
            RequestContext requestContext,
            RouteValueDictionary values)
        {
            // bind domain
            var domain = this._domainParser.Bind(requestContext.RouteData.Values, values);
            if (domain == null) return null;

            // bind path
            var innerValues = new RouteValueDictionary();
            // 避免将域名相关的RouteValue转换为QueryString
            innerValues.Concat(values).RemoveKeys(this._domainParser.Segments);
            var pathData = base.GetVirtualPath(requestContext, innerValues);
            if (pathData == null) return null;

            // merge
            pathData.Route = this;
            pathData.VirtualPath = Merge(requestContext.HttpContext, domain, pathData.VirtualPath);

            return pathData;
        }

        private static string Merge(HttpContextBase context, string domain, string path)
        {
            var domainWithSlash = domain + "/";
            var ignoreDomain = context.Request.Url.ToString().StartsWith(domainWithSlash);
            return ignoreDomain ? path : domainWithSlash + path;
        }

    }
}
