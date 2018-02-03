using Lavie.Extensions;
using Lavie.Infrastructure.InversionOfControl;
using Lavie.InversionOfControl.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.InversionOfControl
{
    /// <summary>
    /// DI/IoC容器工厂
    /// </summary>
    public class DependencyInjectorFactory : IDependencyInjectorFactory
    {
        #region Private Members

        /// <summary>
        /// DI/IoC容器类型
        /// </summary>
        private readonly Type _resolverType;

        #endregion

        #region Constructor

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DependencyInjectorFactory()
            : this(ConfigurationManager.AppSettings["dependencyInjectorType"])
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resolverTypeName">DI/IoC容器类型名称</param>
        public DependencyInjectorFactory(string resolverTypeName)
        {
            if (resolverTypeName.IsNullOrWhiteSpace())
                _resolverType = typeof(UnityDependencyInjector);
            else
                _resolverType = Type.GetType(resolverTypeName, true, true);
        }

        #endregion

        /// <summary>
        /// 创建DI/IoC容器
        /// </summary>
        /// <returns>DI/IoC容器</returns>
        public IDependencyInjector CreateDependencyInjector()
        {
            return Activator.CreateInstance(_resolverType) as IDependencyInjector;
        }
    }
}
