using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Modules.Skinning.ViewEngines;
using Lavie.Html;
using Lavie.Extensions;

namespace Lavie.Modules.Skinning.Extensions
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// 构建文件路径
        /// </summary>
        /// <param name="urlHelper">UrlHelper</param>
        /// <param name="relativePath">相对路径</param>
        /// <param name="viewContext">ViewContext</param>
        /// <returns></returns>
        public static string FilePath(this UrlHelper urlHelper, string relativePath, ViewContext viewContext)
        {
            string path = relativePath;

            if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
                path = "/" + path;

            urlHelper.FilePath(viewContext, (ve, p) => ve.FindFile(p), ref path);

            return path;
        }

        /// <summary>
        /// 构建文件路径
        /// </summary>
        /// <param name="urlHelper">UrlHelper</param>
        /// <param name="viewContext">ViewContext</param>
        /// <param name="findFile">要查找的文件</param>
        /// <param name="path">要返回的路径</param>
        internal static void FilePath(this UrlHelper urlHelper, ViewContext viewContext, Func<ILavieViewEngine, string, FileEngineResult> findFile, ref string path)
        {
            List<string> searchedLocations = new List<string>(50);

            //key为LavieViewEngines的ViewData是通过ViewEnginesResultFilter设置的
            var viewEngines = viewContext.ViewData["LavieViewEngines"] as IEnumerable<ILavieViewEngine>;

            if (!viewEngines.IsNullOrEmpty())
            {
                foreach (ILavieViewEngine viewEngine in (IEnumerable<ILavieViewEngine>)viewContext.ViewData["LavieViewEngines"])
                {
                    FileEngineResult result = findFile(viewEngine, path);

                    if (result.SearchedLocations.Any())
                        searchedLocations.AddRange(result.SearchedLocations);
                    else
                    {
                        path = urlHelper.AppPath(result.FilePath);
                        searchedLocations.Clear();

                        break;
                    }
                }
            }
            if (searchedLocations.Count > 0)
            {
                //如果是调试模式，没找到文件抛出异常；否则输出查找的第一个路径
                //key为Debug的ViewData是通过DebugActionFilter设置的
                if (viewContext.ViewData["Debug"] is bool && (bool)viewContext.ViewData["Debug"])
                {
                    var locationsText = new StringBuilder();

                    foreach (string location in searchedLocations)
                    {
                        locationsText.AppendLine();
                        locationsText.Append(location);
                    }

                    throw new InvalidOperationException(string.Format("The file '{0}' could not be found. The following locations were searched:{1}", path, locationsText));
                }
                else
                {
                    path = urlHelper.AppPath(searchedLocations.ElementAt(0));
                }
            }
        }
    }
}
