using System;
using System.Web;
using System.Web.Security;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;

namespace Nop.Services.Authentication
{
    /// <summary>
    /// Authentication service
    /// 身份验证服务
    /// </summary>
    public partial class FormsAuthenticationService : IAuthenticationService
    {
        #region Fields

        /// <summary>
        /// 当前请求上下文
        /// </summary>
        private readonly HttpContextBase _httpContext;
        /// <summary>
        /// 用户服务
        /// </summary>
        private readonly ICustomerService _customerService;
        /// <summary>
        /// 用户设置
        /// </summary>
        private readonly CustomerSettings _customerSettings;
        /// <summary>
        /// 身份验证票证到期前的时间量
        /// </summary>
        private readonly TimeSpan _expirationTimeSpan;

        private Customer _cachedCustomer;

        #endregion

        #region Ctor 实例化一个身份验证服务对象

        /// <summary>
        /// Ctor 实例化一个身份验证服务对象
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="customerSettings">Customer settings</param>
        public FormsAuthenticationService(HttpContextBase httpContext,
            ICustomerService customerService, CustomerSettings customerSettings)
        {
            this._httpContext = httpContext;
            this._customerService = customerService;
            this._customerSettings = customerSettings;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// 获得当前登录的用户实体
        /// Get authenticated customer
        /// </summary>
        /// <param name="ticket">Ticket 登录验证票据</param>
        /// <returns>Customer</returns>
        protected virtual Customer GetAuthenticatedCustomerFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("ticket");
            }

            //用户数据
            var usernameOrEmail = ticket.UserData;

            if (String.IsNullOrWhiteSpace(usernameOrEmail))
            {
                return null;
            }

            var customer = _customerSettings.UsernamesEnabled
                ? _customerService.GetCustomerByUsername(usernameOrEmail)
                : _customerService.GetCustomerByEmail(usernameOrEmail);

            return customer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sign in
        /// 登录
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="createPersistentCookie">A value indicating whether to create a persistent cookie</param>
        public virtual void SignIn(Customer customer, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                _customerSettings.UsernamesEnabled ? customer.Username : customer.Email,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                _customerSettings.UsernamesEnabled ? customer.Username : customer.Email,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);
            _cachedCustomer = customer;
        }

        /// <summary>
        /// Sign out
        /// 退出
        /// </summary>
        public virtual void SignOut()
        {
            _cachedCustomer = null;
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Get authenticated customer
        /// 获得当前登录用户实体
        /// </summary>
        /// <returns>Customer</returns>
        public virtual Customer GetAuthenticatedCustomer()
        {
            if (_cachedCustomer != null)
                return _cachedCustomer;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var customer = GetAuthenticatedCustomerFromTicket(formsIdentity.Ticket);
            if (customer != null && customer.Active && !customer.RequireReLogin && !customer.Deleted  && customer.IsRegistered())
                _cachedCustomer = customer;
            return _cachedCustomer;
        }

        #endregion

    }
}