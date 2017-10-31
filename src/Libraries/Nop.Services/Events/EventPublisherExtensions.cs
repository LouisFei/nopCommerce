using Nop.Core;
using Nop.Core.Events;

namespace Nop.Services.Events
{
    /// <summary>
    /// 事件发布器扩展
    /// </summary>
    public static class EventPublisherExtensions
    {
        /// <summary>
        /// 实体插入数据库时
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventPublisher"></param>
        /// <param name="entity"></param>
        public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
        {
            eventPublisher.Publish(new EntityInserted<T>(entity));
        }

        /// <summary>
        /// 实体更新时
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventPublisher"></param>
        /// <param name="entity"></param>
        public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
        {
            eventPublisher.Publish(new EntityUpdated<T>(entity));
        }

        /// <summary>
        /// 实体删除时
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventPublisher"></param>
        /// <param name="entity"></param>
        public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
        {
            eventPublisher.Publish(new EntityDeleted<T>(entity));
        }
    }
}