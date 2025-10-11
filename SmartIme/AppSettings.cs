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
        public int DefaultIme { get; set; }
        public System.Drawing.Size WindowSize { get; set; }
        public System.Drawing.Point WindowLocation { get; set; }
        public System.Windows.Forms.FormWindowState WindowState { get; set; }
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
            
            // Try to migrate old settings first
            settings.MigrateFromLegacySettings();
            
            if (!File.Exists(SettingsPath))
            {
                settings = new AppSettings
                {
                    FloatingHintBackColor = "#000000",
                    FloatingHintOpacity = 0.7,
                    FloatingHintFont = "Microsoft YaHei, 12pt",
                    FloatingHintTextColor = "#FFFFFF",
                    DefaultIme = 0,
                    WindowSize = System.Drawing.Size.Empty,
                    WindowLocation = System.Drawing.Point.Empty,
                    WindowState = System.Windows.Forms.FormWindowState.Normal,
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
        
        /// <summary>
        /// Migrate settings from the legacy Settings.settings system to the new AppSettings.json format
        /// </summary>
        private void MigrateFromLegacySettings()
        {
            try
            {
                // Check if AppSettings.json already exists, if so, skip migration
                if (File.Exists(SettingsPath))
                {
                    return;
                }
                
                // Create a new settings object with values from the legacy settings
                var legacySettings = Properties.Settings.Default;
                
                // Only migrate if the legacy settings have non-default values
                // We'll check if FloatingHintBackColor is still at its default value as a proxy for whether
                // settings have been customized
                if (legacySettings.FloatingHintBackColor != "#000000" || 
                    legacySettings.FloatingHintOpacity != 0.7 ||
                    legacySettings.FloatingHintFont != "Microsoft YaHei, 12pt" ||
                    legacySettings.FloatingHintTextColor != "#FFFFFF" ||
                    legacySettings.DefaultIme != 0 ||
                    legacySettings.WindowLocation != System.Drawing.Point.Empty ||
                    legacySettings.WindowSize != System.Drawing.Size.Empty ||
                    legacySettings.WindowState != FormWindowState.Normal)
                {
                    this.FloatingHintBackColor = legacySettings.FloatingHintBackColor;
                    this.FloatingHintOpacity = legacySettings.FloatingHintOpacity;
                    this.FloatingHintFont = legacySettings.FloatingHintFont;
                    this.FloatingHintTextColor = legacySettings.FloatingHintTextColor;
                    this.DefaultIme = legacySettings.DefaultIme;
                    this.WindowSize = legacySettings.WindowSize;
                    this.WindowLocation = legacySettings.WindowLocation;
                    this.WindowState = legacySettings.WindowState;
                    this.ImeColors = legacySettings.ImeColors;
                    
                    // Save the migrated settings to the new format
                    this.Save();
                }
            }
            catch (Exception ex)
            {
                // In case of error during migration, log it and continue with default settings
                System.Diagnostics.Debug.WriteLine($"Settings migration failed: {ex.Message}");
            }
        }
    }
}