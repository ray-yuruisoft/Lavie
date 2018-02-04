using System;
using System.Net;
using System.Web;
using Lavie.Utilities.Cryptography;

namespace Lavie.Extensions
{
    public static class HttpRequestBaseExtensions
    {
        public static string GenerateAntiForgeryToken(this HttpRequestBase request, string key, string salt)
        {
            return MD5.EncryptFromStringToBase64(key + salt + request.UserAgent);
        }

        public static DateTime? IfModifiedSince(this HttpRequestBase request)
        {
            string ifModifiedSinceValue = request.Headers["If-Modified-Since"];
            DateTime ifModifiedSince;

            if (!string.IsNullOrEmpty(ifModifiedSinceValue) && DateTime.TryParse(ifModifiedSinceValue, out ifModifiedSince))
                return ifModifiedSince;

            return null;
        }

    }
}