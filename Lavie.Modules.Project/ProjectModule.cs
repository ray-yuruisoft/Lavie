using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Lavie.ActionFilters.Authorization;
using Lavie.FilterProviders;
using Lavie.FilterProviders.FilterCriterion;
using Lavie.Infrastructure;
using Lavie.Modules.Admin;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Modules.Admin.Repositories;
using Lavie.Modules.Project.Controllers;
using Lavie.Modules.Project.Repositories;
using Lavie.Routing;

namespace Lavie.Modules.Project
{
    public class ProjectModule : Module, IModuleMetaData
    {
        #region IModule Members

        public override string ModuleName
        {
            get { return "Project"; }
        }

        public override void Initialize()
        {
            DependencyInjector
                .RegisterType<IStaffRepository, StaffRepository>()
                .RegisterType<IGroupRepository, GroupRepository>()
                .RegisterType<IFlowRepository, FlowRepository>()
                .RegisterType<IReportRepository, ReportRepository>()
                ;
        }

        public override void WebApiConfigRegister(HttpConfiguration config)
        {
            base.WebApiConfigRegister(config);
        }

        public override void RegisterRoutes()
        {
            var controllerNamespaces = new string[] { "Lavie.Modules.Project.Controllers" };

            #region 法思特OA Api 通配

            //MapRawRoute(
            //    "Admin.Project.Default",
            //    "",
            //    new { controller = "Admin", action = "Index" },
            //    null,
            //    controllerNamespaces
            //);
            MapPrefixRoute(
                "Admin.Project",
                "Api/Project/{controller}/{action}",
                new { controller = "controller", action = "action" },
                null,
                controllerNamespaces
            );

            #endregion

            #region 部门管理 通配
            MapPrefixRoute(
                "Admin.Project.GroupConfig",
                "Project/GroupConfig/{action}",
                new { controller = "GroupConfig", action = "action" },
                null,
                controllerNamespaces
            );
            #endregion

            #region 人员管理
            #region 下拉菜单
            MapPrefixRoute(
                "Admin.Project.UserManager.SubMenu",
                "Project/UserManager/SubMenu",
                new { controller = "UserManager", action = "SubMenu" },
                null,
                controllerNamespaces
            );
            #endregion
            #region 人员信息管理
            MapPrefixRoute(
                "Admin.Project.UserManager.Distribute",
                "Project/UserManager/Distribute",
                new { controller = "UserManager", action = "Distribute" },
                null,
                controllerNamespaces
            );
            MapPrefixRoute(
                "Admin.Project.UserManager.UserRemove",
                "Project/UserManager/UserRemove",
                new { controller = "UserManager", action = "UserRemove" },
                null,
                controllerNamespaces
            );
            MapPrefixRoute(
                "Admin.Project.UserManager.UserList",
                "Project/UserManager/UserList",
                new { controller = "UserManager", action = "UserList" },
                null,
                controllerNamespaces
            );
            MapPrefixRoute(
                "Admin.Project.UserManager.UserAddOrEdit",
                "Project/UserManager/UserAddOrEdit",
                new { controller = "UserManager", action = "UserAddOrEdit" },
                null,
                controllerNamespaces
            );
            MapPrefixRoute(
                "Admin.Project.UserManager.UserEditSave",
                "Project/UserManager/UserEditSave",
                new { controller = "UserManager", action = "UserEditSave" },
                null,
                controllerNamespaces
            );
            MapPrefixRoute(
                "Admin.Project.UserManager.UserAddSave",
                "Project/UserManager/UserAddSave",
                new { controller = "UserManager", action = "UserAddSave" },
                null,
                controllerNamespaces
            );
            #endregion
            #region 组织机构管理
            MapPrefixRoute(
                "Admin.Project.UserManager.SiteConfig",
                "Project/UserManager/SiteConfig",
                new { controller = "UserManager", action = "SiteConfig" },
                null,
                controllerNamespaces
            );
            MapPrefixRoute(
                "Admin.Project.UserManager.SiteConfigAdd",
                "Project/UserManager/SiteConfigAdd",
                new { controller = "UserManager", action = "SiteConfigAdd" },
                null,
                controllerNamespaces
            );
            MapPrefixRoute(
                "Admin.Project.UserManager.GroupList",
                "Project/UserManager/GroupList",
                new { controller = "UserManager", action = "GroupList" },
                null,
                controllerNamespaces
            );
            MapPrefixRoute(
               "Admin.Project.UserManager.GroupEdit",
               "Project/UserManager/GroupEdit",
               new { controller = "UserManager", action = "GroupEdit" },
               null,
               controllerNamespaces
           );
            MapPrefixRoute(
               "Admin.Project.UserManager.GroupRemove",
               "Project/UserManager/GroupRemove",
               new { controller = "UserManager", action = "GroupRemove" },
               null,
               controllerNamespaces
           );
            MapPrefixRoute(
               "Admin.Project.UserManager.GroupMove",
               "Project/UserManager/GroupList",
               new { controller = "UserManager", action = "GroupMove" },
               null,
               controllerNamespaces
           );
            MapPrefixRoute(
               "Admin.Project.UserManager.GroupSave",
               "Project/UserManager/GroupSave",
               new { controller = "UserManager", action = "GroupSave" },
               null,
               controllerNamespaces
           );
            MapPrefixRoute(
              "Admin.Project.UserManager.GroupAdd",
              "Project/UserManager/GroupAdd",
              new { controller = "UserManager", action = "GroupAdd" },
              null,
              controllerNamespaces
          );
            #endregion
            #endregion

            #region 后台管理子菜单(管理员)

            MapPrefixRoute(
                "Admin.Project.StaffSubMenu",
                "Project/StaffSubMenu",
                new { controller = "Staff", action = "StaffSubMenu" },
                null,
                controllerNamespaces
            );

            MapPrefixRoute(
                "Admin.Project.CalculationSubMenu",
                "Project/CalculationSubMenu",
                new { controller = "Project", action = "CalculationSubMenu" },
                null,
                controllerNamespaces
            );

            MapPrefixRoute(
                "Admin.Project.RecordSubMenu",
                "Project/RecordSubMenu",
                new { controller = "Project", action = "RecordSubMenu" },
                null,
                controllerNamespaces
            );

            #endregion

            #region 组织架构管理(人事行政经理)

            #endregion

            #region 员工(人事行政经理)

            MapPrefixRoute(
                "Admin.Project.StaffList",
                "Project/StaffList/{pageSize}/{pageNumber}",
                new { controller = "Staff", action = "StaffList", pageSize = 20, pageNumber = 1 },
                new { pageSize = new IsInt(1, Int32.MaxValue), pageNumber = new IsInt(1, Int32.MaxValue) },
                controllerNamespaces
                );
            MapPrefixRoute(
                "Admin.Project.StaffAdd",
                "Project/StaffAdd",
                new { controller = "Staff", action = "StaffAddOrEdit" },
                null,
                controllerNamespaces
                );
            MapPrefixRoute(
                "Admin.Project.StaffEdit",
                "Project/StaffEdit/{userID}",
                new { controller = "Staff", action = "StaffAddOrEdit" },
                new { userID = new IsInt(1, Int32.MaxValue) },
                controllerNamespaces
                );
            MapPrefixRoute(
                "Admin.Project.StaffSave",
                "Project/StaffSave/{userID}",
                new { controller = "Staff", action = "StaffSave", userID = UrlParameter.Optional },
                new { userID = new IsInt(1, Int32.MaxValue, true) },
                controllerNamespaces
                );

            #endregion

            #region 计算

            MapPrefixRoute(
                "Admin.Project.CalculationList",
                "Project/CalculationList/{pageSize}/{pageNumber}",
                new { controller = "Project", action = "CalculationList", pageSize = 20, pageNumber = 1 },
                new { pageSize = new IsInt(1, Int32.MaxValue), pageNumber = new IsInt(1, Int32.MaxValue) },
                controllerNamespaces
                );
            MapPrefixRoute(
                "Admin.Project.CalculationAdd",
                "Project/CalculationAdd",
                new { controller = "Project", action = "CalculationAddOrEdit" },
                null,
                controllerNamespaces
                );
            MapPrefixRoute(
                "Admin.Project.CalculationEdit",
                "Project/CalculationEdit/{calculationID}",
                new { controller = "Project", action = "CalculationAddOrEdit" },
                new { calculationID = new IsInt(1, Int32.MaxValue) },
                controllerNamespaces
                );
            MapPrefixRoute(
                "Admin.Project.CalculationSave",
                "Project/CalculationSave/{calculationID}",
                new { controller = "Project", action = "CalculationSave", calculationID = UrlParameter.Optional },
                new { calculationID = new IsInt(1, Int32.MaxValue, true) },
                controllerNamespaces
                );

            #endregion

            #region 记录

            MapPrefixRoute(
                "Admin.Project.RecordList",
                "Project/RecordList/{pageSize}/{pageNumber}",
                new { controller = "Project", action = "RecordList", pageSize = 20, pageNumber = 1 },
                new { pageSize = new IsInt(1, Int32.MaxValue), pageNumber = new IsInt(1, Int32.MaxValue) },
                controllerNamespaces
                );
            MapPrefixRoute(
                "Admin.Project.RecordAdd",
                "Project/RecordAdd",
                new { controller = "Project", action = "RecordAddOrEdit" },
                null,
                controllerNamespaces
                );
            MapPrefixRoute(
                "Admin.Project.RecordEdit",
                "Project/RecordEdit/{recordID}",
                new { controller = "Project", action = "RecordAddOrEdit" },
                new { recordID = new IsInt(1, Int32.MaxValue) },
                controllerNamespaces
                );
            MapPrefixRoute(
                "Admin.Project.RecordSave",
                "Project/RecordSave/{recordID}",
                new { controller = "Project", action = "RecordSave", RecordID = UrlParameter.Optional },
                new { recordID = new IsInt(1, Int32.MaxValue, true) },
                controllerNamespaces
                );

            #endregion


        }

        public override void RegisterFilters(FilterRegistryFilterProvider filterRegistry, GlobalFilterCollection globalFilters)
        {

            //var permisssionCriteria = new ControllerTypeFilterCriteria();
            //permisssionCriteria.AddType(typeof(StaffController));
            //filterRegistry.Add(new[] { permisssionCriteria }, new AdvancedAuthorizationFilter(DependencyInjector, user => user.HasPermission("人事管理")));

            #region 人事信息管理
            //ControllerActionFilterCriteria userInfoManager = new ControllerActionFilterCriteria();
            //userInfoManager.AddMethod<UserManagerController>(s => s.UserAddOrEdit(null));
            //filterRegistry.Add(new[] { userInfoManager }, new AuthorizationFilter(DependencyInjector, user => user.HasPermission("后台管理")));
            #endregion

        }
        #endregion

        #region IModuleMetaData

        public List<ModuleMenu> GetModuleMenus()
        {
            return new List<ModuleMenu>
            {

                new ModuleMenu
                {
                    Title = "人事管理",
                    Type = ModuleMenuType.Item,
                    LinkRouteName = "Admin.View",
                    LinkRouteValues = new { Title = "人事管理", Name = "staff" , Components = "ckfinder"}, // 因为 Webpack 调试服务器区分大小写，所以统一使用小写
                    Validator = u => u.IsInRole("人事主管")
                },
                new ModuleMenu
                {
                    Title = "员工列表",
                    Type = ModuleMenuType.Item,
                    LinkRouteName = "Admin.View",
                    LinkRouteValues = new { Title = "人事管理", Name = "stationstaff", Components = "ckfinder" }, // 因为 Webpack 调试服务器区分大小写，所以统一使用小写
                    Validator = u =>  u.IsInRole("站长")
                },
                new ModuleMenu
                {
                    Title = "员工列表",
                    Type = ModuleMenuType.Item,
                    LinkRouteName = "Admin.View",
                    LinkRouteValues = new { Title = "人事管理", Name = "areastaff" }, // 因为 Webpack 调试服务器区分大小写，所以统一使用小写
                    Validator = u => u.IsInRole("区域经理")|| u.IsInRole("城市经理"),
                },
                new ModuleMenu
                {
                    Title = "请假列表",
                    Type = ModuleMenuType.Item,
                    LinkRouteName = "Admin.View",
                    LinkRouteValues = new { Title = "人事管理", Name = "stationleave" }, // 因为 Webpack 调试服务器区分大小写，所以统一使用小写
                    Validator = u => u.IsInRole("站长"),
                },
                new ModuleMenu
                {
                    Title = "部门管理",
                    Type = ModuleMenuType.Item,
                    LinkRouteName = "Admin.View",
                    LinkRouteValues = new { Title = "请假管理", Name = "department" }, // 因为 Webpack 调试服务器区分大小写，所以统一使用小写
                    Validator = u => u.IsInRole("人事主管")
                },
                new ModuleMenu
                {
                    Title = "骑手考勤",
                    Type = ModuleMenuType.Item,
                    LinkRouteName = "Admin.View",
                    LinkRouteValues = new { Title = "骑手考勤", Name = "importattendance", Components ="ckfinder" }, // 因为 Webpack 调试服务器区分大小写，所以统一使用小写
                    Validator = u => u.IsInRole("人事主管"),

                },
                new ModuleMenu
                {
                    Title = "骑手工资计算",
                    Type = ModuleMenuType.Item,
                    LinkRouteName = "Admin.View",
                    LinkRouteValues = new { Title = "工资计算", Name = "evaluationofwages", Components ="ckfinder" }, // 因为 Webpack 调试服务器区分大小写，所以统一使用小写
                    Validator = u => u.IsInRole("人事主管"),
                }

            };
        }
        public List<ModuleMenuItem> GetModuleMenuItems()
        {
            return new List<ModuleMenuItem>
            {
                new ModuleMenuItem
                {
                    Title = "人员管理",
                    SubMenuRouteName = "Admin.Project.UserManager.SubMenu",
                    ContentRouteName = "Admin.Project.UserManager.UserList",
                    Permission="后台管理"
                },
                new ModuleMenuItem
                {
                    Title = "人员信息管理",
                    SubMenuRouteName = "Admin.Project.StaffSubMenu",
                    ContentRouteName = "Admin.Project.StaffList",
                    Permission="人员信息管理"
                },
                new ModuleMenuItem
                {
                    Title = "计算",
                    SubMenuRouteName = "Admin.Project.CalculationSubMenu",
                    ContentRouteName = "Admin.Project.CalculationList",
                    Role = "总部HR"
                },
                new ModuleMenuItem
                {
                    Title = "记录",
                    SubMenuRouteName = "Admin.Project.RecordSubMenu",
                    ContentRouteName = "Admin.Project.RecordList",
                    Role = "总部HR"
                },
            };
        }
        public List<PermissionInput> GetModulePermissions()
        {
            var rootPermission = new Guid("{DA7E7641-5E4D-4133-8837-CEBAD92B884D}");
            var permissions = new List<PermissionInput>()
            {
                new PermissionInput{ PermissionID = rootPermission, Name="系统", ModuleName= this.ModuleName},
                new PermissionInput{ PermissionID = new Guid("{A5CEB900-FB93-47AA-A20C-99DEB472F061}"), ParentID = rootPermission, Name="人员信息管理(含权限)", ModuleName= this.ModuleName},
                new PermissionInput{ PermissionID = new Guid("{0B933253-5B2A-4400-B391-94F700EA32A6}"), ParentID = rootPermission, Name="人员信息管理", ModuleName= this.ModuleName},
                new PermissionInput{ PermissionID = new Guid("{8E152219-CDC5-4025-8691-706D7F746261}"), ParentID = rootPermission, Name="人事异动申请", ModuleName= this.ModuleName},
                new PermissionInput{ PermissionID = new Guid("{9A9599FA-7516-4C69-BD4A-917E2C50B580}"), ParentID = rootPermission, Name="人事异动审批", ModuleName= this.ModuleName},
                new PermissionInput{ PermissionID = new Guid("{80746549-BE95-4B60-B82B-851D950A6BD4}"), ParentID = rootPermission, Name="请假代申请", ModuleName= this.ModuleName},
                new PermissionInput{ PermissionID = new Guid("{F77F4595-791D-47CA-BA07-E73AC8FBF169}"), ParentID = rootPermission, Name="请假审批", ModuleName= this.ModuleName},

            };

            return permissions;
        }

        #endregion
    }
}
