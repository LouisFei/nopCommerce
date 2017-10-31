using System;
using System.Collections.Generic;

namespace Nop.Core.Data
{
    /// <summary>
    /// Data settings (connection string information)
    /// 数据库设置（连接字符串）
    /// </summary>
    public partial class DataSettings
    {
        /// <summary>
        /// Ctor 实例化数据库设置对象
        /// </summary>
        public DataSettings()
        {
            RawDataSettings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Data provider
        /// 数据提供者（ms sqlserver, my sql, oracle，……）
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        /// Connection string
        /// 数据库连接字符串
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Raw settings file
        /// 原始的设置信息
        /// </summary>
        public IDictionary<string, string> RawDataSettings { get; private set; }

        /// <summary>
        /// A value indicating whether entered information is valid
        /// 设置是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !String.IsNullOrEmpty(this.DataProvider) && !String.IsNullOrEmpty(this.DataConnectionString);
        }
    }
}
