using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Admin.Models
{
    public class User : IUser, IPrincipal
    {
        public User(UserInfo userInfo, NameValueCollection authenticationValues)
        {
            Guard.ArgumentNotNull(userInfo, "userInfo");

            UserInfo = userInfo;
            AuthenticationValues = authenticationValues??new NameValueCollection();
            Identity = new UserIdentity(null, true, UserInfo.UserID.ToString());
        }
        public UserInfo UserInfo { get; set; }

        #region IUser Members

        public bool IsAuthenticated { get { return Identity.IsAuthenticated; } }
        public string Name { get { return Identity.Name; } }
        public NameValueCollection AuthenticationValues { get; private set; }

        public T As<T>() where T : class, IUser
        {
            return this as T;
        }

        public bool IsInGroup(string name)
        {
            if (String.IsNullOrEmpty(name)) return true;
            return String.Compare(UserInfo.Group.Name, name, true/*ignoreCase*/) == 0;
        }

        public bool HasPermission(string name)
        {
            return hasPermission(name, UserInfo.Permissions)                    // 直接权限
                || hasPermission(name, UserInfo.GroupPermissions)               // 用户组 - 权限
                || hasPermission(name, UserInfo.GroupRolesPermissions)          // 用户组 - 角色 - 权限
                || hasPermission(name, UserInfo.RolePermissions)                // 直接角色 - 权限
                || hasPermission(name, UserInfo.RolesPermissions);              // 角色 - 权限
        }
        public bool HasPermission(Guid permissionID)
        {
            return hasPermission(permissionID, UserInfo.Permissions)            // 直接权限
                || hasPermission(permissionID, UserInfo.GroupPermissions)       // 用户组 - 权限
                || hasPermission(permissionID, UserInfo.GroupRolesPermissions)  // 用户组 - 角色 - 权限
                || hasPermission(permissionID, UserInfo.RolePermissions)        // 直接角色 - 权限
                || hasPermission(permissionID, UserInfo.RolesPermissions);      // 角色 - 权限
        }

        #endregion

        #region IPrincipal Members

        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            return (UserInfo.Role != null && string.Compare(UserInfo.Role.Name, role, true) == 0)   // 直接角色
                || isInRole(role, UserInfo.Roles)                                                   // 附加角色
                || isInRole(role, UserInfo.GroupRoles);                                             // 用户组 - 角色
        }

        #endregion

        #region Private Static Methods

        private static bool hasPermission(string name, IEnumerable<PermissionBase> permissions)
        {
            if (String.IsNullOrEmpty(name)) return true;
            if (permissions == null || permissions.Count() == 0) return false;

            foreach (PermissionBase permission in permissions)
                if (string.Compare(permission.Name, name, true) == 0)
                    return true;

            return false;
        }
        private static bool hasPermission(Guid permissionID, IEnumerable<PermissionBase> permissions)
        {
            if (permissionID==Guid.Empty) return true;
            if (permissions == null || permissions.Count() == 0) return false;

            foreach (PermissionBase permission in permissions)
                if (permission.PermissionID == permissionID)
                    return true;

            return false;
        }
        private static bool isInRole(string name, IEnumerable<RoleBase> roles)
        {
            if (name.IsNullOrEmpty()) return true;
            if (roles.IsNullOrEmpty()) return false;

            foreach (RoleBase role in roles)
                if (string.Compare(role.Name, name, true) == 0)
                    return true;

            return false;
        }

        #endregion
        
    }
}