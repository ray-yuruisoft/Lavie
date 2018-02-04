using System;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.ModelBinders
{
    public class UserStatusModelBinder: IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string strStatus = bindingContext.ValueProvider.GetAttemptedValue("status");

            if (!String.IsNullOrEmpty(strStatus) && Enum.IsDefined(typeof(UserStatus), strStatus))
            {
                //区分大小写
                return (UserStatus)Enum.Parse(typeof(UserStatus), strStatus);
                //不区分大小写
                //try{status = (UserStatus)Enum.Parse(typeof(UserStatus), strStatus, true);}catch{}
            }
            return UserStatus.NotSet;
            //throw new InvalidOperationException("UserStatus枚举值参数错误");
        }
    }
}
