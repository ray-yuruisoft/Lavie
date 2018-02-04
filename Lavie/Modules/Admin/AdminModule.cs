using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Lavie.ActionFilters.Action;
using Lavie.ActionFilters.Authorization;
using Lavie.FilterProviders;
using Lavie.FilterProviders.FilterCriterion;
using Lavie.Infrastructure;
using Lavie.Infrastructure.FormsAuthentication;
using Lavie.Infrastructure.FormsAuthentication.Extensions;
using Lavie.Models;
using Lavie.Modules.Admin.ActionFilters.Action;
using Lavie.Modules.Admin.Controllers;
using Lavie.Modules.Admin.Extensions;
using Lavie.Modules.Admin.ModelBinders;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Modules.Admin.Routing;
using Lavie.Modules.Admin.Services;
using Lavie.Routing;
using R = Lavie.Modules.Admin.Repositories;

namespace Lavie.Modules.Admin
{
    public class AdminModule : Module, IModuleMetaData, IAuthenticationModule
    {
        private readonly Site _site;

        public AdminModule()
        {
            this._site = DependencyInjector.GetService<Site>();
        }

        #region IModule Members

        public override string ModuleName
        {
            get { return "Admin"; }
        }

        public override void Initialize()
        {
            DependencyInjector
                .RegisterType<IFormsAuthentication, FormsAuthenticationWrapper>();

            DependencyInjector
                .RegisterType<IAdminUserService, AdminUserService>();

            DependencyInjector
                // 系统公告，用于系统后台的通知公告
                .RegisterType<IBulletinService, BulletinService>()
                // 用户信息、用户组、用户角色、权限信息的处理逻辑
                .RegisterType<IUserService, UserService>()
                .RegisterType<IGroupService, GroupService>()
                .RegisterType<IRoleService, RoleService>()
                .RegisterType<IPermissionService, PermissionService>()
                .RegisterType<INotificationService, NotificationService>();

            DependencyInjector
                .RegisterType<R.IBulletinRepository, R.BulletinRepository>()
                .RegisterType<R.IUserRepository, R.UserRepository>()
                .RegisterType<R.IGroupRepository, R.GroupRepository>()
                .RegisterType<R.IRoleRepository, R.RoleRepository>()
                .RegisterType<R.IPermissionRepository, R.PermissionRepository>()
                .RegisterType<R.INotificationRepository, R.NotificationRepository>();

            DependencyInjector
                .RegisterType<ISiteService, SiteService>()
                .RegisterType<R.ISiteRepository, R.SiteRepository>();
            var site = DependencyInjector.GetService<SiteService>().GetItem();
            DependencyInjector
                .RegisterInstance<Site>(site);

        }

        public override void RegisterRoutes()
        {
            string[] controllerNamespaces = new string[] { "Lavie.Modules.Admin.Controllers" };

            #region Api

            MapRoute("Admin.Api"
                , "Api/Admin/{action}"
                , new { controller = "ApiAdmin", action = "Index" }
                , null
                , controllerNamespaces
            );

            #endregion

            #region View

            MapRoute("Admin.ViewCore"
                , "Admin/ViewCore"
                , new { controller = "ViewAdmin", action = "ViewCore" }
                , null
                , controllerNamespaces
            ); // 核心模块通用
            MapRoute("Admin.View"
                , "Admin/View"
                , new { controller = "ViewAdmin", action = "View" }
                , null
                , controllerNamespaces
            ); // 外部模块通用

            MapRoute("Admin.ViewCore.Action"
                , "{action}"
                , new { controller = "ViewAdmin", action = "Index" }
                , null
                , controllerNamespaces
            ); // Login, Index

            #endregion

        }

        public override void RegisterModelBinders(ModelBinderDictionary modelBinders)
        {
            modelBinders[typeof(UserStatus)] = new UserStatusModelBinder();
            //modelBinders[typeof(UserInput)] = new UserInputModelBinder();
        }

        public override void RegisterFilters(FilterRegistryFilterProvider filterRegistry, GlobalFilterCollection globalFilters)
        {
            #region Filter注册示例

            /*

            //AdminController的SignIn Action
            ControllerActionFilterCriteria criteria1 = new ControllerActionFilterCriteria();
            criteria1.AddMethod<AdminController>(s => s.SignIn());
            filterRegistry.Add(new[] { criteria1 }, new Filter(new TimerActionFilter(), FilterScope.First, Int32.MinValue));

            //整个AdminController
            ControllerTypeFilterCriteria criteria2 = new ControllerTypeFilterCriteria();
            criteria2.AddType(typeof(AdminController));
            //criteria2.AddType<AdminController>();
            filterRegistry.Add(new[] { criteria2 }, new Filter(new TimerActionFilter(), FilterScope.First, Int32.MinValue));

            //RouteName为Admin.SignIn的Route
            RouteFilterCriteria criteria3 = new RouteFilterCriteria(routeTable);
            criteria3.AddRoute("Admin.SignIn");
            filterRegistry.Add(new[] { criteria3 }, new Filter(new TimerActionFilter(), FilterScope.First, Int32.MinValue));

            //RouteValue中包含为testKey，且值为testValue
            RouteValueFilterCriteria criteria4 = new RouteValueFilterCriteria("testKey", "testValue");
            filterRegistry.Add(new[] { criteria4 }, new Filter(new TimerActionFilter(), FilterScope.First, Int32.MinValue));
            
            //DataToken中包含为area，且值为admin
            DataTokenFilterCriteria criteria5 = new DataTokenFilterCriteria("area", "admin");
            filterRegistry.Add(new[] { criteria5 }, new Filter(new TimerActionFilter(), FilterScope.First, Int32.MinValue));
            
            
            //15大组合权限判断
            //Controller -> AuthorizationFilter
            //Controller -> ApiAuthorizationFilter
            //Controller -> RouteAuthorizationFilter
            //Action -> AuthorizationFilter
            //Action -> ApiAuthorizationFilter
            //Action -> RouteAuthorizationFilter
            //Route -> AuthorizationFilter
            //Route -> ApiAuthorizationFilter
            //Route -> RouteAuthorizationFilter
            //RouteValue -> AuthorizationFilter
            //RouteValue -> ApiAuthorizationFilter
            //RouteValue -> RouteAuthorizationFilter
            //DataToken -> AuthorizationFilter
            //DataToken -> ApiAuthorizationFilter
            //DataToken -> RouteAuthorizationFilter
            
            */
            #endregion

            #region 页面执行计时器

            // TimerActionFilter 页面执行时间,为了使计时更准确，使其尽量排在其他Filter之前
            //var timerCriteria = new ControllerActionFilterCriteria();
            //timerCriteria.AddMethod<SystemController>(s => s.Modules());
            //timerCriteria.AddMethod<SystemController>(s => s.SiteConfig());
            //timerCriteria.AddMethod<SystemController>(s => s.ServerInfo());
            //filterRegistry.Add(new[] { timerCriteria }, new Filter(new TimerActionFilter(), FilterScope.First, Int32.MinValue));

            #endregion

            #region LavieViewModel信息注入

            // SiteActionFilter 站点基本信息
            filterRegistry.Add(typeof(SiteActionFilter));
            // UserActionFilter 加入至 LavieViewModule
            filterRegistry.Add(typeof(UserActionFilter));
            // AuthenticationModule 加入至 LavieViewModule
            filterRegistry.Add(typeof(AuthenticationModuleActionFilter));
            // ReturnURL 加入至 LavieViewModule
            filterRegistry.Add(typeof(ReturnURLActionFilter));

            #endregion

            #region CSRF AntiForgeryToken

            /*
            var antiForgeryTokenCriteria = new ControllerActionFilterCriteria();
            antiForgeryTokenCriteria.AddMethod<SystemController>(s => s.SiteConfig(null));
            antiForgeryTokenCriteria.AddMethod<SystemController>(s => s.Bulletin(null));
            antiForgeryTokenCriteria.AddMethod<MembershipController>(s => s.UserAddSave(null, null));
            antiForgeryTokenCriteria.AddMethod<MembershipController>(s => s.UserEditSave(null, null));
            // TODO: 如有必要，还可以添加用户组保存、角色保存、权限保存等的验证
            filterRegistry.Add(new[] { antiForgeryTokenCriteria }, new ValidateAntiForgeryTokenAttribute());
            */

            #endregion

            #region Api 跨域

            var allowCrossSiteJsonCriteria = new ControllerTypeFilterCriteria();
            allowCrossSiteJsonCriteria.AddType<ApiAdminController>();
            filterRegistry.Add(new[] { allowCrossSiteJsonCriteria }, new AllowCrossSiteJsonAttribute());

            #endregion

            #region Api 权限

            // View
            var apiViewCriteria = new ControllerActionFilterCriteria();
            apiViewCriteria.AddMethod<ViewAdminController>(s => s.Index());
            apiViewCriteria.AddMethod<ViewAdminController>(s => s.ViewCore(null));
            apiViewCriteria.AddMethod<ViewAdminController>(s => s.View(null));
            filterRegistry.Add(new[] { apiViewCriteria }, new AuthorizationFilter(DependencyInjector, user => user.IsAuthenticated));

            // Index
            var apiIndexCriteria = new ControllerActionFilterCriteria();
            apiIndexCriteria.AddMethod<ApiAdminController>(s => s.GetProfile());
            apiIndexCriteria.AddMethod<ApiAdminController>(s => s.GetMenus());
            filterRegistry.Add(new[] { apiIndexCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.IsAuthenticated));

            // 系统管理
            var apiGetServerInfoCriteria = new ControllerActionFilterCriteria();
            apiGetServerInfoCriteria.AddMethod<ApiAdminController>(s => s.GetServerInfo());
            filterRegistry.Add(new[] { apiGetServerInfoCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.HasPermission("服务器信息")));

            var apiEditSiteConfitCriteria = new ControllerActionFilterCriteria();
            apiEditSiteConfitCriteria.AddMethod<ApiAdminController>(s => s.GetSiteConfig());
            apiEditSiteConfitCriteria.AddMethod<ApiAdminController>(s => s.EditSiteConfig(null));
            filterRegistry.Add(new[] { apiEditSiteConfitCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.HasPermission("系统配置")));

            var apiBulletinCriteria = new ControllerActionFilterCriteria();
            apiBulletinCriteria.AddMethod<ApiAdminController>(s => s.GetBulletin());
            apiBulletinCriteria.AddMethod<ApiAdminController>(s => s.EditBulletin(null));
            filterRegistry.Add(new[] { apiBulletinCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.HasPermission("系统公告")));

            var apiNotificationCriteria = new ControllerActionFilterCriteria();
            apiNotificationCriteria.AddMethod<ApiAdminController>(s => s.GetNotificationsForManager(null, null));
            apiNotificationCriteria.AddMethod<ApiAdminController>(s => s.AddNotification(null));
            apiNotificationCriteria.AddMethod<ApiAdminController>(s => s.EditNotification(null));
            apiNotificationCriteria.AddMethod<ApiAdminController>(s => s.RemoveNotification(null));
            filterRegistry.Add(new[] { apiNotificationCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.HasPermission("通知管理")));

            var apiNotificationUserCriteria = new ControllerActionFilterCriteria();
            apiNotificationUserCriteria.AddMethod<ApiAdminController>(s => s.GetNotifications(null, null));
            apiNotificationUserCriteria.AddMethod<ApiAdminController>(s => s.ReadNotifications(null));
            apiNotificationUserCriteria.AddMethod<ApiAdminController>(s => s.DeleteNotifications(null));
            filterRegistry.Add(new[] { apiNotificationUserCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.IsAuthenticated));

            var apiModuleCriteria = new ControllerActionFilterCriteria();
            apiModuleCriteria.AddMethod<ApiAdminController>(s => s.GetModuleConfig());
            filterRegistry.Add(new[] { apiModuleCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.HasPermission("模块信息")));

            var apiExtractModulePermissionsCriteria = new ControllerActionFilterCriteria();
            apiExtractModulePermissionsCriteria.AddMethod<ApiAdminController>(s => s.ExtractModulePermissions());
            filterRegistry.Add(new[] { apiExtractModulePermissionsCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.HasPermission("提取权限")));

            var apiClearModulePermissionsCriteria = new ControllerActionFilterCriteria();
            apiClearModulePermissionsCriteria.AddMethod<ApiAdminController>(s => s.ClearModulePermissions());
            filterRegistry.Add(new[] { apiClearModulePermissionsCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.HasPermission("清理权限")));

            // 用户
            var apiUserCriteria = new ControllerActionFilterCriteria();
            apiUserCriteria.AddMethod<ApiAdminController>(s => s.GetUsers(null, null));
            apiUserCriteria.AddMethod<ApiAdminController>(s => s.AddUser(null));
            apiUserCriteria.AddMethod<ApiAdminController>(s => s.EditUser(null));
            apiUserCriteria.AddMethod<ApiAdminController>(s => s.RemoveUser(null));
            filterRegistry.Add(new[] { apiUserCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.HasPermission("用户信息管理")));

            // 用户组
            var apiGroupCriteria = new ControllerActionFilterCriteria();
            apiGroupCriteria.AddMethod<ApiAdminController>(s => s.AddGroup(null));
            apiGroupCriteria.AddMethod<ApiAdminController>(s => s.EditGroup(null));
            apiGroupCriteria.AddMethod<ApiAdminController>(s => s.RemoveGroup(Guid.Empty));
            apiGroupCriteria.AddMethod<ApiAdminController>(s => s.MoveGroup(Guid.Empty, Guid.Empty, MovingLocation.Above, null));
            filterRegistry.Add(new[] { apiGroupCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.HasPermission("用户组管理")));

            // 角色
            var apiRoleCriteria = new ControllerActionFilterCriteria();
            apiRoleCriteria.AddMethod<ApiAdminController>(s => s.AddRole(null));
            apiRoleCriteria.AddMethod<ApiAdminController>(s => s.EditRole(null));
            apiRoleCriteria.AddMethod<ApiAdminController>(s => s.RemoveRole(Guid.Empty));
            apiRoleCriteria.AddMethod<ApiAdminController>(s => s.MoveRole(0, 0));
            filterRegistry.Add(new[] { apiRoleCriteria }, new ApiAuthorizationFilter(DependencyInjector, user => user.HasPermission("角色管理")));

            #endregion

        }

        #endregion

        #region ILavieModuleMetaData Members

        public List<ModuleMenu> GetModuleMenus()
        {
            // Type: 0 菜单项(不能包含Children) 1 子菜单(不能链接) 2 菜单组(不能链接)
            return new List<ModuleMenu>
            {
                /*
                new ModuleMenu
                {
                    Title = "管理首页",
                    Type = 1,
                    Children = new List<ModuleMenu> {
                        new ModuleMenu {
                            Title = "管理首页",
                            LinkRouteName = "Admin.ViewCore",
                            LinkRouteValues = new { Title = "管理首页", Name = "welcome" }, // 因为 Webpack 调试服务器区分大小写，所以统一使用小写
                            Validator = u => u.HasPermission("后台管理"),
                        },
                        new ModuleMenu {
                            Title = "修改基本资料",
                            LinkRouteName = "Admin.ChangeProfile",
                            Validator = u => u.HasPermission("后台修改资料"),
                        },
                        new ModuleMenu {
                            Title = "修改登录密码",
                            LinkRouteName = "Admin.ChangePassword",
                            Validator = u => u.HasPermission("后台修改密码"),
                        },
                        new ModuleMenu {
                            Title = "退出登录",
                            LinkRouteName = "Admin.Api",
                            LinkRouteValues = new { Action = "Logout" }
                        },
                    }
                },*/
                new ModuleMenu
                {
                    Title = "系统管理",
                    Type = ModuleMenuType.Sub,
                    Children = new List<ModuleMenu> {
                         new ModuleMenu{
                             Type = ModuleMenuType.Group,
                             Title ="系统管理",
                             Children = new List<ModuleMenu> {
                                new ModuleMenu{ Title="服务器信息", LinkRouteName = "Admin.ViewCore", LinkRouteValues = new { Title = "服务器信息", Name = "serverinfo" }, Validator = u => u.HasPermission("服务器信息")},
                                new ModuleMenu{ Title="系统配置", LinkRouteName = "Admin.ViewCore", LinkRouteValues = new { Title = "系统配置", Name = "siteconfig" }, Validator = u => u.HasPermission("系统配置")},
                                new ModuleMenu{ Title="系统公告", LinkRouteName = "Admin.ViewCore", LinkRouteValues = new { Title = "系统公告", Name = "bulletin" }, Validator = u => u.HasPermission("系统公告")},
                                new ModuleMenu{ Title="通知管理", LinkRouteName = "Admin.ViewCore", LinkRouteValues = new { Title = "通知管理", Name = "notificationmanage", Components = "ckfinder" }, Validator = u => u.HasPermission("通知管理")},
                                new ModuleMenu{ Title="重启系统", LinkRouteName = "Admin.Api", LinkRouteValues = new { Action = "Restart" }, Directly = true, Validator = u => u.HasPermission("重启系统")},
                             }
                         },
                         new ModuleMenu{
                             Type = ModuleMenuType.Group,
                             Title ="模块管理",
                             Children = new List<ModuleMenu> {
                                new ModuleMenu{ Title="模块信息", LinkRouteName = "Admin.ViewCore", LinkRouteValues = new { Title = "模块信息", Name = "module" }, Validator = u => u.HasPermission("模块信息")},
                                new ModuleMenu{ Title="权限列表", LinkRouteName = "Admin.ViewCore", LinkRouteValues = new { Title = "权限列表", Name = "modulepermissions" }, Validator = u => u.HasPermission("权限列表")},
                             }
                         },
                    },
                },
                new ModuleMenu
                {
                    Title = "用户管理",
                    Type = ModuleMenuType.Sub,
                    Children = new List<ModuleMenu> {
                         new ModuleMenu{ Title="用户信息列表", LinkRouteName = "Admin.ViewCore", LinkRouteValues = new { Title = "用户信息列表", Name = "user", Components = "ckfinder" }, Validator = u => u.HasPermission("用户信息管理")},
                         new ModuleMenu{ Title="用户组列表", LinkRouteName = "Admin.ViewCore", LinkRouteValues = new { Title = "用户组列表", Name = "group" }, Validator = u => u.HasPermission("用户组管理")},
                         new ModuleMenu{ Title="角色列表", LinkRouteName = "Admin.ViewCore", LinkRouteValues = new { Title = "角色列表", Name = "role" }, Validator = u => u.HasPermission("角色管理")},
                         new ModuleMenu{ Title="权限列表", LinkRouteName = "Admin.ViewCore", LinkRouteValues = new { Title = "权限列表", Name = "permission" }, Validator = u => u.HasPermission("权限管理")},
                    }
                },
            };
        }

        public List<PermissionInput> GetModulePermissions()
        {
            var permissions = new List<PermissionInput>()
            {
               new PermissionInput{ PermissionID = new Guid("{303EC418-A517-4220-9B08-206FFF81DE2A}"), Name="后台管理", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{2E89D5F5-B949-433b-A166-B1C6211A9302}"), ParentID = new Guid("{303EC418-A517-4220-9B08-206FFF81DE2A}"),Name="后台修改资料", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{B80B1C90-3F1E-4412-9836-46A86DCF9FC2}"), ParentID = new Guid("{303EC418-A517-4220-9B08-206FFF81DE2A}"),Name="后台修改密码", ModuleName= this.ModuleName}

              ,new PermissionInput{ PermissionID = new Guid("{10B03A60-6E59-4cc7-8AB5-2CEC1F0695AE}"), Name="系统管理", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{161A19BD-0173-47AD-96ED-7308AF0D856C}"), ParentID = new Guid("{10B03A60-6E59-4cc7-8AB5-2CEC1F0695AE}"),Name="服务器信息", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{2CC40EA3-E24D-4b1d-BD03-4FC5B4E30EE1}"), ParentID = new Guid("{10B03A60-6E59-4cc7-8AB5-2CEC1F0695AE}"),Name="系统信息", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{F65ACCB5-6BCF-41fa-8614-F3C7EBBFC338}"), ParentID = new Guid("{10B03A60-6E59-4cc7-8AB5-2CEC1F0695AE}"),Name="系统配置", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{23309B5D-5745-4153-8B8D-F00BBCE54EF5}"), ParentID = new Guid("{10B03A60-6E59-4cc7-8AB5-2CEC1F0695AE}"),Name="系统公告", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{486B3B3B-C020-4E96-B3A1-A746B9400692}"), ParentID = new Guid("{10B03A60-6E59-4cc7-8AB5-2CEC1F0695AE}"),Name="通知管理", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{957DFBCC-9139-4247-842C-F9A794BC4BC7}"), ParentID = new Guid("{10B03A60-6E59-4cc7-8AB5-2CEC1F0695AE}"),Name="重启系统", ModuleName= this.ModuleName}
             
              ,new PermissionInput{ PermissionID = new Guid("{39587146-885C-414c-98A7-9B157C83C374}"), Name="模块管理", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{7ACE2FCC-535D-40a6-BA2B-A2D368981ED7}"), ParentID = new Guid("{39587146-885C-414c-98A7-9B157C83C374}"),Name="模块信息", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{973F3E67-8918-49B3-AF79-9993778305C2}"), ParentID = new Guid("{39587146-885C-414c-98A7-9B157C83C374}"),Name="权限列表", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{DB49EA0F-B5AD-44c9-B41C-DB6540C52CC5}"), ParentID = new Guid("{39587146-885C-414c-98A7-9B157C83C374}"),Name="提取权限", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{9A1BAAFF-2246-4d1f-B3A2-52C47EF83DAF}"), ParentID = new Guid("{39587146-885C-414c-98A7-9B157C83C374}"),Name="清理权限", ModuleName= this.ModuleName}

              ,new PermissionInput{ PermissionID = new Guid("{418D9725-3C83-4119-A76C-221E2371944C}"), Name="用户管理", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{C834D9A6-AF92-4c4a-AB01-2277DB8A47A4}"), ParentID = new Guid("{418D9725-3C83-4119-A76C-221E2371944C}"),Name="用户信息管理", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{A627F9C0-41F5-43e0-9D2F-6C5D1988CDC8}"), ParentID = new Guid("{418D9725-3C83-4119-A76C-221E2371944C}"),Name="用户组管理", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{67A9B69F-A513-4c20-928E-532FB5EC4B42}"), ParentID = new Guid("{418D9725-3C83-4119-A76C-221E2371944C}"),Name="角色管理", ModuleName= this.ModuleName}
              ,new PermissionInput{ PermissionID = new Guid("{B16814AA-064D-42f5-B4B7-0E92A925C91F}"), ParentID = new Guid("{418D9725-3C83-4119-A76C-221E2371944C}"),Name="权限管理", ModuleName= this.ModuleName}

            };

            return permissions;
        }

        #endregion

        #region ILavieAuthenticationModule Members

        //public async Task<IUser> GetUser(RequestContext context)

        public IUser GetUser(RequestContext context)
        {
            IUser anonymous = new UserAnonymous();

            // 如果没通过验证
            if (!context.HttpContext.User.Identity.IsAuthenticated) return anonymous;

            // 获取用户ID
            int userID;
            if (!Int32.TryParse(context.HttpContext.User.Identity.Name, out userID)) return anonymous;

            // 通过用户ID获取用户信息
            var userService = DependencyInjector.GetService<IUserService>();
            if (userService == null) return anonymous;
            UserInfo userInfo = userService.GetItemByUserID(userID, UserStatus.Normal);
            if (userInfo == null) return anonymous;

            // 获取当前用户的标识
            var identity = context.HttpContext.User.Identity as FormsIdentity;
            if (identity == null) return anonymous;

            // 获取用户票据中的信息中，ModuleName的值
            var userData = identity.Ticket.GetStructuredUserData();
            if (userData == null) return anonymous;

            // 如果用户票据所属的模块不是当前模块
            if (userData["ModuleName"] != this.ModuleName) return anonymous;

            return new User(userInfo, userData);

        }

        public string GetRegistrationUrl(RequestContext context)
        {
            return null;
        }

        public string GetLoginUrl(RequestContext context)
        {
            var urlHelper = new UrlHelper(context, DependencyInjector.GetService<RouteCollection>());

            return urlHelper.RouteUrl("Admin.ViewCore.Action", new { Action = "Login" }, context.HttpContext.Request.Url.Scheme);
        }

        public string GetLogoutUrl(RequestContext context)
        {
            var urlHelper = new UrlHelper(context, DependencyInjector.GetService<RouteCollection>());

            return urlHelper.RouteUrl("Admin.Api", new { Action = "Logout" }, context.HttpContext.Request.Url.Scheme);
        }

        #endregion

    }
}
