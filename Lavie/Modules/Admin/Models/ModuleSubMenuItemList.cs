using System.Collections.Generic;

namespace Lavie.Modules.Admin.Models
{
    public class ModuleSubMenuItemList : List<ModuleSubMenuItem>
    {
        public ModuleSubMenuItemList(string title)
        {
            Title = title;
        }
        //主菜单标题
        public string Title { get; set; }
    }
}
