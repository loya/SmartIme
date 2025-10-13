using System.Runtime.InteropServices;

public class GlobalKeyboardLayoutWatcher : IDisposable
{
    // 引入必要的Windows API
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);

    [DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint thread);

    private readonly System.Threading.Timer _timer;
    private IntPtr _currentLayout;

    public event Action<IntPtr> KeyboardLayoutChanged;

    public GlobalKeyboardLayoutWatcher()
    {
        _timer = new System.Threading.Timer(CheckLayout, null, 0, 500); // 每500毫秒检查一次
    }

    private void CheckLayout(object state)
    {
        try
        {
            IntPtr newLayout = GetCurrentKeyboardLayout();
            if (_currentLayout != newLayout)
            {
                _currentLayout = newLayout;
                KeyboardLayoutChanged?.Invoke(newLayout);
            }
        }
        catch
        {
            // 异常处理
        }
    }

    public IntPtr GetCurrentKeyboardLayout()
    {
        IntPtr foregroundWindow = GetForegroundWindow();
        uint foregroundThread = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
        return GetKeyboardLayout(foregroundThread);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}