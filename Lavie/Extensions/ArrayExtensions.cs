using System;
using Lavie.Utilities.Exceptions;

namespace Lavie.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// 获取子数组
        /// </summary>
        /// <typeparam name="T">泛型类型参数</typeparam>
        /// <param name="sourceArray">源数组</param>
        /// <param name="length">获取的长度</param>
        /// <returns>子数组</returns>
        public static T[] SubArray<T>(this T[] sourceArray, int length)
        {
            return SubArray(sourceArray, (long)length);
        }
        /// <summary>
        /// 获取子数组
        /// </summary>
        /// <typeparam name="T">泛型类型参数</typeparam>
        /// <param name="sourceArray">源数组</param>
        /// <param name="length">获取的长度</param>
        /// <returns>子数组</returns>
        public static T[] SubArray<T>(this T[] sourceArray, long length)
        {
            ValidParamters(sourceArray, length);
            T[] result = new T[length];
            Array.Copy(sourceArray, result, length);
            return result;
        }
        private static void ValidParamters<T>(T[] sourceArray, long length)
        {
            Guard.ArgumentNotNull(sourceArray, "sourceArray");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length 不能小于零");
            if (sourceArray.Length < length)
                throw new ArgumentOutOfRangeException("length 不能大于 sourceArray 中的元素数");
        }

    }
}
