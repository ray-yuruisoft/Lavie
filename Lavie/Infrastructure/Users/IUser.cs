using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Infrastructure
{
    public interface IUser : IPrincipal
    {
        bool IsAuthenticated { get; }
        NameValueCollection AuthenticationValues { get; }
        string Name { get; }
        T As<T>() where T : class, IUser;
        bool IsInGroup(string name);
        bool HasPermission(string name);
    }
}
