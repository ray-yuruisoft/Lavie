using System;
using Lavie.Models;

namespace Lavie.Extensions
{
    public static class BooleanExtensions
    {
        public static string ToChinese(this bool value, string trueString, string falseString)
        {
            //不对trueString和falseString进行null校验，因为有时候可能是故意的
            return value ? trueString : falseString;
        }
        //CS0854: 表达式树不能包含使用可选参数的调用,所以定义一个不需要默认参数的方法
        public static string ToChinese(this bool value)
        {
            return value.ToChinese("是", "否");
        }
    }
}
