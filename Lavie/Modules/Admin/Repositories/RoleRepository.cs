using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Models;
using Lavie.Modules.Admin.Models.InputModels;
using XM = Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Repositories
{
    public interface IRoleRepository
    {
        Task<XM.Role> GetItemAsync(Guid roleID);
        Task<XM.Role> GetItemAsync(string name);
        Task<List<XM.RoleBase>> GetBaseListAsync();
        Task<List<XM.Role>> GetListAsync();
        Task<XM.Role> SaveAsync(RoleInput roleInput, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(Guid roleID, ModelStateDictionary modelState);
        Task<bool> SaveNameAsync(SaveRoleNameInput saveRoleNameInput, ModelStateDictionary modelState);
        Task<bool> MoveAsync(Guid roleID, MovingTarget target);
        Task<bool> MoveAsync(int sourceDisplayOrder, int targetDisplayOrder, ModelStateDictionary modelState);
        Task<bool> MoveAsync(Guid sourceRoleID, Guid targetRoleID, ModelStateDictionary modelState);
    }

    public class RoleRepository : RepositoryBase, IRoleRepository
    {
        private readonly Expression<Func<Role, XM.Role>> _selector;

        public RoleRepository()
        {
            _selector = r => new XM.Role
            {
                RoleID = r.RoleID,
                Name = r.Name,
                IsSystem = r.IsSystem,
                DisplayOrder = r.DisplayOrder,
                Permissions = from p in r.Permissions
                              orderby p.DisplayOrder
                              select new XM.PermissionBase
                              {
                                  ModuleName = p.ModuleName,
                                  PermissionID = p.PermissionID,
                                  Name = p.Name
                              }
            };
        }

        #region IRoleRepository 成员

        public async Task<XM.Role> GetItemAsync(Guid roleID)
        {
            return await DbContext.Roles.Select(_selector).FirstOrDefaultAsync(m => m.RoleID == roleID);
        }

        public async Task<XM.Role> GetItemAsync(string name)
        {
            return await DbContext.Roles.Select(_selector).FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task<List<XM.RoleBase>> GetBaseListAsync()
        {
            return await (from r in DbContext.Roles
                          orderby r.DisplayOrder
                          select new XM.RoleBase
                          {
                              RoleID = r.RoleID,
                              Name = r.Name,
                              IsSystem = r.IsSystem,
                              DisplayOrder = r.DisplayOrder
                          }).AsNoTracking().ToListAsync();
        }

        public async Task<List<XM.Role>> GetListAsync()
        {
            return await DbContext.Roles.AsNoTracking().OrderBy(m => m.DisplayOrder).Select(_selector).AsNoTracking().ToListAsync();
        }

        public async Task<XM.Role> SaveAsync(RoleInput roleInput, ModelStateDictionary modelState)
        {
            Role roleToSave;
            if (roleInput.RoleID.HasValue)
            {
                roleToSave = await DbContext.Roles.FirstOrDefaultAsync(m => m.RoleID == roleInput.RoleID.Value);
                if (roleToSave == null)
                {
                    modelState.AddModelError("RoleID", "尝试编辑不存在的记录");
                    return null;
                }
            }
            else
            {
                roleToSave = new Role
                {
                    RoleID = Guid.NewGuid(),
                    IsSystem = false
                };
                DbContext.Roles.Add(roleToSave);
                int maxDisplayOrder = await DbContext.Roles.MaxAsync(m => (int?)m.DisplayOrder) ?? 0;
                roleToSave.DisplayOrder = maxDisplayOrder + 1;
            }
            roleToSave.Name = roleInput.Name;

            #region 角色权限
            if (!roleToSave.Permissions.IsNullOrEmpty())
            {
                // 移除项
                if (!roleInput.PermissionIDs.IsNullOrEmpty())
                {
                    List<Permission> permissionToRemove = (from p in roleToSave.Permissions
                                                           where !roleInput.PermissionIDs.Contains(p.PermissionID)
                                                           select p).ToList();
                    for (int i = 0; i < permissionToRemove.Count; i++)
                        roleToSave.Permissions.Remove(permissionToRemove[i]);
                }
                else
                {
                    roleToSave.Permissions.Clear();
                }
            }
            if (!roleInput.PermissionIDs.IsNullOrEmpty())
            {            
                // 添加项
                // 要添加的ID集
                List<Guid> permissionIDToAdd = (from p in roleInput.PermissionIDs
                                                where roleToSave.Permissions.All(m => m.PermissionID != p)
                                                select p).ToList();

                // 要添加的项
                List<Permission> permissionToAdd = await (from p in DbContext.Permissions
                                                          where permissionIDToAdd.Contains(p.PermissionID)
                                                          select p).ToListAsync();
                foreach (var item in permissionToAdd)
                    roleToSave.Permissions.Add(item);

            }
            #endregion

            await DbContext.SaveChangesAsync();

            var role = (new Role[] { roleToSave }).Select(_selector.Compile()).First();

            return role;
        }

        public async Task<bool> RemoveAsync(Guid roleID, ModelStateDictionary modelState)
        {
            var roleToRemove = DbContext.Roles.FirstOrDefault(m => m.RoleID == roleID);
            if (roleToRemove == null || roleToRemove.IsSystem)
            {
                modelState.AddModelError("RoleID", "记录不存在");
                return false;
            }

            using (var dbContextTransaction = DbContext.Database.BeginTransaction())
            {
                string sql;
                sql = "Update Role Set DisplayOrder=DisplayOrder-1 Where DisplayOrder>@DisplayOrder";
                await DbContext.Database.ExecuteSqlCommandAsync(sql, new SqlParameter("DisplayOrder", roleToRemove.DisplayOrder));

                // Delete GroupRoleLimit Where RoleID=@RoleID
                sql = "Delete RolePermissionRelationship Where RoleID=@RoleID; Delete GroupRoleRelationship Where RoleID=@RoleID; UPDATE [User] SET RoleID = null WHERE RoleID=@RoleID;";
                await DbContext.Database.ExecuteSqlCommandAsync(sql, new SqlParameter("RoleID", roleID));

                DbContext.Roles.Remove(roleToRemove);

                await DbContext.SaveChangesAsync();
                dbContextTransaction.Commit();
            }

            return true;
        }

        public async Task<bool> SaveNameAsync(SaveRoleNameInput saveRoleNameInput, ModelStateDictionary modelState)
        {
            var roleToRemove = DbContext.Roles.FirstOrDefault(m => m.RoleID == saveRoleNameInput.RoleID);
            if (roleToRemove == null || roleToRemove.IsSystem)
            {
                modelState.AddModelError("RoleID", "记录不存在");
                return false;
            }

            if (saveRoleNameInput.Name != roleToRemove.Name)
            {
                roleToRemove.Name = saveRoleNameInput.Name;
                await DbContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> MoveAsync(Guid roleID, MovingTarget target)
        {
            var roleToMove = await DbContext.Roles.FirstOrDefaultAsync(m => m.RoleID == roleID);

            // 保证DisplayOrder为 1 的“系统管理员”不被移动
            if (roleToMove == null || roleToMove.DisplayOrder == 1) return false;

            // 防止DisplayOrder为2的项非法篡改"系统管理员",也就是说DisplayOrder必须大于2
            if (MovingTarget.Up == target)
            {
                if (roleToMove.DisplayOrder < 3) return false;

                var targetRole = await DbContext.Roles.OrderByDescending(m => m.DisplayOrder).FirstOrDefaultAsync(m => m.DisplayOrder < roleToMove.DisplayOrder);
                // 某种原因导致当前项之前已经没有项了
                if (targetRole == null) return false;

                roleToMove.DisplayOrder--;
                targetRole.DisplayOrder++;
                await DbContext.SaveChangesAsync();

            }
            else if (MovingTarget.Down == target)
            {
                var targetRole = await DbContext.Roles.OrderBy(m => m.DisplayOrder).FirstOrDefaultAsync(m => m.DisplayOrder > roleToMove.DisplayOrder);
                // 某种原因导致当前项之后已经没有项了
                if (targetRole == null) return false;

                roleToMove.DisplayOrder++;
                targetRole.DisplayOrder--;
                await DbContext.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> MoveAsync(int sourceDisplayOrder, int targetDisplayOrder, ModelStateDictionary modelState)
        {
            if (sourceDisplayOrder == targetDisplayOrder)
            {
                modelState.AddModelError("SourceDisplayOrder", "源DisplayOrder和目标DisplayOrder不能相同");
                return false;
            }
            var sourceRole = await DbContext.Roles.FirstOrDefaultAsync(m => m.DisplayOrder == sourceDisplayOrder);
            if (sourceRole == null)
            {
                modelState.AddModelError("SourceDisplayOrder", "源记录不存在");
                return false;
            }
            var targetRole = await DbContext.Roles.FirstOrDefaultAsync(m => m.DisplayOrder == targetDisplayOrder);
            if (targetRole == null)
            {
                modelState.AddModelError("TargetDisplayOrder", "目标记录不存在");
                return false;
            }
            return await MoveAsync(sourceRole, targetRole, modelState);
        }

        public async Task<bool> MoveAsync(Guid sourceRoleID, Guid targetRoleID, ModelStateDictionary modelState)
        {
            if (sourceRoleID == targetRoleID)
            {
                modelState.AddModelError("SourceGroupID", "源ID和目标ID不能相同");
                return false;
            }
            var sourceRole = await DbContext.Roles.FirstOrDefaultAsync(m => m.RoleID == sourceRoleID);
            if (sourceRole == null)
            {
                modelState.AddModelError("SourceGroupID", "源记录不存在");
                return false;
            }
            var targetRole = await DbContext.Roles.FirstOrDefaultAsync(m => m.RoleID == targetRoleID);
            if (targetRole == null)
            {
                modelState.AddModelError("TargetGroupID", "目标记录不存在");
                return false;
            }

            return await MoveAsync(sourceRole, targetRole, modelState);
        }

        private async Task<bool> MoveAsync(Role sourceRole, Role targetRole, ModelStateDictionary modelState)
        {
            if (sourceRole.DisplayOrder == targetRole.DisplayOrder)
            {
                modelState.AddModelError("SourceGroupID", "源DisplayOrder和目标DisplayOrder不能相同");
                return false;
            }

            // 不允许移动系统管理员
            if (sourceRole.DisplayOrder == 1)
            {
                modelState.AddModelError("SourceGroupID", "不允许移动系统管理员");
                return false;
            }

            // 不允许移动到系统管理员之前
            if (targetRole.DisplayOrder == 1)
            {
                modelState.AddModelError("SourceGroupID", "不允许移动到系统管理员之前");
                return false;
            }

            using (var dbContextTransaction = DbContext.Database.BeginTransaction())
            {
                string sql;
                if (sourceRole.DisplayOrder > targetRole.DisplayOrder)
                {
                    // 向上移动。目标节点及以下，至，源节点之间的节点，序号 + 1
                    sql = "Update Role Set DisplayOrder = DisplayOrder + 1 Where DisplayOrder >= @TargetDisplayOrder And DisplayOrder < @SourceDisplayOrder;";
                }
                else
                {
                    // 向下移动。目标节点及以上，至，源节点之间的节点，序号 - 1
                    sql = "Update Role Set DisplayOrder = DisplayOrder - 1 Where DisplayOrder <= @TargetDisplayOrder And DisplayOrder > @SourceDisplayOrder;";
                }

                await DbContext.Database.ExecuteSqlCommandAsync(sql,
                    new SqlParameter("SourceDisplayOrder", sourceRole.DisplayOrder),
                    new SqlParameter("TargetDisplayOrder", targetRole.DisplayOrder));

                sourceRole.DisplayOrder = targetRole.DisplayOrder;

                await DbContext.SaveChangesAsync();
                dbContextTransaction.Commit();
            }

            return true;
        }

        #endregion

    }
}
