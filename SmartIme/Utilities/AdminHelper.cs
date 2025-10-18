using System.Security.Principal;
using System.Diagnostics;
using System.Windows.Forms;

namespace SmartIme.Utilities
{
    public static class AdminHelper
    {
        /// <summary>
        /// 检查当前应用程序是否以管理员身份运行
        /// </summary>
        /// <returns>如果是管理员权限返回true，否则返回false</returns>
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// 以管理员权限重新启动当前应用程序
        /// </summary>
        public static void RestartAsAdministrator()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Application.ExecutablePath,
                UseShellExecute = true,
                Verb = "runas" // 以管理员权限运行
            };

            try
            {
                Process.Start(startInfo);
                Application.Exit(); // 关闭当前实例
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法以管理员权限重启应用程序: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}