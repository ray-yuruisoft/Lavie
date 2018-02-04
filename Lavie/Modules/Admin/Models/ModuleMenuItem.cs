
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Lavie.Infrastructure;

namespace Lavie.Modules.Admin.Models
{
    public enum ModuleMenuType
    {
        Item,
        Sub,
        Group
    }
    public class ModuleMenuItem
    {
        //菜单标题
        public string Title { get; set; }
        //子菜单RouteName
        public string SubMenuRouteName { get; set; }
        public object SubMenuRouteValues { get; set; }
        //内容页RouteName
        public string ContentRouteName { get; set; }
        public object ContentRouteValues { get; set; }
        //权限（只是用于控制菜单显示，并无实际约束能力）
        public string Permission { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ModuleMenu
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "type")]
        public ModuleMenuType Type { get; set; } // Type: 0 菜单项(不能包含Children) 1 子菜单(不能链接) 2 菜单组(不能链接)
        [JsonProperty(PropertyName = "children", NullValueHandling = NullValueHandling.Ignore)]
        public List<ModuleMenu> Children { get; set; }
        [JsonProperty(PropertyName = "link", NullValueHandling = NullValueHandling.Ignore)]
        public string Link { get; set; } // 运行时计算
        [JsonProperty(PropertyName = "directly", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Directly { get; set; }
        public string LinkRouteName { get; set; }
        public object LinkRouteValues { get; set; }
        [JsonProperty(PropertyName = "linkTarget", NullValueHandling = NullValueHandling.Ignore)]
        public string LinkTarget { get; set; }
        //权限（只是用于控制菜单显示，并无实际约束能力）
        public string Permission { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }

        public Func<IUser, bool> Validator { get; set; }
    }
}
