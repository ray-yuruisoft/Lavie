using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Extensions.Object;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Modules.Admin.Repositories;
using XM = Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Repositories
{
    public interface IBulletinRepository
    {
        Task<XM.Bulletin> GetItemAsync();
        Task<bool> SaveAsync(BulletinInput bulletin, ModelStateDictionary modelState);
    }

    public class BulletinRepository : RepositoryBase, IBulletinRepository
    {
        public async Task<XM.Bulletin> GetItemAsync()
        {
            var item = await DbContext.Bulletins.FirstOrDefaultAsync();
            return item.ToModel<XM.Bulletin>();
        }

        public async Task<bool> SaveAsync(BulletinInput bulletin, ModelStateDictionary modelState)
        {
            var dbBulletin = await DbContext.Bulletins.FirstOrDefaultAsync();
            if (dbBulletin == null) return false;

            dbBulletin.UpdateFrom(bulletin);
            await DbContext.SaveChangesAsync();

            return true;
        }

    }
}
