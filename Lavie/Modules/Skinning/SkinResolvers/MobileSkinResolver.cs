using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lavie.Extensions;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Skinning.SkinResolvers
{
    /// <summary>
    /// 移动设备皮肤解析器
    /// </summary>
    public class MobileSkinResolver : ISkinResolver
    {
        private Regex uaRegex;

        public MobileSkinResolver()
        {
            uaRegex = new Regex("(up.browser|up.link|mmp|symbian|smartphone|midp|wap|phone|windows ce|pda|mobile|mini|palm)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

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

            bool isMobile = false;
            string ua = skinResolverContext.RequestContext.HttpContext.Request.UserAgent ?? String.Empty;

            if (uaRegex.IsMatch(ua))
                isMobile = true;

            if (!isMobile)
            {
                string[] uaPrefixes = new[] { "w3c ", "acs-", "alav", "alca", "amoi", "audi", "avan", "benq", "bird", "blac", "blaz", "brew", "cell", "cldc", "cmd-", "dang", "doco", "eric", "hipt", "inno", "ipaq", "java", "jigs", "kddi", "keji", "leno", "lg-c", "lg-d", "lg-g", "lge-", "maui", "maxo", "midp", "mits", "mmef", "mobi", "mot-", "moto", "mwbp", "nec-", "newt", "noki", "oper", "palm", "pana", "pant", "phil", "play", "port", "prox", "qwap", "sage", "sams", "sany", "sch-", "sec-", "send", "seri", "sgh-", "shar", "sie-", "siem", "smal", "smar", "sony", "sph-", "symb", "t-mo", "teli", "tim-", "tosh", "tsm-", "upg1", "upsi", "vk-v", "voda", "wap-", "wapa", "wapi", "wapp", "wapr", "webc", "winw", "winw", "xda", "xda-" };

                foreach (string uaPrefix in uaPrefixes)
                {
                    if (ua.StartsWith(uaPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        isMobile = true;

                        break;
                    }
                }

                if (isMobile)
                {
                    if (ua.StartsWith("Opera/"))
                        isMobile = false;
                }
            }
            if (isMobile)
            {
                List<string> newSkinPaths = new List<string>(skinPaths.Count);

                foreach (string skinPath in skinPaths)
                    newSkinPaths.Add(string.Format("{0}{1}/{2}", skinPath, skinPath.EndsWith("/") ? "Devices" : "/Devices", "Generic"));

                foreach (string skinPath in newSkinPaths)
                    skinPaths.Add(skinPath);
            }
        }

        #endregion
    }
}
