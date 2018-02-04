using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Infrastructure;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;
using R = Lavie.Modules.Admin.Repositories;

namespace Lavie.Modules.Admin.Services
{
    public interface IBulletinService
    {
        Task<Bulletin> GetItemAsync();
        Task<bool> SaveAsync(BulletinInput bulletin, ModelStateDictionary modelState);
    }

    public class BulletinService : IBulletinService
    {
        private readonly R.IBulletinRepository _repository;
        private readonly ICacheModule _cache;
        private const string CacheKey = "Bulletin";

        public BulletinService(R.IBulletinRepository repository, IModuleRegistry moduleRegistry)
        {
            this._repository = repository;
            this._cache = moduleRegistry.GetModules<ICacheModule>().Last();
        }

        public async Task<Bulletin> GetItemAsync()
        {
            return _cache!=null
                ?await _cache.GetItemAsync<Bulletin>(CacheKey,async () => await _repository.GetItemAsync()) 
                :await _repository.GetItemAsync();
            
        }

        public async Task<bool> SaveAsync(BulletinInput bulletin, ModelStateDictionary modelState)
        {
            bool result = await _repository.SaveAsync(bulletin, modelState);
            if(result)
                _cache.Invalidate(CacheKey);
            return result;
        }
    }
}
