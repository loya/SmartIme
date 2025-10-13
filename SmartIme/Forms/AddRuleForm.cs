using SmartIme.Models;
using SmartIme.Utilities;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SmartIme
{
    public partial class AddRuleForm : Form
    {
        public Rule CreatedRule { get; private set; }
        public string AppName { get; set; }


        public AddRuleForm(IEnumerable<object> imeList, int defaultImeIndex, string appName = null)
        {
            InitializeComponent();
            InitControls();

            // 初始化输入法列表
            cmbIme.Items.AddRange(imeList.ToArray());
            if (cmbIme.Items.Count > 0)
            {
                cmbIme.SelectedIndex = defaultImeIndex;
            }


            // 设置默认选择程序名称规则
            radioProgram.Checked = true;

            // 如果提供了应用名称，设置默认值
            if (!string.IsNullOrEmpty(appName))
            {
                AppName = appName;
                txtContent.Text = appName;
                txtName.Text = Rule.CreateDefaultName(appName, RuleType.程序名称);
            }
        }

        public AddRuleForm(IEnumerable<object> imeList, Rule editRule)
        {

            InitializeComponent();

            // 根据规则类型设置对应的单选按钮和按钮文本
            switch (editRule.RuleType)
            {
                case RuleType.程序名称:
                    radioProgram.Checked = true;
                    btnSelectProcess.Visible = false;
                    break;
                case RuleType.窗口标题:
                    radioTitle.Checked = true;
                    btnSelectProcess.Text = "选择窗口标题";
                    btnSelectProcess.Visible = true;
                    break;
                case RuleType.控件:
                    radioControl.Checked = true;
                    btnSelectProcess.Text = "选择窗口控件";
                    btnSelectProcess.Visible = true;
                    break;
            }
            InitControls();

            // 初始化输入法列表
            cmbIme.Items.AddRange(imeList.ToArray());
            if (cmbIme.Items.Count > 0)
            {
                int index = imeList.ToList().IndexOf(editRule.InputMethod);
                cmbIme.SelectedIndex = index >= 0 ? index : 0;
            }

            // 设置规则名称
            txtName.Text = editRule.RuleName;

            // 设置匹配内容
            txtContent.Text = editRule.MatchContent;

            // 设置匹配模式
            int matchPatternIndex = cmbMatchPattern.Items.IndexOf(editRule.MatchPattern);
            if (matchPatternIndex >= 0)
            {
                cmbMatchPattern.SelectedIndex = matchPatternIndex;
            }


            // 设置App名称用于编辑
            AppName = editRule.AppName;

        }

        private void InitControls()
        {
            // 初始化匹配模式下拉框
            cmbMatchPattern.Items.AddRange(Enum.GetValues(typeof(RuleMatchPattern)).Cast<object>().ToArray());
            if (cmbMatchPattern.Items.Count > 0)
            {
                cmbMatchPattern.SelectedIndex = 0; // 默认选择"等于"模式
            }

            // 添加RadioButton事件处理
            radioProgram.CheckedChanged += RadioButton_CheckedChanged;
            radioTitle.CheckedChanged += RadioButton_CheckedChanged;
            radioControl.CheckedChanged += RadioButton_CheckedChanged;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is not RadioButton radio || !radio.Checked)
            {
                return;
            }

            if (radioTitle.Checked)
            {
                txtName.Text = Rule.CreateDefaultName(AppName, RuleType.窗口标题);
                btnSelectProcess.Text = "选择窗口标题";
                btnSelectProcess.Visible = true;

                // 当切换到窗口标题规则时，自动显示选择窗口标题对话框

                List<string> titles = GetCurrentAppTitles();
                if (titles.Count > 0)
                {
                    txtContent.Text = titles[0];
                }
                else
                {
                    txtContent.Text = null;
                }

            }
            else if (radioControl.Checked)
            {
                txtName.Text = Rule.CreateDefaultName(AppName, RuleType.控件);
                btnSelectProcess.Text = "选择窗口控件";
                btnSelectProcess.Visible = true;
                txtContent.Text = "";
                SelectControlClass();
            }
            else
            {
                txtName.Text = Rule.CreateDefaultName(AppName, RuleType.窗口标题);
                btnSelectProcess.Text = "选择应用程序";
                txtContent.Text = AppName;
                btnSelectProcess.Visible = false;
            }
        }

        private void BtnSelectProcess_Click(object sender, EventArgs e)
        {
            if (radioTitle.Checked) // 窗口标题规则
            {
                ShowWindowTitlesSelection();
            }
            else if (radioControl.Checked) // 控件规则
            {
                SelectControlClass();
                //this.Activate(); // 恢复主窗口焦点
                this.ParentForm?.Activate();
            }
            else // 程序名称规则
            {
                ShowProcessSelection();
            }
        }

        private static string selectedControlClass = string.Empty;
        private static bool mouseClicked = false;

        private Form selectForm; // 添加字段引用

        string selectWindowProcessName;

        private void SelectControlClass()
        {
            selectForm = new Form()
            {
                Text = "选择控件 - 点击左键选择目标控件",
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                Width = 400,
                Height = 200,
                ShowInTaskbar = false,
                TopMost = true,

            };

            var lblInstruction = new Label()
            {
                Text = "请将鼠标移动到目标控件上，然后点击左键选择",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            selectForm.Controls.Add(lblInstruction);

            // 激活要选择控件的窗口（根据AppName激活对应应用的窗口）
            if (!string.IsNullOrEmpty(AppName))
            {
                var targetProcess = Process.GetProcessesByName(AppName)
                    .FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero);
                if (targetProcess != null)
                {

                    if (WinApi.IsIconic(targetProcess.MainWindowHandle))
                    {
                        WinApi.ShowWindow(targetProcess.MainWindowHandle, WinApi.SW_RESTORE);
                    }

                    WinApi.SetForegroundWindow(targetProcess.MainWindowHandle);
                }
                else
                {
                    MessageBox.Show($"未找到应用程序【{AppName}】的窗口，请先启动该应用程序。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            // 设置鼠标钩子
            using (var hook = new MouseHook())
            {
                hook.MouseClick += OnMouseClick;
                mouseClicked = false;
                selectedControlClass = null;
                var r = selectForm.ShowDialog();
                hook.MouseClick -= OnMouseClick;
                WinApi.SetForegroundWindow(this.Handle); // 恢复主窗口的焦点
                if (r == DialogResult.Cancel)
                {

                    return;
                }
                if (selectWindowProcessName != AppName)
                {
                    MessageBox.Show($"选择控件的窗口不是目标窗口，请重新选择;【{selectWindowProcessName}】！=【{AppName}】");
                    return;
                }
                txtContent.Text = selectedControlClass;
            }
            return;
        }
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            // 获取鼠标位置下的窗口句柄
            WinApi.GetCursorPos(out Point cursorPos);
            IntPtr hwndUnderCursor = WinApi.WindowFromPoint(cursorPos);



            // 检查鼠标下的窗口是否是本应用窗口
            uint processId;
            WinApi.GetWindowThreadProcessId(hwndUnderCursor, out processId);
            var processUnderCursor = Process.GetProcessById((int)processId);

            // 如果是本应用窗口，直接返回不处理
            if (processUnderCursor.ProcessName == Process.GetCurrentProcess().ProcessName)
            {
                return;
            }


            if (e.Button == MouseButtons.Left && !mouseClicked)
            {
                // 使用ControlHelper获取焦点控件类名
                Thread.Sleep(500); // 等待100毫秒，确保焦点已经切换
                selectWindowProcessName = AppHelper.GetForegroundProcessName();
                selectedControlClass = AppHelper.GetFocusedControlName();
                mouseClicked = true;
                selectForm.Close();
                selectForm.DialogResult = DialogResult.OK;
                //if (selectWindowProcessName != AppName)
                //{
                //    MessageBox.Show($"选择控件的窗口不是目标窗口，请重新选择;【{selectWindowProcessName}】！=【{AppName}】");
                //}
                //else
                //{

                //    txtPattern.Text = selectedControlClass;
                //}

            }
        }

        private class MouseHook : IDisposable
        {
            private readonly IntPtr hookId = IntPtr.Zero;
            private readonly WinApi.HookProc proc;
            private IntPtr lastHwndWithBorder = IntPtr.Zero;

            public event MouseEventHandler MouseClick;

            public MouseHook()
            {
                proc = HookCallback;
                hookId = SetHook(proc);
            }

            private static IntPtr SetHook(WinApi.HookProc proc)
            {
                using var curModule = Process.GetCurrentProcess().MainModule;
                return WinApi.SetWindowsHookEx(WinApi.WH_MOUSE_LL, proc,
                    WinApi.GetModuleHandle(curModule.ModuleName), 0);
            }

            private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                if (nCode >= 0)
                {
                    if (wParam == (IntPtr)WinApi.WM_LBUTTONDOWN)
                    {
                        var hookStruct = (WinApi.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.MSLLHOOKSTRUCT));
                        MouseClick?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 1,
                            hookStruct.pt.x, hookStruct.pt.y, 0));
                    }
                    else if (wParam == (IntPtr)WinApi.WM_MOUSEMOVE)
                    {
                        // 获取鼠标位置
                        var hookStruct = (WinApi.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.MSLLHOOKSTRUCT));
                        Point mousePos = new Point(hookStruct.pt.x, hookStruct.pt.y);

                        // 获取鼠标下的窗口句柄
                        IntPtr hwndUnderCursor = WinApi.WindowFromPoint(mousePos);

                        // 检查是否是本应用窗口
                        if (hwndUnderCursor != IntPtr.Zero)
                        {
                            uint processId;
                            WinApi.GetWindowThreadProcessId(hwndUnderCursor, out processId);
                            var processUnderCursor = Process.GetProcessById((int)processId);

                            // 如果是本应用窗口，清除边框并返回
                            if (processUnderCursor.ProcessName == Process.GetCurrentProcess().ProcessName)
                            {
                                DrawControlBorder(IntPtr.Zero);
                                return WinApi.CallNextHookEx(hookId, nCode, wParam, lParam);
                            }
                        }

                        // 绘制或清除边框
                        DrawControlBorder(hwndUnderCursor);
                    }
                }
                return WinApi.CallNextHookEx(hookId, nCode, wParam, lParam);
            }

            private void DrawControlBorder(IntPtr hwnd)
            {
                // 如果当前有边框的控件不是当前控件，先清除旧边框
                if (lastHwndWithBorder != IntPtr.Zero && lastHwndWithBorder != hwnd)
                {
                    ClearControlBorder(lastHwndWithBorder);
                }

                // 如果传入空句柄，只清除旧边框
                if (hwnd == IntPtr.Zero)
                {
                    if (lastHwndWithBorder != IntPtr.Zero)
                    {
                        ClearControlBorder(lastHwndWithBorder);
                        lastHwndWithBorder = IntPtr.Zero;
                    }
                    return;
                }



                // 获取控件矩形
                WinApi.RECT rect;
                WinApi.GetWindowRect(hwnd, out rect);

                // 创建绘图设备上下文
                IntPtr hdc = WinApi.GetWindowDC(hwnd);

                // 创建红色画笔
                IntPtr redPen = WinApi.CreatePen(WinApi.PS_SOLID, 6, 0x0000FF);
                IntPtr oldPen = WinApi.SelectObject(hdc, redPen);

                // 绘制矩形边框
                WinApi.SelectObject(hdc, WinApi.GetStockObject(WinApi.NULL_BRUSH));
                WinApi.Rectangle(hdc, 0, 0, rect.right - rect.left, rect.bottom - rect.top);

                // 恢复原有对象并释放资源
                WinApi.SelectObject(hdc, oldPen);
                WinApi.DeleteObject(redPen);
                WinApi.ReleaseDC(hwnd, hdc);

                // 更新最后有边框的控件
                lastHwndWithBorder = hwnd;
            }

            private void ClearControlBorder(IntPtr hwnd)
            {
                if (hwnd == IntPtr.Zero) return;

                // 使控件区域无效，触发系统重绘
                WinApi.InvalidateRect(hwnd, IntPtr.Zero, true);

                // 更新窗口
                WinApi.UpdateWindow(hwnd);
            }

            private bool IsInputControl(IntPtr hwnd)
            {
                if (hwnd == IntPtr.Zero) return false;

                // 获取控件类名
                StringBuilder className = new StringBuilder(256);
                WinApi.GetClassName(hwnd, className, className.Capacity);

                string classNameStr = className.ToString();

                // 常见输入控件类名
                string[] inputControlClasses = new string[]
                {
                    "Edit",          // 标准文本框
                    "RichEdit",      // 富文本框
                    "ComboBox",      // 组合框
                    "ListBox",       // 列表框
                    "RichEdit20W",   // Word富文本框
                    "RICHEDIT50W",   // 新版富文本框
                    "TEdit",         // Delphi文本框
                    "TMemo",         // Delphi多行文本框
                    "TComboBox",     // Delphi组合框
                    "TListBox",      // Delphi列表框
                    "TextBox",       // WPF文本框
                    "PasswordBox",   // WPF密码框
                    "RichTextBox",   // WPF富文本框
                    "ComboBox",      // WPF组合框
                    "ListBox",       // WPF列表框
                    "Chrome_RenderWidgetHostHWND", // Chrome浏览器输入框
                    "MozillaWindowClass",          // Firefox浏览器输入框
                    "Internet Explorer_Server",     // IE浏览器输入框
                    "EdgeTabWindowClass",          // Edge浏览器输入框
                    "Chrome_WidgetWin_1",          // Chrome应用输入框
                    "ElectronNSWindow",            // Electron应用窗口
                    "Windows.UI.Input.InputSite",  // UWP应用输入控件
                    "XamlWebView2",                // WinUI WebView2
                    "WebView2Control",             // WebView2控件
                    "WPFWebView2Control"           // WPF WebView2控件
                };

                // 检查是否为输入控件
                return inputControlClasses.Contains(classNameStr);
            }

            public void Dispose()
            {
                WinApi.UnhookWindowsHookEx(hookId);
            }
        }

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
                    txtContent.Text = listBox.SelectedItem.ToString();
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
                    txtContent.Text = listBox.SelectedItem.ToString();
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

        private void SetRuleName(RuleType ruleNam)
        {
            txtName.Text = $"{AppName} 【{ruleNam}】";
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtContent.Text))
            {
                MessageBox.Show("请填写规则名称和匹配模式", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RuleType type;
            if (radioTitle.Checked)
            {
                type = RuleType.窗口标题;
            }
            else if (radioControl.Checked)
            {
                type = RuleType.控件;
            }
            else
            {
                type = RuleType.程序名称;
            }

            // 获取选中的匹配模式
            RuleMatchPattern matchPattern = (RuleMatchPattern)cmbMatchPattern.SelectedItem;

            CreatedRule = new Rule(txtName.Text, type, matchPattern, txtContent.Text, cmbIme.Text);
            CreatedRule.AppName = AppName;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}