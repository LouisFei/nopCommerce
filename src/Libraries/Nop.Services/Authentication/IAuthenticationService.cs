using Nop.Core.Domain.Customers;

namespace Nop.Services.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// 身份验证服务接口
    /// </summary>
    public partial interface IAuthenticationService 
    {
        /// <summary>
        /// Sign in
        /// 登录
        /// </summary>
        /// <param name="customer">Customer 用户实体</param>
        /// <param name="createPersistentCookie">A value indicating whether to create a persistent cookie, 是否要记住登录状态</param>
        void SignIn(Customer customer, bool createPersistentCookie);

        /// <summary>
        /// Sign out
        /// 退出
        /// </summary>
        void SignOut();

        /// <summary>
        /// Get authenticated customer
        /// 获得已登录的用户
        /// </summary>
        /// <returns>Customer</returns>
        Customer GetAuthenticatedCustomer();
    }
}