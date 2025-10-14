using SmartIme.Utilities;
using System.Text.Json;

namespace SmartIme.Models
{
    /// <summary>
    /// Manages application settings using a JSON file (AppSettings.json) instead of the legacy Settings.settings system.
    /// This class provides methods to load, save, and migrate settings from the old system.
    /// </summary>
    public class AppSettings
    {
        public Size WindowSize { get; set; }
        public Point WindowLocation { get; set; }
        public FormWindowState WindowState { get; set; }
        public double HintOpacity { get; set; }
        public bool TextColorSameHintColor { get; set; }
        public int DefaultIme { get; set; }

        public Font HintFont { get; set; }
        public Color? HintBackColor { get; set; }
        public Color? HintTextColor { get; set; }
        // public Dictionary<string, Color> ImeColors { get; set; }
        public List<ImeColor> ImeColors { get; set; }
        public bool AlwayShowHint { get; set; }

        #region 方法
        private static readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings", "AppSettings.json");

        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new ColorJsonConverter(), new FontJsonConverter() }
        };
        public static AppSettings Load()
        {
            AppSettings settings;
            if (!File.Exists(SettingsPath))
            {
                settings = new AppSettings
                {
                    AlwayShowHint = true,
                    HintBackColor = Color.Black,
                    HintOpacity = 0.6,
                    HintFont = (Font)new FontConverter().ConvertFromString("Microsoft YaHei, 12pt"),
                    HintTextColor = Color.White,
                    TextColorSameHintColor = false,
                    DefaultIme = 0,
                    WindowSize = Size.Empty,
                    WindowLocation = Point.Empty,
                    WindowState = FormWindowState.Normal,
                    // ImeColors = new Dictionary<string, Color>()
                    // {
                    //     { "中文",Color.Red },
                    //     { "英文", Color.Lime},
                    // },
                    ImeColors = SetDefaultImeColors()

                };
                settings.Save();
                return settings;
            }

            string json = File.ReadAllText(SettingsPath);
            settings = JsonSerializer.Deserialize<AppSettings>(json, _options);
            if (settings.ImeColors == null || settings.ImeColors.Count == 0)
            {
                settings.ImeColors = SetDefaultImeColors();
            }
            if (settings.HintFont == null)
            {
                settings.HintFont = (Font)new FontConverter().ConvertFromString("Microsoft YaHei, 12pt,style=bold");
            }
            return settings;

        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this, _options);
            File.WriteAllText(SettingsPath, json);
        }

        private static List<ImeColor> SetDefaultImeColors()
        {
            List<ImeColor> imeColors = new List<ImeColor>();
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                imeColors.Add(new ImeColor()
                {
                    LayoutName = lang.LayoutName,
                    LangID = string.Format($"0x{lang.Handle & 0xFFFF:X4}"),
                    Color = (lang.Handle & 0xFFFF) switch
                    {
                        0x0804 => Color.Red,
                        0x0404 => Color.Red,
                        0x0409 => Color.Lime,
                        0x0809 => Color.Lime,
                        _ => Color.White
                    },
                    HintText = (lang.Handle & 0xFFFF) switch
                    {
                        0x0804 => "中文",
                        0x0404 => "中文(繁体)",
                        0x0409 => "英文",
                        0x0809 => "英文",
                        0x0411 => "日语",
                        0x0412 => "韩语",
                        _ => lang.LayoutName
                    },
                });
            }
            ;
            return imeColors;
        }

        #endregion

    }

    public class ImeColor()
    {
        public string LayoutName { get; set; }
        public string LangID { get; set; }

        public Color Color { get; set; }
        public string HintText { get; set; }
    }
}