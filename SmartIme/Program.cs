using SmartIme.Utilities;

namespace SmartIme
{
    static class Program
    {
        // 定义一个全局互斥体，用于防止应用程序重复运行
        private static Mutex _mutex = null;
        private const string MutexName = "SmartImeApplicationMutex";

        [STAThread]
        static void Main(string[] args)
        {
            // 检查是否以管理员权限运行
            if (!AdminHelper.IsAdministrator())
            {
                // 提示用户需要管理员权限
                var result = MessageBox.Show(
                    "此应用程序需要管理员权限才能正常工作。\n\n单击“是”将以管理员权限重新启动应用程序。\n\n单击“否”将退出应用程序。", 
                    "需要管理员权限", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // 以管理员权限重新启动
                    AdminHelper.RestartAsAdministrator();
                    return;
                }
                else
                {
                    // 用户选择不以管理员权限运行，则退出程序
                    return;
                }
            }

            // 检查应用程序是否已经在运行
            bool createdNew;
            _mutex = new Mutex(true, MutexName, out createdNew);

            if (!createdNew)
            {
                // 应用程序已经在运行，激活已有实例
                ActivateExistingInstance();
                return; // 退出当前实例
            }

            // 继续运行新实例
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //检查settings文件夹是否存在，不存在则创建
                if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings")))
                {
                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings"));
                }

                var mainForm = new MainForm();
                
                // 检查是否需要启动时最小化 - 优先检查命令行参数，然后检查设置
                bool startMinimized = args.Length > 0 && args.Contains("-minimized", StringComparer.OrdinalIgnoreCase);
                if (!startMinimized)
                {
                    // 如果没有命令行参数，则检查设置
                    startMinimized = mainForm.AppSettings.StartMinimized;
                }

                // 如果指定了-minimized参数，则启动时最小化到托盘
                if (startMinimized)
                {
                    mainForm.WindowState = FormWindowState.Minimized;
                    mainForm.ShowInTaskbar = false;
                    // mainForm.Hide();
                }

                Application.Run(mainForm);
            }
            finally
            {
                // 确保释放互斥体
                _mutex.ReleaseMutex();
            }
        }

        private static void ActivateExistingInstance()
        {
            // 查找已运行的实例窗口
            IntPtr hWnd = WinApi.FindWindow(null, "输入法智能切换助手");
            if (hWnd != IntPtr.Zero)
            {
                // 如果窗口最小化，则恢复
                if (WinApi.IsIconic(hWnd))
                {
                    WinApi.ShowWindow(hWnd, WinApi.SW_RESTORE);
                }

                // 将窗口置于前台
                WinApi.SetForegroundWindow(hWnd);
            }
        }
    }
}