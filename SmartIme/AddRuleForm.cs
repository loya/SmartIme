using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmartIme
{
    public partial class AddRuleForm : Form
    {
        public Rule CreatedRule { get; private set; }
        public string AppName { get; set; }

        public AddRuleForm(IEnumerable<object> imeList, int defaultImeIndex, string appName = null)
        {            
            InitializeComponent();
            this.cmbType.SelectedIndexChanged += cmbType_SelectedIndexChanged;
            
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

        private void btnSelectProcess_Click(object sender, EventArgs e)
        {
            if (cmbType.SelectedIndex == 1) // 窗口标题规则
            {
                ShowWindowTitlesSelection();
            }else if (cmbType.SelectedIndex == 2) // 控件类型规则
            {
                SelectControlClass();
            }
            else // 程序名称规则
            {
                ShowProcessSelection();
            }
        }

        private void SelectControlClass()
        {

            // 如果是控件类型规则，添加控件选择功能
            var controlForm = new Form()
            {
                Text = "选择控件",
                Size = new System.Drawing.Size(400, 300),
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var btnSelect = new Button()
            {
                Text = "点击控件进行选择",
                Dock = DockStyle.Fill
            };

            btnSelect.Click += (s, args) =>
            {
                // 这里需要实现控件选择逻辑
                // 实际应用中需要使用Windows API获取当前焦点控件信息
                txtPattern.Text = "Edit"; // 示例值，实际应为选择的控件类名
                controlForm.Close();
            };

            controlForm.Controls.Add(btnSelect);
            controlForm.ShowDialog(this);
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

            List<string> titles = getCurrentAppTitles();

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

        private List<string> getCurrentAppTitles()
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

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelectProcess.Text = cmbType.SelectedIndex == 1 ? "选择窗口标题" : "选择应用程序";
            btnSelectProcess.Visible = cmbType.SelectedIndex == 1 || cmbType.SelectedIndex == 2;
            
            // 当切换到窗口标题规则时，自动显示选择窗口标题对话框
            if (cmbType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtPattern.Text))
            {
                List<string> titles = getCurrentAppTitles();

                if (titles.Count > 0)
                {
                    // 设置第一个标题为默认值
                    txtPattern.Text = titles[0];
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
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