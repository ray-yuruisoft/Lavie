using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Lavie.Extensions;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Lavie.Routing
{
    internal class DomainParser
    {
        private ParsedRoute _parsedRoute;

        public DomainParser(string pattern)
        {
            this.Pattern = pattern;

            this.Segments = CaptureSegments(pattern);

            //如：http://www.lavie.net/home/index,替换为 http/www/lavie/net/home/index
            string routePattern = pattern.Replace(Uri.SchemeDelimiter, "/").Replace('.', '/');
            this._parsedRoute = RouteParser.Parse(routePattern);
        }

        private static ReadOnlyCollection<string> CaptureSegments(string domainPattern)
        {
            var regex = @"{\*?([^}]+)}";
            var matches = Regex.Matches(domainPattern, regex).Cast<Match>();
            var segments = matches.Select(m => m.Groups[1].Value);
            return new ReadOnlyCollection<string>(segments.ToList());
        }

        public ReadOnlyCollection<string> Segments { get; private set; }

        public string Pattern { get; private set; }

        public RouteValueDictionary Match(Uri uri)
        {
            var toParse = ConvertDomainToPath(uri);
            var domainValues = this._parsedRoute.Match(toParse, null);
            if (domainValues == null) return null;

            var result = new RouteValueDictionary();
            foreach (var pair in domainValues)
            {
                var value = pair.Value as string;
                if (value != null)
                {
                    result.Add(pair.Key, value.Replace('/', '.'));
                }
                else
                {
                    result.Add(pair.Key, pair.Value);
                }
            }

            return result;
        }

        public string Bind(RouteValueDictionary currentValues, RouteValueDictionary values)
        {
            currentValues = currentValues ?? new RouteValueDictionary();
            values = values ?? new RouteValueDictionary();

            var acceptValues = new RouteValueDictionary();
            foreach (var name in this.Segments)
            {
                object segmentValue;
                if (values.TryGetValue(name, out segmentValue) ||
                    currentValues.TryGetValue(name, out segmentValue))
                {
                    acceptValues.Add(name, segmentValue);
                }
                else
                {
                    return null;
                }
            }

            var boundUrl = this._parsedRoute.Bind(null, acceptValues, null, null);
            if (boundUrl == null) return null;

            return ConvertPathToDomain(boundUrl.Url);
        }

        private static string ConvertPathToDomain(string url)
        {
            var domainParts = url.Split('/');
            var domain = domainParts[0];
            for (int i = 1; i < domainParts.Length; i++)
            {
                domain += (i == 1 ? Uri.SchemeDelimiter : ".");
                domain += domainParts[i];
            }

            return domain;
        }

        private static string ConvertDomainToPath(Uri uri)
        {
            return uri.Scheme + "/" + uri.Host.Replace('.', '/') + (uri.IsDefaultPort ? String.Empty : ":" + uri.Port.ToString());
        }

    }
}
