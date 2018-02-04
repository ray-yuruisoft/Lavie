using System;
using System.Web.Mvc;
using Xoohoo.Extensions;
using System.Configuration;

namespace Xoohoo.Infrastructure
{
    /// <summary>
    /// DI/IoC容器工厂
    /// </summary>
    public class DependencyResolverFactory : IDependencyResolverFactory
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
        public DependencyResolverFactory()
            : this(ConfigurationManager.AppSettings["dependencyResolverType"])
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resolverTypeName">DI/IoC容器类型名称</param>
        public DependencyResolverFactory(string resolverTypeName)
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
        public IDependencyResolver CreateDependencyResolver()
        {
            return Activator.CreateInstance(_resolverType) as IDependencyResolver;

        }
    }
}
