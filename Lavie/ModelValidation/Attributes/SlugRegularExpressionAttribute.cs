using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Lavie.ModelValidation.Attributes
{
    public class SlugAttribute : ModelClientValidationRegularExpressionAttribute
    {
        /// <summary>
        /// 字母开头，由字母、数字、连词符或下滑线组成的字符串
        /// </summary>
        public SlugAttribute() : base(@"^[a-zA-Z][a-zA-Z0-9-_]*$") { }
    }
    public class SlugWithMobileAttribute : ModelClientValidationRegularExpressionAttribute
    {
        /// <summary>
        /// 字母开头，由字母、数字、连词符或下滑线组成的字符串；或者1开头的11位数字的手机号码
        /// </summary>
        public SlugWithMobileAttribute() : base(@"^(([a-zA-Z][a-zA-Z0-9-_]*)|(1\d{10}))$") { }
    }
    public class SlugWithIntAttribute : ModelClientValidationRegularExpressionAttribute
    {
        /// <summary>
        /// 字母开头，由字母、数字、连词符或下滑线组成的字符串；正整数
        /// </summary>
        public SlugWithIntAttribute() : base(@"^(([a-zA-Z][a-zA-Z0-9-_]*)|(\d+))$") { }
    }

    public class SlugWithChineseAttribute : ModelClientValidationRegularExpressionAttribute
    {
        /// <summary>
        /// 字母或中文开头，由字母、数字、连词符或下滑线组成的字符串
        /// </summary>
        public SlugWithChineseAttribute() : base(@"^[a-zA-Z\u4E00-\u9FA5\uF900-\uFA2D][a-zA-Z0-9-_\u4E00-\u9FA5\uF900-\uFA2D]*$") { }
    }
    public class SlugWithPrefixAttribute : ModelClientValidationRegularExpressionAttribute
    {
        /// <summary>
        /// prefix开头，由字母、数字、连词符或下滑线组成的字符串
        /// </summary>
        public SlugWithPrefixAttribute(String prefix) : base(@"^" + prefix + @"[a-zA-Z0-9-_]*$") { }
    }
    public class SlugWithMobileEmailAttribute : ModelClientValidationRegularExpressionAttribute
    {
        /// <summary>
        /// 字母开头，由字母、数字、连词符或下滑线组成的字符串；或者1开头的11位数字的手机号码；或邮箱地址
        /// </summary>
        public SlugWithMobileEmailAttribute() : base(@"^(([a-zA-Z][a-zA-Z0-9-_]*)|(1\d{10}))|([\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?)$") { }
    }

}
