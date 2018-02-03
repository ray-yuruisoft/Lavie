using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Configuration
{
    public class LavieDataProviderConfigurationElement : ConfigurationElement
    {
        public LavieDataProviderConfigurationElement() { }

        public LavieDataProviderConfigurationElement(string elementName)
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

        [ConfigurationProperty("category", IsRequired = true)]
        public string Category
        {
            get
            {
                return (string)this["category"];
            }
            set
            {
                this["category"] = value;
            }
        }

        [ConfigurationProperty("connectionString")]
        public string ConnectionString
        {
            get
            {
                return (string)this["connectionString"];
            }
            set
            {
                this["connectionString"] = value;
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
