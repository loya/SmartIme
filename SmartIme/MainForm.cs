using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Timers;
using SmartIme.Utilities;

namespace SmartIme
{
    public partial class MainForm : Form
    {
        private readonly BindingList<AppRuleGroup> appRuleGroups = [];
        private readonly System.Timers.Timer monitorTimer = new();
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;

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
            DeserializeRules(Properties.Settings.Default.AppRules);
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
            Properties.Settings.Default.AppRules = SerializeRules();
            Properties.Settings.Default.Save();

            monitorTimer.Stop();
        }

        private string SerializeRules()
        {
            var groupEntries = new List<string>();

            foreach (var group in appRuleGroups)
            {
                var ruleEntries = group.Rules.Select(r => $"{r.Name}|{(int)r.Type}|{r.Pattern}|{r.InputMethod}");
                var rulesStr = string.Join(",", ruleEntries);
                groupEntries.Add($"{group.AppName}|{group.DisplayName}|{rulesStr}");
            }

            return string.Join(";", groupEntries);
        }

        private void DeserializeRules(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var groupEntries = data.Split(';');
                foreach (var groupEntry in groupEntries)
                {
                    var parts = groupEntry.Split('|');
                    if (parts.Length >= 3)
                    {
                        string appName = parts[0];
                        string displayName = parts[1];

                        var group = new AppRuleGroup(appName, displayName);

                        // 解析规则
                        string rulesStr = string.Join("|", parts.Skip(2));
                        if (!string.IsNullOrEmpty(rulesStr))
                        {
                            var ruleEntries = rulesStr.Split(',');
                            foreach (var ruleEntry in ruleEntries)
                            {
                                var ruleParts = ruleEntry.Split('|');
                                if (ruleParts.Length == 4)
                                {
                                    string name = ruleParts[0];
                                    RuleType type = (RuleType)int.Parse(ruleParts[1]);
                                    string pattern = ruleParts[2];
                                    string inputMethod = ruleParts[3];

                                    var rule = new Rule(name, type, pattern, inputMethod);
                                    group.AddRule(rule);
                                }
                            }
                        }

                        appRuleGroups.Add(group);

                        //treeApps.Nodes.Add(group);
                    }
                }
            }
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
                    //lblLog.Text = "(1)" + DateTime.Now.ToString() + " " + controlName;
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
                            WinApi.SendMessage(imeWnd, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, lang.Handle);
                            lblLog.Text = DateTime.Now.ToLongTimeString() + " --[焦点控件] " + controlName ?? processName ?? windowTitle;

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
                        WinApi.SendMessage(imeWnd, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, lang.Handle);
                        lblLog.Text = DateTime.Now.ToString() + " --[焦点控件] " + controlName ?? processName ?? windowTitle;
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
            WinApi.SendMessage(imeWnd, WM_INPUTLANGCHANGEREQUEST, (IntPtr)0x0029, IntPtr.Zero);

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
                lstProcesses.Items.Add($"{process.ProcessName} - {process.MainModule?.ModuleName}");
            }

            // 显示窗口
            if (processSelectForm.ShowDialog(this) == DialogResult.OK && lstProcesses.SelectedIndex >= 0)
            {
                var selectedProcess = processes[lstProcesses.SelectedIndex];
                string appName = selectedProcess.ProcessName;
                string displayName = $"{appName} - {selectedProcess.MainModule?.ModuleName}";

                // 检查是否已存在该应用
                var existingGroup = appRuleGroups.FirstOrDefault(g => g.AppName == appName);
                if (existingGroup != null)
                {
                    MessageBox.Show("该应用已存在规则组中，请双击编辑。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 创建新的应用规则组
                var newGroup = new AppRuleGroup(appName, displayName);

                // 添加默认规则
                var defaultRule = new Rule(Rule.CreateDefaultName(appName,RuleNams.程序名称), RuleType.Program, appName, cmbDefaultIme.Text);
                newGroup.AddRule(defaultRule);

                // 添加到列表
                var list = appRuleGroups.ToList();
                list.Add(newGroup);

                var index = list.OrderBy(t => t.AppName).ToList().FindIndex(t => t.AppName == appName);
                appRuleGroups.Insert(index, newGroup);
                UpdateTreeView();
                // 打开编辑窗口
                using var editAppRulesForm = new EditAppRulesForm(newGroup, cmbDefaultIme.Items.Cast<object>());
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
                    UpdateTreeView();
                }
                else if (treeApps.SelectedNode.Tag is Rule rule && treeApps.SelectedNode.Parent != null)
                {
                    if (treeApps.SelectedNode.Parent.Tag is AppRuleGroup parentGroup)
                    {
                        parentGroup.RemoveRule(rule);
                        UpdateTreeView();
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
                    using var editAppRulesForm = new EditAppRulesForm(group, cmbDefaultIme.Items.Cast<object>());
                    editAppRulesForm.ShowDialog(this);
                    UpdateTreeView();
                }
                else if (treeApps.SelectedNode.Tag is Rule rule && treeApps.SelectedNode.Parent != null)
                {
                    if (treeApps.SelectedNode.Parent.Tag is AppRuleGroup parentGroup)
                    {
                        using var editAppRulesForm = new EditAppRulesForm(parentGroup, cmbDefaultIme.Items.Cast<object>());
                        editAppRulesForm.ShowDialog(this);
                        UpdateTreeView();
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
                    
                    var ruleNode = new TreeNode($"{rule.Name} ({rule.Pattern}) -> {rule.InputMethod}")
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


    }
}