using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Lavie.Extensions.Object;
using Lavie.Modules.Admin.Models.InputModels;
using XM = Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Repositories;
using System.Web.Mvc;

namespace Lavie.Modules.Admin.Repositories
{
    public interface ISiteRepository
    {
        XM.Site GetItem();
        Task<XM.Site> GetItemAsync();
        Task<bool> SaveAsync(SiteInput site, ModelStateDictionary modelState);
    }


    public class SiteRepository : RepositoryBase,ISiteRepository
    {
        public XM.Site GetItem()
        {
            var result = DbContext.Sites.FirstOrDefault();
            return result.ToModel<XM.Site>();
        }
        public async Task<XM.Site> GetItemAsync()
        {
            var result = await DbContext.Sites.FirstOrDefaultAsync();
            return result.ToModel<XM.Site>();
        }

        public async Task<bool> SaveAsync(SiteInput siteInput, ModelStateDictionary modelState)
        {
            //由于SiteInput不包含主键，所以先从数据库中获取实体
            Site site = await DbContext.Sites.FirstOrDefaultAsync();
            if (site == null)
            {
                modelState.AddModelError("Error", "获取站点配置失败");
                return false;
            }

            //将SiteInput中的数据更新入Site实体中
            site.UpdateFrom(siteInput);
            await DbContext.SaveChangesAsync();

            return true;
        }

    }
}
