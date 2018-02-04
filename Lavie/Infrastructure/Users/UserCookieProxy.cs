using System.Runtime.Serialization;

namespace Lavie.Infrastructure
{
    [DataContract]
    public class UserCookieProxy
    {
        public UserCookieProxy()
        {
        }

        public UserCookieProxy(UserAnonymous user)
        {
            Name = user.Name;
            Email = user.Email;
        }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Email { get; set; }

        public UserAnonymous ToUserAnonymous()
        {
            return new UserAnonymous(Name, Email);
        }
    }
}