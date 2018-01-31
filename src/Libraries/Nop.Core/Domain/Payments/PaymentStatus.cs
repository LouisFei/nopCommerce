
namespace Nop.Core.Domain.Payments
{
    /// <summary>
    /// Represents a payment status enumeration
    /// 支付状态
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Pending
        /// 支付中
        /// </summary>
        Pending = 10,
        /// <summary>
        /// Authorized
        /// 已授权
        /// </summary>
        Authorized = 20,
        /// <summary>
        /// Paid
        /// 支付完毕
        /// </summary>
        Paid = 30,
        /// <summary>
        /// Partially Refunded
        /// 部分已退款
        /// </summary>
        PartiallyRefunded = 35,
        /// <summary>
        /// Refunded
        /// 已退款
        /// </summary>
        Refunded = 40,
        /// <summary>
        /// Voided
        /// 取消
        /// </summary>
        Voided = 50,
    }
}
