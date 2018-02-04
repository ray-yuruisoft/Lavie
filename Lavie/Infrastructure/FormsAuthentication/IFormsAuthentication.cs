using System;
using System.Collections.Specialized;

namespace Lavie.Infrastructure.FormsAuthentication
{
    public interface IFormsAuthentication
    {
        /// <summary>
        /// 设置认证Cookie
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="createPersistentCookie">是否创建持久Cookie</param>
        void SetAuthCookie(string username, bool createPersistentCookie);
        /// <summary>
        /// 设置带自定义数据的认证Cookie
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="userData">自定义数据</param>
        /// <param name="createPersistentCookie">是否创建持久Cookie</param>
        void SetAuthCookie(string username, bool createPersistentCookie, NameValueCollection userData, DateTime? expiresOn = null);

        /// <summary>
        /// 注销
        /// </summary>
        void SignOut();
    }
}
