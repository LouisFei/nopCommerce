using Nop.Core.Domain.Stores;

namespace Nop.Core
{
    /// <summary>
    /// Store context
    /// 商店上下文
    /// </summary>
    public interface IStoreContext
    {
        /// <summary>
        /// Gets or sets the current store
        /// </summary>
        Store CurrentStore { get; }
    }
}
