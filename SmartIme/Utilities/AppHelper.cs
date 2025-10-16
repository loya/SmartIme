using Interop.UIAutomationClient;
using SmartIme.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static SmartIme.Utilities.WinApi;

namespace SmartIme.Utilities
{
    public static class AppHelper
    {
        /// <summary>
        /// Adds a rule node to the specified group node.
        /// </summary>
        public static void AddRuleNodeToGroup(TreeNode ruleGroupNode, Rule rule, Font font
            )
        {

            Color ruleColor = rule.RuleType switch
            {
                RuleType.程序名称 => Color.DarkSeaGreen,
                RuleType.窗口标题 => Color.DarkCyan,
                RuleType.控件 => Color.DeepSkyBlue,
                _ => Color.Black
            };

            var ruleNode = new TreeNode(rule.ToString())
            {
                Tag = rule,
                ForeColor = ruleColor,
                NodeFont = font
            };
            ruleGroupNode.Nodes.Add(ruleNode);
        }

        /// <summary>
        /// 获取当前焦点控件的类名
        /// </summary>
        /// <returns>控件类名，如果获取失败返回空字符串</returns>
        public static string GetFocusedControlName()
        {

            var automation = new CUIAutomation();
            IUIAutomationElement element = null;
            try
            {


                element = automation.GetFocusedElement();

                var automationId = string.IsNullOrEmpty(element.CurrentAutomationId) ? null : element.CurrentAutomationId;
                var classname = string.IsNullOrEmpty(element.CurrentClassName) ? null : element.CurrentClassName;
                var name = element.CurrentName;
                var className = ((string.IsNullOrEmpty(name) ? "" : $"{name}:") +
                        (string.IsNullOrEmpty(classname) ? "" : $"{classname}:") +
                        (string.IsNullOrEmpty(automationId) ? "" : $"{automationId}")).TrimEnd(':');
                if (!string.IsNullOrEmpty(className))
                {
                    return className;
                }

                // 获取鼠标位置
                GetCursorPos(out Point mousePos);

                // 尝试获取焦点控件
                var hWnd = GetGlobalFocusWindow();

                if (hWnd != IntPtr.Zero)
                {
                    className = GetWindowClassName(hWnd);
                    if (!string.IsNullOrEmpty(className))
                    {
                        return className;
                    }
                }
                return string.Empty;


            }
            catch (Exception ex)
            {
                // 忽略所有异常，返回空字符串
                AppHelper.LogToFile($"GetFocusedControlName 异常: {ex.Message}", "error", true);
                return string.Empty;
            }
            finally
            {
                if (element != null)
                    Marshal.ReleaseComObject(element);
                Marshal.ReleaseComObject(automation);
            }

        }

        /// <summary>
        /// 获取指定窗口的类名
        /// </summary>
        /// <param processName="hWnd">窗口句柄</param>
        /// <returns>窗口类名，如果获取失败返回空字符串</returns>
        public static string GetWindowClassName(nint hWnd)
        {
            try
            {
                if (hWnd != nint.Zero)
                {
                    var className = new StringBuilder(256);
                    _ = WinApi.GetClassName(hWnd, className, className.Capacity);
                    return className.ToString();
                }
            }
            catch
            {
                // 忽略所有异常，返回空字符串
                throw;
            }

            return string.Empty;
        }

        public static string GetForegroundProcessName()
        {
            try
            {
                var hWnd = WinApi.GetForegroundWindow();
                if (hWnd != nint.Zero)
                {
                    _ = WinApi.GetWindowThreadProcessId(hWnd, out uint processId);
                    string processName = System.Diagnostics.Process.GetProcessById((int)processId).ProcessName;
                    return processName;
                }
            }
            catch
            {
                throw;
            }

            return string.Empty;
        }

        public static IntPtr GetGlobalFocusWindow()
        {
            GUITHREADINFO guiThreadInfo = new GUITHREADINFO();

            guiThreadInfo.cbSize = (uint)Marshal.SizeOf(typeof(WinApi.GUITHREADINFO));

            // 获取前台窗口（用户正在交互的窗口）的线程ID
            var a = GetWindowThreadProcessId(GetForegroundWindow(), out uint foregroundThreadID);

            if (GetGUIThreadInfo(a, ref guiThreadInfo))
            {
                return guiThreadInfo.hwndFocus;  // 返回该线程中的焦点窗口
            }

            return IntPtr.Zero;
        }


        private static string GetWindowTitle(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return "无效窗口";

            StringBuilder sb = new StringBuilder(256);
            WinApi.GetWindowText(hWnd);
            string title = sb.ToString();
            return string.IsNullOrEmpty(title) ? "无标题窗口" : title;
        }

        public static void LogToFile(string message, string level = "info", bool foceDebug = false)
        {
#if !DEBUG && !foceDebug
            return; // 非调试模式下不记录日志
#endif
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "position_debug.log");
                string logMessage = $"{level}: {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // 忽略日志写入错误
            }
        }

        /// <summary>
        /// 验证并调整坐标
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Point ValidateAndAdjustPosition(Point position)
        {
            // 获取屏幕边界
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

            // 获取浮动提示窗口的预估尺寸
            int hintWidth = 120;
            int hintHeight = 40;

            // 记录原始坐标
            Point originalPosition = position;

            // 获取当前活动窗口信息
            IntPtr foregroundWindow = WinApi.GetForegroundWindow();
            //IntPtr foregroundWindow = AppHelper.GetGlobalFocusWindow();
            if (foregroundWindow != IntPtr.Zero)
            {
                string windowTitle = GetWindowTitle(foregroundWindow);
                WinApi.GetWindowRect(foregroundWindow, out WinApi.RECT windowRect);

                // 检查窗口是否有效（非零尺寸）
                bool isWindowValid = !(windowRect.left == 0 && windowRect.top == 0 &&
                                     windowRect.right == 0 && windowRect.bottom == 0);

                // 记录窗口信息用于调试到文件
                LogToFile($"活动窗口: {windowTitle}, 位置: [{windowRect.left},{windowRect.top}-{windowRect.right},{windowRect.bottom}], 有效: {isWindowValid}");
                LogToFile($"原始坐标: {originalPosition}");

                // 检查坐标是否在当前活动窗口内（仅当窗口有效时）
                if (isWindowValid &&
                    windowRect.left <= position.X && position.X <= windowRect.right &&
                    windowRect.top <= position.Y && position.Y <= windowRect.bottom)
                {
                    LogToFile($"坐标在有效活动窗口内，保持原位置: {position}");
                    return position;
                }
                else if (isWindowValid)
                {
                    Point cursourPoint;
                    // 判断鼠标位置是否在窗口内
                    GetCursorPos(out cursourPoint);
                    if ((windowRect.left <= cursourPoint.X && cursourPoint.X <= windowRect.right &&
                        windowRect.top <= cursourPoint.Y && cursourPoint.Y <= windowRect.bottom))
                    {
                        LogToFile($"鼠标坐标在有效活动窗口内,返回鼠标坐标: {cursourPoint}");
                        cursourPoint.X += 10;
                        cursourPoint.Y -= 40;
                        return cursourPoint;
                    }

                    // 如果窗口有效，但坐标不在窗口内，设置坐标未窗口中心
                    LogToFile($"坐标不在有效活动窗口内，设置坐标为窗口中心: {position}");
                    return new Point(windowRect.left + (windowRect.right - windowRect.left) / 2,
                                     windowRect.top + (windowRect.bottom - windowRect.top) / 2);

                }

                // 如果窗口无效，使用原始坐标但进行屏幕边界检查
                if (!isWindowValid)
                {
                    LogToFile($"活动窗口无效，使用原始坐标并进行屏幕边界检查");
                }
                else
                {
                    LogToFile($"坐标不在活动窗口内，使用原始坐标并进行屏幕边界检查");
                }
            }
            else
            {
                LogToFile($"无法获取活动窗口，使用原始坐标并进行屏幕边界检查");
            }

            // 屏幕边界检查
            Point adjustedPosition = originalPosition;

            // 检查坐标是否在屏幕范围内
            if (adjustedPosition.X < screenBounds.Left)
            {
                adjustedPosition.X = screenBounds.Left + 10;
            }
            else if (adjustedPosition.X + hintWidth > screenBounds.Right)
            {
                adjustedPosition.X = screenBounds.Right - hintWidth - 10;
            }

            if (adjustedPosition.Y < screenBounds.Top)
            {
                adjustedPosition.Y = screenBounds.Top + 10;
            }
            else if (adjustedPosition.Y + hintHeight > screenBounds.Bottom)
            {
                adjustedPosition.Y = screenBounds.Bottom - hintHeight - 10;
            }

            LogToFile($"最终调整坐标: {adjustedPosition}");
            return adjustedPosition;
        }

        public static bool TryGetValidForegroundWindow(out IntPtr hWnd)
        {
            hWnd = GetForegroundWindow();
            if (hWnd == IntPtr.Zero) return false;

            // 检查窗口是否可见且启用
            if (!WinApi.IsWindowVisible(hWnd) || !WinApi.IsWindowEnabled(hWnd)) return false;

            // 可选：读取窗口标题，确认非系统内部窗口
            var title = WinApi.GetWindowText(hWnd);
            if (string.IsNullOrWhiteSpace(title))
                return false;

            // todo 可选：获取所属进程ID，避免操作系统服务窗口
            // GetWindowThreadProcessId(hWnd, out uint pid);
            // if (IsSystemProcess(pid)) return false;

            return true;
        }

        private static bool IsSystemProcess(uint pid)
        {
            // 简化判断：排除常见系统PID范围
            return pid < 1000;
        }


        /// <summary>
        /// 强制将窗口置顶到前台
        /// </summary>
        /// <param name="hWnd"></param>
        public static void ForceForegroundWindow(IntPtr hWnd)
        {
            IntPtr foreHwnd = GetForegroundWindow();
            uint foreThread = GetWindowThreadProcessId(foreHwnd, IntPtr.Zero);
            uint appThread = WinApi.GetCurrentThreadId();

            // 将当前线程与前台线程的输入队列临时连接
            WinApi.AttachThreadInput(appThread, foreThread, true);
            SetForegroundWindow(hWnd);
            AttachThreadInput(appThread, foreThread, false);
        }

        /// <summary>
        /// 尝试将窗口强制置顶到前台。
        /// </summary>
        public static void BringWindowToFront(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return;

            // 如果窗口被最小化，先恢复
            ShowWindow(hWnd, SW_RESTORE);

            // 尝试使用 AttachThreadInput 提升前台权限
            IntPtr foreHwnd = GetForegroundWindow();
            uint foreThread = GetWindowThreadProcessId(foreHwnd, IntPtr.Zero);
            uint appThread = GetCurrentThreadId();

            bool attached = false;
            try
            {
                if (foreThread != appThread)
                {
                    attached = AttachThreadInput(appThread, foreThread, true);
                }

                // 尝试直接置顶
                if (!SetForegroundWindow(hWnd))
                {
                    // 如果失败，尝试模拟 ALT 键
                    keybd_event(VK_MENU, 0, 0, 0);
                    SetForegroundWindow(hWnd);
                    keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, 0);
                }
            }
            finally
            {
                if (attached)
                {
                    AttachThreadInput(appThread, foreThread, false);
                }
            }

            // 如果还是没置顶，强制用 SetWindowPos 物理置顶
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

            // 再次尝试获取焦点
            SetForegroundWindow(hWnd);
        }


    }

}