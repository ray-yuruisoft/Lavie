using System;
using System.ComponentModel;
using System.Web.Helpers;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;

namespace Lavie.Modules.Admin.ModelBinders
{
    public class UserInputModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // 当添加用户时，必须输入密码。
            // 因为依赖 UserID 和 Password 两个字段，所以在 BindPropery 和 OnModelUpdated 方法中都不合适
            var result = base.BindModel(controllerContext, bindingContext) as UserInput;

            if (result != null && result.UserID.HasValue)
            {
                if (result.Password.IsNullOrWhiteSpace())
                {
                    bindingContext.ModelState.AddModelError("Password", "登录密码不能为空");
                }
                if (result.PasswordConfirm.IsNullOrWhiteSpace())
                {
                    bindingContext.ModelState.AddModelError("PasswordConfirm", "确认密码不能为空");
                }
            }

            return result;

        }

    }
}
