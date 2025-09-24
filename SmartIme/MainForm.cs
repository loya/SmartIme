using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Timers;
using System.Text.Json;
using System.Reflection;
using SmartIme.Utilities;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SmartIme
{
    public partial class MainForm : Form
    {
        private readonly BindingList<AppRuleGroup> appRuleGroups = [];
        private readonly System.Timers.Timer monitorTimer = new();
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        private readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // 光标颜色配置
        private Dictionary<string, Color> imeColors = new();
        private string currentImeName = "";
        private const int SPI_SETCURSOR = 0x0057;
        
        // 跟踪当前打开的浮动提示窗口
        private FloatingHintForm currentHintForm = null;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        public MainForm()
        {
            InitializeComponent();
            InitializeImeList();
            CheckForIllegalCrossThreadCalls = false;

            SetupMonitor();
            SetupTrayIcon();

            // TreeView不需要设置ItemHeight

            // 绑定事件处理程序
            btnSwitchIme.Click += BtnSwitchIme_Click;
            btnAddApp.Click += BtnAddApp_Click;
            btnRemoveApp.Click += BtnRemoveApp_Click;
            btnColorConfig.Click += BtnColorConfig_Click;
            btnPickColor.Click += BtnPickColor_Click;
            cmbImeForColor.SelectedIndexChanged += CmbImeForColor_SelectedIndexChanged;
            this.FormClosing += MainForm_FormClosing;
            this.SizeChanged += (s, e) =>
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                }
            };

            // 加载保存的设置
            cmbDefaultIme.SelectedIndex = Properties.Settings.Default.DefaultIme;
            LoadRulesFromJson();

            // 恢复窗口大小和位置
            if (Properties.Settings.Default.WindowSize != System.Drawing.Size.Empty)
            {
                this.Size = Properties.Settings.Default.WindowSize;
            }
            if (Properties.Settings.Default.WindowLocation != System.Drawing.Point.Empty)
            {
                this.Location = Properties.Settings.Default.WindowLocation;
            }
            if (Properties.Settings.Default.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = Properties.Settings.Default.WindowState;
            }

            // 初始化光标颜色配置
            InitializeCursorColorConfig();
            UpdateTreeView();
        }

        private void SetupTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("显示主窗口", null, (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            });
            trayMenu.Items.Add("退出", null, (s, e) => Application.Exit());

            trayIcon = new NotifyIcon
            {
                Text = "输入法智能切换助手",
                Icon = this.Icon,
                ContextMenuStrip = trayMenu,
                Visible = true
            };
            trayIcon.DoubleClick += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 保存设置
            Properties.Settings.Default.DefaultIme = cmbDefaultIme.SelectedIndex;
            //SaveRulesToJson();

            // 保存窗口状态
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowSize = this.Size;
                Properties.Settings.Default.WindowLocation = this.Location;
            }
            else
            {
                Properties.Settings.Default.WindowSize = this.RestoreBounds.Size;
                Properties.Settings.Default.WindowLocation = this.RestoreBounds.Location;
            }
            Properties.Settings.Default.WindowState = this.WindowState;

            // 保存光标颜色配置
            SaveCursorColorConfig();

            Properties.Settings.Default.Save();

            monitorTimer.Stop();
        }

        public void SaveRulesToJson()
        {
            try
            {
                string jsonPath = GetRulesJsonPath();
                string json = JsonSerializer.Serialize(appRuleGroups, options);
                File.WriteAllText(jsonPath, json);
                UpdateTreeView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存规则失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRulesFromJson()
        {
            try
            {
                string jsonPath = GetRulesJsonPath();
                if (File.Exists(jsonPath))
                {
                    string json = File.ReadAllText(jsonPath);
                    var loadedGroups = JsonSerializer.Deserialize<List<AppRuleGroup>>(json);
                    if (loadedGroups != null)
                    {
                        appRuleGroups.Clear();
                        foreach (var group in loadedGroups)
                        {
                            appRuleGroups.Add(group);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载规则失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetRulesJsonPath()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string appDirectory = Path.GetDirectoryName(assemblyPath);
            return Path.Combine(appDirectory, "rules.json");
        }

        private void InitializeImeList()
        {
            cmbDefaultIme.Items.Clear();
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                cmbDefaultIme.Items.Add(lang.LayoutName);
            }
            if (cmbDefaultIme.Items.Count > 0)
            {
                cmbDefaultIme.SelectedIndex = 0;
            }
        }

        private IntPtr lastActiveWindow = IntPtr.Zero;
        private string lastActiveApp = string.Empty;
        private string lastClassName = string.Empty;

        private void SetupMonitor()
        {
            monitorTimer.Interval = 300;
            monitorTimer.Elapsed += CheckActiveWindowChanged;
            monitorTimer.Enabled = true;
            monitorTimer.Start();
        }

        private void CheckActiveWindowChanged(object sender, ElapsedEventArgs e)
        {
            monitorTimer.Stop();
            IntPtr currentWindow = WinApi.GetForegroundWindow();

            if (currentWindow != lastActiveWindow)
            {
                lastActiveWindow = currentWindow;
            }
            MonitorActiveApp();
            monitorTimer.Start();
        }

        private void MonitorActiveApp()
        {
            IntPtr hWnd = WinApi.GetForegroundWindow();
            _ = WinApi.GetWindowThreadProcessId(hWnd, out uint processId);
            try
            {
                var process = System.Diagnostics.Process.GetProcessById((int)processId);
                string processName = process.ProcessName;
                string controlName = null;
                
                if (processName == lastActiveApp)
                {
                    controlName = ControlHelper.GetFocusedControlName();
                    
                    if (controlName == lastClassName)
                    {
                        return;
                    }
                    else
                    {
                        lastClassName = controlName;
                    }
                    
                    bool hasRulesForThisApp = appRuleGroups.Any(g => processName == g.AppName);
                    if (!hasRulesForThisApp)
                    {
                        return;
                    }
                }

                lastActiveApp = processName;
                lastClassName = controlName;

                var titleBuilder = new System.Text.StringBuilder(256);
                _ = WinApi.GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);
                string windowTitle = titleBuilder.ToString();

                if (string.IsNullOrEmpty(controlName))
                {
                    controlName = ControlHelper.GetWindowClassName(hWnd);
                }

                Rule matchedRule = FindMatchingRule(processName, windowTitle, controlName);

                if (matchedRule != null)
                {
                    string targetIme = matchedRule.InputMethod;
                    lblCurrentIme.Text = "当前输入法：" + targetIme + " (规则：" + matchedRule.Name + ")";

                    var inputLanguages = InputLanguage.InstalledInputLanguages;
                    foreach (InputLanguage lang in inputLanguages)
                    {
                        if (lang.LayoutName == targetIme)
                        {
                            InputLanguage.CurrentInputLanguage = lang;
                            IntPtr imeWnd = WinApi.ImmGetDefaultIMEWnd(hWnd);
                            WinApi.SendMessage(imeWnd, WinApi.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, lang.Handle);
                            lblLog.Text = DateTime.Now.ToLongTimeString() + " --[焦点控件] " + controlName ?? processName ?? windowTitle;

                            ChangeCursorColorByIme(targetIme);
                            break;
                        }
                    }
                }
                else
                {
                    lblCurrentIme.Text = "当前输入法：" + cmbDefaultIme.Text + " (默认)";
                    var inputLanguages = InputLanguage.InstalledInputLanguages;
                    if (cmbDefaultIme.SelectedIndex >= 0 && cmbDefaultIme.SelectedIndex < inputLanguages.Count)
                    {
                        var lang = inputLanguages[cmbDefaultIme.SelectedIndex];
                        InputLanguage.CurrentInputLanguage = lang;
                        IntPtr imeWnd = WinApi.ImmGetDefaultIMEWnd(hWnd);
                        WinApi.SendMessage(imeWnd, WinApi.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, lang.Handle);
                        lblLog.Text = DateTime.Now.ToString() + " --[焦点控件] " + controlName ?? processName ?? windowTitle;
                        
                        ChangeCursorColorByIme(lang.LayoutName);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private Rule FindMatchingRule(string appName, string windowTitle, string controlClass)
        {
            foreach (var group in appRuleGroups)
            {
                if (Regex.IsMatch(appName, group.AppName))
                {
                    var rule = group.FindMatchingRule(appName, windowTitle, controlClass);
                    if (rule != null)
                    {
                        return rule;
                    }
                }
            }

            return null;
        }

        private void BtnSwitchIme_Click(object sender, EventArgs e)
        {
            IntPtr hWnd = WinApi.GetForegroundWindow();
            IntPtr imeWnd = WinApi.ImmGetDefaultIMEWnd(hWnd);

            WinApi.SendMessage(imeWnd, WinApi.WM_INPUTLANGCHANGEREQUEST, (IntPtr)0x0029, IntPtr.Zero);

            _ = WinApi.GetWindowThreadProcessId(hWnd, out uint threadId);
            IntPtr hkl = WinApi.GetKeyboardLayout(threadId);
            lblCurrentIme.Text = "当前输入法：" + GetImeName(hkl);
        }

        private string GetImeName(IntPtr hkl)
        {
            return cmbDefaultIme.Text;
        }

        private void BtnAddApp_Click(object sender, EventArgs e)
        {
            var processSelectForm = new Form
            {
                ShowInTaskbar = false,
                Text = "选择应用程序",
                Size = new System.Drawing.Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lstProcesses = new ListBox
            {
                Dock = DockStyle.Fill,
                FormattingEnabled = true
            };

            var btnSelect = new Button
            {
                Text = "选择",
                DialogResult = DialogResult.OK,
                Dock = DockStyle.Bottom
            };

            processSelectForm.Controls.Add(lstProcesses);
            processSelectForm.Controls.Add(btnSelect);

            var processes = System.Diagnostics.Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .OrderBy(p => p.ProcessName)
                .ToList();

            foreach (var process in processes)
            {
                try
                {
                    lstProcesses.Items.Add($"{process.ProcessName} - {process.MainModule?.ModuleName}");
                }
                catch
                {
                    try
                    {
                        lstProcesses.Items.Add($"{process.ProcessName} - {process.MainWindowTitle}");
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            if (processSelectForm.ShowDialog(this) == DialogResult.OK && lstProcesses.SelectedIndex >= 0)
            {
                var selectedProcess = processes[lstProcesses.SelectedIndex];
                string appName = selectedProcess.ProcessName;

                System.Diagnostics.ProcessModule mainModule = null;
                try
                {
                    mainModule = selectedProcess.MainModule;
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                
                string displayName = $"{appName} - {mainModule?.ModuleName}";

                var existingGroup = appRuleGroups.FirstOrDefault(g => g.AppName == appName);
                if (existingGroup != null)
                {
                    MessageBox.Show("该应用已存在规则组中，请双击编辑。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var newGroup = new AppRuleGroup(appName, displayName,mainModule?.FileName);

                var defaultRule = new Rule(Rule.CreateDefaultName(appName, RuleNams.程序名称), RuleType.Program, appName, cmbDefaultIme.Text);
                newGroup.AddRule(defaultRule);

                var list = appRuleGroups.ToList();
                list.Add(newGroup);

                var index = list.OrderBy(t => t.AppName).ToList().FindIndex(t => t.AppName == appName);
                appRuleGroups.Insert(index, newGroup);
                UpdateTreeView();
                SaveRulesToJson();
                
                using var editAppRulesForm = new EditAppRulesForm(this,newGroup, cmbDefaultIme.Items.Cast<string>());
                editAppRulesForm.ShowDialog(this);
            }
        }

        private void BtnRemoveApp_Click(object sender, EventArgs e)
        {
            if (treeApps.SelectedNode != null)
            {
                if (treeApps.SelectedNode.Tag is AppRuleGroup group)
                {
                    appRuleGroups.Remove(group);
                    SaveRulesToJson();
                }
                else if (treeApps.SelectedNode.Tag is Rule rule && treeApps.SelectedNode.Parent != null)
                {
                    if (treeApps.SelectedNode.Parent.Tag is AppRuleGroup parentGroup)
                    {
                        parentGroup.RemoveRule(rule);
                        SaveRulesToJson();
                    }
                }
            }
        }

        private void TreeApps_DoubleClick(object sender, EventArgs e)
        {
            if (treeApps.SelectedNode != null)
            {
                if (treeApps.SelectedNode.Tag is AppRuleGroup group)
                {
                    using var editAppRulesForm = new EditAppRulesForm(this,group, cmbDefaultIme.Items.Cast<string>());
                    editAppRulesForm.ShowDialog(this);
                }
                else if (treeApps.SelectedNode.Tag is Rule rule && treeApps.SelectedNode.Parent != null)
                {
                    if (treeApps.SelectedNode.Parent.Tag is AppRuleGroup parentGroup)
                    {
                        using var editAppRulesForm = new EditAppRulesForm(this,parentGroup, cmbDefaultIme.Items.Cast<string>());
                        editAppRulesForm.ShowDialog(this);
                    }
                }
            }
        }

        private void UpdateTreeView()
        {
            treeApps.Nodes.Clear();

            foreach (var group in appRuleGroups.OrderBy(g => g.AppName))
            {
                var groupNode = new TreeNode(group.DisplayName)
                {
                    Tag = group,
                    NodeFont = new Font(treeApps.Font, FontStyle.Bold),
                    ForeColor = Color.DarkOrange
                };

                foreach (var rule in group.Rules)
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
                        ForeColor = ruleColor
                    };
                    groupNode.Nodes.Add(ruleNode);
                }

                treeApps.Nodes.Add(groupNode);
                groupNode.Expand();
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 光标颜色配置功能

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(int uiAction, int uiParam, IntPtr pvParam, int fWinIni);

        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private static extern bool SetCaretBlinkTime(int wMSeconds);

        [DllImport("user32.dll")]
        private static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll")]
        private static extern bool ShowCaret(IntPtr hWnd);
        
        [DllImport("user32.dll")]
        private static extern bool DestroyCaret();

        private void InitializeCursorColorConfig()
        {
            cmbImeForColor.Items.Clear();
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                cmbImeForColor.Items.Add(lang.LayoutName);
            }
            if (cmbImeForColor.Items.Count > 0)
            {
                cmbImeForColor.SelectedIndex = 0;
            }

            LoadCursorColorConfig();
        }

        private void LoadCursorColorConfig()
        {
            try
            {
                imeColors.Clear();
                var savedColors = Properties.Settings.Default.ImeColors;
                if (!string.IsNullOrEmpty(savedColors))
                {
                    var colorDict = JsonSerializer.Deserialize<Dictionary<string, string>>(savedColors);
                    if (colorDict != null)
                    {
                        foreach (var kvp in colorDict)
                        {
                            try
                            {
                                var color = ColorTranslator.FromHtml(kvp.Value);
                                imeColors[kvp.Key] = color;
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                if (imeColors.Count == 0)
                {
                    imeColors["中文(简体) - 微软拼音"] = Color.Red;
                    imeColors["英语(美国)"] = Color.Blue;
                    imeColors["中文(简体) - 美式键盘"] = Color.Green;
                }

                UpdateCursorColorDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载光标颜色配置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCursorColorConfig()
        {
            try
            {
                var colorDict = new Dictionary<string, string>();
                foreach (var kvp in imeColors)
                {
                    colorDict[kvp.Key] = ColorTranslator.ToHtml(kvp.Value);
                }
                Properties.Settings.Default.ImeColors = JsonSerializer.Serialize(colorDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存光标颜色配置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCursorColorDisplay()
        {
            if (cmbImeForColor.SelectedItem != null)
            {
                string imeName = cmbImeForColor.SelectedItem.ToString();
                if (imeColors.TryGetValue(imeName, out Color color))
                {
                    pnlCursorColor.BackColor = color;
                    lblCursorColor.Text = color.Name;
                }
                else
                {
                    pnlCursorColor.BackColor = Color.Black;
                    lblCursorColor.Text = "黑色";
                }
            }
        }

        private void ChangeCursorColor(Color color, string imeName = null)
        {
            try
            {
                UpdateTrayIconColor(color);
                ShowFloatingHint(color, imeName ?? currentImeName);
                TryUpdateCaretWidth(color);
            }
            catch (Exception ex)
            {
                lblLog.Text = $"视觉提示设置失败: {ex.Message}";
            }
        }

        private void ShowFloatingHint(Color color, string imeName)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowFloatingHint(color, imeName)));
                return;
            }
            
            if (currentHintForm != null && !currentHintForm.IsDisposed)
            {
                currentHintForm.Close();
                currentHintForm.Dispose();
                currentHintForm = null;
            }
            
            Point displayPos = GetBestFloatingHintPosition();
            
            // 验证和调整坐标
            displayPos = ValidateAndAdjustPosition(displayPos);
            
            FloatingHintForm hintForm = new FloatingHintForm(color, imeName);
            hintForm.StartPosition = FormStartPosition.Manual;
            hintForm.Location = displayPos;

            hintForm.Show();
            currentHintForm = hintForm;
        }

        private Point GetBestFloatingHintPosition()
        {
            Point? caretPosition = GetCaretPositionFromActiveWindow();
            if (caretPosition.HasValue)
            {
                if(caretPosition.Value != Point.Empty)
                    return new Point(caretPosition.Value.X + 5, caretPosition.Value.Y - 40);
            }
            
            Point cursorPos = Cursor.Position;
            return new Point(cursorPos.X + 15, cursorPos.Y - 40);
        }

        private Point ValidateAndAdjustPosition(Point position)
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

        private void LogToFile(string message)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "position_debug.log");
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // 忽略日志写入错误
            }
        }

        private Point? TryGetCaretPositionWithRetry(IntPtr hWnd)
        {
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

        private Point? GetCaretPositionUsingGUIThreadInfo(IntPtr hWnd)
        {
            try
            {
                uint threadId = WinApi.GetWindowThreadProcessId(hWnd, IntPtr.Zero);
                if (threadId == 0)
                {
                    lblLog.Text = $"GUIThreadInfo: 无法获取线程ID (窗口: {hWnd})";
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
                        lblLog.Text = debugInfo;

                        if (threadInfo.rcCaret.left >= 0 && threadInfo.rcCaret.top >= 0)
                        {
                            // 插入符位置是相对于插入符窗口的客户端坐标
                            Point caretPos = new Point(threadInfo.rcCaret.left, threadInfo.rcCaret.top);
                            Point? screenPos = ConvertClientToScreen(threadInfo.hwndCaret, caretPos);
                            
                            if (screenPos.HasValue)
                            {
                                lblLog.Text += $", 屏幕位置=({screenPos.Value.X},{screenPos.Value.Y})";
                            }
                            
                            return screenPos;
                        }
                    }
                    else
                    {
                        lblLog.Text = debugInfo + ", 无插入符窗口";
                    }
                }
                else
                {
                    lblLog.Text = $"GUIThreadInfo: API调用失败 (线程: {threadId})";
                }
            }
            catch (Exception ex)
            {
                lblLog.Text = $"GUIThreadInfo异常: {ex.Message}";
            }
            
            return null;
        }

        private Point? ConvertClientToScreen(IntPtr hWnd, Point clientPoint)
        {
            try
            {
                // 方法1: 使用标准的ClientToScreen API
                Point screenPoint = clientPoint;
                if (WinApi.ClientToScreen(hWnd, ref screenPoint))
                {
                    lblLog.Text += $" 方法1成功: ({screenPoint.X},{screenPoint.Y})";
                    return screenPoint;
                }

                // 方法2: 使用POINT结构体的ClientToScreen
                WinApi.POINT point = new WinApi.POINT { x = clientPoint.X, y = clientPoint.Y };
                if (WinApi.ClientToScreen(hWnd, ref point))
                {
                    lblLog.Text += $" 方法2成功: ({point.x},{point.y})";
                    return new Point(point.x, point.y);
                }

                // 方法3: 手动计算窗口位置 + 客户端坐标
                if (WinApi.GetWindowRect(hWnd, out WinApi.RECT windowRect))
                {
                    Point manualPoint = new Point(windowRect.left + clientPoint.X, windowRect.top + clientPoint.Y);
                    lblLog.Text += $" 方法3: 手动计算({manualPoint.X},{manualPoint.Y})";
                    
                    // 验证坐标是否在屏幕范围内
                    if (IsPointOnScreen(manualPoint))
                    {
                        return manualPoint;
                    }
                    else
                    {
                        lblLog.Text += " 坐标超出屏幕范围";
                    }
                }

                // 方法4: 如果插入符窗口不是目标窗口，尝试获取焦点窗口
                IntPtr focusWnd = WinApi.GetFocus();
                if (focusWnd != hWnd && focusWnd != IntPtr.Zero)
                {
                    lblLog.Text += $" 尝试焦点窗口: {focusWnd}";
                    return ConvertClientToScreen(focusWnd, clientPoint);
                }

                // 方法5: 使用鼠标位置作为备选方案
                if (WinApi.GetCursorPos(out Point cursorPos))
                {
                    lblLog.Text += $" 使用鼠标位置: ({cursorPos.X},{cursorPos.Y})";
                    return cursorPos;
                }
            }
            catch (Exception ex)
            {
                lblLog.Text += $" 坐标转换异常: {ex.Message}";
            }
            
            return null;
        }

        private bool IsPointOnScreen(Point point)
        {
            // 检查坐标是否在屏幕范围内
            return point.X >= 0 && point.Y >= 0 && 
                   point.X <= Screen.PrimaryScreen.Bounds.Width && 
                   point.Y <= Screen.PrimaryScreen.Bounds.Height;
        }
        
        private string GetWindowTitle(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return "无效窗口";
            
            StringBuilder sb = new StringBuilder(256);
            WinApi.GetWindowText(hWnd, sb, sb.Capacity);
            string title = sb.ToString();
            return string.IsNullOrEmpty(title) ? "无标题窗口" : title;
        }

        private Point? GetCaretPositionFromActiveWindow()
        {
            IntPtr hWnd = WinApi.GetFocus();
            if (hWnd == IntPtr.Zero)
            {
                hWnd = WinApi.GetForegroundWindow();
                if (hWnd == IntPtr.Zero)
                {
                    return null;
                }
            }
            
            return TryGetCaretPositionWithRetry(hWnd);
        }

        private void UpdateTrayIconColor(Color color)
        {
            using (Bitmap bmp = new Bitmap(16, 16))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                
                using (Brush brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 2, 2, 12, 12);
                }
                
                using (Pen pen = new Pen(Color.White, 1))
                {
                    g.DrawEllipse(pen, 2, 2, 12, 12);
                }
                
                trayIcon.Icon = Icon.FromHandle(bmp.GetHicon());
            }
        }

        private void TryUpdateCaretWidth(Color color)
        {
            try
            {
                IntPtr hWnd = GetFocus();
                if (hWnd != IntPtr.Zero)
                {
                    DestroyCaret();
                    int caretWidth = GetCaretWidthFromColor(color);
                    CreateCaret(hWnd, IntPtr.Zero, caretWidth, 20);
                    ShowCaret(hWnd);
                    SetCaretBlinkTime(500);
                }
            }
            catch
            {
            }
        }

        private int GetCaretWidthFromColor(Color color)
        {
            return Math.Max(1, color.GetHashCode() % 5 + 1);
        }

        private void ChangeCursorColorByIme(string imeName)
        {
            if(string.IsNullOrEmpty(currentImeName)){
                currentImeName = imeName;
                return;
            }
            if (imeName != currentImeName && !string.IsNullOrEmpty(currentImeName))
            {
                if (imeColors.TryGetValue(imeName, out Color color))
                {
                    ChangeCursorColor(color, imeName);
                    currentImeName = imeName;
                }
                else
                {
                    ChangeCursorColor(Color.Black, imeName);
                    currentImeName = "";
                }
            }
            else
            {
                currentImeName = imeName;
            }
        }

        private void BtnColorConfig_Click(object sender, EventArgs e)
        {
            MessageBox.Show("请在下方选择输入法并设置对应的光标颜色。切换输入法时，光标颜色会自动改变。", 
                "光标颜色配置", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPickColor_Click(object sender, EventArgs e)
        {
            if (cmbImeForColor.SelectedItem != null)
            {
                string imeName = cmbImeForColor.SelectedItem.ToString();
                if (imeColors.TryGetValue(imeName, out Color currentColor))
                {
                    colorDialog.Color = currentColor;
                }
                else
                {
                    colorDialog.Color = Color.Black;
                }

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    imeColors[imeName] = colorDialog.Color;
                    UpdateCursorColorDisplay();
                    SaveCursorColorConfig();

                    if (imeName == currentImeName)
                    {
                        ChangeCursorColor(colorDialog.Color);
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择一个输入法", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CmbImeForColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCursorColorDisplay();
        }

        #endregion

    }
}