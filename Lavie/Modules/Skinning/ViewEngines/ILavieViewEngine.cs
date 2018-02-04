using System.Web.Mvc;

namespace Lavie.Modules.Skinning.ViewEngines
{
    public interface ILavieViewEngine : IViewEngine
    {
        void SetRootPath(string rootPath, bool onlySearchRootPathForPartialViews);
        string RootPath { get; set; }
        FileEngineResult FindFile(string fileName);
    }
}
