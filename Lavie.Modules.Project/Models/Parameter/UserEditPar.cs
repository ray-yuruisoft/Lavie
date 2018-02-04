using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.ModelValidation.Attributes;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;

namespace Lavie.Modules.Project.Models
{
    public class UserEditPar : UserInput
    {
        [DisplayName("用户ID")]
        [Range(1, Int32.MaxValue, ErrorMessage = "请选择存在的用户")]
        new public int UserID { get; set; }

        [DisplayName("所属用户组")]
        new public Guid? GroupID { get; set; }

        [StringLength(20, MinimumLength = 4, ErrorMessage = "登录密码请保持在4-20个字符之间")]
        [DataType(DataType.Password)]
        [DisplayName("登录密码")]
        public override string Password { get; set; }

        [StringLength(20, MinimumLength = 4, ErrorMessage = "确认密码请保持在4-20个字符之间")]
        [Lavie.ModelValidation.Attributes.Compare("Password", ValidationCompareOperator.Equal, ValidationDataType.String, ErrorMessage = "请确认两次输入的密码一致")]
        [DataType(DataType.Password)]
        [DisplayName("确认密码")]
        public override string PasswordConfirm { get; set; }

        [DisplayName("用户状态")]
        [JsonConverter(typeof(StringEnumConverter))]
        [Required(ErrorMessage ="请选择用户状态")]
        new public UserStatus Status { get; set; }

    }
}
