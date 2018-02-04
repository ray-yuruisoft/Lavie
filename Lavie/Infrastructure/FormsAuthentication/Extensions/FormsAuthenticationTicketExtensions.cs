using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;

namespace Lavie.Infrastructure.FormsAuthentication.Extensions
{
    public static class FormsAuthenticationTicketExtensions
    {
        public static NameValueCollection GetStructuredUserData(this FormsAuthenticationTicket ticket)
        {
            if (ticket == null) return new NameValueCollection();

            return !string.IsNullOrEmpty(ticket.UserData)
                ? HttpUtility.ParseQueryString(ticket.UserData)
                : null;
        }
    }


}
