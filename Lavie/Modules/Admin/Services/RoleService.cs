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
    public interface IRoleService
    {
        Task<Role> GetItemAsync(Guid roleID);
        Task<Role> GetItemAsync(string name);
        Task<List<RoleBase>> GetBaseListAsync();
        Task<List<Role>> GetListAsync();
        Task<Role> SaveAsync(RoleInput roleInput, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(Guid roleID, ModelStateDictionary modelState);
        Task<bool> EditNameAsync(SaveRoleNameInput roleEditNameInput, ModelStateDictionary modelState);
        Task<bool> MoveAsync(Guid roleID, MovingTarget target);
        Task<bool> MoveAsync(int sourceDisplayOrder, int targetDisplayOrder, ModelStateDictionary modelState);
        Task<bool> MoveAsync(Guid sourceRoleID, Guid targetRoleID, ModelStateDictionary modelState);
    }
    public class RoleService : IRoleService
    {
        private readonly R.IRoleRepository _repository;
        private readonly ICacheModule _cache;
        private const string RoleListCacheKey = "RoleList";

        public RoleService(R.IRoleRepository repository, IModuleRegistry moduleRegistry)
        {
            this._repository = repository;
            this._cache = moduleRegistry.GetModules<ICacheModule>().Last();
        }

        #region IRoleService Members

        public async Task<Role> GetItemAsync(Guid roleID)
        {
            return await _repository.GetItemAsync(roleID);
        }

        public async Task<Role> GetItemAsync(string name)
        {
            return await _repository.GetItemAsync(name);
        }

        public async Task<List<RoleBase>> GetBaseListAsync()
        {
            if (_cache != null)
            {
                // 注意：缓存的是 Role 列表而非 RoleBase 列表
                var roles = await _cache.GetItemAsync<List<Role>>(
                    RoleListCacheKey, 
                    async () => await _repository.GetListAsync(),
                    TimeSpan.FromDays(1)
                    );
                return CloneRoleBases(roles);
            }
            else
            { 
                return await _repository.GetBaseListAsync();
            }
        }
        public async Task<List<Role>> GetListAsync()
        {
            if (_cache != null)
            {
                var roles = await _cache.GetItemAsync<List<Role>>(
                    RoleListCacheKey,
                    async () => await _repository.GetListAsync(),
                    TimeSpan.FromDays(1)
                    );
                return CloneRoles(roles);
            }
            else
            {
                return await _repository.GetListAsync();
            }
        }

        public async Task<Role> SaveAsync(RoleInput roleInput, ModelStateDictionary modelState)
        {
            Guard.ArgumentNotNull(roleInput, "roleInput");
            Guard.ArgumentNotNull(modelState, "modelState");

            if (!await ValidateExists(roleInput, modelState)) return null;
            var result = await _repository.SaveAsync(roleInput, modelState);
            if (result == null)
                modelState.AddModelError("Name","添加或编辑时保存失败");
            else
                _cache.Invalidate(RoleListCacheKey);

            return result;
        }

        public async Task<bool> RemoveAsync(Guid roleID, ModelStateDictionary modelState)
        {
            bool result = await _repository.RemoveAsync(roleID, modelState);
            if (result)
                _cache.Invalidate(RoleListCacheKey);
            return result;
        }

        public async Task<bool> EditNameAsync(SaveRoleNameInput saveRoleNameInput, ModelStateDictionary modelState)
        {
            bool result = await _repository.SaveNameAsync(saveRoleNameInput, modelState);
            if (result)
                _cache.Invalidate(RoleListCacheKey);
            return result;
        }

        public async Task<bool> MoveAsync(Guid roleID, MovingTarget target)
        {
            bool result = await _repository.MoveAsync(roleID, target);
            if(result)
                _cache.Invalidate(RoleListCacheKey);
            return result;
        }

        public async Task<bool> MoveAsync(int sourceDisplayOrder, int targetDisplayOrder, ModelStateDictionary modelState)
        {
            bool result = await _repository.MoveAsync(sourceDisplayOrder, targetDisplayOrder, modelState);
            if (result)
                _cache.Invalidate(RoleListCacheKey);
            return result;
        }

        public async Task<bool> MoveAsync(Guid sourceRoleID, Guid targetRoleID, ModelStateDictionary modelState)
        {
            bool result = await _repository.MoveAsync(sourceRoleID, targetRoleID, modelState);
            if (result)
                _cache.Invalidate(RoleListCacheKey);
            return result;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 验证角色名称是否已经被使用
        /// </summary>
        private async Task<bool> ValidateExists(RoleInput roleInput, ModelStateDictionary modelState)
        {
            Role foundRole = await _repository.GetItemAsync(roleInput.Name);

            if (foundRole != null && roleInput.RoleID != foundRole.RoleID)
            {
                modelState.AddModelError("Name", "角色名称【" + roleInput.Name + "】已经被使用");
                return false;
            }
            return true;
        }

        private List<RoleBase> CloneRoles(IEnumerable<RoleBase> source)
        {
            if (source.IsNullOrEmpty())
                return new List<RoleBase>(0);

            //深度克隆
            var clone = source.DeepClone() as IEnumerable<RoleBase>;
            if (clone == null) return new List<RoleBase>(0);
            return clone.ToList();

            //List<RoleBase> roles = new List<RoleBase>();
            //foreach (var item in source)
            //{
            //    roles.Add(new RoleBase
            //    {
            //        RoleID = item.RoleID,
            //        Name = item.Name,
            //        IsSystem = item.IsSystem,
            //        DisplayOrder = item.DisplayOrder
            //    });
            //}
            //return roles;
        }
        private List<Role> CloneRoles(IEnumerable<Role> source)
        {
            if (source.IsNullOrEmpty())
                return new List<Role>(0);

            //深度克隆
            var clone = source.DeepClone() as IEnumerable<Role>;
            if (clone == null) return new List<Role>(0);
            return clone.ToList();
        }

        private List<RoleBase> CloneRoleBases(IEnumerable<Role> source)
        {
            if (source.IsNullOrEmpty())
                return new List<RoleBase>(0);

            List<RoleBase> roles = new List<RoleBase>();
            foreach (var item in source)
            {
                roles.Add(new RoleBase
                {
                    RoleID = item.RoleID,
                    Name = item.Name,
                    IsSystem = item.IsSystem,
                    DisplayOrder = item.DisplayOrder
                });
            }
            return roles;
        }

        #endregion
    }
}
