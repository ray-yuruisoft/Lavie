using System.Collections.Generic;

namespace Lavie.Modules.Admin.Models
{
    /// <summary>
    /// 菜单组
    /// </summary>
    public class ModuleSubMenuItemGroup : ModuleSubMenuItem
    {
        public List<ModuleSubMenuItem> Items { get; set; }
    }
}
