using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lavie.Modules.Project.Extentions
{
    [AttributeUsage(
     AttributeTargets.Class
   | AttributeTargets.Enum
   | AttributeTargets.Interface
   | AttributeTargets.Parameter
   | AttributeTargets.Struct
   | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]

    public class BirthDayModelBinderAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new BirthdayModelBinder();
        }
    }
}
