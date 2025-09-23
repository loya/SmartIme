namespace SmartIme.Utilities
{
    /// <summary>
    /// 应用规则组，包含一个应用的多个规则
    /// </summary>
    public class AppRuleGroup(string appName, string displayName = null,string appPath=null)
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
        public string AppPath { get; set; }=appPath;

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
            Rules.Add(rule);
        }
        /// <summary>
        /// 添加规则
        /// </summary>
        public void InsertRule(int index, Rule rule)
        {
            if (index < 0 )
            {
                index = 0;
            }
            if (index> Rules.Count)
            {
              index=Rules.Count;
            }
            Rules.Insert(index,rule);
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
            foreach (var rule in sortedRules.Where(r => r.Type == RuleType.Control))
            {
                if (controlClass == rule.Pattern)
                    return rule;
            }

            // 再检查标题规则
            foreach (var rule in sortedRules.Where(r => r.Type == RuleType.Title))
            {
                if (windowTitle == rule.Pattern)
                    return rule;
            }

            // 最后检查程序规则
            foreach (var rule in sortedRules.Where(r => r.Type == RuleType.Program))
            {
                if (appName == rule.Pattern)
                    return rule;
            }

            return null;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}