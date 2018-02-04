﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Lavie.Extensions;
using Lavie.Utilities.Exceptions;

namespace Lavie.Utilities.Cryptography
{
    /// <summary>
    /// DES加密解密算法(默认采用的是ECB模式)
    /// </summary>
    public static class DES
    {
        private const String DefaultKey = "$uo@5%8*";

        #region 加密
        // 字节数组 -> 字节数组
        // 字节数组 -> Base64
        // 字符串   -> 字节数组
        // 字符串   -> Base64
        // 字符串   -> Base64(指定填充模式)
        // 字符串   -> Hex

        /// <summary>
        /// 核心方法
        /// </summary>
        /// <param name="inputByteArray"></param>
        /// <param name="mode"></param>
        /// <param name="paddingMode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Byte[] EncryptFromByteArrayToByteArray(Byte[] inputByteArray, CipherMode mode, PaddingMode paddingMode, String key = null)
        {
            Guard.ArgumentNotNull(inputByteArray, "inputByteArray");

            Byte[] keyBytes = EnsureKey(key);
            Byte[] keyIV = keyBytes;
            var provider = new DESCryptoServiceProvider();
            provider.Mode = mode;
            provider.Padding = paddingMode;
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return mStream.ToArray();
        }
        public static Byte[] EncryptFromByteArrayToByteArray(Byte[] inputByteArray, String key=null)
        {
            return EncryptFromByteArrayToByteArray(inputByteArray, CipherMode.ECB, PaddingMode.Zeros, key);
        }
        public static String EncryptFromByteArrayToBase64String(Byte[] inputByteArray, String key = null)
        {
            return Convert.ToBase64String(EncryptFromByteArrayToByteArray(inputByteArray, key));
        }
        public static Byte[] EncryptFromStringToByteArray(String encryptString, String key = null)
        {
            Guard.ArgumentNotNullOrEmpty(encryptString, "encryptString");

            Byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);

            return EncryptFromByteArrayToByteArray(inputByteArray, key);
        }
        public static String EncryptFromStringToBase64String(String encryptString, String key = null)
        {
            Guard.ArgumentNotNullOrEmpty(encryptString, "encryptString");

            Byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);

            return EncryptFromByteArrayToBase64String(inputByteArray,key);
        }
        public static String EncryptFromStringToBase64String(String encryptString, PaddingMode paddingMode, String key = null)
        {
            Guard.ArgumentNotNull(encryptString, "encryptString");

            Byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);

            Byte[] keyBytes = EnsureKey(key);
            Byte[] keyIV = keyBytes;

            var provider = new DESCryptoServiceProvider();
            provider.Mode = CipherMode.ECB;
            provider.Padding = paddingMode;
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }
        public static String EncryptFromStringToHex(String encryptString, String key = null)
        {
            var encryptBuffer = EncryptFromStringToByteArray(encryptString, key);
            return HexFromByteArray(encryptBuffer);
        }

        #endregion

        #region 解密

        /// <summary>
        /// 核心方法
        /// </summary>
        /// <param name="inputByteArray"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Byte[] DecryptFromByteArrayToByteArray(Byte[] inputByteArray, CipherMode mode, PaddingMode paddingMode, String key = null)
        {
            Guard.ArgumentNotNull(inputByteArray, "inputByteArray");

            Byte[] keyBytes = EnsureKey(key);
            Byte[] keyIV = keyBytes;
            var provider = new DESCryptoServiceProvider();
            provider.Mode = mode;
            provider.Padding = paddingMode;
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return mStream.ToArray();
        }
        public static Byte[] DecryptFromByteArrayToByteArray(Byte[] inputByteArray, String key = null)
        {
            return DecryptFromByteArrayToByteArray(inputByteArray, CipherMode.ECB, PaddingMode.Zeros);
        }
        public static String DecryptFromByteArrayToString(Byte[] inputByteArray, String key = null)
        {
            Guard.ArgumentNotNull(inputByteArray, "inputByteArray");

            return Encoding.UTF8.GetString(DecryptFromByteArrayToByteArray(inputByteArray, key));
        }
        public static Byte[] DecryptFromBase64StringToByteArray(String decryptBase64String, String key = null)
        {
            Guard.ArgumentNotNullOrEmpty(decryptBase64String, "decryptBase64String");

            Byte[] inputByteArray = Convert.FromBase64String(decryptBase64String);
            return DecryptFromByteArrayToByteArray(inputByteArray,key);
        }
        public static String DecryptFromBase64StringToString(String decryptBase64String, PaddingMode paddingMode, String key)
        {
            Guard.ArgumentNotNullOrEmpty(decryptBase64String, "decryptString");
            Byte[] inputByteArray = Convert.FromBase64String(decryptBase64String);
            Byte[] keyBytes = EnsureKey(key);
            Byte[] keyIV = keyBytes;
            var provider = new DESCryptoServiceProvider();
            provider.Mode = CipherMode.ECB;
            provider.Padding = paddingMode;
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        public static String DecryptFromHexToString(String decryptString, String key = null)
        {
            var decryptBuffer = new Byte[decryptString.Length / 2];
            for (int i = 0; i < decryptBuffer.Length; i++)
            {
                decryptBuffer[i] = (Byte)(Convert.ToByte(decryptString.Substring(i * 2, 2), 16));
            }
            return Encoding.UTF8.GetString(DecryptFromByteArrayToByteArray(decryptBuffer,key)).Replace("\0", "");
            
        }

        #endregion

        public static String HexFromByteArray(Byte[] value)
        {
            if (value == null || value.Length == 0) return String.Empty;
            var sb = new StringBuilder();
            foreach (var item in value)
            {
                sb.AppendFormat("{0:X2}", item);
            }
            return sb.ToString();
        }

        #region Private Methods

        private static Byte[] EnsureKey(String key)
        {
            if (key != null)
            {
                Byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                if (keyBytes.Length < 8)
                    throw new ArgumentOutOfRangeException("key", "key应该是经过UTF8编码后的长度至少需要8个字节的字符串");

                return keyBytes.Length == 8 ? keyBytes : keyBytes.SubArray(8);
            }
            return Encoding.UTF8.GetBytes(DES.DefaultKey);
        }


        #endregion
    }
}
