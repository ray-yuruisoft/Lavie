using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Project.FastRiderPayPolicy.Chengdu;

namespace Lavie.Modules.Project.Extentions
{

    public class EnumRangeAttribute : ValidationAttribute
    {

        private int[] _arr;
        private Type _type;

        public EnumRangeAttribute(int[] arr)
        {
            _arr = arr;
        }

        public EnumRangeAttribute(Type type)
        {
            _type = type;
        }

        public override bool IsValid(object value)
        {

            if (value is int temp)
            {
                if (_arr != null && _arr.Any(c => c == temp))
                    return true;
                else if (_type != null && Enum.IsDefined(_type, value))
                    return true;
            }
            return false;

        }

    }

    public class GuidIsEmptyAttribute : ValidationAttribute
    {

        public override bool IsValid(object value)
        {

            if (value is Guid temp)
            {
                if (temp == Guid.Empty)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

    }

    public static class ValidationAttributeExtensions
    {

        private static bool IsValidMethord(object ValidationData, out string ErrMessage)
        {

            if (ValidationData == null)
            {
                ErrMessage = "需验证模型值为空";
                return false;
            }
            Type type = ValidationData.GetType();
            foreach (var item in type.GetProperties())
            {
                if (item.GetCustomAttribute(typeof(ValidationAttribute)) is ValidationAttribute attr)
                {
                    if (!attr.IsValid(item.GetValue(ValidationData)))
                    {
                        ErrMessage = attr.ErrorMessage;
                        return false;
                    }
                }
            }
            ErrMessage = null;
            return true;

        }
        public static bool IsValid(this object source, out string ErrMessage)
        {
            return IsValidMethord(source, out ErrMessage);
        }
        public static bool IsValid(this object source)
        {
            return IsValidMethord(source, out string ErrMessage);
        }

    }

}
