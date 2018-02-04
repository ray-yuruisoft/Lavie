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
    public interface IPermissionService
    {
        Task<Permission> GetItemAsync(Guid permissionID);
        Task<Permission> GetItemAsync(string name);
        Task<List<Permission>> GetListAsync();
        Task<bool> SaveAsync(PermissionInput permissionInput, ModelStateDictionary modelState);
        Task<bool> SaveAsync(IEnumerable<PermissionInput> permissions, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(Guid permissionID);
        Task<bool> RemoveAsync(IEnumerable<Guid> ids);
        Task<bool> MoveAsync(Guid permissionID, MovingTarget target);
    }

    public class PermissionService : IPermissionService
    {
        private readonly R.IPermissionRepository _repository;
        private readonly ICacheModule _cache;
        private const string PermissionListCacheKey = "PermissionList";

        public PermissionService(R.IPermissionRepository repository, IModuleRegistry modules)
        {
            this._repository = repository;
            this._cache = modules.GetModules<ICacheModule>().Last();
        }

        #region IPermissionService Members

        public async Task<Permission> GetItemAsync(string name)
        {
            List<Permission> permissions = await GetListFromCacheAsync();
            if (!permissions.IsNullOrEmpty())
                return permissions.FirstOrDefault(m => m.Name == name);
            else
                return await _repository.GetItemAsync(name);
        }

        public async Task<Permission> GetItemAsync(Guid permissionID)
        {
            List<Permission> permissions = await GetListFromCacheAsync();
            if (!permissions.IsNullOrEmpty())
                return permissions.FirstOrDefault(m => m.PermissionID == permissionID);
            else
                return await _repository.GetItemAsync(permissionID);
        }

        public async Task<List<Permission>> GetListAsync()
        {
            var permissions = await GetListFromCacheAsync();
            if (!permissions.IsNullOrEmpty())
                return ClonePermissions(permissions);
            else
                return await _repository.GetListAsync();
        }

        public async Task<bool> SaveAsync(PermissionInput permissionInput, ModelStateDictionary modelState)
        {
            Guard.ArgumentNotNull(permissionInput, "permissionInput");
            Guard.ArgumentNotNull(modelState, "modelState");

            bool result = await _repository.SaveAsync(permissionInput);
            if (!result)
                modelState.AddModelError("Name", "添加或编辑时保存失败");
            else
                _cache.Invalidate(PermissionListCacheKey);
            return result;
        }

        public async Task<bool> SaveAsync(IEnumerable<PermissionInput> permissions, ModelStateDictionary modelState)
        {
            Guard.ArgumentNotNull(permissions, "permissions");
            Guard.ArgumentNotNull(modelState, "modelState");

            foreach (var per in permissions)
            {
                if (!await _repository.SaveAsync(per))
                {
                    throw new InvalidOperationException("{0} 模块的 {1} 权限添加失败".FormatWith(per.Name));
                }
            }

            _cache.Invalidate(PermissionListCacheKey);
            return true;
        }

        public async Task<bool> RemoveAsync(Guid permissionID)
        {
            bool result = await _repository.RemoveAsync(permissionID);
            if (result)
                _cache.Invalidate(PermissionListCacheKey);
            return result;
        }

        public async Task<bool> RemoveAsync(IEnumerable<Guid> ids)
        {
            if (ids == null) return true;

            bool result = true;
            foreach (var id in ids)
                result = await _repository.RemoveAsync(id);

            if (result)
                _cache.Invalidate(PermissionListCacheKey);
            return result;
        }

        public async Task<bool> MoveAsync(Guid permissionID, MovingTarget target)
        {
            bool result = await _repository.MoveAsync(permissionID, target);
            if (result)
                _cache.Invalidate(PermissionListCacheKey);
            return result;
        }

        #endregion

        #region Private Methods

        private List<Permission> ClonePermissions(IEnumerable<Permission> source)
        {
            if (source.IsNullOrEmpty())
                return new List<Permission>(0);

            var clone = source.DeepClone() as IEnumerable<Permission>;
            if(clone==null) return new List<Permission>(0);
            return clone.ToList();

            //List<Permission> permissions = new List<Permission>();
            //foreach (var item in source)
            //{
            //    permissions.Add(new Permission
            //    {
            //        ParentID = item.ParentID,
            //        ModuleName = item.ModuleName,
            //        PermissionID = item.PermissionID,
            //        Name = item.Name,
            //        Level = item.Level,
            //        DisplayOrder = item.DisplayOrder
            //    });
            //}
            //return permissions;

        }

        private async Task<List<Permission>> GetListFromCacheAsync()
        {
            if (_cache != null)
                return await _cache.GetItemAsync<List<Permission>>(
                    PermissionListCacheKey,
                    async () => await _repository.GetListAsync(),
                    TimeSpan.FromDays(1)
                    );

            return null;
        }

        #endregion
    }
}
