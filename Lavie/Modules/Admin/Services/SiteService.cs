using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Infrastructure;
using Lavie.Models;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;
using R = Lavie.Modules.Admin.Repositories;

namespace Lavie.Modules.Admin.Services
{
    public interface ISiteService
    {
        //Site GetItem();
        Site GetItem();
        Task<Site> GetItemAsync();
        Task<bool> SaveAsync(SiteInput site, ModelStateDictionary modelState);
        bool Restart();
    }
    public class SiteService : ISiteService
    {
        private readonly R.ISiteRepository _repository;
        private readonly ICacheModule _cache;
        private readonly ILoggingModule _logger;
        private const string SiteCacheKeyFormat = "Site";

        public SiteService(R.ISiteRepository repository, IModuleRegistry moduleRegistry)
        {
            this._repository = repository;
            this._cache = moduleRegistry.GetModules<ICacheModule>().LastOrDefault();
            this._logger = moduleRegistry.GetModules<ILoggingModule>().LastOrDefault();
        }

        public Site GetItem()
        {
            if (_cache != null)
            {
                return _cache.GetItem<Site>(SiteCacheKeyFormat,
                    () => _repository.GetItem(), TimeSpan.FromDays(1));
            }
            return _repository.GetItem();
        }
        public async Task<Site> GetItemAsync()
        {
            if (_cache != null)
            {
                return await _cache.GetItemAsync<Site>(SiteCacheKeyFormat,
                    async () => await _repository.GetItemAsync(), TimeSpan.FromDays(1));
            }
            return await _repository.GetItemAsync();
        }

        public async Task<bool> SaveAsync(SiteInput site, ModelStateDictionary modelState)
        {
            //保存实体
            bool result = await _repository.SaveAsync(site, modelState);
            //清空Site实体缓存
            _cache.Invalidate(SiteCacheKeyFormat);

            _logger.Info("修改系统配置");

            return result;
        }

        public bool Restart()
        {
            try
            {
                string webConfig = System.Web.HttpRuntime.AppDomainAppPath + "Web.Config";
                System.IO.File.SetLastWriteTime(webConfig, DateTime.Now);
                _logger.Info("重新启动站点成功");
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("重新启动站点失败", e);
                return false;
            }
        }

    }
}
