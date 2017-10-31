
namespace Nop.Services.Events
{
    /// <summary>
    /// 事件消费者接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConsumer<T>
    {
        /// <summary>
        /// 事件处理方法
        /// </summary>
        /// <param name="eventMessage">事件消息</param>
        void HandleEvent(T eventMessage);
    }
}
