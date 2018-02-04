using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Lavie.ModelValidation.Attributes;
using Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Models.InputModels
{
    public class GroupInput
    {
        [DisplayName("用户组ID")]
        public Guid? GroupID { get; set; }

        [DisplayName("所属用户组")]
        public Guid? ParentID { get; set; }

        [Required(ErrorMessage = "用户组名称不能为空")]
        [StringLength(50, ErrorMessage = "用户组名称请保持在50个字符以内")]
        //[SlugWithChinese(ErrorMessage = "用户组名称只能包含中文、字母、数字、_和-")]
        [DisplayName("用户组名称")]
        public string Name { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsIncludeUser { get; set; }
        public Guid[] RoleIDs { get; set; }
        public Guid[] LimitRoleIDs { get; set; }
        public Guid[] PermissionIDs { get; set; }

        public static GroupInput FromGroup(Group group)
        {
            if (group == null) return null;
            var groupT = group;
            return new GroupInput
            {
                ParentID = group.ParentID,
                Name = group.Name,
                IsIncludeUser = group.IsIncludeUser,
                IsDisabled = group.IsDisabled,
                RoleIDs = groupT.Roles.Select(m=>m.RoleID).ToArray(),
                LimitRoleIDs = groupT.LimitRoles.Select(m => m.RoleID).ToArray(),
                PermissionIDs = groupT.Permissions.Select(m => m.PermissionID).ToArray(),

            };
        }
        public Group ToGroup()
        {
            return new Group
            {
                ParentID = this.ParentID??Guid.Empty,
                Name = this.Name,
                IsIncludeUser = this.IsIncludeUser,
                IsDisabled = this.IsDisabled,
                Roles = from r in this.RoleIDs select new RoleBase{
                    RoleID = r                     
                },
                LimitRoles = from r in this.LimitRoleIDs
                        select new RoleBase
                        {
                            RoleID = r
                        },
                Permissions = from p in this.PermissionIDs select new PermissionBase
                {
                    PermissionID = p
                }
            };
        }
    }
}
