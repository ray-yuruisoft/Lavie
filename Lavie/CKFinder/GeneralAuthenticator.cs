using CKSource.CKFinder.Connector.Core;
using CKSource.CKFinder.Connector.Core.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lavie.CKFinder
{
    public class GeneralAuthenticator : IAuthenticator
    {
        public Task<IUser> AuthenticateAsync(ICommandRequest commandRequest, CancellationToken cancellationToken)
        {
            var user = new User(commandRequest.Principal.Identity.IsAuthenticated, null);
            return Task.FromResult((IUser)user);
        }
    }
}
