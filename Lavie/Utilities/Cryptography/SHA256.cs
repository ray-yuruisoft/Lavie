﻿using System;
using System.Security.Cryptography;
using System.Text;
using Lavie.Extensions;
using Lavie.Utilities.Exceptions;

namespace Lavie.Utilities.Cryptography
{
    /// <summary>
    /// SHA256加密算法
    /// </summary>
    public static class SHA256
    {
        public static String Encrypt(String rawString, String salt)
        {
            Guard.ArgumentNotNullOrEmpty(rawString, "rawString");
            Guard.ArgumentNotNullOrEmpty(salt, "salt");

            return Convert.ToBase64String(EncryptToByteArray(rawString, salt));
        }
        public static Byte[] EncryptToByteArray(String rawString, String salt)
        {
            Guard.ArgumentNotNullOrEmpty(rawString, "rawString");
            Guard.ArgumentNotNullOrEmpty(salt, "salt");
            Byte[] salted = Encoding.UTF8.GetBytes(String.Concat(rawString, salt));
            System.Security.Cryptography.SHA256 hasher = new SHA256Managed();
            Byte[] hashed = hasher.ComputeHash(salted);
            return hashed;
        }
    }
}
