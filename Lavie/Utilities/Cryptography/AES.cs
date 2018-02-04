﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Lavie.Extensions;
using Lavie.Utilities.Exceptions;

namespace Lavie.Utilities.Cryptography
{
    /// <summary>
    /// AES加密解密算法
    /// </summary>
    public static class AES
    {
        private const String DefaultKey = "$uqn.atko@5!7%8*"; // 16 Bytes

        private static readonly byte[] DefaultIV = { 0x96, 0x47, 0x22, 0x18, 0x69, 0xCB, 0xDA, 0xFE, 0xAC, 0xBE, 0x85, 0x71, 0x23, 0x18, 0x39, 0x67 };

        #region 加密

        public static Byte[] EncryptFromByteArrayToByteArray(Byte[] inputByteArray, PaddingMode padding, CipherMode mode, String key = null)
        {
            Guard.ArgumentNotNull(inputByteArray, "inputByteArray");

            //分组加密算法
            var aes = Rijndael.Create();
            aes.Padding = padding;
            aes.Mode = mode;
            aes.Key = EnsureKey(key);
            aes.IV = DefaultIV;

            Byte[] cipherBytes = null;        
            using (var ms = new MemoryStream())
            {
                var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cipherBytes = ms.ToArray();
            }
            return cipherBytes;
        }

        /// <summary>
        /// 字节数组 - 字节数组
        /// </summary>
        /// <param name="inputByteArray"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Byte[] EncryptFromByteArrayToByteArray(Byte[] inputByteArray, String key = null)
        {
            Guard.ArgumentNotNull(inputByteArray, "inputByteArray");

            //分组加密算法
            var aes = Rijndael.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = EnsureKey(key);
            aes.IV = DefaultIV;

            Byte[] cipherBytes = null;
            using (var ms = new MemoryStream())
            {
                var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cipherBytes = ms.ToArray();
            }

            return cipherBytes;
        }

        /// <summary>
        /// 字符串 - 字节数组
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Byte[] EncryptFromStringToByteArray(String encryptString, String key = null)
        {
            Byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            return EncryptFromByteArrayToByteArray(inputByteArray, key);
        }

        /// <summary>
        /// 字符串 - 16进制
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String EncryptFromStringToHex(String encryptString, String key = null)
        {
            Byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray, key);
            var sb = new StringBuilder();
            foreach (var item in encryptBuffer)
            {
                sb.AppendFormat("{0:X2}", item);
            }
            return sb.ToString();
            //return BitConverter.ToString(encryptBuffer).Replace("-", "");
        }

        /// <summary>
        /// 字符串 - Base64
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String EncryptFromStringToBase64(String encryptString, String key = null)
        {
            Byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray, key);
            return Convert.ToBase64String(encryptBuffer);

        }


        #endregion

        #region 解密

        /// <summary>
        /// 字节数组 - 字节数组
        /// </summary>
        /// <param name="inputByteArray"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Byte[] DecryptFromByteArrayToByteArray(Byte[] inputByteArray, String key = null)
        {
            Guard.ArgumentNotNull(inputByteArray, "inputByteArray");

            var aes = Rijndael.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = EnsureKey(key);
            aes.IV = DefaultIV;

            var decryptBytes = new Byte[inputByteArray.Length];
            MemoryStream ms = null;
            try
            {
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cs.Read(decryptBytes, 0, decryptBytes.Length);
                }
            }
            finally {
                if(ms != null)
                {
                    ms.Dispose();
                }
            }

            return decryptBytes;
        }

        public static Byte[] DecryptFromHexToByteArray(String decryptString, String key = null)
        {
            Guard.ArgumentNotNullOrEmpty(decryptString, "decryptString");

            var buffer = ByteArrayFromHexString(decryptString);
            if (buffer.IsNullOrEmpty())
            {
                return new Byte[0];
            }
            return DecryptFromByteArrayToByteArray(buffer, key);
        }

        public static String DecryptFromHexToString(String decryptString, String key = null)
        {
            var buffer = ByteArrayFromHexString(decryptString);
            if (buffer.IsNullOrEmpty())
            {
                return String.Empty;
            }

            return Encoding.UTF8.GetString(DecryptFromByteArrayToByteArray(buffer, key)).Replace("\0", "");
        }

        public static Byte[] DecryptFromBase64ToByteArray(String decryptString, String key = null)
        {
            Guard.ArgumentNotNullOrEmpty(decryptString, "decryptString");

            var buffer = Convert.FromBase64String(decryptString);
            if (buffer.IsNullOrEmpty())
            {
                return new Byte[0];
            }
            return DecryptFromByteArrayToByteArray(buffer, key);
        }

        #endregion

        #region Private Methods
        private static Byte[] ByteArrayFromHexString(String hexString)
        {
            if (hexString.IsNullOrWhiteSpace() || hexString.Length % 2 != 0)
            {
                return new Byte[0];
            }
            var buffer = new Byte[hexString.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return buffer;
        }

        private static Byte[] EnsureKey(String key)
        {
            if (key != null)
            {
                var keyBytes = Encoding.UTF8.GetBytes(key);
                if (keyBytes.Length < 16)
                    throw new ArgumentOutOfRangeException("key", "key的经过UTF8编码后的长度至少需要16个字节");

                return keyBytes.Length == 16 ? keyBytes : keyBytes.SubArray(16);
            }
            return Encoding.UTF8.GetBytes(AES.DefaultKey);
        }
        #endregion
    }
}
