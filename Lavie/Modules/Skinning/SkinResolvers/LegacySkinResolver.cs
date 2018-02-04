using System.Collections.Generic;
using Lavie.Extensions;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Skinning.SkinResolvers
{
    /// <summary>
    /// 遗留皮肤解析器(处理老式浏览器的一些兼容性问题)
    /// </summary>
    public class LegacySkinResolver : ISkinResolver
    {
        #region ISkinResolver Members

        /// <summary>
        /// 皮肤解析
        /// </summary>
        public void Resolve(SkinResolverContext skinResolverContext, IList<string> skinPaths)
        {
            Guard.ArgumentNotNull(skinResolverContext, "skinResolverContext");
            Guard.ArgumentNotNull(skinPaths, "skinPaths");

            if (skinResolverContext.Skin.IsNullOrWhiteSpace())
                return;

            if (skinResolverContext.RequestContext.HttpContext.Request.Browser.Browser.Contains("IE"))
            {
                if (skinResolverContext.RequestContext.HttpContext.Request.Browser.MajorVersion == 8)
                {
                    ResolveHacksFolder(skinPaths, "IE8");
                }
                if (skinResolverContext.RequestContext.HttpContext.Request.Browser.MajorVersion == 7)
                {
                    ResolveHacksFolder(skinPaths, "IE7");
                }
                else if (skinResolverContext.RequestContext.HttpContext.Request.Browser.MajorVersion == 6)
                {
                    ResolveHacksFolder(skinPaths, "IE6");
                }
            }
        }

        #endregion

        public static void ResolveHacksFolder(IList<string> skinPaths, string foldername)
        {
            List<string> newSkinPaths = new List<string>(skinPaths.Count);

            foreach (string skinPath in skinPaths)
                newSkinPaths.Add(string.Format("{0}{1}/{2}", skinPath, skinPath.EndsWith("/") ? "Hacks" : "/Hacks", foldername));

            foreach (string skinPath in newSkinPaths)
                skinPaths.Add(skinPath);
        }
    }
}
