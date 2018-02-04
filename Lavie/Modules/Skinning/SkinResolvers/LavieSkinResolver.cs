using System;
using System.Collections.Generic;
using Lavie.Extensions;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Skinning.SkinResolvers
{
    /// <summary>
    /// 基本皮肤解析器
    /// </summary>
    public class LavieSkinResolver : ISkinResolver
    {
        private readonly string _skinsPath;

        public LavieSkinResolver(string skinsPath)
        {
            Guard.ArgumentNotNullOrEmpty(skinsPath, "skinsPath");

            this._skinsPath = skinsPath.EnsureVirtualDirectory();
        }

        #region ISkinResolver Members

        /// <summary>
        /// 皮肤解析
        /// </summary>
        /// <example>
        /// <para>如皮肤主目录是~/skins，皮肤名是default，则会生成：
        /// <para>~/skins/default</para>
        /// <para>如皮肤主目录是~/skins，皮肤名是default/mydefault,则会生成：</para>
        /// <para>~/skins/default</para>
        /// <para>~/skins/default/mydefault</para>
        /// <para>如皮肤主目录是~/skins，皮肤名是/default/mydefault,则会生成：</para>
        /// <para>~/skins/default</para>
        /// <para>~/skins/default/mydefault</para>
        /// <para>如皮肤主目录是~/skins/myskins，皮肤名是default/mydefault,则会生成：</para>
        /// <para>~/skins/myskins/default</para>
        /// <para>~/skins/myskins/default/mydefault</para>
        /// <para>如皮肤主目录是~/skins/myskins，皮肤名是/default/mydefault,则会生成：</para>
        /// <para>~/skins/myskins/default</para>
        /// <para>~/skins/myskins/default/mydefault</para>
        /// </example>
        /// <remarks>
        /// 尽管皮肤名称支持多级目录形式，但建议使用单目录名的形式，如：defualt
        /// </remarks>
        /// <param name="skinResolverContext">皮肤解析器上下文</param>
        /// <param name="skinPaths">皮肤路径集</param>
        public void Resolve(SkinResolverContext skinResolverContext, IList<string> skinPaths)
        {
            Guard.ArgumentNotNull(skinResolverContext, "skinResolverContext");
            Guard.ArgumentNotNull(skinPaths, "skinPaths");

            if (skinResolverContext.Skin.IsNullOrWhiteSpace())
                return;

            string[] skinNames = skinResolverContext.Skin.EnsureSubFolder().Split('/');
            string skinPath = string.Format("{0}/{1}", _skinsPath, skinNames[0]);

            skinPaths.Add(skinPath);

            if (skinNames.Length > 1)
                for (int i = 1; i < skinNames.Length; i++)
                    skinPaths.Add(skinPath = string.Format("{0}/{1}", skinPath, skinNames[i]));
        }

        #endregion
    }
}
