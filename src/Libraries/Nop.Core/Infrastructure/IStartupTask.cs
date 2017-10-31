namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Interface which should be implemented by tasks run on startup
    /// 程序启动时执行的任务接口
    /// </summary>
    public interface IStartupTask 
    {
        /// <summary>
        /// Executes a task
        /// 执行一个任务
        /// </summary>
        void Execute();

        /// <summary>
        /// Order
        /// 任务执行的顺序
        /// </summary>
        int Order { get; }
    }
}
