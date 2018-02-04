using System.Configuration;

namespace Lavie.Configuration
{
    public class LavieModuleConfigurationElementCollection : ConfigurationElementCollection
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
            return new LavieModuleConfigurationElement();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new LavieModuleConfigurationElement(elementName);
        }

        protected new LavieModuleConfigurationElement BaseGet(int index)
        {
            return (LavieModuleConfigurationElement)base.BaseGet(index);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LavieModuleConfigurationElement)element).Name;
        }
    }
}
