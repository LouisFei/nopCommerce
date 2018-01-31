
namespace Nop.Core.Domain.Payments
{
    /// <summary>
    /// Represents a payment status enumeration
    /// ֧��״̬
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Pending
        /// ֧����
        /// </summary>
        Pending = 10,
        /// <summary>
        /// Authorized
        /// ����Ȩ
        /// </summary>
        Authorized = 20,
        /// <summary>
        /// Paid
        /// ֧�����
        /// </summary>
        Paid = 30,
        /// <summary>
        /// Partially Refunded
        /// �������˿�
        /// </summary>
        PartiallyRefunded = 35,
        /// <summary>
        /// Refunded
        /// ���˿�
        /// </summary>
        Refunded = 40,
        /// <summary>
        /// Voided
        /// ȡ��
        /// </summary>
        Voided = 50,
    }
}
