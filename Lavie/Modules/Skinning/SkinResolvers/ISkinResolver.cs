using System.Collections.Generic;

namespace Lavie.Modules.Skinning.SkinResolvers
{
    /// <summary>
    /// 皮肤解析器
    /// </summary>
    public interface ISkinResolver
    {
        /// <summary>
        /// 皮肤解析
        /// </summary>
        /// <param name="skinResolverContext">皮肤解析器上下文</param>
        /// <param name="skinPaths">皮肤路径集</param>
        void Resolve(SkinResolverContext skinResolverContext, IList<string> skinPaths);
    }
}
