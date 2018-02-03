using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Configuration
{
    public class LavieBackgroundServiceConfigurationElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
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

        protected override ConfigurationElement CreateNewElement()
        {
            return new LavieBackgroundServiceConfigurationElement();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new LavieBackgroundServiceConfigurationElement(elementName);
        }

        protected new LavieBackgroundServiceConfigurationElement BaseGet(int index)
        {
            return (LavieBackgroundServiceConfigurationElement)base.BaseGet(index);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LavieBackgroundServiceConfigurationElement)element).Name;
        }
    }
}
