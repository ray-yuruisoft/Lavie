using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Collections.Specialized;
using SystemFormsAuthentication = System.Web.Security.FormsAuthentication;

namespace Lavie.Infrastructure.FormsAuthentication.Extensions
{
    public static class FormsAuthenticationExtensions
    {
        public static void SetAuthCookie(this SystemFormsAuthentication formsAuthentication, string userName, bool createPersistentCookie, NameValueCollection userData, DateTime? expiresOn = null)
        {
            var response = new HttpResponseWrapper(HttpContext.Current.Response);
            SetAuthCookie(formsAuthentication, response, userName, createPersistentCookie, userData, expiresOn);
        }

        public static void SetAuthCookie(this SystemFormsAuthentication formsAuthentication, HttpResponseBase response, string userName, bool createPersistentCookie, NameValueCollection userData, DateTime? expiresOn = null)
        {
            var encodedUserData = EncodeAsQueryString(userData);
            SetAuthCookie(formsAuthentication, response, userName, createPersistentCookie, encodedUserData, expiresOn);
        }

        public static void SetAuthCookie(this SystemFormsAuthentication formsAuthentication, string userName, bool createPersistentCookie, string userData, DateTime? expiresOn = null)
        {
            var response = new HttpResponseWrapper(HttpContext.Current.Response);
            SetAuthCookie(formsAuthentication, response, userName, createPersistentCookie, userData, expiresOn);
        }

        public static void SetAuthCookie(this SystemFormsAuthentication formsAuthentication, HttpResponseBase response, string userName, bool createPersistentCookie, string userData, DateTime? expiresOn = null)
        {
            var cookie = SystemFormsAuthentication.GetAuthCookie(userName, createPersistentCookie);
            var ticket = SystemFormsAuthentication.Decrypt(cookie.Value);
            var newTicket = new FormsAuthenticationTicket(
                ticket.Version,
                ticket.Name,
                ticket.IssueDate,
                expiresOn ?? ticket.Expiration,
                ticket.IsPersistent,
                userData,
                ticket.CookiePath);
            cookie.Value = SystemFormsAuthentication.Encrypt(newTicket);
            if (newTicket.IsPersistent)
                cookie.Expires = newTicket.Expiration;
            cookie.HttpOnly = true;
            response.Cookies.Set(cookie);
        }

        static string EncodeAsQueryString(NameValueCollection userData)
        {
            var targetCollection = HttpUtility.ParseQueryString(string.Empty);
            targetCollection.Add(userData);
            return targetCollection.ToString();
        }
    }

}
