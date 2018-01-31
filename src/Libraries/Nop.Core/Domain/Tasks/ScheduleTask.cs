using System;

namespace Nop.Core.Domain.Tasks
{
    /// <summary>
    /// Schedule task
    /// 计划任务，数据库实体
    /// </summary>
    public partial class ScheduleTask : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// 计划任务的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the run period (in seconds)
        /// 任务间隔时长（秒）
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Gets or sets the type of appropriate ITask class
        /// 执行任务的类所对应的程序集和命名空间的字符串，用于反射创建类
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task is enabled
        /// 是否启用任务
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task should be stopped on some error
        /// 发送错误是否停止执行
        /// </summary>
        public bool StopOnError { get; set; }


        /// <summary>
        /// Gets or sets the machine name (instance) that leased this task. It's used when running in web farm (ensure that a task in run only on one machine). It could be null when not running in web farm.
        /// 计算机的名称，这个好像是多网站（分布式）的时候有用 （具体的使用暂不清楚）
        /// </summary>
        public string LeasedByMachineName { get; set; }
        /// <summary>
        /// Gets or sets the datetime until the task is leased by some machine (instance). It's used when running in web farm (ensure that a task in run only on one machine).
        /// 实例上执行的时间  （具体的使用暂不清楚）
        /// </summary>
        public DateTime? LeasedUntilUtc { get; set; }

        /// <summary>
        /// Gets or sets the datetime when it was started last time
        /// 最后开始执行任务的开始时间
        /// </summary>
        public DateTime? LastStartUtc { get; set; }
        /// <summary>
        /// Gets or sets the datetime when it was finished last time (no matter failed ir success)
        /// 开始执行任务的结束时间
        /// </summary>
        public DateTime? LastEndUtc { get; set; }
        /// <summary>
        /// Gets or sets the datetime when it was sucessfully finished last time
        /// 最后一次执行成功的时间
        /// </summary>
        public DateTime? LastSuccessUtc { get; set; }
    }
}
