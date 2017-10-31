using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Configuration
{
    /// <summary>
    /// Represents a setting
    /// 领域模型实体类：设置
    /// </summary>
    public partial class Setting : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Setting() { }
        
        /// <summary>
        /// 带参构造函数，实例化一个设置实体类
        /// </summary>
        /// <param name="name">设置名</param>
        /// <param name="value">设置值</param>
        /// <param name="storeId">归属（可忽略）</param>
        public Setting(string name, string value, int storeId = 0) {
            this.Name = name;
            this.Value = value;
            this.StoreId = storeId;
        }
        
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the store for which this setting is valid. 0 is set when the setting is for all stores
        /// </summary>
        public int StoreId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
