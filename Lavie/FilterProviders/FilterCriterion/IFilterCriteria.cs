using System.Web.Mvc;

namespace Lavie.FilterProviders.FilterCriterion
{
    public interface IFilterCriteria
    {
        bool Match(ControllerContext controllerContext, ActionDescriptor actionDescriptor);
    }
}
