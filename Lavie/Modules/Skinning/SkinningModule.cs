using System;
using System.Web.Mvc;
using Lavie.FilterProviders;
using Lavie.Infrastructure;
using Lavie.Modules.Skinning.ActionFilters.Result;
using Lavie.Modules.Skinning.SkinResolvers;
using Lavie.Modules.Skinning.ViewEngines;

namespace Lavie.Modules.Skinning
{
    public class SkinningModule : Module
    {
        #region IModule Members

        public override string ModuleName
        {
            get { return "Skinning"; }
        }

        public override void Initialize()
        {
            //设置DI映射
            SetupContainer();
            //注册皮肤解析器
            RegisterSkinResolvers();
            //注册视图引擎
            RegisterViewEngines();
        }

        public override void RegisterFilters(FilterRegistryFilterProvider filterRegistry, GlobalFilterCollection globalFilters)
        {
            // ViewEnginesResultFilter,ViewEngines设置
            // Skinning 从这里开始,让Filter尽可能靠前
            filterRegistry.Add(null, typeof(ViewEnginesResultFilter),FilterScope.First,Int32.MinValue);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 设置DI映射
        /// </summary>
        private void SetupContainer()
        {
            DependencyInjector
                .RegisterType<ISkinResolverRegistry, SkinResolverRegistry>();
        }

        /// <summary>
        /// 注册皮肤解析器
        /// </summary>
        private void RegisterSkinResolvers()
        {
            ISkinResolverRegistry skinResolverRegistry = DependencyInjector.GetService<SkinResolverRegistry>();
            skinResolverRegistry.Default = new LavieSkinResolver(this.Settings.GetString("SkinsPath"));

            //移动设备
            skinResolverRegistry.Add(DependencyInjector.GetService<MobileSkinResolver>());
            //IE6、IE7和IE8等Hacks
            skinResolverRegistry.Add(DependencyInjector.GetService<LegacySkinResolver>());

            DependencyInjector.RegisterInstance(skinResolverRegistry);
        }

        private void RegisterViewEngines()
        {
            #region 注册视图引擎

            //获取ASP.NET MVC内置的视图引擎
            ViewEngineCollection viewEngines = DependencyInjector.GetService<ViewEngineCollection>() ?? System.Web.Mvc.ViewEngines.Engines;
            viewEngines.Clear();
            viewEngines.Add(DependencyInjector.GetService<LavieWebFormViewEngine>());
            viewEngines.Add(DependencyInjector.GetService<LavieRazorViewEngine>());

            #endregion

        }

        #endregion
    }
}
