using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Lavie.Utilities.Exceptions;

namespace Lavie.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 根据枚举值，获取枚举的DisplayName
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="enumValue">枚举值</param>
        /// <returns>DisplayName</returns>
        public static string GetEnumDisplayName(this object enumValue)
        {
            Guard.ArgumentNotNull(enumValue, "enumValue");

            Type type = enumValue.GetType();
            if (!type.IsEnum)
                throw new ArgumentOutOfRangeException("enumValue", "The parameter named \"enumValue\" is not an enum value.");

            return GetEnumDisplayName(enumValue, type);            
        }
        /// <summary>
        /// 根据枚举的类型，获取枚举值与DisplayName形成的字典
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="type">枚举类型</param>
        /// <returns>枚举值与DisplayName形成的字典</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetEnumDictionary<T>(this Type type)
        {
            Guard.ArgumentNotNull(type, "type");

            if (!type.IsEnum)
                throw new ArgumentOutOfRangeException("type", "The parameter named \"type\" is not an enum.");

            return from e in Enum.GetValues(type).Cast<T>()
                   select new KeyValuePair<T, string>(e, e.GetEnumDisplayName(type));

        }
        /// <summary>
        /// 根据枚举的类型，获取枚举值与DisplayName形成的字典(非扩展方法)
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<T, string>> GetEnumDictionary<T>()
        {
            return GetEnumDictionary<T>(typeof(T));
        }

        public static string GetEnumRawConstantValue(this object enumValue)
        {
            Guard.ArgumentNotNull(enumValue, "enumValue");

            Type type = enumValue.GetType();
            if (!type.IsEnum)
                throw new ArgumentOutOfRangeException("enumValue", "The parameter named \"enumValue\" is not an enum value.");

            return GetEnumRawConstantValue(enumValue, type);
        }

        private static string GetEnumRawConstantValue(this object enumValue, Type type)
        {
            var filedInfo = type.GetField(Enum.GetName(type, enumValue));
            return filedInfo.GetRawConstantValue().ToString();
        }

        private static string GetEnumDisplayName(this object enumValue, Type type)
        {
            var attributes = type.GetField(Enum.GetName(type, enumValue)).GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length > 0) // TODO: 如果DisplayAttribute的DisplayName属性为IsNullOrWhiteSpace,尝试从资源文件获取
                return ((DisplayAttribute)attributes[0]).GetName();
            else
                return enumValue.ToString();
        }

    }
}
