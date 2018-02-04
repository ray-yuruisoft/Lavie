using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Modules.Skinning;
using Lavie.Modules.Skinning.ViewEngines;

namespace Lavie.Modules.Skinning.ActionFilters.Result
{

    public class ViewEnginesResultFilter : IResultFilter, IExceptionFilter
    {
        private readonly IModuleRegistry _modules;
        private readonly ISkinResolverRegistry _skinResolverRegistry;

        public ViewEnginesResultFilter(IModuleRegistry modules, ISkinResolverRegistry skinResolvers)
        {
            this._modules = modules;
            this._skinResolverRegistry = skinResolvers;
        }

        #region IResultFilter Members

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            SetupSkinViewEngines(filterContext.RequestContext, filterContext.Controller.ViewData, filterContext.Result as ViewResultBase);
        }

        #endregion

        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            SetupSkinViewEngines(filterContext.RequestContext, filterContext.Controller.ViewData, filterContext.Result as ViewResultBase);
        }

        #endregion

        #region Public Methods

        public void SetupSkinViewEngines(RequestContext requestContext, ViewDataDictionary viewData, ViewResultBase result)
        {
            if (result == null) return;

            //当前模块名称
            string currentModuleName = requestContext.RouteData.DataTokens["ModuleName"] as string;
            if (currentModuleName.IsNullOrWhiteSpace()) return;

            //当前模块
            IModule currentModule = _modules.GetModules<IModule>().FirstOrDefault(m => m.ModuleName == currentModuleName);
            if (currentModule == null) return;

            HttpRequestBase request = requestContext.HttpContext.Request;
            string queryStringSkinName = request.QueryString["skin"];
            string cookieSkinName = request.Cookies.GetString("skin");
            string skin = null;

            if (!string.IsNullOrEmpty(queryStringSkinName))
                skin = queryStringSkinName;
            else if (!string.IsNullOrEmpty(cookieSkinName))
                skin = cookieSkinName;

            
            //如果QueryString和Cookie都没有包含Skin值            
            if (skin.IsNullOrWhiteSpace())
            {
                skin = viewData["Skin"] as string;
            }
            if (skin.IsNullOrWhiteSpace())
            {
                skin = currentModule.Settings.GetString("Skin");
            }
            //GenerateViewEngines方法
            //通过ViewEngines.Engines生成新的IEnumerable<ILavieViewEngine>
            IEnumerable<ILavieViewEngine> viewEngines = _skinResolverRegistry.GenerateViewEngines(new SkinResolverContext(requestContext, skin));

            //如果ActionResult是ViewResultBase值，则result参数不会为null
            //这里将把result的ViewEngineCollection设置为GenerateViewEngines方法刚才生成的视图引擎集合。
            //也就是说，在ViewResult执行之前，默认的视图引擎就被替换了
            result.ViewEngineCollection = new ViewEngineCollection(viewEngines.Cast<IViewEngine>().ToList()); ;

            viewData["LavieViewEngines"] = viewEngines;
        }

        #endregion

    }
}
