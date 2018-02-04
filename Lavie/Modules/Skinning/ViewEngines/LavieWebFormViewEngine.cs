using System.Web.Mvc;
using Lavie.Infrastructure;
using Lavie.Extensions;
using System.Web.Hosting;

namespace Lavie.Modules.Skinning.ViewEngines
{
    public class LavieWebFormViewEngine : WebFormViewEngine, ILavieViewEngine
    {
        private string _rootPath;
       
        #region ILavieViewEngine Members

        public void SetRootPath(string rootPath, bool onlySearchRootPathForPartialViews)
        {
            this._rootPath = rootPath.EnsureVirtualDirectory();

            base.ViewLocationFormats = new[]
            {
                rootPath + "/Views/{1}/{0}.aspx",
                rootPath + "/Views/Shared/{0}.aspx",
                //rootPath + "/Views/{1}/{0}.ascx",
                //rootPath + "/Views/Shared/{0}.ascx"
            };
            base.MasterLocationFormats = base.ViewLocationFormats;

            base.PartialViewLocationFormats = !onlySearchRootPathForPartialViews
                ? base.ViewLocationFormats
                : new[] { rootPath + "/{0}.ascx" };

            base.AreaViewLocationFormats = new[]
            {
                rootPath + "/Area/{2}/Views/{1}/{0}.aspx",
                rootPath + "/Area/{2}/Views/Shared/{0}.aspx",
                //rootPath + "/Area/{2}/Views/{1}/{0}.ascx",
                //rootPath + "/Area/{2}/Views/Shared/{0}.ascx"
            };
            base.AreaMasterLocationFormats = new[]
            {
                rootPath + "/Area/{2}/Views/{1}/{0}.master",
                rootPath + "/Area/{2}/Views/Shared/{0}.master"
            };
            base.AreaPartialViewLocationFormats = !onlySearchRootPathForPartialViews
                ? base.AreaViewLocationFormats
                : new[] { rootPath + "/Area/{2}/{0}.ascx" };

        }

        public string RootPath
        {
            set { SetRootPath(value, false); }
            get { return _rootPath; }
        }
        
        public FileEngineResult FindFile(string fileName)
        {
            if (!fileName.StartsWith("/"))
            {
                fileName = "/" + fileName;
            }

            fileName = _rootPath + fileName;

            if (VirtualPathProvider.FileExists(fileName))
            {
                return new FileEngineResult(fileName, this);
            }

            return new FileEngineResult(new[] { fileName });
        }

        #endregion

    }
}
