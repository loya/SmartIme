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

                // 处理命令行参数
                bool startMinimized = args.Length > 0 && args.Contains("-minimized", StringComparer.OrdinalIgnoreCase);

                var mainForm = new MainForm();

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