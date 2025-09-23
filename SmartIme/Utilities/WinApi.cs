using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace SmartIme.Utilities
{
    internal static partial class WinApi
    {
        // Input Method APIs
        [DllImport("imm32.dll")]
        public static extern nint ImmGetDefaultIMEWnd(nint hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern nint SendMessage(nint hWnd, uint Msg, nint wParam, nint lParam);

        // Window and Process APIs
        [DllImport("user32.dll")]
        public static extern nint GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern nint GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetClassName(nint hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern nint GetFocus();

        [DllImport("user32.dll")]
        public static extern nint WindowFromPoint(Point point);

        [DllImport("user32.dll")]
        public static extern nint GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(nint hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern nint ChildWindowFromPointEx(nint hWndParent, Point point, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(nint hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(nint hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(nint hWnd);

        // Hook APIs
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint SetWindowsHookEx(int idHook, HookProc lpfn, nint hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(nint hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint GetModuleHandle(string lpModuleName);

        // GDI APIs for drawing
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern nint GetWindowDC(nint hWnd);

        [DllImport("gdi32.dll")]
        public static extern nint CreatePen(int fnPenStyle, int nWidth, uint crColor);

        [DllImport("gdi32.dll")]
        public static extern nint SelectObject(nint hdc, nint hgdiobj);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Rectangle(nint hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(nint hObject);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(nint hWnd, nint hDC);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InvalidateRect(nint hWnd, IntPtr lpRect, [MarshalAs(UnmanagedType.Bool)] bool bErase);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UpdateWindow(nint hWnd);

        [DllImport("gdi32.dll")]
        public static extern nint GetStockObject(int fnObject);

        // Constants        
        public const int SW_RESTORE = 9;
        public const int PS_SOLID = 0;
        public const int NULL_BRUSH = 5;
        public const int WH_MOUSE_LL = 14;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_XBUTTONDOWN = 0x020B;
        public const int WM_XBUTTONUP = 0x020C;
        public const int WM_MOUSEHWHEEL = 0x020E;

        public const uint CWP_ALL = 0x0000;
        public const uint CWP_SKIPINVISIBLE = 0x0001;

        // Delegates
        public delegate nint HookProc(int nCode, nint wParam, nint lParam);

        // Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public nint dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}