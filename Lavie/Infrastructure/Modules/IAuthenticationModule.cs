using System.Threading.Tasks;
using System.Web.Routing;
using Lavie.Models;

namespace Lavie.Infrastructure
{
    public interface IAuthenticationModule : IModule
    {
        //Task<IUser> GetUser(RequestContext context);
        IUser GetUser(RequestContext context);
        string GetRegistrationUrl(RequestContext context);
        string GetLoginUrl(RequestContext context);
        string GetLogoutUrl(RequestContext context);
    }
}
