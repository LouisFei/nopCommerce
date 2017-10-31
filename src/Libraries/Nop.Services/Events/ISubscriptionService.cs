using System.Collections.Generic;

namespace Nop.Services.Events
{
    /// <summary>
    /// Event subscription service
    /// 事件订阅服务接口
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Get subscriptions
        /// 获得所有订阅者
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        IList<IConsumer<T>> GetSubscriptions<T>();
    }
}
