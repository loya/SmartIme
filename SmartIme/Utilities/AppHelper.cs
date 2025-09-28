using Interop.UIAutomationClient;
using SmartIme.Models;
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
        public static void AddRuleNodeToGroup(TreeNode groupNode, Rule rule)
        {
            Color ruleColor = rule.Type switch
            {
                RuleType.Program => Color.DarkSeaGreen,
                RuleType.Title => Color.DarkCyan,
                RuleType.Control => Color.DeepSkyBlue,
                _ => Color.Black
            };

            var ruleNode = new TreeNode(rule.ToString())
            {
                Tag = rule,
                ForeColor = ruleColor,
            };
            groupNode.Nodes.Add(ruleNode);
        }

        /// <summary>
        /// 获取当前焦点控件的类名
        /// </summary>
        /// <returns>控件类名，如果获取失败返回空字符串</returns>
        public static string GetFocusedControlName()
        {
            try
            {
                // 获取鼠标位置
                Point mousePos;
                GetCursorPos(out mousePos);

                // 尝试获取焦点控件
                IntPtr hFocus = GetFocus();
                IntPtr hWnd;

                if (hFocus != IntPtr.Zero)
                {
                    // 使用焦点控件
                    hWnd = hFocus;
                }
                else
                {
                    // 如果没有焦点控件，使用鼠标下的窗口
                    hWnd = WindowFromPoint(mousePos);

                    // 尝试获取更精确的子控件
                    IntPtr hParent = GetActiveWindow();
                    if (hParent != IntPtr.Zero)
                    {
                        // 将屏幕坐标转换为窗口客户区坐标
                        POINT clientPoint = new POINT { x = mousePos.X, y = mousePos.Y };
                        ScreenToClient(hParent, ref clientPoint);

                        // 获取指定坐标下的子控件
                        IntPtr hChild = ChildWindowFromPointEx(hParent,
                            new Point(clientPoint.x, clientPoint.y), CWP_SKIPINVISIBLE);

                        if (hChild != IntPtr.Zero && hChild != hParent)
                        {
                            hWnd = hChild;
                        }
                    }
                }

                //if (hWnd != IntPtr.Zero)
                //{
                var element = new CUIAutomation().GetFocusedElement();
                var automationId = string.IsNullOrEmpty(element.CurrentAutomationId) ? null : element.CurrentAutomationId;
                var classname = string.IsNullOrEmpty(element.CurrentClassName) ? null : element.CurrentClassName;
                var name = element.CurrentName;
                return ((string.IsNullOrEmpty(name) ? "" : $"{name}:") +
                    (string.IsNullOrEmpty(classname) ? "" : $"{classname}:") +
                    (string.IsNullOrEmpty(automationId) ? "" : $"{automationId}")).TrimEnd(':');

                // 获取控件类名
                //var windowText = new StringBuilder(256);
                //GetClassName(hWnd, windowText, windowText.Capacity);
                //return windowText.ToString();
                //}
            }
            catch
            {
                // 忽略所有异常，返回空字符串
            }

            return string.Empty;
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

        public static string GetActiveWindowProcessName()
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
            WinApi.GetWindowText(hWnd, sb, sb.Capacity);
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

    }

}