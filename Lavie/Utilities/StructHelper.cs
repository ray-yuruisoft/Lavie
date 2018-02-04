﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Lavie.Utilities
{
    public class StructHelper
    {
        /// <summary>
        /// 结构体转Byte数组
        /// </summary>
        /// <param name="structObj">要转换的结构体</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] StructToBytes<T>(T structObj) where T:struct 
        {
            //得到结构体的大小
            Int32 size = Marshal.SizeOf(structObj);
            //创建byte数组
            var bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }

        /// <summary>
        /// Byte数组转结构体
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <param name="type">结构体类型</param>
        /// <returns>转换后的结构体</returns>
        public static T BytesToStuct<T>(byte[] bytes) where T:struct
        {
            Type type = typeof (T);
            //得到结构体的大小
            Int32 size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                throw new ArgumentException(@"bytes的长度不足", "bytes");
                //返回空
                //return default(T);
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return (T)obj;
        }

        /// <summary>
        /// Byte数组转结构体
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <param name="type">结构体类型</param>
        /// <param name="offset"></param>
        /// <returns>转换后的结构体</returns>
        public static T BytesToStuct<T>(byte[] bytes,Int32 offset) where T : struct
        {
            Type type = typeof(T);
            //得到结构体的大小
            Int32 size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length - offset)
            {
                throw new ArgumentException(@"bytes的长度不足", "bytes");
                //返回空
                //return default(T);
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, offset, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return (T)obj;
        }

        public static String ByteArray2String(IEnumerable<Byte> buffer)
        {
            var sb = new StringBuilder();
            foreach (byte item in buffer)
            {
                sb.AppendFormat("{0:X2}", item);
            }
            return sb.ToString();
        }

    }
}
