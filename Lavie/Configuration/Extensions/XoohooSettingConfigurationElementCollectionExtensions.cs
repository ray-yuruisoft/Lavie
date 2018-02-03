using Lavie.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
