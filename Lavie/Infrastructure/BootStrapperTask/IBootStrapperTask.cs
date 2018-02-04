using System.Collections.Generic;

namespace Lavie.Infrastructure
{
    /// <summary>
    /// 引导程序
    /// </summary>
    public interface IBootStrapperTask
    {
        /// <summary>
        /// 执行引导程序
        /// </summary>
        void Execute();

        /// <summary>
        /// 引导程序清理
        /// </summary>
        void Cleanup();
    }
}
