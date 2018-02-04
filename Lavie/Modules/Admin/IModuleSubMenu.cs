using System.Threading.Tasks;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.ViewModels;

namespace Lavie.Modules.Admin
{
    /// <summary>
    /// 用户约定Controller实现名为SubMenu的Action
    /// </summary>
    public interface IModuleSubMenu
    {
        Task<LavieViewModelItem<ModuleSubMenuItemList>> SubMenu();
    }
}
