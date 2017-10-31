using System;
using Nop.Core;
using Nop.Core.Data;

namespace Nop.Data
{
    /// <summary>
    /// 数据库提供者管理器
    /// </summary>
    public partial class EfDataProviderManager : BaseDataProviderManager
    {
        /// <summary>
        /// 实例化数据库提供者管理器对象
        /// </summary>
        /// <param name="settings">数据库连接字符串</param>
        public EfDataProviderManager(DataSettings settings):base(settings)
        {
        }

        /// <summary>
        /// 加载数据提供者
        /// </summary>
        /// <returns></returns>
        public override IDataProvider LoadDataProvider()
        {
            var providerName = Settings.DataProvider;

            if (String.IsNullOrWhiteSpace(providerName))
            {
                throw new NopException("Data Settings doesn't contain a providerName");
            }

            switch (providerName.ToLowerInvariant())
            {
                case "sqlserver":
                    return new SqlServerDataProvider();
                case "sqlce":
                    return new SqlCeDataProvider();
                default:
                    throw new NopException(string.Format("Not supported dataprovider name: {0}", providerName));
            }
        }

    }
}
