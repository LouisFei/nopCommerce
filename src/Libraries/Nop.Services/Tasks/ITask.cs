namespace Nop.Services.Tasks
{
    /// <summary>
    /// Interface that should be implemented by each task
    ///  计划任务的接口，所有需要计划执行任务的类都应该继承该类，里面有个Execute()，
    ///  而实现类需要实现该类完成需要具体执行的任务 。
    /// </summary>
    public partial interface ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        void Execute();
    }
}
