using Lavie.Utilities.Exceptions;
using Microsoft.International.Converters.PinYinConverter;
using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lavie.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex TagRegex = new Regex("<[^<>]*>", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex GuidRegex = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex SpaceRegex = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex NonWordCharsRegex = new Regex(@"[^\w]+", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex URLRegex = new Regex("(^|[^\\w'\"]|\\G)(?<uri>(?:https?|ftp)(?:&#58;|:)(?:&#47;&#47;|//)(?:[^./\\s'\"<)\\]]+\\.)+[^./\\s'\"<)\\]]+(?:(?:&#47;|/).*?)?)(?:[\\s\\.,\\)\\]'\"]?(?:\\s|\\.|\\)|\\]|,|<|$))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SubDirectoryRegex = new Regex(@"^[a-zA-Z0-9-_]+(/[a-zA-Z0-9-_]+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex VirtualDirectoryRegex = new Regex(@"^~(/[a-zA-Z0-9-_]+)+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region Guid相关

        /// <summary>
        /// 校验字符串是否是Guid格式
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGuid(this string source)
        {
            return !source.IsNullOrWhiteSpace() && GuidRegex.IsMatch(source);
        }
        /// <summary>
        /// 字符串转换为Guid
        /// </summary>
        /// <param name="source"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool GuidTryParse(this string source, out Guid result)
        {
            if (source.IsNullOrWhiteSpace())
            {
                result = Guid.Empty;
                return false;
            }

            try
            {
                result = new Guid(source);
                return true;
            }
            catch (FormatException)
            {
                result = Guid.Empty;
                return false;
            }
            catch (OverflowException)
            {
                result = Guid.Empty;
                return false;
            }
            catch
            {
                result = Guid.Empty;
                return false;
            }
        }

        #endregion

        #region 字符串清理

        /// <summary>
        /// 清除字符串的Html标签
        /// </summary>
        /// <example>
        /// <para>如：CleanHtmlTags(source,"a")则不清理a标签</para>
        /// <para>如：CleanHtmlTags(source,"a|br")则不清理a标签和br标签</para>
        /// </example>
        /// <param name="source">源字符串</param>
        /// <param name="exceptionPattern">要排除的标签</param>
        /// <returns></returns>
        public static string CleanHtmlTags(this string source, string exceptionPattern = null)
        {
            if (!string.IsNullOrEmpty(exceptionPattern))
                return
                    new Regex(string.Format("<(?!{0})[^<>]*>", exceptionPattern),
                              RegexOptions.Compiled | RegexOptions.Singleline).Replace(source, String.Empty);

            return TagRegex.Replace(source, String.Empty);
        }

        /// <summary>
        /// 清理空格，包括空格、制表符、换页符等
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string CleanWhitespace(this string source)
        {
            return SpaceRegex.Replace(source, String.Empty);
        }

        public static string CleanCssClassName(this string source)
        {
            return NonWordCharsRegex.Replace(source, "_").ToLower(System.Globalization.CultureInfo.CurrentCulture);
        }

        public static string CleanText(this string source)
        {
            if (source == null) return null;

            return AntiXss.HtmlEncode(source);
        }

        public static string CleanHtml(this string source)
        {
            //AntiXss library from Microsoft 
            //(http://antixss.codeplex.com)
            string encodedText = AntiXss.HtmlEncode(source);
            //convert line breaks into an html break tag
            return encodedText.Replace("&#13;&#10;", "<br />");
        }

        public static string CleanForQueryString(this string source)
        {
            return AntiXss.UrlEncode(source);
        }

        public static string CleanAttribute(this string source)
        {
            return AntiXss.HtmlAttributeEncode(source);
        }

        //TODO: (nheskew) rename to something more generic (CleanAttributeALittle?) because not everything needs
        // the cleaning power of CleanAttribute (everything should but AntiXss.HtmlAttributeEncode encodes 
        // *everyting* incl. white space :|) so attributes can get really long...but then my only current worry is around
        // the description meta tag. Attributes from untrusted sources *do* need the current CleanAttribute...
        public static string CleanHref(this string source)
        {
            return HttpUtility.HtmlAttributeEncode(source);
        }

        #endregion

        #region 字符串截取

        public static string Substr(this string source, int len)
        {
            return source.Substr(len, "...");
        }
        public static string Substr(this string source, int len, string att)
        {
            if (string.IsNullOrEmpty(source)) return String.Empty;

            att = att ?? String.Empty;

            var rChinese = new Regex(@"[\u4e00-\u9fa5]"); //验证中文
            var rEnglish = new Regex(@"^[A-Za-z0-9]+$");  //验证字母

            if (rChinese.IsMatch(source))
            {
                //中文
                return (source.Length > len) ? source.Substring(0, len) + att : source;
            }
            else if (rEnglish.IsMatch(source))
            {
                //英文
                return (source.Length > len * 2) ? source.Substring(0, len * 2) + att : source;
            }
            return (source.Length > len) ? source.Substring(0, len) + att : source;


        }

        #endregion

        #region 文件读写
        public static string GetFileText(this string virtualPath)
        {
            return virtualPath.GetFileText(new HttpContextWrapper(HttpContext.Current));
        }
        public static string GetFileText(this string virtualPath, HttpContextBase httpContext)
        {
            string path = httpContext.Server.MapPath(virtualPath);

            if (File.Exists(path))
                return File.ReadAllText(path);

            return null;
        }
        public static string GetFileName(this string virtualPath)
        {
            return Path.GetFileNameWithoutExtension(virtualPath);
        }
        public static void SaveFileText(this string virtualPath, string code)
        {
            virtualPath.SaveFileText(code, new HttpContextWrapper(HttpContext.Current));
        }
        public static void SaveFileText(this string virtualPath, string code, HttpContextBase httpContext)
        {
            string path = httpContext.Server.MapPath(virtualPath);

            if (path.IsFileWritable())
                File.WriteAllText(path, code);
        }
        public static bool IsFileWritable(this string filePath)
        {
            //TODO: (nheskew)still doesn't catch if write is explicitly denied
            return !filePath.IsNullOrWhiteSpace()
                && File.Exists(filePath)
                && (File.GetAttributes(filePath) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly;
            //&& SecurityManager.IsGranted(new FileIOPermission(FileIOPermissionAccess.Write, filePath));

        }
        public static bool IsFileWritable(this string virtualPath, HttpContextBase httpContext)
        {
            return IsFileWritable(httpContext.Server.MapPath(virtualPath));
        }
        public static DateTime? FileModifiedDate(this string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                return File.GetLastWriteTime(filePath);

            return null;
        }
        public static DateTime? FileModifiedDate(this string virtualPath, HttpContextBase httpContext)
        {
            return httpContext.Server.MapPath(virtualPath).FileModifiedDate();
        }
        #endregion

        #region 字符串空/null校验
        public static bool IsNullOrEmpty(this string source)
        {
            return String.IsNullOrEmpty(source);
        }
        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }
        #endregion

        #region 字符串格式化

        public static string FormatWith(this string format, object arg0)
        {
            return String.Format(format, arg0);
        }
        public static string FormatWith(this string format, object arg0, object arg1)
        {
            return String.Format(format, arg0, arg1);
        }
        public static string FormatWith(this string format, object arg0, object arg1, object arg2)
        {
            return String.Format(format, arg0, arg1, arg2);
        }
        public static string FormatWith(this string format, params object[] args)
        {
            return String.Format(format, args);
        }
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            return String.Format(provider, format, args);
        }

        #endregion

        #region 串联字符串集合

        public static string Join(this IEnumerable<String> source, string separator)
        {
            Guard.ArgumentNotNull(separator, "separator");

            var enumerable = source as string[] ?? source.ToArray();
            if (enumerable.IsNullOrEmpty()) return String.Empty;
            return String.Join(separator, enumerable);
        }

        public static string Join<T>(this IEnumerable<T> source, string separator, Func<T, String> selector)
        {
            Guard.ArgumentNotNull(separator, "separator");
            Guard.ArgumentNotNull(selector, "selector");

            var enumerable = source as T[] ?? source.ToArray();
            if (enumerable.IsNullOrEmpty()) return String.Empty;

            return String.Join(separator, enumerable.Select(selector));
        }

        #endregion

        #region 目录相关

        /// <summary>
        /// 确保目录为子目录
        /// </summary>
        /// <example>
        /// <para>~/目录名/ -> 目录名</para>
        /// <para>~/目录名/子目录名/ -> 目录名/子目录名</para>
        /// </example>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string EnsureSubFolder(this string path)
        {
            Guard.ArgumentNotNullOrEmpty(path, "path");

            if (path.StartsWith("~"))
                path = path.Substring(1, path.Length - 1);

            string[] pathNames = path.Split('/');
            path = string.Empty;

            foreach (string p in pathNames)
            {
                if (p != string.Empty)
                    path += p + "/";
            }
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }

        /// <summary>
        /// 确保目录为根目录
        /// </summary>
        /// <example>
        /// <para>目录名/ -> ~/目录名</para>
        /// <para>/目录名/子目录名/ -> ~/目录名/子目录名</para>
        /// </example>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string EnsureVirtualDirectory(this string path)
        {
            Guard.ArgumentNotNullOrEmpty(path, "path");

            if (!path.StartsWith("~/"))
            {
                if (!path.StartsWith("/"))
                    path = "/" + path;
                if (!path.StartsWith("~"))
                    path = "~" + path;
            }
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }

        /// <summary>
        /// 判断目录是否为子目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns><c>true</c>是子目录；<c>false</c>不是子目录</returns>
        public static bool IsSubDirectory(this string path)
        {
            return !path.IsNullOrWhiteSpace() && SubDirectoryRegex.IsMatch(path);
        }

        /// <summary>
        /// 判断目录是否为虚拟目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns><c>true</c>是虚拟目录；<c>false</c>不是虚拟目录</returns>
        public static bool IsVirtualDirectory(this string path)
        {
            return !path.IsNullOrWhiteSpace() && VirtualDirectoryRegex.IsMatch(path);
        }

        #endregion

        /// <summary>
        /// 字符串重复
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="times">重复次数</param>
        /// <returns></returns>
        public static string Repeat(this string source, int times)
        {
            if (String.IsNullOrEmpty(source) || times <= 0)
                return source;
            var sb = new StringBuilder();
            while (times > 0)
            {
                sb.Append(source);
                times--;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 简单过滤SQL语句
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public static string SqlFilter(this string sqlString)
        {
            if (sqlString == null)
                return null;
            sqlString = sqlString.ToLower();
            string words = "and|exec|insert|select|delete|update|chr|mid|master|or|truncate|char|declare|join";
            foreach (string i in words.Split('|'))
            {
                if ((sqlString.IndexOf(i + " ", StringComparison.Ordinal) > -1) || (sqlString.IndexOf(" " + i, StringComparison.Ordinal) > -1))
                {
                    sqlString = sqlString.Replace(i, String.Empty);
                }
            }
            return sqlString;
        }

        /// <summary>
        /// 将字符串转换为MvcHtmlString类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static MvcHtmlString ToMvcHtmlString(this string source)
        {
            return MvcHtmlString.Create(source);
        }

        /// <summary>
        /// 如果源对象为null，则返回null，否则返回其ToString方法返回值
        /// </summary>
        /// <param name="source">源对象</param>
        /// <returns>字符串</returns>
        public static string ToNullableString<T>(this T source) where T : class
        {
            return source == null ? null : source.ToString();
        }
        public static string ToEmptyableString<T>(this T source) where T : class
        {
            return source == null ? null : source.ToString();
        }
        public static string WithUrl(this string source, string url)
        {
            if (source == null)
                return null;

            return String.Format("<a href=\"{0}\">{1}</a>", url, source);
        }

        #region 拼音
        public static string ConvertToPinYin(this string source)
        {
            string pinYin = "";
            foreach (char item in source.ToCharArray())
            {
                if (ChineseChar.IsValidChar(item))
                {
                    ChineseChar cc = new ChineseChar(item);

                    //PYstr += string.Join("", cc.Pinyins.ToArray());
                    pinYin += cc.Pinyins[0].Substring(0, cc.Pinyins[0].Length - 1).ToLowerInvariant();
                    //PYstr += cc.Pinyins[0].Substring(0, cc.Pinyins[0].Length - 1).Substring(0, 1).ToLower();
                }
                else
                {
                    pinYin += item.ToString();
                }
            }
            return pinYin;
        }
        public static Tuple<string, string> ConvertToPinYinPY(this string source)
        {
            string pinYin = "";
            string py = "";
            foreach (char item in source.ToCharArray())
            {
                if (ChineseChar.IsValidChar(item))
                {
                    ChineseChar cc = new ChineseChar(item);
                    var pinYinString = cc.Pinyins[0].Substring(0, cc.Pinyins[0].Length - 1).ToLowerInvariant();
                    pinYin += pinYinString;
                    py += pinYinString.Substring(0, 1);
                }
                else
                {
                    var charString = item.ToString();
                    pinYin += charString;
                    py += charString;
                }
            }
            return new Tuple<string, string>(pinYin, py);
        }
        public static string ConvertToPY(this string source)
        {
            string py = "";
            foreach (char item in source.ToCharArray())
            {
                if (ChineseChar.IsValidChar(item))
                {
                    ChineseChar cc = new ChineseChar(item);
                    py += cc.Pinyins[0].Substring(0, 1).ToLowerInvariant();
                }
                else
                {
                    var charString = item.ToString();
                    py += charString;
                }
            }
            return py;
        }
        #endregion

    }
}
