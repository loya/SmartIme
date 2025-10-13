using System.Text.Json.Serialization;

namespace SmartIme.Models
{
    public enum RuleType
    {
        程序名称,    // 基于程序名称
        窗口标题,      // 基于窗口标题
        控件     // 基于控件类型
    }

    /// <summary>
    /// 规则匹配模式
    /// </summary>
    public enum RuleMatchPattern
    {
        等于,
        包含
    }

    public class Rule : ICloneable
    {
        public string RuleName { get; set; }
        public string AppName { get; set; } = string.Empty;
        /// <summary>
        /// 规则类型，程序名称、窗口标题、控件类型
        /// </summary>
        [JsonConverter(typeof(SmartIme.Utilities.EnumJsonConverter<RuleType>))]
        public RuleType RuleType { get; set; }
        /// <summary>
        /// 匹配内容，程序名称、窗口标题、控件类型
        /// </summary>
        public string MatchContent { get; set; }
        /// <summary>
        /// 匹配模式，等于、包含
        /// </summary>
        [JsonConverter(typeof(SmartIme.Utilities.EnumJsonConverter<RuleMatchPattern>))]
        public RuleMatchPattern MatchPattern { get; set; }
        public string InputMethod { get; set; }
        /// <summary>
        /// 优先级，数字越大优先级越高   
        /// </summary>
        public int Priority { get; set; }

        public Rule()
        {
        }

        public Rule(string ruleName, RuleType ruleType, RuleMatchPattern matchPattern, string matchContent, string inputMethod)
        {
            RuleName = ruleName;
            RuleType = ruleType;
            MatchPattern = matchPattern;
            MatchContent = matchContent;
            InputMethod = inputMethod;

            // 设置优先级
            switch (ruleType)
            {
                case RuleType.控件:
                    Priority = 3;
                    break;
                case RuleType.窗口标题:
                    Priority = 2;
                    break;
                case RuleType.程序名称:
                    Priority = 1;
                    break;
            }
        }

        public override string ToString()
        {
            return $"{RuleName} -（{MatchPattern}：{MatchContent}）-> [{InputMethod}]";
        }

        public static string CreateDefaultName(string appName, RuleType ruleNams)
        {
            return $"{appName}【{ruleNams}】";
        }

        public object Clone()
        {
            return new Rule
            {
                RuleName = this.RuleName,
                AppName = this.AppName,
                RuleType = this.RuleType,
                MatchPattern = this.MatchPattern,
                MatchContent = this.MatchContent,
                InputMethod = this.InputMethod,
                Priority = this.Priority
            };
        }
    }
}