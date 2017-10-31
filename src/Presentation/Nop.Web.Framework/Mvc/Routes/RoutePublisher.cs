using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

using Nop.Core.Infrastructure;
using Nop.Core.Plugins;

namespace Nop.Web.Framework.Mvc.Routes
{
    /// <summary>
    /// 路由发布者具体实现。
    /// Route publisher
    /// </summary>
    public class RoutePublisher : IRoutePublisher
    {
        protected readonly ITypeFinder typeFinder;

        /// <summary>
        /// Ctor 实例化一个路由发布者对象
        /// </summary>
        /// <param name="typeFinder"></param>
        public RoutePublisher(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }

        /// <summary>
        /// 发现一个插件描述器（通过插件类型，从程序集里查找）
        /// Find a plugin descriptor by some type which is located into its assembly
        /// </summary>
        /// <param name="providerType">Provider type</param>
        /// <returns>Plugin descriptor</returns>
        protected virtual PluginDescriptor FindPlugin(Type providerType)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");

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
        /// Register routes
        /// 注册路由
        /// </summary>
        /// <param name="routes">Routes</param>
        public virtual void RegisterRoutes(RouteCollection routes)
        {
            //查找所有路由规则提供者
            //通过typeFinder找出所有（包括插件）实现了接口IRouteProvider相关的类型
            var routeProviderTypes = typeFinder.FindClassesOfType<IRouteProvider>();

            var routeProviders = new List<IRouteProvider>();
            foreach (var providerType in routeProviderTypes)
            {
                //忽略没有安装的插件
                //Ignore not installed plugins
                var plugin = FindPlugin(providerType);
                if (plugin != null && !plugin.Installed)
                {
                    continue;
                }                    

                //采用反射动态创建IRouteProvider的具体类的实例。
                var provider = Activator.CreateInstance(providerType) as IRouteProvider;
                routeProviders.Add(provider);
            }

            routeProviders = routeProviders.OrderByDescending(rp => rp.Priority).ToList();

            //依次调用RouteProvider的RegisterRoutes方法，注册路由规则。
            routeProviders.ForEach(rp => rp.RegisterRoutes(routes));
        }
    }
}
