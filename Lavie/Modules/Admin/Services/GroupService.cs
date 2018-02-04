using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Extensions.Object;
using Lavie.Infrastructure;
using Lavie.Models;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;
using R = Lavie.Modules.Admin.Repositories;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Admin.Services
{
    public interface IGroupService
    {
        Task<Group> GetItemAsync(Guid groupID);
        Task<Group> GetItemAsync(string name);
        Task<List<Group>> GetListAsync(Guid? parentID = null);
        Task<List<GroupBase>> GetBasePathAsync(Guid groupID);
        Task<List<GroupInfo>> GetInfoPathAsync(Guid groupID);
        Task<bool> SaveAsync(GroupInput groupInput, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(Guid groupID, ModelStateDictionary modelState);
        Task<bool> MoveAsync(Guid groupID, MovingTarget movingTarget);
        Task<bool> MoveAsync(Guid sourceGroupID, Guid targetGroupID, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState);
        Task<bool> MoveAsync(int sourceDisplayOrder, int targetDisplayOrder, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState);
    }
    public class GroupService : IGroupService
    {
        private readonly R.IGroupRepository _repository;
        private readonly ICacheModule _cache;
        private const string GroupListCacheKey = "GroupList";

        #region IGroupService Members

        public GroupService(R.IGroupRepository repository, IModuleRegistry moduleRegistry)
        {
            this._repository = repository;
            this._cache = moduleRegistry.GetModules<ICacheModule>().Last();
        }

        public async Task<Group> GetItemAsync(Guid groupID)
        {
            return await _repository.GetItemAsync(groupID);
        }

        public async Task<Group> GetItemAsync(string name)
        {
            return await _repository.GetItemAsync(name);
        }

        public async Task<List<Group>> GetListAsync(Guid? parentID = null)
        {
            List<Group> groups = await GetListFromCacheAsync();
            if (!groups.IsNullOrEmpty())
                return CloneTree(groups, parentID);
            else
                return await _repository.GetListAsync(parentID);
        }
        public async Task<List<GroupBase>> GetBasePathAsync(Guid groupID)
        {
            List<Group> groups = await GetListFromCacheAsync();
            if (!groups.IsNullOrEmpty())
                return CloneBasePath(groups, groupID);
            else
                return await _repository.GetBasePathAsync(groupID);
        }

        public async Task<List<GroupInfo>> GetInfoPathAsync(Guid groupID)
        {
            List<Group> groups = await GetListFromCacheAsync();
            if (!groups.IsNullOrEmpty())
                return CloneInfoPath(groups, groupID);
            else
                return await _repository.GetInfoPathAsync(groupID);
        }

        public async Task<bool> SaveAsync(GroupInput groupInput, ModelStateDictionary modelState)
        {
            Guard.ArgumentNotNull(groupInput, "groupInput");
            Guard.ArgumentNotNull(modelState, "modelState");

            if (!await ValidateExists(groupInput, modelState)) return false;

            bool result = await _repository.SaveAsync(groupInput, modelState);

            if (result)
            {
                _cache.Invalidate(GroupListCacheKey);
            }

            return result;
        }

        public async Task<bool> RemoveAsync(Guid groupID, ModelStateDictionary modelState)
        {
            bool result = await _repository.RemoveAsync(groupID, modelState);
            if (result)
                _cache.Invalidate(GroupListCacheKey);
            return result;
        }

        public async Task<bool> MoveAsync(Guid groupID, MovingTarget movingTarget)
        {
            bool result = await _repository.MoveAsync(groupID, movingTarget);
            if (result)
                _cache.Invalidate(GroupListCacheKey);
            return result;
        }

        public async Task<bool> MoveAsync(Guid sourceGroupID, Guid targetGroupID, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState)
        {
            bool result = await _repository.MoveAsync(sourceGroupID, targetGroupID, movingLocation, isChild, modelState);
            if (result)
                _cache.Invalidate(GroupListCacheKey);
            return result;
        }

        public async Task<bool> MoveAsync(int sourceDisplayOrder, int targetDisplayOrder, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState)
        {
            bool result = await _repository.MoveAsync(sourceDisplayOrder, targetDisplayOrder, movingLocation, isChild, modelState);
            if (result)
                _cache.Invalidate(GroupListCacheKey);
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 验证用户组名称是否已经被使用
        /// </summary>
        private async Task<bool> ValidateExists(GroupInput groupInput, ModelStateDictionary modelState)
        {
            Group foundGroup = await _repository.GetItemAsync(groupInput.Name);

            if (foundGroup != null && groupInput.GroupID != foundGroup.GroupID)
            {
                modelState.AddModelError("Name","用户组名称【" + groupInput.Name + "】已经被使用");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 克隆列表
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private List<Group> CloneTree(List<Group> source, Guid? parentID = null)
        {
            // 算法正确性的前提是记录是按正确顺序排序的

            if (source.IsNullOrEmpty())
                return new List<Group>(0);

            if (!parentID.HasValue)
            {
                //深度克隆
                return source.DeepClone<List<Group>>();
            }

            var list = new List<Group>();
            for (var index = 0; index < source.Count; index++)
            {
                var item = source[index];
                if (list.Count == 0)
                {
                    if (item.GroupID == parentID.Value)
                        list.Add(item);
                }
                else
                {
                    if (item.ParentID == parentID.Value)
                    {
                        list.Add(item);
                        AddChild(source, list, item.GroupID, index);
                    }
                }
            }
            if(list.Count == 0)
                return new List<Group>(0);

            return list.DeepClone<List<Group>>();
        }

        private void AddChild(List<Group> source, List<Group> target, Guid parentID, int index)
        {
            // 算法正确性的前提是记录是按正确顺序排序的
            // index 的作用是减少遍历开销

            for (var i = index; i < source.Count; i++)
            {
                var item = source[i];
                if (item.ParentID == parentID)
                {
                    target.Add(item);
                    AddChild(source, target, item.GroupID, i);
                }
            }
        }

        private List<GroupBase> CloneBasePath(List<Group> source, Guid groupID)
        {
            // 算法正确性的前提是记录是按正确顺序排序的

            if (source.IsNullOrEmpty())
                return new List<GroupBase>(0);

            var list = GeneratePath(source, groupID);

            var baseList = list.Select(m => new GroupBase
            {
                GroupID = m.GroupID,
                Name = m.Name,
                IsIncludeUser = m.IsIncludeUser,
                IsDisabled = m.IsDisabled,
                IsSystem = m.IsSystem,
            }).ToList();

            return baseList;
        }

        private List<GroupInfo> CloneInfoPath(List<Group> source, Guid groupID)
        {
            // 算法正确性的前提是记录是按正确顺序排序的

            if (source.IsNullOrEmpty())
                return new List<GroupInfo>(0);

            var list = GeneratePath(source, groupID);

            var infoList = list.Select(m => new GroupInfo
            {
                GroupID = m.GroupID,
                Name = m.Name,
            }).ToList();

            return infoList;
        }

        private List<Group> GeneratePath(List<Group> source, Guid groupID)
        {
            var list = new List<Group>();
            Group item = null;
            int index = -1;
            for (var i = 0; i < source.Count; i++)
            {
                if (source[i].GroupID == groupID)
                {
                    index = i;
                    item = source[i];
                    break;
                }
            }

            if (item == null)
                return null;

            list.Add(item);

            if (item.ParentID.HasValue)
            {
                AddParent(source, list, item.ParentID.Value, index);
            }

            return list;
        }

        private void AddParent(List<Group> source, List<Group> target, Guid parentID, int index)
        {
            // 算法正确性的前提是记录是按正确顺序排序的
            // index 的作用是减少遍历开销

            for (var i = index - 1; i >= 0; i--)
            {
                var item = source[i];
                if (item.GroupID == parentID)
                {
                    target.Insert(0, item);
                    if (item.ParentID.HasValue)
                    {
                        AddParent(source, target, item.ParentID.Value, i);
                    }
                    break;
                }
            }
        }

        private async Task<List<Group>> GetListFromCacheAsync()
        {
            if (_cache != null)
                return await _cache.GetItemAsync<List<Group>>(
                    GroupListCacheKey,
                    async () => await _repository.GetListAsync(),
                    TimeSpan.FromDays(1)
                    );
            else
                return null;
        }

        #endregion

    }
}
