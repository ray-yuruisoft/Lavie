﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Lavie.Extensions;

namespace Lavie.Utilities.Cryptography
{
    /// <summary>
    /// MD5加密算法
    /// </summary>
    public static class MD5
    {
        public static Byte[] EncryptFromByteArrayToByteArray(Byte[] inputByteArray)
        {
            //MD5 md5 = System.Security.Cryptography.MD5.Create();
            var provider = new MD5CryptoServiceProvider();
            return provider.ComputeHash(inputByteArray);
        }
        public static Byte[] EncryptFromStringToByteArray(String encryptString)
        {
            if (encryptString.IsNullOrWhiteSpace()) return null;

            Byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            return EncryptFromByteArrayToByteArray(inputByteArray);
        }
        public static String EncryptFromByteArrayToBase64(Byte[] inputByteArray)
        {
            Byte[] encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);
            return Convert.ToBase64String(encryptBuffer);
        }
        public static String EncryptFromStringToBase64(String encryptString)
        {
            if (encryptString.IsNullOrWhiteSpace()) return null;

            Byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            Byte[] encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);
            return Convert.ToBase64String(encryptBuffer);
        }
        public static String EncryptFromByteArrayToHex(Byte[] inputByteArray)
        {
            Byte[] encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);
            return ByteArrayToHex(encryptBuffer);
        }
        public static String EncryptFromStringToHex(String encryptString)
        {
            Byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            Byte[] encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);
            return ByteArrayToHex(encryptBuffer);
        }

        private static String ByteArrayToHex(IEnumerable<byte> inputByteArray)
        {
            var sb = new StringBuilder();
            foreach (Byte item in inputByteArray)
            {
                sb.AppendFormat("{0:X2}", item);
            }
            return sb.ToString();
        }
    }
}
