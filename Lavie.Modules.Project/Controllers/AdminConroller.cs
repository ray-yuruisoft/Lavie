using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lavie.Modules.Project.Controllers
{
    public class AdminConroller : Controller
    {
        public object Index()
        {
            return Redirect("/Manager/Index");
        }
    }
}
