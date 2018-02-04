using System.Security.Principal;

namespace Lavie.Infrastructure
{
    public class UserIdentity : IIdentity
    {
        public UserIdentity(string authenticationType, bool isAuthenticated, string name)
        {
            AuthenticationType = authenticationType;
            IsAuthenticated = isAuthenticated;
            Name = name;
        }

        #region IIdentity Members

        public string AuthenticationType { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string Name { get; private set; }

        #endregion
    }
}