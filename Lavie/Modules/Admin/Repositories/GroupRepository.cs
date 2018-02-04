using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Models;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Modules.Admin.Repositories;
using XM = Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Repositories
{
    public interface IGroupRepository
    {
        Task<XM.Group> GetItemAsync(Guid groupID);
        Task<XM.Group> GetItemAsync(string name);
        Task<List<XM.Group>> GetListAsync(Guid? parentID = null);
        Task<List<XM.GroupBase>> GetBasePathAsync(Guid groupID);
        Task<List<XM.GroupInfo>> GetInfoPathAsync(Guid groupID);
        Task<bool> SaveAsync(GroupInput groupInput, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(Guid groupID, ModelStateDictionary modelState);
        Task<bool> MoveAsync(Guid groupID, MovingTarget movingTarget);
        Task<bool> MoveAsync(Guid sourceGroupID, Guid targetGroupID, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState);
        Task<bool> MoveAsync(int sourceDisplayOrder, int targetDisplayOrder, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState);
    }

    public class GroupRepository : RepositoryBase, IGroupRepository
    {
        private readonly Expression<Func<Group, XM.Group>> _selector;

        public GroupRepository()
        {
            _selector = ug => new XM.Group
            {
                GroupID = ug.GroupID,
                Name = ug.Name,
                IsIncludeUser = ug.IsIncludeUser,
                IsDisabled = ug.IsDisabled,
                IsSystem = ug.IsSystem,
                DisplayOrder = ug.DisplayOrder,
                ParentID = ug.ParentID,
                Level = ug.Level,
                Roles = from r in ug.Roles
                        orderby r.DisplayOrder
                        select new XM.RoleBase
                        {
                            RoleID = r.RoleID,
                            Name = r.Name,
                            IsSystem = r.IsSystem,
                            DisplayOrder = r.DisplayOrder
                        },
                LimitRoles = from r in ug.LimitRoles
                             orderby r.DisplayOrder
                             select new XM.RoleBase
                             {
                                 RoleID = r.RoleID,
                                 Name = r.Name,
                                 IsSystem = r.IsSystem,
                                 DisplayOrder = r.DisplayOrder
                             },
                Permissions = from p in ug.Permissions
                              orderby p.DisplayOrder
                              select new XM.PermissionBase
                              {
                                  PermissionID = p.PermissionID,
                                  ModuleName = p.ModuleName,
                                  Name = p.Name,
                              }
            };
        }

        #region IGroupRepository 成员

        public async Task<XM.Group> GetItemAsync(Guid groupID)
        {
            return await DbContext.Groups.Select(_selector).FirstOrDefaultAsync(m => m.GroupID == groupID);
        }
        public async Task<XM.Group> GetItemAsync(string name)
        {
            return await DbContext.Groups.Select(_selector).FirstOrDefaultAsync(m => m.Name == name);
        }
        public async Task<List<XM.Group>> GetListAsync(Guid? parentID = null)
        {
            if (parentID.HasValue)
            {
                var parent = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == parentID.Value);
                if (parent == null)
                    return new List<XM.Group>();
                else
                {
                    int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(parent.DisplayOrder, parent.Level);
                    if (displayOrderOfNextParentOrNextBrother != 0)
                        return await DbContext.Groups.Where(m => m.DisplayOrder >= parent.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrother)
                            .OrderBy(m => m.DisplayOrder)
                            .Select(_selector)
                            .AsNoTracking()
                            .ToListAsync();
                    else
                        return await DbContext.Groups.Where(m => m.DisplayOrder >= parent.DisplayOrder)
                            .OrderBy(m => m.DisplayOrder)
                            .Select(_selector)
                            .AsNoTracking()
                            .ToListAsync();
                }
            }
            else
            {
                return await DbContext.Groups
                    .OrderBy(m => m.DisplayOrder)
                    .Select(_selector)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
        public async Task<List<XM.GroupBase>> GetBasePathAsync(Guid groupID)
        {
            const string sql = @"WITH CET AS
                     (
                     SELECT GroupID,Name,DisplayOrder,ParentID,IsIncludeUser,IsSystem
                     FROM [Group]
                     WHERE  GroupID = @GroupID
                     UNION ALL
                     SELECT P.GroupID,P.Name,P.DisplayOrder,P.ParentID,P.IsIncludeUser,P.IsSystem
                     FROM [Group] P
                     JOIN CET Curr ON Curr.ParentID = P.GroupID
                    )
                    SELECT GroupID,Name,DisplayOrder,ParentID,IsIncludeUser,IsSystem 
                    FROM CET ORDER BY DisplayOrder";

            return await DbContext.Database.SqlQuery<XM.GroupBase>(sql, new SqlParameter("GroupID", groupID)).ToListAsync();
        }
        public async Task<List<XM.GroupInfo>> GetInfoPathAsync(Guid groupID)
        {
            const string sql = @"WITH CET AS
                     (
                     SELECT GroupID,Name,ParentID,DisplayOrder
                     FROM [Group]
                     WHERE  GroupID = @GroupID
                     UNION ALL
                     SELECT P.GroupID,P.Name,P.ParentID
                     FROM [Group] P
                     JOIN CET Curr ON Curr.ParentID = P.GroupID
                    )
                    SELECT GroupID,Name,DisplayOrder,ParentID
                    FROM CET ORDER BY DisplayOrder";

            return await DbContext.Database.SqlQuery<XM.GroupInfo>(sql, new SqlParameter("GroupID", groupID)).ToListAsync();
        }
        public async Task<bool> SaveAsync(GroupInput groupInput, ModelStateDictionary modelState)
        {
            string sql;

            Group groupToSave = null;
            Group parent = null;
            if (!groupInput.GroupID.IsNullOrEmpty())
            {
                if (groupInput.GroupID == groupInput.ParentID)
                {
                    modelState.AddModelError("GroupID", "尝试将自身作为父节点");
                    return false;
                }

                groupToSave = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == groupInput.GroupID.Value);
                if (groupToSave == null)
                {
                    modelState.AddModelError("GroupID", "尝试编辑不存在的记录");
                    return false;
                }

                if (groupToSave.IsSystem)
                {
                    modelState.AddModelError("GroupID", "当前用户组是系统分组，不允许编辑");
                    return false;
                }
            }

            if (!groupInput.ParentID.IsNullOrEmpty())
            {
                parent = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == groupInput.ParentID.Value);
                if (parent == null)
                {
                    modelState.AddModelError("GroupID", "尝试添加或编辑至不存在的父节点上");
                    return false;
                }
                if (parent.IsSystem)
                {
                    modelState.AddModelError("GroupID", "不允许将节点添加至系统节点上");
                    return false;
                }
            }

            // 添加操作
            if (groupToSave == null)
            {
                #region 添加操作
                // 创建要保存的对象
                groupToSave = new Group
                {
                    GroupID = Guid.NewGuid(),
                    ParentID = groupInput.ParentID,
                    IsSystem = false,
                };
                DbContext.Groups.Add(groupToSave);
                if (parent == null)
                {
                    // 如果添加的是新的顶级节点,直接添加到末尾，不会影响其他节点
                    groupToSave.DisplayOrder = await GetMaxDisplayOrder() + 1;
                    groupToSave.Level = 1;
                }
                else
                {
                    //如果添加的是子节点，会影响其他节点的DisplayOrder

                    //父节点树的最大DisplayerOrder
                    int maxDisplayOrderInParentTree = await GetMaxDisplayOrderInTree(groupInput.ParentID.Value);
                    //父节点树的最大DisplayerOrder基础上加1作为保存对象的DisplayOrder
                    groupToSave.DisplayOrder = maxDisplayOrderInParentTree + 1;
                    //父节点的Level基础上加1作为保存对象的Level
                    groupToSave.Level = parent.Level + 1;

                    //父节点树之后的所有节点的DisplayOrder加1
                    sql = "Update [Group] Set DisplayOrder = DisplayOrder + 1 Where DisplayOrder > @DisplayOrder";
                    await DbContext.Database.ExecuteSqlCommandAsync(sql, new SqlParameter("DisplayOrder", maxDisplayOrderInParentTree));
                }

                #endregion
            }
            else if(groupInput.ParentID != groupToSave.ParentID)
            {
                //如果父节点不改变，则仅仅保存数据就行了。下面处理的是父节点改变了的情况
                //如果父节点改变(从无父节点到有父节点，从有父节点到无父节点，从一个父节点到另一个父节点)
                groupToSave.ParentID = groupInput.ParentID;

                //获取当前节点的下一个兄弟节点或更高层下一个父节点（不是自己的父节点）的DisplayOrder
                int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(groupToSave.DisplayOrder, groupToSave.Level);
                // 当前节点树ID集合
                var currTreeIDs = await GetTreeIDListAsync(groupToSave, displayOrderOfNextParentOrNextBrother, true);
                int currentTreeItemCount = currTreeIDs.Count;

                if (!groupToSave.ParentID.HasValue)
                {
                    //当前节点将由子节点升为顶级节点，直接将该节点数移到最后

                    #region 将由子节点升为顶级节点（成为最后一个顶级节点）

                    //需要提升的层级数
                    int xLevel = groupToSave.Level - 1;

                    if (displayOrderOfNextParentOrNextBrother == 0)
                    {
                        //当前节点树之后已无任何节点
                        //将当前节点树的所有节点的Level都进行提升
                        sql = "Update [Group] Set Level = Level - @Level Where DisplayOrder>=@DisplayOrder";
                        await DbContext.Database.ExecuteSqlCommandAsync(sql
                            , new SqlParameter("Level", xLevel)
                            , new SqlParameter("DisplayOrder", groupToSave.DisplayOrder)
                            );
                    }
                    else
                    {
                        //当前节点树之后还有节点，应该将这些节点的向前面排，并且将当前节点树的所有节点往后排
                        //当前节点树之后的节点数量
                        int nextItemCount = await DbContext.Groups.CountAsync(m => m.DisplayOrder >= displayOrderOfNextParentOrNextBrother);

                        sql = "Update [Group] Set DisplayOrder = DisplayOrder - @CTIC Where DisplayOrder>=@DOONPONB";

                        await DbContext.Database.ExecuteSqlCommandAsync(sql
                            , new SqlParameter("CTIC", currentTreeItemCount)
                            , new SqlParameter("DOONPONB", displayOrderOfNextParentOrNextBrother)
                            );

                        sql = "Update [Group] Set Level = Level - @Level,DisplayOrder = DisplayOrder + @NextItemCount Where 1<>1 ";
                        foreach (var id in currTreeIDs)
                            sql += " Or GroupID = '{0}'".FormatWith(id.ToString());

                        await DbContext.Database.ExecuteSqlCommandAsync(sql
                            , new SqlParameter("Level", xLevel)
                            , new SqlParameter("NextItemCount", nextItemCount)
                            );

                    }

                    #endregion
                }
                else
                {
                    //当前节点将改变父节点，包括从顶级节点移至另一节点下，或从当前父节点下移至另一节点下

                    #region 从顶级节点移至另一节点下，或从当前父节点下移至另一节点下（成为目标节点的最有一个子节点）

                    //目标父节点
                    var newParent = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == groupInput.ParentID.Value);

                    int xDisplayOrder = groupToSave.DisplayOrder - newParent.DisplayOrder;
                    int xLevel = groupToSave.Level - newParent.Level;

                    if (xDisplayOrder > 0) //从下往上移
                    {
                        #region 从下往上移
                        //特例处理，当前节点要移至的父节点就是上一个节点，只需要改变当前树Level
                        if (xDisplayOrder == 1)
                        {
                            sql = "Update [Group] Set Level = Level - @Level Where 1<>1 ";
                            foreach (var id in currTreeIDs)
                                sql += " Or GroupID = '{0}'".FormatWith(id.ToString());

                            await DbContext.Database.ExecuteSqlCommandAsync(sql
                                , new SqlParameter("Level", xLevel - 1)
                                );
                        }
                        else
                        {
                            //新的父节点和本节点之间的节点往下移动，DisplayOrder增加
                            sql = "Update [Group] Set DisplayOrder=DisplayOrder+@CurTreeCount Where DisplayOrder>@TDisplayOrder And DisplayOrder<@CDisplayOrder";
                            await DbContext.Database.ExecuteSqlCommandAsync(sql
                                , new SqlParameter("CurTreeCount", currentTreeItemCount)
                                , new SqlParameter("TDisplayOrder", newParent.DisplayOrder)
                                , new SqlParameter("CDisplayOrder", groupToSave.DisplayOrder)
                                );

                            sql = "Update [Group] Set DisplayOrder = DisplayOrder-@XCount,Level = Level - @Level Where 1<>1 ";
                            foreach (var id in currTreeIDs)
                                sql += " Or GroupID = '{0}'".FormatWith(id.ToString());
                            await DbContext.Database.ExecuteSqlCommandAsync(sql
                                , new SqlParameter("XCount", xDisplayOrder - 1)//也就是新节点和本节点之间的节点的数量
                                , new SqlParameter("Level", xLevel - 1)
                                );

                        }
                        #endregion
                    }
                    else//从上往下移
                    {
                        #region 从上往下移
                        // 本节点树下已经不存在任何节点了，当然无法向下移
                        if (displayOrderOfNextParentOrNextBrother == 0)
                        {
                            modelState.AddModelError("GroupID", "无法下移");
                            return false;
                        }

                        // 更新本节点树至新的父节点（包括新的父节点）之间的节点的DisplayOrder
                        sql = "Update [Group] Set DisplayOrder=DisplayOrder-@CurTreeCount Where DisplayOrder>=@DOONPONB And DisplayOrder<=@TDisplayOrder";
                        await DbContext.Database.ExecuteSqlCommandAsync(sql
                        , new SqlParameter("CurTreeCount", currentTreeItemCount)
                            , new SqlParameter("DOONPONB", displayOrderOfNextParentOrNextBrother)
                            , new SqlParameter("TDisplayOrder", newParent.DisplayOrder)
                            );

                        // 本节点至新的节点之间的节点数
                        int nextItemCount = newParent.DisplayOrder - displayOrderOfNextParentOrNextBrother + 1;
                        sql = "Update [Group] Set DisplayOrder = DisplayOrder+ @XCount,Level = Level - @Level Where 1<>1 ";
                        foreach (var id in currTreeIDs)
                            sql += " Or GroupID = '{0}'".FormatWith(id.ToString());
                        await DbContext.Database.ExecuteSqlCommandAsync(sql
                        , new SqlParameter("XCount", nextItemCount)
                            , new SqlParameter("Level", xLevel - 1)
                            );

                        #endregion
                    }

                    #endregion
                }
            }

            groupToSave.Name = groupInput.Name;
            groupToSave.IsIncludeUser = groupInput.IsIncludeUser;
            groupToSave.IsDisabled = groupInput.IsDisabled;

            #region 角色
            // 移除项
            if (!groupToSave.Roles.IsNullOrEmpty())
            {
                if (!groupInput.RoleIDs.IsNullOrEmpty())
                {
                    List<Role> roleToRemove = (from p in groupToSave.Roles
                                               where !groupInput.RoleIDs.Contains(p.RoleID)
                                               select p).ToList();
                    for (int i = 0; i < roleToRemove.Count; i++)
                        groupToSave.Roles.Remove(roleToRemove[i]);
                }
                else
                    groupToSave.Roles.Clear();
            }
            // 添加项
            if (!groupInput.RoleIDs.IsNullOrEmpty())
            {
                // 要添加的ID集
                List<Guid> roleIDToAdd = (from p in groupInput.RoleIDs
                                          where groupToSave.Roles.All(m => m.RoleID != p)
                                          select p).ToList();

                // 要添加的项
                List<Role> roleToAdd = await (from p in DbContext.Roles
                                              where roleIDToAdd.Contains(p.RoleID)
                                              select p).ToListAsync();
                foreach (var item in roleToAdd)
                    groupToSave.Roles.Add(item);

            }
            #endregion

            #region 限制角色
            // 移除项
            if (!groupToSave.LimitRoles.IsNullOrEmpty())
            {
                if (!groupInput.LimitRoleIDs.IsNullOrEmpty())
                {
                    List<Role> roleToRemove = (from p in groupToSave.LimitRoles
                                               where !groupInput.LimitRoleIDs.Contains(p.RoleID)
                                               select p).ToList();
                    for (int i = 0; i < roleToRemove.Count; i++)
                        groupToSave.LimitRoles.Remove(roleToRemove[i]);
                }
                else
                    groupToSave.LimitRoles.Clear();
            }
            // 添加项
            if (!groupInput.LimitRoleIDs.IsNullOrEmpty())
            {
                // 要添加的ID集
                List<Guid> roleIDToAdd = (from p in groupInput.LimitRoleIDs
                                          where groupToSave.LimitRoles.All(m => m.RoleID != p)
                                          select p).ToList();

                // 要添加的项
                List<Role> roleToAdd = await (from p in DbContext.Roles
                                              where roleIDToAdd.Contains(p.RoleID)
                                              select p).ToListAsync();
                foreach (var item in roleToAdd)
                    groupToSave.LimitRoles.Add(item);

            }
            #endregion

            #region 权限
            // 移除项
            if (!groupToSave.Permissions.IsNullOrEmpty())
            {
                if (!groupInput.PermissionIDs.IsNullOrEmpty())
                {
                    List<Permission> permissionToRemove = (from p in groupToSave.Permissions
                                                           where !groupInput.PermissionIDs.Contains(p.PermissionID)
                                                           select p).ToList();
                    for (int i = 0; i < permissionToRemove.Count; i++)
                        groupToSave.Permissions.Remove(permissionToRemove[i]);
                }
                else
                {
                    groupToSave.Permissions.Clear();
                }
            }
            // 添加项
            if (!groupInput.PermissionIDs.IsNullOrEmpty())
            {
                // 要添加的ID集
                List<Guid> permissionIDToAdd = (from p in groupInput.PermissionIDs
                                                where groupToSave.Permissions.All(m => m.PermissionID != p)
                                                select p).ToList();

                // 要添加的项
                List<Permission> permissionToAdd = await (from p in DbContext.Permissions
                                                          where permissionIDToAdd.Contains(p.PermissionID)
                                                          select p).ToListAsync();
                foreach (var item in permissionToAdd)
                    groupToSave.Permissions.Add(item);

            }
            #endregion

            await DbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> RemoveAsync(Guid groupID, ModelStateDictionary modelState)
        {
            // 移除无限级分类步骤：

            // 1、获取预删节点信息
            Group groupToRemove = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == groupID);
            // 当然，如果无法获取节点，属于无效操作；另外，超级管理员组和等待分配组不允许被删除
            if (groupToRemove == null)
            {
                modelState.AddModelError("GroupID", "尝试删除不存在的记录");
                return false;
            }
            if (groupToRemove.IsSystem)
            {
                modelState.AddModelError("GroupID", "当前用户组是系统分组，不允许删除");
                return false;
            }

            // 2、节点包含子节点不允许删除
            if (await DbContext.Groups.AnyAsync(m => m.ParentID == groupID))
            {
                modelState.AddModelError("GroupID", "当前用户组存在子分组，不允许删除");
                return false;
            }

            // 4、更新用户表
            using (var dbContextTransaction = DbContext.Database.BeginTransaction())
            {
                // 注：这里硬编码 DisplayOrder = 2 的分组是“待分配组”
                string sql = "Select GroupID From [Group] Where DisplayOrder = 2";
                Guid targetGroupID = await DbContext.Database.SqlQuery<Guid>(sql).FirstOrDefaultAsync();
                if (targetGroupID == Guid.Empty) return false;

                sql = "Update [User] Set GroupID=@TGroupID Where GroupID=@GroupID";
                await DbContext.Database.ExecuteSqlCommandAsync(sql,
                    new SqlParameter("GroupID", groupID)
                    , new SqlParameter("TGroupID", targetGroupID)
                    );

                // 4、更新DisplayOrder大于预删节点DisplayOrder的节点
                sql = "Update [Group] Set DisplayOrder=DisplayOrder-1 Where DisplayOrder>@DisplayOrder";
                await DbContext.Database.ExecuteSqlCommandAsync(sql,
                    new SqlParameter("DisplayOrder", groupToRemove.DisplayOrder)
                    );

                // 5、删除关联节点
                sql = "Delete [GroupRoleRelationship] Where GroupID=@GroupID";
                await DbContext.Database.ExecuteSqlCommandAsync(sql,
                    new SqlParameter("GroupID", groupID)
                    );

                // 6、删除节点
                DbContext.Groups.Remove(groupToRemove);
                await DbContext.SaveChangesAsync();

                dbContextTransaction.Commit();
            }


            return true;
        }
        public async Task<bool> MoveAsync(Guid groupID, MovingTarget movingTarget)
        {
            string sql;

            Group groupToMove = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == groupID);
            // 保证DisplayOrder为 1 的“系统管理组”和“等待分配组”不被移动
            if (groupToMove == null || groupToMove.DisplayOrder <= 2) return false;

            #region 获取当前节点树(包含自身)

            List<Guid> currTreeIDs;
            int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(groupToMove.DisplayOrder, groupToMove.Level);
            if (displayOrderOfNextParentOrNextBrother == 0)
            {
                currTreeIDs = await DbContext.Groups.Where(m => m.DisplayOrder >= groupToMove.DisplayOrder).Select(m => m.GroupID).ToListAsync();
            }
            else
            {
                currTreeIDs = await DbContext.Groups
                    .Where(m => m.DisplayOrder >= groupToMove.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrother)
                    .Select(m => m.GroupID)
                    .ToListAsync();

            }
            int curTreeCount = currTreeIDs.Count;

            #endregion

            if (MovingTarget.Up == movingTarget)
            {
                // 如果是处于两个系统分组之下的第一个节点，不允许上移
                if (groupToMove.DisplayOrder == 3) return false;

                #region 获取上一个兄弟节点

                Group targetGroup;
                if (groupToMove.ParentID.HasValue)
                    targetGroup = await DbContext.Groups.OrderByDescending(m => m.DisplayOrder).FirstOrDefaultAsync(m =>
                    m.ParentID == groupToMove.ParentID && m.DisplayOrder < groupToMove.DisplayOrder);
                else
                    targetGroup = await DbContext.Groups.OrderByDescending(m => m.DisplayOrder).FirstOrDefaultAsync(m =>
                    m.ParentID == null && m.DisplayOrder < groupToMove.DisplayOrder);
                #endregion

                if (targetGroup == null) return false;

                using (var dbContextTransaction = DbContext.Database.BeginTransaction())
                {
                    // 获取兄弟节点树的节点数
                    int targetTreeCount = await DbContext.Groups.CountAsync(m =>
                    m.DisplayOrder >= targetGroup.DisplayOrder
                    && m.DisplayOrder < groupToMove.DisplayOrder);

                    // 更新兄弟节点树的DisplayOrder
                    sql = "Update [Group] Set DisplayOrder = DisplayOrder + @CurTreeCount Where DisplayOrder >= @TDisplayOrder And DisplayOrder<@CDisplayOrder";
                    await DbContext.Database.ExecuteSqlCommandAsync(sql
                        , new SqlParameter("CurTreeCount", curTreeCount)
                        , new SqlParameter("TDisplayOrder", targetGroup.DisplayOrder)
                        , new SqlParameter("CDisplayOrder", groupToMove.DisplayOrder)
                        );

                    sql = "Update [Group] Set DisplayOrder = DisplayOrder - @TargetTreeCount Where 1 <> 1 ";
                    foreach (var id in currTreeIDs)
                        sql += " Or GroupID = '{0}'".FormatWith(id.ToString());
                    await DbContext.Database.ExecuteSqlCommandAsync(sql
                        , new SqlParameter("TargetTreeCount", targetTreeCount)
                        );

                    dbContextTransaction.Commit();
                }
            }
            else
            {
                #region 获取下一个兄弟节点

                Group targetGroup;
                if (groupToMove.ParentID.HasValue)
                    targetGroup = await DbContext.Groups.OrderBy(m => m.DisplayOrder).FirstOrDefaultAsync(m =>
                    m.ParentID == groupToMove.ParentID && m.DisplayOrder > groupToMove.DisplayOrder);
                else
                    targetGroup = await DbContext.Groups.OrderBy(m => m.DisplayOrder).FirstOrDefaultAsync(m =>
                    m.ParentID == null && m.DisplayOrder > groupToMove.DisplayOrder);

                #endregion

                // 如果已经是最后一个节点，不允许下移
                if (targetGroup == null) return false;

                #region 获取兄弟节点树的节点数

                int displayOrderOfNextParentOrNextBrotherOfTarget = await GetDisplayOrderOfNextParentOrNextBrother(targetGroup.DisplayOrder, targetGroup.Level);
                int targetTreeCount;
                if (displayOrderOfNextParentOrNextBrotherOfTarget == 0)
                    targetTreeCount = await DbContext.Groups.CountAsync(m => m.DisplayOrder >= targetGroup.DisplayOrder);
                else
                    targetTreeCount = await DbContext.Groups
                        .CountAsync(m => m.DisplayOrder >= targetGroup.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrotherOfTarget);

                #endregion

                using (var dbContextTransaction = DbContext.Database.BeginTransaction())
                {
                    // 更新兄弟节点树的DisplayOrder
                    sql = "Update [Group] Set DisplayOrder = DisplayOrder - @CurTreeCount Where DisplayOrder >= @DisplayOrder And DisplayOrder < @TDisplayOrder";

                    await DbContext.Database.ExecuteSqlCommandAsync(sql
                        , new SqlParameter("CurTreeCount", curTreeCount)
                        , new SqlParameter("DisplayOrder", targetGroup.DisplayOrder)
                        , new SqlParameter("TDisplayOrder", targetGroup.DisplayOrder + targetTreeCount)
                        );

                    sql = "Update [Group] Set DisplayOrder = DisplayOrder + @TargetTreeCount Where 1 <> 1 ";
                    foreach (var id in currTreeIDs)
                        sql += " Or GroupID = '{0}'".FormatWith(id.ToString());

                    await DbContext.Database.ExecuteSqlCommandAsync(sql
                        , new SqlParameter("TargetTreeCount", targetTreeCount)
                        );

                    dbContextTransaction.Commit();
                }

            }
            return true;
        }
        public async Task<bool> MoveAsync(Guid sourceGroupID, Guid targetGroupID, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState)
        {
            if (sourceGroupID == targetGroupID)
            {
                modelState.AddModelError("SourceGroupID", "源节点ID和目标节点ID不能相同");
                return false;
            }
            var sourceGroup = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == sourceGroupID);
            if (sourceGroup == null)
            {
                modelState.AddModelError("SourceGroupID", "源节点不存在");
                return false;
            }
            var targetGroup = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == targetGroupID);
            if (targetGroup == null)
            {
                modelState.AddModelError("TargetGroupID", "目标节点不存在");
                return false;
            }

            return await MoveAsync(sourceGroup, targetGroup, movingLocation, isChild, modelState);
        }
        public async Task<bool> MoveAsync(int sourceDisplayOrder, int targetDisplayOrder, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState)
        {
            if (sourceDisplayOrder == targetDisplayOrder)
            {
                modelState.AddModelError("SourceDisplayOrder", "源节点的DisplayOrder和目标节点的DisplayOrder不能相同");
                return false;
            }
            var sourceGroup = await DbContext.Groups.FirstOrDefaultAsync(m => m.DisplayOrder == sourceDisplayOrder);
            if (sourceGroup == null)
            {
                modelState.AddModelError("SourceDisplayOrder", "源节点不存在");
                return false;
            }
            var targetGroup = await DbContext.Groups.FirstOrDefaultAsync(m => m.DisplayOrder == targetDisplayOrder);
            if (targetGroup == null)
            {
                modelState.AddModelError("TargetDisplayOrder", "目标节点不存在");
                return false;
            }
            return await MoveAsync(sourceGroup, targetGroup, movingLocation, isChild, modelState);
        }
        private async Task<bool> MoveAsync(Group sourceGroup, Group targetGroup, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState)
        {
            #region 数据验证: 基本

            if (sourceGroup.DisplayOrder == targetGroup.DisplayOrder)
            {
                modelState.AddModelError("SourceGroupID", "源DisplayOrder和目标DisplayOrder不能相同".FormatWith(sourceGroup.Name, targetGroup.Name));
                return false;
            }
            // 不允许移动两个系统分组
            if (sourceGroup.DisplayOrder <= 2)
            {
                modelState.AddModelError("SourceGroupID", "不允许移动两个系统节点");
                return false;
            }
            // 不允许移动到两个系统分组之前
            if (movingLocation == MovingLocation.Above && targetGroup.DisplayOrder <= 2)
            {
                modelState.AddModelError("SourceGroupID", "不允许移动到两个系统节点之前");
                return false;
            }
            // 不允许移动到两个系统分组之间
            if (movingLocation == MovingLocation.Under && targetGroup.DisplayOrder == 1)
            {
                modelState.AddModelError("SourceGroupID", "不允许移动到两个系统节点之间");
                return false;
            }
            // 不允许移动到分组前面而作为子节点
            if (movingLocation == MovingLocation.Above && isChild.HasValue && isChild.Value)
            {
                modelState.AddModelError("SourceGroupID", "不允许移动到节点前面而作为子节点");
                return false;
            }

            #endregion

            // 获取当前节点的下一个兄弟节点或更高层下一个父节点（不是自己的父节点）的DisplayOrder
            int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(sourceGroup.DisplayOrder, sourceGroup.Level);
            // 当前节点树ID集合（包含本节点）
            var sourceTree = await GetTreeAsync(sourceGroup, displayOrderOfNextParentOrNextBrother, true);

            #region 数据验证: 贪吃蛇

            if (sourceTree.Any(m => m.GroupID == targetGroup.GroupID))
            {
                modelState.AddModelError("SourceGroupID", "源节点不能是目标节点的直接或间接父节点");
                return false;
            }

            #endregion

            #region 数据验证: 如果节点已经在指定的位置，则直接报错返回。

            if (movingLocation == MovingLocation.Above /*&& (!isChild.HasValue || !isChild.Value)*/ && sourceGroup.ParentID == targetGroup.ParentID && displayOrderOfNextParentOrNextBrother == targetGroup.DisplayOrder)
            {
                modelState.AddModelError("SourceGroupID", "源节点已经作为目标节点的上一个兄弟节点存在");
                return false;
            }
            if (movingLocation == MovingLocation.Under && (isChild.HasValue && isChild.Value) && sourceGroup.ParentID == targetGroup.GroupID && sourceGroup.DisplayOrder == targetGroup.DisplayOrder + 1)
            {
                modelState.AddModelError("SourceGroupID", "源节点已经作为目标节点的第一个节点存在");
                return false;
            }
            if (movingLocation == MovingLocation.Under && (!isChild.HasValue || !isChild.Value) && sourceGroup.ParentID == targetGroup.ParentID && sourceGroup.DisplayOrder == await GetDisplayOrderOfNextParentOrNextBrother(targetGroup))
            {
                modelState.AddModelError("SourceGroupID", "源节点已经作为目标节点的下一个兄弟节点存在");
                return false;
            }

            #endregion

            // 备注：
            // 1、节点移动到另一个节点之上，实际上就是作为兄弟节点。
            // 2、节点移动到另一个节点之下，如果是作为子节点，实际上就是目前编辑节点，选择另一个节点作为父节点的操作。
            // 3、isChild 影响 ParentID 和 Level。

            var maxDisplayOrderInSourceTree = sourceGroup.DisplayOrder + sourceTree.Count - 1;

            // 用于移动其他节点
            int moveTargetDisplayOrderMin;
            int moveTargetDisplayOrderMax;
            int moveTargetXDisplayOrder;

            int xDisplayOrder;
            int xLevel;
            Guid? sourceNewParentID = sourceNewParentID = isChild.HasValue && isChild.Value ? targetGroup.GroupID : targetGroup.ParentID;

            if (sourceGroup.ParentID == targetGroup.GroupID && movingLocation == MovingLocation.Under && (!isChild.HasValue || !isChild.Value))
            {
                // 平移算法：
                // 如果 sourceGroup 之前是 targetGroup 的子节点，转换为非子节点。targetGroup 的相关的子节点向上移动。sourceGroup 节点树向下移动。
                // 1、本节点树以下、目标在“本节点树”之后的节点的上移 DisplayOrder = DisplayOrder - 本节点树的节点数
                // 2、本节点树的 DisplayOrder = DisplayOrder + (1 中移动的节点数)
                // 3、本节点树的 Level = Level + xLevel
                // 4、本节点的 ParentID = sourceNewParentID

                moveTargetDisplayOrderMin = maxDisplayOrderInSourceTree + 1;
                moveTargetDisplayOrderMax = await GetMaxDisplayOrderInTree(targetGroup);
                moveTargetXDisplayOrder = -sourceTree.Count;

                var moveTargetNodeCount = moveTargetDisplayOrderMax - moveTargetDisplayOrderMin + 1;
                xDisplayOrder = moveTargetNodeCount;

                xLevel = targetGroup.Level - sourceGroup.Level; // xLevel 永远为 -1
            }
            else if (targetGroup.DisplayOrder - sourceGroup.DisplayOrder > 0)
            {
                // 下移算法： 
                // 1、本节点树以下、目标节点（包括或不包括）之上的节点的上移 DisplayOrder = DisplayOrder - 本节点树的节点数
                // 2、本节点树的 DisplayOrder = DisplayOrder + (1 中移动的节点数)
                // 3、本节点树的 Level = Level + xLevel
                // 4、本节点的 ParentID = sourceNewParentID

                moveTargetDisplayOrderMin = maxDisplayOrderInSourceTree + 1;
                moveTargetDisplayOrderMax = targetGroup.DisplayOrder - (movingLocation == MovingLocation.Above ? 1 : 0); // 放入目标节点下面，则包括目标节点本身
                moveTargetXDisplayOrder = -sourceTree.Count;

                var moveTargetNodeCount = moveTargetDisplayOrderMax - moveTargetDisplayOrderMin + 1;
                xDisplayOrder = moveTargetNodeCount;

                xLevel = targetGroup.Level - sourceGroup.Level + (isChild.HasValue && isChild.Value ? 1 : 0);
            }
            else
            {
                // 上移算法：
                // 1、本节点树以上、目标节点之下（包括或不包括）的节点的下移 DisplayOrder = DisplayOrder + 本节点树的节点数
                // 2、本节点树的 DisplayOrder = DisplayOrder + (1 中移动的节点数)
                // 3、本节点树的 Level = Level + xLevel
                // 4、本节点的 ParentID = sourceNewParentID

                moveTargetDisplayOrderMin = targetGroup.DisplayOrder + (movingLocation == MovingLocation.Above ? 0 : 1); // 放入目标节点上面，则包括目标节点本身
                moveTargetDisplayOrderMax = sourceGroup.DisplayOrder - 1;
                moveTargetXDisplayOrder = sourceTree.Count;

                var moveTargetNodeCount = moveTargetDisplayOrderMax - moveTargetDisplayOrderMin + 1;
                xDisplayOrder = -moveTargetNodeCount;

                xLevel = targetGroup.Level - sourceGroup.Level + (isChild.HasValue && isChild.Value ? 1 : 0);
            }

            #region 保存

            using (var dbContextTransaction = DbContext.Database.BeginTransaction())
            {
                var sql = "Update [Group] Set DisplayOrder = DisplayOrder + @MoveTargetXDisplayOrder Where DisplayOrder >= @MoveTargetDisplayOrderMin And DisplayOrder <= @MoveTargetDisplayOrderMax";
                await DbContext.Database.ExecuteSqlCommandAsync(sql
                    , new SqlParameter("MoveTargetXDisplayOrder", moveTargetXDisplayOrder)
                    , new SqlParameter("MoveTargetDisplayOrderMin", moveTargetDisplayOrderMin)
                    , new SqlParameter("MoveTargetDisplayOrderMax", moveTargetDisplayOrderMax)
                    );

                sourceTree.ForEach(m =>
                {
                    m.DisplayOrder += xDisplayOrder;
                    m.Level += xLevel;
                });

                sourceGroup.ParentID = sourceNewParentID;
                await DbContext.SaveChangesAsync();

                dbContextTransaction.Commit();
            }

            #endregion

            return true;
        }

        #endregion

        #region Private Methods

        private async Task<List<Guid>> GetTreeIDListAsync(Guid groupID, bool isIncludeSelf)
        {
            var group = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == groupID);
            if (group == null)
                return new List<Guid>(0);

            return await GetTreeIDListAsync(group, isIncludeSelf);
        }
        private async Task<List<Guid>> GetTreeIDListAsync(Group group, bool isIncludeSelf)
        {
            int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(group.DisplayOrder, group.Level);

            return await GetTreeIDListAsync(group, displayOrderOfNextParentOrNextBrother, isIncludeSelf);
        }
        private async Task<List<Guid>> GetTreeIDListAsync(Group group, int displayOrderOfNextParentOrNextBrother, bool isIncludeSelf)
        {
            List<Guid> list;
            if (displayOrderOfNextParentOrNextBrother == 0)
            {
                //说明当前节点是最后一个节点,直接获取
                list = await
                    DbContext.Groups.Where(m => m.DisplayOrder > group.DisplayOrder)
                        .Select(m => m.GroupID)
                        .ToListAsync();
            }
            else
            {
                list = await
                    DbContext.Groups.Where(m => m.DisplayOrder > group.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrother)
                        .Select(m => m.GroupID)
                        .ToListAsync();
            }

            if (isIncludeSelf)
            {
                list.Insert(0, group.GroupID);
            }

            return list;
        }
        private async Task<List<Group>> GetTreeAsync(Guid groupID, bool isIncludeSelf)
        {
            var group = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == groupID);
            if (group == null)
                return new List<Group>(0);

            return await GetTreeAsync(group, isIncludeSelf);
        }
        private async Task<List<Group>> GetTreeAsync(Group group, bool isIncludeSelf)
        {
            int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(group.DisplayOrder, group.Level);

            return await GetTreeAsync(group, displayOrderOfNextParentOrNextBrother, isIncludeSelf);
        }
        private async Task<List<Group>> GetTreeAsync(Group group, int displayOrderOfNextParentOrNextBrother, bool isIncludeSelf)
        {
            List<Group> list;
            if (displayOrderOfNextParentOrNextBrother != 0)
                list = await DbContext.Groups.Where(m => m.DisplayOrder > group.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrother)
                    .OrderBy(m => m.DisplayOrder)
                    .ToListAsync();
            else
                list = await DbContext.Groups.Where(m => m.DisplayOrder > group.DisplayOrder)
                    .OrderBy(m => m.DisplayOrder)
                    .ToListAsync();

            if (isIncludeSelf)
            {
                list.Insert(0, group);
            }

            return list;
        }
        private async Task<int> GetTreeNodeCountAsync(Guid groupID, bool isIncludeSelf)
        {
            var group = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == groupID);
            if (group == null)
                return 0;

            return await GetTreeNodeCountAsync(group, isIncludeSelf);
        }
        private async Task<int> GetTreeNodeCountAsync(Group group, bool isIncludeSelf)
        {
            int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(group.DisplayOrder, group.Level);

            int count;
            if (displayOrderOfNextParentOrNextBrother == 0)
            {
                //说明当前节点是最后一个节点,直接获取
                count = 0;
            }
            else
            {
                count = await DbContext.Groups.Where(m => m.DisplayOrder > group.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrother).
                    OrderByDescending(m => m.DisplayOrder).Select(m => m.DisplayOrder).FirstOrDefaultAsync();
            }

            if (isIncludeSelf)
            {
                count++;
            }

            return count;
        }
        private async Task<int> GetDisplayOrderOfNextParentOrNextBrother(Guid groupID)
        {
            var group = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == groupID);
            if (group == null)
                return 0;

            return await GetDisplayOrderOfNextParentOrNextBrother(group.DisplayOrder, group.Level);
        }
        private async Task<int> GetDisplayOrderOfNextParentOrNextBrother(Group group)
        {
            return await GetDisplayOrderOfNextParentOrNextBrother(group.DisplayOrder, group.Level);
        }
        private async Task<int> GetDisplayOrderOfNextParentOrNextBrother(int displayOrder, int level)
        {
            return await DbContext.Groups.Where(m => m.Level <= level && m.DisplayOrder > displayOrder)
                .OrderBy(m => m.DisplayOrder)
                .Select(m => m.DisplayOrder)
                .FirstOrDefaultAsync();
        }
        private async Task<int> GetMaxDisplayOrder()
        {
            return await DbContext.Groups.MaxAsync(m => (int?)m.DisplayOrder) ?? 0;
        }
        private async Task<int> GetMaxDisplayOrderInTree(Guid groupID)
        {
            var group = await DbContext.Groups.FirstOrDefaultAsync(m => m.GroupID == groupID);
            if (group == null)
                throw new NullReferenceException("节点不存在");

            return await GetMaxDisplayOrderInTree(group);
        }
        private async Task<int> GetMaxDisplayOrderInTree(Group group)
        {
            int maxDisplayOrder;

            // 获取父节点之下的兄弟节点或更高层次的父节点(不是自己的父节点)的DisplayOrder
            int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(group);

            if (displayOrderOfNextParentOrNextBrother == 0)
                maxDisplayOrder = await DbContext.Groups.Where(m => m.DisplayOrder > group.DisplayOrder || m.GroupID == group.GroupID).MaxAsync(m => m.DisplayOrder);
            else
                maxDisplayOrder = displayOrderOfNextParentOrNextBrother - 1;

            return maxDisplayOrder;
        }

        #endregion

    }
}
