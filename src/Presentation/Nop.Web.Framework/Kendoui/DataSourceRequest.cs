namespace Nop.Web.Framework.Kendoui
{
    /// <summary>
    /// 数据源请求
    /// </summary>
    public class DataSourceRequest
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 每页显示数量
        /// </summary>
        public int PageSize { get; set; }

        public DataSourceRequest()
        {
            this.Page = 1; //默认从第1页显示
            this.PageSize = 10; //默认每页显示10条数据
        }
    }
}
