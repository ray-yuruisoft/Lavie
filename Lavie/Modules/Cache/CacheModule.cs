using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lavie.Infrastructure;
using Lavie.Models;
using Lavie.Infrastructure.InversionOfControl;
using System.Web.Routing;

namespace Lavie.Modules.Cache
{
    public class CacheModule : Module, ICacheModule
    {
        private readonly System.Web.Caching.Cache _cache;

        public CacheModule()
        {
            _cache = HttpRuntime.Cache;
        }

        #region IModule Members

        public override string ModuleName
        {
            get { return "Cache"; }
        }

        #endregion

        #region ICacheModule Members

        /// <summary>
        /// 插入缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Insert(string key, object value, TimeSpan? cacheTime = null)
        {
            _cache.Insert(key
                , value
                , null
                , System.Web.Caching.Cache.NoAbsoluteExpiration
                , cacheTime??Settings.GetTimeSpan("CacheTime", TimeSpan.FromMinutes(30)));

        }

        public T GetItems<T, K>(string key, Func<T> getUncachedItems, Func<K, IEnumerable<ICacheEntity>> getDependencies, TimeSpan? cacheTime = null)
            where T : IEnumerable<K>
        {
            return GetItems<T, K>(key, null, getUncachedItems, getDependencies);
        }

        public T GetItems<T, K>(string key, Func<T> getUncachedItems, TimeSpan? cacheTime = null)
            where T : IEnumerable<K>
        {
            return GetItems<T, K>(key,getUncachedItems,null,cacheTime);
        }

        public T GetItems<T, K>(string key, CachePartition partition, Func<T> getUncachedItems, Func<K, IEnumerable<ICacheEntity>> getDependencies, TimeSpan? cacheTime = null)
            where T : IEnumerable<K>
        {
            string fullKey = partition != null ? key + partition.ToString() : key;
            T cacheItem = (T)_cache[fullKey];

            if (cacheItem != null) return cacheItem;

            T items = getUncachedItems();

            if (items != null)
            {
                Insert(fullKey, items, cacheTime);

                // partitions
                if (partition != null)
                {
                    var partitionList = _cache[key] as CacheDependencyList;

                    if (partitionList == null)
                        Insert(key, partitionList = new CacheDependencyList());

                    string keyAndPartition = key + partition.ToString();

                    if (!partitionList.Contains(keyAndPartition))
                        partitionList.Add(keyAndPartition);
                }

                // dependencies
                if (getDependencies != null)
                    if (items.Any())
                        foreach (K item in items)
                            // add dependencies for each item
                            foreach (ICacheEntity dependency in getDependencies(item))
                                CacheDependency(key, dependency);
                    else
                        // allow the caller to add dependencies for dependent objects even though no items were found
                        foreach (ICacheEntity dependency in getDependencies(default(K)))
                            CacheDependency(key, dependency);
            }

            return items;
        }

        public T GetItems<T, K>(string key, CachePartition partition, Func<T> getUncachedItems, TimeSpan? cacheTime = null)
            where T : IEnumerable<K>
        {
            return GetItems<T, K>(key, partition, getUncachedItems, null, cacheTime);
        }

        public T GetItem<T>(string key, Func<T> getUncachedItem, Func<T, IEnumerable<ICacheEntity>> getDependencies, TimeSpan? cacheTime = null)
        {
            T cacheItem = (T)_cache[key];

            if (cacheItem != null) return cacheItem;

            T item = getUncachedItem();

            if (item != null)
            {
                Insert(key, item, cacheTime);

                if (getDependencies != null)
                    foreach (ICacheEntity dependency in getDependencies(item))
                        CacheDependency(key, dependency);
            }

            return item;
        }
        public async Task<T> GetItemAsync<T>(string key, Func<Task<T>> getUncachedItem, Func<T, IEnumerable<ICacheEntity>> getDependencies, TimeSpan? cacheTime = null)
        {
            T cacheItem = (T)_cache[key];

            if (cacheItem != null) return cacheItem;

            T item = await getUncachedItem();

            if (item != null)
            {
                Insert(key, item, cacheTime);

                if (getDependencies != null)
                    foreach (ICacheEntity dependency in getDependencies(item))
                        CacheDependency(key, dependency);
            }

            return item;
        }

        public T GetItem<T>(string key, Func<T> getUncachedItem, TimeSpan? cacheTime = null)
        {
            return GetItem<T>(key, getUncachedItem,null, cacheTime);
        }

        public async Task<T> GetItemAsync<T>(string key, Func<Task<T>> getUncachedItem, TimeSpan? cacheTime = null)
        {
            return await GetItemAsync<T>(key, getUncachedItem, null, cacheTime);

        }


        /// <summary>
        /// 销毁ICacheEntity对象
        /// </summary>
        /// <param name="item"></param>
        public void InvalidateItem(ICacheEntity item)
        {
            if (item!=null)
                Invalidate(item.GetCacheItemKey());

            foreach (ICacheEntity cacheItem in item.GetCacheDependencyItems())
                InvalidateDependency(cacheItem.GetCacheItemKey());
        }

        public void Invalidate<T>() where T : ICacheEntity
        {
            var dependencyList = _cache[typeof(T).Name] as CacheDependencyList;

            if (dependencyList != null)
                foreach (string dependency in dependencyList)
                    InvalidateDependency(dependency);
        }

        /// <summary>
        /// 销毁指定键值的缓存对象
        /// </summary>
        /// <param name="key"></param>
        public void Invalidate(string key)
        {
            _cache.Remove(key);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 销毁依赖项
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dependencyKey"></param>
        private void InvalidateDependency(string dependencyKey)
        {
            CacheDependencyList dependencyList = _cache[dependencyKey] as CacheDependencyList;

            if (dependencyList != null)
            {
                foreach (string dependency in dependencyList)
                {
                    CacheDependencyList dependencyItems = _cache[dependency] as CacheDependencyList;

                    if (dependencyItems != null)
                        foreach (string dependencyItem in dependencyItems)
                            Invalidate(dependencyItem);

                    Invalidate(dependency);
                }

                Invalidate(dependencyKey);
            }
        }

        /// <summary>
        /// 缓存依赖项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="context"></param>
        /// <param name="item"></param>
        private void CacheDependency(string key, ICacheEntity item)
        {
            string dependencyKey = item.GetCacheItemKey();
            CacheDependencyList dependencyList = _cache[dependencyKey] as CacheDependencyList;

            if (dependencyList == null)
                Insert(dependencyKey, dependencyList = new CacheDependencyList());

            if (!dependencyList.Contains(key))
                dependencyList.Add(key);

            string typeDependencyKey = item.GetType().Name;
            CacheDependencyList typeDependencyList = _cache[typeDependencyKey] as CacheDependencyList;

            if (typeDependencyList == null)
                Insert(typeDependencyKey, typeDependencyList = new CacheDependencyList());

            if (!typeDependencyList.Contains(dependencyKey))
                typeDependencyList.Add(dependencyKey);
        }

        #endregion

    }
}
