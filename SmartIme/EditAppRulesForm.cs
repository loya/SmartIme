using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using SmartIme.Utilities;

namespace SmartIme
{
    public partial class EditAppRulesForm : Form
    {
        private readonly AppRuleGroup appRuleGroup;
        private readonly IEnumerable<object> inputMethods;

        public EditAppRulesForm(AppRuleGroup appRuleGroup, IEnumerable<object> inputMethods)
        {
            InitializeComponent();
            this.appRuleGroup = appRuleGroup;
            this.inputMethods = inputMethods;

            // 设置窗口标题
            this.Text = $"编辑 {appRuleGroup.DisplayName} 的规则";

            // 加载规则列表
            RefreshRulesList();
        }

        private void RefreshRulesList()
        {
            lstRules.Items.Clear();
            //foreach (var rule in appRuleGroup.Rules)
            //{
            //    lstRules.Items.Add(rule);
            //}
            lstRules.Items.AddRange(appRuleGroup.Rules.Select(r => r).OrderByDescending(t => t.Priority).ToArray());
        }

        private void BtnAddRule_Click(object sender, EventArgs e)
        {
            using var addRuleForm = new AddRuleForm(inputMethods, 0, appRuleGroup.AppName);
            if (addRuleForm.ShowDialog(this) == DialogResult.OK)
            {
                var rule = addRuleForm.CreatedRule;
                if (rule != null)
                {

                    int index = appRuleGroup.Rules.FindIndex(t => t.Priority <= rule.Priority);
                    appRuleGroup.InsertRule(index, rule);
                    RefreshRulesList();
                }
            }
        }

        private void BtnRemoveRule_Click(object sender, EventArgs e)
        {
            if (lstRules.SelectedItem != null)
            {
                if (lstRules.SelectedItem is Rule rule)
                {
                    appRuleGroup.RemoveRule(rule);
                    RefreshRulesList();
                }
            }
        }

        private void LstRules_DoubleClick(object sender, EventArgs e)
        {
            if (lstRules.SelectedItem != null)
            {
                if (lstRules.SelectedItem is Rule rule)
                {
                    using var editRuleForm = new EditRuleForm(rule, inputMethods);
                    if (editRuleForm.ShowDialog(this) == DialogResult.OK)
                    {
                        RefreshRulesList();
                    }
                }
            }
        }
    }
}