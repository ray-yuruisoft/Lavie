﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// 获取类型属性的字符串表示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string PropertyName<T>(Expression<Func<T, object>> expression)
        {
            switch (expression.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression.Body).Member.Name;
                case ExpressionType.Convert:
                    return ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member.Name;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 判断类型是否是复合类型
        /// <para>string,int等从string转化而来的类型为简单类型，其他为复合类型</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsComplexType(this Type type)
        {
            return !TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
        }
    }
}
