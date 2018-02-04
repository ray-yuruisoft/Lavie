using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Utilities.Exceptions;

namespace Lavie.FilterProviders.FilterCriterion
{
    public class RouteValueFilterCriteria: IFilterCriteria
    {
        private string _routeKey;
        private string _routeValue;
        private bool _ignoreCase;
        public RouteValueFilterCriteria(string routeKey, string routeValue)
            : this(routeKey, routeValue, false)
        {
        }
        public RouteValueFilterCriteria(string routeKey, string routeValue, bool ignoreCase)
        {
            Guard.ArgumentNotNull(routeKey, "routeKey");
            Guard.ArgumentNotNull(routeValue, "routeValue");

            this._routeKey = routeKey;
            this._routeValue = routeValue;
            this._ignoreCase = ignoreCase;
        }

        #region IFilterCriteria Members

        public bool Match(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            string currentValue = controllerContext.RouteData.Values[_routeKey] as String;
            return String.Compare(currentValue, _routeValue, _ignoreCase) == 0;
        }

        #endregion

    }
}
