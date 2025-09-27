namespace SmartIme.Utilities
{
    public enum RuleType
    {
        Program,    // 基于程序名称
        Title,      // 基于窗口标题
        Control     // 基于控件类型
    }

    public enum RuleNams
    {
        程序名称,
        窗口标题,
        控件
    };

    public class Rule : ICloneable
    {
        public string Name { get; set; }
        public string AppName { get; set; } = string.Empty;
        public RuleType Type { get; set; }
        public string Pattern { get; set; }
        public string InputMethod { get; set; }
        /// <summary>
        /// 优先级，数字越大优先级越高   
        /// </summary>
        public int Priority { get; set; }

        public Rule()
        {
        }

        public Rule(string name, RuleType type, string pattern, string inputMethod)
        {
            Name = name;
            Type = type;
            Pattern = pattern;
            InputMethod = inputMethod;

            // 设置优先级
            switch (type)
            {
                case RuleType.Control:
                    Priority = 3;
                    break;
                case RuleType.Title:
                    Priority = 2;
                    break;
                case RuleType.Program:
                    Priority = 1;
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Name}（{Pattern}）-> [{InputMethod}]";
        }

        public static string CreateDefaultName(string appName, RuleNams ruleNams)
        {
            return $"{appName}【{ruleNams}】";
        }

        public object Clone()
        {
            return new Rule
            {
                Name = this.Name,
                AppName = this.AppName,
                Type = this.Type,
                Pattern = this.Pattern,
                InputMethod = this.InputMethod,
                Priority = this.Priority
            };
        }
    }
}