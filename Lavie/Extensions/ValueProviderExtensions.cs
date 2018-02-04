using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lavie.Utilities.Exceptions;

namespace Lavie.Extensions
{
    public static class ValueProviderExtensions
    {
        public static string GetAttemptedValue(this IValueProvider valueProvider, string modelName)
        {
            Guard.ArgumentNotNullOrEmpty(modelName, "modelName");

            ValueProviderResult result = valueProvider.GetValue(modelName);

            return result != null ? result.AttemptedValue : null;
        }

        public static List<Guid> GetGuidList(this IValueProvider valueProvider, string modelName)
        {
            string tmpStr = valueProvider.GetAttemptedValue(modelName);
            if (!String.IsNullOrEmpty(tmpStr))
            {
                string[] array = tmpStr.Split(',');

                return (from s in array
                        where s.IsGuid()
                        select new Guid(s)).ToList();
            }
            return new List<Guid>();
        }
        public static List<int> GetInt32List(this IValueProvider valueProvider, string modelName)
        {
            string tmpStr = valueProvider.GetAttemptedValue(modelName);
            if (!String.IsNullOrEmpty(tmpStr))
            {
                string[] array = tmpStr.Split(',');
                int tryInt = 0;
                return (from i in array
                        where Int32.TryParse(i, out tryInt)
                        select tryInt).ToList();
            }
            return new List<int>();
        }

        public static Guid? GetNullableGuid(this IValueProvider valueProvider, string modelName)
        {
            Guid result;
            string tmpStr = valueProvider.GetAttemptedValue(modelName);
            if (tmpStr.GuidTryParse(out result))
                return result;
            else
                return null;
        }
        public static Guid GetGuid(this IValueProvider valueProvider, string modelName,Guid? defaultValue=null)
        {
            Guid result;
            string tmpStr = valueProvider.GetAttemptedValue(modelName);
            if (!tmpStr.GuidTryParse(out result))
            {
                if (defaultValue.HasValue)
                    result = defaultValue.Value;
                else
                    throw new InvalidCastException("modelName");
            }

            return result;
        }

        public static int? GetNullableInt32(this IValueProvider valueProvider, string modelName)
        {
            int result;
            string tmpStr = valueProvider.GetAttemptedValue(modelName);
            if (Int32.TryParse(tmpStr, out result))
                return result;
            else
                return null;
        }
        public static int GetInt32(this IValueProvider valueProvider, string modelName,int? defaultValue = null)
        {
            int result;
            string tmpStr = valueProvider.GetAttemptedValue(modelName);
            if (!Int32.TryParse(tmpStr, out result))
            {
                if (defaultValue.HasValue)
                    result = defaultValue.Value;
                else
                    throw new InvalidCastException("modelName");
            }

            return result;
        }

        public static DateTime? GetNullableDateTime(this IValueProvider valueProvider, string modelName)
        {
            DateTime result;
            string tmpStr = valueProvider.GetAttemptedValue(modelName);
            if (DateTime.TryParse(tmpStr, out result))
                return result;
            else
                return null;
        }
        public static DateTime GetDateTime(this IValueProvider valueProvider, string modelName, DateTime? defaultValue = null)
        {
            DateTime result;
            string tmpStr = valueProvider.GetAttemptedValue(modelName);
            if (!DateTime.TryParse(tmpStr, out result))
            {
                if (defaultValue.HasValue)
                    result = defaultValue.Value;
                else
                    throw new InvalidCastException("modelName");
            }

            return result;
        }
        public static Boolean GetBoolean(this IValueProvider valueProvider, string modelName, Boolean defaultValue = false)
        {
            Boolean result;
            string tmpStr = valueProvider.GetAttemptedValue(modelName);
            if (tmpStr.IsNullOrWhiteSpace()) return false;
            string[] tmpStrArr = tmpStr.Split(',');
            if (!Boolean.TryParse(tmpStrArr[0], out result))
            {
                return defaultValue;
            }

            return result;
        }
    }
}
