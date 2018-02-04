using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Utilities.Exceptions;

namespace Lavie.FilterProviders.FilterCriterion
{
    public class RouteFilterCriteria : IFilterCriteria
    {
        private readonly RouteCollection _routeTable;
        private readonly List<RouteBase> _routes;

        public RouteFilterCriteria(RouteCollection routeTable)
        {
            this._routeTable = routeTable;
            this._routes = new List<RouteBase>();
        }
        public void AddRoute(RouteBase route)
        {
            _routes.Add(route);
        }
        public void AddRoute(string routeName)
        {
            Guard.ArgumentNotNull(routeName, "routeName");

            var route = _routeTable[routeName];

            if (route == null)
                throw new ArgumentOutOfRangeException("routeName");

            _routes.Add(route);
        }

        #region IFilterCriteria Members

        public bool Match(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            return _routes.Any(r => r == controllerContext.RouteData.Route);
        }

        #endregion


    }
}
