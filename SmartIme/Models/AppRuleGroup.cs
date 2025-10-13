using System.Text.Json;

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
            foreach (var rule in sortedRules.Where(r => r.RuleType == RuleType.控件))
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
            foreach (var rule in sortedRules.Where(r => r.RuleType == RuleType.窗口标题))
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
            foreach (var rule in sortedRules.Where(r => r.RuleType == RuleType.程序名称))
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

        #region 序列化和反序列化相关方法

        /// <summary>
        /// 保存所有AppRuleGroup到JSON文件
        /// </summary>
        /// <param name="groups">要保存的AppRuleGroup列表</param>
        /// <param name="jsonPath">JSON文件路径</param>
        /// <param name="updateTreeView">是否更新树视图</param>
        public static void SaveGroupsToJson(List<AppRuleGroup> groups, string jsonPath, bool updateTreeView = true)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters =
                {
                    new SmartIme.Utilities.EnumJsonConverter<Models.RuleType>(),
                    new SmartIme.Utilities.EnumJsonConverter<Models.RuleMatchPattern>()
                }
            };

            try
            {
                string json = JsonSerializer.Serialize(groups, options);
                File.WriteAllText(jsonPath, json);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"保存规则失败: {ex.Message}", "错误",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 从JSON文件加载AppRuleGroup列表
        /// </summary>
        /// <param name="jsonPath">JSON文件路径</param>
        /// <returns>加载的AppRuleGroup列表</returns>
        public static List<AppRuleGroup> LoadGroupsFromJson(string jsonPath)
        {
            var groups = new List<AppRuleGroup>();

            try
            {
                if (File.Exists(jsonPath))
                {
                    string json = File.ReadAllText(jsonPath);
                    var options = new JsonSerializerOptions
                    {
                        Converters =
                        {
                            new SmartIme.Utilities.EnumJsonConverter<Models.RuleType>(),
                            new SmartIme.Utilities.EnumJsonConverter<Models.RuleMatchPattern>()
                        }
                    };
                    var loadedGroups = JsonSerializer.Deserialize<List<AppRuleGroup>>(json, options);
                    if (loadedGroups != null)
                    {
                        groups = loadedGroups;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"加载规则失败: {ex.Message}", "错误",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            return groups;
        }

        /// <summary>
        /// 保存单个AppRuleGroup到JSON文件
        /// </summary>
        /// <param name="jsonPath">JSON文件路径</param>
        public void SaveGroupToJson(string jsonPath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters =
                {
                    new SmartIme.Utilities.EnumJsonConverter<Models.RuleType>(),
                    new SmartIme.Utilities.EnumJsonConverter<Models.RuleMatchPattern>()
                }
            };

            try
            {
                var singleGroupList = new List<AppRuleGroup> { this };
                string json = JsonSerializer.Serialize(singleGroupList, options);
                File.WriteAllText(jsonPath, json);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"保存规则失败: {ex.Message}", "错误",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}