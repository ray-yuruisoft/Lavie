using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Lavie.Extensions;
using System.Linq;

namespace Lavie.Modules.Admin.Models
{
    public class UserInfoWarpper
    {
        [JsonProperty(PropertyName = "userID")]
        public int UserID { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "headURL")]
        public string HeadURL { get; set; }
        [JsonProperty(PropertyName = "logoURL")]
        public string LogoURL { get; set; }
    }

    public class Profile : UserInfoWarpper
    {
        [JsonProperty(PropertyName = "groups")]
        public IEnumerable<GroupInfo> Groups { get; set; }
        [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "RoleID", "00000000-0000-0000-0000-000000000000")]
        [JsonProperty(PropertyName = "role")]
        public RoleInfo Role { get; set; }
    }

    public class UserInfoProfile
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string LogoURL { get; set; }
        public string RealName { get; set; }
        public bool RealNameIsValid { get; set; }
        public string Email { get; set; }
        public bool EmailIsValid { get; set; }
        public string Mobile { get; set; }
        public bool MobileIsValid { get; set; }
        public string HeadURL { get; set; }
        public string Token { get; set; }
        public bool IsBindedToWX { get; set; }
        public bool IsBindedToWXA { get; set; }
        // public IEnumerable<string> Permissions { get; set; }
        // TODO: Group Role, etc.

        public UserInfoProfile()
        {
            //Permissions = new List<string>();
        }
        public static UserInfoProfile FromUserInfoBase(UserInfoBase userInfo)
        {
            return new UserInfoProfile
            {
                UserID = userInfo.UserID,
                Username = userInfo.Username,
                DisplayName = userInfo.DisplayName ?? String.Empty,
                LogoURL = userInfo.LogoURL ?? String.Empty,
                RealName = userInfo.RealName ?? String.Empty,
                RealNameIsValid = userInfo.RealNameIsValid,
                Email = userInfo.Email ?? String.Empty,
                EmailIsValid = userInfo.EmailIsValid,
                Mobile = userInfo.Mobile ?? String.Empty,
                MobileIsValid = userInfo.MobileIsValid,
                HeadURL = userInfo.HeadURL ?? String.Empty,
                Token = userInfo.Token ?? String.Empty,
                IsBindedToWX = !userInfo.WXOpenID.IsNullOrWhiteSpace(),
                IsBindedToWXA = !userInfo.WXAOpenID.IsNullOrWhiteSpace(),
            };
        }
        public static UserInfoProfile FromUserInfo(UserInfo userInfo)
        {
            return new UserInfoProfile
            {
                UserID = userInfo.UserID,
                Username = userInfo.Username,
                DisplayName = userInfo.DisplayName ?? String.Empty,
                LogoURL = userInfo.LogoURL ?? String.Empty,
                RealName = userInfo.RealName ?? String.Empty,
                RealNameIsValid = userInfo.RealNameIsValid,
                Email = userInfo.Email ?? String.Empty,
                EmailIsValid = userInfo.EmailIsValid,
                Mobile = userInfo.Mobile ?? String.Empty,
                MobileIsValid = userInfo.MobileIsValid,
                HeadURL = userInfo.HeadURL ?? String.Empty,
                Token = userInfo.Token ?? String.Empty,
                IsBindedToWX = !userInfo.WXOpenID.IsNullOrWhiteSpace(),
                IsBindedToWXA = !userInfo.WXAOpenID.IsNullOrWhiteSpace(),
                /*
                Permissions = userInfo.Permissions.Select(m=>m.Name)
                .Union(userInfo.RolePermissions.Select(m => m.Name))
                .Union(userInfo.RolesPermissions.Select(m=>m.Name))
                .Union(userInfo.GroupPermissions.Select(m => m.Name))
                .Union(userInfo.GroupRolesPermissions.Select(m => m.Name))*/
            };
        }
    }

    public class UserInfoBase
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string LogoURL { get; set; }
        public string RealName { get; set; }
        public bool RealNameIsValid { get; set; }
        public string Email { get; set; }
        public bool EmailIsValid { get; set; }
        public string Mobile { get; set; }
        public bool MobileIsValid { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public UserStatus Status { get; set; }
        public string StatusText { get; set; }
        public DateTime CreationDate { get; set; }
        public string HeadURL { get; set; }
        public string Token { get; set; }
        public string WXOpenID { get; set; }
        public string WXAOpenID { get; set; }

        public static UserInfoBase FromUserInfo(UserInfo userInfo)
        {
            return new UserInfoBase
            {
                UserID = userInfo.UserID,
                Username = userInfo.Username,
                DisplayName = userInfo.DisplayName,
                LogoURL = userInfo.LogoURL,
                RealName = userInfo.RealName,
                RealNameIsValid = userInfo.RealNameIsValid,
                Email = userInfo.Email,
                EmailIsValid = userInfo.EmailIsValid,
                Mobile = userInfo.Mobile,
                MobileIsValid = userInfo.MobileIsValid,
                Status = userInfo.Status,
                CreationDate = userInfo.CreationDate,
                HeadURL = userInfo.HeadURL,
                Token = userInfo.Token,
            };
        }
    }

    public class UserInfo : UserInfoBase
    {
        public virtual GroupInfo Group { get; set; }
        [JsonConverter(typeof(Lavie.ActionResults.DependencyJsonConverterGuid), "RoleID", "00000000-0000-0000-0000-000000000000")]
        public RoleInfo Role { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 用户拥有的特定角色
        /// </summary>
        public virtual IEnumerable<RoleBase> Roles { get; set; }
        /// <summary>
        /// 用户拥有的特定权限
        /// </summary>
        public virtual IEnumerable<PermissionBase> Permissions { get; set; }
        /// <summary>
        /// 用户所属用户组所拥有的角色
        /// </summary>
        public virtual IEnumerable<RoleBase> GroupRoles { get; set; }
        /// <summary>
        /// 用户所属用户组所拥有的角色所拥有的权限
        /// </summary>
        public virtual IEnumerable<PermissionBase> GroupRolesPermissions { get; set; }
        /// <summary>
        /// 用户所属用户组所拥有的权限
        /// </summary>
        public virtual IEnumerable<PermissionBase> GroupPermissions { get; set; }
        /// <summary>
        /// 用户的直接角色所拥有的权限
        /// </summary>
        public virtual IEnumerable<PermissionBase> RolePermissions { get; set; }
        /// <summary>
        /// 用户拥有的角色所拥有的权限
        /// </summary>
        public virtual IEnumerable<PermissionBase> RolesPermissions { get; set; }

        public string FullDisplayName
        {
            get
            {
                if (!DisplayName.IsNullOrWhiteSpace() && !RealName.IsNullOrWhiteSpace())
                {
                    return "{0}({1})".FormatWith(DisplayName, RealName);
                }
                else if (!DisplayName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else if (!RealName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string DisplayNameRealName
        {
            get
            {
                if (!DisplayName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else if (!RealName.IsNullOrWhiteSpace())
                {
                    return RealName;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string RealNameDisplayNme
        {
            get
            {
                if (!RealName.IsNullOrWhiteSpace())
                {
                    return RealName;
                }
                else if (!DisplayName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }

    public class Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}