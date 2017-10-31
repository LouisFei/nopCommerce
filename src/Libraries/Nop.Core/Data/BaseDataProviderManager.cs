using System;

namespace Nop.Core.Data
{
    /// <summary>
    /// Base data provider manager
    /// 数据提供者管理器抽象类
    /// </summary>
    public abstract class BaseDataProviderManager
    {
        /// <summary>
        /// Ctor 实例化数据库提供者管理器
        /// </summary>
        /// <param name="settings">Data settings</param>
        protected BaseDataProviderManager(DataSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this.Settings = settings;
        }

        /// <summary>
        /// Gets or sets settings
        /// 获取或设置数据库连接设置
        /// </summary>
        protected DataSettings Settings { get; private set; }

        /// <summary>
        /// Load data provider
        /// 加载数据提供者
        /// </summary>
        /// <returns>Data provider</returns>
        public abstract IDataProvider LoadDataProvider();
    }
}
