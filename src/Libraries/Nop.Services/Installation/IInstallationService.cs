
namespace Nop.Services.Installation
{
    /// <summary>
    /// 数据库安装服务
    /// </summary>
    public partial interface IInstallationService
    {
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="defaultUserEmail">默认的管理员帐户电子邮箱</param>
        /// <param name="defaultUserPassword">默认的管理员帐户密码</param>
        /// <param name="installSampleData">是否安装示例数据</param>
        void InstallData(string defaultUserEmail, string defaultUserPassword, bool installSampleData = true);
    }
}
