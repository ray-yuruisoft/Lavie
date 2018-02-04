using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.ActionFilters.Action;
using Lavie.ActionResults;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Models;
using Lavie.Modules.Admin.Models.Api;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Modules.Admin.Services;
using Lavie.Modules.Project.Models;
using Lavie.Modules.Project.Repositories;
using XM = Lavie.Modules.Admin.Models;
using XMP = Lavie.Modules.Project.Models;

namespace Lavie.Modules.Project.Controllers
{
    [AllowCrossSiteJson]
    public class GroupController : Controller
    {

        private readonly LavieContext _context;
        private readonly IGroupService _groupService;
        private readonly IRoleService _roleService;
        private readonly XM.User _currentUser;
        private readonly IFlowRepository _flowRepository;
        private readonly Guid zbrsxzb = new Guid("BF872ED0-EE64-484F-9518-7F4ECF9926C4");//总部人事行政部
        private readonly Guid Hr = new Guid("6672379B-B66A-46F5-AC1C-590047968E09");//人事主管
        private readonly Guid HrManager = new Guid("6672379B-B66A-46F5-AC1C-590047968E09");//人事行政经理

        public GroupController(LavieContext context
            , IFlowRepository flowRepository
            , IGroupService groupService
            , IRoleService roleService)
        {
            _flowRepository = flowRepository;
            _roleService = roleService;
            _groupService = groupService;
            _context = context;
            _currentUser = _context.User.As<XM.User>();
        }



        public async Task<object> GetCitys()
        {
            var groups = await _groupService.GetListAsync();
            var citys = groups.Where(c => c.Level == 3 && c.IsIncludeUser == false).Select(c => new
            {
                GroupID = c.GroupID,
                Name = c.Name
            });
            var result = new ApiListResult
            {
                Code = 200,
                Message = "获取城市列表成功",
                List = citys,
            };
            return this.DateTimeJson(result);
        }

        public async Task<object> GetRoles()
        {

            var rolesBase = await _roleService.GetListAsync();
            var roles = rolesBase.Where(c => c.RoleID != new Guid("10c0b1fd-f284-4a7d-bbe0-38a671e2bd34")).Select(c => new
            {
                RoleID = c.RoleID,
                Name = c.Name
            });

            var result = new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = roles,
            };
            return this.DateTimeJson(result);

        }

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
        //获取用户组树
        public async Task<object> GetGroupTree()
        {

            //if (_currentUser.UserInfo.Group.GroupID != zbrsxzb)
            //{
            //    return ErrorReturn("该用户不具备此操作权限");
            //}
            //if (!_currentUser.UserInfo.Roles.Any(c => c.RoleID == Hr) && !_currentUser.UserInfo.Roles.Any(c => c.RoleID == HrManager))
            //{
            //    return ErrorReturn("该用户不具备此操作权限");
            //}
            var staffLeaveAuditFlows = await _flowRepository.GetStaffLeaveAuditFlowList();
            var groups = await _groupService.GetListAsync();
            var roles = await _roleService.GetBaseListAsync();
            groups = groups.Where(c => c.Level > 1).ToList();
            var tree = new List<XMP.GroupTreeNode>();
            for (var i = 0; i < groups.Count; i++)
            {
                var item = groups[i];
                if (item.Level == 2)
                {
                    var node = GroupTreeNodeFromGroup(item);
                    node.ParentIDPath = null;
                    var temp = staffLeaveAuditFlows.FirstOrDefault(c => c.StaffLeaveAuditFlowID == node.ID);
                    if (temp != null)
                    {
                        node.StaffLeaveAuditFlow = GetStaffLeaveAuditFlowInfo(roles, groups, temp);
                    }

                    tree.Add(node);
                    GroupTreeAddChildren(staffLeaveAuditFlows, roles, groups, node, i);
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

        public async Task<object> ConfigStaffLeaveAuditFlow(StaffLeaveAuditFlowInput input)
        {

            if (!ModelState.IsValid) return ErrorReturn(ModelState.FirstErrorMessage());
            if (await _flowRepository.ConfigStaffLeaveAuditFlow(input, ModelState))
                return SuccessReturn();
            return ErrorReturn(ModelState.FirstErrorMessage());

        }

        #region Private Methods

        private StaffLeaveAuditFlowInfo GetStaffLeaveAuditFlowInfo(List<XM.RoleBase> roles, List<XM.Group> groups, StaffLeaveAuditFlowBase temp)
        {
            var a = new GroupBaseInfo()
            {
                GroupID = temp.StaffLeaveAuditFlowID,
                Name = groups.FirstOrDefault(m => m.GroupID == temp.StaffLeaveAuditFlowID)?.Name
            };
            var b = new GroupBaseInfo()
            {
                GroupID = temp.AuditGroupID1 ?? Guid.Empty,
                Name = groups.FirstOrDefault(m => m.GroupID == temp.AuditGroupID1)?.Name
            };
            var c = new GroupBaseInfo()
            {
                GroupID = temp.AuditGroupID2 ?? Guid.Empty,
                Name = groups.FirstOrDefault(m => m.GroupID == temp.AuditGroupID2)?.Name
            };
            var d = new GroupBaseInfo()
            {
                GroupID = temp.AuditGroupID3 ?? Guid.Empty,
                Name = groups.FirstOrDefault(m => m.GroupID == temp.AuditGroupID3)?.Name
            };
            var e = new GroupBaseInfo()
            {
                GroupID = temp.AuditGroupID4 ?? Guid.Empty,
                Name = groups.FirstOrDefault(m => m.GroupID == temp.AuditGroupID4)?.Name
            };
            var f = new RoleBaseInfo()
            {
                RoleID = temp.AuditRoleID1 ?? Guid.Empty,
                Name = roles.FirstOrDefault(m => m.RoleID == temp.AuditRoleID1)?.Name
            };
            var g = new RoleBaseInfo()
            {
                RoleID = temp.AuditRoleID2 ?? Guid.Empty,
                Name = roles.FirstOrDefault(m => m.RoleID == temp.AuditRoleID2)?.Name
            };
            var h = new RoleBaseInfo()
            {
                RoleID = temp.AuditRoleID3 ?? Guid.Empty,
                Name = roles.FirstOrDefault(m => m.RoleID == temp.AuditRoleID3)?.Name
            };
            var i = new RoleBaseInfo()
            {
                RoleID = temp.AuditRoleID4 ?? Guid.Empty,
                Name = roles.FirstOrDefault(m => m.RoleID == temp.AuditRoleID4)?.Name
            };
            var j = new RoleBaseInfo()
            {
                RoleID = temp.RequestRoleID,
                Name = roles.FirstOrDefault(m => m.RoleID == temp.RequestRoleID)?.Name
            };
            return new StaffLeaveAuditFlowInfo()
            {
                StaffLeaveAuditFlowID = a,
                AuditGroupID1 = b,
                AuditGroupID2 = c,
                AuditGroupID3 = d,
                AuditGroupID4 = e,
                AuditRoleID1 = f,
                AuditRoleID2 = g,
                AuditRoleID3 = h,
                AuditRoleID4 = i,
                RequestRoleID = j,
                AuditStaffLevelMaxDays1 = temp.AuditStaffLevelMaxDays1,
                AuditStaffLevelMaxDays2 = temp.AuditStaffLevelMaxDays2,
                AuditStaffLevelMaxDays3 = temp.AuditStaffLevelMaxDays3,
                AuditStaffLevelMaxDays4 = temp.AuditStaffLevelMaxDays4,
            };

        }
        private void GroupTreeAddChildren(List<StaffLeaveAuditFlowBase> staffLeaveAuditFlows, List<XM.RoleBase> roles, List<XM.Group> groups, XMP.GroupTreeNode node, int index)
        {
            for (var i = index + 1; i < groups.Count; i++)
            {
                var item = groups[i];
                if (item.ParentID == node.ID)
                {
                    if (node.Children == null)
                    {
                        node.Children = new List<XMP.GroupTreeNode>();
                    };
                    var child = GroupTreeNodeFromGroup(item);
                    // 在父节点的 ParentIDPath 基础上增加 ParentID
                    child.ParentIDPath = node.ParentIDPath != null ? new List<Guid>(node.ParentIDPath) : new List<Guid>(1);
                    child.ParentIDPath.Add(node.ID);

                    var temp = staffLeaveAuditFlows.FirstOrDefault(c => c.StaffLeaveAuditFlowID == child.ID);
                    if (temp != null)
                    {
                        child.StaffLeaveAuditFlow = GetStaffLeaveAuditFlowInfo(roles, groups, temp);
                    }

                    node.Children.Add(child);
                    GroupTreeAddChildren(staffLeaveAuditFlows, roles, groups, child, i);
                }
            }
        }
        private XMP.GroupTreeNode GroupTreeNodeFromGroup(XM.Group group)
        {
            return new XMP.GroupTreeNode
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
        private DateTimeJsonResult ErrorReturn(string message)
        {
            return this.DateTimeJson(new
            {
                code = 400,
                message = message
            });
        }
        private DateTimeJsonResult SuccessReturn()
        {
            return this.DateTimeJson(new
            {
                code = 200,
                message = "成功"
            });
        }
        private void ProjectGroups(List<XM.Group> groups)
        {
            if (groups == null)
            {
                groups = new List<XM.Group>();
                return;
            }
            string s = "　";

            foreach (var p in groups)
            {
                if (p.Level > 1)
                    p.Name = s.Repeat(p.Level - 1) + "┗ " + p.Name;
            }
        }

        #endregion

        private class GroupTreeResult : ApiResult
        {
            [JsonProperty(PropertyName = "tree")]
            public List<XMP.GroupTreeNode> Tree { get; set; }
        }

    }
}
