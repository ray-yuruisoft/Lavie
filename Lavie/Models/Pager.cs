using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Extensions;
using Lavie.Html;
using Lavie.Utilities.Exceptions;

namespace Lavie.Models
{
    public class Pager
    {
        private readonly IPageSkin _pageSkin;
        private readonly ViewContext _viewContext;
        private readonly int _pageSize;
        private readonly int _currentPage;
        private readonly int _totalItemCount;
        private readonly RouteValueDictionary _linkWithoutPageValuesDictionary;

        public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary)
            : this(DependencyResolver.Current.GetService<IPageSkin>(), viewContext, pageSize, currentPage, totalItemCount, valuesDictionary)
        { 
        
        }
        public Pager(IPageSkin pageSkin,ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary)
        {
            Guard.ArgumentNotNull(pageSkin, "pageSkin");
            Guard.ArgumentNotNull(viewContext, "viewContext");

            this._pageSkin = pageSkin;
            this._viewContext = viewContext;
            this._pageSize = pageSize;
            this._currentPage = currentPage;
            this._totalItemCount = totalItemCount;
            this._linkWithoutPageValuesDictionary = viewContext.RouteData.Values;
            foreach(var d in valuesDictionary)
            {
                if (_linkWithoutPageValuesDictionary.ContainsKey(d.Key))
                    _linkWithoutPageValuesDictionary[d.Key] = d.Value;
                else
                    _linkWithoutPageValuesDictionary.Add(d.Key,d.Value);
            }
            this._linkWithoutPageValuesDictionary.Remove("pageSize");
            this._linkWithoutPageValuesDictionary.Remove("pageNumber");

        }

        public MvcHtmlString RenderHtml()
        {
            int pageCount = (int)Math.Ceiling(this._totalItemCount / (double)this._pageSize);
            int nrOfPagesToDisplay = 10;

            var sb = new StringBuilder();

            sb.Append(_pageSkin.InfoFormat.FormatWith(_currentPage, pageCount, _totalItemCount, _pageSize));

            // First,Previous
            if (this._currentPage > 1)
            {
                sb.Append(GeneratePageLink(_pageSkin.FirstLinkText, 1));
                sb.Append(GeneratePageLink(_pageSkin.PreviousLinkText, this._currentPage - 1));
            }
            else
            {
                sb.Append(_pageSkin.FirstLinkDisabledText);
                sb.Append(_pageSkin.PreviousLinkDisabledText);
            }

            int start = 1;
            int end = pageCount;

            if (pageCount > nrOfPagesToDisplay)
            {
                int middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                int below = (this._currentPage - middle);
                int above = (this._currentPage + middle);

                if (below < 4)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 4))
                {
                    above = pageCount;
                    below = (pageCount - nrOfPagesToDisplay);
                }

                start = below;
                end = above;
            }

            if (start > 3)
            {
                sb.Append(GeneratePageLink("1", 1));
                sb.Append(GeneratePageLink("2", 2));
                sb.Append(_pageSkin.LinkSeparator);
            }
            for (int i = start; i <= end; i++)
            {
                sb.Append(GeneratePageLink(i.ToString(), i));

                /*
                if (i == this._currentPage)
                {
                    sb.AppendFormat(_pageSkin.CurrentLinkFormat, i);
                }
                else
                {
                    sb.Append(GeneratePageLink(i.ToString(), i));
                }
                */
            }
            if (end < (pageCount - 3))
            {
                sb.Append(_pageSkin.LinkSeparator);
                sb.Append(GeneratePageLink((pageCount - 1).ToString(), pageCount - 1));
                sb.Append(GeneratePageLink(pageCount.ToString(), pageCount));
            }

            // Next,Last
            if (this._currentPage < pageCount)
            {
                sb.Append(GeneratePageLink(_pageSkin.NextLinkText, this._currentPage + 1));
                sb.Append(GeneratePageLink(_pageSkin.LastLinkText, pageCount));
            }
            else
            {
                sb.Append(_pageSkin.NextLinkDisabledText);
                sb.Append(_pageSkin.LastLinkDisabledText);
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        private string GeneratePageLink(string linkText, int pageNumber)
        {
            var pageLinkValueDictionary = GetNewRouteValus(_linkWithoutPageValuesDictionary);
            pageLinkValueDictionary.Add("pageSize", _pageSize);
            pageLinkValueDictionary.Add("pageNumber", pageNumber);
            //var virtualPathData = this._viewContext.RouteData.Route.GetVirtualPath(this.viewContext.RequestContext, pageLinkValueDictionary);
            var virtualPathData = RouteTable.Routes.GetVirtualPath(_viewContext.RequestContext, pageLinkValueDictionary);

            if (virtualPathData == null)
            {
                StringBuilder exception = new StringBuilder();
                exception.AppendLine("Can not get the virtual path.");
                exception.AppendLine("Paging Route Values:");
                pageLinkValueDictionary.ForEach(m => exception.AppendFormat("{0}:{1}\r\n",m.Key,m.Value));

                throw new InvalidOperationException(exception.ToString());
            }

            return _pageSkin.LinkFormat.FormatWith(UrlHelperExtensions.EnsureUrl(virtualPathData.VirtualPath), linkText);
        }

        private RouteValueDictionary GetNewRouteValus(RouteValueDictionary values)
        {
            // 目的：
            // 1、将集合类的值转换为以逗号分隔的字符串
            // 2、清除空值
            var pageLinkValueDictionary = new RouteValueDictionary();
            foreach (KeyValuePair<string, object> value in values)
            {
                if (value.Value == null) continue;

                // 字符串
                var stringValue = value.Value as String;
                if (value.Value is String)
                {
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
