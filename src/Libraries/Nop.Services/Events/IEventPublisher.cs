
namespace Nop.Services.Events
{
    /// <summary>
    /// Evnt publisher
    /// 事件发布器
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// 发布事件
        /// Publish event
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="eventMessage">Event message</param>
        void Publish<T>(T eventMessage);
    }
}
