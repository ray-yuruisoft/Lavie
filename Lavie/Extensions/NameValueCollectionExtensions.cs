using System;
using System.Collections.Specialized;
using System.Text;

namespace Lavie.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static bool IsTrue(this NameValueCollection collection, string key)
        {
            if (collection == null) return false;

            bool isTrue;

            string[] values = collection.GetValues(key);

            return !values.IsNullOrEmpty()
                && bool.TryParse(values[0], out isTrue)
                && isTrue;
        }

        public static bool? IsTrueNullable(this NameValueCollection collection, string key)
        {
            if (collection == null) return null;

            bool? isTrue = null;
            string[] values = collection.GetValues(key);
            if (!values.IsNullOrEmpty())
            {
                bool isTrueValue;
                if (bool.TryParse(values[0], out isTrueValue))
                    isTrue = isTrueValue;
            }

            return isTrue;
        }

        public static string ToQueryString(this NameValueCollection queryString)
        {
            if (queryString.Count > 0)
            {
                StringBuilder qs = new StringBuilder();

                qs.Append("?");

                for (int i = 0; i < queryString.Count; i++)
                {
                    if (i > 0)
                        qs.Append("&");

                    qs.AppendFormat("{0}={1}", queryString.Keys[i], queryString[i]);
                }

                return qs.ToString();
            }

            return String.Empty;
        }
    }
}