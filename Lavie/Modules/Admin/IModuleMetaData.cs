using System.Collections.Generic;
using Lavie.Infrastructure;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;

namespace Lavie.Modules.Admin
{
    public interface IModuleMetaData : IModule
    {
        List<ModuleMenu> GetModuleMenus();
        List<PermissionInput> GetModulePermissions();
    }
}
