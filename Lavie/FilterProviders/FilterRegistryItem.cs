using Lavie.FilterProviders.FilterCriterion;
using Lavie.Utilities.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lavie.FilterProviders
{
    public class FilterRegistryItem
    {
        private readonly IEnumerable<IFilterCriteria> _filterCriteria;

        public Filter Filter { get; private set; }

        public FilterRegistryItem(Filter filter)
            : this(null, filter) { }

        public FilterRegistryItem(IEnumerable<IFilterCriteria> filterCriteria, Filter filter)
        {
            Guard.ArgumentNotNull(filter, "filter");

            object filterInstance = filter.Instance;
            if (filterInstance == null)
                throw new NullReferenceException("filter.Instance");
            if (!(filterInstance is IActionFilter
                || filterInstance is IAuthorizationFilter
                || filterInstance is IExceptionFilter
                || filterInstance is IResultFilter))
                throw new ArgumentOutOfRangeException("filter.Instance");

            if (filterCriteria == null)
                filterCriteria = Enumerable.Empty<IFilterCriteria>();

            _filterCriteria = filterCriteria;
            Filter = filter;
        }

        public bool Match(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            // 条件 &&
            //return _filterCriteria.Aggregate(true, (prev, f) => prev && f.Match(controllerContext, actionDescriptor));

            // 条件 ||
            if (!_filterCriteria.Any()) return true;
            return _filterCriteria.Aggregate(false, (prev, f) => prev || f.Match(controllerContext, actionDescriptor));
        }
    }
}
