﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lavie.Extensions;
using Lavie.Extensions.Object;
using Lavie.Models;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Modules.Admin.Repositories;
using XM = Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Repositories
{
    public interface IPermissionRepository
    {
        Task<XM.Permission> GetItemAsync(Guid permissionID);
        Task<XM.Permission> GetItemAsync(string name);
        Task<List<XM.Permission>> GetListAsync(Guid? parentID = null);
        Task<bool> SaveAsync(PermissionInput permissionInput);
        Task<bool> RemoveAsync(Guid permissionID);
        Task<bool> MoveAsync(Guid permissionID, MovingTarget target);
    }

    public class PermissionRepository : RepositoryBase, IPermissionRepository
    {

        public async Task<XM.Permission> GetItemAsync(Guid permissionID)
        {
            var item = await DbContext.Permissions.FirstOrDefaultAsync(m => m.PermissionID == permissionID);
            return item.ToModel<XM.Permission>();
        }

        public async Task<XM.Permission> GetItemAsync(string name)
        {
            var item = await DbContext.Permissions.FirstOrDefaultAsync(m => m.Name == name);
            return item.ToModel<XM.Permission>();
        }

        public async Task<List<XM.Permission>> GetListAsync(Guid? parentID = null)
        {
            //Func<Permission, XM.Permission> selector
            //    = ((Expression<Func<Permission, XM.Permission>>)(m => m.ToModel<XM.Permission>())).Compile();
            //Expression<Func<Permission, XM.Permission>> selector
            //    = m => m.ToModel<XM.Permission>();


            Expression<Func<Permission, XM.Permission>> selector = m => new XM.Permission
            {
                ParentID = m.ParentID,
                PermissionID = m.PermissionID,
                Name = m.Name,
                ModuleName = m.ModuleName,
                Level = m.Level,
                DisplayOrder = m.DisplayOrder,
            };

            if (parentID.HasValue)
            {
                var parent = await DbContext.Permissions.FirstOrDefaultAsync(m => m.PermissionID == parentID.Value);
                if (parent == null)
                    return new List<XM.Permission>();
                else
                {
                    int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(parent.DisplayOrder, parent.Level);
                    if (displayOrderOfNextParentOrNextBrother != 0)
                        return await DbContext.Permissions.Where(m => m.DisplayOrder >= parent.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrother)
                            .OrderBy(m => m.DisplayOrder)
                            .Select(selector)
                            .AsNoTracking()
                            .ToListAsync();
                    else
                        return await DbContext.Permissions.Where(m => m.DisplayOrder >= parent.DisplayOrder)
                            .OrderBy(m => m.DisplayOrder)
                            .Select(selector)
                            .AsNoTracking()
                            .ToListAsync();
                }
            }
            else
            {
                return await DbContext.Permissions
                    .OrderBy(m => m.DisplayOrder)
                    .Select(selector)
                    .ToListAsync();
            }
        }

        public async Task<bool> SaveAsync(PermissionInput permissionInput)
        {
            string sql;

            Permission permissionToSave = null;
            if (!permissionInput.PermissionID.IsNullOrEmpty())
            {
                permissionToSave = await DbContext.Permissions.FirstOrDefaultAsync(m => m.PermissionID == permissionInput.PermissionID.Value);

                if (permissionInput.PermissionID == permissionInput.ParentID)
                {
                    // modelState.AddModelError("PermissionID", "尝试将自身作为父节点");
                    return false;
                }
            }
            //添加操作
            if (permissionToSave == null)
            {
                #region 添加操作
                //创建要保存的对象
                permissionToSave = new Permission
                {
                    //提取权限时，permissionToSave的PermissionID为null，这时不用创建新ID
                    PermissionID = permissionInput.PermissionID.IsNullOrEmpty()?Guid.NewGuid():permissionInput.PermissionID.Value,
                    ParentID = permissionInput.ParentID,
                    ModuleName = permissionInput.ModuleName,
                    Name = permissionInput.Name,
                };
                DbContext.Permissions.Add(permissionToSave);
                //如果添加的是新的顶级节点,直接添加到末尾，不会影响其他节点
                if (permissionInput.ParentID.IsNullOrEmpty())
                {
                    permissionToSave.DisplayOrder = await GetMaxDisplayOrder() + 1;
                    permissionToSave.Level = 1;
                }
                else//如果添加的是子节点，会影响其他节点的DisplayOrder
                {
                    //父节点树的最大DisplayerOrder
                    int maxDisplayOrderInParentTree = await GetMaxDisplayOrderInParentTree(permissionInput.ParentID.Value);
                    //父节点树的最大DisplayerOrder基础上加1作为保存对象的DisplayOrder
                    permissionToSave.DisplayOrder = maxDisplayOrderInParentTree + 1;
                    //父节点的Level基础上加1作为保存对象的Level
                    permissionToSave.Level = await GetLevel(permissionInput.ParentID.Value) + 1;

                    //父节点树之后的所有节点的DisplayOrder加1
                    sql = "Update Permission Set DisplayOrder=DisplayOrder+1 Where DisplayOrder > @DisplayOrder";
                    await DbContext.Database.ExecuteSqlCommandAsync(sql, new SqlParameter("DisplayOrder",maxDisplayOrderInParentTree));
                }
                #endregion
            }
            else//编辑操作
            {
                permissionToSave.ModuleName = permissionInput.ModuleName;
                permissionToSave.Name = permissionInput.Name;

                //如果父节点不改变，则仅仅保存数据就行了。下面处理的是父节点改变了的情况
                //如果父节点改变(从无父节点到有父节点，从有父节点到无父节点，从一个父节点到另一个父节点)
                if (permissionInput.ParentID != permissionToSave.ParentID)
                {
                    permissionToSave.ParentID = permissionInput.ParentID;

                    //获取当前节点的下一个兄弟节点或更高层下一个父节点（不是自己的父节点）的DisplayOrder
                    int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(permissionToSave.DisplayOrder, permissionToSave.Level);

                    #region 当前节点树ID集合

                    List<Guid> currTreeIDs;
                    if (displayOrderOfNextParentOrNextBrother == 0)
                    {
                        //说明当前节点是最后一个节点,直接获取
                        currTreeIDs = await DbContext.Permissions.Where(m => m.DisplayOrder >= permissionToSave.DisplayOrder).Select(m=>m.PermissionID).ToListAsync();
                    }
                    else
                    {
                        currTreeIDs = await DbContext.Permissions
                            .Where(m => m.DisplayOrder >= permissionToSave.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrother)
                            .Select(m => m.PermissionID).ToListAsync();

                    }
                    int currentTreeItemCount = currTreeIDs.Count;

                    #endregion

                    //当前节点将由子节点升为顶级节点，直接将该节点数移到最后
                    if (!permissionToSave.ParentID.HasValue)
                    {
                        #region 将由子节点升为顶级节点

                        //需要提升的层级数
                        int xLevel = permissionToSave.Level - 1;

                        //当前节点树之后已无任何节点
                        if (displayOrderOfNextParentOrNextBrother == 0)
                        {
                            //将当前节点树的所有节点的Level都进行提升
                            sql = "Update Permission Set Level = Level - @Level Where DisplayOrder>=@DisplayOrder";
                            await DbContext.Database.ExecuteSqlCommandAsync(sql
                                , new SqlParameter("Level", xLevel)
                                , new SqlParameter("DisplayOrder", permissionToSave.DisplayOrder)
                                );
                        }
                        else//当前节点树之后还有节点，应该将这些节点的向前面排，并且将当前节点树的所有节点往后排
                        {
                            //当前节点树之后的节点数量
                            int nextItemCount = DbContext.Permissions.Count(m => m.DisplayOrder >= displayOrderOfNextParentOrNextBrother);

                            sql = "Update Permission Set DisplayOrder = DisplayOrder - @CTIC Where DisplayOrder>=@DOONPONB";

                            await DbContext.Database.ExecuteSqlCommandAsync(sql
                                , new SqlParameter("CTIC", currentTreeItemCount)
                                , new SqlParameter("DOONPONB", displayOrderOfNextParentOrNextBrother)
                                );

                            sql = "Update Permission Set Level = Level - @Level,DisplayOrder = DisplayOrder + @NextItemCount Where 1<>1 ";
                            foreach (var id in currTreeIDs)
                                sql += " Or PermissionID = '{0}'".FormatWith(id.ToString());
                         
                            await DbContext.Database.ExecuteSqlCommandAsync(sql
                                , new SqlParameter("Level", xLevel)
                                , new SqlParameter("NextItemCount", nextItemCount)
                                );

                        }

                        #endregion
                    }
                    else//当前节点将改变父节点，包括从顶级节点移至另一节点下，或从当前父节点下移至另一节点下
                    {
                        #region 从顶级节点移至另一节点下，或从当前父节点下移至另一节点下

                        //目标父节点
                        var tarParent = await DbContext.Permissions.FirstOrDefaultAsync(m => m.PermissionID == permissionInput.ParentID.Value);

                        int xDisplayOrder = permissionToSave.DisplayOrder - tarParent.DisplayOrder;
                        int xLevel = permissionToSave.Level - tarParent.Level;

                        if (xDisplayOrder > 0)//从下往上移
                        {
                            #region 从下往上移
                            //特例处理，当前节点要移至的父节点就是上一个节点，只需要改变当前树Level
                            if (xDisplayOrder == 1)
                            {
                                sql = "Update Permission Set Level = Level - @Level Where 1<>1 ";
                                foreach (var id in currTreeIDs)
                                    sql += " Or PermissionID = '{0}'".FormatWith(id.ToString());

                                await DbContext.Database.ExecuteSqlCommandAsync(sql
                                    , new SqlParameter("Level", xLevel-1)
                                    );
                            }
                            else
                            {
                                //新的父节点和本节点之间的节点往下移动，DisplayOrder增加
                                sql = "Update Permission Set DisplayOrder=DisplayOrder+@CurTreeCount Where DisplayOrder>@TDisplayOrder And DisplayOrder<@CDisplayOrder";
                                await DbContext.Database.ExecuteSqlCommandAsync(sql
                                    , new SqlParameter("CurTreeCount", currentTreeItemCount)
                                    , new SqlParameter("TDisplayOrder", tarParent.DisplayOrder)
                                    , new SqlParameter("CDisplayOrder", permissionToSave.DisplayOrder)
                                    );

                                sql = "Update Permission Set DisplayOrder = DisplayOrder-@XCount,Level = Level - @Level Where 1<>1 ";
                                foreach (var id in currTreeIDs)
                                    sql += " Or PermissionID = '{0}'".FormatWith(id.ToString());
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
                                return false;

                            // 更新本节点树至新的父节点（包括新的父节点）之间的节点的DisplayOrder
                            sql = "Update Permission Set DisplayOrder=DisplayOrder-@CurTreeCount Where DisplayOrder>=@DOONPONB And DisplayOrder<=@TDisplayOrder";
                            await DbContext.Database.ExecuteSqlCommandAsync(sql
                                , new SqlParameter("CurTreeCount", currentTreeItemCount)
                                , new SqlParameter("DOONPONB", displayOrderOfNextParentOrNextBrother)
                                , new SqlParameter("TDisplayOrder", tarParent.DisplayOrder)
                                );

                            // 本节点至新的节点之间的节点数
                            int nextItemCount = tarParent.DisplayOrder - displayOrderOfNextParentOrNextBrother + 1;
                            sql = "Update Permission Set DisplayOrder = DisplayOrder+ @XCount,Level = Level - @Level Where 1<>1 ";
                            foreach (var id in currTreeIDs)
                                sql += " Or PermissionID = '{0}'".FormatWith(id.ToString());
                            await DbContext.Database.ExecuteSqlCommandAsync(sql
                                , new SqlParameter("XCount", nextItemCount)
                                , new SqlParameter("Level", xLevel - 1)
                                );

                            #endregion
                        }

                        #endregion
                    }
                }
            }
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAsync(Guid permissionID)
        {
            //移除无限级分类步骤：

            //1、获取预删节点信息
            Permission permissionToRemove = await DbContext.Permissions.FirstOrDefaultAsync(m => m.PermissionID == permissionID);

            //当然，如果无法获取节点，属于无效操作
            if (permissionToRemove == null) return false;

            //2、节点包含子节点不允许删除
            if (DbContext.Permissions.Any(m => m.ParentID == permissionID))
                return false;

            //3、更新DisplayOrder大于预删节点DisplayOrder的节点
            string sql = "Update Permission Set DisplayOrder=DisplayOrder-1 Where DisplayOrder>@DisplayOrder";
            await DbContext.Database.ExecuteSqlCommandAsync(sql,
                new SqlParameter("DisplayOrder", permissionToRemove.DisplayOrder)
                );

            //4、删除关联节点
            sql = "Delete RolePermissionRelationship Where PermissionID=@PermissionID";
            await DbContext.Database.ExecuteSqlCommandAsync(sql,
                new SqlParameter("PermissionID", permissionID)
                );
            sql = "Delete UserPermissionRelationship Where PermissionID=@PermissionID";
            await DbContext.Database.ExecuteSqlCommandAsync(sql,
                new SqlParameter("PermissionID", permissionID)
                );

            //5、删除节点
            DbContext.Permissions.Remove(permissionToRemove);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveAsync(Guid permissionID, MovingTarget target)
        {
            string sql;

            var permissionToMove = DbContext.Permissions.FirstOrDefault(m => m.PermissionID == permissionID);
            if (permissionToMove == null) return false;

            #region 获取当前节点树

            List<Guid> currTreeIDs;
            int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(permissionToMove.DisplayOrder, permissionToMove.Level);
            if (displayOrderOfNextParentOrNextBrother == 0)
            {
                // 无兄弟节点
                currTreeIDs = await DbContext.Permissions.Where(m => m.DisplayOrder >= permissionToMove.DisplayOrder).Select(m=>m.PermissionID).ToListAsync();
            }
            else
            {
                // 有兄弟节点
                currTreeIDs = await DbContext.Permissions
                    .Where(m => m.DisplayOrder >= permissionToMove.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrother)
                    .Select(m => m.PermissionID)
                    .ToListAsync();

            }
            // 目标节点树的总数为目标自己+目标所有子孙的总和
            int curTreeCount = currTreeIDs.Count;

            #endregion

            if (MovingTarget.Up == target)
            {
                #region 获取上一个兄弟节点

                Permission targetPermission;
                if (permissionToMove.ParentID.HasValue)
                    targetPermission = await DbContext.Permissions
                        .OrderByDescending(m=>m.DisplayOrder)
                        .FirstOrDefaultAsync(m => 
                    m.ParentID == permissionToMove.ParentID && m.DisplayOrder < permissionToMove.DisplayOrder);
                else
                    targetPermission = await DbContext.Permissions
                        .OrderByDescending(m => m.DisplayOrder)
                        .FirstOrDefaultAsync(m =>
                    m.ParentID == null && m.DisplayOrder < permissionToMove.DisplayOrder);
                #endregion

                if (targetPermission == null) return false;

                //获取兄弟节点树的节点数
                int targetTreeCount = await DbContext.Permissions.CountAsync(m => 
                    m.DisplayOrder >= targetPermission.DisplayOrder 
                    && m.DisplayOrder < permissionToMove.DisplayOrder);

                //更新兄弟节点树的DisplayOrder
                sql = "Update Permission Set DisplayOrder = DisplayOrder+@CurTreeCount Where DisplayOrder>=@TDisplayOrder And DisplayOrder<@CDisplayOrder";
                await DbContext.Database.ExecuteSqlCommandAsync(sql
                    , new SqlParameter("CurTreeCount", curTreeCount)
                    , new SqlParameter("TDisplayOrder", targetPermission.DisplayOrder)
                    , new SqlParameter("CDisplayOrder", permissionToMove.DisplayOrder)
                    );

                sql = "Update Permission Set DisplayOrder = DisplayOrder-@TargetTreeCount Where 1<>1 ";
                foreach (var id in currTreeIDs)
                    sql += " Or PermissionID = '{0}'".FormatWith(id.ToString());
                await DbContext.Database.ExecuteSqlCommandAsync(sql
                    , new SqlParameter("TargetTreeCount", targetTreeCount)
                    );

            }
            else
            {
                #region 获取下一个兄弟节点
                Permission nextBrotherPermission ;
                if (permissionToMove.ParentID.HasValue)
                    nextBrotherPermission = await DbContext.Permissions.OrderBy(m => m.DisplayOrder).FirstOrDefaultAsync(m =>
                    m.ParentID == permissionToMove.ParentID && m.DisplayOrder > permissionToMove.DisplayOrder);
                else
                    nextBrotherPermission = await DbContext.Permissions.OrderBy(m => m.DisplayOrder).FirstOrDefaultAsync(m =>
                    m.ParentID == null && m.DisplayOrder > permissionToMove.DisplayOrder);
                #endregion

                if (nextBrotherPermission == null) return false;

                #region 获取兄弟节点树的节点数
                int displayOrderOfNextParentOrNextBrotherOfBrother = await GetDisplayOrderOfNextParentOrNextBrother(nextBrotherPermission.DisplayOrder, nextBrotherPermission.Level);
                int nextBrotherTreeCount;
                if (displayOrderOfNextParentOrNextBrotherOfBrother == 0)
                    nextBrotherTreeCount = await DbContext.Permissions.CountAsync(m => m.DisplayOrder >= nextBrotherPermission.DisplayOrder);
                else
                    nextBrotherTreeCount = await DbContext.Permissions.CountAsync(m => m.DisplayOrder >= nextBrotherPermission.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrotherOfBrother);
                #endregion

                //更新兄弟节点树的DisplayOrder
                sql = "Update Permission Set DisplayOrder = DisplayOrder-@CurTreeCount Where DisplayOrder>=@DisplayOrder And DisplayOrder<@TDisplayOrder";

                await DbContext.Database.ExecuteSqlCommandAsync(sql
                    , new SqlParameter("CurTreeCount", curTreeCount)
                    , new SqlParameter("DisplayOrder", nextBrotherPermission.DisplayOrder)
                    , new SqlParameter("TDisplayOrder", nextBrotherPermission.DisplayOrder + nextBrotherTreeCount)
                    );

                sql = "Update Permission Set DisplayOrder = DisplayOrder+@NextBrotherTreeCount Where 1<>1 ";
                foreach (var id in currTreeIDs)
                    sql += " Or PermissionID = '{0}'".FormatWith(id.ToString());

                await DbContext.Database.ExecuteSqlCommandAsync(sql
                    , new SqlParameter("NextBrotherTreeCount", nextBrotherTreeCount)
                    );

            }
            return true;
        }

        #region Private Methods

        private async Task<int> GetDisplayOrderOfNextParentOrNextBrother(int displayOrder, int permissionLevel)
        {
            return await DbContext.Permissions.Where(m => m.Level <= permissionLevel && m.DisplayOrder > displayOrder)
                .OrderBy(m => m.DisplayOrder)
                .Select(m => m.DisplayOrder)
                .FirstOrDefaultAsync();
        }
        private async Task<int> GetMaxDisplayOrder()
        {
            return await DbContext.Permissions.MaxAsync(m => (int?)m.DisplayOrder) ?? 0;
        }
        private async Task<int> GetMaxDisplayOrderInParentTree(Guid parentID)
        {
            int maxDisplayOrder;
            var parent = await DbContext.Permissions.FirstOrDefaultAsync(m => m.PermissionID == parentID);
            if (parent == null)
                throw new NullReferenceException("或许尝试将节点加到不存在的父节点之上");

            //获取父节点之下的兄弟节点或更高层次的父节点(不是自己的父节点)的DisplayOrder
            int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(parent.DisplayOrder, parent.Level);

            if (displayOrderOfNextParentOrNextBrother == 0)
                maxDisplayOrder = await DbContext.Permissions.Where(m=>m.DisplayOrder>parent.DisplayOrder||m.PermissionID==parent.PermissionID).MaxAsync(m => m.DisplayOrder);
            else
                maxDisplayOrder = displayOrderOfNextParentOrNextBrother - 1;

            return maxDisplayOrder;

        }
        private async Task<int> GetLevel(Guid pessmissionID)
        {
            return await DbContext.Permissions.Where(m => m.PermissionID == pessmissionID).Select(m => m.Level).FirstOrDefaultAsync();
        }

        #endregion
    }
}
