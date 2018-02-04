using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lavie.FilterProviders
{
    public class FilterRegistry
    {

        static FilterRegistry()
        {
            Filters = new FilterRegistryFilterProvider();
        }

        public static FilterRegistryFilterProvider Filters
        {
            get;
            private set;
        }
    }
}
