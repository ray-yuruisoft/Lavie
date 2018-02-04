using System;
using System.Web.Mvc;
using Lavie.Utilities.Exceptions;

namespace Lavie.FilterProviders.FilterCriterion
{
    public class DataTokenFilterCriteria: IFilterCriteria
    {
        private string _dataTokenKey;
        private string _dataTokenValue;
        private bool _ignoreCase;

        public DataTokenFilterCriteria(string dataTokenKey, string dataTokenValue)
            : this(dataTokenKey, dataTokenValue, false)
        {
        }

        public DataTokenFilterCriteria(string dataTokenKey, string dataTokenValue, bool ignoreCase)
        {
            Guard.ArgumentNotNull(dataTokenKey, "dataTokenKey");
            Guard.ArgumentNotNull(dataTokenValue, "dataTokenValue");

            this._dataTokenKey = dataTokenKey;
            this._dataTokenValue = dataTokenValue;
            this._ignoreCase = ignoreCase;
        }

        #region IFilterCriteria Members

        public bool Match(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            string currentValue = controllerContext.RouteData.DataTokens[_dataTokenKey] as String;
            return String.Compare(currentValue, _dataTokenValue, _ignoreCase) == 0;
        }

        #endregion

    }
}
