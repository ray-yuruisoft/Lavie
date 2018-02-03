using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Configuration
{
    public class LavieDataProviderConfigurationElementCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("defaultConnectionString")]
        public string DefaultConnectionString
        {
            get
            {
                return (string)this["defaultConnectionString"];
            }
            set
            {
                this["defaultConnectionString"] = value;
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new LavieDataProviderConfigurationElement();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new LavieDataProviderConfigurationElement(elementName);
        }

        protected new LavieDataProviderConfigurationElement BaseGet(int index)
        {
            return (LavieDataProviderConfigurationElement)base.BaseGet(index);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LavieDataProviderConfigurationElement)element).Name;
        }
    }
}
