using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lavie.Models;

namespace Lavie.Infrastructure
{
    public interface ICacheModule : IModule
    {
        void Insert(string key, object value, TimeSpan? cacheTime = null);
        T GetItems<T, K>(string key, Func<T> getUncachedItems, Func<K, IEnumerable<ICacheEntity>> getDependencies, TimeSpan? cacheTime = null)
            where T : IEnumerable<K>;
        T GetItems<T, K>(string key, Func<T> getUncachedItems, TimeSpan? cacheTime = null)
            where T : IEnumerable<K>;
        T GetItems<T, K>(string key, CachePartition partition, Func<T> getUncachedItems, Func<K, IEnumerable<ICacheEntity>> getDependencies, TimeSpan? cacheTime = null)
            where T : IEnumerable<K>;
        T GetItems<T, K>(string key, CachePartition partition, Func<T> getUncachedItems, TimeSpan? cacheTime = null)
            where T : IEnumerable<K>;
        T GetItem<T>(string key, Func<T> getUncachedItem, Func<T, IEnumerable<ICacheEntity>> getDependencies, TimeSpan? cacheTime = null);
        T GetItem<T>(string key, Func<T> getUncachedItem, TimeSpan? cacheTime = null);
        Task<T> GetItemAsync<T>(string key, Func<Task<T>> getUncachedItem, TimeSpan? cacheTime = null);
        void InvalidateItem(ICacheEntity item);
        void Invalidate<T>() where T : ICacheEntity;
        void Invalidate(string key);
    }
}
