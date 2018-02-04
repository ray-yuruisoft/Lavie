using System;
using System.Collections.Generic;
using System.Linq;
using Lavie.Models;
using Lavie.Utilities.Exceptions;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Lavie.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 对枚举器的每个元素执行指定的操作
        /// </summary>
        /// <typeparam name="T">枚举器类型参数</typeparam>
        /// <param name="source">枚举器</param>
        /// <param name="action">要对枚举器的每个元素执行的委托</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            var enumerable = source as T[] ?? source.ToArray();
            if (enumerable.Length == 0 || action == null)
            {
                return;
            }
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        /// 指示指定的枚举器是null还是没有任何元素
        /// </summary>
        /// <typeparam name="T">枚举器类型参数</typeparam>
        /// <param name="source">要测试的枚举器</param>
        /// <returns>true:枚举器是null或者没有任何元素 false:枚举器不为null并且包含至少一个元素</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool IsNullOrEmpty<T>(this List<T> source)
        {
            return source == null || source.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this T[] source)
        {
            return source == null || source.Length == 0;
        }

        /// <summary>
        /// 对String型序列的每个元素进行字符串替换操作
        /// </summary>
        /// <param name="source">源序列</param>
        /// <param name="oldValue">查找字符串</param>
        /// <param name="newValue">替换字符串</param>
        /// <returns>新的String型序列</returns>
        public static IEnumerable<string> Replace(IEnumerable<string> source, string oldValue, string newValue)
        {
            return source.Select(format => format.Replace(oldValue, newValue));
        }

        /// <summary>
        /// 将序列转化为ReadOnlyCollection
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <param name="source">源序列</param>
        /// <returns></returns>
        public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            return new ReadOnlyCollection<T>(source.ToList());
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T">泛型类型参数</typeparam>
        /// <param name="sourceQuery">查询集</param>
        /// <param name="pagingInfo">分页信息</param>
        /// <param name="topQuery">跳过记录集</param>
        /// <returns></returns>
        public static async Task<PagedList<T>> GetPagedListAsync<T>(this IQueryable<T> sourceQuery, PagingInfo pagingInfo, IEnumerable<T> topQuery = null) where T: class
        {
            Guard.ArgumentNotNull(sourceQuery, "sourceQuery");

            // 跳过记录集无记录
            if (topQuery.IsNullOrEmpty())
            {
                var list = await sourceQuery.Skip(pagingInfo.PageIndex * pagingInfo.PageSize).Take(pagingInfo.PageSize).ToListAsync();
                return new PagedList<T>(list);
            }
            else
            {
                // 查询集记录数
                // int sourceItemCount = await sourceQuery.CountAsync();

                // 跳过的记录数
                int topItemCount = topQuery.Count();
                // 跳过的页数，比如一页显示10条，跳过4条、14条或24条，则跳过的页数为0、1或2
                int skipPage = (int)Math.Floor((double)topItemCount / pagingInfo.PageSize);

                // 如果目标页数在跳过的页数范围内，直接从topItems获取
                if (skipPage > pagingInfo.PageIndex)
                {
                    var topItems = topQuery.Skip(pagingInfo.PageIndex * pagingInfo.PageSize).Take(pagingInfo.PageSize);
                    return new PagedList<T>(topItems, pagingInfo.PageIndex, pagingInfo.PageSize, !pagingInfo.IsExcludeMetaData ? (await sourceQuery.CountAsync() + topItemCount) : 0);
                }
                else
                {
                    int topSkipCount = skipPage * pagingInfo.PageSize;
                    int topTakeCount = topItemCount % pagingInfo.PageSize;
                    var topItems = topQuery.Skip(skipPage * pagingInfo.PageSize).Take(topTakeCount);

                    int sourceSkipCount = (pagingInfo.PageIndex - skipPage) * pagingInfo.PageSize;
                    int sourceTakeCount = pagingInfo.PageSize - topTakeCount;
                    var sourceItems = await sourceQuery.Skip(sourceSkipCount).Take(sourceTakeCount).ToListAsync();

                    return new PagedList<T>(topItems.Concat(sourceItems), pagingInfo.PageIndex, pagingInfo.PageSize, !pagingInfo.IsExcludeMetaData ? (await sourceQuery.CountAsync() + topItemCount) : 0);
                }

            }
        }

        public static async Task<Page<T>> GetPageAsync<T>(this IQueryable<T> sourceQuery, PagingInfo pagingInfo, IEnumerable<T> topQuery = null) where T : class
        {
            Guard.ArgumentNotNull(sourceQuery, "sourceQuery");

            var page = new Page<T>();

            // 跳过记录集无记录
            if (topQuery.IsNullOrEmpty())
            {
                page.List = await sourceQuery.Skip(pagingInfo.PageIndex * pagingInfo.PageSize).Take(pagingInfo.PageSize).ToListAsync();
                if (!pagingInfo.IsExcludeMetaData)
                {
                    page.TotalItemCount = await sourceQuery.CountAsync();
                    page.TotalPageCount = (int)Math.Ceiling(page.TotalItemCount / (double)pagingInfo.PageSize);
                }
            }
            else
            {
                // 查询集记录数
                // int sourceItemCount = await sourceQuery.CountAsync();

                // 跳过的记录数
                int topItemCount = topQuery.Count();
                // 跳过的页数，比如一页显示10条，跳过4条、14条或24条，则跳过的页数为0、1或2
                int skipPage = (int)Math.Floor((double)topItemCount / pagingInfo.PageSize);

                // 如果目标页数在跳过的页数范围内，直接从topItems获取
                if (skipPage > pagingInfo.PageIndex)
                {
                    page.List = topQuery.Skip(pagingInfo.PageIndex * pagingInfo.PageSize).Take(pagingInfo.PageSize).ToList();
                    if (!pagingInfo.IsExcludeMetaData)
                    {
                        page.TotalItemCount = await sourceQuery.CountAsync() + topItemCount;
                        page.TotalPageCount = (int)Math.Ceiling(page.TotalItemCount / (double)pagingInfo.PageSize);
                    }
                }
                else
                {
                    int topSkipCount = skipPage * pagingInfo.PageSize;
                    int topTakeCount = topItemCount % pagingInfo.PageSize;
                    var topItems = topQuery.Skip(skipPage * pagingInfo.PageSize).Take(topTakeCount);

                    int sourceSkipCount = (pagingInfo.PageIndex - skipPage) * pagingInfo.PageSize;
                    int sourceTakeCount = pagingInfo.PageSize - topTakeCount;
                    var sourceItems = await sourceQuery.Skip(sourceSkipCount).Take(sourceTakeCount).ToListAsync();

                    page.List = topItems.Concat(sourceItems).ToList();
                    if (!pagingInfo.IsExcludeMetaData)
                    {
                        page.TotalItemCount = await sourceQuery.CountAsync() + topItemCount;
                        page.TotalPageCount = (int)Math.Ceiling(page.TotalItemCount / (double)pagingInfo.PageSize);
                    }
                }

            }

            return page;
        }

    }
}
