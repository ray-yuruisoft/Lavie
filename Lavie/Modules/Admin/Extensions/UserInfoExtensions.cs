using System;
using System.Linq;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;

namespace Lavie.Modules.Admin.Extensions
{
    public static class UserInfoExtensions
    {
        public static UserInputEdit ToUserInputEdit(this UserInfo userInfo)
        {
            return new UserInputEdit
            {
                UserID = userInfo.UserID,
                GroupID = userInfo.Group.GroupID,
                RoleID = userInfo.Role != null && userInfo.Role.RoleID != Guid.Empty ? userInfo.Role.RoleID : (Guid?)null,
                Username = userInfo.Username,
                DisplayName = userInfo.DisplayName,
                RealName = userInfo.RealName,
                RealNameIsValid = userInfo.RealNameIsValid,
                Email = userInfo.Email,
                EmailIsValid = userInfo.EmailIsValid,
                Mobile = userInfo.Mobile,
                MobileIsValid = userInfo.MobileIsValid,
                Description = userInfo.Description,
                Status = userInfo.Status,
                RoleIDs = userInfo.Roles != null ? from r in userInfo.Roles select r.RoleID : Enumerable.Empty<Guid>(),
                PermissionIDs = userInfo.Permissions != null ? from p in userInfo.Permissions select p.PermissionID : Enumerable.Empty<Guid>(),
            };
        }

    }
}
