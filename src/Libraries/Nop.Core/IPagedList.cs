
using System.Collections.Generic;

namespace Nop.Core
{
    /// <summary>
    /// Paged list interface
    /// 分页列表接口
    /// </summary>
    public interface IPagedList<T> : IList<T>
    {
        /// <summary>
        /// 第几页，页号
        /// </summary>
        int PageIndex { get; }
        /// <summary>
        /// 每页显示几条记录
        /// </summary>
        int PageSize { get; }
        /// <summary>
        /// 数据总数量
        /// </summary>
        int TotalCount { get; }
        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPages { get; }
        /// <summary>
        /// 是否有上一页
        /// </summary>
        bool HasPreviousPage { get; }
        /// <summary>
        /// 是否有下一页
        /// </summary>
        bool HasNextPage { get; }
    }
}
