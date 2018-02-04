using System.Collections.Generic;

namespace Lavie.Infrastructure
{
    public interface ICacheEntity
    {
        /// <summary>
        /// 获取缓存键
        /// </summary>
        /// <returns></returns>
        string GetCacheItemKey();

        /// <summary>
        /// 获取缓存依赖项
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICacheEntity> GetCacheDependencyItems();
    }
}
