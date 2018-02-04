using System.Collections.Specialized;
using Lavie.Configuration;
using Lavie.Infrastructure;

namespace Lavie.Configuration.Extensions
{
    public static class LavieSettingConfigurationElementCollectionExtensions
    {
        public static NameValueCollection ToNameValueCollection(this LavieSettingConfigurationElementCollection settings)
        {
            NameValueCollection nvm = new NameValueCollection(settings.Count);

            foreach (LavieSettingConfigurationElement setting in settings)
                nvm.Add(setting.Name, setting.Value);

            return nvm;
        }
        public static AppSettingsHelper ToAppSettingsHelper(this LavieSettingConfigurationElementCollection settings)
        {
            return new AppSettingsHelper(settings.ToNameValueCollection());
        }
    }
}
