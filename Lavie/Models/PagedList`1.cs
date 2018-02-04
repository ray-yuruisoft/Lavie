using System;
using System.Collections.Generic;

namespace Lavie.Models
{
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public SortInfo SortInfo { get; set; }

        #region IPagedList<T> Members

        public int PageNumber { get { return PageIndex + 1; } }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItemCount { get; private set; }
        public int TotalPageCount { get; private set; }
        public bool HasPreviousPage { get; private set; }
        public bool HasNextPage { get; private set; }
        public bool IsFirstPage { get; private set; }
        public bool IsLastPage { get; private set; }

        #endregion

        #region Constructor

        public PagedList(IEnumerable<T> items, int pageIndex, int pageSize, int totalItemCount)
        {
            AddRange(items);
            PageIndex = pageIndex;
            PageSize = pageSize;
            if (totalItemCount > 0)
            {
                TotalItemCount = totalItemCount;
                TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
                HasPreviousPage = (PageIndex > 0);
                HasNextPage = (PageIndex < (TotalPageCount - 1));
                IsFirstPage = (PageIndex <= 0);
                IsLastPage = (PageIndex >= (TotalItemCount - 1));
            }
            SortInfo = new SortInfo();
        }

        public PagedList(IEnumerable<T> items): 
            this(items, 0, 0, 0)
        {
        }

        #endregion

    }

    public interface IPagedList<T> : IEnumerable<T>
    {
        int PageNumber { get; }
        int PageIndex { get; }
        int PageSize { get; }
        int TotalPageCount { get; }
        int TotalItemCount { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }
    }

}
