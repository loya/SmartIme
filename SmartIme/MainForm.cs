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
                    // var options = new JsonSerializerOptions 
                    // { 
                    //     PropertyNameCaseInsensitive = true,
                    //     AllowTrailingCommas = true,
                    //     WriteIndented = true
                    // };
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
            // 使用Assembly.GetExecutingAssembly().Location获取程序所在目录
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string appDirectory = Path.GetDirectoryName(assemblyPath);
            return Path.Combine(appDirectory, "rules.json");
        }

        private void InitializeImeList()
        {
            cmbDefaultIme.Items.Clear();
            // 枚举系统安装的输入法
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

            // 只有当活动窗口变化时才处理
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
                // 如果是同一个应用程序，检查是否有针对该应用的规则
                if (processName == lastActiveApp)
                {
                    controlName = ControlHelper.GetFocusedControlName();
                    
                    //todo 未判断应用标题

                    if (controlName == lastClassName)
                    {
                        return;
                    }
                    else
                    {
                        lastClassName = controlName;
                    }
                    // 检查是否存在针对该应用的规则组
                    bool hasRulesForThisApp = appRuleGroups.Any(g => processName == g.AppName);

                    // 如果没有针对该应用的规则，则不重复切换输入法
                    if (!hasRulesForThisApp)
                    {
                        return;
                    }
                    // 如果有规则，继续处理以检查控件级别的规则
                }

                lastActiveApp = processName;
                lastClassName = controlName;

                // 获取窗口标题
                var titleBuilder = new System.Text.StringBuilder(256);
                _ = WinApi.GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);
                string windowTitle = titleBuilder.ToString();

                // 获取当前焦点控件名称
                //string controlName = ControlHelper.GetFocusedControlName();
                //lblLog.Text = DateTime.Now +"--"+ controlName;
                if (string.IsNullOrEmpty(controlName))
                {
                    // 如果获取焦点控件失败，使用窗口类名作为后备
                    controlName = ControlHelper.GetWindowClassName(hWnd);
                }

                // 按优先级查找匹配的规则
                Rule matchedRule = FindMatchingRule(processName, windowTitle, controlName);

                if (matchedRule != null)
                {
                    // 切换输入法
                    string targetIme = matchedRule.InputMethod;
                    lblCurrentIme.Text = "当前输入法：" + targetIme + " (规则：" + matchedRule.Name + ")";

                    // 获取所有输入法列表
                    var inputLanguages = InputLanguage.InstalledInputLanguages;
                    foreach (InputLanguage lang in inputLanguages)
                    {
                        if (lang.LayoutName == targetIme)
                        {
                            // 两种方式结合确保切换成功
                            InputLanguage.CurrentInputLanguage = lang;
                            IntPtr imeWnd = WinApi.ImmGetDefaultIMEWnd(hWnd);
                            WinApi.SendMessage(imeWnd, WinApi.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, lang.Handle);
                            lblLog.Text = DateTime.Now.ToLongTimeString() + " --[焦点控件] " + controlName ?? processName ?? windowTitle;

                            // 切换光标颜色
                            ChangeCursorColorByIme(targetIme);

                            break;
                        }
                    }
                }
                else
                {
                    // 使用默认输入法
                    lblCurrentIme.Text = "当前输入法：" + cmbDefaultIme.Text + " (默认)";
                    // 切换到默认输入法
                    var inputLanguages = InputLanguage.InstalledInputLanguages;
                    if (cmbDefaultIme.SelectedIndex >= 0 && cmbDefaultIme.SelectedIndex < inputLanguages.Count)
                    {
                        var lang = inputLanguages[cmbDefaultIme.SelectedIndex];
                        InputLanguage.CurrentInputLanguage = lang;
                        IntPtr imeWnd = WinApi.ImmGetDefaultIMEWnd(hWnd);
                        WinApi.SendMessage(imeWnd, WinApi.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, lang.Handle);
                        lblLog.Text = DateTime.Now.ToString() + " --[焦点控件] " + controlName ?? processName ?? windowTitle;
                        
                        // 切换光标颜色
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
            // 查找匹配的应用规则组
            foreach (var group in appRuleGroups)
            {
                // 检查应用名称是否匹配
                if (Regex.IsMatch(appName, group.AppName))
                {
                    // 在应用规则组中查找匹配的规则
                    //lblLog.Text = controlName;
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

            // 0x0029 是切换到下一个输入法的消息
            WinApi.SendMessage(imeWnd, WinApi.WM_INPUTLANGCHANGEREQUEST, (IntPtr)0x0029, IntPtr.Zero);

            // 更新显示
            _ = WinApi.GetWindowThreadProcessId(hWnd, out uint threadId);
            IntPtr hkl = WinApi.GetKeyboardLayout(threadId);
            lblCurrentIme.Text = "当前输入法：" + GetImeName(hkl);
        }

        private string GetImeName(IntPtr hkl)
        {
            // 这里应该根据hkl获取输入法名称
            // 简化处理，直接返回下拉框中的选项
            return cmbDefaultIme.Text;
        }

        private void BtnAddApp_Click(object sender, EventArgs e)
        {
            // 创建进程选择窗口
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

            // 获取所有进程
            var processes = System.Diagnostics.Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .OrderBy(p => p.ProcessName)
                .ToList();

            // 添加到列表
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

            // 显示窗口
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

                // 检查是否已存在该应用
                var existingGroup = appRuleGroups.FirstOrDefault(g => g.AppName == appName);
                if (existingGroup != null)
                {
                    MessageBox.Show("该应用已存在规则组中，请双击编辑。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 创建新的应用规则组
                var newGroup = new AppRuleGroup(appName, displayName,mainModule?.FileName);

                // 添加默认规则
                var defaultRule = new Rule(Rule.CreateDefaultName(appName, RuleNams.程序名称), RuleType.Program, appName, cmbDefaultIme.Text);
                newGroup.AddRule(defaultRule);

                // 添加到列表
                var list = appRuleGroups.ToList();
                list.Add(newGroup);

                var index = list.OrderBy(t => t.AppName).ToList().FindIndex(t => t.AppName == appName);
                appRuleGroups.Insert(index, newGroup);
                UpdateTreeView();
                SaveRulesToJson(); // 保存到JSON文件
                // 打开编辑窗口
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
                    SaveRulesToJson(); // 保存到JSON文件
                }
                else if (treeApps.SelectedNode.Tag is Rule rule && treeApps.SelectedNode.Parent != null)
                {
                    if (treeApps.SelectedNode.Parent.Tag is AppRuleGroup parentGroup)
                    {
                        parentGroup.RemoveRule(rule);
                        SaveRulesToJson(); // 保存到JSON文件
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
                    //SaveRulesToJson(); // 保存到JSON文件
                }
                else if (treeApps.SelectedNode.Tag is Rule rule && treeApps.SelectedNode.Parent != null)
                {
                    if (treeApps.SelectedNode.Parent.Tag is AppRuleGroup parentGroup)
                    {
                        using var editAppRulesForm = new EditAppRulesForm(this,parentGroup, cmbDefaultIme.Items.Cast<string>());
                        editAppRulesForm.ShowDialog(this);
                        
                        //SaveRulesToJson(); // 保存到JSON文件
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
                    ForeColor = Color.DarkOrange  // 应用组节点使用深蓝色

                };

                foreach (var rule in group.Rules)
                {
                    // 根据规则类型设置不同的颜色
                    Color ruleColor = rule.Type switch
                    {
                        RuleType.Program => Color.DarkSeaGreen,  // 程序规则：绿色
                        RuleType.Title => Color.DarkCyan,   // 窗口标题规则：橙色
                        RuleType.Control => Color.DeepSkyBlue, // 控件规则：紫色
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
            // 初始化输入法颜色下拉框
            cmbImeForColor.Items.Clear();
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                cmbImeForColor.Items.Add(lang.LayoutName);
            }
            if (cmbImeForColor.Items.Count > 0)
            {
                cmbImeForColor.SelectedIndex = 0;
            }

            // 加载保存的光标颜色配置
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
                                // 如果颜色解析失败，跳过
                            }
                        }
                    }
                }

                // 设置默认颜色
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
                // 更新系统托盘图标颜色
                UpdateTrayIconColor(color);
                
                // 显示浮动提示窗口，使用指定的输入法名称或当前输入法名称
                ShowFloatingHint(color, imeName ?? currentImeName);
                
                // 同时保留原有的插入符宽度提示（可选）
                TryUpdateCaretWidth(color);
            }
            catch (Exception ex)
            {
                lblLog.Text = $"视觉提示设置失败: {ex.Message}";
            }
        }

        private void ShowFloatingHint(Color color, string imeName)
        {
            // 在主UI线程中创建和显示浮动提示窗口，但使用异步方式避免阻塞
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowFloatingHint(color, imeName)));
                return;
            }
            
            // 先关闭已存在的提示窗口
            if (currentHintForm != null && !currentHintForm.IsDisposed)
            {
                currentHintForm.Close();
                currentHintForm.Dispose();
                currentHintForm = null;
            }
            
            // 获取当前光标位置
            Point cursorPos = Cursor.Position;
            
            // 创建浮动提示窗口（窗体内部已经实现了自动关闭）
            FloatingHintForm hintForm = new FloatingHintForm(color, imeName);
            hintForm.StartPosition = FormStartPosition.Manual;
            
            // 将窗口位置设置在光标右下方
            hintForm.Location = new Point(cursorPos.X + 5, cursorPos.Y - 40);

            // 异步显示窗口（1秒后会自动关闭）
            if (imeName == currentImeName)
                return;
            hintForm.Show();
            
            // 保存当前提示窗口引用
            currentHintForm = hintForm;
        }

        private void UpdateTrayIconColor(Color color)
        {
            // 创建带有颜色提示的系统托盘图标
            using (Bitmap bmp = new Bitmap(16, 16))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 绘制背景
                g.Clear(Color.Transparent);
                
                // 绘制颜色指示圆
                using (Brush brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 2, 2, 12, 12);
                }
                
                // 绘制边框
                using (Pen pen = new Pen(Color.White, 1))
                {
                    g.DrawEllipse(pen, 2, 2, 12, 12);
                }
                
                // 更新托盘图标
                trayIcon.Icon = Icon.FromHandle(bmp.GetHicon());
            }
        }

        private void TryUpdateCaretWidth(Color color)
        {
            try
            {
                // 尝试更新插入符宽度（可能在某些应用中不起作用）
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
                // 忽略插入符更新错误
            }
        }

        private int GetCaretWidthFromColor(Color color)
        {
            // 简化宽度映射
            return Math.Max(1, color.GetHashCode() % 5 + 1);
        }

        private void ChangeCursorColorByIme(string imeName)
        {
            // 只有当输入法真正切换时才显示提示窗口
            // 并且排除程序启动时的第一次切换（currentImeName为空）
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
                    // 使用默认黑色
                    ChangeCursorColor(Color.Black, imeName);
                    currentImeName = "";
                }
            }
            else
            {
                // 更新当前输入法名称但不显示提示
                currentImeName = imeName;
            }
        }

        private void BtnColorConfig_Click(object sender, EventArgs e)
        {
            // 显示颜色配置提示
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

                    // 如果当前输入法匹配，立即应用颜色变化
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