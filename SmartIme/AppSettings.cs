using System;
using System.IO;
using System.Text.Json;
using System.Configuration;
using System.Windows.Forms; // Needed for FormWindowState


namespace SmartIme
{
    /// <summary>
    /// Manages application settings using a JSON file (AppSettings.json) instead of the legacy Settings.settings system.
    /// This class provides methods to load, save, and migrate settings from the old system.
    /// </summary>
    public class AppSettings
    {
        public string FloatingHintBackColor { get; set; }
        public double FloatingHintOpacity { get; set; }
        public string FloatingHintFont { get; set; }
        public string FloatingHintTextColor { get; set; }
        public bool SameHintColor { get; set; }
        public int DefaultIme { get; set; }
        public Size WindowSize { get; set; }
        public Point WindowLocation { get; set; }
        public FormWindowState WindowState { get; set; }
        public string ImeColors { get; set; }

        private static readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings", "AppSettings.json");

        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static AppSettings Load()
        {
            var settings = new AppSettings();


            if (!File.Exists(SettingsPath))
            {
                settings = new AppSettings
                {
                    FloatingHintBackColor = "#000000",
                    FloatingHintOpacity = 0.7,
                    FloatingHintFont = "Microsoft YaHei, 12pt",
                    FloatingHintTextColor = "#FFFFFF",
                    SameHintColor = false,
                    DefaultIme = 0,
                    WindowSize = Size.Empty,
                    WindowLocation = Point.Empty,
                    WindowState = FormWindowState.Normal,
                    ImeColors = ""
                };
                settings.Save();
                return settings;
            }

            string json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json, _options);
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this, _options);
            File.WriteAllText(SettingsPath, json);
        }

    }
}