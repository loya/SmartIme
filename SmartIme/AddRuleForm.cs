using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmartIme
{
    public partial class AddRuleForm : Form
    {
        public Rule CreatedRule { get; private set; }

        public AddRuleForm(IEnumerable<object> imeList, int defaultImeIndex)
        {
            InitializeComponent();
            
            // 初始化输入法列表
            cmbIme.Items.AddRange(imeList.ToArray());
            if (cmbIme.Items.Count > 0)
            {
                cmbIme.SelectedIndex = defaultImeIndex;
            }
            
            // 初始化规则类型
            cmbType.Items.AddRange(new object[] { "程序名称", "窗口标题", "控件类型" });
            cmbType.SelectedIndex = 0;
        }

        private void btnSelectProcess_Click(object sender, EventArgs e)
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