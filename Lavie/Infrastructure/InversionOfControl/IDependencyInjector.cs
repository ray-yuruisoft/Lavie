using System;
using System.Web.Mvc;

namespace Lavie.Infrastructure.InversionOfControl
{
    /// <summary>
    /// IoC/DI 容器
    /// </summary>
    public interface IDependencyInjector : IDependencyResolver
    {
        IDependencyInjector RegisterInstance<T>(T instance) where T:class;
        IDependencyInjector RegisterInstance<TFrom, TTo>() where TTo : class,TFrom;
        IDependencyInjector RegisterInstance(Type from, Type to);
        IDependencyInjector RegisterType<TFrom, TTo>() where TTo :class,TFrom;
        IDependencyInjector RegisterType(Type from, Type to);
        T Inject<T>(T existing);

        bool IsRegistered<T>();
        bool IsRegistered(Type type);
        bool CanResolve<T>();
        bool CanResolve(Type type);
    }
}
