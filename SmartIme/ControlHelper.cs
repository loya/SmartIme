using System;
using System.Runtime.InteropServices;
using System.Drawing;
using Interop.UIAutomationClient;
using System.Text;

namespace SmartIme
{
    public static class ControlHelper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, Point point, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        private const uint CWP_SKIPINVISIBLE = 0x0001;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// 获取当前焦点控件的类名
        /// </summary>
        /// <returns>控件类名，如果获取失败返回空字符串</returns>
        public static string GetFocusedControlClassName()
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

                if (hWnd != IntPtr.Zero)
                {
                    var element= new CUIAutomation().GetFocusedElement();
                    var automationId = string.IsNullOrEmpty(element.CurrentAutomationId)?null:element.CurrentAutomationId;
                    var classname = string.IsNullOrEmpty(element.CurrentClassName)?null:element.CurrentClassName;
                    var name = element.CurrentName;
                    return automationId??name??classname;

                    // 获取控件类名
                    var className = new StringBuilder(256);
                    GetClassName(hWnd, className, className.Capacity);
                    return className.ToString();
                }
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
        /// <param name="hWnd">窗口句柄</param>
        /// <returns>窗口类名，如果获取失败返回空字符串</returns>
        public static string GetWindowClassName(IntPtr hWnd)
        {
            try
            {
                if (hWnd != IntPtr.Zero)
                {
                    var className = new StringBuilder(256);
                    GetClassName(hWnd, className, className.Capacity);
                    return className.ToString();
                }
            }
            catch
            {
                // 忽略所有异常，返回空字符串
            }

            return string.Empty;
        }
    }
}