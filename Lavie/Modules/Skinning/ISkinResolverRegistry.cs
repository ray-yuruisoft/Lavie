using System.Collections.Generic;
using Lavie.Modules.Skinning.SkinResolvers;
using Lavie.Modules.Skinning.ViewEngines;

namespace Lavie.Modules.Skinning
{
    /// <summary>
    /// 皮肤解析器表
    /// </summary>
    public interface ISkinResolverRegistry
    {
        /// <summary>
        /// 默认皮肤解析器
        /// </summary>
        ISkinResolver Default { get; set; }
        /// <summary>
        /// 添加皮肤解析器
        /// </summary>
        /// <param name="skinResolver">皮肤解析器</param>
        void Add(ISkinResolver skinResolver);
        /// <summary>
        /// 生成视图引擎
        /// </summary>
        /// <param name="skinResolverContext">皮肤解析器上下文</param>
        /// <returns></returns>
        IEnumerable<ILavieViewEngine> GenerateViewEngines(SkinResolverContext context);
    }
}
