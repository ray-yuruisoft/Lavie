using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Lavie.ModelValidation.Attributes;

namespace Lavie.Modules.Admin.Models.InputModels
{
    public class RoleInput
    {
        public Guid? RoleID { get; set; }

        [Required(ErrorMessage = "角色名称不能为空")]
        //[SlugWithChinese(ErrorMessage = "角色名称只能包含中文、字母、数字、_和-，并且以中文或字母开头")]
        [StringLength(50, ErrorMessage = "角色名称请保持在50个字符以内")]
        [DisplayName("角色名称")]
        public string Name { get;set; }
        public Guid[] PermissionIDs { get; set; }

        public static RoleInput FromRole(Role role)
        {
            if (role == null) return null;

            return new RoleInput
            {
                Name = role.Name,
                PermissionIDs = role.Permissions.Select(m => m.PermissionID).ToArray()
            };
        }

    }

    public class SaveRoleNameInput
    {
        public Guid RoleID { get; set; }

        [Required(ErrorMessage = "角色名称不能为空")]
        [SlugWithChinese(ErrorMessage = "角色名称只能包含中文、字母、数字、_和-，并且以中文或字母开头")]
        [StringLength(50, ErrorMessage = "角色名称请保持在50个字符以内")]
        [DisplayName("角色名称")]
        public string Name { get; set; }
    }
}
