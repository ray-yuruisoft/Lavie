using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Modules.Admin.Extensions;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Admin.Services
{
    public interface IAdminUserService
    {
        Task<bool> ChangePassword(User user, UserChangePasswordInput userInput, ModelStateDictionary modelState);
        Task<bool> ChangeProfile(User user, UserChangeProfileInput userInput, ModelStateDictionary modelState);
    }
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserService _userService;

        public AdminUserService(IUserService userService)
        {
            this._userService = userService;
        }

        #region IAdminUserService Members

        public async Task<bool> ChangePassword(User user, UserChangePasswordInput input, ModelStateDictionary modelState)
        {
            Guard.ArgumentNotNull(user, "user");
            Guard.ArgumentNotNull(input, "input");
            Guard.ArgumentNotNull(modelState, "modelState");

            //判断当前密码是否输入正确
            UserInfo chkUser = await _userService.GetNormalUser(user.UserInfo.Username, input.CurrentPassword);
            if (chkUser == null)
            {
                modelState.AddModelError("CurrentPassword", "当前密码不正确");
                return false;
            }

            return await _userService.ChangePasswordAsync(chkUser.UserID, input.NewPassword, modelState);

        }

        public async Task<bool> ChangeProfile(User user, UserChangeProfileInput input, ModelStateDictionary modelState)
        {
            Guard.ArgumentNotNull(user, "user");
            Guard.ArgumentNotNull(input, "input");
            Guard.ArgumentNotNull(modelState, "modelState");

            UserInfo userInfo = user.UserInfo;

            var userInput = new UserInputEdit
            {
                DisplayName = input.DisplayName,
                HeadURL = input.HeadURL,
                LogoURL = input.LogoURL,

                // 下列资料都不进行修改
                UserID = userInfo.UserID,
                Username = userInfo.Username,
                RealName = userInfo.RealName,
                RealNameIsValid = userInfo.RealNameIsValid,
                Email = userInfo.Email,
                EmailIsValid = userInfo.EmailIsValid,
                Mobile = userInfo.Mobile,
                MobileIsValid = userInfo.MobileIsValid,
                Description = userInfo.Description,
                Password = String.Empty,
                PasswordConfirm = String.Empty,
                PermissionIDs = userInfo.Permissions != null ? from p in userInfo.Permissions select p.PermissionID : Enumerable.Empty<Guid>(),
                Status = userInfo.Status,
                RoleIDs = userInfo.Roles != null ? from r in userInfo.Roles select r.RoleID : Enumerable.Empty<Guid>(),
                GroupID = userInfo.Group.GroupID,
                RoleID = userInfo.Role != null && userInfo.Role.RoleID != Guid.Empty ? userInfo.Role.RoleID : (Guid?)null,
            };

            return await _userService.SaveAsync(userInput, modelState);
        }

        #endregion

    }
}
