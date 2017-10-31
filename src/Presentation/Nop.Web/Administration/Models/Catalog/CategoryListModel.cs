using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    /// <summary>
    /// 视图模型：商品目录列表
    /// </summary>
    public partial class CategoryListModel : BaseNopModel
    {
        /// <summary>
        /// 实例化商品目录列表视图模型对象
        /// </summary>
        public CategoryListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        /// <summary>
        /// 
        /// </summary>
        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]
        [AllowHtml]
        public string SearchCategoryName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchStore")]
        public int SearchStoreId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}