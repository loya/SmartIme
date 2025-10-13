using SmartIme.Models;
using SmartIme.Utilities;

namespace SmartIme
{
    public partial class EditAppRulesForm : Form
    {
        private TreeNode _appRuleGroupNode;
        private readonly AppRuleGroup _originalEditAppRuleGroup;
        private readonly AppRuleGroup _tempEditAppRuleGroup;
        private readonly IEnumerable<string> _inputMethods;
        private MainForm MainForm { get; }
        private bool _isModify = false;

        public EditAppRulesForm(MainForm mainForm, TreeNode selectedNode, IEnumerable<string> inputMethods, bool isAddApp = false)
        {
            InitializeComponent();
            FormClosing += (s, e) =>
            {
                if (_isModify)
                {
                    var result = MessageBox.Show("规则已修改，是否保存？", "保存修改", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        //mainForm.SaveRulesToJson(false);
                        //_isModify = false;
                        btnOK.PerformClick();
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
                _appRuleGroupNode = selectedNode;
                this._originalEditAppRuleGroup = group;
                this._tempEditAppRuleGroup = (AppRuleGroup)group.Clone();
            }
            else if (selectedNode.Tag is Rule rule && selectedNode.Parent != null)
            {
                if (selectedNode.Parent.Tag is AppRuleGroup parentGroup)
                {
                    _appRuleGroupNode = selectedNode.Parent;
                    this._originalEditAppRuleGroup = parentGroup;
                    this._tempEditAppRuleGroup = (AppRuleGroup)parentGroup.Clone();
                }
            }

            this._inputMethods = inputMethods;
            this.MainForm = mainForm;
            // 设置窗口标题
            this.Text = $"编辑 {_tempEditAppRuleGroup.DisplayName} 的规则";

            // 加载规则列表
            RefreshRulesList();
            if (isAddApp)
            {
                lstRules.SelectedIndex = 0;
                this.Shown += (s, e) =>
                {
                    LstRules_DoubleClick(null, EventArgs.Empty);
                };

            }
        }

        private void RefreshRulesList()
        {
            lstRules.Items.Clear();
            //foreach (var rule in _tempEditAppRuleGroup.Rules)
            //{
            //    lstRules.Items.Add(rule);
            //}
            lstRules.Items.AddRange(_tempEditAppRuleGroup.Rules.Select(r => r).OrderByDescending(t => t.Priority).ToArray());
        }

        private void BtnAddRule_Click(object sender, EventArgs e)
        {
            using var addRuleForm = new AddRuleForm(_inputMethods, 0, _tempEditAppRuleGroup.AppName);
            if (addRuleForm.ShowDialog(this) == DialogResult.OK)
            {
                var rule = addRuleForm.CreatedRule;
                if (rule != null)
                {
                    //int index = _tempEditAppRuleGroup.Rules.FindIndex(t => t.Priority <= rule.Priority);
                    //_tempEditAppRuleGroup.InsertRule(index, rule);
                    _tempEditAppRuleGroup.AddRule(rule);
                    _tempEditAppRuleGroup.Rules = _tempEditAppRuleGroup.Rules
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
                    _tempEditAppRuleGroup.RemoveRule(rule);
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
                    using var addRuleForm = new AddRuleForm(_inputMethods, rule);
                    if (addRuleForm.ShowDialog(this) == DialogResult.OK)
                    {
                        if (addRuleForm.CreatedRule != null)
                        {
                            _tempEditAppRuleGroup.Rules[lstRules.SelectedIndex] = addRuleForm.CreatedRule;
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
                this._originalEditAppRuleGroup.Rules = [.. this._tempEditAppRuleGroup.Rules];
                if (this._originalEditAppRuleGroup.Rules.Count == 0)
                {
                    var result = MessageBox.Show("当前应用的规则列表为空，是否删除该应用？", "删除应用", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        MainForm.AppRuleGroups.Remove(this._originalEditAppRuleGroup);
                    }
                    else
                    {
                        // 如果不删除应用，则不允许关闭窗口
                        DialogResult = DialogResult.None;
                        return;
                    }
                }
                MainForm.SaveRulesToJson(false);
                if (_originalEditAppRuleGroup.Rules.Count != 0)
                {
                    _appRuleGroupNode.Nodes.Clear();
                    foreach (var rule in _originalEditAppRuleGroup.Rules)
                    {
                        AppHelper.AddRuleNodeToGroup(_appRuleGroupNode, rule, MainForm.TreeNodefont);
                    }
                    _appRuleGroupNode.Expand();
                }
                else
                {
                    _appRuleGroupNode.Remove();
                }
                _isModify = false;
            }
        }
    }
}