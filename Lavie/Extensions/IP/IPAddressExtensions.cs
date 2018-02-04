using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace Lavie.Extensions.IP
{
    public static class IPAddressExtensions
    {
        /// <summary>
        /// IPAddress 转 Int32 (可能产生负数)
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static int ToInt(this IPAddress ip)
        {
            int x = 3;
            int v = 0;
            var bytes = ip.GetAddressBytes();
            foreach (byte f in bytes)
            {
                v += (int)f << 8 * x--;
            }
            return v;
        }

        /// <summary>
        /// IPAddress 转 Int64
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long ToInt64(this IPAddress ip)
        {
            int x = 3;
            long v = 0;
            var bytes = ip.GetAddressBytes();
            foreach (byte f in bytes)
            {
                v += (long)f << 8 * x--;
            }
            return v;
        }

        /// <summary>
        /// Int32 转 IPAddress (注意：由于是基于int的扩展方法，故会造成一定的污染)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static IPAddress ToIP(this int ip)
        {

            var b = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                b[3 - i] = (byte)(ip >> 8 * i & 255);
            }
            return new IPAddress(b);
        }

        /// <summary>
        /// Int64 转 IPAddress (注意：由于是基于Int64的扩展方法，故会造成一定的污染)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static IPAddress ToIP(this long ip)
        {

            var b = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                b[3 - i] = (byte)(ip >> 8 * i & 255);
            }
            return new IPAddress(b);
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetIPString(this HttpRequestBase request)
        {
            string ip;
            if (request.ServerVariables["HTTP_VIA"] != null)
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            else
                ip = request.ServerVariables["REMOTE_ADDR"];
            if (ip.IsNullOrWhiteSpace())
            {
                ip = request.UserHostAddress;
            }
            return ip;

        }
        public static IPAddress GetIPAddress(this HttpRequestBase request)
        {
            if (request == null || request.UserHostAddress == null) return null;

            IPAddress address;
            if (!IPAddress.TryParse(request.UserHostAddress, out address))
            {
                address = null;
            }
            return address;
        }

        private static readonly Regex IPRegex = new Regex(@"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$", RegexOptions.Compiled);
        /// <summary>
        /// 是否ip格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIPAddress(this string ip)
        {
            if (ip.IsNullOrWhiteSpace() || ip.Length < 7 || ip.Length > 15) return false;
            return IPRegex.IsMatch(ip);
        }
    }
}
