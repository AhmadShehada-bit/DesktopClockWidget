using System;
using System.IO;
using System.Configuration;
using System.Text.Json;

namespace DesktopClockWidget
{
    public class WidgetSettings
    {
        public string FontFamily { get; set; } = "Rajdhani";
        public double FontSizeWeekday { get; set; } = 64;
        public double FontSizeGreeting { get; set; } = 20;
        public double FontSizeDateTime { get; set; } = 18;
        public double FontSizeSymbols { get; set; } = 16;
        public string TextColor { get; set; } = "#D1CFCC";
        public double OpacityWeekday { get; set; } = 0.94;
        public double OpacityGreeting { get; set; } = 0.92;
        public double OpacityDateTime { get; set; } = 0.85;
        public double OpacitySymbols { get; set; } = 0.82;
        public bool Topmost { get; set; } = false;
        public bool ClickThrough { get; set; } = true;
        public bool RunOnStartup { get; set; } = true;
        public double Left { get; set; } = 0;
        public double Top { get; set; } = 0;
        public double Width { get; set; } = 300;
        public double Height { get; set; } = 200;
    }

    public static class SettingsManager
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DesktopClock", "DesktopClockWidget.settings");

        public static WidgetSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    var settings = JsonSerializer.Deserialize<WidgetSettings>(json);
                    if (settings != null) return settings;
                }
            }
            catch { }
            return new WidgetSettings();
        }

        public static void Save(WidgetSettings settings)
        {
            try
            {
                var dir = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch { }
        }
    }
}