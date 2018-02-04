using System;
using System.Web;
using Lavie.Infrastructure;
using Lavie.Extensions.Object;
using Lavie.Utilities.Exceptions;

namespace Lavie.Extensions
{
    public static class HttpCookieCollectionExtensions
    {
        private const string anonymousUserCookieName = "anon";
        public static void ClearAnonymousUser(this HttpCookieCollection cookies)
        {
            cookies.RemoveCookie(anonymousUserCookieName);
        }
        public static UserAnonymous GetAnonymousUser(this HttpCookieCollection cookies)
        {
            HttpCookie cookie = cookies[anonymousUserCookieName];

            if (cookie != null)
            {
                try
                {
                    UserCookieProxy userCookieProxy = typeof(UserCookieProxy).FromJson(cookie.Value) as UserCookieProxy;

                    if (userCookieProxy != null)
                        return userCookieProxy.ToUserAnonymous();
                }
                catch { }
            }

            return new UserAnonymous();
        }
        public static void SetAnonymousUser(this HttpCookieCollection cookies, UserAnonymous user)
        {
            Guard.ArgumentNotNull(cookies, "cookies");
            Guard.ArgumentNotNull(user, "user");

            HttpCookie cookie = new HttpCookie(anonymousUserCookieName, new UserCookieProxy(user).ToJson());
            cookie.Expires = DateTime.Now.AddDays(14);
            cookies.Remove(anonymousUserCookieName);
            cookies.Add(cookie);
        }

        public static string GetString(this HttpCookieCollection cookies,string cookieName)
        {
            Guard.ArgumentNotNull(cookies, "cookies");
            Guard.ArgumentNotNullOrEmpty(cookieName, "cookieName");

            HttpCookie cookie = cookies[cookieName];
            if (cookie != null)
                return cookie.Value;
            return null;

        }
        public static T ConverTo<T>(this HttpCookieCollection cookies, string cookieName)
        {
            Guard.ArgumentNotNull(cookies, "cookies");
            Guard.ArgumentNotNullOrEmpty(cookieName, "cookieName");

            string value = cookies.GetString(cookieName);
            if (value == null)
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }
        public static void AddCookie(this HttpCookieCollection cookies, object obj, string cookieName,string domain=null, DateTime? expries=null)
        {
            Guard.ArgumentNotNull(cookies, "cookies");
            Guard.ArgumentNotNull(obj, "obj");
            Guard.ArgumentNotNullOrEmpty(cookieName, "cookieName");

            HttpCookie cookie = new HttpCookie(cookieName, obj.ToString());
            if (!domain.IsNullOrWhiteSpace())
                cookie.Domain = domain;
            if (expries.HasValue)
                cookie.Expires = expries.Value;

            cookies.Add(cookie);
        }
        public static void RemoveCookie(this HttpCookieCollection cookies,string cookieName)
        {
            Guard.ArgumentNotNullOrEmpty(cookieName, "cookieName");
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = new DateTime(1900, 1, 1);
            cookies.Add(cookie);
        }
    }
}
