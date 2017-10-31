namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// 用户登录事件
    /// Customer logged-in event
    /// </summary>
    public class CustomerLoggedinEvent
    {
        public CustomerLoggedinEvent(Customer customer)
        {
            this.Customer = customer;
        }

        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer
        {
            get; private set;
        }
    }
    /// <summary>
    /// 用户退出事件
    /// "Customer is logged out" event
    /// </summary>
    public class CustomerLoggedOutEvent
    {
        public CustomerLoggedOutEvent(Customer customer)
        {
            this.Customer = customer;
        }

        /// <summary>
        /// Get or set the customer
        /// </summary>
        public Customer Customer { get; private set; }
    }

    /// <summary>
    /// Customer registered event
    /// 用户注册事件
    /// </summary>
    public class CustomerRegisteredEvent
    {
        public CustomerRegisteredEvent(Customer customer)
        {
            this.Customer = customer;
        }

        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer
        {
            get; private set;
        }
    }

    /// <summary>
    /// Customer password changed event
    /// 用户密码修改事件
    /// </summary>
    public class CustomerPasswordChangedEvent
    {
        public CustomerPasswordChangedEvent(CustomerPassword password)
        {
            this.Password = password;
        }

        /// <summary>
        /// Customer password
        /// </summary>
        public CustomerPassword Password { get; private set; }
    }

}