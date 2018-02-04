using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Infrastructure;
using Lavie.Infrastructure.FormsAuthentication;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Modules.Admin.Models.Api;
using Lavie.Modules.Admin.Services;
using Lavie.Utilities.Security;
using Lavie.Extensions;
using System.Collections.Specialized;
using System.Globalization;
using Lavie.Modules.Admin.Extensions;
using Lavie.ActionResults;
using Lavie.ActionFilters.Action;
using Lavie.Modules.Admin.Models;
using System.Diagnostics;
using Lavie.Models;
using Lavie.Extensions.Object;
using Lavie.Configuration;
using System.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Lavie.Modules.Admin.Controllers
{
    public class ApiAdminController : Controller
    {
        private readonly IFormsAuthentication _formsAuthentication;
        private readonly IUserService _userService;
        private readonly IAdminUserService _adminUserService;
        private readonly ISiteService _siteService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IGroupService _groupService;
        private readonly IRoleService _roleService;
        private readonly IModuleRegistry _moduleRegistry;
        private readonly IBulletinService _bulletinService;
        private readonly IDependencyResolver _dependencyResolver;
        private readonly User _user;
        private readonly LavieContext _context;

        // /Api/Admin/{action}
        public ApiAdminController(IFormsAuthentication formsAuthentication
            , IUserService userService
            , IAdminUserService adminUserService
            , ISiteService siteService
            , INotificationService notificationService
            , IPermissionService permissionService
            , IGroupService groupService
            , IRoleService roleService
            , IModuleRegistry moduleRegistry
            , IBulletinService bulletinService
            , IDependencyResolver dependencyResolver
            , LavieContext context)
        {
            this._formsAuthentication = formsAuthentication;
            this._userService = userService;
            this._adminUserService = adminUserService;
            this._notificationService = notificationService;
            this._permissionService = permissionService;
            this._groupService = groupService;
            this._siteService = siteService;
            this._roleService = roleService;
            this._moduleRegistry = moduleRegistry;
            this._bulletinService = bulletinService;
            this._context = context;
            this._user = _context.User.As<User>();
            this._dependencyResolver = dependencyResolver;
        }

        #region  Login

        public ActionResult GetValidateCode()
        {
            string code;
            var vCode = new ValidationCodeCreater(5, out code);
            Session["ValidateCode"] = code;
            byte[] bytes = vCode.CreateValidationCodeGraphic();
            return File(bytes, @"image/jpeg");
        }

        public async Task<object> Login(UserLoginInput userLoginInput)
        {
            var result = new ApiResult();

            if (!ModelState.IsValid)
            {
                result.Code = 400;
                result.Message = ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            if (Session["ValidateCode"] == null)
            {
                result.Code = 401;
                result.Message = "验证码已到期，请重新输入";
                return this.DateTimeJson(result);
            }

            if (String.Compare(Session["ValidateCode"].ToString(), userLoginInput.ValidateCode, StringComparison.OrdinalIgnoreCase) != 0)
            {
                result.Code = 402;
                result.Message = "请输入正确的验证码";
                return this.DateTimeJson(result);
            }

            //登陆成功与否都清理验证码，以防止暴力测试密码
            Session.Remove("ValidateCode");

            if (await _userService.SignInAsync(async () => await _userService.GetNormalUser(userLoginInput.Account, userLoginInput.Password)
                , (u) =>
                {
                    var userData = new NameValueCollection();
                    userData.Add("ModuleName", "Admin");
                    _formsAuthentication.SetAuthCookie(u.UserID.ToString(CultureInfo.InvariantCulture), false, userData);
                }))
            {
                result.Code = 200;
                result.Message = "登录成功";
                result.URL = Url.RouteUrl("Admin.ViewCore.Action", new { Action = "Index"/* 必须填写 */ }, Request.Url.Scheme);
                return this.DateTimeJson(result);
            }
            else
            {
                result.Code = 404;
                result.Message = "账号或密码错误，或用户状态不允许登录";
                return this.DateTimeJson(result);
            }
        }

        #endregion

        #region  Logout

        public ActionResult Logout()
        {
            _userService.SignOutAsync();
            _formsAuthentication.SignOut();
            var result = new ApiResult
            {
                Code = 200,
                Message = "注销成功",
                URL = Url.RouteUrl("Admin.ViewCore.Action", new { Action = "Login" })
            };

            return this.DateTimeJson(result);
        }

        #endregion

        #region Index

        public async Task<object> GetProfile()
        {
            var user = _context.User.As<User>();
            var profile = new Profile
            {
                UserID = user.UserInfo.UserID,
                Username = user.UserInfo.Username,
                DisplayName = user.UserInfo.DisplayName,
                HeadURL = user.UserInfo.HeadURL,
                LogoURL = user.UserInfo.LogoURL,
                Groups = await _groupService.GetInfoPathAsync(user.UserInfo.Group.GroupID),
                Role = user.UserInfo.Role,
            };

            var result = new ProfileResult
            {
                Code = 200,
                Message = "获取用户信息成功",
                Profile = profile
            };
            return this.DateTimeJson(result);

        }

        public async Task<object> ChangeProfile(UserChangeProfileInput input)
        {
            var result = new ApiResult();
            User user = _context.User.As<User>();
            if (user == null) return null; // 配置不当导致的异常情况

            if (!ModelState.IsValid || !await _adminUserService.ChangeProfile(user, input, ModelState))
            {
                result.Code = 400;
                result.Message = "修改资料失败" + ModelState.FirstErrorMessage();
            }
            else
            {
                result.Code = 200;
                result.Message = "修改资料成功";
            }

            return this.DateTimeJson(result);

        }

        public async Task<object> ChangePassword(UserChangePasswordInput input)
        {
            var result = new ApiResult();

            User user = _context.User.As<User>();
            if (user == null) return null; // 配置不当导致的异常情况

            if (!ModelState.IsValid || !await _adminUserService.ChangePassword(user, input, ModelState))
            {
                result.Code = 400;
                result.Message = "修改密码失败" + ModelState.FirstErrorMessage();
            }
            else
            {
                result.Code = 200;
                result.Message = "修改密码成功";
            }

            return this.DateTimeJson(result);
        }

        public object GetMenus()
        {
            var list = new List<ModuleMenu>();
            var modules = _moduleRegistry.GetModules<IModuleMetaData>();
            foreach (IModuleMetaData module in modules)
            {
                var items = module.GetModuleMenus();
                if (items.IsNullOrEmpty()) continue;

                foreach (var item in items)
                {
                    AddMenuToList(list, item);
                }
            }

            var result = new ApiListResult
            {
                Code = 200,
                Message = "获取菜单成功",
                List = list
            };
            return this.DateTimeJson(result);
        }

        #endregion

        #region 系统管理

        public async Task<object> GetServerInfo()
        {
            var content = System.Web.Helpers.ServerInfo.GetHtml().ToString();
            var roles = await _roleService.GetListAsync();
            var result = new ApiHTMLResult
            {
                Code = 200,
                Message = "获取服务器信息成功",
                HTML = content,
            };

            return this.DateTimeJson(result);
        }

        public async Task<object> GetSiteConfig()
        {
            var site = await _siteService.GetItemAsync();
            var siteInput = site.ToModel<SiteInput>();
            var result = new ApiItemResult
            {
                Code = 200,
                Message = "获取系统配置成功",
                Item = siteInput,
            };

            return this.DateTimeJson(result);
        }

        public async Task<object> EditSiteConfig(SiteInput siteInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid)
            {
                result.Code = 400;
                result.Message = "编辑系统配置失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }
            if (!await _siteService.SaveAsync(siteInput, ModelState))
            {
                result.Code = 400;
                result.Message = "编辑系统配置失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "编辑系统配置成功";
            return this.DateTimeJson(result);
        }

        public async Task<object> GetBulletin()
        {
            var bulletin = await _bulletinService.GetItemAsync();
            var bulletinInput = bulletin.ToModel<BulletinInput>();
            var result = new ApiItemResult
            {
                Code = 200,
                Message = "获取系统公告成功",
                Item = bulletinInput,
            };

            return this.DateTimeJson(result);
        }

        public async Task<object> EditBulletin(BulletinInput bulletinInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid)
            {
                result.Code = 400;
                result.Message = "编辑系统公告失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }
            if (bulletinInput.IsShow && (bulletinInput.Title.IsNullOrWhiteSpace() || bulletinInput.Content.IsNullOrWhiteSpace()))
            {
                result.Code = 400;
                result.Message = "编辑系统公告失败：显示公告需要输入标题和内容";
                return this.DateTimeJson(result);
            }
            if (!await _bulletinService.SaveAsync(bulletinInput, ModelState))
            {
                result.Code = 400;
                result.Message = "编辑系统公告失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "编辑系统公告成功";
            return this.DateTimeJson(result);
        }

        public object Restart()
        {
            var result = new ApiResult();
            if (_siteService.Restart())
            {
                result.Code = 200;
                result.Message = "重启系统成功";
            }
            else
            {
                result.Code = 200;
                result.Message = "重启系统失败，请确认对系统根目录下的Web.Config有写入的操作权限。";
            }
            return this.DateTimeJson(result);
        }

        #endregion

        #region 模块管理

        public object GetModuleConfig()
        {
            var result = new ApiItemResult();

            var moduleConfig = _dependencyResolver.GetService<LavieConfigurationSection>();

            var modules = moduleConfig.Modules.Cast<LavieModuleConfigurationElement>();
            var backgroundServices = moduleConfig.BackgroundServices.Cast<LavieBackgroundServiceConfigurationElement>();
            var dataProviders = moduleConfig.Providers.Cast<LavieDataProviderConfigurationElement>();

            var webConfigConnectionStrings = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>();
            var lavieConfigConnectionStrigns = moduleConfig.ConnectionStrings.Cast<ConnectionStringSettings>();
            var connectionStrings = (from s in webConfigConnectionStrings
                                     select s
                        )
                         .Union(
                         from s in lavieConfigConnectionStrigns
                         select s
                        );

            result.Item = new
            {
                modules,
                backgroundServices,
                dataProviders,
                connectionStrings,
            };

            result.Code = 200;
            result.Message = "获取模块信息成功";
            return this.DateTimeJson(result);
        }

        public async Task<object> GetPermissions()
        {
            var permissions = await _permissionService.GetListAsync();
            ProjectPermissions(permissions);
            var result = new ApiListResult
            {
                Code = 200,
                Message = "获取权限列表成功",
                List = permissions,
            };

            return this.DateTimeJson(result);
        }

        public async Task<object> ExtractModulePermissions()
        {
            var result = new ApiResult();

            IEnumerable<IModuleMetaData> modules = _dependencyResolver.GetService<IModuleRegistry>().GetModules<IModuleMetaData>();
            foreach (var module in modules)
            {
                //ModelResult modelResult = perService.Save(module.GetModulePermissions().OrderBy(p => p.Level));
                //ModelResult modelResult = perService.Save(module.GetModulePermissions().OrderBy(p => p.DisplayOrder));

                var permissions = module.GetModulePermissions();
                if (permissions == null) continue;

                if (!await _permissionService.SaveAsync(permissions, ModelState))
                {
                    result.Code = 400;
                    result.Message = "提取模块权限失败：" + ModelState.FirstErrorMessage();
                    return this.DateTimeJson(result);
                }
            }
            result.Code = 200;
            result.Message = "提取模块权限成功";
            return this.DateTimeJson(result);
        }

        public async Task<object> ClearModulePermissions()
        {
            //如下实现方式是先从模块中获取全部权限信息，然后从数据库中获取全部权限信息
            //求其差集再删除
            //最佳实现方案当然是将ID传到数据库
            //以delete * from Permission Where PermissionID not in(...)的方式或者 or 的方式

            var result = new ApiResult();

            var modules = _dependencyResolver.GetService<IModuleRegistry>().GetModules<IModuleMetaData>();
            var modulePermissionIDs = new List<Guid>();
            foreach (var module in modules)
            {
                IEnumerable<PermissionInput> modulePermissions = module.GetModulePermissions();
                if (modulePermissions != null)
                    modulePermissionIDs.AddRange(modulePermissions
                        .Where(m => m.PermissionID.HasValue)
                        .Select(m => m.PermissionID.Value));
            }

            var perToClear = (await _permissionService.GetListAsync())
                .OrderByDescending(m => m.Level)
                .Select(m => m.PermissionID)
                .Except(modulePermissionIDs);

            await _permissionService.RemoveAsync(perToClear);

            result.Code = 200;
            result.Message = "清理模块权限成功";
            return this.DateTimeJson(result);
        }

        #endregion

        #region 用户管理

        #region 用户

        public async Task<object> GetUsers(UserSearchCriteria criteria, PagingInfo pagingInfo)
        {
            if (!ModelState.IsValid)
            {
                var errorResult = new ApiResult();
                errorResult.Code = 400;
                errorResult.Message = "获取用户列表失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(errorResult);
            }
            var page = await _userService.GetPageAsync(criteria, pagingInfo);
            var result = new ApiPageResult
            {
                Code = 200,
                Message = "获取用户列表成功",
                Page = page,
            };
            return this.DateTimeJson(result);
        }

        public async Task<object> AddUser(UserInputAdd userInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid)
            {
                result.Code = 400;
                result.Message = "添加用户失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            if (userInput.UserID.HasValue)
            {
                // Guid.Empty 也不允许
                result.Code = 400;
                result.Message = "添加用户失败：无需提供参数 UserID";
                return this.DateTimeJson(result);
            }

            if (!await _userService.SaveAsync(userInput, ModelState))
            {
                result.Code = 400;
                result.Message = "添加用户失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "添加用户成功";
            return this.DateTimeJson(result);
        }

        public async Task<object> EditUser(UserInputEdit userInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid)
            {
                result.Code = 400;
                result.Message = "编辑用户失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            if (!userInput.UserID.HasValue)
            {
                result.Code = 400;
                result.Message = "编辑用户失败：必须提供参数 GroupID";
                return this.DateTimeJson(result);
            }

            if (!await _userService.SaveAsync(userInput, ModelState))
            {
                result.Code = 400;
                result.Message = "编辑用户失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "编辑用户成功";
            return this.DateTimeJson(result);
        }

        public async Task<object> RemoveUser(UserIDInput userIDInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid || !await _userService.RemoveAsync(userIDInput.UserID, ModelState))
            {
                result.Code = 400;
                result.Message = "编辑用户失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "删除成功";
            return this.DateTimeJson(result);

        }

        #endregion

        #region 用户组

        public async Task<object> GetGroups()
        {
            var groups = await _groupService.GetListAsync();
            ProjectGroups(groups);
            var result = new ApiListResult
            {
                Code = 200,
                Message = "获取用户组列表成功",
                List = groups,
            };

            return this.DateTimeJson(result);
        }

        public async Task<object> GetGroupTree()
        {
            var groups = await _groupService.GetListAsync();
            var tree = new List<GroupTreeNode>();
            for (var i = 0; i < groups.Count; i++)
            {
                var item = groups[i];
                if (item.Level == 1)
                {
                    var node = GroupTreeNodeFromGroup(item);
                    node.ParentIDPath = null;
                    tree.Add(node);
                    GroupTreeAddChildren(groups, node, i);
                }
            }
            var result = new GroupTreeResult
            {
                Code = 200,
                Message = "获取用户组树成功",
                Tree = tree,
            };

            return this.DateTimeJson(result);
        }

        public async Task<object> AddGroup(GroupInput groupInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid)
            {
                result.Code = 400;
                result.Message = "添加用户组失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            if (groupInput.GroupID.HasValue)
            {
                // Guid.Empty 也不允许
                result.Code = 400;
                result.Message = "添加用户组失败：无需提供参数 GroupID";
                return this.DateTimeJson(result);
            }
            if (!await _groupService.SaveAsync(groupInput, ModelState))
            {
                result.Code = 400;
                result.Message = "添加用户组失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "添加用户组成功";
            return this.DateTimeJson(result);
        }

        public async Task<object> EditGroup(GroupInput groupInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid)
            {
                result.Code = 400;
                result.Message = "编辑用户组失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            if (groupInput.GroupID.IsNullOrEmpty())
            {
                result.Code = 400;
                result.Message = "编辑用户组失败：必须提供参数 GroupID";
                return this.DateTimeJson(result);
            }

            if (!await _groupService.SaveAsync(groupInput, ModelState))
            {
                result.Code = 400;
                result.Message = "编辑用户组失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "编辑用户组成功";
            return this.DateTimeJson(result);
        }

        public async Task<object> RemoveGroup(Guid groupID)
        {
            var result = new ApiResult();

            if (await _groupService.RemoveAsync(groupID, ModelState))
            {
                result.Code = 200;
                result.Message = "删除成功";
            }
            else
            {
                result.Code = 400;
                result.Message = "删除失败：" + ModelState.FirstErrorMessage();
            }

            return this.DateTimeJson(result);

        }

        public async Task<object> MoveGroup(Guid sourceID, Guid targetID, MovingLocation movingLocation, bool? isChild)
        {
            var result = new ApiResult();

            if (await _groupService.MoveAsync(sourceID, targetID, movingLocation, isChild, ModelState))
            {
                result.Code = 200;
                result.Message = "移动成功";
            }
            else
            {
                result.Code = 400;
                result.Message = "移动失败：" + ModelState.FirstErrorMessage();
            }

            return this.DateTimeJson(result);

        }

        #endregion

        #region 角色管理

        public async Task<object> GetRoles()
        {
            var roles = await _roleService.GetListAsync();
            var result = new ApiListResult
            {
                Code = 200,
                Message = "获取角色列表成功",
                List = roles,
            };

            return this.DateTimeJson(result);
        }

        public async Task<object> GetRoleBases()
        {
            var roles = await _roleService.GetBaseListAsync();
            var result = new ApiListResult
            {
                Code = 200,
                Message = "获取角色列表成功",
                List = roles,
            };

            return this.DateTimeJson(result);
        }

        public async Task<object> SaveRoleName(SaveRoleNameInput saveRoleNameInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid || await _roleService.EditNameAsync(saveRoleNameInput, ModelState))
            {
                result.Code = 400;
                result.Message = "编辑名称失败：" + ModelState.FirstErrorMessage();
            }
            else
            {
                result.Code = 200;
                result.Message = "编辑名称成功";
            }

            return this.DateTimeJson(result);

        }

        public async Task<object> EditRole(RoleInput roleInput)
        {
            var result = new ApiItemResult();
            if (!ModelState.IsValid)
            {
                result.Code = 400;
                result.Message = "编辑角色失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            if (roleInput.RoleID.IsNullOrEmpty())
            {
                result.Code = 400;
                result.Message = "编辑角色失败：必须提供参数 RoleID";
                return this.DateTimeJson(result);
            }

            var role = await _roleService.SaveAsync(roleInput, ModelState);
            if (role == null)
            {
                result.Code = 400;
                result.Message = "编辑角色失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "编辑角色成功";
            result.Item = role;
            return this.DateTimeJson(result);
        }

        public async Task<object> AddRole(RoleInput roleInput)
        {
            var result = new ApiItemResult();
            if (!ModelState.IsValid)
            {
                result.Code = 400;
                result.Message = "添加角色失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            if (roleInput.RoleID.HasValue)
            {
                // Guid.Empty 也不允许
                result.Code = 400;
                result.Message = "添加角色失败：无需提供参数 RoleID";
                return this.DateTimeJson(result);
            }
            var role = await _roleService.SaveAsync(roleInput, ModelState);
            if (role == null)
            {
                result.Code = 400;
                result.Message = "添加角色失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "添加角色成功";
            result.Item = role;
            return this.DateTimeJson(result);
        }

        public async Task<object> RemoveRole(Guid roleID)
        {
            var result = new ApiResult();

            if (await _roleService.RemoveAsync(roleID, ModelState))
            {
                result.Code = 200;
                result.Message = "删除成功";
            }
            else
            {
                result.Code = 400;
                result.Message = "删除失败：" + ModelState.FirstErrorMessage();
            }

            return this.DateTimeJson(result);

        }

        public async Task<object> MoveRole(int sourceDisplayOrder, int targetDisplayOrder)
        {
            var result = new ApiResult();

            if (await _roleService.MoveAsync(sourceDisplayOrder, targetDisplayOrder, ModelState))
            {
                result.Code = 200;
                result.Message = "排序成功";
            }
            else
            {
                result.Code = 400;
                result.Message = "排序失败：" + ModelState.FirstErrorMessage();
            }

            return this.DateTimeJson(result);
        }

        #endregion

        #region 权限管理

        public async Task<object> GetPermissionTree()
        {
            var permissions = await _permissionService.GetListAsync();
            var tree = new List<TreeNode>();
            for (var i = 0; i < permissions.Count; i++)
            {
                var item = permissions[i];
                if (item.Level == 1)
                {
                    var node = new TreeNode
                    {
                        ID = item.PermissionID,
                        Name = item.Name
                    };
                    tree.Add(node);
                    PermissionTreeAddChildren(permissions, node, i);
                }
            }
            var result = new ApiTreeResult
            {
                Code = 200,
                Message = "获取权限树成功",
                Tree = tree,
            };

            return this.DateTimeJson(result);
        }

        #endregion

        #region 用户状态

        public object GetUserStatus()
        {
            var list = typeof(UserStatus).GetEnumDictionary<UserStatus>();
            var result = new ApiListResult
            {
                Code = 200,
                Message = "获取用户状态列表成功",
                List = list,
            };

            return this.DateTimeJson(result);
        }

        #endregion

        #endregion

        #region 通知

        public async Task<object> GetNotificationsForManager(NotificationSearchCriteria criteria, PagingInfo pagingInfo)
        {
            if (!ModelState.IsValid)
            {
                var errorResult = new ApiResult();
                errorResult.Code = 400;
                errorResult.Message = "获取通知列表失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(errorResult);
            }
            var page = await _notificationService.GetPageAsync(criteria, pagingInfo);
            var result = new ApiPageResult
            {
                Code = 200,
                Message = "获取通知列表成功",
                Page = page,
            };
            return this.DateTimeJson(result);
        }

        public async Task<object> AddNotification(NotificationInput notificationInput)
        {
            notificationInput.FromUserID = _user.UserInfo.UserID;
            var result = new ApiResult();
            if (notificationInput.NotificationID.HasValue)
            {
                result.Code = 400;
                result.Message = "编辑通知失败：无需通知ID";
                return this.DateTimeJson(result);
            }
            if (!ModelState.IsValid || !await _notificationService.SaveAsync(notificationInput, ModelState))
            {
                result.Code = 400;
                result.Message = "发布通知失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "发布通知成功";
            return this.DateTimeJson(result);
        }

        public async Task<object> EditNotification(NotificationInput notificationInput)
        {
            notificationInput.FromUserID = _user.UserInfo.UserID;

            var result = new ApiResult();
            if (!notificationInput.NotificationID.HasValue)
            {
                result.Code = 400;
                result.Message = "编辑通知失败：无通知ID";
                return this.DateTimeJson(result);
            }
            if (!ModelState.IsValid || !await _notificationService.SaveAsync(notificationInput, ModelState))
            {
                result.Code = 400;
                result.Message = "编辑通知失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }


            result.Code = 200;
            result.Message = "编辑通知成功";
            return this.DateTimeJson(result);
        }

        public async Task<object> RemoveNotification(NotificationIDInput notificationIDInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid || !await _notificationService.RemoveAsync(notificationIDInput.NotificationID, ModelState))
            {
                result.Code = 400;
                result.Message = "删除失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "删除成功";
            return this.DateTimeJson(result);

        }

        public async Task<object> GetNotifications(NotificationSearchCriteria criteria, PagingInfo pagingInfo)
        {
            criteria.ToUserID = _user.UserInfo.UserID;
            if (!ModelState.IsValid)
            {
                var errorResult = new ApiResult();
                errorResult.Code = 400;
                errorResult.Message = "获取通知列表失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(errorResult);
            }
            var page = await _notificationService.GetPageAsync(criteria, pagingInfo);
            var result = new ApiPageResult
            {
                Code = 200,
                Message = "获取通知列表成功",
                Page = page,
            };
            return this.DateTimeJson(result);
        }

        public async Task<object> ReadNotifications(NotificationIDListInput notificationIDListInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid || !await _notificationService.ReadAsync(_user.UserInfo.UserID, notificationIDListInput.NotificationIDs, ModelState))
            {
                result.Code = 400;
                result.Message = "设置已读失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "设置已读成功";
            return this.DateTimeJson(result);

        }

        public async Task<object> DeleteNotifications(NotificationIDListInput notificationIDListInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid || !await _notificationService.DeleteAsync(_user.UserInfo.UserID, notificationIDListInput.NotificationIDs, ModelState))
            {
                result.Code = 400;
                result.Message = "删除失败：" + ModelState.FirstErrorMessage();
                return this.DateTimeJson(result);
            }

            result.Code = 200;
            result.Message = "删除成功";
            return this.DateTimeJson(result);

        }

        public async Task<object> GetNewestNotification(int? currentNotificationID)
        {
            var result = new ApiItemResult
            {
                Code = 200,
                Message = "获取最新消息成功",
                Item = await _notificationService.GetNewestAsync(_user.UserInfo.UserID, currentNotificationID),
            };

            return this.DateTimeJson(result);
        }

        #endregion

        #region Test

        public async Task<object>  TestNotification(int userID, string title, string message, string url)
        {
            await Hubs.NotificationHub.SendMessageByUserID(userID, new Hubs.ApiResultNotification
            {
                Code = 201,
                Title = title,
                Message = message,
                URL = url
            });

            return "发送消息成功";
        }

        #endregion

        #region Private Methods

        private void AddMenuToList(List<ModuleMenu> list, ModuleMenu item)
        {
            if (!ValidateMenu(item)) return;

            if (item.Directly.HasValue && !item.Directly.Value)
            {
                // 避免无意义的序列化
                item.Directly = null;
            }

            if (item.Type == ModuleMenuType.Item && !item.Children.IsNullOrEmpty())
            {
                throw new Exception("菜单项【{0}】不能包含子项".FormatWith(item.Title));
            }
            if (item.Type == ModuleMenuType.Sub || item.Type == ModuleMenuType.Group)
            {
                if (!item.LinkRouteName.IsNullOrWhiteSpace())
                {
                    throw new Exception("{0}【{1}】不能设置路由".FormatWith(item.Type == ModuleMenuType.Sub ? "子菜单" : "菜单组", item.Title));
                }
                if (item.Directly.HasValue && item.Directly.Value)
                {
                    throw new Exception("{0}【{1}】不能设置为直接访问".FormatWith(item.Type == ModuleMenuType.Sub ? "子菜单" : "菜单组", item.Title));
                }

                if (item.Children.IsNullOrEmpty())
                {
                    // 如果类型是 1 或 2并且没有任何 Child，则本菜单也不用显示。
                    return;
                }

                var newChildren = new List<ModuleMenu>();
                item.Children.ForEach(m => AddMenuToList(newChildren, m));
                if (newChildren.IsNullOrEmpty())
                {
                    // 如果经过全选过滤，子菜单或菜单组已无子项，则本子菜单或菜单组也不用显示。
                    return;
                }
                item.Children = newChildren;
            }

            if (item.Type == ModuleMenuType.Item && ValidateMenu(item))
            {
                item.Link = Url.RouteUrl(item.LinkRouteName, item.LinkRouteValues, Request.Url.Scheme);
            }
            list.Add(item);

        }

        private bool ValidateMenu(ModuleMenu item)
        {
            if (item.Permission.IsNullOrWhiteSpace() && item.Role.IsNullOrWhiteSpace() && item.Group.IsNullOrWhiteSpace() && item.Validator == null)
            {
                return true;
            }

            if (item.Validator != null && item.Validator(_context.User))
            {
                return true;
            }

            if (item.Permission != null)
            {
                var perArray = item.Permission.Split('|');
                foreach (var it in perArray)
                {
                    if (_context.User.HasPermission(it))
                    {
                        return true;
                    }
                }
            }
            if (item.Role != null)
            {
                var rolArray = item.Role.Split('|');
                foreach (var it in rolArray)
                {
                    if (_context.User.IsInRole(it))
                    {
                        return true;
                    }
                }
            }
            if (item.Group != null)
            {
                var grpArray = item.Group.Split('|');
                foreach (var it in grpArray)
                {
                    if (_context.User.IsInGroup(it))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ProjectPermissions(List<Permission> permissions)
        {
            if (permissions == null)
            {
                permissions = new List<Permission>();
                return;
            }
            string s = "　";

            foreach (var p in permissions)
            {
                if (p.Level > 1)
                    p.Name = s.Repeat(p.Level - 1) + "┗ " + p.Name;
            }
        }

        private void PermissionTreeAddChildren(List<Permission> permissions, TreeNode node, int index)
        {
            for (var i = index + 1; i < permissions.Count; i++)
            {
                var item = permissions[i];
                if (item.ParentID == node.ID)
                {
                    if (node.Children == null)
                    {
                        node.Children = new List<TreeNode>();
                    };
                    var child = new TreeNode
                    {
                        ID = item.PermissionID,
                        Name = item.Name
                    };
                    node.Children.Add(child);
                    PermissionTreeAddChildren(permissions, child, i);
                }
            }
        }

        private void ProjectGroups(List<Group> groups)
        {
            if (groups == null)
            {
                groups = new List<Group>();
                return;
            }
            string s = "　";

            foreach (var p in groups)
            {
                if (p.Level > 1)
                    p.Name = s.Repeat(p.Level - 1) + "┗ " + p.Name;
            }
        }

        private void GroupTreeAddChildren(List<Group> groups, GroupTreeNode node, int index)
        {
            for (var i = index + 1; i < groups.Count; i++)
            {
                var item = groups[i];
                if (item.ParentID == node.ID)
                {
                    if (node.Children == null)
                    {
                        node.Children = new List<GroupTreeNode>();
                    };
                    var child = GroupTreeNodeFromGroup(item);
                    // 在父节点的 ParentIDPath 基础上增加 ParentID
                    child.ParentIDPath = node.ParentIDPath != null ? new List<Guid>(node.ParentIDPath) : new List<Guid>(1);
                    child.ParentIDPath.Add(node.ID);
                    node.Children.Add(child);
                    GroupTreeAddChildren(groups, child, i);
                }
            }
        }

        private GroupTreeNode GroupTreeNodeFromGroup(Group group)
        {
            return new GroupTreeNode
            {
                ID = group.GroupID,
                ParentID = group.ParentID,
                Name = group.Name,
                Level = group.Level,
                DisplayOrder = group.DisplayOrder,
                IsSystem = group.IsSystem,
                IsIncludeUser = group.IsIncludeUser,
                IsDisabled = group.IsDisabled,
                Roles = group.Roles,
                LimitRoles = group.LimitRoles,
                Permissions = group.Permissions,
            };
        }

        #endregion
    }
}
