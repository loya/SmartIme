using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SmartIme
{
    public partial class AddRuleForm : Form
    {
        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        public Rule CreatedRule { get; private set; }
        public string AppName { get; set; }

        public AddRuleForm(IEnumerable<object> imeList, int defaultImeIndex, string appName = null)
        {            
            InitializeComponent();
            this.cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;
            
            // 初始化输入法列表
            cmbIme.Items.AddRange(imeList.ToArray());
            if (cmbIme.Items.Count > 0)
            {
                cmbIme.SelectedIndex = defaultImeIndex;
            }
            
            // 初始化规则类型
            cmbType.Items.AddRange(new object[] { "程序名称", "窗口标题", "控件类型" });
            cmbType.SelectedIndex = 0;
            
            // 如果提供了应用名称，设置默认值
            if (!string.IsNullOrEmpty(appName))
            {
                AppName = appName;
                txtPattern.Text = appName;
                txtName.Text = $"{appName}规则";
            }
        }

        private void BtnSelectProcess_Click(object sender, EventArgs e)
        {
            if (cmbType.SelectedIndex == 1) // 窗口标题规则
            {
                ShowWindowTitlesSelection();
            }
            else if (cmbType.SelectedIndex == 2) // 控件类型规则
            {
                string controlClass = SelectControlClass();
                if (!string.IsNullOrEmpty(controlClass))
                {
                    txtPattern.Text = controlClass;
                }
            }
            else // 程序名称规则
            {
                ShowProcessSelection();
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private static extern IntPtr ChildWindowFromPointEx(IntPtr hwndParent, Point pt, uint uFlags);

        private const uint CWP_ALL = 0x0000;
        private const uint CWP_SKIPINVISIBLE = 0x0001;

        private static IntPtr hHook = IntPtr.Zero;
        private static string selectedControlClass = null;
        private static bool mouseClicked = false;

        private Form selectForm; // 添加字段引用

        private string SelectControlClass()
        {
            selectForm = new Form()
            {
                Text = "选择控件 - 点击左键选择目标控件",
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                Width = 400,
                Height = 200
            };

            var lblInstruction = new Label()
            {
                Text = "请将鼠标移动到目标控件上，然后点击左键选择",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            selectForm.Controls.Add(lblInstruction);

            // 设置鼠标钩子
            using (var hook = new MouseHook())
            {
                hook.MouseClick += OnMouseClick;
                mouseClicked = false;
                selectedControlClass = null;

                selectForm.ShowDialog();

                hook.MouseClick -= OnMouseClick;
            }

            return selectedControlClass;
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
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
                    IntPtr hParent = GetForegroundWindow();
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
                    // 获取控件类名
                    selectedControlClass = ControlHelper.GetWindowClassName(hWnd);
                    
                    // 显示控件信息
                }
                
                mouseClicked = true;

                selectForm.Close();
                //MessageBox.Show($"已选择控件类名: {selectedControlClass}",
                //    "控件选择", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        private class MouseHook : IDisposable
        {
            private const int WH_MOUSE_LL = 14;
            private IntPtr hookId = IntPtr.Zero;
            private NativeMethods.HookProc proc;

            public event MouseEventHandler MouseClick;

            public MouseHook()
            {
                proc = HookCallback;
                hookId = SetHook(proc);
            }

            private IntPtr SetHook(NativeMethods.HookProc proc)
            {
                using (var curModule = Process.GetCurrentProcess().MainModule)
                {
                    return SetWindowsHookEx(WH_MOUSE_LL, proc, 
                        GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
                {
                    var hookStruct = (NativeMethods.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MSLLHOOKSTRUCT));
                    MouseClick?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 1, 
                        hookStruct.pt.x, hookStruct.pt.y, 0));
                }
                return CallNextHookEx(hookId, nCode, wParam, lParam);
            }

            public void Dispose()
            {
                UnhookWindowsHookEx(hookId);
            }
        }

        private static class NativeMethods
        {
            public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

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
                public IntPtr dwExtraInfo;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, 
            NativeMethods.HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, 
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WM_LBUTTONDOWN = 0x0201;

        private void ShowProcessSelection()
        {
            var processForm = new Form()
            {
                Text = "选择运行中的应用程序",
                Size = new Size(400, 500),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent
            };

            var listBox = new ListBox()
            {
                Dock = DockStyle.Fill,
                SelectionMode = SelectionMode.One
            };

            var btnSelect = new Button()
            {
                Text = "选择",
                Dock = DockStyle.Bottom,
                Height = 40
            };

            // 获取所有非系统进程
            var processes = System.Diagnostics.Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.ProcessName) && p.MainWindowHandle != IntPtr.Zero)
                .Select(p => p.ProcessName)
                .Distinct()
                .OrderBy(p => p);

            listBox.Items.AddRange(processes.ToArray());

            btnSelect.Click += (s, args) =>
            {
                if (listBox.SelectedItem != null)
                {
                    txtPattern.Text = listBox.SelectedItem.ToString();
                    processForm.Close();
                }
            };

            processForm.Controls.Add(listBox);
            processForm.Controls.Add(btnSelect);
            processForm.ShowDialog(this);
        }

        private void ShowWindowTitlesSelection()
        {
            var titleForm = new Form()
            {
                Text = "选择窗口标题",
                Size = new Size(400, 500),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent
            };

            var listBox = new ListBox()
            {
                Dock = DockStyle.Fill,
                SelectionMode = SelectionMode.One
            };

            var btnSelect = new Button()
            {
                Text = "选择",
                Dock = DockStyle.Bottom,
                Height = 40
            };

            List<string> titles = GetCurrentAppTitles();

            if (titles.Count == 0)
            {
                MessageBox.Show("未找到该应用的窗口标题", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            listBox.Items.AddRange(titles.ToArray());

            btnSelect.Click += (s, args) =>
            {
                if (listBox.SelectedItem != null)
                {
                    txtPattern.Text = listBox.SelectedItem.ToString();
                    titleForm.Close();
                }
            };

            titleForm.Controls.Add(listBox);
            titleForm.Controls.Add(btnSelect);
            titleForm.ShowDialog(this);
        }

        private List<string> GetCurrentAppTitles()
        {
            // 获取当前应用的窗口标题
            string currentApp = AppName;
            var titles = System.Diagnostics.Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle) &&
                           p.MainWindowHandle != IntPtr.Zero &&
                           (string.IsNullOrEmpty(currentApp) || p.ProcessName == currentApp))
                .Select(p => p.MainWindowTitle)
                .Distinct()
                .OrderBy(p => p)
                .ToList();
            return titles;
        }

        private void CmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelectProcess.Text = cmbType.SelectedIndex == 1 ? "选择窗口标题" : "选择应用程序";
            btnSelectProcess.Visible = cmbType.SelectedIndex == 1 || cmbType.SelectedIndex == 2;
            
            // 当切换到窗口标题规则时，自动显示选择窗口标题对话框
            if (cmbType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtPattern.Text))
            {
                List<string> titles = GetCurrentAppTitles();

                if (titles.Count > 0)
                {
                    // 设置第一个标题为默认值
                    txtPattern.Text = titles[0];
                }
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPattern.Text))
            {
                MessageBox.Show("请填写规则名称和匹配模式", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RuleType type;
            switch (cmbType.SelectedIndex)
            {
                case 1:
                    type = RuleType.Title;
                    break;
                case 2:
                    type = RuleType.Control;
                    break;
                default:
                    type = RuleType.Program;
                    break;
            }

            CreatedRule = new Rule(txtName.Text, type, txtPattern.Text, cmbIme.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}