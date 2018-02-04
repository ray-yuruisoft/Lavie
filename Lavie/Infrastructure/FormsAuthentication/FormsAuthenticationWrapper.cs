using System.Web.Security;
using Lavie.Infrastructure.FormsAuthentication.Extensions;
using SystemFormsAuthentication = System.Web.Security.FormsAuthentication;
using System;
using System.Collections.Specialized;

namespace Lavie.Infrastructure.FormsAuthentication
{
    public class FormsAuthenticationWrapper : IFormsAuthentication
    {
        /// <summary>
        /// 设置认证Cookie
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="createPersistentCookie">是否创建持久Cookie</param>
        public void SetAuthCookie(string username, bool createPersistentCookie)
        {
            SystemFormsAuthentication.SetAuthCookie(username, createPersistentCookie);
        }
        /// <summary>
        /// 设置带自定义数据的认证Cookie
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="userData">自定义数据</param>
        /// <param name="createPersistentCookie">是否创建持久Cookie</param>
        public void SetAuthCookie(string username, bool createPersistentCookie, NameValueCollection userData, DateTime? expiresOn = null)
        {
            new SystemFormsAuthentication().SetAuthCookie(username, createPersistentCookie, userData, expiresOn); 
        }

        /// <summary>
        /// 注销
        /// </summary>
        public void SignOut()
        {
            SystemFormsAuthentication.SignOut();
        }
    }
}
