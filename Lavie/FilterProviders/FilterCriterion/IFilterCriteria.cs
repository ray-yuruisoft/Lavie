using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lavie.FilterProviders.FilterCriterion
{
    public interface IFilterCriteria
    {
        bool Match(ControllerContext controllerContext, ActionDescriptor actionDescriptor);
    }
}
