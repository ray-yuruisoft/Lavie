using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Configuration
{
    public class LavieBackgroundServiceConfigurationElement : ConfigurationElement
    {
        public LavieBackgroundServiceConfigurationElement() { }

        public LavieBackgroundServiceConfigurationElement(string elementName)
        {
            Name = elementName;
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

        [ConfigurationProperty("executorType", IsRequired = true)]
        public string ExecutorType
        {
            get
            {
                return (string)this["executorType"];
            }
            set
            {
                this["executorType"] = value;
            }
        }
        [ConfigurationProperty("interval", IsRequired = true)]
        public int Interval
        {
            get
            {
                return (int)this["interval"];
            }
            set
            {
                this["interval"] = value;
            }
        }
        [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return (bool)this["enabled"];
            }
            set
            {
                this["enabled"] = value;
            }
        }

        [ConfigurationProperty("settings")]
        public LavieSettingConfigurationElementCollection Settings
        {
            get
            {
                return (LavieSettingConfigurationElementCollection)this["settings"];
            }
            set
            {
                this["settings"] = value;
            }
        }

    }
}
