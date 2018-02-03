using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lavie.Infrastructure.InversionOfControl
{
    public interface IDependencyInjector : IDependencyResolver
    {
        IDependencyInjector RegisterInstance<T>(T instance) where T : class;
        IDependencyInjector RegisterInstance<TFrom, TTo>() where TTo : class, TFrom;
        IDependencyInjector RegisterInstance(Type from, Type to);
        IDependencyInjector RegisterType<TFrom, TTo>() where TTo : class, TFrom;
        IDependencyInjector RegisterType(Type from, Type to);
        T Inject<T>(T existing);

        bool IsRegistered<T>();
        bool IsRegistered(Type type);
        bool CanResolve<T>();
        bool CanResolve(Type type);
    }
}
