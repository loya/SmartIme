using SmartIme.Forms;
using SmartIme.Models;
using SmartIme.Utilities;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace SmartIme
{
    public partial class MainForm : Form
    {
        public BindingList<AppRuleGroup> AppRuleGroups = new();
        private readonly int _monitorInterval = 50; // 监测间隔，单位毫秒
        private CancellationTokenSource _cancellationTokenSource = new();
        private NotifyIcon _trayIcon;
        private ContextMenuStrip _trayMenu;
        private readonly List<string> _whitelistedApps = new List<string>();

        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // 全局AppSettings字段
        private AppSettings _appSettings;

        // 光标颜色配置
        // private Dictionary<string, Color> _imeColors = new();
        private string _currentImeName = "";

        // 跟踪当前打开的浮动提示窗口
        private FloatingHintForm _hintForm = null;

        //改变输入法时的进程名
        private string _changeColorProcessName = "";

        private string _lastActiveApp = string.Empty;
        private string _lastClassName = string.Empty;

        private FormWindowState _lastWindowState;
        // private bool _InputMethodChanged = false;

        #region 属性

        public string AppDirectory
        {
            get
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                string appDirectory = Path.GetDirectoryName(assemblyPath);
                return appDirectory;
            }
        }
        public string SettingsDir => Path.Combine(AppDirectory, "settings");

        #endregion

        #region 获取设置文件路径方法


        public string GetRulesJsonPath()
        {
            var path = Path.Combine(SettingsDir, "rules.json");
            return path;
        }

        public string GetWhitelistJsonPath()
        {
            var path = Path.Combine(SettingsDir, "whitelist.json");
            return path;
        }

        #endregion

        public MainForm()
        {
            InitializeComponent();
            this.Icon = Assembly.GetExecutingAssembly().GetManifestResourceStream("SmartIme.appIcon.ico") != null ?
                new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("SmartIme.appIcon.ico")) :
                SystemIcons.Application;


            // 加载保存的应用设置
            loadAppSetting();

            InitializeImeList();
            CheckForIllegalCrossThreadCalls = false;

            // 初始化光标颜色配置
            // _imeColors = new Dictionary<string, Color>();
            // LoadCursorColorConfig();

            // 绑定事件处理程序
            btnSwitchIme.Click += BtnSwitchIme_Click;
            btnAddApp.Click += BtnAddApp_Click;
            btnRemoveApp.Click += BtnRemoveApp_Click;
            this.FormClosing += MainForm_FormClosing;
            this.SizeChanged += (s, e) =>
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                }
                else
                {
                    _lastWindowState = this.WindowState;
                    this.ShowInTaskbar = true;
                }
            };
            treeApps.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter && treeApps.SelectedNode != null)
                {
                    TreeApps_DoubleClick(s, e);
                    e.Handled = true;
                }
            };

            //检查设置路径是否存在
            if (!Directory.Exists(SettingsDir))
            {
                Directory.CreateDirectory(SettingsDir);
            }
            // 加载保存的设置
            cmbDefaultIme.SelectedIndex = _appSettings.DefaultIme;
            LoadRulesFromJson();
            LoadWhitelist();
            UpdateTreeView();
            SetupTrayIcon();
            //SetupMonitor();
            _ = MonitorSystemAsync(_cancellationTokenSource.Token);

        }

        /// <summary>
        /// Loads application settings from AppSettings.json file using the new settings system.
        /// Previously, settings were stored in the legacy Properties.Settings system which has been migrated.
        /// </summary>
        /// <summary>
        /// Loads application settings from AppSettings.json file using the new settings system.
        /// Previously, settings were stored in the legacy Properties.Settings system which has been migrated.
        /// </summary>
        public void loadAppSetting()
        {
            // 加载AppSettings并保存为全局字段
            _appSettings = AppSettings.Load();

            // 恢复窗口大小和位置
            if (_appSettings.WindowSize != System.Drawing.Size.Empty)
            {
                this.Size = _appSettings.WindowSize;
            }
            if (_appSettings.WindowLocation != System.Drawing.Point.Empty)
            {
                this.Location = _appSettings.WindowLocation;
            }
            if (_appSettings.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = _appSettings.WindowState;
            }
            chkAlwayShowHint.Checked = _appSettings.AlwayShowHint;

            // 浮动提示设置现在直接从AppSettings的属性获取，无需额外加载
        }

        private void LoadFloatingHintSettings()
        {
            // 恢复悬浮提示窗设置 - 现在直接使用AppSettings的转换属性
            _appSettings = AppSettings.Load(); // 重新加载以确保所有转换属性是最新的
        }

        private void SetupTrayIcon()
        {
            _trayMenu = new ContextMenuStrip();

            // 添加开机自启动菜单项
            var startupItem = new ToolStripMenuItem("开机自启动");
            startupItem.CheckOnClick = true;
            startupItem.Checked = AppStartupHelper.IsAppSetToStartup();
            startupItem.Click += (s, e) =>
            {
                AppStartupHelper.SetAppStartup(startupItem.Checked);
                startupItem.Checked = AppStartupHelper.IsAppSetToStartup();
            };
            _trayMenu.Items.Add(startupItem);

            _trayMenu.Items.Add("-");

            _trayMenu.Items.Add("显示主窗口", null, (s, e) =>
            {
                this.Show();
                this.WindowState = _lastWindowState;
                this.Activate();
            });

            _trayMenu.Items.Add("退出", null, (s, e) => this.Close());

            _trayIcon = new NotifyIcon
            {
                Text = "输入法智能切换助手",
                Icon = (Icon)this.Icon.Clone(),
                ContextMenuStrip = _trayMenu,
                Visible = true
            };
            _trayIcon.Click += (s, e) =>
            {
                if (e is MouseEventArgs me && me.Button == MouseButtons.Left)
                {
                    this.Show();
                    this.WindowState = _lastWindowState;
                    this.Activate();
                }
            };
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 保存设置 to the new AppSettings.json file
            var settings = _appSettings;
            settings.DefaultIme = cmbDefaultIme.SelectedIndex;
            //SaveRulesToJson();

            // 保存窗口状态
            if (this.WindowState == FormWindowState.Normal)
            {
                settings.WindowSize = this.Size;
                settings.WindowLocation = this.Location;
            }
            else
            {
                settings.WindowSize = this.RestoreBounds.Size;
                settings.WindowLocation = this.RestoreBounds.Location;
            }
            settings.WindowState = this.WindowState;

            // // 保存光标颜色配置
            // SaveCursorColorConfig();

            settings.Save();

            //monitorTimer.Stop();
            // 修复 SYSLIB0006: 不再使用 thread.Abort()，改为安全地请求线程停止
            // if (thread != null && thread.IsAlive)
            // {
            //     thread.Interrupt();
            // }
        }

        public void SaveRulesToJson(bool updateTreeView = true)
        {
            try
            {
                string jsonPath = GetRulesJsonPath();
                string json = JsonSerializer.Serialize(AppRuleGroups, _options);
                File.WriteAllText(jsonPath, json);
                if (updateTreeView)
                {
                    UpdateTreeView();
                }
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
                        AppRuleGroups.Clear();
                        foreach (var group in loadedGroups)
                        {
                            AppRuleGroups.Add(group);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载规则失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private async Task MonitorSystemAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!MonitorInputMathChange())
                {
                    MonitorActiveApp();
                }
                await Task.Delay(_monitorInterval, token);
            }
        }

        private bool MonitorInputMathChange()
        {
            var imeSwitched = CaretHelper.MonitorInputMethodSwitch(out string newImeName, out _changeColorProcessName);
            if (imeSwitched)
            {
                Debug.WriteLine($"\n\r监测输入法切换: {newImeName} 进程: {_changeColorProcessName}");
                this.lblLog.Text = DateTime.Now.ToLongTimeString() + " --[输入法切换] " + newImeName + " 进程: " + _changeColorProcessName;
                _lastActiveApp = _changeColorProcessName;
                ChangeCursorColorByIme(newImeName);
            }
            else
            {

            }
            return imeSwitched;
        }

        private void MonitorActiveApp()
        {
            bool isAppChagned = false;
            IntPtr hWnd = WinApi.GetForegroundWindow();
            _ = WinApi.GetWindowThreadProcessId(hWnd, out uint processId);
            try
            {
                var process = System.Diagnostics.Process.GetProcessById((int)processId);
                string processName = process.ProcessName;
                if (processName == "explorer") { return; }
                string controlName = null;
                controlName = AppHelper.GetFocusedControlName();
                string[] strings = new string[] { "menu", "popup", "bar" };
                if (strings.Any(s => controlName.Contains(s, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return;
                }

                string windowTitle = process.MainWindowTitle;

                Rule matchedRule = FindMatchingRule(processName, windowTitle, controlName);
                if (processName == _lastActiveApp)
                {
                    controlName = AppHelper.GetFocusedControlName();

                    if (controlName == _lastClassName)
                    {

                        //if (matchedRule != null && InputLanguage.CurrentInputLanguage.LayoutName != matchedRule.InputMethod)
                        //{
                        //    ChangeCursorColorByIme(matchedRule.InputMethod);

                        //    return;
                        //}
                        return;
                    }

                    bool hasRulesForThisApp = AppRuleGroups.Any(g => processName == g.AppName);
                    if (!hasRulesForThisApp)
                    {
                        return;
                    }
                }
                else
                {
                    // 切换窗口立即关闭提示窗
                    if (processName != _lastActiveApp) { _hintForm?.Close(); _hintForm = null; }

                    isAppChagned = true;
                }

                //if (string.IsNullOrEmpty(controlName))
                //{
                //    return;
                //}
                //Debug.WriteLine($"当前窗口: {processName} {controlName}");
                //Debug.WriteLine($"前次窗口: {lastActiveApp} {lastClassName}");
                _lastActiveApp = processName;
                _lastClassName = controlName;
                Debug.WriteLine("last App:" + processName);
                Debug.WriteLine("last ClassName:" + _lastClassName);

                windowTitle = WinApi.GetWindowText(hWnd);
                lblLog.Text = DateTime.Now.ToLongTimeString() + " --[焦点控件] " + controlName ?? processName ?? windowTitle;

                if (string.IsNullOrEmpty(controlName))
                {
                    controlName = AppHelper.GetWindowClassName(hWnd);
                }

                //Rule matchedRule = FindMatchingRule(processName, windowTitle, controlName);


                if (matchedRule != null)
                {
                    string targetIme = matchedRule.InputMethod;
                    lblCurrentIme.Text = "当前输入法：" + targetIme + " (规则：" + matchedRule.Name + ")";

                    var inputLanguages = InputLanguage.InstalledInputLanguages;
                    foreach (InputLanguage lang in inputLanguages)
                    {
                        if (lang.LayoutName == targetIme)
                        {
                            Debug.WriteLine("dangqian ime:" + InputLanguage.CurrentInputLanguage.LayoutName);
                            // if (InputLanguage.CurrentInputLanguage.LayoutName != targetIme)
                            // {
                            InputLanguage.CurrentInputLanguage = lang;
                            IntPtr imeWnd = WinApi.ImmGetDefaultIMEWnd(hWnd);
                            WinApi.SendMessage(imeWnd, WinApi.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, lang.Handle);
                            // }

                            //ChangeCursorColorByIme(targetIme);
                            break;
                        }
                    }
                }

                if (isAppChagned)
                {
                    var inputMethod = CaretHelper.GetCurrentInputMethod(hWnd);
                    if (_appSettings.AlwayShowHint)
                        ChangeCursorColorByIme(inputMethod);

                }

                //else
                //{
                //    lblCurrentIme.Text = "当前输入法：" + cmbDefaultIme.Text + " (默认)";
                //    var inputLanguages = InputLanguage.InstalledInputLanguages;
                //    if (cmbDefaultIme.SelectedIndex >= 0 && cmbDefaultIme.SelectedIndex < inputLanguages.Count)
                //    {
                //        var lang = inputLanguages[cmbDefaultIme.SelectedIndex];
                //        InputLanguage.CurrentInputLanguage = lang;
                //        IntPtr imeWnd = WinApi.ImmGetDefaultIMEWnd(hWnd);
                //        WinApi.SendMessage(imeWnd, WinApi.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, lang.Handle);
                //        lblLog.Text = DateTime.Now.ToString() + " --[焦点控件] " + controlName ?? processName ?? windowTitle;

                //        ChangeCursorColorByIme(lang.LayoutName);
                //    }
                //}
            }
            catch
            {
                throw;
            }


        }

        private Rule FindMatchingRule(string appName, string windowTitle, string controlClass)
        {
            foreach (var group in AppRuleGroups)
            {
                //if (Regex.IsMatch(appName, group.AppName))
                if (appName == group.AppName)
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
            var existingApps = AppRuleGroups.Select(g => g.AppName).ToList();
            using var processSelectForm = new ProcessSelectForm(existingApps);
            if (processSelectForm.ShowDialog(this) == DialogResult.OK && processSelectForm.SelectedProcess != null)
            {
                var selectedProcess = processSelectForm.SelectedProcess;
                string appName = selectedProcess.ProcessName;
                string displayName = processSelectForm.SelectedProcessDisplayName;

                System.Diagnostics.ProcessModule mainModule = null;
                try
                {
                    mainModule = selectedProcess.MainModule;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
                var existingGroup = AppRuleGroups.FirstOrDefault(g => g.AppName == appName);
                if (existingGroup != null)
                {
                    MessageBox.Show("该应用已存在规则组中，请双击编辑。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var newGroup = new AppRuleGroup(appName, displayName, mainModule?.FileName);

                var defaultRule = new Rule(Rule.CreateDefaultName(appName, RuleNams.程序名称), RuleType.Program, appName, cmbDefaultIme.Text);
                newGroup.AddRule(defaultRule);

                var list = AppRuleGroups.ToList();
                list.Add(newGroup);

                var index = list.OrderBy(t => t.AppName).ToList().FindIndex(t => t.AppName == appName);
                AppRuleGroups.Insert(index, newGroup);
                SaveRulesToJson(false);
                TreeNode groupNode = new(newGroup.ToString())
                {
                    Tag = newGroup,
                    NodeFont = new Font(treeApps.Font, FontStyle.Bold),
                    ForeColor = Color.DarkOrange
                };
                groupNode.Nodes.Add(new TreeNode(defaultRule.ToString())
                {
                    Tag = defaultRule,
                    ForeColor = Color.DarkSeaGreen
                });
                treeApps.Nodes.Insert(index, groupNode);
                treeApps.SelectedNode = groupNode;
                treeApps.SelectedNode.Expand();
                treeApps.Focus();
                //UpdateTreeView();

                using var editAppRulesForm = new EditAppRulesForm(this, treeApps.SelectedNode, cmbDefaultIme.Items.Cast<string>());
                editAppRulesForm.ShowDialog(this);
            }
        }

        private void BtnRemoveApp_Click(object sender, EventArgs e)
        {
            if (treeApps.SelectedNode != null)
            {
                if (treeApps.SelectedNode.Tag is AppRuleGroup group)
                {
                    AppRuleGroups.Remove(group);
                    SaveRulesToJson(false);
                    treeApps.Nodes.Remove(treeApps.SelectedNode);
                }
                else if (treeApps.SelectedNode.Tag is Rule rule && treeApps.SelectedNode.Parent != null)
                {
                    if (treeApps.SelectedNode.Parent.Tag is AppRuleGroup parentGroup)
                    {
                        parentGroup.RemoveRule(rule);
                        var parentNode = treeApps.SelectedNode.Parent;
                        treeApps.Nodes.Remove(treeApps.SelectedNode);
                        if (parentGroup.Rules.Count == 0)
                        {
                            AppRuleGroups.Remove(parentGroup);
                            treeApps.Nodes.Remove(parentNode);
                        }
                        SaveRulesToJson(false);
                    }
                }
                treeApps.Focus();
            }
        }

        private void TreeApps_DoubleClick(object sender, EventArgs e)
        {
            if (treeApps.SelectedNode != null)
            {
                treeApps.SelectedNode.Expand();
                //if (treeApps.SelectedNode.Tag is AppRuleGroup group)
                //{
                //}
                //else if (treeApps.SelectedNode.Tag is Rule rule && treeApps.SelectedNode.Parent != null)
                //{
                //    if (treeApps.SelectedNode.Parent.Tag is AppRuleGroup parentGroup)
                //    {
                //        treeApps.SelectedNode = treeApps.SelectedNode.Parent;
                //        treeApps.Focus();
                //    }
                //}

                using var editAppRulesForm = new EditAppRulesForm(this, treeApps.SelectedNode, cmbDefaultIme.Items.Cast<string>());
                editAppRulesForm.ShowDialog(this);
            }
        }

        private void UpdateTreeView()
        {
            treeApps.Nodes.Clear();

            //Font boldFont = new Font(treeApps.Font, FontStyle.Bold);
            Font regularFont = new Font(treeApps.Font.FontFamily, 11, FontStyle.Regular);
            foreach (var group in AppRuleGroups.OrderBy(g => g.AppName))
            {
                var groupNode = new TreeNode(group.ToString())
                {
                    Tag = group,
                    //NodeFont = boldFont,
                    ForeColor = Color.DarkOrange
                };

                foreach (var rule in group.Rules)
                {
                    AppHelper.AddRuleNodeToGroup(groupNode, rule);
                }
                foreach (TreeNode node in groupNode.Nodes)
                {
                    node.NodeFont = regularFont;
                }
                treeApps.Nodes.Add(groupNode);
                groupNode.Expand();
            }
            if (treeApps.Nodes.Count > 0)
            {
                treeApps.Nodes[0]?.EnsureVisible();
            }
        }


        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnWhitelist_Click(object sender, EventArgs e)
        {
            using var whitelistForm = new WhitelistForm(this);
            whitelistForm.ShowDialog(this);
            LoadWhitelist(); // 重新加载白名单
        }

        public void LoadWhitelist()
        {
            try
            {
                string jsonPath = GetWhitelistJsonPath();
                if (File.Exists(jsonPath))
                {
                    string json = File.ReadAllText(jsonPath);

                    // 尝试反序列化为新格式

                    var loadedApps = JsonSerializer.Deserialize<List<WhitelistApp>>(json);
                    if (loadedApps != null)
                    {
                        _whitelistedApps.Clear();
                        foreach (var app in loadedApps)
                        {
                            _whitelistedApps.Add(app.Name);
                        }
                        _whitelistedApps.Add("explorer"); // 永远忽略资源管理器
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载白名单失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }



        private void SaveCursorColorConfig()
        {
            try
            {
                var colorDict = new Dictionary<string, string>();
                // foreach (var kvp in _imeColors)
                // {
                //     colorDict[kvp.Key] = ColorTranslator.ToHtml(kvp.Value);
                // }
                // var settings = _appSettings;
                // //todo
                // // settings.ImeColors = JsonSerializer.Serialize(colorDict);
                // settings.ImeColors = _imeColors;

                // settings.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存光标颜色配置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        /// <summary>
        /// 改变光标颜色
        /// </summary>
        /// <param name="color"></param>
        /// <param name="imeName"></param>
        private void ChangeCursorColor(Color color, string imeName = null)
        {
            try
            {
                ChangeCaretColor(color);
                UpdateTrayIconColor(color);


                ShowFloatingHint(color, imeName ?? _currentImeName);



                //TryUpdateCaretWidth(color);
            }
            catch (Exception ex)
            {
                lblLog.Text = $"视觉提示设置失败: {ex.Message}";
                throw;
            }
        }

        /// <summary>
        /// 改变插入符号颜色
        /// </summary>
        /// <param name="color"></param>
        /// <param name="imeName"></param>
        private void ChangeCaretColor(Color color)
        {
            try
            {
                CaretHelper.SetCaretColor(color);
            }
            catch (Exception ex)
            {
                lblLog.Text = $"视觉提示设置失败: {ex.Message}";
                throw;
            }
        }

        private void ShowFloatingHint(Color color, string imeName)
        {
            Debug.WriteLine("chagne color Process Name:" + _changeColorProcessName);
            Debug.WriteLine("last Active App:" + _lastActiveApp);

            _hintForm?.Close();
            _hintForm?.Dispose();
            _hintForm = null;
            if (string.IsNullOrEmpty(_changeColorProcessName) || string.IsNullOrEmpty(_lastActiveApp))
            {
                return;
            }
            if (_changeColorProcessName == "explorer" && _lastActiveApp == "explorer")
            {
                return;
            }


            // 检查是否在白名单中
            if (_whitelistedApps.Any(app => app == _changeColorProcessName || app == _lastActiveApp))
            {
                return;
            }
            //if (this.InvokeRequired)
            //{
            //    this.Invoke(new Action(() => ShowFloatingHint(color, imeName)));
            //    return;
            //}

            //if (currentHintForm != null && !currentHintForm.IsDisposed)
            //{
            //    currentHintForm.Close();
            //    currentHintForm.Dispose();
            //    currentHintForm = null;
            //}


            Point displayPos = CaretHelper.GetBestFloatingHintPosition();

            // 验证和调整坐标
            displayPos = AppHelper.ValidateAndAdjustPosition(displayPos);

            Color backColor = _appSettings.HintBackColor.Value;

            // 根据SameHintColor设置决定提示窗文本颜色
            Color textColor = _appSettings.TextColorSameHintColor ? color : _appSettings.HintTextColor.Value;

            _hintForm = new FloatingHintForm(color, imeName, backColor, _appSettings.HintOpacity, _appSettings.HintFont, textColor);
            _hintForm.Location = displayPos;
            _hintForm.Show();
            Debug.WriteLine($"{DateTime.Now:T}  ShowFloatingHint:{imeName}\n");
            // _currentHintForm = hintForm;
        }

        private void UpdateTrayIconColor(Color color)
        {
            using (Bitmap bmp = new Bitmap(18, 18))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                if (_currentImeName == "中文(英)")
                {
                    g.Clear(Color.Transparent);

                    using (Pen pen = new Pen(color, 3))
                    {
                        g.DrawEllipse(pen, 0, 0, 16, 16);
                    }//在右边绘制一个绿色字符A
                    using (Font font = new Font("Arial", 10, FontStyle.Bold))
                    using (Brush textBrush = new SolidBrush(Color.LightGreen))
                    {
                        g.DrawString("A", font, textBrush, 3, 1);
                    }

                }
                else
                {
                    using (Brush brush = new SolidBrush(color))
                    {
                        g.FillEllipse(brush, 0, 0, 16, 16);
                    }

                    using (Pen pen = new Pen(Color.White, 2))
                    {
                        g.DrawEllipse(pen, 0, 0, 16, 16);
                    }

                }

                Icon oldIcon = _trayIcon.Icon;
                _trayIcon.Icon = Icon.FromHandle(bmp.GetHicon());
                if (oldIcon != null)
                {
                    oldIcon.Dispose();
                }
                if (_trayIcon.Visible == false)
                {
                    _trayIcon.Visible = true;
                }
                if (_trayIcon.Icon == null)
                {
                    _trayIcon.Icon = (Icon)this.Icon.Clone();
                }

            }
        }

        private int GetCaretWidthFromColor(Color color)
        {
            return Math.Max(1, color.GetHashCode() % 5 + 1);
        }

        /// <summary>
        /// 根据输入法改变光标颜色
        /// </summary>
        /// <param name="imeName"></param>
        private void ChangeCursorColorByIme(string imeName)
        {
            if (imeName == _currentImeName)
            {
                //todo 是否需要判断
                // return;
            }
            //if (imeName.Contains("中文") || imeName == "英文")
            //{
            //    //Debug.WriteLine("输入法变化: " + imeName);

            //    string layoutName = null;
            //    if (imeName.Contains("中文"))
            //    {
            //        //Debug.WriteLine("currentImeName:" + currentImeName);
            //        //if (currentImeName.Contains("中")) return;
            //        layoutName = InputLanguage.InstalledInputLanguages.Cast<InputLanguage>().FirstOrDefault(t => t.LayoutName.Contains("中"))?.LayoutName;
            //    }
            //    else
            //    {
            //        //Debug.WriteLine("currentImeName:" + currentImeName);
            //        //if (currentImeName.Contains("英") || currentImeName.Contains("美")) return;
            //        layoutName = InputLanguage.InstalledInputLanguages.Cast<InputLanguage>().FirstOrDefault(t => t.LayoutName == ("美式键盘"))?.LayoutName;
            //        if (layoutName == null)
            //        {
            //            layoutName = InputLanguage.InstalledInputLanguages.Cast<InputLanguage>().FirstOrDefault(t => t.LayoutName.Contains("英"))?.LayoutName;
            //        }
            //    }
            //    imeColors.TryGetValue(layoutName ?? imeName, out Color color);
            //    ChangeCursorColor(color, imeName);
            //    currentImeName = layoutName ?? imeName;
            //    return;
            //}
            //Debug.WriteLine("输入法变化: " + imeName);
            //if (string.IsNullOrEmpty(_currentImeName))
            //{
            //    _currentImeName = imeName;
            //    return;
            //}
            string activeProcessName = AppHelper.GetForegroundProcessName();
            if (activeProcessName == "explorer")
            {
                return;
            }
            Debug.WriteLine("activeProcessName:" + activeProcessName);
            //if (imeName != currentImeName && !string.IsNullOrEmpty(currentImeName))
            // 
            if (true)
            {
                _currentImeName = imeName;
                if (_appSettings.ImeColors.TryGetValue(imeName, out Color color))
                {
                    ChangeCursorColor(color, imeName);
                }
                else
                {

                    switch (imeName)
                    {
                        case "中文(英)":
                            color = _appSettings.ImeColors.GetValueOrDefault("中文", Color.Black);
                            ChangeCursorColor(color, imeName);
                            break;
                        default:
                            ChangeCursorColor(Color.Black, imeName);
                            break;

                    }
                }
            }
            //else
            //{
            //    //currentImeName = imeName;
            //    currentImeName = "";
            //}
        }


        private void btnExpanAll_Click(object sender, EventArgs e)
        {
            treeApps.ExpandAll();
            treeApps.Nodes[0].EnsureVisible();
        }

        private void BtnCollapseAll_Click(object sender, EventArgs e)
        {
            treeApps.CollapseAll();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRulesFromJson();
            UpdateTreeView();
        }

        private void BtnHintColorSettings_Click(object sender, EventArgs e)
        {
            OpenHintColorSettings();
        }

        private void OpenHintColorSettings()
        {
            using (var colorSettingsForm = new HintColorSettingsForm())
            {
                colorSettingsForm.ImeColors = new Dictionary<string, Color>(_appSettings.ImeColors);

                if (colorSettingsForm.ShowDialog() == DialogResult.OK)
                {
                    //todo
                    // _appSettings.ImeFormColors = colorSettingsForm.ImeColors;
                    SaveCursorColorConfig();

                    // 重新加载appSettings以确保获取最新保存的配置
                    _appSettings = AppSettings.Load();

                    // 如果当前输入法有颜色设置，更新光标颜色
                    if (_appSettings.ImeColors.TryGetValue(_currentImeName, out Color currentColor))
                    {
                        ChangeCursorColor(currentColor);
                    }
                    // LoadFloatingHintSettings不再需要，因为我们直接使用_appSettings的属性
                }
            }
        }

        private void chkAlwayShowHint_CheckedChanged(object sender, EventArgs e)
        {
            //this._alwayShowHint = chkAlwayShowHint.Checked;
            _appSettings.AlwayShowHint = chkAlwayShowHint.Checked;
        }
    }
}