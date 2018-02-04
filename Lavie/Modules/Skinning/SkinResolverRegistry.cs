using System.Collections.Generic;
using System.Web.Mvc;
using Lavie.Infrastructure;
using Lavie.Infrastructure.InversionOfControl;
using Lavie.Modules.Skinning.SkinResolvers;
using Lavie.Modules.Skinning.ViewEngines;

namespace Lavie.Modules.Skinning
{
    /// <summary>
    /// 皮肤解析器注册表
    /// </summary>
    public class SkinResolverRegistry : ISkinResolverRegistry
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly ViewEngineCollection _viewEngines;
        private readonly List<ISkinResolver> _skinResolvers;

        public SkinResolverRegistry(IDependencyResolver dependencyResolver, ViewEngineCollection viewEngines)
        {
            this._dependencyResolver = dependencyResolver;
            this._viewEngines = viewEngines;
            this._skinResolvers = new List<ISkinResolver>();
        }

        #region ISkinResolverRegistry Members

        /// <summary>
        /// 默认皮肤解析器
        /// </summary>
        public ISkinResolver Default { get; set; }

        /// <summary>
        /// 添加皮肤解析器
        /// </summary>
        /// <param name="skinResolver"></param>
        public void Add(ISkinResolver skinResolver)
        {
            _skinResolvers.Add(skinResolver);
        }

        /// <summary>
        /// 生成视图引擎
        /// </summary>
        /// <param name="skinResolverContext"></param>
        /// <returns></returns>
        public IEnumerable<ILavieViewEngine> GenerateViewEngines(SkinResolverContext skinResolverContext)
        {
            // newViewEngines用于保存生成的视图引擎集合
            List<ILavieViewEngine> newViewEngines = new List<ILavieViewEngine>();
            // skinPaths用于保存皮肤解析器生成的路径信息
            List<string> skinPaths = new List<string>();

            // 虚拟根目录
            skinPaths.Add("~/");

            if (Default != null)
                Default.Resolve(skinResolverContext, skinPaths);

            foreach (ISkinResolver skinResolver in _skinResolvers)
                skinResolver.Resolve(skinResolverContext, skinPaths);

            if (skinPaths.Count > 0)
            {
                skinPaths.Reverse();

                foreach (string skinPath in skinPaths)
                {
                    foreach (IViewEngine viewEngine in _viewEngines)
                    {
                        if (viewEngine is ILavieViewEngine)
                        {
                            ILavieViewEngine viewEngineInstance = (ILavieViewEngine)_dependencyResolver.GetService(viewEngine.GetType());

                            viewEngineInstance.RootPath = skinPath;

                            newViewEngines.Add(viewEngineInstance);
                        }
                    }
                }
            }

            return newViewEngines;
        }

        #endregion

    }
}
