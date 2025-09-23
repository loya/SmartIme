using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SmartIme.Utilities;

namespace SmartIme
{
    public partial class EditRuleForm : Form
    {
        public Rule EditedRule { get; private set; }

        public EditRuleForm(Rule rule, IEnumerable<string> imeList)
        {
            InitializeComponent();
            EditedRule = rule;
            
            // 初始化表单数据
            txtName.Text = rule.Name;
            txtPattern.Text = rule.Pattern;
            
            // 初始化输入法列表
            cmbIme.Items.AddRange(imeList.ToArray());
            cmbIme.SelectedItem = rule.InputMethod;
            
            // 初始化规则类型
            cmbType.Items.AddRange(new object[] { "程序名称", "窗口标题", "控件类型" });
            cmbType.SelectedIndex = (int)rule.Type;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPattern.Text))
            {
                MessageBox.Show("请填写规则名称和匹配模式", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 更新规则
            EditedRule.Name = txtName.Text;
            EditedRule.Type = (RuleType)cmbType.SelectedIndex;
            EditedRule.Pattern = txtPattern.Text;
            EditedRule.InputMethod = cmbIme.Text;
            
            // 更新优先级
            switch (EditedRule.Type)
            {
                case RuleType.Control:
                    EditedRule.Priority = 3;
                    break;
                case RuleType.Title:
                    EditedRule.Priority = 2;
                    break;
                case RuleType.Program:
                    EditedRule.Priority = 1;
                    break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}