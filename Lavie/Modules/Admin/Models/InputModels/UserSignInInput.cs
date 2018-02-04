using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Lavie.ModelValidation.Attributes;

namespace Lavie.Modules.Admin.Models.InputModels
{
    public class UserSignInInput
    {
        [Required(ErrorMessage = "用户名不能为空")]
        //[Slug(ErrorMessage = "以字母开头的字母数字_和-组成")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "用户名请保持在2-20个字符之间")]
        [Slug(ErrorMessage = " ")]
        [DisplayName("用户名")]
        public string Username { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码请保持在6-20个字符之间")]
        [DataType(DataType.Password)]
        [DisplayName("密码")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "验证码不能为空")]
        [Required(ErrorMessage = " ")]
        [DisplayName("验证码")]
        public string ValidateCode { get; set; }
    }
}
