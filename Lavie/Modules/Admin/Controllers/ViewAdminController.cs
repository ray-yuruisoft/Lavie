using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Infrastructure;
using Lavie.Extensions;

namespace Lavie.Modules.Admin.Controllers
{
    public class ViewAdminController : Controller
    {
        private readonly IModule _currentModule;
        private readonly string ProductionCoreTemplate = "<!DOCTYPE html>" +
            "<html>" +
            "<head>" +
            "  <meta charset=\"utf-8\">" +
            "  <title>{0}</title >" +
            "  <link href=\"/static/css/modules/{2}.css\" rel=\"stylesheet\">" +
            "</head>" +
            "<body>" +
            "<div id=\"app\"></div>{3}" +
            "<script type = \"text/javascript\" src=\"{1}/static/js/manifestcore.js\"></script>" +
            "<script type = \"text/javascript\" src=\"{1}/static/js/vendorcore.js\"></script>" +
            "<script type = \"text/javascript\" src=\"{1}/static/js/modules/{2}.js\"></script>" +
            "</body>" +
            "</html>"
            ;
        private readonly string DevelopmentCoreModule = "<!DOCTYPE html>" +
            "<html>" +
            "<head>" +
            "  <meta charset=\"utf-8\">" +
            "  <title>{0}</title >" +
            "</head>" +
            "<body>" +
            "<div id=\"app\"></div>{3}" +
            "<script type = \"text/javascript\" src=\"{1}/modules/{2}.js\"></script>" +
            "</body>" +
            "</html>"
            ;
        private readonly string ProductionTemplate = "<!DOCTYPE html>" +
            "<html>" +
            "<head>" +
            "  <meta charset=\"utf-8\">" +
            "  <title>{0}</title >" +
            "  <link href=\"/static/css/modules/{2}.css\" rel=\"stylesheet\">" +
            "</head>" +
            "<body>" +
            "<div id=\"app\"></div>{3}" + 
            "<script type = \"text/javascript\" src=\"{1}/static/js/manifest.js\"></script>" +
            "<script type = \"text/javascript\" src=\"{1}/static/js/vendor.js\"></script>" +
            "<script type = \"text/javascript\" src=\"{1}/static/js/modules/{2}.js\"></script>" +
            "</body>" +
            "</html>"
            ;
        private readonly string DevelopmentModule = "<!DOCTYPE html>" +
            "<html>" +
            "<head>" +
            "  <meta charset=\"utf-8\">" +
            "  <title>{0}</title >" +
            "</head>" +
            "<body>" +
            "<div id=\"app\"></div>{3}" +
            "<script type = \"text/javascript\" src=\"{1}/modules/{2}.js\"></script>" +
            "</body>" +
            "</html>"
            ;
       
        public ViewAdminController(IModule currentModule)
        {
            _currentModule = currentModule;
        }
        public ActionResult Login()
        {
            return Content(GenerateHTML(new ViewInput
            {
                Name = "login",
                Title = "系统登录"
            }, true), "text/html");
        }
        public ActionResult Index()
        {
            return Content(GenerateHTML(new ViewInput
            {
                Name = "index",
                Title = "系统管理",
                Components = "signalr",
            }, true), "text/html");
        }
        public ActionResult ViewCore(ViewInput viewInput)
        {
            return Content(GenerateHTML(viewInput, true), "text/html");
        }
        public ActionResult View(ViewInput viewInput)
        {
            return Content(GenerateHTML(viewInput), "text/html");
        }
        private string GenerateHTML(ViewInput viewInput, bool isCore = false)
        {
            var isDevelopment = String.Compare(_currentModule.Settings.GetString(isCore ? "CoreEnvironment" : "Environment", String.Empty), "development", true) == 0;
            var host = _currentModule.Settings.GetString(isCore ? "CoreEnvironmentHost" : "EnvironmentHost", String.Empty).Split(';');

            if (host.Length != 2)
            {
                return "请检查 Lavie.config 配置文件，确保当前模块的 CoreEnvironmentHost 和 EnvironmentHost 配置正确。";
            }
            // 模板有4种
            string template;
            if (isCore)
            {
                template = isDevelopment ? DevelopmentCoreModule : ProductionCoreTemplate;
            }
            else
            {
                template = isDevelopment ? DevelopmentModule : ProductionTemplate;
            }

            var componentScripts = new StringBuilder();
            if (!viewInput.Components.IsNullOrWhiteSpace())
            {
                var components = viewInput.Components.Split(',', ';');
                foreach (var compent in components)
                {
                    switch (compent.ToLower())
                    {
                        case "ckfinder":
                            // ckfinder 需要获取语言包，涉及跨域，故总是从服务器取脚本
                            componentScripts.AppendFormat("<script type = \"text/javascript\" src=\"{0}/ckfinderscripts/ckfinder.js\"></script>", host[1]);
                            break;
                        case "signalr":
                            componentScripts.AppendFormat("<script type = \"text/javascript\" src=\"{0}/scripts/jquery.min.js\"></script>", host[1]);
                            componentScripts.AppendFormat("<script type = \"text/javascript\" src=\"{0}/scripts/jquery.signalR-2.2.2.min.js\"></script>", host[1]);
                            componentScripts.AppendFormat("<script type = \"text/javascript\" src=\"{0}/signalr/hubs\"></script>", host[1]);
                            break;
                        default:
                            break;
                    }
                }
            }

            var html = String.Format(template, viewInput.Title, isDevelopment ? host[0] : host[1], viewInput.Name, componentScripts.ToString());
            return html;
        }
        public class ViewInput
        {
            public string Title { get; set; }
            public string Name { get; set; }
            public string Components { get; set; } // 以半角逗号或分好分隔
        }
    }
}
