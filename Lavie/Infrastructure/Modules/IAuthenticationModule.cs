using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Lavie.Infrastructure.Modules
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
