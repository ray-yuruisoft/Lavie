using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Configuration
{
    public class LavieModuleConfigurationElement : ConfigurationElement
    {
        public LavieModuleConfigurationElement() { }

        public LavieModuleConfigurationElement(string elementName)
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
        [ConfigurationProperty("dataProvider"/*, IsRequired = true*/)]
        public string DataProvider
        {
            get
            {
                return (string)this["dataProvider"];
            }
            set
            {
                this["dataProvider"] = value;
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
