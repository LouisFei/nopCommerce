
namespace Nop.Services.Installation
{
    /// <summary>
    /// ���ݿⰲװ����
    /// </summary>
    public partial interface IInstallationService
    {
        /// <summary>
        /// �������ݿ�
        /// </summary>
        /// <param name="defaultUserEmail">Ĭ�ϵĹ���Ա�ʻ���������</param>
        /// <param name="defaultUserPassword">Ĭ�ϵĹ���Ա�ʻ�����</param>
        /// <param name="installSampleData">�Ƿ�װʾ������</param>
        void InstallData(string defaultUserEmail, string defaultUserPassword, bool installSampleData = true);
    }
}
