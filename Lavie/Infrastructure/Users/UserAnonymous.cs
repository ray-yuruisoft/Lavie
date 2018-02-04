using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Collections.Specialized;

namespace Lavie.Infrastructure
{
    public class UserAnonymous : IUser
    {
        private NameValueCollection _authenticationValues = new NameValueCollection();
        public UserAnonymous()
        {
            Identity = new UserIdentity(null, false, null);
        }

        public UserAnonymous(string name, string email)
        {
            Identity = new UserIdentity(null, false, name);
            Email = email;
        }

        public string Email { get; private set; }

        #region IUser Members

        public bool IsAuthenticated { get { return Identity.IsAuthenticated; } }
        public string Name { get { return Identity.Name; } }
        public NameValueCollection AuthenticationValues { get { return _authenticationValues; } }

        public T As<T>() where T : class, IUser
        {
            return null;
        }

        public bool IsInGroup(string name)
        {
            return false;
        }
        public bool HasPermission(string name)
        {
            return false;
        }
        #endregion

        #region IPrincipal Members

        public IIdentity Identity { get; private set; }

        public bool IsInRole(string role)
        {
            return false;
        }
        #endregion
    }
}