using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartIme
{
    public partial class MainForm : Form
    {
        private List<Rule> rules = new List<Rule>();
        private Timer monitorTimer;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        
        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);
        
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
        
        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        public MainForm()
        {
            InitializeComponent();
            InitializeImeList();
            SetupMonitor();
            SetupTrayIcon();
            
            // 绑定事件处理程序
            btnSwitchIme.Click += btnSwitchIme_Click;
            btnAddApp.Click += btnAddApp_Click;
            btnRemoveApp.Click += btnRemoveApp_Click;
            this.FormClosing += MainForm_FormClosing;
            
            // 加载保存的设置
            cmbDefaultIme.SelectedIndex = Properties.Settings.Default.DefaultIme;
            DeserializeRules(Properties.Settings.Default.AppRules);
        }
        
        private void SetupTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("显示主窗口", null, (s, e) => this.Show());
            trayMenu.Items.Add("退出", null, (s, e) => Application.Exit());
            
            trayIcon = new NotifyIcon();
            trayIcon.Text = "输入法智能切换助手";
            trayIcon.Icon = System.Drawing.SystemIcons.Application;
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;
            trayIcon.DoubleClick += (s, e) => this.Show();
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 保存设置
            Properties.Settings.Default.DefaultIme = cmbDefaultIme.SelectedIndex;
            Properties.Settings.Default.AppRules = SerializeRules();
            Properties.Settings.Default.Save();
        }
        
        private string SerializeRules()
        {
            var entries = rules.Select(r => $"{r.Name}|{(int)r.Type}|{r.Pattern}|{r.InputMethod}");
            return string.Join(";", entries);
        }
        
        private void DeserializeRules(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var entries = data.Split(';');
                foreach (var entry in entries)
                {
                    var parts = entry.Split('|');
                    if (parts.Length == 4)
                    {
                        string name = parts[0];
                        RuleType type = (RuleType)int.Parse(parts[1]);
                        string pattern = parts[2];
                        string inputMethod = parts[3];
                        
                        var rule = new Rule(name, type, pattern, inputMethod);
                        rules.Add(rule);
                        lstApps.Items.Add(rule);
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

        private void SetupMonitor()
        {
            monitorTimer = new Timer();
            monitorTimer.Interval = 300;
            monitorTimer.Tick += CheckActiveWindowChanged;
            monitorTimer.Start();
        }

        private void CheckActiveWindowChanged(object sender, EventArgs e)
        {
            IntPtr currentWindow = GetForegroundWindow();
            
            // 只有当活动窗口变化时才处理
            if (currentWindow != lastActiveWindow)
            {
                lastActiveWindow = currentWindow;
                MonitorActiveApp();
            }
        }

        private void MonitorActiveApp()
        {
            IntPtr hWnd = GetForegroundWindow();
            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);
            
            try
            {
                var process = System.Diagnostics.Process.GetProcessById((int)processId);
                string appName = process.ProcessName;
                
                // 如果是同一个应用程序，检查是否有基于窗口标题或控件类型的规则
                if (appName == lastActiveApp)
                {
                    // 检查是否存在针对该应用的规则
                    bool hasRulesForThisApp = rules.Any(r => 
                        // 检查程序规则
                        (r.Type == RuleType.Program && Regex.IsMatch(appName, r.Pattern)) ||
                        // 或者存在标题或控件规则
                        (r.Type == RuleType.Title || r.Type == RuleType.Control));
                    
                    // 如果没有针对该应用的规则，则不重复切换输入法
                    if (!hasRulesForThisApp)
                    {
                        return;
                    }
                }
                
                lastActiveApp = appName;
                
                // 获取窗口标题
                var titleBuilder = new System.Text.StringBuilder(256);
                GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);
                string windowTitle = titleBuilder.ToString();
                
                // 获取控件类名
                var classBuilder = new System.Text.StringBuilder(256);
                GetClassName(hWnd, classBuilder, classBuilder.Capacity);
                string controlClass = classBuilder.ToString();
                
                // 按优先级查找匹配的规则
                Rule matchedRule = FindMatchingRule(appName, windowTitle, controlClass);
                
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
                            IntPtr imeWnd = ImmGetDefaultIMEWnd(hWnd);
                            SendMessage(imeWnd, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, lang.Handle);
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
                        IntPtr imeWnd = ImmGetDefaultIMEWnd(hWnd);
                        SendMessage(imeWnd, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, lang.Handle);
                    }
                }
            }
            catch { }
        }
        
        private Rule FindMatchingRule(string appName, string windowTitle, string controlClass)
        {
            // 按优先级排序规则
            var sortedRules = rules.OrderByDescending(r => r.Priority).ToList();
            
            // 先检查控件规则
            foreach (var rule in sortedRules.Where(r => r.Type == RuleType.Control))
            {
                if (Regex.IsMatch(controlClass, rule.Pattern))
                    return rule;
            }
            
            // 再检查标题规则
            foreach (var rule in sortedRules.Where(r => r.Type == RuleType.Title))
            {
                if (Regex.IsMatch(windowTitle, rule.Pattern))
                    return rule;
            }
            
            // 最后检查程序规则
            foreach (var rule in sortedRules.Where(r => r.Type == RuleType.Program))
            {
                if (Regex.IsMatch(appName, rule.Pattern))
                    return rule;
            }
            
            return null;
        }

        private void btnSwitchIme_Click(object sender, EventArgs e)
        {
            IntPtr hWnd = GetForegroundWindow();
            IntPtr imeWnd = ImmGetDefaultIMEWnd(hWnd);
            
            // 0x0029 是切换到下一个输入法的消息
            SendMessage(imeWnd, WM_INPUTLANGCHANGEREQUEST, (IntPtr)0x0029, IntPtr.Zero);
            
            // 更新显示
            uint threadId = GetWindowThreadProcessId(hWnd, out _);
            IntPtr hkl = GetKeyboardLayout(threadId);
            lblCurrentIme.Text = "当前输入法：" + GetImeName(hkl);
        }
        
        private string GetImeName(IntPtr hkl)
        {
            // 这里应该根据hkl获取输入法名称
            // 简化处理，直接返回下拉框中的选项
            return cmbDefaultIme.Text;
        }

        private void btnAddApp_Click(object sender, EventArgs e)
        {
            using (var addRuleForm = new AddRuleForm(cmbDefaultIme.Items.Cast<object>(), cmbDefaultIme.SelectedIndex))
            {
                if (addRuleForm.ShowDialog(this) == DialogResult.OK)
                {
                    var rule = addRuleForm.CreatedRule;
                    if (rule != null)
                    {
                        rules.Add(rule);
                        lstApps.Items.Add(rule);
                    }
                }
            }
        }

        private void btnRemoveApp_Click(object sender, EventArgs e)
        {
            if (lstApps.SelectedItem != null)
            {
                var rule = lstApps.SelectedItem as Rule;
                if (rule != null)
                {
                    rules.Remove(rule);
                    lstApps.Items.Remove(rule);
                }
            }
        }

        private void lstApps_DoubleClick(object sender, EventArgs e)
        {
            if (lstApps.SelectedItem != null)
            {
                var rule = lstApps.SelectedItem as Rule;
                if (rule != null)
                {
                    using (var editRuleForm = new EditRuleForm(rule, cmbDefaultIme.Items.Cast<object>()))
                    {
                        if (editRuleForm.ShowDialog(this) == DialogResult.OK)
                        {
                            // 刷新列表显示
                            int index = lstApps.Items.IndexOf(rule);
                            if (index >= 0)
                            {
                                lstApps.Items[index] = rule;
                            }
                        }
                    }
                }
            }
        }
    }
}