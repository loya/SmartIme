using SmartIme.Models;
using SmartIme.Utilities;

namespace SmartIme
{
    public partial class EditAppRulesForm : Form
    {
        private TreeNode appRuleGroupNode;
        private readonly AppRuleGroup originalAppRuleGroup;
        private readonly AppRuleGroup tempAppRuleGroup;
        private readonly IEnumerable<string> inputMethods;
        private MainForm mainForm { get; }
        private bool _isModify = false;

        public EditAppRulesForm(MainForm mainForm, TreeNode selectedNode, IEnumerable<string> inputMethods)
        {
            InitializeComponent();
            FormClosing += (s, e) =>
            {
                if (_isModify)
                {
                    var result = MessageBox.Show("规则已修改，是否保存？", "保存修改", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        mainForm.SaveRulesToJson(false);
                        _isModify = false;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            };

            // 将 group.Clone() 的返回值强制转换为 AppRuleGroup 类型
            if (selectedNode.Tag is AppRuleGroup group)
            {
                appRuleGroupNode = selectedNode;
                this.originalAppRuleGroup = group;
                this.tempAppRuleGroup = (AppRuleGroup)group.Clone();
            }
            else if (selectedNode.Tag is Rule rule && selectedNode.Parent != null)
            {
                if (selectedNode.Parent.Tag is AppRuleGroup parentGroup)
                {
                    appRuleGroupNode = selectedNode.Parent;
                    this.originalAppRuleGroup = parentGroup;
                    this.tempAppRuleGroup = parentGroup;
                }
            }

            this.inputMethods = inputMethods;
            this.mainForm = mainForm;
            // 设置窗口标题
            this.Text = $"编辑 {tempAppRuleGroup.DisplayName} 的规则";

            // 加载规则列表
            RefreshRulesList();
        }

        private void RefreshRulesList()
        {
            lstRules.Items.Clear();
            //foreach (var rule in tempAppRuleGroup.Rules)
            //{
            //    lstRules.Items.Add(rule);
            //}
            lstRules.Items.AddRange(tempAppRuleGroup.Rules.Select(r => r).OrderByDescending(t => t.Priority).ToArray());
        }

        private void BtnAddRule_Click(object sender, EventArgs e)
        {
            using var addRuleForm = new AddRuleForm(inputMethods, 0, tempAppRuleGroup.AppName);
            if (addRuleForm.ShowDialog(this) == DialogResult.OK)
            {
                var rule = addRuleForm.CreatedRule;
                if (rule != null)
                {
                    //int index = tempAppRuleGroup.Rules.FindIndex(t => t.Priority <= rule.Priority);
                    //tempAppRuleGroup.InsertRule(index, rule);
                    tempAppRuleGroup.AddRule(rule);
                    tempAppRuleGroup.Rules = tempAppRuleGroup.Rules
                        .OrderByDescending(t => t.Priority).ThenBy(t => t.RuleName).ToList();
                    RefreshRulesList();
                    _isModify = true;
                }
            }
        }

        private void BtnRemoveRule_Click(object sender, EventArgs e)
        {
            if (lstRules.SelectedItem != null)
            {
                if (lstRules.SelectedItem is Rule rule)
                {
                    tempAppRuleGroup.RemoveRule(rule);
                    RefreshRulesList();
                    _isModify = true;
                }
            }
        }

        private void LstRules_DoubleClick(object sender, EventArgs e)
        {
            if (lstRules.SelectedItem != null)
            {
                if (lstRules.SelectedItem is Rule rule)
                {
                    using var addRuleForm = new AddRuleForm(inputMethods, rule);
                    if (addRuleForm.ShowDialog(this) == DialogResult.OK)
                    {
                        if (addRuleForm.CreatedRule != null)
                        {
                            rule = addRuleForm.CreatedRule;
                            RefreshRulesList();
                            _isModify = true;
                        }
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_isModify)
            {
                // 将修改应用到 mainForm.AppRuleGroups 中
                this.originalAppRuleGroup.Rules = [.. this.tempAppRuleGroup.Rules];
                if (this.originalAppRuleGroup.Rules.Count == 0)
                {
                    var result = MessageBox.Show("当前应用的规则列表为空，是否删除该应用？", "删除应用", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        mainForm.AppRuleGroups.Remove(this.originalAppRuleGroup);
                    }
                    else
                    {
                        // 如果不删除应用，则不允许关闭窗口
                        DialogResult = DialogResult.None;
                        return;
                    }
                }
                mainForm.SaveRulesToJson(false);
                if (originalAppRuleGroup.Rules.Count != 0)
                {
                    appRuleGroupNode.Nodes.Clear();
                    foreach (var rule in originalAppRuleGroup.Rules)
                    {
                        AppHelper.AddRuleNodeToGroup(appRuleGroupNode, rule);
                    }
                    appRuleGroupNode.Expand();
                }
                else
                {
                    appRuleGroupNode.Remove();
                }
                _isModify = false;
            }
        }
    }
}