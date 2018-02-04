using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Optimization;
using Lavie.ActionFilters.Authorization;
using Lavie.ActionFilters.Exception;
using Lavie.FilterProviders;
using Lavie.Infrastructure;
using Lavie.ModelBinders;
using Lavie.Models;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Lavie.Modules.Core
{
    public class CoreModule : Module
    {
        #region IModule Members

        public override string ModuleName
        {
            get { return "Core"; }
        }
        public override void WebApiConfigRegister(HttpConfiguration config)
        {
            base.WebApiConfigRegister(config);

            // Web API 配置和服务
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();

            // Web API 路由

            // 其他模块实现

        }

        public override void RegisterFilters(FilterRegistryFilterProvider filterRegistry, GlobalFilterCollection globalFilters)
        {
            // 如果是调试模式，或则QueryString包含Debug键并且值为true，则设置View["Debug"]为true，否则设置为false
            // 比如，在View中可以获取ViewData["Debug"]的值来判断是否是调试模式
            filterRegistry.Add(null, typeof(DebugAuthorizationFilter), FilterScope.First, Int32.MinValue);

            // 异常处理，让Filter尽可能靠后，因为Skinning模块需要重新设置ViewEngines
            filterRegistry.Add(null, typeof(ErrorExceptionFilter), FilterScope.Last, Int32.MaxValue);
        }

        public override void RegisterModelBinders(ModelBinderDictionary modelBinders)
        {
            // modelBinders[typeof(Int32[])] = DependencyInjector.GetService<Int32ArrayModelBinder>();
            // modelBinders[typeof(KeyValuePair<int, int>[])] = DependencyInjector.GetService<Int32KeyValuePairArrayModelBinder>();
            // modelBinders[typeof(Tuple<string, string>[])] = DependencyInjector.GetService<StringPairArrayModelBinder>();
            modelBinders[typeof(IPAddress)] = DependencyInjector.GetService<IPAddressModelBinder>();
        }

        public override void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.EnableOptimizations = true;

            /*
             备注：
             1、仅脚本，则路径以~/Scripts开头；仅CSS，则路径以~/Content开头
             2、包含脚本和CSS的套件，则路径以~/Compents开头，并且CSS文件的路径层次数必须和物理路径一致以防止CSS中引用的图片路径错误
             * 
             */

            // jQuery 
            bundles.Add(new ScriptBundle("~/Scripts/jquery").Include(
                "~/Scripts/jquery.min.js"
                ));

            // jquery.validate
            bundles.Add(new ScriptBundle("~/Compents/jquery.validate/Scripts").Include(
                "~/Scripts/jquery.unobtrusive-ajax.min.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.c.js"
                ));
            bundles.Add(new StyleBundle("~/Compents/jquery.validate/Styles").Include(
                "~/Styles/Validator/validator.css"
                ));

            // Bootstrap
            bundles.Add(new ScriptBundle("~/Compents/bootstrap/Scripts").Include(
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/respond.min.js"
                ));
            bundles.Add(new StyleBundle("~/Compents/bootstrap/Styles").Include(
                "~/Content/bootstrap.min.css"
                ));

            // jQueryUI
            bundles.Add(new ScriptBundle("~/Compents/jQueryUI/Scripts").Include(
                "~/Compents/jQueryUI/jquery-ui.min.js"
                ));
            bundles.Add(new StyleBundle("~/Compents/jQueryUI/Styles").Include(
                "~/Compents/jQueryUI/jquery-ui.min.css"
                ));

            // Modernizr
            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/Scripts/modernizr").Include(
                "~/Scripts/modernizr-*"
                ));


        }

        #endregion

    }
}
