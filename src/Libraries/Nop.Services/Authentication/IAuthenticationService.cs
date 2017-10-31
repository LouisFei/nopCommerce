using Nop.Core.Domain.Customers;

namespace Nop.Services.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// �����֤����ӿ�
    /// </summary>
    public partial interface IAuthenticationService 
    {
        /// <summary>
        /// Sign in
        /// ��¼
        /// </summary>
        /// <param name="customer">Customer �û�ʵ��</param>
        /// <param name="createPersistentCookie">A value indicating whether to create a persistent cookie, �Ƿ�Ҫ��ס��¼״̬</param>
        void SignIn(Customer customer, bool createPersistentCookie);

        /// <summary>
        /// Sign out
        /// �˳�
        /// </summary>
        void SignOut();

        /// <summary>
        /// Get authenticated customer
        /// ����ѵ�¼���û�
        /// </summary>
        /// <returns>Customer</returns>
        Customer GetAuthenticatedCustomer();
    }
}