using System;
using System.Linq;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Logging;

namespace Nop.Services.Events
{
    /// <summary>
    /// Evnt publisher
    /// 事件发布器
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly ISubscriptionService _subscriptionService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscriptionService"></param>
        public EventPublisher(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// Publish to cunsumer
        /// 向消费者发布消息
        /// </summary>
        /// <typeparam name="T">Type 消费者类型</typeparam>
        /// <param name="x">Event consumer 事件消费者</param>
        /// <param name="eventMessage">Event message 事件消息</param>
        protected virtual void PublishToConsumer<T>(IConsumer<T> x, T eventMessage)
        {
            //Ignore not installed plugins
            var plugin = FindPlugin(x.GetType());
            if (plugin != null && !plugin.Installed)
                return;

            try
            {
                x.HandleEvent(eventMessage);
            }
            catch (Exception exc)
            {
                //log error
                var logger = EngineContext.Current.Resolve<ILogger>();
                //we put in to nested try-catch to prevent possible cyclic (if some error occurs)
                try
                {
                    logger.Error(exc.Message, exc);
                }
                catch (Exception)
                {
                    //do nothing
                }
            }
        }

        /// <summary>
        /// 通过类型查找插件描述器
        /// Find a plugin descriptor by some type which is located into its assembly
        /// </summary>
        /// <param name="providerType">Provider type</param>
        /// <returns>Plugin descriptor</returns>
        protected virtual PluginDescriptor FindPlugin(Type providerType)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");

            if (PluginManager.ReferencedPlugins == null)
                return null;

            foreach (var plugin in PluginManager.ReferencedPlugins)
            {
                if (plugin.ReferencedAssembly == null)
                    continue;

                if (plugin.ReferencedAssembly.FullName == providerType.Assembly.FullName)
                    return plugin;
            }

            return null;
        }

        /// <summary>
        /// Publish event
        /// 发布事件
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="eventMessage">Event message</param>
        public virtual void Publish<T>(T eventMessage)
        {
            var subscriptions = _subscriptionService.GetSubscriptions<T>();
            subscriptions.ToList().ForEach(x => PublishToConsumer(x, eventMessage));
        }

    }
}
