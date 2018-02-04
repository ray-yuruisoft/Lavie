using System.Web.Mvc;
using Lavie.Infrastructure;
using Lavie.Extensions;
using System.Web.Hosting;

namespace Lavie.Modules.Skinning.ViewEngines
{
    public class LavieRazorViewEngine : RazorViewEngine, ILavieViewEngine
    {
        private string _rootPath;

        #region ILavieViewEngine Members

        public void SetRootPath(string rootPath, bool onlySearchRootPathForPartialViews)
        {
            this._rootPath = rootPath.EnsureVirtualDirectory();

            base.ViewLocationFormats = new[]
            {
                _rootPath + "/Views/{1}/{0}.cshtml",
                _rootPath + "/Views/Shared/{0}.cshtml",
                //_rootPath + "/Views/{1}/{0}.vbhtml",
                //_rootPath + "/Views/Shared/{0}.vbhtml"
            };

            base.MasterLocationFormats = base.ViewLocationFormats;

            base.PartialViewLocationFormats = !onlySearchRootPathForPartialViews
                ? base.ViewLocationFormats
                : new[] { 
                    _rootPath + "/{0}.cshtml",
                    //_rootPath + "/{0}.vbhtml" 
                };

            base.AreaViewLocationFormats = new[]
            {
                _rootPath + "/Area/{2}/Views/{1}/{0}.cshtml",
                _rootPath + "/Area/{2}/Views/Shared/{0}.cshtml",
                //_rootPath + "/Area/{2}/Views/{1}/{0}.vbhtml",
                //_rootPath + "/Area/{2}/Views/Shared/{0}.vbhtml"
            };
            base.AreaMasterLocationFormats = base.AreaViewLocationFormats;
            base.AreaPartialViewLocationFormats = !onlySearchRootPathForPartialViews
                ? base.AreaViewLocationFormats
                : new[] { 
                    _rootPath + "/Area/{2}/{0}.cshtml",
                    //_rootPath + "/Area/{2}/{0}.vbhtml"
                };

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
