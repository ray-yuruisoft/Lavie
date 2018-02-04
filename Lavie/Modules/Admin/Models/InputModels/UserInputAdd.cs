using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Lavie.ModelValidation.Attributes;
using System.Collections.Generic;

namespace Lavie.Modules.Admin.Models.InputModels
{
    public class UserInputAdd : UserInput
    {
        [Required(ErrorMessage = "登录密码不能为空")]
        public override string Password { get; set; }

        [Required(ErrorMessage = "确认密码不能为空")]
        public override string PasswordConfirm { get; set; }
    }

}
