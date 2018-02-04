﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Lavie.ModelValidation.Attributes;

namespace Lavie.Modules.Admin.Models.InputModels
{
    public class GetMobileValidationCodeInput
    {
        [Required(ErrorMessage = "请输入手机号码")]
        [Mobile(ErrorMessage = "请输入正确的手机号码")]
        [DisplayName("手机号码")]
        public string Mobile { get; set; }

        [DisplayName("验证类型")]
        [Range(1, 4, ErrorMessage = "请输入TypeID: 1 注册 2 重置密码 3 更换手机号 4 短信登录(如果没注册，则自动注册)")]
        public int TypeID { get; set; }
    }
    public class VerifyMobileValidationCodeInput
    {
        [Required(ErrorMessage = "请输入手机号码")]
        [Mobile(ErrorMessage = "请输入正确的手机号码")]
        [DisplayName("手机号码")]
        public string Mobile { get; set; }

        [DisplayName("验证类型")]
        [Range(1, 4, ErrorMessage = "请输入TypeID: 1 注册 2 重置密码 3 更换手机号 4 短信登录(如果没注册，则自动注册)")]
        public int TypeID { get; set; }

        [Required(ErrorMessage = "请输入短信验证码")]
        [StringLength(10, ErrorMessage = "短信验证码最多支持10位")]
        public string ValidationCode { get; set; }
    }

}
