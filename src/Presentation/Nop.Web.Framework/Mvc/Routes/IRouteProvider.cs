using System.Web.Routing;

namespace Nop.Web.Framework.Mvc.Routes
{
    /// <summary>
    /// 路由提供者
    /// </summary>
    public interface IRouteProvider
    {
        /// <summary>
        /// 注册路由
        /// </summary>
        /// <param name="routes"></param>
        void RegisterRoutes(RouteCollection routes);

        /// <summary>
        /// 优先次序
        /// </summary>
        int Priority { get; }
    }
}
