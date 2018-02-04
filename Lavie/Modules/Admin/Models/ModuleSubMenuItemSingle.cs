using System.Collections.Generic;
using System.Web.Mvc;
using Lavie.Extensions;
using System;
using System.Web.Routing;

namespace Lavie.Modules.Admin.Models
{
    public class ModuleSubMenuItemSingle : ModuleSubMenuItem
    {
        //菜单标题
        public string Title { get; set; }
        //菜单页RouteName
        public string SubMenuRouteName { get; set; }
        //菜单页RouteValues
        public object SubMenuContentRouteValues { get; set; }
        //内容页RouteName
        public string ContentRouteName { get; set; }
        //内容页RouteValues
        public object ContentRouteValues { get; set; }

    }
}
