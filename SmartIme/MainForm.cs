using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace SmartIme
{
    public partial class MainForm : Form
    {
        private Dictionary<string, string> appImeRules = new Dictionary<string, string>();
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
            return string.Join(";", appImeRules.Select(x => $"{x.Key},{x.Value}"));
        }
        
        private void DeserializeRules(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var entries = data.Split(';');
                foreach (var entry in entries)
                {
                    var parts = entry.Split(',');
                    if (parts.Length == 2)
                    {
                        appImeRules[parts[0]] = parts[1];
                        lstApps.Items.Add(parts[0]);
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

        private void SetupMonitor()
        {
            monitorTimer = new Timer();
            monitorTimer.Interval = 500;
            monitorTimer.Tick += MonitorActiveApp;
            monitorTimer.Start();
        }

        private void MonitorActiveApp(object sender, EventArgs e)
        {
            IntPtr hWnd = GetForegroundWindow();
            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);
            
            try
            {
                var process = System.Diagnostics.Process.GetProcessById((int)processId);
                string appName = process.ProcessName;
                
                // 检查是否有对应的输入法规则
                if (appImeRules.ContainsKey(appName))
                {
                    // 切换输入法
                    string targetIme = appImeRules[appName];
                    lblCurrentIme.Text = "当前输入法：" + targetIme;
                    
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
                    lblCurrentIme.Text = "当前输入法：" + cmbDefaultIme.Text;
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
            var processForm = new Form()
            {
                Text = "选择运行中的应用程序",
                Size = new System.Drawing.Size(400, 500),
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
                    string appName = listBox.SelectedItem.ToString();
                    if (!appImeRules.ContainsKey(appName))
                    {
                        // 弹出规则配置对话框
                        var ruleForm = new Form()
                        {
                            Text = "配置输入法规则 - " + appName,
                            Size = new System.Drawing.Size(300, 200),
                            FormBorderStyle = FormBorderStyle.FixedDialog,
                            StartPosition = FormStartPosition.CenterParent
                        };

                        var label = new Label()
                        {
                            Text = "选择默认输入法:",
                            Location = new System.Drawing.Point(20, 20),
                            AutoSize = true
                        };

                        var combo = new ComboBox()
                        {
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            Location = new System.Drawing.Point(20, 50),
                            Width = 250
                        };
                        combo.Items.AddRange(cmbDefaultIme.Items.Cast<object>().ToArray());
                        combo.SelectedIndex = cmbDefaultIme.SelectedIndex;

                        var btnOk = new Button()
                        {
                            Text = "确定",
                            DialogResult = DialogResult.OK,
                            Location = new System.Drawing.Point(120, 100)
                        };

                        ruleForm.Controls.Add(label);
                        ruleForm.Controls.Add(combo);
                        ruleForm.Controls.Add(btnOk);
                        ruleForm.AcceptButton = btnOk;

                        if (ruleForm.ShowDialog(this) == DialogResult.OK)
                        {
                            appImeRules[appName] = combo.Text;
                            lstApps.Items.Add(appName);
                        }
                    }
                    processForm.Close();
                }
            };

            processForm.Controls.Add(listBox);
            processForm.Controls.Add(btnSelect);
            processForm.ShowDialog(this);
        }

        private void btnRemoveApp_Click(object sender, EventArgs e)
        {
            if (lstApps.SelectedItem != null)
            {
                string appName = lstApps.SelectedItem.ToString();
                appImeRules.Remove(appName);
                lstApps.Items.Remove(appName);
            }
        }

        private void lstApps_DoubleClick(object sender, EventArgs e)
        {
            if (lstApps.SelectedItem != null)
            {
                string appName = lstApps.SelectedItem.ToString();
                
                // 弹出规则编辑对话框
                var ruleForm = new Form()
                {
                    Text = "编辑输入法规则 - " + appName,
                    Size = new System.Drawing.Size(300, 200),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent
                };

                var label = new Label()
                {
                    Text = "选择默认输入法:",
                    Location = new System.Drawing.Point(20, 20),
                    AutoSize = true
                };

                var combo = new ComboBox()
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new System.Drawing.Point(20, 50),
                    Width = 250
                };
                combo.Items.AddRange(cmbDefaultIme.Items.Cast<object>().ToArray());
                combo.SelectedItem = appImeRules[appName];

                var btnOk = new Button()
                {
                    Text = "确定",
                    DialogResult = DialogResult.OK,
                    Location = new System.Drawing.Point(120, 100)
                };

                ruleForm.Controls.Add(label);
                ruleForm.Controls.Add(combo);
                ruleForm.Controls.Add(btnOk);
                ruleForm.AcceptButton = btnOk;

                if (ruleForm.ShowDialog(this) == DialogResult.OK)
                {
                    appImeRules[appName] = combo.Text;
                }
            }
        }
    }
}