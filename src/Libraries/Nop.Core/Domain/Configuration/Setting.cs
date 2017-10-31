using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Configuration
{
    /// <summary>
    /// Represents a setting
    /// ����ģ��ʵ���ࣺ����
    /// </summary>
    public partial class Setting : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public Setting() { }
        
        /// <summary>
        /// ���ι��캯����ʵ����һ������ʵ����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="value">����ֵ</param>
        /// <param name="storeId">�������ɺ��ԣ�</param>
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
