using System.Web.Mvc;

namespace Lavie.Infrastructure.InversionOfControl
{
    public interface IDependencyInjectorFactory
    {
        IDependencyInjector CreateDependencyInjector();
    }
}
