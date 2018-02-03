using Lavie.Configuration;
using Lavie.Configuration.Extensions;
using Lavie.Extensions;
using Lavie.Infrastructure.InversionOfControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;

namespace Lavie.Infrastructure
{
    public class ModuleRegistry : IModuleRegistry
    {
        private readonly IDependencyInjector _dependencyInjector;
        private readonly List<IModule> _modules;

        public ModuleRegistry(IDependencyInjector dependencyInjector)
        {
            this._dependencyInjector = dependencyInjector;
            this._modules = new List<IModule>();
        }

        #region IModulesLoaded Members

        public IModule Load(LavieConfigurationSection config, LavieModuleConfigurationElement module)
        {
            // 如果module为null或module未启用
            if (module == null || !module.Enabled) return null;

            // 遍历配置文件中的dataProviders节点下的元素
            foreach (LavieDataProviderConfigurationElement dataProvider in config.Providers)
            {
                // 如果dataProviders节点下的元素的name属性的值，和module的dataProvider属性值相等
                // 创建相关IXoohooDataProvider实例并执行ConfigureProvider方法以配置数据代理相关的信息
                if (dataProvider.Name == module.DataProvider)
                {
                    Type dataProviderType = Type.GetType(dataProvider.Type);

                    if (dataProviderType == null)
                        throw new TypeLoadException("Could not load type '{0}'.".FormatWith(dataProvider.Type));

                    var dataProviderInstance = _dependencyInjector.GetService(dataProviderType) as IDataProvider;

                    if (dataProviderInstance != null)
                        dataProviderInstance.ConfigureProvider(config, dataProvider, _dependencyInjector);

                    break;
                }
            }

            // 根据module的type属性在引用程序集中获取类型
            Type type = Type.GetType(module.Type);

            // 如果在引用程序集中无法获取类型
            if (type == null && BuildManager.CodeAssemblies != null)
            {
                foreach (var assembly in BuildManager.CodeAssemblies.OfType<Assembly>())
                {
                    type = assembly.GetExportedTypes().FirstOrDefault(t => t.FullName == module.Type);

                    if (type != null) break;
                }
            }

            // 如果从以上两种途径都无法获取类型，抛出异常
            if (type == null)
                throw new TypeLoadException("Could not load type '{0}'.".FormatWith(module.Type));

            // 根据类型获取IModule实例
            var moduleInstance = _dependencyInjector.GetService(type) as IModule;

            if (moduleInstance == null)
                throw new NullReferenceException("{0} does not implement the IModule interface.".FormatWith(type));

            // 获取模块设置
            moduleInstance.Settings = module.Settings.ToAppSettingsHelper();

            // 将IModule实例放入容器中
            _modules.Add(moduleInstance);

            // 返回IModule实例
            return moduleInstance;
        }

        public IEnumerable<IModule> GetModules()
        {
            return GetModules<IModule>();
        }

        public IEnumerable<T> GetModules<T>() where T : IModule
        {
            return _modules.OfType<T>();
        }

        public void UnloadModules()
        {
            foreach (IModule module in _modules)
                module.Unload();

            _modules.Clear();
        }

        #endregion
    }
}
