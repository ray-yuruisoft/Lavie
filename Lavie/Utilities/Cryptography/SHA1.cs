using System;
using System.Security.Cryptography;
using System.Text;
using Lavie.Extensions;
using Lavie.Utilities.Exceptions;

namespace Lavie.Utilities.Cryptography
{
    /// <summary>
    /// SHA1加密算法
    /// </summary>
    public static class SHA1
    {
        public static String Encrypt(String rawString)
        {
            Guard.ArgumentNotNullOrEmpty(rawString, "rawString");

            return Convert.ToBase64String(EncryptToByteArray(rawString));
        }
        public static Byte[] EncryptToByteArray(String rawString)
        {
            Guard.ArgumentNotNullOrEmpty(rawString, "rawString");

            Byte[] salted = Encoding.UTF8.GetBytes(rawString);
            System.Security.Cryptography.SHA1 hasher = new SHA1Managed();
            Byte[] hashed = hasher.ComputeHash(salted);
            return hashed;
        }
    }
}
