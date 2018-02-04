using System.Collections.Generic;
using System.Web.Mvc;

namespace Lavie.Modules.Admin.Models
{
    public abstract class ModuleSubMenuItem
    {
        //菜单层级
        public int MenuLevel { get; set; }
        //是否默认选中
        public bool IsDefault { get; set; }
        //权限（只是用于控制菜单显示，并无实际约束能力）
        public string Permission { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }
    }
}
