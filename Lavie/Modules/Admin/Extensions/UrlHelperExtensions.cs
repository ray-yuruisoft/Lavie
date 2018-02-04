using System.Web.Mvc;
using Lavie.Html;

namespace Lavie.Modules.Admin.Extensions
{
    public static class UrlHelperExtensions
    {

        public static string Home(this UrlHelper urlHelper)
        {
            return urlHelper.AppPath("~/");
        }

        public static string Admin(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrlEx("Admin.Index");
        }

        public static string AdminSignIn(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrlEx("Admin.SignIn");
        }

        public static string AdminSignOut(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrlEx("Admin.SignOut");
        }

        public static string GetValidateCode(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrlEx("Admin.GetValidateCode");
        }

    }
}
