using System.Configuration;

namespace Lavie.Configuration
{
    /// <summary>
    /// Lavie自定义配置节点
    /// </summary>
    public class LavieConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// 数据库链接字符串集合
        /// </summary>
        [ConfigurationProperty("connectionStrings")]
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                return (ConnectionStringSettingsCollection)this["connectionStrings"];
            }
            set
            {
                this["connectionStrings"] = value;
            }
        }
        /// <summary>
        /// 数据源提供器集合
        /// </summary>
        [ConfigurationProperty("dataProviders")]
        public LavieDataProviderConfigurationElementCollection Providers
        {
            get
            {
                return (LavieDataProviderConfigurationElementCollection)this["dataProviders"];
            }
            set
            {
                this["dataProviders"] = value;
            }
        }
        /// <summary>
        /// Module(模块)集
        /// </summary>
        [ConfigurationProperty("modules")]
        public LavieModuleConfigurationElementCollection Modules
        {
            get
            {
                return (LavieModuleConfigurationElementCollection)this["modules"];
            }
            set
            {
                this["modules"] = value;
            }
        }
        /// <summary>
        /// 后台服务(模块)集
        /// </summary>
        [ConfigurationProperty("backgroundServices")]
        public LavieBackgroundServiceConfigurationElementCollection BackgroundServices
        {
            get
            {
                return (LavieBackgroundServiceConfigurationElementCollection)this["backgroundServices"];
            }
            set
            {
                this["backgroundServices"] = value;
            }
        }
    }
}
