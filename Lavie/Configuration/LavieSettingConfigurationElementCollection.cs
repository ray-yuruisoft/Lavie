using System.Configuration;

namespace Lavie.Configuration
{
    public class LavieSettingConfigurationElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new LavieSettingConfigurationElement();
        }

        protected new LavieSettingConfigurationElement BaseGet(int index)
        {
            return (LavieSettingConfigurationElement)base.BaseGet(index);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LavieSettingConfigurationElement)element).Name;
        }
    }
}
