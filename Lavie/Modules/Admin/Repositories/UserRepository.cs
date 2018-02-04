using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Models;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Utilities.Cryptography;
using XM = Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Repositories
{
    public interface IUserRepository
    {
        Task<XM.UserInfo> GetItemByUserIDAsync(int userID, XM.UserStatus? status = null);
        XM.UserInfo GetItemByUserID(int userID, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByUsernameAsync(string username, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByMobileAsync(string mobile, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByEmailAsync(string email, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByTokenAsync(string token, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByWXOpenIDAsync(string wxOpenID);
        Task<XM.UserInfo> GetItemByWXAOpenIDAsync(string wxaOpenID);
        Task<string> GetHeadURLAsync(int userID);
        Task<List<XM.UserInfoWarpper>> GetUserInfoWarpperList(IEnumerable<int> userIDs);
        Task<bool> IsExistsUsernameAsync(string username);
        Task<bool> IsExistsMobileAsync(string mobile);
        Task<bool> IsExistsEmailAsync(string email);
        Task<bool> IsExistsMobilesAsync(IEnumerable<string> mobiles);
        Task<bool> IsExistsAsync(int userID, XM.UserStatus? status = null);
        bool IsExists(int userID, XM.UserStatus? status = null);
        Task<bool> VerifyExistsUsernameAsync(int userID, string username);
        Task<bool> VerifyExistsMobileAsync(int userID, string mobile);
        Task<bool> VerifyExistsEmailAsync(int userID, string email);
        Task<bool> VerifyExistsAsync(UserInput userInput, ModelStateDictionary modelState);
        Task<IPagedList<XM.UserInfo>> GetPagedListAsync(XM.UserSearchCriteria criteria, PagingInfo pagingInfo);
        Task<Page<XM.UserInfo>> GetPageAsync(XM.UserSearchCriteria criteria, PagingInfo pagingInfo);
        Task<XM.UserInfo> SaveAsync(UserInput userInput, ModelStateDictionary modelState);
        Task<bool> ChangeUsernameAsync(int userID, string newUsername, ModelStateDictionary modelState);
        Task<bool> ChangeMobileAsync(int userID, string newMobile, ModelStateDictionary modelState);
        Task<bool> ChangeDisplayNameAsync(int userID, string newDisplayName);
        Task<bool> ChangeLogoAsync(int userID, string logoURL);
        Task<bool> ChangePasswordAsync(int userID, string newPassword);
        Task<bool> ChangeHeadAsync(int userID, string newHeadURL);
        Task<int> ChangePasswordAsync(string username, string newPassword);
        Task<int> ResetPasswordByMobileAsync(string username, string newPassword, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(int userID, ModelStateDictionary modelState);
        Task<bool> ChangeStatusAsync(int userID, XM.UserStatus status);
        Task<bool> UpdateClientAgentAsync(int userID, String clientAgent, String ip);
        Task<bool> UpdateTokenAsync(int userID, String token);
        Task<bool> UpdateWXOpenIDAsync(int userID, String wxOpenID, ModelStateDictionary modelState);
        Task<bool> CleanWXOpenIDAsync(int userID);
        Task<bool> UpdateWXAOpenIDAsync(int userID, String wxaOpenID, ModelStateDictionary modelState);
        Task<bool> CleanWXAOpenIDAsync(int userID);
        Task<bool> ClearClientAgentAsync(int userID, String clientAgent);
        Task<string> GetMobileValidationCode(GetMobileValidationCodeInput getMobileValidationCodeInput, ModelStateDictionary modelState);
        Task<bool> VerifyMobileValidationCode(VerifyMobileValidationCodeInput verifyMobileValidationCodeInput, ModelStateDictionary modelState);
        Task<bool> FinishVerifyMobileValidationCode(string mobile, int typeID, ModelStateDictionary modelState);
    }

    public class UserRepository : RepositoryBase, IUserRepository
    {
        // TODO: 改为从配置文件读取
        private const int MobileValidationCodeLength = 6;
        private const int MobileValidationCodeRequestRateInterval = 1;
        public const int MobileValidationCodeExpirationInterval = 30;
        private const int MobileValidationCodeMaxVerifyTimes = 3;

        private readonly Expression<Func<User, XM.UserInfo>> _selector;

        public UserRepository()
        {
            _selector = u => new XM.UserInfo
            {
                UserID = u.UserID,
                Username = u.Username,
                DisplayName = u.DisplayName,
                LogoURL = u.LogoURL,
                RealName = u.RealName,
                RealNameIsValid = u.RealNameIsValid,
                Email = u.Email,
                EmailIsValid = u.EmailIsValid,
                Mobile = u.Mobile,
                MobileIsValid = u.MobileIsValid,
                Password = u.Password,
                Token = u.Token,
                WXAOpenID = u.WXAOpenID,
                CreationDate = u.CreationDate,
                Description = u.Description,
                Status = u.Status,
                HeadURL = u.HeadURL,
                Group = new XM.GroupInfo
                {
                    GroupID = u.Group.GroupID,
                    Name = u.Group.Name,
                },
                Role = new XM.RoleInfo
                {
                    RoleID = u.Role != null ? u.Role.RoleID : Guid.Empty,
                    Name = u.Role != null ? u.Role.Name : String.Empty,
                },
                Roles = from ur in u.Roles
                        select new XM.RoleBase
                        {
                            RoleID = ur.RoleID,
                            Name = ur.Name,
                            IsSystem = ur.IsSystem,
                            DisplayOrder = ur.DisplayOrder
                        },
                Permissions = from up in u.Permissions
                              select new XM.PermissionBase
                              {
                                  ModuleName = up.ModuleName,
                                  PermissionID = up.PermissionID,
                                  Name = up.Name
                              },
                GroupPermissions = from upp in u.Group.Permissions
                                   select new XM.PermissionBase
                                   {
                                       ModuleName = upp.ModuleName,
                                       PermissionID = upp.PermissionID,
                                       Name = upp.Name
                                   },
                RolePermissions = from ur in u.Role.Permissions
                                  where u.Role != null
                                  select new XM.PermissionBase
                                  {
                                      ModuleName = ur.ModuleName,
                                      PermissionID = ur.PermissionID,
                                      Name = ur.Name
                                  },
                RolesPermissions = from usr in u.Roles
                                  from urp in usr.Permissions
                                  select new XM.PermissionBase
                                  {
                                      ModuleName = urp.ModuleName,
                                      PermissionID = urp.PermissionID,
                                      Name = urp.Name
                                  },
                GroupRoles = from ugr in u.Group.Roles
                             select new XM.RoleBase
                             {
                                 RoleID = ugr.RoleID,
                                 Name = ugr.Name,
                                 IsSystem = ugr.IsSystem,
                                 DisplayOrder = ugr.DisplayOrder
                             },

            };
        }

        #region IUserRepository 成员

        public async Task<XM.UserInfo> GetItemByUserIDAsync(int userID, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await DbContext.Users.Select(_selector).FirstOrDefaultAsync(m => m.UserID == userID && m.Status == status.Value);
            }
            else
            {
                user = await DbContext.Users.Select(_selector).FirstOrDefaultAsync(m => m.UserID == userID);
            }
            await ProjectUserAsync(user);
            return user;
        }
        public XM.UserInfo GetItemByUserID(int userID, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = DbContext.Users.Select(_selector).FirstOrDefault(m => m.UserID == userID && m.Status == status.Value);
            }
            else
            {
                user = DbContext.Users.Select(_selector).FirstOrDefault(m => m.UserID == userID);
            }
            ProjectUser(user);
            return user;
        }
        public async Task<XM.UserInfo> GetItemByUsernameAsync(string username, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await DbContext.Users.Select(_selector).FirstOrDefaultAsync(m => m.Username == username && m.Status == status.Value);
            }
            else
            {
                user = await DbContext.Users.Select(_selector).FirstOrDefaultAsync(m => m.Username == username);
            }

            await ProjectUserAsync(user);
            return user;
        }
        public async Task<XM.UserInfo> GetItemByMobileAsync(string mobile, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await DbContext.Users.Select(_selector).FirstOrDefaultAsync(m => (m.MobileIsValid && m.Mobile == mobile) && m.Status == status.Value);
            }
            else
            {
                user = await DbContext.Users.Select(_selector).FirstOrDefaultAsync(m => (m.MobileIsValid && m.Mobile == mobile));
            }
            await ProjectUserAsync(user);
            return user;
        }
        public async Task<XM.UserInfo> GetItemByEmailAsync(string email, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await DbContext.Users.Select(_selector).FirstOrDefaultAsync(m => (m.EmailIsValid && m.Email == email) && m.Status == status.Value);
            }
            else
            {
                user = await DbContext.Users.Select(_selector).FirstOrDefaultAsync(m => (m.EmailIsValid && m.Email == email));
            }
            await ProjectUserAsync(user);
            return user;
        }
        public async Task<XM.UserInfo> GetItemByTokenAsync(string token, XM.UserStatus? status = null)
        {
            if (token.IsNullOrWhiteSpace()) return null;

            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await DbContext.Users.Select(_selector).FirstOrDefaultAsync(m => m.Token == token && m.Status == status.Value);
            }
            else
            {
                user = await DbContext.Users.Select(_selector).FirstOrDefaultAsync(m => m.Token == token);
            }

            await ProjectUserAsync(user);
            return user;
        }
        public async Task<XM.UserInfo> GetItemByWXOpenIDAsync(string wxOpenID)
        {
            if (wxOpenID.IsNullOrWhiteSpace()) return null;
            XM.UserInfo user = await DbContext.Users.Where(m => m.WXOpenID == wxOpenID).Select(_selector).FirstOrDefaultAsync();
            await ProjectUserAsync(user);
            return user;
        }
        public async Task<XM.UserInfo> GetItemByWXAOpenIDAsync(string wxaOpenID)
        {
            if (wxaOpenID.IsNullOrWhiteSpace()) return null;
            XM.UserInfo user = await DbContext.Users.Where(m => m.WXAOpenID == wxaOpenID).Select(_selector).FirstOrDefaultAsync();
            await ProjectUserAsync(user);
            return user;
        }
        public async Task<string> GetHeadURLAsync(int userID)
        {
            var head = await DbContext.Users.Where(m => m.UserID == userID).Select(m => m.HeadURL).FirstOrDefaultAsync();
            return head;
        }
        public async Task<List<XM.UserInfoWarpper>> GetUserInfoWarpperList(IEnumerable<int> userIDs)
        {
            if (userIDs == null) return new List<XM.UserInfoWarpper>(0);
            userIDs = userIDs.Distinct();
            var iDs = userIDs as int[] ?? userIDs.ToArray();
            var list = await DbContext.Users.Where(m => iDs.Contains(m.UserID)).Select(m => new XM.UserInfoWarpper
            {
                UserID = m.UserID,
                Username = m.Username,
                DisplayName = m.DisplayName,
                HeadURL = m.HeadURL,
            }).AsNoTracking().ToListAsync();

            return list;
        }
        public async Task<bool> IsExistsUsernameAsync(string username)
        {
            return await DbContext.Users.AnyAsync(m => m.Username == username);
        }
        public async Task<bool> IsExistsMobileAsync(string mobile)
        {
            if (mobile.IsNullOrWhiteSpace()) return false;
            return await DbContext.Users.AnyAsync(m => m.Mobile == mobile);
        }
        public async Task<bool> IsExistsEmailAsync(string email)
        {
            if (email.IsNullOrWhiteSpace()) return false;
            return await DbContext.Users.AnyAsync(m => m.Email == email);
        }
        public async Task<bool> IsExistsMobilesAsync(IEnumerable<string> mobiles)
        {
            var enumerable = mobiles as string[] ?? mobiles.ToArray();
            if (enumerable.Length == 0) return false;
            return await DbContext.Users.Where(m => mobiles.Contains(m.Mobile)).AnyAsync();
        }
        public async Task<bool> IsExistsAsync(int userID, XM.UserStatus? status = null)
        {
            if (status.HasValue)
            {
                return await DbContext.Users.AnyAsync(m => m.UserID == userID && m.Status == status);
            }
            else
            {
                return await DbContext.Users.AnyAsync(m => m.UserID == userID);
            }
        }
        public bool IsExists(int userID, XM.UserStatus? status = null)
        {
            if (status.HasValue)
            {
                return DbContext.Users.Any(m => m.UserID == userID && m.Status == status);
            }
            else
            {
                return DbContext.Users.Any(m => m.UserID == userID);
            }
        }
        public async Task<bool> VerifyExistsUsernameAsync(int userID, string username)
        {
            return await DbContext.Users.AnyAsync(m => m.UserID != userID && m.Username == username);
        }
        public async Task<bool> VerifyExistsMobileAsync(int userID, string mobile)
        {
            if (mobile.IsNullOrWhiteSpace()) return false;
            return await DbContext.Users.AnyAsync(m => m.UserID != userID && m.Mobile == mobile);
        }
        public async Task<bool> VerifyExistsEmailAsync(int userID, string email)
        {
            if (email.IsNullOrWhiteSpace()) return false;
            return await DbContext.Users.AnyAsync(m => m.UserID != userID && m.Email == email);
        }
        public async Task<bool> VerifyExistsAsync(UserInput userInput, ModelStateDictionary modelState)
        {
            bool isExistsUsername = false;
            bool isExistsMobile = false;
            bool isExistsEmail = false;
            if (userInput.UserID.HasValue) //根据用户ID编辑
            {
                var item = await DbContext.Users.Where(m => m.UserID != userInput.UserID.Value).Select(m => new
                {
                    Username = m.Username,
                    Email = m.Email,
                    Mobile = m.Mobile,
                }).FirstOrDefaultAsync();

                if (item != null)
                {
                    if (!item.Username.IsNullOrWhiteSpace() && item.Username.Equals(userInput.Username, StringComparison.InvariantCultureIgnoreCase))
                    {
                        isExistsUsername = true;
                    }
                    else if (!item.Mobile.IsNullOrWhiteSpace() && item.Mobile.Equals(userInput.Mobile, StringComparison.InvariantCultureIgnoreCase))
                    {
                        isExistsMobile = true;
                    }
                    else if (!item.Email.IsNullOrWhiteSpace() && item.Email.Equals(userInput.Email, StringComparison.InvariantCultureIgnoreCase))
                    {
                        isExistsEmail = true;
                    }
                }
            }
            else //添加
            {
                var item = await DbContext.Users.Select(m => new
                {
                    Username = m.Username,
                    Email = m.Email,
                    Mobile = m.Mobile,
                }).FirstOrDefaultAsync();

                if (!item.Username.IsNullOrWhiteSpace() && item.Username.Equals(userInput.Username, StringComparison.InvariantCultureIgnoreCase))
                {
                    isExistsUsername = true;
                }
                else if (!item.Mobile.IsNullOrWhiteSpace() && item.Mobile.Equals(userInput.Mobile, StringComparison.InvariantCultureIgnoreCase))
                {
                    isExistsMobile = true;
                }
                else if (!item.Email.IsNullOrWhiteSpace() && item.Email.Equals(userInput.Email, StringComparison.InvariantCultureIgnoreCase))
                {
                    isExistsEmail = true;
                }
            }

            if (isExistsUsername)
            {
                modelState.AddModelError("Username", "用户名[" + userInput.Username + "]已经被使用");
            }
            else if (isExistsMobile)
            {
                modelState.AddModelError("Mobile", "手机号[" + userInput.Mobile + "]已经被使用");

            }
            else if (isExistsEmail)
            {
                modelState.AddModelError("Mobile", "邮箱[" + userInput.Email + "]已经被使用");
            }
            return isExistsUsername || isExistsMobile || isExistsEmail;
        }
        public async Task<IPagedList<XM.UserInfo>> GetPagedListAsync(XM.UserSearchCriteria criteria, PagingInfo pagingInfo)
        {
            Expression<Func<User, XM.UserInfo>> selector = u => new XM.UserInfo
            {
                UserID = u.UserID,
                Username = u.Username,
                DisplayName = u.DisplayName,
                Email = u.Email,
                EmailIsValid = u.EmailIsValid,
                Mobile = u.Mobile,
                MobileIsValid = u.MobileIsValid,
                CreationDate = u.CreationDate,
                Description = u.Description,
                Status = u.Status,
                Group = new XM.GroupInfo
                {
                    GroupID = u.Group.GroupID,
                    Name = u.Group.Name,
                },
            };

            IQueryable<User> query = DbContext.Users;
            if (criteria != null)
            {
                if (!criteria.GroupIDs.IsNullOrEmpty())
                {
                    query = query.Where(m => criteria.GroupIDs.Contains(m.GroupID));
                }
                if (criteria.Status.HasValue)
                {
                    //int status = (int)criteria.Status.Value;
                    query = query.Where(m => m.Status == criteria.Status.Value);
                }
                if (!criteria.Keyword.IsNullOrWhiteSpace())
                    query = query.Where(m =>
                    m.Username.Contains(criteria.Keyword) ||
                    m.RealName.Contains(criteria.Keyword) ||
                    m.Mobile.Contains(criteria.Keyword) ||
                    m.DisplayName.Contains(criteria.Keyword));
                if (criteria.CreationDateBegin.HasValue)
                {
                    var begin = criteria.CreationDateBegin.Value.Date;
                    query = query.Where(m => m.CreationDate >= begin);
                }
                if (criteria.CreationDateEnd.HasValue)
                {
                    var end = criteria.CreationDateEnd.Value.Date.AddDays(1);
                    query = query.Where(m => m.CreationDate < end);
                }
            }

            return await query.OrderBy(m => m.UserID).Select(selector).GetPagedListAsync(pagingInfo);
        }
        public async Task<Page<XM.UserInfo>> GetPageAsync(XM.UserSearchCriteria criteria, PagingInfo pagingInfo)
        {
            Expression<Func<User, XM.UserInfo>> selector = u => new XM.UserInfo
            {
                UserID = u.UserID,
                Username = u.Username,
                DisplayName = u.DisplayName,
                RealName = u.RealName,
                RealNameIsValid = u.RealNameIsValid,
                Email = u.Email,
                EmailIsValid = u.EmailIsValid,
                Mobile = u.Mobile,
                MobileIsValid = u.MobileIsValid,
                CreationDate = u.CreationDate,
                Description = u.Description,
                Status = u.Status,
                Group = new XM.GroupInfo
                {
                    GroupID = u.Group.GroupID,
                    Name = u.Group.Name,
                },
                Role = new XM.RoleInfo
                {
                    RoleID = u.Role != null ? u.Role.RoleID : Guid.Empty,
                    Name = u.Role != null ? u.Role.Name : String.Empty,
                },
                Roles = from r in u.Roles
                        select new XM.RoleBase
                        {
                            RoleID = r.RoleID,
                            Name = r.Name,
                            IsSystem = r.IsSystem,
                            DisplayOrder = r.DisplayOrder
                        },
                Permissions = from p in u.Permissions
                              select new XM.PermissionBase
                              {
                                  PermissionID = p.PermissionID,
                                  Name = p.Name,
                                  ModuleName = p.ModuleName
                              }
            };

            IQueryable<User> query = DbContext.Users;
            if (criteria != null)
            {
                if (!criteria.GroupIDs.IsNullOrEmpty())
                {
                    query = query.Where(m => criteria.GroupIDs.Contains(m.GroupID));
                }
                if (criteria.Status.HasValue)
                {
                    //int status = (int)criteria.Status.Value;
                    query = query.Where(m => m.Status == criteria.Status.Value);
                }
                if (criteria.Keyword != null)
                {
                    var keyword = criteria.Keyword.Trim();
                    if (keyword.Length != 0)
                    {
                        query = query.Where(m =>
                        m.Username.Contains(keyword) ||
                        m.RealName.Contains(keyword) ||
                        m.Mobile.Contains(keyword) ||
                        m.DisplayName.Contains(keyword));
                    }
                }

                if (criteria.CreationDateBegin.HasValue)
                {
                    var begin = criteria.CreationDateBegin.Value.Date;
                    query = query.Where(m => m.CreationDate >= begin);
                }
                if (criteria.CreationDateEnd.HasValue)
                {
                    var end = criteria.CreationDateEnd.Value.Date.AddDays(1);
                    query = query.Where(m => m.CreationDate < end);
                }
            }

            IOrderedQueryable<User> orderedQuery;
            if (pagingInfo.SortInfo != null && !pagingInfo.SortInfo.Sort.IsNullOrWhiteSpace())
            {
                orderedQuery = query.Order(pagingInfo.SortInfo.Sort, pagingInfo.SortInfo.SortDir == SortDir.DESC);
            }
            else
            {
                // 默认排序
                orderedQuery = query.OrderBy(m => m.UserID);
            }

            var page = await orderedQuery.Select(selector).GetPageAsync(pagingInfo);
            page.List.ForEach(m => m.StatusText = m.Status.GetEnumDisplayName());
            return page;
        }
        public async Task<XM.UserInfo> SaveAsync(UserInput userInput, ModelStateDictionary modelState)
        {
            User userToSave;
            if (userInput.UserID.HasValue)
            {
                userToSave = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userInput.UserID.Value);
                if (userToSave == null)
                {
                    modelState.AddModelError("UserID", "尝试编辑不存在的记录");
                    return null;
                }
                if (!userInput.Password.IsNullOrWhiteSpace())
                    userToSave.Password = userInput.Password;

                userToSave.Status = userInput.Status;

            }
            else
            {
                userToSave = DbContext.Users.Create();
                DbContext.Users.Add(userToSave);
                userToSave.Status = XM.UserStatus.Normal; // Fix
                userToSave.Password = userInput.Password;
                userToSave.CreationDate = DateTime.Now;
            }

            var group = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == userInput.GroupID);
            if (group == null)
            {
                modelState.AddModelError("GroupID", "用户组不存在");
                return null;
            }
            if (userInput.RoleID.HasValue && !group.LimitRoles.Any(m => m.RoleID == userInput.RoleID.Value))
            {
                modelState.AddModelError("GroupID", "用户组【{0}】不允许使用该主要角色".FormatWith(group.Name));
                return null;
            }
            if (!group.IsIncludeUser)
            {
                modelState.AddModelError("GroupID", "用户组【{0}】不允许包含用户".FormatWith(group.Name));
                return null;
            }

            userToSave.GroupID = userInput.GroupID;
            userToSave.RoleID = userInput.RoleID;
            userToSave.Username = userInput.Username;
            userToSave.DisplayName = userInput.DisplayName;
            userToSave.HeadURL = userInput.HeadURL;
            userToSave.LogoURL = userInput.LogoURL;
            userToSave.RealName = userInput.RealName;
            userToSave.RealNameIsValid = userInput.RealNameIsValid;
            userToSave.Email = userInput.Email;
            userToSave.EmailIsValid = userInput.EmailIsValid;
            userToSave.Mobile = userInput.Mobile;
            userToSave.MobileIsValid = userInput.MobileIsValid;
            userToSave.Description = userInput.Description;

            #region 用户角色
            //移除项
            if (!userToSave.Roles.IsNullOrEmpty())
            {
                if (!userInput.RoleIDs.IsNullOrEmpty())
                {
                    List<Role> roleToRemove = (from p in userToSave.Roles
                                               where !userInput.RoleIDs.Contains(p.RoleID)
                                               select p).ToList();
                    for (int i = 0; i < roleToRemove.Count; i++)
                        userToSave.Roles.Remove(roleToRemove[i]);
                }
                else
                {
                    userToSave.Roles.Clear();
                }
            }
            //添加项
            if (!userInput.RoleIDs.IsNullOrEmpty())
            {
                //要添加的ID集
                List<Guid> roleIDToAdd = (from p in userInput.RoleIDs
                                          where userToSave.Roles.All(m => m.RoleID != p)
                                          select p).ToList();

                //要添加的项
                List<Role> roleToAdd = await (from p in DbContext.Roles
                                              where roleIDToAdd.Contains(p.RoleID)
                                              select p).ToListAsync();
                foreach (var item in roleToAdd)
                    userToSave.Roles.Add(item);

            }
            #endregion

            #region 用户权限
            //移除项
            if (!userToSave.Permissions.IsNullOrEmpty())
            {
                if (!userInput.PermissionIDs.IsNullOrEmpty())
                {
                    List<Permission> permissionToRemove = (from p in userToSave.Permissions
                                                           where !userInput.PermissionIDs.Contains(p.PermissionID)
                                                           select p).ToList();
                    for (int i = 0; i < permissionToRemove.Count; i++)
                        userToSave.Permissions.Remove(permissionToRemove[i]);
                }
                else
                {
                    userToSave.Permissions.Clear();
                }
            }
            //添加项
            if (!userInput.PermissionIDs.IsNullOrEmpty())
            {
                //要添加的ID集
                List<Guid> permissionIDToAdd = (from p in userInput.PermissionIDs
                                                where userToSave.Permissions.All(m => m.PermissionID != p)
                                                select p).ToList();

                //要添加的项
                List<Permission> permissionToAdd = await (from p in DbContext.Permissions
                                                          where permissionIDToAdd.Contains(p.PermissionID)
                                                          select p).ToListAsync();
                foreach (var item in permissionToAdd)
                    userToSave.Permissions.Add(item);

            }
            #endregion

            await DbContext.SaveChangesAsync();

            //return new[] { userToSave }.Select(_selector.Compile()).First();
            return await GetItemByUserIDAsync(userToSave.UserID);
        }
        public async Task<bool> ChangeUsernameAsync(int userID, string newUsername, ModelStateDictionary modelState)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (user == null)
            {
                modelState.AddModelError("UserID", "当前用户不存在");
                return false;
            }
            if (!user.Username.IsNullOrWhiteSpace() &&
                user.Username.Equals(newUsername, StringComparison.InvariantCultureIgnoreCase))
            {
                modelState.AddModelError("UserID", "目标用户名和当前用户名相同");
                return false;
            }
            if (DbContext.Users.Any(m => m.UserID != userID && m.Username == newUsername))
            {
                modelState.AddModelError("UserID", "用户名[{0}]已经被使用".FormatWith(newUsername));
                return false;
            }
            user.Username = newUsername;
            await DbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangeMobileAsync(int userID, string newMobile, ModelStateDictionary modelState)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (user == null)
            {
                modelState.AddModelError("UserID", "当前用户不存在");
                return false;
            }
            if (!user.Mobile.IsNullOrWhiteSpace() &&
                user.Mobile.Equals(newMobile, StringComparison.InvariantCultureIgnoreCase))
            {
                modelState.AddModelError("UserID", "目标手机号和当前手机号相同");
                return false;
            }
            if (DbContext.Users.Any(m => m.UserID != userID && m.Mobile == newMobile))
            {
                modelState.AddModelError("UserID", "手机号[{0}]已经被使用".FormatWith(newMobile));
                return false;
            }
            user.Mobile = newMobile;
            await DbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangeDisplayNameAsync(int userID, string newDisplayName)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (user == null)
                return false;
            user.DisplayName = newDisplayName;
            await DbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangeLogoAsync(int userID, string logoURL)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (user == null)
                return false;
            user.LogoURL = logoURL;
            await DbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangePasswordAsync(int userID, string newPassword)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (user == null)
                return false;

            user.Password = newPassword;
            await DbContext.SaveChangesAsync();

            return true;
        }
        public async Task<int> ChangePasswordAsync(string username, string newPassword)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(m => m.Username == username);
            if (user == null)
                return 0;

            user.Password = newPassword;
            await DbContext.SaveChangesAsync();

            return user.UserID;
        }
        public async Task<int> ResetPasswordByMobileAsync(string username, string newPassword, ModelStateDictionary modelState)
        {
            var user =
                await DbContext.Users.FirstOrDefaultAsync(m => m.Username == username || (m.MobileIsValid && m.Mobile == username) || (m.EmailIsValid && m.Email == username));
            if (user == null)
            {
                modelState.AddModelError("Mobile", "重置密码失败:用户不存在");
                return 0;
            }

            user.Password = newPassword;
            await DbContext.SaveChangesAsync();

            return user.UserID;
        }
        public async Task<bool> ChangeHeadAsync(int userID, string newHeadURL)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (user == null)
                return false;

            user.HeadURL = newHeadURL;
            await DbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> RemoveAsync(int userID, ModelStateDictionary modelState)
        {
            User user = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (user == null)
                return false;
            using (var dbContextTransaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    const string sql = "DELETE [NotificationUser] WHERE UserID = @UserID; " +
                        "DELETE [Notification] WHERE FromUserID = @UserID OR ToUserID = @UserID;" +
                        "DELETE UserRoleRelationship WHERE UserID = @UserID;" +
                        "DELETE UserPermissionRelationship WHERE UserID = @UserID;"
                        ;
                    await DbContext.Database.ExecuteSqlCommandAsync(sql, new SqlParameter("UserID", userID));

                    DbContext.Users.Remove(user);
                    await DbContext.SaveChangesAsync();

                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    modelState.AddModelError("Exception", ex.Message);
                    return false;
                }

            }

            return true;
        }
        public async Task<bool> ChangeStatusAsync(int userID, XM.UserStatus status)
        {
            User user = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (user == null) return false;
            user.Status = status;
            await DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateClientAgentAsync(int userID, String clientAgent, String ip)
        {
            var item = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (item == null) return false;
            item.ClientAgent = clientAgent;
            var log = new Log
            {
                UserID = userID,
                TypeID = 1,
                IP = ip,
                CreationDate = DateTime.Now,
            };
            DbContext.Logs.Add(log);
            await DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateTokenAsync(int userID, String token)
        {
            var item = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (item == null) return false;
            item.Token = token;
            await DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateWXOpenIDAsync(int userID, String wxOpenID, ModelStateDictionary modelState)
        {
            if (wxOpenID.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("WXOpenID", "未知微信");
                return false;
            }
            // 微信已经被使用
            var user = await DbContext.Users.FirstOrDefaultAsync(m => m.WXOpenID == wxOpenID);
            if (user != null)
            {
                if (user.UserID != userID)
                {
                    // 微信已经绑定本人
                    return true;
                }
                else
                {
                    // 微信已经被他人绑定
                    modelState.AddModelError("WXOpenID", "微信已经绑定了其他用户");
                    return false;
                }
            }

            // 本人已经绑定
            user = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (user == null)
            {
                modelState.AddModelError("UserID", "用户不存在");
                return false;
            }
            user.WXOpenID = wxOpenID;
            await DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CleanWXOpenIDAsync(int userID)
        {
            var item = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (item == null) return false;
            // 不判断本人是否已经绑定
            item.WXOpenID = null;
            await DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateWXAOpenIDAsync(int userID, String wxaOpenID, ModelStateDictionary modelState)
        {
            if (wxaOpenID.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("WXOpenID", "未知微信");
                return false;
            }
            // 微信已经被使用
            var user = await DbContext.Users.FirstOrDefaultAsync(m => m.WXAOpenID == wxaOpenID);
            if (user != null)
            {
                if (user.UserID == userID)
                {
                    // 微信已经绑定本人
                    return true;
                }
                else
                {
                    // 微信已经被他人绑定
                    modelState.AddModelError("WXOpenID", "微信已经绑定了其他用户");
                    return false;
                }
            }

            // 本人已经绑定
            user = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (user == null)
            {
                modelState.AddModelError("UserID", "用户不存在");
                return false;
            }
            user.WXAOpenID = wxaOpenID;
            await DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CleanWXAOpenIDAsync(int userID)
        {
            var item = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID);
            if (item == null) return false;
            // 不判断本人是否已经绑定
            item.WXAOpenID = null;
            await DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ClearClientAgentAsync(int userID, String clientAgent)
        {
            var item = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == userID && m.ClientAgent == clientAgent);
            if (item == null) return false;
            item.ClientAgent = null;
            await DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<string> GetMobileValidationCode(GetMobileValidationCodeInput getMobileValidationCodeInput, ModelStateDictionary modelState)
        {
            /* 备注：
             * 1、用户注册时先判断用户是否已经注册
             * 2、如果验证码不存在、类型不一致、到期、验证次数超标、已完成验证，则生成新的验证码。
             * 3、通过返回值判断是否请求过于频繁。
             * 
             * TypeID： 1. 注册 2.重设密码 3.更换手机号 4 短信登录(如果没注册，则自动注册) 
             */
            if (getMobileValidationCodeInput.TypeID == 1)
            {
                if (await DbContext.Users.Where(m => m.Mobile == getMobileValidationCodeInput.Mobile).AnyAsync())
                {
                    modelState.AddModelError("Mobile", "手机号码已经被使用");
                    return String.Empty;
                }
            }
            else if (getMobileValidationCodeInput.TypeID == 2 || getMobileValidationCodeInput.TypeID == 3)
            {
                if (!await DbContext.Users.Where(m => m.Mobile == getMobileValidationCodeInput.Mobile).AnyAsync())
                {
                    modelState.AddModelError("Mobile", "手机号码尚未注册");
                    return String.Empty;
                }
            }
            else if (getMobileValidationCodeInput.TypeID != 4)
            {
                modelState.AddModelError("Mobile", "未知目的");
                return String.Empty;
            }

            var code = await DbContext.MobileValidationCodes.FirstOrDefaultAsync(m => m.Mobile == getMobileValidationCodeInput.Mobile);

            var now = DateTime.Now;
            if (code != null)
            {
                if (now - code.CreationDate < TimeSpan.FromMinutes(MobileValidationCodeRequestRateInterval))
                {
                    modelState.AddModelError("Mobile", "请求过于频繁，请稍后再试");
                    return String.Empty;
                }

                if (!code.ValidationCode.IsNullOrWhiteSpace() &&
                    code.TypeID != getMobileValidationCodeInput.TypeID /* 验证码用途更改 */ &&
                    code.ExpirationDate <= now /* 验证码没到期 */ &&
                    code.VerifyTimes < code.MaxVerifyTimes /* 验证码在合理使用次数内 */ &&
                    code.FinishVerifyDate == null /* 验证码没完成使用 */)
                {
                    return code.ValidationCode;
                }
            }
            else
            {
                code = new MobileValidationCode
                {
                    Mobile = getMobileValidationCodeInput.Mobile,
                };
                DbContext.MobileValidationCodes.Add(code);
            }

            code.TypeID = getMobileValidationCodeInput.TypeID;
            code.ValidationCode = GenerateValidationCode(MobileValidationCodeLength);
            code.CreationDate = now;
            code.ExpirationDate = now.AddMinutes(MobileValidationCodeExpirationInterval);
            code.VerifyTimes = 0;
            code.MaxVerifyTimes = MobileValidationCodeMaxVerifyTimes;
            code.FinishVerifyDate = null;

            await DbContext.SaveChangesAsync();

            return code.ValidationCode;
        }
        public async Task<bool> VerifyMobileValidationCode(VerifyMobileValidationCodeInput verifyMobileValidationCodeInput, ModelStateDictionary modelState)
        {
            /* 备注：
             * 1、如果验证码不存在、类型不匹配、验证次数过多，则报错。
             */
            var code =
                 await
                 DbContext.MobileValidationCodes.FirstOrDefaultAsync(
                     m => m.Mobile == verifyMobileValidationCodeInput.Mobile && m.TypeID == verifyMobileValidationCodeInput.TypeID);

            if (code == null)
            {
                modelState.AddModelError("Mobile", "尚未请求验证码");
                return false;
            }

            code.VerifyTimes++;
            await DbContext.SaveChangesAsync();

            if (code.ValidationCode.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("Mobile", "异常：尚未生成验证码");
                return false;
            }
            if (code.TypeID != verifyMobileValidationCodeInput.TypeID)
            {
                modelState.AddModelError("Mobile", "验证码类型错误，请重新请求");
                return false;
            }
            if (code.VerifyTimes > code.MaxVerifyTimes)
            {
                modelState.AddModelError("Mobile", "验证码验证次数过多，请重新请求");
                return false;
            }
            if (DateTime.Now > code.ExpirationDate)
            {
                modelState.AddModelError("Mobile", "验证码已经过期，请重新请求");
                return false;
            }
            if (code.FinishVerifyDate != null)
            {
                modelState.AddModelError("Mobile", "验证码已经使用，请重新请求");
                return false;
            }
            if (!code.ValidationCode.Equals(verifyMobileValidationCodeInput.ValidationCode, StringComparison.InvariantCultureIgnoreCase))
            {
                modelState.AddModelError("Mobile", "验证码输入错误，请重新输入");
                return false;
            }

            return true;
        }
        public async Task<bool> FinishVerifyMobileValidationCode(string mobile, int typeID, ModelStateDictionary modelState)
        {
            var code =
                 await
                 DbContext.MobileValidationCodes.FirstOrDefaultAsync(
                     m => m.Mobile == mobile && m.TypeID == typeID);
            if (code == null || code.ValidationCode.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("Mobile", "尚未请求验证码");
                return false;
            }

            code.FinishVerifyDate = DateTime.Now;
            await DbContext.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Private Methods

        private async Task ProjectUserAsync(XM.UserInfo user)
        {
            if (user == null) return;
            await SetGroupRolePermissionsAsync(user);
        }
        private async Task SetGroupRolePermissionsAsync(XM.UserInfo user)
        {
            var permissions = from ug in DbContext.Groups
                              from r in ug.Roles
                              from p in r.Permissions
                              where ug.GroupID == user.Group.GroupID
                              select new XM.PermissionBase
                              {
                                  ModuleName = p.ModuleName,
                                  PermissionID = p.PermissionID,
                                  Name = p.Name
                              };
            user.GroupRolesPermissions = await permissions.ToListAsync();
        }
        private void ProjectUser(XM.UserInfo user)
        {
            if (user == null) return;
            SetGroupRolePermissions(user);
        }
        private void SetGroupRolePermissions(XM.UserInfo user)
        {
            var permissions = from ug in DbContext.Groups
                              from r in ug.Roles
                              from p in r.Permissions
                              where ug.GroupID == user.Group.GroupID
                              select new XM.PermissionBase
                              {
                                  ModuleName = p.ModuleName,
                                  PermissionID = p.PermissionID,
                                  Name = p.Name
                              };
            user.GroupRolesPermissions = permissions.ToList();
        }
        private string GenerateValidationCode(int codeLength)
        {
            int[] randMembers = new int[codeLength];
            int[] validateNums = new int[codeLength];
            string validateNumberStr = String.Empty;
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - codeLength * 10000);
            int[] seeks = new int[codeLength];
            for (int i = 0; i < codeLength; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (int i = 0; i < codeLength; i++)
            {
                var rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, codeLength);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字
            for (int i = 0; i < codeLength; i++)
            {
                string numStr = randMembers[i].ToString(CultureInfo.InvariantCulture);
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (int i = 0; i < codeLength; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        #endregion

        public static string GeneratePassword(string rawPassword)
        {
            if (rawPassword.IsNullOrWhiteSpace()) return String.Empty;
            string passwordSalt = Guid.NewGuid().ToString("N");
            string data = SHA256.Encrypt(rawPassword, passwordSalt);
            return "{0}|{1}".FormatWith(passwordSalt, data);
        }

    }
}
