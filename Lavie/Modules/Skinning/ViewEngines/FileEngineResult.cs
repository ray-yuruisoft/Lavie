using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lavie.Modules.Skinning.ViewEngines
{
    /// <summary>
    /// 文件搜索结果，类似于ViewEngineResult类
    /// </summary>
    public class FileEngineResult
    {
        public FileEngineResult(IEnumerable<string> searchedLocations)
        {
            SearchedLocations = searchedLocations;
        }

        public FileEngineResult(string filePath, IViewEngine viewEngine)
            : this(Enumerable.Empty<string>())
        {
            FilePath = filePath;
            ViewEngine = viewEngine;
        }

        /// <summary>
        /// 搜索了的位置
        /// </summary>
        public IEnumerable<string> SearchedLocations { get; private set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; private set; }
        /// <summary>
        /// 视图引擎
        /// </summary>
        public IViewEngine ViewEngine { get; private set; }
    }
}
