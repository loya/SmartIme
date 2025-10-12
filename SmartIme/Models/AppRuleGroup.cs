namespace SmartIme.Models
{
    /// <summary>
    /// 应用规则组，包含一个应用的多个规则
    /// </summary>
    public class AppRuleGroup(string appName, string displayName = null, string appPath = null) : ICloneable
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; } = appName;

        /// <summary>
        /// 应用显示名称
        /// </summary>
        public string DisplayName { get; set; } = displayName ?? appName;

        /// <summary>
        /// 应用路径
        /// </summary>
        public string AppPath { get; set; } = appPath;

        /// <summary>
        /// 应用图标路径（可选）
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// 应用的规则列表
        /// </summary>
        public List<Rule> Rules { get; set; } = [];

        /// <summary>
        /// 添加规则
        /// </summary>
        public void AddRule(Rule rule)
        {
            rule.AppName = AppName;
            Rules.Add(rule);
        }
        /// <summary>
        /// 添加规则
        /// </summary>
        public void InsertRule(int index, Rule rule)
        {
            if (index < 0)
            {
                index = 0;
            }
            if (index > Rules.Count)
            {
                index = Rules.Count;
            }
            Rules.Insert(index, rule);
        }

        /// <summary>
        /// 移除规则
        /// </summary>
        public void RemoveRule(Rule rule)
        {
            Rules.Remove(rule);
        }

        /// <summary>
        /// 查找匹配的规则
        /// </summary>
        public Rule FindMatchingRule(string appName, string windowTitle, string controlClass)
        {
            // 按优先级排序规则
            var sortedRules = Rules.OrderByDescending(r => r.Priority).ToList();

            // 先检查控件规则
            foreach (var rule in sortedRules.Where(r => r.RuleType == RuleType.Control))
            {
                //MessageBox.Show(rule.MatchPattern.ToString(), "模式");
                bool isMatch = rule.MatchPattern switch
                {
                    RuleMatchPattern.等于 => controlClass == rule.MatchContent,
                    RuleMatchPattern.包含 => controlClass?.Contains(rule.MatchContent) == true,
                    _ => controlClass == rule.MatchContent
                };

                if (isMatch)
                    return rule;
            }

            // 再检查标题规则
            foreach (var rule in sortedRules.Where(r => r.RuleType == RuleType.Title))
            {
                bool isMatch = rule.MatchPattern switch
                {
                    RuleMatchPattern.等于 => windowTitle == rule.MatchContent,
                    RuleMatchPattern.包含 => windowTitle?.Contains(rule.MatchContent) == true,
                    _ => windowTitle == rule.MatchContent
                };

                if (isMatch)
                    return rule;
            }

            // 最后检查程序规则
            foreach (var rule in sortedRules.Where(r => r.RuleType == RuleType.Program))
            {
                bool isMatch = rule.MatchPattern switch
                {
                    RuleMatchPattern.等于 => appName == rule.MatchContent,
                    RuleMatchPattern.包含 => appName?.Contains(rule.MatchContent) == true,
                    _ => appName == rule.MatchContent
                };

                if (isMatch)
                    return rule;
            }

            return null;
        }

        public override string ToString()
        {
            return DisplayName + (AppPath != null ? "（" + AppPath + "）" : "");
        }

        public object Clone()
        {
            var clonedGroup = new AppRuleGroup(AppName, DisplayName, AppPath)
            {
                AppName = AppName,
                DisplayName = DisplayName,
                AppPath = AppPath,
                IconPath = IconPath,
                Rules = Rules.Select(r => (Rule)r.Clone()).ToList()
            };
            return clonedGroup;
        }
    }
}