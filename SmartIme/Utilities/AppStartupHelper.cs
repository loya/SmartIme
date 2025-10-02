using System.Diagnostics;

namespace SmartIme.Utilities
{
    internal class AppStartupHelper
    {
        public static bool IsAppSetToStartup()
        {
            // 方法1: 使用当前用户注册表
            //try
            //{
            //    using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
            //    {
            //        string appName = Assembly.GetExecutingAssembly().GetName().Name;
            //        return key?.GetValue(appName) != null;
            //    }
            //}
            //catch
            //{
            //    // 如果注册表方法失败，检查启动文件夹
            //    return IsStartupShortcutExists();
            //}
            return IsStartupShortcutExists();
        }

        public static void SetAppStartup(bool enable)
        {
            string appName = Application.ProductName;
            string appPath = Application.ExecutablePath;

            // 方法1: 尝试使用当前用户注册表（无需管理员权限）
            //bool registrySuccess = TrySetRegistryStartup(appName, appPath, enable);

            //if (!registrySuccess)
            //{
            //    // 方法2: 使用启动文件夹
            //    SetStartupFolderShortcut(appName, appPath, enable);
            //}
            SetStartupFolderShortcut(appName, appPath, enable);
        }

        private bool TrySetRegistryStartup(string appName, string appPath, bool enable)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key == null) return false;

                    if (enable)
                    {
                        key.SetValue(appName, $"\"{appPath}\" -minimized");
                    }
                    else
                    {
                        key.DeleteValue(appName, false);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"注册表方式设置开机自启动失败: {ex.Message}");
                return false;
            }
        }

        static void SetStartupFolderShortcut(string appName, string appPath, bool enable)
        {
            try
            {
                string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupFolder, $"{appName}.lnk");

                if (enable)
                {
                    CreateShortcut(shortcutPath, appPath, "-minimized");
                }
                else
                {
                    if (File.Exists(shortcutPath))
                    {
                        File.Delete(shortcutPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置开机自启动失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static bool IsStartupShortcutExists()
        {
            try
            {
                string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupFolder, $"{Application.ProductName}.lnk");
                return File.Exists(shortcutPath);
            }
            catch
            {
                return false;
            }
        }

        static void CreateShortcut(string shortcutPath, string targetPath, string arguments = "")
        {
            try
            {
                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);
                dynamic shortcut = shell.CreateShortcut(shortcutPath);

                shortcut.TargetPath = targetPath;
                shortcut.Arguments = arguments;
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.Description = "智能输入法切换助手";
                shortcut.Save();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(shortcut);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(shell);
            }
            catch (Exception ex)
            {
                throw new Exception($"创建快捷方式失败: {ex.Message}", ex);
            }
        }
    }
}
