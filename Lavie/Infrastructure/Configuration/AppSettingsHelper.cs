using System;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;

namespace Lavie.Infrastructure
{
    public class AppSettingsHelper
    {
        private readonly NameValueCollection _appSettings;
        private static readonly Regex EmailRegex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public AppSettingsHelper(NameValueCollection appSettings)
        {
            this._appSettings = appSettings;
        }

        #region GetString

        public string GetString(string name)
        {
            return GetValue(name, true);
        }

        public string GetString(string name, string defaultValue)
        {
            return GetValue(name, false, defaultValue);
        }

        #endregion

        #region GetEmail

        public string GetEmail(string name)
        {
            string value = GetValue(name, true);
            ValidateEmail(name, value);
            return value;
        }

        public string GetEmail(string name, string defaultValue)
        {
            string value = GetValue(name, false, defaultValue);
            ValidateEmail(name, value);
            return value;
        }

        #endregion

        #region GetStringArray

        public string[] GetStringArray(string name, string separator)
        {
            return GetStringArray(name, separator, true, null);
        }

        public string[] GetStringArray(string name, string separator, string[] defaultValue)
        {
            return GetStringArray(name, separator, false, defaultValue);
        }
       
        private string[] GetStringArray(string name, string separator, bool valueRequired, string[] defaultValue)
        {
            string value = GetValue(name, valueRequired);

            if (!string.IsNullOrEmpty(value))
                return value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            else if (!valueRequired)
                return defaultValue;

            throw GenerateRequiredSettingException(name);
        }

        #endregion

        #region GetInt32

        public int GetInt32(string name)
        {
            return GetInt32(name, null);
        }

        public int GetInt32(string name, int? defaultValue)
        {
            return GetValue<int>(name, (string v, out int pv) => int.TryParse(v, out pv), defaultValue);
        }

        #endregion

        #region GetBoolean

        public bool GetBoolean(string name)
        {
            return GetBoolean(name, null);
        }

        public bool GetBoolean(string name, bool? defaultValue)
        {
            return GetValue<bool>(name, (string v, out bool pv) => bool.TryParse(v, out pv), defaultValue);
        }

        #endregion

        #region GetTimeSpan

        public TimeSpan GetTimeSpan(string name)
        {
            return TimeSpan.Parse(GetValue(name, true));
        }

        public TimeSpan GetTimeSpan(string name, TimeSpan defaultValue)
        {
            string val = GetValue(name, false);

            if (val == null)
                return defaultValue;

            return TimeSpan.Parse(val);
        }

        #endregion

        #region GetIPAddress

        public IPAddress GetIPAddress(string name)
        {
            return IPAddress.Parse(GetValue(name, true));
        }

        public IPAddress GetIPAddress(string name, IPAddress defaultValue)
        {
            string val = GetValue(name, false);

            if (val == null)
                return defaultValue;

            return IPAddress.Parse(val);
        }

        #endregion

        #region Private Methods
      
        private delegate bool Parser<T1, T2, TResult>(T1 t1, out T2 t2);
        private T GetValue<T>(string name, Parser<string, T, bool> parseValue, T? defaultValue) where T : struct
        {
            string value = _appSettings[name];

            if (value != null)
            {
                T parsedValue = default(T);

                if (parseValue(value, out parsedValue))
                    return parsedValue;
                else
                    throw new InvalidOperationException(string.Format("Setting '{0}' was not a valid {1}", name, typeof(T).FullName));
            }

            if (!defaultValue.HasValue)
                throw GenerateRequiredSettingException(name);
            else
                return defaultValue.Value;
        }
        private string GetValue(string name, bool valueRequired, string defaultValue = null)
        {
            string value = _appSettings[name];

            if (value != null)
                return value;
            else if (!valueRequired)
                return defaultValue;

            throw GenerateRequiredSettingException(name);
        }

        private void ValidateEmail(string name, string email)
        {
            if (!EmailRegex.IsMatch(email))
                throw new InvalidOperationException(string.Format("'{0}' is not an email address.", name));
        }

        private InvalidOperationException GenerateRequiredSettingException(string name)
        {
            return new InvalidOperationException(string.Format("Could not find required setting '{0}'", name));
        }

        #endregion
    }
}