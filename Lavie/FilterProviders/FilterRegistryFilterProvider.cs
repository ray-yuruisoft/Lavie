using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lavie.FilterProviders.FilterCriterion;

namespace Lavie.FilterProviders
{
    public class FilterRegistryFilterProvider :IFilterProvider
    {
        private readonly List<FilterRegistryItem> _items;

        public FilterRegistryFilterProvider()
        {
            this._items = new List<FilterRegistryItem>();
        }

        public void Add(FilterRegistryItem registryItem)
        {
            _items.Add(registryItem);
        }

        #region 包含过滤条件

        public void Add(IEnumerable<IFilterCriteria> filterCriteria, Filter filter)
        {
            Add(new FilterRegistryItem(filterCriteria, filter));
        }
        public void Add(IEnumerable<IFilterCriteria> filterCriteria, object filterInstance)
        {
            Add(filterCriteria,  new Filter(filterInstance, FilterScope.Last, Int32.MaxValue));
        }
        public void Add(IEnumerable<IFilterCriteria> filterCriteria, object filterInstance, FilterScope scope, int? order)
        {
            Add(filterCriteria, new Filter(filterInstance, scope, order));
        }
        public void Add(IEnumerable<IFilterCriteria> filterCriteria, Type filterType)
        {
            Add(filterCriteria, new Filter(DependencyResolver.Current.GetService(filterType), FilterScope.Last, Int32.MaxValue));
        }
        public void Add(IEnumerable<IFilterCriteria> filterCriteria, Type filterType, FilterScope scope, int? order)
        {
            Add(filterCriteria, new Filter(DependencyResolver.Current.GetService(filterType), scope, order));
        }
        public void Add(IEnumerable<IFilterCriteria> filterCriteria, Func<Filter> getFilter)
        {
            Add(filterCriteria, getFilter());
        }
        public void Add(IEnumerable<IFilterCriteria> filterCriteria, Func<object> getFilter)
        {
            Add(filterCriteria, new Filter(getFilter(), FilterScope.Last, Int32.MaxValue));
        }

        #endregion 

        #region 不包含过滤条件，类似于GlobalFilters.Filters的作用，但是可以设置顺序
        public void Add(Filter filter)
        {
            Add(null, filter);
        }
        public void Add(object filterInstance)
        {
            Add(null, filterInstance);
        }
        public void Add(object filterInstance, FilterScope scope, int? order)
        {
            Add(null, filterInstance, scope, order);
        }
        public void Add(Type filterType)
        {
            Add(null, filterType);
        }
        public void Add(Type filterType, FilterScope scope, int? order)
        {
            Add(null, filterType, scope, order);
        }
        public void Add(Func<object> getFilter)
        {
            Add(null, getFilter);
        }

        #endregion

        public void Clear()
        {
            _items.Clear();            
        }

        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var result = from f in _items
                   where f.Match(controllerContext, actionDescriptor)
                   select f.Filter;
            return result;
        }
    }
}
