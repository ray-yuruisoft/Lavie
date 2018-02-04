using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lavie.Modules.Project.Extentions
{
    public class BirthdayModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            var a = controllerContext;
            var b = bindingContext;

            return null;
        }
    }
}
