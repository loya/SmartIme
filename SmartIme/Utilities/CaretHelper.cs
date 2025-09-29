using System.Runtime.InteropServices;
using System.Text;

namespace SmartIme.Utilities
{
    public class CaretHelper
    {

        public static Point GetBestFloatingHintPosition()
        {
            Point? caretPosition = GetCaretPositionFromActiveWindow();
            if (caretPosition.HasValue)
            {
                if (caretPosition.Value != Point.Empty)
                    return new Point(caretPosition.Value.X + 5, caretPosition.Value.Y - 40);
            }

            Point cursorPos = Cursor.Position;
            return new Point(cursorPos.X + 15, cursorPos.Y - 40);
        }

        private static Point? GetCaretPositionFromActiveWindow()
        {
            IntPtr hWnd = WinApi.GetFocus();
            if (hWnd == IntPtr.Zero)
            {
                //    hWnd = AppHelper.GetGlobalFocusWindow();
                //}
                //if (hWnd == IntPtr.Zero)
                //{

                hWnd = WinApi.GetForegroundWindow();
                if (hWnd == IntPtr.Zero)
                {
                    return null;
                }
            }

            return TryGetCaretPositionWithRetry(hWnd);
        }




        private static Point? TryGetCaretPositionWithRetry(IntPtr hWnd)
        {
            System.Threading.Thread.Sleep(30);
            // 首先尝试使用GetGUIThreadInfo API获取更准确的插入符位置
            Point? caretPos = GetCaretPositionUsingGUIThreadInfo(hWnd);
            if (caretPos.HasValue)
            {
                return caretPos;
            }

            // 如果GetGUIThreadInfo失败，回退到原来的GetCaretPos方法
            for (int i = 0; i < 5; i++)
            {
                if (WinApi.GetCaretPos(out Point caretPos2))
                {
                    if (caretPos2.X >= 0 && caretPos2.Y >= 0)
                    {
                        Point? screenPos = ConvertClientToScreen(hWnd, caretPos2);
                        if (screenPos.HasValue)
                        {
                            return screenPos;
                        }
                    }
                }

                System.Threading.Thread.Sleep(30);
            }

            return null;
        }


        private static Point? GetCaretPositionUsingGUIThreadInfo(IntPtr hWnd)
        {
            try
            {
                uint threadId = WinApi.GetWindowThreadProcessId(hWnd, IntPtr.Zero);
                if (threadId == 0)
                {
                    AppHelper.LogToFile($"GUIThreadInfo: 无法获取线程ID (窗口: {hWnd})", "error", true);
                    return null;
                }

                WinApi.GUITHREADINFO threadInfo = new WinApi.GUITHREADINFO();
                threadInfo.cbSize = (uint)Marshal.SizeOf(typeof(WinApi.GUITHREADINFO));

                if (WinApi.GetGUIThreadInfo(threadId, ref threadInfo))
                {
                    string debugInfo = $"GUIThreadInfo: 线程={threadId}, 焦点窗口={threadInfo.hwndFocus}, 插入符窗口={threadInfo.hwndCaret}";

                    if (threadInfo.hwndCaret != IntPtr.Zero)
                    {
                        debugInfo += $", 插入符位置=({threadInfo.rcCaret.left},{threadInfo.rcCaret.top})";
                        AppHelper.LogToFile(debugInfo);

                        if (threadInfo.rcCaret.left >= 0 && threadInfo.rcCaret.top >= 0)
                        {
                            // 插入符位置是相对于插入符窗口的客户端坐标
                            Point caretPos = new Point(threadInfo.rcCaret.left, threadInfo.rcCaret.top);
                            Point? screenPos = ConvertClientToScreen(threadInfo.hwndCaret, caretPos);

                            if (screenPos.HasValue)
                            {
                                debugInfo += $", 屏幕位置=({screenPos.Value.X},{screenPos.Value.Y})";
                                AppHelper.LogToFile(debugInfo);
                            }

                            return screenPos;
                        }
                    }
                    else
                    {
                        AppHelper.LogToFile(debugInfo + ", 无插入符窗口");
                    }
                }
                else
                {
                    AppHelper.LogToFile($"GUIThreadInfo: API调用失败 (线程: {threadId})", "error", true);
                }
            }
            catch (Exception ex)
            {
                AppHelper.LogToFile($"GUIThreadInfo异常: {ex.Message}", "error", true);
            }

            return null;
        }

        private static Point? ConvertClientToScreen(IntPtr hWnd, Point clientPoint)
        {
            try
            {
                // 方法1: 使用标准的ClientToScreen API
                Point screenPoint = clientPoint;
                string info = "";
                if (WinApi.ClientToScreen(hWnd, ref screenPoint))
                {
                    info += $" 方法1成功: ({screenPoint.X},{screenPoint.Y})";
                    return screenPoint;
                }

                // 方法2: 使用POINT结构体的ClientToScreen
                WinApi.POINT point = new WinApi.POINT { x = clientPoint.X, y = clientPoint.Y };
                if (WinApi.ClientToScreen(hWnd, ref point))
                {
                    info += $" 方法2成功: ({point.x},{point.y})";
                    AppHelper.LogToFile(info);
                    return new Point(point.x, point.y);
                }

                // 方法3: 手动计算窗口位置 + 客户端坐标
                if (WinApi.GetWindowRect(hWnd, out WinApi.RECT windowRect))
                {
                    Point manualPoint = new Point(windowRect.left + clientPoint.X, windowRect.top + clientPoint.Y);
                    info += $" 方法3: 手动计算({manualPoint.X},{manualPoint.Y})";

                    // 验证坐标是否在屏幕范围内
                    if (IsPointOnScreen(manualPoint))
                    {
                        return manualPoint;
                    }
                    else
                    {
                        info += " 坐标超出屏幕范围";
                    }
                    AppHelper.LogToFile(info);
                }

                // 方法4: 如果插入符窗口不是目标窗口，尝试获取焦点窗口
                IntPtr focusWnd = WinApi.GetFocus();
                if (focusWnd != hWnd && focusWnd != IntPtr.Zero)
                {
                    AppHelper.LogToFile($" 尝试焦点窗口: {focusWnd}");
                    return ConvertClientToScreen(focusWnd, clientPoint);
                }

                // 方法5: 使用鼠标位置作为备选方案
                if (WinApi.GetCursorPos(out Point cursorPos))
                {
                    AppHelper.LogToFile($" 使用鼠标位置: ({cursorPos.X},{cursorPos.Y})");
                    return cursorPos;
                }
            }
            catch (Exception ex)
            {
                AppHelper.LogToFile($" 坐标转换异常: {ex.Message}", "error");
            }

            return null;
        }

        private static bool IsPointOnScreen(Point point)
        {
            // 检查坐标是否在屏幕范围内
            return point.X >= 0 && point.Y >= 0 &&
                   point.X <= Screen.PrimaryScreen.Bounds.Width &&
                   point.Y <= Screen.PrimaryScreen.Bounds.Height;
        }

        static string currentInputMethod = "";
        public static bool MonitorInputMethodSwitch(out string newImeName, out string processName)
        {
            try
            {
                string newInputMethod = GetCurrentInputMethod();

                // 调试输出

                //string winTitle = WinApi.GetWindowText(WinApi.GetForegroundWindow());
                processName = AppHelper.GetForegroundProcessName();
                if (processName == "explorer")
                {
                    // 如果是资源管理器，直接返回不处理
                    newImeName = "";
                    return false;
                }

                if (!string.IsNullOrEmpty(newInputMethod) && newInputMethod != currentInputMethod)
                {
                    newImeName = newInputMethod;

                    // 如果设置不显示英文输入法切换，且新输入法是英文，则跳过
                    //if (!config.ShowEnglishInputMethod &&
                    //    (newInputMethod.Contains("英语") || newInputMethod.Contains("English") || newInputMethod.Contains("美国")))
                    //{
                    //    if (config.EnableDebugMode)
                    //    {
                    //        Console.WriteLine("跳过英文输入法显示");
                    //    }
                    //    currentInputMethod = newInputMethod;
                    //    return;
                    //}



                    currentInputMethod = newInputMethod;
                    return true;
                }
                else
                {
                    newImeName = "";
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static string GetCurrentInputMethod()
        {
            try
            {
                IntPtr foregroundWindow = WinApi.GetForegroundWindow();
                if (foregroundWindow == IntPtr.Zero) return "";
                //打印foregroundWindow窗口名称

                string windowTitle = WinApi.GetWindowText(foregroundWindow);
                //Console.WriteLine($"{DateTime.Now} {windowTitle}");
                uint threadId = WinApi.GetWindowThreadProcessId(foregroundWindow, out uint processId);
                IntPtr keyboardLayout = WinApi.GetKeyboardLayout(threadId);

                if (keyboardLayout == IntPtr.Zero) return "";

                // 获取语言ID
                uint langId = (uint)(keyboardLayout.ToInt64() & 0xFFFF);

                StringBuilder langName = new StringBuilder(256);
                int result = WinApi.GetLocaleInfo(langId, WinApi.LOCALE_SLANGUAGE, langName, langName.Capacity);

                //Debug.WriteLine($"检测到的语言ID: {langId:X4}  {langName}");
                //Console.WriteLine($"检测到的语言ID: {langId:X4}, 语言名称: {langName}");
                if (result > 0)
                {
                    var lang = GetInputMethodNameById(langId);
                    if (!string.IsNullOrEmpty(lang))
                    {
                        return lang;
                    }
                    return langName.ToString();
                }

                // 如果获取不到语言名称，尝试获取英文名称
                result = WinApi.GetLocaleInfo(langId, WinApi.LOCALE_SENGLANGUAGE, langName, langName.Capacity);
                if (result > 0)
                {
                    return langName.ToString();
                }

                // 根据常见的输入法布局ID返回中文名称
                return GetInputMethodNameById(langId);
            }
            catch (Exception ex)
            {
                throw;
                return $"检测失败: {ex.Message}";
            }
        }

        private static string GetInputMethodNameById(uint langId)
        {
            return langId switch
            {
                0x0804 => "中文",
                0x0404 => "中文(繁体)",
                0x0409 => "英文",
                0x0809 => "英文",
                0x0411 => "日语",
                0x0412 => "韩语",
                //_ => $"未知输入法 (ID: {langId:X4})"
                _ => ""
            };
        }
    }
}
