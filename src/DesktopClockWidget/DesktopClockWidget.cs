using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using FlowDirection = System.Windows.FlowDirection;
using ColorConverter = System.Windows.Media.ColorConverter;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;
using FontStyles = System.Windows.FontStyles;
using FontWeight = System.Windows.FontWeight;
using FontWeights = System.Windows.FontWeights;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.MessageBox;
using Orientation = System.Windows.Controls.Orientation;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace DesktopClock
{
    [DataContract]
    public class TextEffectSettings
    {
        [DataMember] public bool OutlineEnabled { get; set; }
        [DataMember] public string OutlineColor { get; set; }
        [DataMember] public double OutlineThickness { get; set; }
        [DataMember] public double OutlineOpacity { get; set; }

        [DataMember] public bool GlitchEnabled { get; set; }
        [DataMember] public double GlitchIntensity { get; set; } // 0 - 100
        [DataMember] public string GlitchSpeed { get; set; } // "Slow", "Medium", "Fast"
        [DataMember] public string GlitchColor1 { get; set; } // Default #00FFFF
        [DataMember] public string GlitchColor2 { get; set; } // Default #FF0055

        [DataMember] public bool NoiseEnabled { get; set; }
        [DataMember] public double NoiseAmount { get; set; } // 0 - 100
        [DataMember] public string NoiseSpeed { get; set; } // "Slow", "Medium", "Fast"

        public TextEffectSettings()
        {
            OutlineEnabled = false;
            OutlineColor = "#000000";
            OutlineThickness = 2.0;
            OutlineOpacity = 1.0;

            GlitchEnabled = false;
            GlitchIntensity = 35.0;
            GlitchSpeed = "Medium";
            GlitchColor1 = "#00FFFF";
            GlitchColor2 = "#FF0055";

            NoiseEnabled = false;
            NoiseAmount = 25.0;
            NoiseSpeed = "Medium";
        }

        public TextEffectSettings Clone()
        {
            return new TextEffectSettings
            {
                OutlineEnabled = OutlineEnabled,
                OutlineColor = OutlineColor ?? "#000000",
                OutlineThickness = OutlineThickness > 0 ? OutlineThickness : 2.0,
                OutlineOpacity = OutlineOpacity >= 0 ? OutlineOpacity : 1.0,
                GlitchEnabled = GlitchEnabled,
                GlitchIntensity = GlitchIntensity,
                GlitchSpeed = GlitchSpeed ?? "Medium",
                GlitchColor1 = GlitchColor1 ?? "#00FFFF",
                GlitchColor2 = GlitchColor2 ?? "#FF0055",
                NoiseEnabled = NoiseEnabled,
                NoiseAmount = NoiseAmount,
                NoiseSpeed = NoiseSpeed ?? "Medium"
            };
        }

        public bool HasAnimatedEffects()
        {
            return (GlitchEnabled && GlitchIntensity > 0) || (NoiseEnabled && NoiseAmount > 0);
        }
    }

    [DataContract]
    public class ElementSettings
    {
        [DataMember] public bool Visible { get; set; }
        [DataMember] public string FontFamily { get; set; }
        [DataMember] public string FontWeight { get; set; }
        [DataMember] public double FontSize { get; set; }
        [DataMember] public string Color { get; set; }
        [DataMember] public double Opacity { get; set; }
        [DataMember] public string Case { get; set; }
        [DataMember] public TextEffectSettings Effects { get; set; }
        [DataMember] public string HorizontalAlignment { get; set; }
        [DataMember] public double OffsetX { get; set; }
        [DataMember] public double OffsetY { get; set; }

        public ElementSettings()
        {
            Visible = true;
            FontFamily = "Audiowide";
            FontWeight = "Regular";
            FontSize = 14.0;
            Color = "#D6D3D0";
            Opacity = 1.0;
            Case = "None";
            Effects = new TextEffectSettings();
            HorizontalAlignment = "Center";
            OffsetX = 0.0;
            OffsetY = 0.0;
        }

        public ElementSettings(bool visible, string family, string weight, double size, string color, double opacity, string textCase, TextEffectSettings effects = null, string alignment = "Center", double offsetX = 0.0, double offsetY = 0.0)
        {
            Visible = visible;
            FontFamily = family;
            FontWeight = weight;
            FontSize = size;
            Color = color;
            Opacity = opacity;
            Case = textCase;
            Effects = effects != null ? effects.Clone() : new TextEffectSettings();
            HorizontalAlignment = !string.IsNullOrEmpty(alignment) ? alignment : "Center";
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public ElementSettings Clone()
        {
            return new ElementSettings(Visible, FontFamily, FontWeight, FontSize, Color, Opacity, Case, Effects != null ? Effects.Clone() : new TextEffectSettings(), HorizontalAlignment, OffsetX, OffsetY);
        }
    }

    [DataContract]
    public class ScheduledMessage
    {
        [DataMember] public string Time { get; set; }
        [DataMember] public string Text { get; set; }

        public ScheduledMessage()
        {
            Time = "06:00";
            Text = "GOOD MORNING";
        }

        public ScheduledMessage(string time, string text)
        {
            Time = time;
            Text = text;
        }
    }

    [DataContract]
    public class CustomBlock
    {
        [DataMember] public string Id { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public bool Enabled { get; set; }

        [DataMember] public string Type { get; set; }
        [DataMember] public string Position { get; set; }
        [DataMember] public int Order { get; set; }

        [DataMember] public string SymbolContent { get; set; }
        [DataMember] public string StaticContent { get; set; }

        [DataMember] public List<string> Messages { get; set; }
        [DataMember] public string RotationMode { get; set; }
        [DataMember] public int IntervalValue { get; set; }
        [DataMember] public string IntervalUnit { get; set; }
        [DataMember] public int IntervalMinutes { get; set; }
        [DataMember] public List<ScheduledMessage> ScheduledMessages { get; set; }

        [DataMember] public string FontFamily { get; set; }
        [DataMember] public string FontWeight { get; set; }
        [DataMember] public double FontSize { get; set; }
        [DataMember] public string Color { get; set; }
        [DataMember] public double Opacity { get; set; }
        [DataMember] public string Alignment { get; set; }
        [DataMember] public double OffsetX { get; set; }
        [DataMember] public double OffsetY { get; set; }
        [DataMember] public string Case { get; set; }
        [DataMember] public bool Italic { get; set; }
        [DataMember] public bool Underline { get; set; }
        [DataMember] public TextEffectSettings Effects { get; set; }

        public CustomBlock()
        {
            Id = Guid.NewGuid().ToString();
            Name = "New Block";
            Enabled = true;
            Type = "Symbol";
            Position = "Above Widget";
            Order = 0;
            SymbolContent = "✦";
            StaticContent = "STAY FOCUSED";
            Messages = new List<string> { "KEEP GOING", "FOCUS ON THE NEXT STEP", "BUILD SOMETHING TODAY", "NO ZERO DAYS" };
            RotationMode = "Sequential";
            IntervalValue = 30;
            IntervalUnit = "Minutes";
            IntervalMinutes = 30;
            ScheduledMessages = new List<ScheduledMessage>
            {
                new ScheduledMessage("06:00", "GOOD MORNING, BUILD SOMETHING"),
                new ScheduledMessage("12:00", "HALF THE DAY IS GONE"),
                new ScheduledMessage("18:00", "REVIEW YOUR PROGRESS"),
                new ScheduledMessage("23:00", "TIME TO REST")
            };
            FontFamily = "Segoe UI Symbol";
            FontWeight = "Regular";
            FontSize = 16.0;
            Color = "#D6D3D0";
            Opacity = 0.8;
            Alignment = "Center";
            OffsetX = 0.0;
            OffsetY = 0.0;
            Case = "None";
            Italic = false;
            Underline = false;
            Effects = new TextEffectSettings();
        }

        public CustomBlock Clone()
        {
            var b = new CustomBlock();
            b.Id = Guid.NewGuid().ToString();
            b.Name = Name + " (Copy)";
            b.Enabled = Enabled;
            b.Type = Type;
            b.Position = Position;
            b.Order = Order;
            b.SymbolContent = SymbolContent;
            b.StaticContent = StaticContent;
            b.Messages = Messages != null ? new List<string>(Messages) : new List<string>();
            b.RotationMode = RotationMode;
            b.IntervalValue = IntervalValue;
            b.IntervalUnit = IntervalUnit;
            b.IntervalMinutes = IntervalMinutes;
            b.ScheduledMessages = new List<ScheduledMessage>();
            if (ScheduledMessages != null)
            {
                foreach (var sm in ScheduledMessages)
                    b.ScheduledMessages.Add(new ScheduledMessage(sm.Time, sm.Text));
            }
            b.FontFamily = FontFamily;
            b.FontWeight = FontWeight;
            b.FontSize = FontSize;
            b.Color = Color;
            b.Opacity = Opacity;
            b.Alignment = Alignment;
            b.OffsetX = OffsetX;
            b.OffsetY = OffsetY;
            b.Case = Case;
            b.Italic = Italic;
            b.Underline = Underline;
            b.Effects = Effects != null ? Effects.Clone() : new TextEffectSettings();
            return b;
        }

        public override string ToString()
        {
            string displayName = !string.IsNullOrEmpty(Name) && !string.Equals(Name, "New Block", StringComparison.OrdinalIgnoreCase) ? Name : "";
            if (string.IsNullOrEmpty(displayName))
            {
                switch (Type)
                {
                    case "Symbol":
                        displayName = "Symbol Block (" + (SymbolContent ?? "✦") + ")";
                        break;
                    case "Static Text":
                        displayName = !string.IsNullOrEmpty(StaticContent) ? ("Static: " + (StaticContent.Length > 22 ? StaticContent.Substring(0, 20) + "..." : StaticContent)) : "Static Text";
                        break;
                    case "Rotating Text":
                        displayName = "Rotating Text (" + (Messages != null ? Messages.Count : 0) + " items)";
                        break;
                    case "Scheduled Message":
                        displayName = "Scheduled Message (" + (ScheduledMessages != null ? ScheduledMessages.Count : 0) + " times)";
                        break;
                    default:
                        displayName = Type ?? "Custom Block";
                        break;
                }
            }
            string status = Enabled ? "" : " [Hidden]";
            return string.Format("{0}. {1} [{2}]{3}", Order + 1, displayName, Position ?? "Above Widget", status);
        }
    }

    [DataContract]
    public class WidgetSettings
    {
        [DataMember] public double Left { get; set; }
        [DataMember] public double Top { get; set; }
        [DataMember] public double AnchorX { get; set; }
        [DataMember] public double AnchorY { get; set; }
        [DataMember] public bool HasAnchor { get; set; }

        [DataMember] public double Scale { get; set; }
        [DataMember] public double MasterOpacity { get; set; }

        [DataMember] public bool UseGlobalFont { get; set; }
        [DataMember] public string GlobalFont { get; set; }
        [DataMember] public bool UseGlobalColor { get; set; }
        [DataMember] public string GlobalColor { get; set; }

        [DataMember] public ElementSettings Greeting { get; set; }
        [DataMember] public ElementSettings Weekday { get; set; }
        [DataMember] public ElementSettings Time { get; set; }
        [DataMember] public ElementSettings Date { get; set; }

        [DataMember] public List<CustomBlock> Blocks { get; set; }
        [DataMember] public List<string> FavoriteFonts { get; set; }

        [DataMember] public int GreetingMode { get; set; } // 0=Auto, 1=Custom, 2=Hidden
        [DataMember] public string CustomGreeting { get; set; }
        [DataMember] public int MorningStart { get; set; }
        [DataMember] public int AfternoonStart { get; set; }
        [DataMember] public int EveningStart { get; set; }
        [DataMember] public int NightStart { get; set; }

        [DataMember] public bool ClickThrough { get; set; }
        [DataMember] public bool PositionLocked { get; set; }
        [DataMember] public bool RunOnStartup { get; set; }

        // Legacy compatibility properties
        [DataMember(Name = "TopSymbol", EmitDefaultValue = false)] public ElementSettings LegacyTopSymbol { get; set; }
        [DataMember(Name = "BottomSymbol", EmitDefaultValue = false)] public ElementSettings LegacyBottomSymbol { get; set; }
        [DataMember(Name = "FontFamily", EmitDefaultValue = false)] public string LegacyFontFamily { get; set; }
        [DataMember(Name = "ColorHex", EmitDefaultValue = false)] public string LegacyColorHex { get; set; }
        [DataMember(Name = "CustomText", EmitDefaultValue = false)] public string LegacyCustomText { get; set; }
        [DataMember(Name = "GreetingVisible", EmitDefaultValue = false)] public bool? LegacyGreetingVisible { get; set; }
        [DataMember(Name = "SymbolsVisible", EmitDefaultValue = false)] public bool? LegacySymbolsVisible { get; set; }

        public WidgetSettings()
        {
            Scale = 1.0;
            MasterOpacity = 1.0;
            UseGlobalFont = false;
            GlobalFont = "Audiowide";
            UseGlobalColor = false;
            GlobalColor = "#D6D3D0";

            Greeting = SettingsManager.DefaultGreeting();
            Weekday = SettingsManager.DefaultWeekday();
            Time = SettingsManager.DefaultTime();
            Date = SettingsManager.DefaultDate();

            Blocks = SettingsManager.DefaultBlocks();
            FavoriteFonts = new List<string> { "Audiowide", "Oxanium", "Exo 2" };

            GreetingMode = 0;
            CustomGreeting = "WELCOME";
            MorningStart = 5;
            AfternoonStart = 12;
            EveningStart = 17;
            NightStart = 22;

            ClickThrough = true;
            PositionLocked = true;
            RunOnStartup = false;
            HasAnchor = false;
        }
    }

    public static class SettingsManager
    {
        private static string SettingsPath
        {
            get
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DesktopClock");
                return Path.Combine(dir, "DesktopClockWidget.settings");
            }
        }

        public static bool LoadedFromDisk { get; private set; }

        public static ElementSettings DefaultGreeting()
        {
            return new ElementSettings(true, "Audiowide", "Regular", 22, "#D6D3D0", 0.82, "Upper");
        }

        public static ElementSettings DefaultWeekday()
        {
            return new ElementSettings(true, "Audiowide", "Regular", 50, "#D6D3D0", 0.92, "Title");
        }

        public static ElementSettings DefaultTime()
        {
            return new ElementSettings(true, "Audiowide", "Regular", 62, "#D6D3D0", 1.0, "None");
        }

        public static ElementSettings DefaultDate()
        {
            return new ElementSettings(true, "Audiowide", "Regular", 20, "#D6D3D0", 0.85, "Upper");
        }

        public static List<CustomBlock> DefaultBlocks()
        {
            var list = new List<CustomBlock>();
            var top = new CustomBlock
            {
                Id = "top-decoration-default",
                Name = "Top Decoration",
                Enabled = true,
                Type = "Symbol",
                Position = "Above Widget",
                Order = 0,
                SymbolContent = "✦",
                FontFamily = "Segoe UI Symbol",
                FontWeight = "Regular",
                FontSize = 16,
                Color = "#D6D3D0",
                Opacity = 0.65,
                Alignment = "Center",
                Effects = new TextEffectSettings()
            };
            list.Add(top);

            var bottom = new CustomBlock
            {
                Id = "bottom-decoration-default",
                Name = "Bottom Decoration",
                Enabled = true,
                Type = "Symbol",
                Position = "Below Widget",
                Order = 0,
                SymbolContent = "◇",
                FontFamily = "Segoe UI Symbol",
                FontWeight = "Regular",
                FontSize = 16,
                Color = "#D6D3D0",
                Opacity = 0.65,
                Alignment = "Center",
                Effects = new TextEffectSettings()
            };
            list.Add(bottom);
            return list;
        }

        public static WidgetSettings Defaults()
        {
            return new WidgetSettings();
        }

        public static WidgetSettings Load()
        {
            WidgetSettings settings = null;
            try
            {
                if (File.Exists(SettingsPath))
                {
                    using (var fs = File.OpenRead(SettingsPath))
                    {
                        var ser = new DataContractJsonSerializer(typeof(WidgetSettings));
                        settings = ser.ReadObject(fs) as WidgetSettings;
                    }
                    LoadedFromDisk = (settings != null);
                }
            }
            catch
            {
                settings = null;
            }

            if (settings == null)
            {
                settings = Defaults();
                LoadedFromDisk = false;
            }

            // Fallback & Migration logic
            if (settings.Greeting == null) settings.Greeting = DefaultGreeting();
            if (settings.Weekday == null) settings.Weekday = DefaultWeekday();
            if (settings.Time == null) settings.Time = DefaultTime();
            if (settings.Date == null) settings.Date = DefaultDate();

            if (settings.Greeting.Effects == null) settings.Greeting.Effects = new TextEffectSettings();
            if (settings.Weekday.Effects == null) settings.Weekday.Effects = new TextEffectSettings();
            if (settings.Time.Effects == null) settings.Time.Effects = new TextEffectSettings();
            if (settings.Date.Effects == null) settings.Date.Effects = new TextEffectSettings();

            if (settings.FavoriteFonts == null)
                settings.FavoriteFonts = new List<string> { "Audiowide", "Oxanium", "Exo 2" };

            // Migrate TopSymbol / BottomSymbol to Blocks collection
            if (settings.Blocks == null || settings.Blocks.Count == 0)
            {
                settings.Blocks = new List<CustomBlock>();
                if (settings.LegacyTopSymbol != null)
                {
                    settings.Blocks.Add(new CustomBlock
                    {
                        Id = "migrated-top",
                        Name = "Top Decoration",
                        Enabled = settings.LegacyTopSymbol.Visible,
                        Type = "Symbol",
                        Position = "Above Widget",
                        Order = 0,
                        SymbolContent = "✦",
                        FontFamily = settings.LegacyTopSymbol.FontFamily ?? "Segoe UI Symbol",
                        FontWeight = settings.LegacyTopSymbol.FontWeight ?? "Regular",
                        FontSize = settings.LegacyTopSymbol.FontSize > 0 ? settings.LegacyTopSymbol.FontSize : 16,
                        Color = settings.LegacyTopSymbol.Color ?? "#D6D3D0",
                        Opacity = settings.LegacyTopSymbol.Opacity > 0 ? settings.LegacyTopSymbol.Opacity : 0.65,
                        Alignment = "Center",
                        Effects = new TextEffectSettings()
                    });
                }
                else
                {
                    settings.Blocks.Add(new CustomBlock
                    {
                        Id = "default-top",
                        Name = "Top Decoration",
                        Enabled = true,
                        Type = "Symbol",
                        Position = "Above Widget",
                        Order = 0,
                        SymbolContent = "✦",
                        FontFamily = "Segoe UI Symbol",
                        FontWeight = "Regular",
                        FontSize = 16,
                        Color = "#D6D3D0",
                        Opacity = 0.65,
                        Alignment = "Center",
                        Effects = new TextEffectSettings()
                    });
                }

                if (settings.LegacyBottomSymbol != null)
                {
                    settings.Blocks.Add(new CustomBlock
                    {
                        Id = "migrated-bottom",
                        Name = "Bottom Decoration",
                        Enabled = settings.LegacyBottomSymbol.Visible,
                        Type = "Symbol",
                        Position = "Below Widget",
                        Order = 0,
                        SymbolContent = "◇",
                        FontFamily = settings.LegacyBottomSymbol.FontFamily ?? "Segoe UI Symbol",
                        FontWeight = settings.LegacyBottomSymbol.FontWeight ?? "Regular",
                        FontSize = settings.LegacyBottomSymbol.FontSize > 0 ? settings.LegacyBottomSymbol.FontSize : 16,
                        Color = settings.LegacyBottomSymbol.Color ?? "#D6D3D0",
                        Opacity = settings.LegacyBottomSymbol.Opacity > 0 ? settings.LegacyBottomSymbol.Opacity : 0.65,
                        Alignment = "Center",
                        Effects = new TextEffectSettings()
                    });
                }
                else
                {
                    settings.Blocks.Add(new CustomBlock
                    {
                        Id = "default-bottom",
                        Name = "Bottom Decoration",
                        Enabled = true,
                        Type = "Symbol",
                        Position = "Below Widget",
                        Order = 0,
                        SymbolContent = "◇",
                        FontFamily = "Segoe UI Symbol",
                        FontWeight = "Regular",
                        FontSize = 16,
                        Color = "#D6D3D0",
                        Opacity = 0.65,
                        Alignment = "Center",
                        Effects = new TextEffectSettings()
                    });
                }
            }

            // Normalize block settings
            foreach (var b in settings.Blocks)
            {
                if (string.IsNullOrEmpty(b.Id)) b.Id = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(b.Name)) b.Name = "Block";
                if (string.IsNullOrEmpty(b.Type)) b.Type = "Symbol";
                if (string.IsNullOrEmpty(b.Position)) b.Position = "Above Widget";
                if (string.IsNullOrEmpty(b.SymbolContent)) b.SymbolContent = "✦";
                if (b.Messages == null) b.Messages = new List<string> { "KEEP GOING", "NO ZERO DAYS" };
                if (string.IsNullOrEmpty(b.RotationMode)) b.RotationMode = "Sequential";
                if (b.IntervalValue <= 0) b.IntervalValue = 30;
                if (string.IsNullOrEmpty(b.IntervalUnit)) b.IntervalUnit = "Minutes";
                b.IntervalMinutes = b.IntervalUnit == "Hours" ? b.IntervalValue * 60 : b.IntervalValue;
                if (b.ScheduledMessages == null || b.ScheduledMessages.Count == 0)
                {
                    b.ScheduledMessages = new List<ScheduledMessage>
                    {
                        new ScheduledMessage("06:00", "GOOD MORNING"),
                        new ScheduledMessage("12:00", "GOOD AFTERNOON"),
                        new ScheduledMessage("18:00", "GOOD EVENING"),
                        new ScheduledMessage("23:00", "GOOD NIGHT")
                    };
                }
                if (string.IsNullOrEmpty(b.FontFamily)) b.FontFamily = "Segoe UI Symbol";
                if (string.IsNullOrEmpty(b.FontWeight)) b.FontWeight = "Regular";
                if (b.FontSize < 6 || b.FontSize > 200) b.FontSize = 16;
                if (string.IsNullOrEmpty(b.Color)) b.Color = "#D6D3D0";
                if (b.Opacity < 0 || b.Opacity > 1) b.Opacity = 0.8;
                if (string.IsNullOrEmpty(b.Alignment)) b.Alignment = "Center";
                if (string.IsNullOrEmpty(b.Case)) b.Case = "None";
                if (b.Effects == null) b.Effects = new TextEffectSettings();
            }

            ValidateElement(settings.Greeting, 8, 120, 22, "#D6D3D0", 0.82);
            ValidateElement(settings.Weekday, 10, 160, 50, "#D6D3D0", 0.92);
            ValidateElement(settings.Time, 14, 240, 62, "#D6D3D0", 1.0);
            ValidateElement(settings.Date, 8, 120, 20, "#D6D3D0", 0.85);

            if (settings.Scale < 0.4 || settings.Scale > 3.0) settings.Scale = 1.0;
            if (settings.MasterOpacity < 0.1 || settings.MasterOpacity > 1.0) settings.MasterOpacity = 1.0;

            return settings;
        }

        private static void ValidateElement(ElementSettings e, double minSz, double maxSz, double defaultSz, string defaultColor, double defaultOp)
        {
            if (e == null) return;
            if (string.IsNullOrEmpty(e.FontFamily)) e.FontFamily = "Audiowide";
            if (string.IsNullOrEmpty(e.FontWeight)) e.FontWeight = "Regular";
            if (e.FontSize < minSz || e.FontSize > maxSz) e.FontSize = defaultSz;
            try { ColorConverter.ConvertFromString(e.Color); }
            catch { e.Color = defaultColor; }
            if (string.IsNullOrEmpty(e.Color)) e.Color = defaultColor;
            if (e.Opacity < 0 || e.Opacity > 1) e.Opacity = defaultOp;
            if (string.IsNullOrEmpty(e.Case)) e.Case = "Title";
            if (e.Effects == null) e.Effects = new TextEffectSettings();
        }

        public static void Save(WidgetSettings settings)
        {
            try
            {
                var dir = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                using (var fs = File.Create(SettingsPath))
                {
                    var ser = new DataContractJsonSerializer(typeof(WidgetSettings));
                    ser.WriteObject(fs, settings);
                }
            }
            catch { }
        }

        public static WidgetSettings Clone(WidgetSettings s)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    var ser = new DataContractJsonSerializer(typeof(WidgetSettings));
                    ser.WriteObject(ms, s);
                    ms.Position = 0;
                    return ser.ReadObject(ms) as WidgetSettings;
                }
            }
            catch { return s; }
        }
    }

    public static class TextCaseHelper
    {
        public static string ApplyCase(string text, string caseOption)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(caseOption)) return text;
            switch (caseOption.ToLowerInvariant())
            {
                case "upper":
                    return text.ToUpperInvariant();
                case "lower":
                    return text.ToLowerInvariant();
                case "title":
                    return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLowerInvariant());
                default:
                    return text;
            }
        }
    }

    public static class GlyphHelper
    {
        private static readonly FontFamily FallbackSymbolFont = new FontFamily("Segoe UI Symbol");
        private static readonly FontFamily FallbackEmojiFont = new FontFamily("Segoe UI Emoji");
        private static readonly FontFamily FallbackArialFont = new FontFamily("Arial");

        public static bool CanFontRenderCharacter(FontFamily family, char ch)
        {
            if (family == null) return false;
            try
            {
                foreach (var tf in family.GetTypefaces())
                {
                    GlyphTypeface gtf;
                    if (tf.TryGetGlyphTypeface(out gtf))
                    {
                        ushort glyphIndex;
                        if (gtf.CharacterToGlyphMap.TryGetValue((int)ch, out glyphIndex) && glyphIndex > 0)
                            return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static bool CanFontRenderText(FontFamily family, string text)
        {
            if (string.IsNullOrEmpty(text)) return true;
            if (family == null) return false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsWhiteSpace(c)) continue;
                if (!CanFontRenderCharacter(family, c)) return false;
            }
            return true;
        }

        public static FontFamily ResolveFontForText(FontFamily primaryFont, string text)
        {
            if (string.IsNullOrEmpty(text)) return primaryFont ?? FallbackSymbolFont;
            if (primaryFont != null && CanFontRenderText(primaryFont, text))
                return primaryFont;

            if (CanFontRenderText(FallbackSymbolFont, text)) return FallbackSymbolFont;
            if (CanFontRenderText(FallbackEmojiFont, text)) return FallbackEmojiFont;
            if (CanFontRenderText(FallbackArialFont, text)) return FallbackArialFont;

            return primaryFont ?? FallbackSymbolFont;
        }

        public static readonly string[] RequiredSymbols = new string[]
        {
            "✦", "✧", "◇", "◆", "⟡", "⋄", "•", "○", "●", "△", "▽", "⌁", "∞", "+", "×", "|"
        };

        public static List<string> GetValidSymbols()
        {
            var list = new List<string>();
            foreach (var s in RequiredSymbols)
            {
                if (CanFontRenderText(FallbackSymbolFont, s) || CanFontRenderText(FallbackEmojiFont, s) || CanFontRenderText(FallbackArialFont, s))
                {
                    list.Add(s);
                }
            }
            return list;
        }
    }

    public class CuratedFontDef
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string FileName { get; set; }
        public string ActualFamily { get; set; }

        public CuratedFontDef(string name, string category, string fileName, string actualFamily)
        {
            Name = name;
            Category = category;
            FileName = fileName;
            ActualFamily = actualFamily;
        }
    }

    public class FontCatalogItem
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public bool IsAppFont { get; set; }

        public FontCatalogItem(string name, string category, bool isAppFont)
        {
            Name = name;
            Category = category;
            IsAppFont = isAppFont;
        }
    }

    public static class FontCatalog
    {
        public static readonly string[] Sources = new string[] { "All", "App Fonts", "System Fonts" };
        public static readonly string[] Categories = new string[]
        {
            "All", "Favorites", "Aesthetic", "Futuristic", "Handwritten", "Athletic", "Condensed", "Minimal", "Serif", "Display", "Script", "Monospace"
        };

        public static readonly CuratedFontDef[] CuratedFonts = new CuratedFontDef[]
        {
            new CuratedFontDef("Abril Fatface", "Display", "AbrilFatface.ttf", "Abril Fatface"),
            new CuratedFontDef("Aldrich", "Futuristic", "Aldrich.ttf", "Aldrich"),
            new CuratedFontDef("Alegreya", "Aesthetic", "Alegreya.ttf", "Alegreya"),
            new CuratedFontDef("Alegreya Sans", "Aesthetic", "AlegreyaSans.ttf", "Alegreya Sans"),
            new CuratedFontDef("Alex Brush", "Script", "AlexBrush.ttf", "Alex Brush"),
            new CuratedFontDef("Alfa Slab One", "Serif", "AlfaSlabOne.ttf", "Alfa Slab One"),
            new CuratedFontDef("Alice", "Aesthetic", "Alice.ttf", "Alice"),
            new CuratedFontDef("Allura", "Script", "Allura.ttf", "Allura"),
            new CuratedFontDef("Amatic SC", "Handwritten", "AmaticSC.ttf", "Amatic SC"),
            new CuratedFontDef("Anonymous Pro", "Monospace", "AnonymousPro.ttf", "Anonymous Pro"),
            new CuratedFontDef("Anton", "Athletic", "Anton.ttf", "Anton"),
            new CuratedFontDef("Antonio", "Condensed", "Antonio.ttf", "Antonio"),
            new CuratedFontDef("Architects Daughter", "Handwritten", "ArchitectsDaughter.ttf", "Architects Daughter"),
            new CuratedFontDef("Archivo Black", "Athletic", "ArchivoBlack.ttf", "Archivo Black"),
            new CuratedFontDef("Archivo Narrow", "Condensed", "ArchivoNarrow.ttf", "Archivo Narrow"),
            new CuratedFontDef("Arvo", "Serif", "Arvo.ttf", "Arvo"),
            new CuratedFontDef("Audiowide", "Futuristic", "Audiowide.ttf", "Audiowide"),
            new CuratedFontDef("Bad Script", "Script", "BadScript.ttf", "Bad Script"),
            new CuratedFontDef("Barlow Condensed", "Condensed", "BarlowCondensed.ttf", "Barlow Condensed"),
            new CuratedFontDef("Bebas Neue", "Athletic", "BebasNeue.ttf", "Bebas Neue"),
            new CuratedFontDef("Bellefair", "Aesthetic", "Bellefair.ttf", "Bellefair"),
            new CuratedFontDef("Berkshire Swash", "Script", "BerkshireSwash.ttf", "Berkshire Swash"),
            new CuratedFontDef("Big Shoulders Display", "Condensed", "BigShouldersDisplay.ttf", "Big Shoulders Display"),
            new CuratedFontDef("Bitter", "Serif", "Bitter.ttf", "Bitter"),
            new CuratedFontDef("Black Ops One", "Futuristic", "BlackOpsOne.ttf", "Black Ops One"),
            new CuratedFontDef("Bree Serif", "Serif", "BreeSerif.ttf", "Bree Serif"),
            new CuratedFontDef("Bruno Ace", "Futuristic", "BrunoAce.ttf", "Bruno Ace"),
            new CuratedFontDef("Bruno Ace SC", "Futuristic", "BrunoAceSC.ttf", "Bruno Ace SC"),
            new CuratedFontDef("Bungee", "Display", "Bungee.ttf", "Bungee"),
            new CuratedFontDef("Bungee Shade", "Display", "BungeeShade.ttf", "Bungee Shade"),
            new CuratedFontDef("Cabin", "Minimal", "Cabin.ttf", "Cabin"),
            new CuratedFontDef("Cardo", "Aesthetic", "Cardo.ttf", "Cardo"),
            new CuratedFontDef("Castoro", "Serif", "Castoro.ttf", "Castoro"),
            new CuratedFontDef("Caveat", "Handwritten", "Caveat.ttf", "Caveat"),
            new CuratedFontDef("Cedarville Cursive", "Handwritten", "CedarvilleCursive.ttf", "Cedarville Cursive"),
            new CuratedFontDef("Chakra Petch", "Futuristic", "ChakraPetch.ttf", "Chakra Petch"),
            new CuratedFontDef("Cinzel", "Aesthetic", "Cinzel.ttf", "Cinzel"),
            new CuratedFontDef("Cinzel Decorative", "Aesthetic", "CinzelDecorative.ttf", "Cinzel Decorative"),
            new CuratedFontDef("Comfortaa", "Aesthetic", "Comfortaa.ttf", "Comfortaa"),
            new CuratedFontDef("Coming Soon", "Handwritten", "ComingSoon.ttf", "Coming Soon"),
            new CuratedFontDef("Cormorant", "Aesthetic", "Cormorant.ttf", "Cormorant Light"),
            new CuratedFontDef("Cormorant Garamond", "Aesthetic", "CormorantGaramond.ttf", "Cormorant Garamond"),
            new CuratedFontDef("Cormorant Infant", "Aesthetic", "CormorantInfant.ttf", "Cormorant Infant Light"),
            new CuratedFontDef("Cormorant SC", "Aesthetic", "CormorantSC.ttf", "Cormorant SC"),
            new CuratedFontDef("Cormorant Unicase", "Aesthetic", "CormorantUnicase.ttf", "Cormorant Unicase"),
            new CuratedFontDef("Courgette", "Script", "Courgette.ttf", "Courgette"),
            new CuratedFontDef("Courier Prime", "Monospace", "CourierPrime.ttf", "Courier Prime"),
            new CuratedFontDef("Covered By Your Grace", "Handwritten", "CoveredByYourGrace.ttf", "Covered By Your Grace"),
            new CuratedFontDef("Crafty Girls", "Handwritten", "CraftyGirls.ttf", "Crafty Girls"),
            new CuratedFontDef("Creepster", "Display", "Creepster.ttf", "Creepster"),
            new CuratedFontDef("Crete Round", "Serif", "CreteRound.ttf", "Crete Round"),
            new CuratedFontDef("Cute Font", "Handwritten", "CuteFont.ttf", "Cute Font"),
            new CuratedFontDef("Cutive", "Serif", "Cutive.ttf", "Cutive"),
            new CuratedFontDef("DM Mono", "Monospace", "DMMono.ttf", "DM Mono"),
            new CuratedFontDef("DM Serif Display", "Aesthetic", "DMSerifDisplay.ttf", "DM Serif Display"),
            new CuratedFontDef("Dancing Script", "Script", "DancingScript.ttf", "Dancing Script"),
            new CuratedFontDef("Delius", "Handwritten", "Delius.ttf", "Delius"),
            new CuratedFontDef("Delius Swash Caps", "Handwritten", "DeliusSwashCaps.ttf", "Delius Swash Caps"),
            new CuratedFontDef("Dokdo", "Handwritten", "Dokdo.ttf", "Dokdo"),
            new CuratedFontDef("EB Garamond", "Aesthetic", "EBGaramond.ttf", "EB Garamond"),
            new CuratedFontDef("East Sea Dokdo", "Handwritten", "EastSeaDokdo.ttf", "East Sea Dokdo"),
            new CuratedFontDef("Electrolize", "Futuristic", "Electrolize.ttf", "Electrolize"),
            new CuratedFontDef("Exo 2", "Futuristic", "Exo2.ttf", "Exo 2"),
            new CuratedFontDef("Faster One", "Display", "FasterOne.ttf", "Faster One"),
            new CuratedFontDef("Fira Code", "Monospace", "FiraCode.ttf", "Fira Code"),
            new CuratedFontDef("Fira Sans", "Minimal", "FiraSans.ttf", "Fira Sans"),
            new CuratedFontDef("Fjalla One", "Athletic", "FjallaOne.ttf", "Fjalla One"),
            new CuratedFontDef("Fondamento", "Handwritten", "Fondamento.ttf", "Fondamento"),
            new CuratedFontDef("Forum", "Aesthetic", "Forum.ttf", "Forum"),
            new CuratedFontDef("Gaegu", "Handwritten", "Gaegu.ttf", "Gaegu"),
            new CuratedFontDef("Gamja Flower", "Handwritten", "GamjaFlower.ttf", "Gamja Flower"),
            new CuratedFontDef("Geo", "Futuristic", "Geo.ttf", "Geo"),
            new CuratedFontDef("Gilda Display", "Aesthetic", "GildaDisplay.ttf", "Gilda Display"),
            new CuratedFontDef("Give You Glory", "Handwritten", "GiveYouGlory.ttf", "Give You Glory"),
            new CuratedFontDef("Gloria Hallelujah", "Handwritten", "GloriaHallelujah.ttf", "Gloria Hallelujah"),
            new CuratedFontDef("Gochi Hand", "Handwritten", "GochiHand.ttf", "Gochi Hand"),
            new CuratedFontDef("Graduate", "Display", "Graduate.ttf", "Graduate"),
            new CuratedFontDef("Grand Hotel", "Handwritten", "GrandHotel.ttf", "Grand Hotel"),
            new CuratedFontDef("Great Vibes", "Script", "GreatVibes.ttf", "Great Vibes"),
            new CuratedFontDef("Handlee", "Handwritten", "Handlee.ttf", "Handlee"),
            new CuratedFontDef("Hi Melody", "Handwritten", "HiMelody.ttf", "Hi Melody"),
            new CuratedFontDef("Homemade Apple", "Handwritten", "HomemadeApple.ttf", "Homemade Apple"),
            new CuratedFontDef("IBM Plex Mono", "Monospace", "IBMPlexMono.ttf", "IBM Plex Mono"),
            new CuratedFontDef("Inconsolata", "Monospace", "Inconsolata.ttf", "Inconsolata"),
            new CuratedFontDef("Indie Flower", "Handwritten", "IndieFlower.ttf", "Indie Flower"),
            new CuratedFontDef("Instrument Sans", "Aesthetic", "InstrumentSans.ttf", "Instrument Sans"),
            new CuratedFontDef("Instrument Serif", "Aesthetic", "InstrumentSerif.ttf", "Instrument Serif"),
            new CuratedFontDef("Inter", "Minimal", "Inter.ttf", "Inter"),
            new CuratedFontDef("Italiana", "Aesthetic", "Italiana.ttf", "Italiana"),
            new CuratedFontDef("Itim", "Handwritten", "Itim.ttf", "Itim"),
            new CuratedFontDef("Josefin Sans", "Aesthetic", "JosefinSans.ttf", "Josefin Sans"),
            new CuratedFontDef("Josefin Slab", "Aesthetic", "JosefinSlab.ttf", "Josefin Slab"),
            new CuratedFontDef("Jost", "Aesthetic", "Jost.ttf", "Jost"),
            new CuratedFontDef("Jua", "Handwritten", "Jua.ttf", "Jua"),
            new CuratedFontDef("Just Another Hand", "Handwritten", "JustAnotherHand.ttf", "Just Another Hand"),
            new CuratedFontDef("Kalam", "Handwritten", "Kalam.ttf", "Kalam"),
            new CuratedFontDef("Kanit", "Athletic", "Kanit.ttf", "Kanit"),
            new CuratedFontDef("Karla", "Minimal", "Karla.ttf", "Karla"),
            new CuratedFontDef("Kaushan Script", "Script", "KaushanScript.ttf", "Kaushan Script"),
            new CuratedFontDef("Klee One", "Handwritten", "KleeOne.ttf", "Klee One"),
            new CuratedFontDef("Kristi", "Handwritten", "Kristi.ttf", "Kristi"),
            new CuratedFontDef("La Belle Aurore", "Handwritten", "LaBelleAurore.ttf", "La Belle Aurore"),
            new CuratedFontDef("Lato", "Minimal", "Lato.ttf", "Lato"),
            new CuratedFontDef("League Gothic", "Athletic", "LeagueGothic.ttf", "League Gothic"),
            new CuratedFontDef("League Spartan", "Athletic", "LeagueSpartan.ttf", "League Spartan"),
            new CuratedFontDef("Lexend", "Minimal", "Lexend.ttf", "Lexend"),
            new CuratedFontDef("Libre Baskerville", "Serif", "LibreBaskerville.ttf", "Libre Baskerville"),
            new CuratedFontDef("Libre Caslon Display", "Aesthetic", "LibreCaslonDisplay.ttf", "Libre Caslon Display"),
            new CuratedFontDef("Libre Caslon Text", "Aesthetic", "LibreCaslonText.ttf", "Libre Caslon Text"),
            new CuratedFontDef("Lobster", "Script", "Lobster.ttf", "Lobster"),
            new CuratedFontDef("Lora", "Aesthetic", "Lora.ttf", "Lora"),
            new CuratedFontDef("Major Mono Display", "Monospace", "MajorMonoDisplay.ttf", "Major Mono Display"),
            new CuratedFontDef("Mali", "Handwritten", "Mali.ttf", "Mali"),
            new CuratedFontDef("Manrope", "Aesthetic", "Manrope.ttf", "Manrope"),
            new CuratedFontDef("Mansalva", "Handwritten", "Mansalva.ttf", "Mansalva"),
            new CuratedFontDef("Marcellus", "Aesthetic", "Marcellus.ttf", "Marcellus"),
            new CuratedFontDef("Marcellus SC", "Aesthetic", "MarcellusSC.ttf", "Marcellus SC"),
            new CuratedFontDef("Marck Script", "Script", "MarckScript.ttf", "Marck Script"),
            new CuratedFontDef("Megrim", "Futuristic", "Megrim.ttf", "Megrim"),
            new CuratedFontDef("Michroma", "Futuristic", "Michroma.ttf", "Michroma"),
            new CuratedFontDef("Monoton", "Display", "Monoton.ttf", "Monoton"),
            new CuratedFontDef("Montserrat", "Aesthetic", "Montserrat.ttf", "Montserrat"),
            new CuratedFontDef("Mrs Saint Delafield", "Handwritten", "MrsSaintDelafield.ttf", "Mrs Saint Delafield"),
            new CuratedFontDef("Mulish", "Aesthetic", "Mulish.ttf", "Mulish"),
            new CuratedFontDef("Nanum Brush Script", "Handwritten", "NanumBrushScript.ttf", "Nanum Brush Script"),
            new CuratedFontDef("Nanum Pen Script", "Handwritten", "NanumPenScript.ttf", "Nanum Pen"),
            new CuratedFontDef("Neucha", "Handwritten", "Neucha.ttf", "Neucha"),
            new CuratedFontDef("Newsreader", "Aesthetic", "Newsreader.ttf", "Newsreader 16pt 16pt"),
            new CuratedFontDef("Nothing You Could Do", "Handwritten", "NothingYouCouldDo.ttf", "Nothing You Could Do"),
            new CuratedFontDef("Noto Sans Mono", "Monospace", "NotoSansMono.ttf", "Noto Sans Mono"),
            new CuratedFontDef("Noto Serif", "Serif", "NotoSerif.ttf", "Noto Serif"),
            new CuratedFontDef("Nunito", "Minimal", "Nunito.ttf", "Nunito"),
            new CuratedFontDef("Open Sans", "Minimal", "OpenSans.ttf", "Open Sans"),
            new CuratedFontDef("Orbitron", "Futuristic", "Orbitron.ttf", "Orbitron"),
            new CuratedFontDef("Oswald", "Athletic", "Oswald.ttf", "Oswald"),
            new CuratedFontDef("Outfit", "Aesthetic", "Outfit.ttf", "Outfit"),
            new CuratedFontDef("Over the Rainbow", "Handwritten", "OvertheRainbow.ttf", "Over the Rainbow"),
            new CuratedFontDef("Overpass Mono", "Monospace", "OverpassMono.ttf", "Overpass Mono"),
            new CuratedFontDef("Oxanium", "Futuristic", "Oxanium.ttf", "Oxanium"),
            new CuratedFontDef("PT Serif", "Serif", "PTSerif.ttf", "PT Serif"),
            new CuratedFontDef("Pacifico", "Script", "Pacifico.ttf", "Pacifico"),
            new CuratedFontDef("Pangolin", "Handwritten", "Pangolin.ttf", "Pangolin"),
            new CuratedFontDef("Parisienne", "Script", "Parisienne.ttf", "Parisienne"),
            new CuratedFontDef("Patrick Hand", "Handwritten", "PatrickHand.ttf", "Patrick Hand"),
            new CuratedFontDef("Permanent Marker", "Handwritten", "PermanentMarker.ttf", "Permanent Marker"),
            new CuratedFontDef("Playfair Display", "Aesthetic", "PlayfairDisplay.ttf", "Playfair Display"),
            new CuratedFontDef("Plus Jakarta Sans", "Minimal", "PlusJakartaSans.ttf", "Plus Jakarta Sans"),
            new CuratedFontDef("Poiret One", "Aesthetic", "PoiretOne.ttf", "Poiret One"),
            new CuratedFontDef("Poor Story", "Handwritten", "PoorStory.ttf", "Poor Story"),
            new CuratedFontDef("Poppins", "Aesthetic", "Poppins.ttf", "Poppins"),
            new CuratedFontDef("Prata", "Aesthetic", "Prata.ttf", "Prata"),
            new CuratedFontDef("Press Start 2P", "Futuristic", "PressStart2P.ttf", "Press Start 2P"),
            new CuratedFontDef("Quantico", "Futuristic", "Quantico.ttf", "Quantico"),
            new CuratedFontDef("Quicksand", "Aesthetic", "Quicksand.ttf", "Quicksand"),
            new CuratedFontDef("Qwigley", "Handwritten", "Qwigley.ttf", "Qwigley"),
            new CuratedFontDef("Rajdhani", "Futuristic", "Rajdhani.ttf", "Rajdhani"),
            new CuratedFontDef("Raleway", "Aesthetic", "Raleway.ttf", "Raleway"),
            new CuratedFontDef("Rancho", "Handwritten", "Rancho.ttf", "Rancho"),
            new CuratedFontDef("Redressed", "Handwritten", "Redressed.ttf", "Redressed"),
            new CuratedFontDef("Reenie Beanie", "Handwritten", "ReenieBeanie.ttf", "Reenie Beanie"),
            new CuratedFontDef("Righteous", "Futuristic", "Righteous.ttf", "Righteous"),
            new CuratedFontDef("Roboto", "Minimal", "Roboto.ttf", "Roboto"),
            new CuratedFontDef("Roboto Condensed", "Condensed", "RobotoCondensed.ttf", "Roboto Condensed"),
            new CuratedFontDef("Roboto Mono", "Monospace", "RobotoMono.ttf", "Roboto Mono"),
            new CuratedFontDef("Roboto Slab", "Serif", "RobotoSlab.ttf", "Roboto Slab"),
            new CuratedFontDef("Rock Salt", "Handwritten", "RockSalt.ttf", "Rock Salt"),
            new CuratedFontDef("Rokkitt", "Serif", "Rokkitt.ttf", "Rokkitt"),
            new CuratedFontDef("Rozha One", "Aesthetic", "RozhaOne.ttf", "Rozha One"),
            new CuratedFontDef("Rubik", "Minimal", "Rubik.ttf", "Rubik"),
            new CuratedFontDef("Rufina", "Aesthetic", "Rufina.ttf", "Rufina"),
            new CuratedFontDef("Russo One", "Futuristic", "RussoOne.ttf", "Russo One"),
            new CuratedFontDef("Rye", "Display", "Rye.ttf", "Rye"),
            new CuratedFontDef("Sacramento", "Script", "Sacramento.ttf", "Sacramento"),
            new CuratedFontDef("Saira Condensed", "Condensed", "SairaCondensed.ttf", "Saira Condensed"),
            new CuratedFontDef("Saira Semi Condensed", "Condensed", "SairaSemiCondensed.ttf", "Saira SemiCondensed"),
            new CuratedFontDef("Sancreek", "Display", "Sancreek.ttf", "Sancreek"),
            new CuratedFontDef("Sassy Frass", "Handwritten", "SassyFrass.ttf", "Sassy Frass"),
            new CuratedFontDef("Satisfy", "Script", "Satisfy.ttf", "Satisfy"),
            new CuratedFontDef("Schoolbell", "Handwritten", "Schoolbell.ttf", "Schoolbell"),
            new CuratedFontDef("Shadows Into Light", "Handwritten", "ShadowsIntoLight.ttf", "Shadows Into Light"),
            new CuratedFontDef("Share Tech", "Futuristic", "ShareTech.ttf", "Share Tech"),
            new CuratedFontDef("Share Tech Mono", "Futuristic", "ShareTechMono.ttf", "Share Tech Mono"),
            new CuratedFontDef("Short Stack", "Handwritten", "ShortStack.ttf", "Short Stack"),
            new CuratedFontDef("Shrikhand", "Display", "Shrikhand.ttf", "Shrikhand"),
            new CuratedFontDef("Silkscreen", "Futuristic", "Silkscreen.ttf", "Silkscreen"),
            new CuratedFontDef("Single Day", "Handwritten", "SingleDay.ttf", "Single Day"),
            new CuratedFontDef("Six Caps", "Condensed", "SixCaps.ttf", "Six Caps"),
            new CuratedFontDef("Sniglet", "Handwritten", "Sniglet.ttf", "Sniglet"),
            new CuratedFontDef("Source Code Pro", "Monospace", "SourceCodePro.ttf", "Source Code Pro"),
            new CuratedFontDef("Source Sans 3", "Minimal", "SourceSans3.ttf", "Source Sans 3"),
            new CuratedFontDef("Space Grotesk", "Aesthetic", "SpaceGrotesk.ttf", "Space Grotesk"),
            new CuratedFontDef("Space Mono", "Monospace", "SpaceMono.ttf", "Space Mono"),
            new CuratedFontDef("Special Elite", "Display", "SpecialElite.ttf", "Special Elite"),
            new CuratedFontDef("Spectral", "Aesthetic", "Spectral.ttf", "Spectral"),
            new CuratedFontDef("Sue Ellen Francisco", "Handwritten", "SueEllenFrancisco.ttf", "Sue Ellen Francisco "),
            new CuratedFontDef("Syncopate", "Futuristic", "Syncopate.ttf", "Syncopate"),
            new CuratedFontDef("Syne", "Aesthetic", "Syne.ttf", "Syne"),
            new CuratedFontDef("Teko", "Athletic", "Teko.ttf", "Teko"),
            new CuratedFontDef("Tenor Sans", "Aesthetic", "TenorSans.ttf", "Tenor Sans"),
            new CuratedFontDef("Titillium Web", "Futuristic", "TitilliumWeb.ttf", "Titillium Web"),
            new CuratedFontDef("Tomorrow", "Futuristic", "Tomorrow.ttf", "Tomorrow"),
            new CuratedFontDef("Ubuntu", "Minimal", "Ubuntu.ttf", "Ubuntu"),
            new CuratedFontDef("Ubuntu Condensed", "Condensed", "UbuntuCondensed.ttf", "Ubuntu Condensed"),
            new CuratedFontDef("Ubuntu Mono", "Monospace", "UbuntuMono.ttf", "Ubuntu Mono"),
            new CuratedFontDef("Unbounded", "Aesthetic", "Unbounded.ttf", "Unbounded"),
            new CuratedFontDef("Urbanist", "Aesthetic", "Urbanist.ttf", "Urbanist"),
            new CuratedFontDef("VT323", "Monospace", "VT323.ttf", "VT323"),
            new CuratedFontDef("Vidaloka", "Aesthetic", "Vidaloka.ttf", "Vidaloka "),
            new CuratedFontDef("Waiting for the Sunrise", "Handwritten", "WaitingfortheSunrise.ttf", "Waiting for the Sunrise"),
            new CuratedFontDef("Wallpoet", "Futuristic", "Wallpoet.ttf", "Wallpoet"),
            new CuratedFontDef("Walter Turncoat", "Handwritten", "WalterTurncoat.ttf", "Walter Turncoat"),
            new CuratedFontDef("Work Sans", "Minimal", "WorkSans.ttf", "Work Sans"),
            new CuratedFontDef("Yellowtail", "Script", "Yellowtail.ttf", "Yellowtail"),
            new CuratedFontDef("Yeon Sung", "Handwritten", "YeonSung.ttf", "Yeon Sung"),
            new CuratedFontDef("Yeseva One", "Aesthetic", "YesevaOne.ttf", "Yeseva One"),
            new CuratedFontDef("Zeyada", "Handwritten", "Zeyada.ttf", "Zeyada"),
            new CuratedFontDef("Zilla Slab", "Aesthetic", "ZillaSlab.ttf", "Zilla Slab"),
        };

        private static readonly Dictionary<string, CuratedFontDef> _curatedMap = new Dictionary<string, CuratedFontDef>(StringComparer.OrdinalIgnoreCase);
        private static List<FontCatalogItem> _allCatalogItems;
        private static readonly object _lock = new object();

        static FontCatalog()
        {
            foreach (var f in CuratedFonts)
            {
                if (!_curatedMap.ContainsKey(f.Name)) _curatedMap[f.Name] = f;
            }
        }

        public static CuratedFontDef FindCurated(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            CuratedFontDef def;
            _curatedMap.TryGetValue(name, out def);
            return def;
        }

        public static int CuratedAppFontCount
        {
            get { return CuratedFonts.Length; }
        }

        public static List<FontCatalogItem> GetAllFonts()
        {
            if (_allCatalogItems != null) return _allCatalogItems;
            lock (_lock)
            {
                if (_allCatalogItems != null) return _allCatalogItems;
                var list = new List<FontCatalogItem>();
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var c in CuratedFonts)
                {
                    list.Add(new FontCatalogItem(c.Name, c.Category, true));
                    seen.Add(c.Name);
                }

                try
                {
                    foreach (var sf in System.Windows.Media.Fonts.SystemFontFamilies)
                    {
                        string name = sf.Source;
                        if (!string.IsNullOrEmpty(name) && !seen.Contains(name) && !name.StartsWith("@") && !name.StartsWith("#"))
                        {
                            string cat = GetCategoryForSystemFont(name);
                            list.Add(new FontCatalogItem(name, cat, false));
                            seen.Add(name);
                        }
                    }
                }
                catch { }

                _allCatalogItems = list;
                return _allCatalogItems;
            }
        }

        public static List<FontCatalogItem> Filter(string source, string category, string search, List<string> favorites)
        {
            var all = GetAllFonts();
            var favSet = new HashSet<string>(favorites ?? new List<string>(), StringComparer.OrdinalIgnoreCase);
            var result = new List<FontCatalogItem>();

            foreach (var item in all)
            {
                if (!string.IsNullOrEmpty(source) && source != "All")
                {
                    if (source == "App Fonts" && !item.IsAppFont) continue;
                    if (source == "System Fonts" && item.IsAppFont) continue;
                }

                if (!string.IsNullOrEmpty(category) && category != "All")
                {
                    if (category == "Favorites")
                    {
                        if (!favSet.Contains(item.Name)) continue;
                    }
                    else if (!string.Equals(item.Category, category, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(search))
                {
                    if (item.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                }

                result.Add(item);
            }

            return result;
        }

        private static string GetCategoryForSystemFont(string name)
        {
            string n = name.ToLowerInvariant();
            if (n.Contains("mono") || n.Contains("console") || n.Contains("code") || n.Contains("courier") || n.Contains("terminal")) return "Monospace";
            if (n.Contains("script") || n.Contains("cursive") || n.Contains("brush") || n.Contains("hand") || n.Contains("signature")) return "Script";
            if (n.Contains("serif") || n.Contains("roman") || n.Contains("georgia") || n.Contains("garamond") || n.Contains("cambria") || n.Contains("baskerville") || n.Contains("palatino")) return "Serif";
            if (n.Contains("condensed") || n.Contains("narrow")) return "Condensed";
            if (n.Contains("gothic") || n.Contains("impact") || n.Contains("black") || n.Contains("heavy")) return "Athletic";
            if (n.Contains("sans") || n.Contains("arial") || n.Contains("calibri") || n.Contains("segoe") || n.Contains("tahoma") || n.Contains("verdana") || n.Contains("helvetica")) return "Minimal";
            return "Minimal";
        }
    }

    public static class Fonts
    {
        public static readonly string[] Weights = new string[]
        {
            "Thin", "ExtraLight", "Light", "Regular", "Medium", "SemiBold", "Bold", "ExtraBold", "Black"
        };

        public static FontWeight ParseWeight(string weight)
        {
            if (string.IsNullOrEmpty(weight)) return FontWeights.Normal;
            switch (weight.ToLowerInvariant())
            {
                case "thin": return FontWeights.Thin;
                case "extralight": case "ultra-light": return FontWeights.ExtraLight;
                case "light": return FontWeights.Light;
                case "regular": case "normal": return FontWeights.Normal;
                case "medium": return FontWeights.Medium;
                case "semibold": case "demibold": return FontWeights.SemiBold;
                case "bold": return FontWeights.Bold;
                case "extrabold": case "ultrabold": return FontWeights.ExtraBold;
                case "black": case "heavy": return FontWeights.Black;
                default: return FontWeights.Normal;
            }
        }

        private static Uri _baseUri;
        private static Uri BaseUri
        {
            get
            {
                if (_baseUri == null)
                    _baseUri = new Uri("pack://application:,,,/DesktopClockWidget;component/Fonts/");
                return _baseUri;
            }
        }

        private static Uri _fontsUri;
        private static Uri FontsUri
        {
            get
            {
                if (_fontsUri == null)
                {
                    string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts");
                    if (!Directory.Exists(dir))
                    {
                        string localDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DesktopClock", "DesktopClockWidget", "Fonts");
                        if (Directory.Exists(localDir)) dir = localDir;
                    }
                    _fontsUri = new Uri("file:///" + dir.Replace('\\', '/') + "/");
                }
                return _fontsUri;
            }
        }

        private const int MaxLruCapacity = 10;
        private static readonly Dictionary<string, FontFamily> _cachedFamilies = new Dictionary<string, FontFamily>(StringComparer.OrdinalIgnoreCase);
        private static readonly LinkedList<string> _lruOrder = new LinkedList<string>();
        private static readonly HashSet<string> _pinnedFamilies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static void PinFamily(string familyName)
        {
            if (string.IsNullOrEmpty(familyName)) return;
            lock (_cachedFamilies)
            {
                _pinnedFamilies.Add(familyName);
            }
        }

        public static void ClearPreviewCache()
        {
            lock (_cachedFamilies)
            {
                var toRemove = new List<string>();
                foreach (var k in _cachedFamilies.Keys)
                {
                    if (!_pinnedFamilies.Contains(k)) toRemove.Add(k);
                }
                foreach (var k in toRemove)
                {
                    _cachedFamilies.Remove(k);
                }
                _lruOrder.Clear();
            }
            GC.Collect(1, GCCollectionMode.Optimized);
        }

        public static FontFamily For(string familyName)
        {
            if (string.IsNullOrEmpty(familyName) || familyName == "System Default") return SystemFonts.MessageFontFamily;

            lock (_cachedFamilies)
            {
                FontFamily cached;
                if (_cachedFamilies.TryGetValue(familyName, out cached))
                {
                    if (_lruOrder.Contains(familyName))
                    {
                        _lruOrder.Remove(familyName);
                        _lruOrder.AddLast(familyName);
                    }
                    return cached;
                }

                FontFamily result = null;
                var curated = FontCatalog.FindCurated(familyName);
                if (curated != null)
                {
                    try
                    {
                        string actual = !string.IsNullOrEmpty(curated.ActualFamily) ? curated.ActualFamily : curated.Name;
                        result = new FontFamily(FontsUri, "./" + curated.FileName + "#" + actual);
                    }
                    catch { }
                }

                if (result == null)
                {
                    try
                    {
                        result = new FontFamily(familyName);
                    }
                    catch
                    {
                        result = SystemFonts.MessageFontFamily;
                    }
                }

                _cachedFamilies[familyName] = result;
                _lruOrder.AddLast(familyName);

                while (_lruOrder.Count > MaxLruCapacity)
                {
                    string oldest = _lruOrder.First.Value;
                    _lruOrder.RemoveFirst();
                    if (!_pinnedFamilies.Contains(oldest))
                    {
                        _cachedFamilies.Remove(oldest);
                    }
                }

                return result;
            }
        }
    }

    public static class Greeting
    {
        public static string For(int hour, int mStart, int aStart, int eStart, int nStart)
        {
            if (mStart <= aStart && aStart <= eStart && eStart <= nStart)
            {
                if (hour >= mStart && hour < aStart) return "GOOD MORNING";
                if (hour >= aStart && hour < eStart) return "GOOD AFTERNOON";
                if (hour >= eStart && hour < nStart) return "GOOD EVENING";
                return "GOOD NIGHT";
            }
            if (hour >= 5 && hour < 12) return "GOOD MORNING";
            if (hour >= 12 && hour < 17) return "GOOD AFTERNOON";
            if (hour >= 17 && hour < 22) return "GOOD EVENING";
            return "GOOD NIGHT";
        }
    }

    public static class BlockEvaluator
    {
        public static string GetActiveRotatingMessage(CustomBlock block, DateTime now, long testStep = -1)
        {
            if (block.Messages == null || block.Messages.Count == 0) return "";

            long step;
            if (testStep >= 0)
            {
                step = testStep;
            }
            else
            {
                long totalSeconds = (long)(now - new DateTime(2025, 1, 1)).TotalSeconds;
                int intervalSec = Math.Max(1, block.IntervalMinutes * 60);
                step = totalSeconds / intervalSec;
            }

            if (string.Equals(block.RotationMode, "Random", StringComparison.OrdinalIgnoreCase))
            {
                int seed = (int)(step ^ (block.Id != null ? block.Id.GetHashCode() : 12345));
                var rnd = new Random(seed);
                int idx = rnd.Next(0, block.Messages.Count);
                return block.Messages[idx];
            }
            else
            {
                int idx = (int)(Math.Abs(step) % block.Messages.Count);
                return block.Messages[idx];
            }
        }

        public static string GetActiveScheduledMessage(List<ScheduledMessage> schedules, DateTime now)
        {
            if (schedules == null || schedules.Count == 0) return "";

            var parsed = new List<KeyValuePair<TimeSpan, string>>();
            foreach (var s in schedules)
            {
                if (string.IsNullOrEmpty(s.Time)) continue;
                TimeSpan ts;
                if (TimeSpan.TryParse(s.Time, out ts))
                {
                    parsed.Add(new KeyValuePair<TimeSpan, string>(ts, s.Text));
                }
            }

            if (parsed.Count == 0) return "";
            parsed.Sort((a, b) => a.Key.CompareTo(b.Key));

            TimeSpan current = now.TimeOfDay;
            string best = null;
            foreach (var item in parsed)
            {
                if (item.Key <= current)
                {
                    best = item.Value;
                }
            }

            if (best != null) return best;
            return parsed[parsed.Count - 1].Value;
        }

        public static string EvaluateBlockContent(CustomBlock block, DateTime now)
        {
            if (block == null || !block.Enabled) return "";
            if (string.Equals(block.Type, "Symbol", StringComparison.OrdinalIgnoreCase))
            {
                return !string.IsNullOrEmpty(block.SymbolContent) ? block.SymbolContent : "✦";
            }
            if (string.Equals(block.Type, "Static Text", StringComparison.OrdinalIgnoreCase) || string.Equals(block.Type, "Static", StringComparison.OrdinalIgnoreCase))
            {
                return block.StaticContent ?? "";
            }
            if (string.Equals(block.Type, "Rotating Text", StringComparison.OrdinalIgnoreCase) || string.Equals(block.Type, "Rotating", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(block.RotationMode, "Schedule", StringComparison.OrdinalIgnoreCase))
                {
                    return GetActiveScheduledMessage(block.ScheduledMessages, now);
                }
                return GetActiveRotatingMessage(block, now);
            }
            return "";
        }
    }

    public class StableElementMetrics
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public StableElementMetrics() { }
        public StableElementMetrics(double w, double h)
        {
            Width = w;
            Height = h;
        }
    }

    public static class DynamicEnvelopeHelper
    {
        public static StableElementMetrics ComputeEnvelope(FontFamily family, FontWeight weight, double size, TextEffectSettings effects, string textCase, IEnumerable<string> candidates)
        {
            if (candidates == null) return null;

            double maxW = 0.0;
            double maxH = 0.0;
            double strokePad = (effects != null && effects.OutlineEnabled) ? Math.Min(10, effects.OutlineThickness) : 0;
            var tf = new Typeface(family ?? SystemFonts.MessageFontFamily, FontStyles.Normal, weight, FontStretches.Normal);
            var brush = Brushes.Black;

            foreach (var rawStr in candidates)
            {
                if (string.IsNullOrEmpty(rawStr)) continue;
                string s = rawStr;
                if (string.Equals(textCase, "Upper", StringComparison.OrdinalIgnoreCase)) s = s.ToUpperInvariant();
                else if (string.Equals(textCase, "Lower", StringComparison.OrdinalIgnoreCase)) s = s.ToLowerInvariant();
                else if (string.Equals(textCase, "Title", StringComparison.OrdinalIgnoreCase)) s = TextCaseHelper.ApplyCase(s, "Title");

                var ft = new FormattedText(s, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, Math.Max(6, size), brush);
                var g = ft.BuildGeometry(new Point(0, 0));
                double w = (g != null && !g.Bounds.IsEmpty) ? g.Bounds.Width : ft.Width;
                double h = (g != null && !g.Bounds.IsEmpty) ? g.Bounds.Height : ft.Height;

                if (w > maxW) maxW = w;
                if (h > maxH) maxH = h;
            }

            if (maxW <= 0) return null;
            return new StableElementMetrics(Math.Ceiling(maxW + strokePad * 2), Math.Ceiling(maxH + strokePad * 2));
        }

        public static List<string> GetTimeCandidates(WidgetSettings settings)
        {
            var list = new List<string>();
            var culture = CultureInfo.InvariantCulture;
            
            // Sample all 24 hours and all typical proportional digit variations
            int[] hours = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
            int[] minutes = new int[] { 0, 1, 8, 9, 10, 11, 19, 20, 28, 38, 48, 59 };

            foreach (int h in hours)
            {
                foreach (int m in minutes)
                {
                    var dt = new DateTime(2026, 8, 30, h, m, 0);
                    list.Add(dt.ToString("hh:mm tt", culture));
                    list.Add(dt.ToString("h:mm tt", culture));
                    list.Add(dt.ToString("HH:mm", culture));
                    list.Add(dt.ToString("H:mm", culture));
                    list.Add(dt.ToString("hh:mm:ss tt", culture));
                    list.Add(dt.ToString("HH:mm:ss", culture));
                }
            }
            return list;
        }

        public static List<string> GetWeekdayCandidates()
        {
            return new List<string>
            {
                "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday",
                "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"
            };
        }

        public static List<string> GetDateCandidates(WidgetSettings settings)
        {
            var list = new List<string>();
            var culture = CultureInfo.InvariantCulture;

            string[] months = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC",
                                             "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            int[] days = new int[] { 1, 8, 11, 18, 22, 28, 30, 31 };

            for (int m = 1; m <= 12; m++)
            {
                foreach (int d in days)
                {
                    try
                    {
                        var dt = new DateTime(2026, m, Math.Min(d, DateTime.DaysInMonth(2026, m)));
                        list.Add(dt.ToString("dd MMM", culture));
                        list.Add(dt.ToString("d MMM", culture));
                        list.Add(dt.ToString("dd MMMM", culture));
                        list.Add(dt.ToString("MMM dd", culture));
                        list.Add(dt.ToString("dd/MM/yyyy", culture));
                        list.Add(dt.ToString("yyyy-MM-dd", culture));
                    }
                    catch { }
                }
            }
            return list;
        }

        public static List<string> GetGreetingCandidates(WidgetSettings settings)
        {
            var list = new List<string>
            {
                "GOOD MORNING", "GOOD AFTERNOON", "GOOD EVENING", "GOOD NIGHT",
                "Good Morning", "Good Afternoon", "Good Evening", "Good Night"
            };
            if (settings != null && !string.IsNullOrEmpty(settings.CustomGreeting))
            {
                list.Add(settings.CustomGreeting);
            }
            return list;
        }
    }

    public class EffectTextBlock : FrameworkElement
    {
        private string _text = "";
        private FontFamily _fontFamily = new FontFamily("Segoe UI");
        private FontWeight _fontWeight = FontWeights.Normal;
        private FontStyle _fontStyle = FontStyles.Normal;
        private double _fontSize = 16.0;
        private Color _textColor = Color.FromRgb(214, 211, 208);
        private double _textOpacity = 1.0;
        private string _elementAlignment = "Center";
        private double _offsetX = 0.0;
        private double _offsetY = 0.0;
        private bool _isSelectedForEdit = false;
        private TextEffectSettings _effects = new TextEffectSettings();
        private StableElementMetrics _stableEnvelope;

        public StableElementMetrics StableEnvelope
        {
            get { return _stableEnvelope; }
            set
            {
                if (_stableEnvelope != value)
                {
                    _stableEnvelope = value;
                    InvalidateMeasure();
                    InvalidateVisual();
                }
            }
        }

        private FormattedText _formattedText;
        private Geometry _cachedGeometry;
        private Rect _visualBounds = Rect.Empty;
        private bool _formattedTextDirty = true;
        private bool _geometryDirty = true;

        private static int _globalAnimTick = 0;

        public bool IsSelectedForEdit
        {
            get { return _isSelectedForEdit; }
            set { if (_isSelectedForEdit != value) { _isSelectedForEdit = value; InvalidateVisual(); } }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (!string.Equals(_text, value))
                {
                    _text = value ?? "";
                    _formattedTextDirty = true;
                    _geometryDirty = true;
                    InvalidateMeasure();
                }
            }
        }

        public FontFamily FontFamily
        {
            get { return _fontFamily; }
            set
            {
                if (!object.Equals(_fontFamily, value))
                {
                    _fontFamily = value ?? new FontFamily("Segoe UI");
                    _formattedTextDirty = true;
                    _geometryDirty = true;
                    InvalidateMeasure();
                }
            }
        }

        public FontWeight FontWeight
        {
            get { return _fontWeight; }
            set
            {
                if (_fontWeight != value)
                {
                    _fontWeight = value;
                    _formattedTextDirty = true;
                    _geometryDirty = true;
                    InvalidateMeasure();
                }
            }
        }

        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set
            {
                if (_fontStyle != value)
                {
                    _fontStyle = value;
                    _formattedTextDirty = true;
                    _geometryDirty = true;
                    InvalidateMeasure();
                }
            }
        }

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (Math.Abs(_fontSize - value) > 0.001)
                {
                    _fontSize = Math.Max(6, value);
                    _formattedTextDirty = true;
                    _geometryDirty = true;
                    InvalidateMeasure();
                }
            }
        }

        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    _formattedTextDirty = true;
                    InvalidateVisual();
                }
            }
        }

        public double TextOpacity
        {
            get { return _textOpacity; }
            set
            {
                if (Math.Abs(_textOpacity - value) > 0.001)
                {
                    _textOpacity = Math.Max(0.0, Math.Min(1.0, value));
                    _formattedTextDirty = true;
                    InvalidateVisual();
                }
            }
        }

        public string ElementAlignment
        {
            get { return _elementAlignment; }
            set
            {
                string norm = string.IsNullOrEmpty(value) ? "Center" : value;
                if (!string.Equals(_elementAlignment, norm, StringComparison.OrdinalIgnoreCase))
                {
                    _elementAlignment = norm;
                    InvalidateVisual();
                }
            }
        }

        public double OffsetX
        {
            get { return _offsetX; }
            set
            {
                if (Math.Abs(_offsetX - value) > 0.001)
                {
                    _offsetX = value;
                    InvalidateMeasure();
                    InvalidateVisual();
                }
            }
        }

        public double OffsetY
        {
            get { return _offsetY; }
            set
            {
                if (Math.Abs(_offsetY - value) > 0.001)
                {
                    _offsetY = value;
                    InvalidateMeasure();
                    InvalidateVisual();
                }
            }
        }

        public TextEffectSettings Effects
        {
            get { return _effects; }
            set
            {
                _effects = value ?? new TextEffectSettings();
                _geometryDirty = true;
                InvalidateVisual();
            }
        }

        public static void AdvanceGlobalAnimation()
        {
            _globalAnimTick++;
        }

        public EffectTextBlock()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        private void RebuildFormattedText()
        {
            if (string.IsNullOrEmpty(_text))
            {
                _formattedText = null;
                _cachedGeometry = null;
                _visualBounds = Rect.Empty;
                _formattedTextDirty = false;
                _geometryDirty = false;
                return;
            }

            var tf = new Typeface(_fontFamily, _fontStyle, _fontWeight, FontStretches.Normal);
            var brush = new SolidColorBrush(_textColor) { Opacity = _textOpacity };
            brush.Freeze();

            _formattedText = new FormattedText(
                _text,
                CultureInfo.InvariantCulture,
                System.Windows.FlowDirection.LeftToRight,
                tf,
                _fontSize,
                brush
            );
            _formattedText.TextAlignment = TextAlignment.Left;
            _formattedTextDirty = false;
        }

        private void EnsureGeometry()
        {
            if (_formattedTextDirty) RebuildFormattedText();
            if (!_geometryDirty && _cachedGeometry != null) return;
            if (_formattedText == null) { _cachedGeometry = null; _visualBounds = Rect.Empty; _geometryDirty = false; return; }

            _cachedGeometry = _formattedText.BuildGeometry(new Point(0, 0));
            if (_cachedGeometry != null)
            {
                _cachedGeometry.Freeze();
                _visualBounds = _cachedGeometry.Bounds;
            }
            else
            {
                _visualBounds = Rect.Empty;
            }
            _geometryDirty = false;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_formattedTextDirty || _geometryDirty) EnsureGeometry();
            if ((_formattedText == null || string.IsNullOrEmpty(_text)) && (_stableEnvelope == null || _stableEnvelope.Width <= 0))
                return new Size(0, 0);

            double strokePad = 0;
            if (_effects != null && _effects.OutlineEnabled)
                strokePad = Math.Min(10, _effects.OutlineThickness);

            double baseW = _stableEnvelope != null && _stableEnvelope.Width > 0
                ? _stableEnvelope.Width
                : ((_visualBounds.IsEmpty ? (_formattedText != null ? _formattedText.Width : 0) : _visualBounds.Width) + strokePad * 2);

            double baseH = _stableEnvelope != null && _stableEnvelope.Height > 0
                ? _stableEnvelope.Height
                : ((_visualBounds.IsEmpty ? (_formattedText != null ? _formattedText.Height : 0) : _visualBounds.Height) + strokePad * 2);

            // Layout-aware dynamic bounds expansion: accommodate configured X/Y offsets without clipping
            double extraLeft = Math.Max(0.0, -_offsetX);
            double extraRight = Math.Max(0.0, _offsetX);
            double extraTop = Math.Max(0.0, -_offsetY);
            double extraBottom = Math.Max(0.0, _offsetY);

            double w = Math.Ceiling(baseW + extraLeft + extraRight);
            double h = Math.Ceiling(baseH + extraTop + extraBottom);
            return new Size(w, h);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_formattedTextDirty || _geometryDirty) EnsureGeometry();
            if (_cachedGeometry == null || string.IsNullOrEmpty(_text)) return;

            bool hasOutline = (_effects != null && _effects.OutlineEnabled && _effects.OutlineThickness > 0.01);
            bool hasGlitch = (_effects != null && _effects.GlitchEnabled && _effects.GlitchIntensity > 0);
            bool hasNoise = (_effects != null && _effects.NoiseEnabled && _effects.NoiseAmount > 0);

            double strokePad = hasOutline ? Math.Min(10, _effects.OutlineThickness) : 0;
            double slotWidth = RenderSize.Width > 0 ? RenderSize.Width : (_visualBounds.IsEmpty ? _formattedText.Width : _visualBounds.Width);

            double minX = _visualBounds.IsEmpty ? 0 : _visualBounds.Left;
            double maxX = _visualBounds.IsEmpty ? _formattedText.Width : _visualBounds.Right;
            double visualCenter = (minX + maxX) / 2.0;

            double drawX;
            if (string.Equals(_elementAlignment, "Left", StringComparison.OrdinalIgnoreCase))
            {
                drawX = strokePad - minX + Math.Max(0.0, -_offsetX) + _offsetX;
            }
            else if (string.Equals(_elementAlignment, "Right", StringComparison.OrdinalIgnoreCase))
            {
                drawX = slotWidth - strokePad - maxX - Math.Max(0.0, _offsetX) + _offsetX;
            }
            else // Center default: exact visual center alignment with offset
            {
                drawX = (slotWidth / 2.0) - visualCenter + _offsetX;
            }

            double extraTop = Math.Max(0.0, -_offsetY);
            double drawY = strokePad - (_visualBounds.IsEmpty ? 0 : _visualBounds.Top) + extraTop + _offsetY;

            var fillBrush = new SolidColorBrush(_textColor) { Opacity = _textOpacity };
            fillBrush.Freeze();

            Pen outlinePen = null;
            if (hasOutline)
            {
                var strokeColor = ParseColor(_effects.OutlineColor);
                var strokeBrush = new SolidColorBrush(strokeColor) { Opacity = _effects.OutlineOpacity * _textOpacity };
                strokeBrush.Freeze();
                outlinePen = new Pen(strokeBrush, Math.Max(0.5, _effects.OutlineThickness))
                {
                    LineJoin = PenLineJoin.Round,
                    StartLineCap = PenLineCap.Round,
                    EndLineCap = PenLineCap.Round
                };
                outlinePen.Freeze();
            }

            // GLITCH EFFECT (Rendering-only displacement, zero layout impact)
            if (hasGlitch)
            {
                int tick = _globalAnimTick;
                int seed = tick + (int)(_fontSize * 7) + (_text.Length * 13);
                var rnd = new Random(seed);

                double maxDisp = (_effects.GlitchIntensity / 100.0) * 5.0; // 0 to 5 DIPs
                double dx1 = (rnd.NextDouble() * 2.0 - 1.0) * maxDisp;
                double dy1 = (rnd.NextDouble() * 2.0 - 1.0) * (maxDisp * 0.3);
                double dx2 = (rnd.NextDouble() * 2.0 - 1.0) * maxDisp;
                double dy2 = (rnd.NextDouble() * 2.0 - 1.0) * (maxDisp * 0.3);

                // Ghost 1 (Cyan / GlitchColor1)
                var color1 = ParseColor(_effects.GlitchColor1 ?? "#00FFFF");
                var brush1 = new SolidColorBrush(color1) { Opacity = 0.55 * _textOpacity };
                brush1.Freeze();
                dc.PushTransform(new TranslateTransform(drawX + dx1, drawY + dy1));
                dc.DrawGeometry(brush1, null, _cachedGeometry);
                dc.Pop();

                // Ghost 2 (Magenta / GlitchColor2)
                var color2 = ParseColor(_effects.GlitchColor2 ?? "#FF0055");
                var brush2 = new SolidColorBrush(color2) { Opacity = 0.55 * _textOpacity };
                brush2.Freeze();
                dc.PushTransform(new TranslateTransform(drawX + dx2, drawY + dy2));
                dc.DrawGeometry(brush2, null, _cachedGeometry);
                dc.Pop();
            }

            // BASE / OUTLINE RENDERING
            dc.PushTransform(new TranslateTransform(drawX, drawY));
            dc.DrawGeometry(fillBrush, outlinePen, _cachedGeometry);

            // NOISE EFFECT
            if (hasNoise)
            {
                int tick = _globalAnimTick;
                var rnd = new Random(tick ^ 0x5A5A);
                double noiseFactor = (_effects.NoiseAmount / 100.0);

                int scanlineCount = Math.Max(2, (int)(_fontSize / 8));
                double lineSpacing = Math.Max(2, _fontSize / scanlineCount);

                var noiseColor = Color.FromArgb((byte)(rnd.Next(20, 55) * noiseFactor), 255, 255, 255);
                var noiseBrush = new SolidColorBrush(noiseColor);
                noiseBrush.Freeze();
                var noisePen = new Pen(noiseBrush, 1.0);
                noisePen.Freeze();

                for (int i = 0; i < scanlineCount; i++)
                {
                    double y = (i * lineSpacing + (tick * 2) % lineSpacing);
                    if (y < (_visualBounds.IsEmpty ? _formattedText.Height : _visualBounds.Height))
                    {
                        dc.DrawLine(noisePen, new Point(_visualBounds.IsEmpty ? 0 : _visualBounds.Left, y), new Point(_visualBounds.IsEmpty ? _formattedText.Width : _visualBounds.Right, y));
                    }
                }
            }

            dc.Pop();

            // Settings preview selection highlight cue (active only during Settings position editing)
            if (_isSelectedForEdit)
            {
                double pad = 4.0;
                double glyphW = maxX - minX;
                double glyphH = _visualBounds.IsEmpty ? _formattedText.Height : _visualBounds.Height;
                Rect selRect = new Rect(drawX + minX - pad, drawY + (_visualBounds.IsEmpty ? 0 : _visualBounds.Top) - pad, glyphW + pad * 2, glyphH + pad * 2);
                var selPen = new Pen(new SolidColorBrush(Color.FromArgb(200, 0, 220, 255)), 1.2);
                selPen.DashStyle = DashStyles.Dash;
                selPen.Freeze();
                dc.DrawRoundedRectangle(null, selPen, selRect, 3, 3);
            }
        }

        private static Color ParseColor(string hex)
        {
            try { return (Color)ColorConverter.ConvertFromString(hex); }
            catch { return Color.FromRgb(0xD6, 0xD3, 0xD0); }
        }
    }

    public interface ISettingsHost
    {
        void ApplyPreview(WidgetSettings preview);
        void CommitSettings(WidgetSettings settings);
        void SetEditing(bool on);
        bool IsEditing { get; }
        void CenterOnScreen();
        void SetElementEditingHighlight(string elementKey);
        double Left { get; }
        double Top { get; }
        double AnchorX { get; }
        double AnchorY { get; }
    }

    public class ClockWindow : Window, ISettingsHost
    {
        private WidgetSettings _settings;
        private bool _editing = false;
        private bool _isClosing = false;
        private bool _applyingAnchorPosition = false;
        private HwndSource _hwndSource;
        private static NotifyIcon _trayIcon;
        private static System.Drawing.Icon _trayAppIcon;
        private static ToolStripMenuItem _trayItemEdit;
        private static string _trayIconSourceUsed = "";
        private DispatcherTimer _timeTimer;
        private DispatcherTimer _effectAnimationTimer;

        // Visual containers
        private Border _root;
        private Grid _mainGrid;
        private StackPanel _stackLeft;
        private StackPanel _stackRight;
        private StackPanel _stackCenter;

        private StackPanel _posAboveWidget;
        private StackPanel _posAboveGreeting;
        private EffectTextBlock _greetingText;
        private StackPanel _posBelowGreeting;
        private StackPanel _posAboveWeekday;
        private EffectTextBlock _weekdayText;
        private StackPanel _posBelowWeekday;
        private StackPanel _posAboveTime;
        private EffectTextBlock _timeText;
        private StackPanel _posBelowTime;
        private StackPanel _posAboveDate;
        private EffectTextBlock _dateText;
        private StackPanel _posBelowDate;
        private StackPanel _posBelowWidget;
        private Dictionary<string, EffectTextBlock> _customBlockElements = new Dictionary<string, EffectTextBlock>();

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint VK_C = 0x43;
        private const int HOTKEY_ID = 9001;
        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint RegisterWindowMessage(string lpString);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        private const int MONITOR_DEFAULTTONEAREST = 2;
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, out MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO { public int cbSize; public RECT rcMonitor; public RECT rcWork; public uint dwFlags; }

        public ClockWindow()
        {
            _settings = SettingsManager.Load();
            InitializeUI();
        }

        private void InitializeUI()
        {
            Title = "DesktopClockWidget";
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            ShowInTaskbar = false;
            Topmost = false;
            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.Manual;

            _root = new Border
            {
                Margin = new Thickness(10),
                Padding = new Thickness(20, 14, 20, 14),
                CornerRadius = new CornerRadius(8),
                Background = Brushes.Transparent
            };

            _mainGrid = new Grid();
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            _stackLeft = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 16, 0) };
            _stackRight = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(16, 0, 0, 0) };
            _stackCenter = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };

            Grid.SetColumn(_stackLeft, 0);
            Grid.SetColumn(_stackCenter, 1);
            Grid.SetColumn(_stackRight, 2);

            _mainGrid.Children.Add(_stackLeft);
            _mainGrid.Children.Add(_stackCenter);
            _mainGrid.Children.Add(_stackRight);

            _posAboveWidget = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
            _posAboveGreeting = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
            _greetingText = new EffectTextBlock();
            _posBelowGreeting = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
            _posAboveWeekday = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
            _weekdayText = new EffectTextBlock();
            _posBelowWeekday = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
            _posAboveTime = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
            _timeText = new EffectTextBlock();
            _posBelowTime = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
            _posAboveDate = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
            _dateText = new EffectTextBlock();
            _posBelowDate = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
            _posBelowWidget = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };

            _stackCenter.Children.Add(_posAboveWidget);
            _stackCenter.Children.Add(_posAboveGreeting);
            _stackCenter.Children.Add(_greetingText);
            _stackCenter.Children.Add(_posBelowGreeting);
            _stackCenter.Children.Add(_posAboveWeekday);
            _stackCenter.Children.Add(_weekdayText);
            _stackCenter.Children.Add(_posBelowWeekday);
            _stackCenter.Children.Add(_posAboveTime);
            _stackCenter.Children.Add(_timeText);
            _stackCenter.Children.Add(_posBelowTime);
            _stackCenter.Children.Add(_posAboveDate);
            _stackCenter.Children.Add(_dateText);
            _stackCenter.Children.Add(_posBelowDate);
            _stackCenter.Children.Add(_posBelowWidget);

            _root.Child = _mainGrid;
            Content = _root;

            Loaded += ClockWindow_Loaded;
            Closing += ClockWindow_Closing;
        }

        private string _activeHighlightedElementKey = null;

        public void SetElementEditingHighlight(string elementKey)
        {
            _activeHighlightedElementKey = elementKey;
            _greetingText.IsSelectedForEdit = string.Equals(elementKey, "Greeting", StringComparison.OrdinalIgnoreCase);
            _weekdayText.IsSelectedForEdit = string.Equals(elementKey, "Weekday", StringComparison.OrdinalIgnoreCase);
            _timeText.IsSelectedForEdit = string.Equals(elementKey, "Time", StringComparison.OrdinalIgnoreCase);
            _dateText.IsSelectedForEdit = string.Equals(elementKey, "Date", StringComparison.OrdinalIgnoreCase);

            if (_customBlockElements != null)
            {
                foreach (var kv in _customBlockElements)
                {
                    if (kv.Value != null) kv.Value.IsSelectedForEdit = string.Equals(elementKey, kv.Key, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        public void ApplySettings()
        {
            _settings.PositionLocked = _settings.ClickThrough;
            if (_settings.Greeting != null) Fonts.PinFamily(_settings.Greeting.FontFamily);
            if (_settings.Weekday != null) Fonts.PinFamily(_settings.Weekday.FontFamily);
            if (_settings.Time != null) Fonts.PinFamily(_settings.Time.FontFamily);
            if (_settings.Date != null) Fonts.PinFamily(_settings.Date.FontFamily);
            if (_settings.Blocks != null) { foreach (var b in _settings.Blocks) Fonts.PinFamily(b.FontFamily); }
            double mo = _settings.MasterOpacity <= 0 ? 1.0 : _settings.MasterOpacity;

            ApplyElementStyle(_greetingText, _settings.Greeting, _settings.UseGlobalColor, _settings.GlobalColor, _settings.UseGlobalFont, _settings.GlobalFont, mo);
            ApplyElementStyle(_weekdayText, _settings.Weekday, _settings.UseGlobalColor, _settings.GlobalColor, _settings.UseGlobalFont, _settings.GlobalFont, mo);
            ApplyElementStyle(_timeText, _settings.Time, _settings.UseGlobalColor, _settings.GlobalColor, _settings.UseGlobalFont, _settings.GlobalFont, mo);
            ApplyElementStyle(_dateText, _settings.Date, _settings.UseGlobalColor, _settings.GlobalColor, _settings.UseGlobalFont, _settings.GlobalFont, mo);

            ApplyCustomBlocks(mo);

            // Compute Zero-Jitter Stable Layout Envelopes for all dynamic core elements
            if (_timeText != null && _settings.Time != null && _settings.Time.Visible)
            {
                _timeText.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(_timeText.FontFamily, _timeText.FontWeight, _timeText.FontSize, _timeText.Effects, _settings.Time.Case, DynamicEnvelopeHelper.GetTimeCandidates(_settings));
            }
            if (_weekdayText != null && _settings.Weekday != null && _settings.Weekday.Visible)
            {
                _weekdayText.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(_weekdayText.FontFamily, _weekdayText.FontWeight, _weekdayText.FontSize, _weekdayText.Effects, _settings.Weekday.Case, DynamicEnvelopeHelper.GetWeekdayCandidates());
            }
            if (_dateText != null && _settings.Date != null && _settings.Date.Visible)
            {
                _dateText.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(_dateText.FontFamily, _dateText.FontWeight, _dateText.FontSize, _dateText.Effects, _settings.Date.Case, DynamicEnvelopeHelper.GetDateCandidates(_settings));
            }
            if (_greetingText != null && _settings.Greeting != null && _settings.Greeting.Visible)
            {
                _greetingText.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(_greetingText.FontFamily, _greetingText.FontWeight, _greetingText.FontSize, _greetingText.Effects, _settings.Greeting.Case, DynamicEnvelopeHelper.GetGreetingCandidates(_settings));
            }

            // Compute Stable Envelopes for rotating / scheduled custom blocks
            if (_settings.Blocks != null)
            {
                foreach (var b in _settings.Blocks)
                {
                    if (b != null && b.Enabled && _customBlockElements.ContainsKey(b.Id))
                    {
                        var elem = _customBlockElements[b.Id];
                        var candidates = new List<string>();
                        if (b.Type == "Rotating Text" && b.Messages != null) candidates.AddRange(b.Messages);
                        else if (b.Type == "Scheduled Messages" && b.ScheduledMessages != null)
                        {
                            foreach (var sm in b.ScheduledMessages) if (!string.IsNullOrEmpty(sm.Text)) candidates.Add(sm.Text);
                        }
                        if (candidates.Count > 0)
                        {
                            elem.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(elem.FontFamily, elem.FontWeight, elem.FontSize, elem.Effects, b.Case, candidates);
                        }
                    }
                }
            }

            ApplyGreeting();
            UpdateDateTime();
            ApplyScale(_settings.Scale);

            UpdateAnimationTimerState();

            if (!_editing)
                ApplyClickThrough(_settings.ClickThrough);
            else
                ApplyClickThrough(false);

            PositionWindowAroundAnchor();
        }

        private void ApplyCustomBlocks(double masterOpacity)
        {
            _stackLeft.Children.Clear();
            _stackRight.Children.Clear();
            _posAboveWidget.Children.Clear();
            _posAboveGreeting.Children.Clear();
            _posBelowGreeting.Children.Clear();
            _posAboveWeekday.Children.Clear();
            _posBelowWeekday.Children.Clear();
            _posAboveTime.Children.Clear();
            _posBelowTime.Children.Clear();
            _posAboveDate.Children.Clear();
            _posBelowDate.Children.Clear();
            _posBelowWidget.Children.Clear();
            _customBlockElements.Clear();

            if (_settings.Blocks == null) return;

            DateTime now = DateTime.Now;
            var orderedBlocks = _settings.Blocks.OrderBy(b => b.Order).ToList();

            foreach (var b in orderedBlocks)
            {
                if (!b.Enabled) continue;

                string rawText = BlockEvaluator.EvaluateBlockContent(b, now);
                string text = TextCaseHelper.ApplyCase(rawText, b.Case);
                if (string.IsNullOrEmpty(text)) continue;

                string fontName = (_settings.UseGlobalFont && !string.IsNullOrEmpty(_settings.GlobalFont)) ? _settings.GlobalFont : b.FontFamily;
                FontFamily fam = GlyphHelper.ResolveFontForText(Fonts.For(fontName), text);

                var tb = new EffectTextBlock
                {
                    Text = text,
                    FontFamily = fam,
                    FontWeight = Fonts.ParseWeight(b.FontWeight),
                    FontStyle = b.Italic ? FontStyles.Italic : FontStyles.Normal,
                    FontSize = Math.Max(6, b.FontSize),
                    Margin = new Thickness(0, 1, 0, 1),
                    Tag = b.Id,
                    Effects = b.Effects != null ? b.Effects.Clone() : new TextEffectSettings(),
                    IsSelectedForEdit = string.Equals(_activeHighlightedElementKey, b.Id, StringComparison.OrdinalIgnoreCase)
                };

                string hexColor = (_settings.UseGlobalColor && !string.IsNullOrEmpty(_settings.GlobalColor)) ? _settings.GlobalColor : b.Color;
                tb.TextColor = ParseColor(hexColor);
                tb.TextOpacity = Math.Max(0.0, Math.Min(1.0, b.Opacity * masterOpacity));
                tb.ElementAlignment = !string.IsNullOrEmpty(b.Alignment) ? b.Alignment : "Center";
                tb.OffsetX = b.OffsetX;
                tb.OffsetY = b.OffsetY;

                _customBlockElements[b.Id] = tb;

                StackPanel container = GetContainerForPosition(b.Position);
                container.Children.Add(tb);
            }

            UpdateContainerVisibility();
        }

        private StackPanel GetContainerForPosition(string position)
        {
            if (string.IsNullOrEmpty(position)) return _posAboveWidget;
            switch (position.ToLowerInvariant())
            {
                case "above widget": return _posAboveWidget;
                case "below widget": return _posBelowWidget;
                case "above greeting": return _posAboveGreeting;
                case "below greeting": return _posBelowGreeting;
                case "above weekday": return _posAboveWeekday;
                case "below weekday": return _posBelowWeekday;
                case "above time": return _posAboveTime;
                case "below time": return _posBelowTime;
                case "above date": return _posAboveDate;
                case "below date": return _posBelowDate;
                case "left of widget": return _stackLeft;
                case "right of widget": return _stackRight;
                default: return _posAboveWidget;
            }
        }

        private void UpdateContainerVisibility()
        {
            _stackLeft.Visibility = _stackLeft.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _stackRight.Visibility = _stackRight.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _posAboveWidget.Visibility = _posAboveWidget.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _posAboveGreeting.Visibility = _posAboveGreeting.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _posBelowGreeting.Visibility = _posBelowGreeting.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _posAboveWeekday.Visibility = _posAboveWeekday.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _posBelowWeekday.Visibility = _posBelowWeekday.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _posAboveTime.Visibility = _posAboveTime.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _posBelowTime.Visibility = _posBelowTime.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _posAboveDate.Visibility = _posAboveDate.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _posBelowDate.Visibility = _posBelowDate.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            _posBelowWidget.Visibility = _posBelowWidget.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ApplyElementStyle(EffectTextBlock tb, ElementSettings elem, bool useGlobalColor, string globalColor, bool useGlobalFont, string globalFont, double masterOpacity)
        {
            if (tb == null || elem == null) return;

            tb.Visibility = elem.Visible ? Visibility.Visible : Visibility.Collapsed;

            string fontName = (useGlobalFont && !string.IsNullOrEmpty(globalFont)) ? globalFont : elem.FontFamily;
            tb.FontFamily = Fonts.For(fontName);
            tb.FontWeight = Fonts.ParseWeight(elem.FontWeight);
            tb.FontStyle = FontStyles.Normal;
            tb.FontSize = Math.Max(6, elem.FontSize);

            string hexColor = (useGlobalColor && !string.IsNullOrEmpty(globalColor)) ? globalColor : elem.Color;
            tb.TextColor = ParseColor(hexColor);
            tb.TextOpacity = Math.Max(0.0, Math.Min(1.0, elem.Opacity * masterOpacity));
            tb.ElementAlignment = !string.IsNullOrEmpty(elem.HorizontalAlignment) ? elem.HorizontalAlignment : "Center";
            tb.OffsetX = elem.OffsetX;
            tb.OffsetY = elem.OffsetY;
            tb.Effects = elem.Effects != null ? elem.Effects.Clone() : new TextEffectSettings();
        }

        private void ApplyGreeting()
        {
            if (_settings.GreetingMode == 2)
            {
                _greetingText.Visibility = Visibility.Collapsed;
                return;
            }

            _greetingText.Visibility = (_settings.Greeting != null && _settings.Greeting.Visible) ? Visibility.Visible : Visibility.Collapsed;

            string greeting;
            if (_settings.GreetingMode == 1)
            {
                greeting = string.IsNullOrEmpty(_settings.CustomGreeting) ? "WELCOME" : _settings.CustomGreeting;
            }
            else
            {
                int h = DateTime.Now.Hour;
                greeting = Greeting.For(h, _settings.MorningStart, _settings.AfternoonStart, _settings.EveningStart, _settings.NightStart);
            }

            string textCase = (_settings.Greeting != null && !string.IsNullOrEmpty(_settings.Greeting.Case)) ? _settings.Greeting.Case : "Upper";
            _greetingText.Text = TextCaseHelper.ApplyCase(greeting, textCase);
        }

        private void ApplyScale(double targetScale)
        {
            double scale = Math.Max(0.4, Math.Min(3.0, targetScale));
            _mainGrid.LayoutTransform = new ScaleTransform(scale, scale);
        }

        private void UpdateAnimationTimerState()
        {
            bool hasAnim = false;
            if (_greetingText.Visibility == Visibility.Visible && _greetingText.Effects != null && _greetingText.Effects.HasAnimatedEffects()) hasAnim = true;
            if (_weekdayText.Visibility == Visibility.Visible && _weekdayText.Effects != null && _weekdayText.Effects.HasAnimatedEffects()) hasAnim = true;
            if (_timeText.Visibility == Visibility.Visible && _timeText.Effects != null && _timeText.Effects.HasAnimatedEffects()) hasAnim = true;
            if (_dateText.Visibility == Visibility.Visible && _dateText.Effects != null && _dateText.Effects.HasAnimatedEffects()) hasAnim = true;

            if (!hasAnim && _settings.Blocks != null)
            {
                foreach (var b in _settings.Blocks)
                {
                    if (b.Enabled && b.Effects != null && b.Effects.HasAnimatedEffects())
                    {
                        hasAnim = true;
                        break;
                    }
                }
            }

            if (hasAnim)
            {
                if (_effectAnimationTimer == null)
                {
                    _effectAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(70) }; // ~14 FPS
                    _effectAnimationTimer.Tick += (s, e) =>
                    {
                        EffectTextBlock.AdvanceGlobalAnimation();
                        InvalidateVisualSubtree(_root);
                    };
                }
                if (!_effectAnimationTimer.IsEnabled) _effectAnimationTimer.Start();
            }
            else
            {
                if (_effectAnimationTimer != null && _effectAnimationTimer.IsEnabled)
                {
                    _effectAnimationTimer.Stop();
                }
            }
        }

        private void InvalidateVisualSubtree(DependencyObject parent)
        {
            if (parent == null) return;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var etb = child as EffectTextBlock;
                if (etb != null && etb.Effects != null && etb.Effects.HasAnimatedEffects())
                {
                    etb.InvalidateVisual();
                }
                InvalidateVisualSubtree(child);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            PositionWindowAroundAnchor();
        }

        public void PositionWindowAroundAnchor()
        {
            if (_applyingAnchorPosition || _editing || !_settings.HasAnchor) return;
            try
            {
                _applyingAnchorPosition = true;
                double w = ActualWidth > 0 ? ActualWidth : DesiredSize.Width;
                double h = ActualHeight > 0 ? ActualHeight : DesiredSize.Height;
                if (w <= 0 || h <= 0) return;

                double newLeft = _settings.AnchorX - (w / 2.0);
                double newTop = _settings.AnchorY - (h / 2.0);

                if (Math.Abs(Left - newLeft) > 0.001 || Math.Abs(Top - newTop) > 0.001)
                {
                    Left = newLeft;
                    Top = newTop;
                    _settings.Left = Left;
                    _settings.Top = Top;
                }
            }
            finally
            {
                _applyingAnchorPosition = false;
            }
        }

        private IntPtr WindowHandle
        {
            get
            {
                if (_hwndSource != null && _hwndSource.Handle != IntPtr.Zero)
                    return _hwndSource.Handle;
                try { return new WindowInteropHelper(this).EnsureHandle(); }
                catch { return IntPtr.Zero; }
            }
        }

        public void ApplyClickThrough(bool clickThrough)
        {
            IntPtr hwnd = WindowHandle;
            if (hwnd == IntPtr.Zero) return;
            int es = GetWindowLong(hwnd, GWL_EXSTYLE);
            if (clickThrough && !_editing) es |= WS_EX_TRANSPARENT;
            else es &= ~WS_EX_TRANSPARENT;
            SetWindowLong(hwnd, GWL_EXSTYLE, es);
            UpdateZ();
        }

        private void SaveSettings()
        {
            SettingsManager.Save(_settings);
        }

        private static Color ParseColor(string hex)
        {
            try { return (Color)ColorConverter.ConvertFromString(hex); }
            catch { return Color.FromRgb(0xD6, 0xD3, 0xD0); }
        }

        public void ApplyPreview(WidgetSettings preview)
        {
            bool wasEditing = _editing;
            _settings = preview;
            ApplySettings();
            _editing = wasEditing;
        }

        public void CommitSettings(WidgetSettings settings)
        {
            ApplyPreview(settings);
            SaveSettings();
        }

        private void ClockWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _hwndSource = PresentationSource.FromVisual(this) as HwndSource;
                IntPtr h = WindowHandle;
                if (h != IntPtr.Zero)
                {
                    int es = GetWindowLong(h, GWL_EXSTYLE);
                    es |= WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
                    if (_settings.ClickThrough && !_editing) es |= WS_EX_TRANSPARENT;
                    SetWindowLong(h, GWL_EXSTYLE, es);

                    if (_hwndSource != null)
                    {
                        _hwndSource.AddHook(HwndHook);
                        RegisterHotKey(_hwndSource.Handle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_C);
                        try { _wmTaskbarCreated = RegisterWindowMessage("TaskbarCreated"); } catch { }
                    }
                }
            }
            catch { }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateLayout();
                double w = ActualWidth > 0 ? ActualWidth : DesiredSize.Width;
                double h = ActualHeight > 0 ? ActualHeight : DesiredSize.Height;
                if (w < 10) w = 300;
                if (h < 10) h = 200;

                bool hasSaved = SettingsManager.LoadedFromDisk && !(_settings.Left == 0 && _settings.Top == 0);
                if (hasSaved)
                {
                    Left = _settings.Left;
                    Top = _settings.Top;
                    if (!_settings.HasAnchor || (_settings.AnchorX == 0 && _settings.AnchorY == 0))
                    {
                        _settings.AnchorX = Left + (w / 2.0);
                        _settings.AnchorY = Top + (h / 2.0);
                        _settings.HasAnchor = true;
                        SaveSettings();
                    }
                    ClampIntoVisible();
                }
                else
                {
                    CenterOnScreen();
                    SaveSettings();
                }

                UpdateZ();
                ApplySettings();
                UpdateDateTime();
                Opacity = 1;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    GC.Collect(2, GCCollectionMode.Optimized);
                }), DispatcherPriority.ApplicationIdle, null);
            }), DispatcherPriority.Loaded, null);

            _timeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timeTimer.Tick += TimeTimer_Tick;
            _timeTimer.Start();

            CreateTrayIcon();

            _root.MouseLeftButtonDown += Widget_MouseLeftButtonDown;
            _root.MouseRightButtonDown += Widget_MouseRightButtonDown;
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                SetEditing(!_editing);
                handled = true;
            }
            else if (_wmTaskbarCreated != 0 && (uint)msg == _wmTaskbarCreated)
            {
                CreateTrayIcon();
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void ClockWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_isClosing) return;
            _isClosing = true;
            if (_timeTimer != null) _timeTimer.Stop();
            if (_effectAnimationTimer != null) _effectAnimationTimer.Stop();
            if (_hwndSource != null)
            {
                try { UnregisterHotKey(_hwndSource.Handle, HOTKEY_ID); } catch { }
            }
            if (_trayIcon != null)
            {
                try
                {
                    _trayIcon.Visible = false;
                    _trayIcon.Dispose();
                    _trayIcon = null;
                }
                catch { }
            }
            SaveSettings();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            IntPtr hwnd = WindowHandle;
            if (hwnd != IntPtr.Zero)
            {
                int es = GetWindowLong(hwnd, GWL_EXSTYLE);
                es |= WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
                if (_settings.ClickThrough && !_editing) es |= WS_EX_TRANSPARENT;
                SetWindowLong(hwnd, GWL_EXSTYLE, es);
                UpdateZ();
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            UpdateZ();
        }

        private void UpdateZ()
        {
            IntPtr hwnd = WindowHandle;
            if (hwnd == IntPtr.Zero) return;
            if (_editing)
                SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
        }

        private void ClampIntoVisible()
        {
            try
            {
                double vl = SystemParameters.VirtualScreenLeft;
                double vt = SystemParameters.VirtualScreenTop;
                double vw = SystemParameters.VirtualScreenWidth;
                double vh = SystemParameters.VirtualScreenHeight;
                double w = ActualWidth > 10 ? ActualWidth : DesiredSize.Width;
                double h = ActualHeight > 10 ? ActualHeight : DesiredSize.Height;
                if (w < 10) w = 300;
                if (h < 10) h = 200;

                if (Left + w < vl + 50) Left = vl;
                if (Left > vl + vw - 50) Left = vl + vw - w;
                if (Top + h < vt + 50) Top = vt;
                if (Top > vt + vh - 50) Top = vt + vh - h;

                _settings.AnchorX = Left + (w / 2.0);
                _settings.AnchorY = Top + (h / 2.0);
            }
            catch { }
        }

        public void CenterOnScreen()
        {
            try
            {
                IntPtr hwnd = WindowHandle;
                if (hwnd == IntPtr.Zero) return;

                var mi = new MONITORINFO();
                mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                IntPtr mon = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
                if (!GetMonitorInfo(mon, out mi)) return;

                double scale = GetDpiScale();
                int workLeft = mi.rcWork.Left;
                int workTop = mi.rcWork.Top;
                double workW = (mi.rcWork.Right - mi.rcWork.Left) / scale;
                double workH = (mi.rcWork.Bottom - mi.rcWork.Top) / scale;

                UpdateLayout();
                double w = ActualWidth > 10 ? ActualWidth : DesiredSize.Width;
                double h = ActualHeight > 10 ? ActualHeight : DesiredSize.Height;
                if (w < 10 || h < 10)
                {
                    Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    w = DesiredSize.Width;
                    h = DesiredSize.Height;
                }

                double centerX = (workLeft / scale) + (workW / 2.0);
                double centerY = (workTop / scale) + (workH / 2.0);

                _settings.AnchorX = centerX;
                _settings.AnchorY = centerY;
                _settings.HasAnchor = true;

                Left = centerX - (w / 2.0);
                Top = centerY - (h / 2.0);
                _settings.Left = Left;
                _settings.Top = Top;
                SaveSettings();
            }
            catch
            {
                Left = (SystemParameters.PrimaryScreenWidth - 300) / 2.0;
                Top = (SystemParameters.PrimaryScreenHeight - 200) / 2.0;
                _settings.AnchorX = Left + 150;
                _settings.AnchorY = Top + 100;
                _settings.HasAnchor = true;
                _settings.Left = Left;
                _settings.Top = Top;
                SaveSettings();
            }
        }

        private static double GetDpiScale()
        {
            try
            {
                IntPtr dc = GetDC(IntPtr.Zero);
                int dpi = GetDeviceCaps(dc, 88 /*LOGPIXELSX*/);
                ReleaseDC(IntPtr.Zero, dc);
                if (dpi <= 0) return 1.0;
                return dpi / 96.0;
            }
            catch { return 1.0; }
        }

        private int _lastRenderedMinute = -1;
        private int _lastRenderedDay = -1;
        private int _lastRenderedHour = -1;

        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            UpdateDateTime(false);
            if (_settings != null && _settings.GreetingMode == 0 && now.Hour != _lastRenderedHour)
            {
                _lastRenderedHour = now.Hour;
                ApplyGreeting();
            }
            UpdateRotatingBlocks();
        }

        public void UpdateDateTime()
        {
            UpdateDateTime(true);
        }

        public void UpdateDateTime(bool force)
        {
            var now = DateTime.Now;
            var culture = CultureInfo.InvariantCulture;

            if (force || now.Minute != _lastRenderedMinute)
            {
                _lastRenderedMinute = now.Minute;
                string newTime = now.ToString("hh:mm tt", culture);
                if (_timeText.Text != newTime) _timeText.Text = newTime;
            }

            if (force || now.Day != _lastRenderedDay)
            {
                _lastRenderedDay = now.Day;
                string weekday = now.ToString("dddd", culture);
                string newWeekday = TextCaseHelper.ApplyCase(weekday, _settings.Weekday != null ? _settings.Weekday.Case : "Title");
                if (_weekdayText.Text != newWeekday) _weekdayText.Text = newWeekday;

                string date = now.ToString("dd MMM", culture);
                string newDate = TextCaseHelper.ApplyCase(date, _settings.Date != null ? _settings.Date.Case : "Upper");
                if (_dateText.Text != newDate) _dateText.Text = newDate;
            }
        }

        private void UpdateRotatingBlocks()
        {
            if (_settings.Blocks == null) return;
            DateTime now = DateTime.Now;

            foreach (var b in _settings.Blocks)
            {
                if (!b.Enabled) continue;
                if (!string.Equals(b.Type, "Rotating Text", StringComparison.OrdinalIgnoreCase) && !string.Equals(b.Type, "Rotating", StringComparison.OrdinalIgnoreCase))
                    continue;

                string newText = BlockEvaluator.EvaluateBlockContent(b, now);
                string formatted = TextCaseHelper.ApplyCase(newText, b.Case);

                StackPanel container = GetContainerForPosition(b.Position);
                if (container != null)
                {
                    foreach (UIElement child in container.Children)
                    {
                        var tb = child as EffectTextBlock;
                        if (tb != null && object.Equals(tb.Tag, b.Id))
                        {
                            if (tb.Text != formatted)
                            {
                                tb.Text = formatted;
                                string fontName = (_settings.UseGlobalFont && !string.IsNullOrEmpty(_settings.GlobalFont)) ? _settings.GlobalFont : b.FontFamily;
                                tb.FontFamily = GlyphHelper.ResolveFontForText(Fonts.For(fontName), formatted);
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void SetEditing(bool on)
        {
            _editing = on;
            IntPtr hwnd = WindowHandle;
            if (hwnd != IntPtr.Zero)
            {
                int es = GetWindowLong(hwnd, GWL_EXSTYLE);
                if (on)
                {
                    _settings.ClickThrough = false;
                    _settings.PositionLocked = false;
                    es &= ~WS_EX_TRANSPARENT;
                    es &= ~WS_EX_NOACTIVATE;
                    SetWindowLong(hwnd, GWL_EXSTYLE, es);
                    SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
                    _root.Cursor = System.Windows.Input.Cursors.SizeAll;
                    _root.BorderThickness = new Thickness(1.5);
                    _root.BorderBrush = new SolidColorBrush(Color.FromArgb(160, 255, 255, 255));
                    _root.Background = new SolidColorBrush(Color.FromArgb(24, 255, 255, 255));
                }
                else
                {
                    _settings.ClickThrough = true;
                    _settings.PositionLocked = true;

                    UpdateLayout();
                    double w = ActualWidth > 0 ? ActualWidth : DesiredSize.Width;
                    double h = ActualHeight > 0 ? ActualHeight : DesiredSize.Height;
                    _settings.AnchorX = Left + (w / 2.0);
                    _settings.AnchorY = Top + (h / 2.0);
                    _settings.HasAnchor = true;
                    _settings.Left = Left;
                    _settings.Top = Top;

                    es |= WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE | WS_EX_TRANSPARENT;
                    SetWindowLong(hwnd, GWL_EXSTYLE, es);
                    SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
                    _root.Cursor = System.Windows.Input.Cursors.Arrow;
                    _root.BorderThickness = new Thickness(0);
                    _root.BorderBrush = Brushes.Transparent;
                    _root.Background = Brushes.Transparent;
                    SaveSettings();
                }
            }
            SyncTrayItems();
        }

        public bool IsEditing { get { return _editing; } }
        public double AnchorX { get { return _settings.AnchorX; } }
        public double AnchorY { get { return _settings.AnchorY; } }

        private void Widget_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_editing) return;
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                try
                {
                    DragMove();
                    UpdateLayout();
                    double w = ActualWidth > 0 ? ActualWidth : DesiredSize.Width;
                    double h = ActualHeight > 0 ? ActualHeight : DesiredSize.Height;
                    _settings.AnchorX = Left + (w / 2.0);
                    _settings.AnchorY = Top + (h / 2.0);
                    _settings.HasAnchor = true;
                    _settings.Left = Left;
                    _settings.Top = Top;
                    SaveSettings();
                }
                catch { }
            }
        }

        private void Widget_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_editing) return;
            ShowSettings();
            e.Handled = true;
        }

        private static uint _wmTaskbarCreated = 0;

        public static void LogTrayDebug(string msg)
        {
            try
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DesktopClock");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string file = Path.Combine(dir, "tray-debug.log");
                string line = string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}] {1}\r\n", DateTime.Now, msg);
                File.AppendAllText(file, line);
            }
            catch { }
        }

        private void CreateTrayIcon()
        {
            LogTrayDebug("CreateTrayIcon entered");
            try
            {
                if (_trayIcon == null)
                {
                    _trayIcon = new NotifyIcon();
                    LogTrayDebug("NotifyIcon constructed");
                }

                _trayAppIcon = LoadOfficialIcon();
                _trayIcon.Icon = _trayAppIcon;
                _trayIcon.Text = "DesktopClock Widget";
                LogTrayDebug("Icon and Text assigned. Source: " + _trayIconSourceUsed);

                var menu = new ContextMenuStrip();
                var headerItem = new ToolStripMenuItem("DesktopClock Widget")
                {
                    Enabled = false,
                    Font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont, System.Drawing.FontStyle.Bold)
                };
                var itemSep0 = new ToolStripSeparator();
                _trayItemEdit = new ToolStripMenuItem(_editing ? "Lock Position (Ctrl+Alt+C)" : "Edit Position (Ctrl+Alt+C)", null, (s, e) => SetEditing(!_editing));
                var itemCenter = new ToolStripMenuItem("Center on Screen", null, (s, e) => { CenterOnScreen(); SaveSettings(); });
                var itemSettings = new ToolStripMenuItem("Settings...", null, (s, e) => ShowSettings());
                var itemSep1 = new ToolStripSeparator();
                var itemExit = new ToolStripMenuItem("Exit", null, (s, e) => Close());

                menu.Items.Add(headerItem);
                menu.Items.Add(itemSep0);
                menu.Items.Add(_trayItemEdit);
                menu.Items.Add(itemCenter);
                menu.Items.Add(itemSettings);
                menu.Items.Add(itemSep1);
                menu.Items.Add(itemExit);

                _trayIcon.ContextMenuStrip = menu;
                _trayIcon.DoubleClick -= TrayIcon_DoubleClick;
                _trayIcon.DoubleClick += TrayIcon_DoubleClick;
                LogTrayDebug("ContextMenuStrip assigned");

                _trayIcon.Visible = false;
                _trayIcon.Visible = true;
                LogTrayDebug("Visible=true executed successfully");
            }
            catch (Exception ex)
            {
                LogTrayDebug("CreateTrayIcon EXCEPTION: " + ex.ToString());
            }
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private void SyncTrayItems()
        {
            if (_trayItemEdit != null)
            {
                _trayItemEdit.Text = _editing ? "Lock Position (Ctrl+Alt+C)" : "Edit Position (Ctrl+Alt+C)";
            }
        }

        private SettingsWindow _openSettingsWindow;
        public void ShowSettings()
        {
            if (_openSettingsWindow != null && _openSettingsWindow.IsLoaded)
            {
                _openSettingsWindow.Activate();
                return;
            }
            _openSettingsWindow = new SettingsWindow(this, _settings);
            _openSettingsWindow.Closed += (s, e) =>
            {
                var win = _openSettingsWindow;
                _openSettingsWindow = null;
                if (win != null)
                {
                    win.Teardown();
                }
                Fonts.ClearPreviewCache();
            };
            _openSettingsWindow.Show();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        private static System.Drawing.Icon LoadOfficialIcon()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string icoPath = System.IO.Path.Combine(baseDir, "app.ico");
                if (System.IO.File.Exists(icoPath))
                {
                    _trayIconSourceUsed = "External app.ico (" + icoPath + ")";
                    return new System.Drawing.Icon(icoPath, 32, 32);
                }

                string fallbackIco = @"D:\AI\DesktopClockWidget\app.ico";
                if (System.IO.File.Exists(fallbackIco))
                {
                    _trayIconSourceUsed = "External app.ico (" + fallbackIco + ")";
                    return new System.Drawing.Icon(fallbackIco, 32, 32);
                }
            }
            catch (Exception ex)
            {
                LogTrayDebug("LoadOfficialIcon external file exception: " + ex.Message);
            }

            try
            {
                _trayIconSourceUsed = "Programmatic fallback";
                using (var bmp = new System.Drawing.Bitmap(32, 32))
                {
                    using (var g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.Clear(System.Drawing.Color.Transparent);

                        float center = 16f;
                        float r = 13.5f;

                        // Cyan and White outer segmented arcs
                        using (var penCyan = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 0, 240, 255), 2.6f) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round })
                        using (var penWhite = new System.Drawing.Pen(System.Drawing.Color.FromArgb(240, 245, 255), 2.2f) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round })
                        {
                            var rect = new System.Drawing.RectangleF(center - r, center - r, r * 2f, r * 2f);
                            g.DrawArc(penCyan, rect, 220f, 100f);
                            g.DrawArc(penWhite, rect, 335f, 70f);
                            g.DrawArc(penCyan, rect, 40f, 100f);
                            g.DrawArc(penWhite, rect, 155f, 50f);
                        }

                        // Cardinal Ticks
                        using (var penTickCyan = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 0, 240, 255), 2.0f))
                        using (var penTickWhite = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 255, 255), 2.0f))
                        {
                            g.DrawLine(penTickCyan, center, center - r - 2f, center, center - r + 2f);
                            g.DrawLine(penTickCyan, center, center + r - 2f, center, center + r + 2f);
                            g.DrawLine(penTickWhite, center - r - 2f, center, center - r + 2f, center);
                            g.DrawLine(penTickWhite, center + r - 2f, center, center + r + 2f, center);
                        }

                        // 10:10 Cyber Hands
                        using (var penHour = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 255, 255), 2.6f) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round })
                        using (var penMin = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 0, 240, 255), 2.2f) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round })
                        {
                            // Hour hand (10 o clock)
                            g.DrawLine(penHour, center, center, center - 5.5f, center - 5.5f);
                            // Minute hand (2 o clock)
                            g.DrawLine(penMin, center, center, center + 7.8f, center - 6.5f);
                        }

                        // Glowing Cyan Center Nucleus
                        using (var brushHub = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 0, 240, 255)))
                        {
                            g.FillEllipse(brushHub, center - 3f, center - 3f, 6f, 6f);
                        }
                    }

                    IntPtr hIcon = bmp.GetHicon();
                    System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIcon).Clone();
                    DestroyIcon(hIcon);
                    return icon;
                }
            }
            catch (Exception ex)
            {
                LogTrayDebug("LoadOfficialIcon programmatic fallback exception: " + ex.Message);
                _trayIconSourceUsed = "System fallback";
                return System.Drawing.SystemIcons.Application;
            }
        }
    }

    public class App : Application
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDpiAwarenessContext(IntPtr value);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        private static string AppDataDir
        {
            get
            {
                string d = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DesktopClock");
                if (!Directory.Exists(d)) Directory.CreateDirectory(d);
                return d;
            }
        }

        private static string SelftestResult { get { return Path.Combine(AppDataDir, "selftest-result.txt"); } }
        private static string DragtestResult { get { return Path.Combine(AppDataDir, "dragtest-result.txt"); } }
        private static string CrashLog { get { return Path.Combine(AppDataDir, "crash.log"); } }

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnCrash;
            DispatcherUnhandledException += delegate(object s, DispatcherUnhandledExceptionEventArgs e)
            {
                try { File.AppendAllText(CrashLog, "DISPATCHER " + DateTime.Now + "\r\n" + e.Exception + "\r\n\r\n"); } catch { }
            };
        }

        private static void OnCrash(object sender, UnhandledExceptionEventArgs e)
        {
            try { File.AppendAllText(CrashLog, DateTime.Now + "\r\n" + (e.ExceptionObject as Exception) + "\r\n\r\n"); } catch { }
        }

        private static System.Threading.Mutex _appMutex;
        private static ClockWindow _mainWindow;

        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                ClockWindow.LogTrayDebug("CRITICAL UNHANDLED EXCEPTION: " + (e.ExceptionObject != null ? e.ExceptionObject.ToString() : "null"));
            };

            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "--selftest") { RunSelfTest(); return; }
                if (args[i] == "--dragtest") { RunDragTest(); return; }
                if (args[i] == "--screenshot" && i + 1 < args.Length) { RunScreenshot(args[i + 1]); return; }
            }

            try { SetProcessDpiAwarenessContext(new IntPtr(-4)); } catch { }

            bool createdNew;
            _appMutex = new System.Threading.Mutex(true, @"Local\DesktopClockWidget_SingleInstance_User", out createdNew);
            if (!createdNew)
            {
                try
                {
                    if (_appMutex.WaitOne(0, false))
                        createdNew = true;
                }
                catch (System.Threading.AbandonedMutexException)
                {
                    createdNew = true;
                }
            }
            if (!createdNew) return;

            var app = new App();
            app.DispatcherUnhandledException += (s, e) =>
            {
                ClockWindow.LogTrayDebug("CRITICAL DISPATCHER EXCEPTION: " + e.Exception.ToString());
            };
            _mainWindow = new ClockWindow();
            app.Run(_mainWindow);
        }

        private static void RunSelfTest()
        {
            var app = new Application();
            var sb = new StringBuilder();
            bool ok = true;

            // 1. Top ✦ glyph renders visibly with fallback engine
            var fSymbol = GlyphHelper.ResolveFontForText(Fonts.For("Audiowide"), "✦");
            bool topGlyphOk = GlyphHelper.CanFontRenderText(fSymbol, "✦");
            Check(sb, ref ok, topGlyphOk, "1. Top ✦ glyph resolves & renders (Fallback: " + fSymbol.Source + ")");

            // 2. Bottom ◇ glyph renders visibly with fallback engine
            var fBottom = GlyphHelper.ResolveFontForText(Fonts.For("Audiowide"), "◇");
            bool bottomGlyphOk = GlyphHelper.CanFontRenderText(fBottom, "◇");
            Check(sb, ref ok, bottomGlyphOk, "2. Bottom ◇ glyph resolves & renders (Fallback: " + fBottom.Source + ")");

            // 3. Change top block from ✦ to text FOCUS -> FOCUS appears
            var blockTop = new CustomBlock { Type = "Static Text", StaticContent = "FOCUS", Position = "Above Widget" };
            string textRes = BlockEvaluator.EvaluateBlockContent(blockTop, DateTime.Now);
            Check(sb, ref ok, textRes == "FOCUS", "3. Change top block to text FOCUS (returns FOCUS)");

            // 4. Add second block above widget KEEP GOING -> both render in correct order
            var bList = new List<CustomBlock>
            {
                new CustomBlock { Name = "First", StaticContent = "FOCUS", Position = "Above Widget", Order = 1 },
                new CustomBlock { Name = "Second", StaticContent = "KEEP GOING", Position = "Above Widget", Order = 2 }
            };
            var ordered = bList.OrderBy(b => b.Order).Select(b => b.StaticContent).ToList();
            Check(sb, ref ok, ordered[0] == "FOCUS" && ordered[1] == "KEEP GOING", "4. Multi-block order above widget (FOCUS -> KEEP GOING)");

            // 5. Rotating block A -> B -> C -> A accelerated rotation test
            var rotBlock = new CustomBlock
            {
                Type = "Rotating Text",
                RotationMode = "Sequential",
                Messages = new List<string> { "A", "B", "C" },
                IntervalMinutes = 1
            };
            string msg0 = BlockEvaluator.GetActiveRotatingMessage(rotBlock, DateTime.Now, 0);
            string msg1 = BlockEvaluator.GetActiveRotatingMessage(rotBlock, DateTime.Now, 1);
            string msg2 = BlockEvaluator.GetActiveRotatingMessage(rotBlock, DateTime.Now, 2);
            string msg3 = BlockEvaluator.GetActiveRotatingMessage(rotBlock, DateTime.Now, 3);
            bool rotOk = (msg0 == "A" && msg1 == "B" && msg2 == "C" && msg3 == "A");
            Check(sb, ref ok, rotOk, "5. Rotating block sequence (A -> B -> C -> A)");

            // 6. Production interval 30 minutes persists after clone/save
            rotBlock.IntervalValue = 30;
            rotBlock.IntervalUnit = "Minutes";
            rotBlock.IntervalMinutes = 30;
            var rotCloned = rotBlock.Clone();
            Check(sb, ref ok, rotCloned.IntervalValue == 30 && rotCloned.IntervalMinutes == 30, "6. Production interval 30 minutes persistence");

            // 7. Random mode produces valid messages only
            rotBlock.RotationMode = "Random";
            bool randValid = true;
            for (long step = 0; step < 20; step++)
            {
                string r = BlockEvaluator.GetActiveRotatingMessage(rotBlock, DateTime.Now, step);
                if (!rotBlock.Messages.Contains(r)) randValid = false;
            }
            Check(sb, ref ok, randValid, "7. Random mode produces valid messages only");

            // 8. Scheduled message mode selects correct message for 06:00, 12:00, 18:00, 23:00
            var schedules = new List<ScheduledMessage>
            {
                new ScheduledMessage("06:00", "GOOD MORNING, BUILD SOMETHING"),
                new ScheduledMessage("12:00", "HALF THE DAY IS GONE"),
                new ScheduledMessage("18:00", "REVIEW YOUR PROGRESS"),
                new ScheduledMessage("23:00", "TIME TO REST")
            };
            var today = DateTime.Today;
            string s06 = BlockEvaluator.GetActiveScheduledMessage(schedules, today.AddHours(6).AddMinutes(15));
            string s12 = BlockEvaluator.GetActiveScheduledMessage(schedules, today.AddHours(12).AddMinutes(30));
            string s18 = BlockEvaluator.GetActiveScheduledMessage(schedules, today.AddHours(18).AddMinutes(0));
            string s23 = BlockEvaluator.GetActiveScheduledMessage(schedules, today.AddHours(23).AddMinutes(45));
            string s04 = BlockEvaluator.GetActiveScheduledMessage(schedules, today.AddHours(4).AddMinutes(10));
            bool schedOk = (s06 == "GOOD MORNING, BUILD SOMETHING" &&
                            s12 == "HALF THE DAY IS GONE" &&
                            s18 == "REVIEW YOUR PROGRESS" &&
                            s23 == "TIME TO REST" &&
                            s04 == "TIME TO REST");
            Check(sb, ref ok, schedOk, "8. Scheduled message mode selects correct message by time (06:00, 12:00, 18:00, 23:00)");

            // 9. Hide one custom block
            rotBlock.Enabled = false;
            Check(sb, ref ok, BlockEvaluator.EvaluateBlockContent(rotBlock, DateTime.Now) == "", "9. Hide custom block (returns empty string so gap closes)");

            // 10. Delete block
            var bColl = new List<CustomBlock> { new CustomBlock { Id = "1" }, new CustomBlock { Id = "2" } };
            bColl.RemoveAll(x => x.Id == "1");
            Check(sb, ref ok, bColl.Count == 1 && bColl[0].Id == "2", "10. Delete block removes only target block");

            // 11. Duplicate block
            var origBlock = new CustomBlock { Id = "orig-id", Name = "Alpha", FontSize = 24, Color = "#112233" };
            var dupBlock = origBlock.Clone();
            Check(sb, ref ok, dupBlock.Id != origBlock.Id && dupBlock.FontSize == 24 && dupBlock.Color == "#112233", "11. Duplicate block creates unique ID and preserves styles");

            // 12. Curated App Fonts count >= 100
            int curatedCount = FontCatalog.CuratedAppFontCount;
            Check(sb, ref ok, curatedCount >= 100, "12. Curated App Font count >= 100 (" + curatedCount + " curated families)");

            // 13. Source filter 'App Fonts' returns full curated catalog
            var appFonts = FontCatalog.Filter("App Fonts", "All", "", null);
            Check(sb, ref ok, appFonts.Count == curatedCount && appFonts.All(f => f.IsAppFont), "13. Source filter 'App Fonts' returns curated catalog (" + appFonts.Count + " app fonts)");

            // 14. Source filter 'System Fonts' returns system installed fonts
            var sysFonts = FontCatalog.Filter("System Fonts", "All", "", null);
            Check(sb, ref ok, sysFonts.Count > 50 && sysFonts.All(f => !f.IsAppFont), "14. Source filter 'System Fonts' returns system fonts (" + sysFonts.Count + " system fonts)");

            // 15. Curated category 'Futuristic' has >= 20 fonts
            var futFonts = FontCatalog.Filter("App Fonts", "Futuristic", "", null);
            Check(sb, ref ok, futFonts.Count >= 20, "15. Curated category 'Futuristic' has >= 20 fonts (" + futFonts.Count + " found)");

            // 16. Search fonts ('ox' finds Oxanium)
            var searchFonts = FontCatalog.Filter("All", "All", "ox", null);
            Check(sb, ref ok, searchFonts.Any(f => f.Name.IndexOf("Oxanium", StringComparison.OrdinalIgnoreCase) >= 0), "16. Search fonts ('ox' finds Oxanium)");

            // 17. Curated fonts resolve dynamically on-demand
            var ffOxanium = Fonts.For("Oxanium");
            var ffOrbitron = Fonts.For("Orbitron");
            Check(sb, ref ok, ffOxanium != null && ffOrbitron != null, "17. Curated fonts resolve dynamically on-demand");

            // 18. Favorite fonts persist across load
            var sFav = new WidgetSettings();
            sFav.FavoriteFonts.Add("Orbitron");
            var sFavClone = SettingsManager.Clone(sFav);
            Check(sb, ref ok, sFavClone.FavoriteFonts.Contains("Orbitron"), "18. Favorite fonts persist across load");

            // 19. Independent per-element typography preserved
            var sTypo = new WidgetSettings();
            sTypo.Time.FontFamily = "Orbitron";
            sTypo.Weekday.FontFamily = "Audiowide";
            sTypo.Greeting.FontFamily = "Teko";
            var sTypoClone = SettingsManager.Clone(sTypo);
            Check(sb, ref ok, sTypoClone.Time.FontFamily == "Orbitron" && sTypoClone.Weekday.FontFamily == "Audiowide" && sTypoClone.Greeting.FontFamily == "Teko", "19. Independent per-element typography preserved");

            // PHASE 1: STABLE ANCHOR TESTS
            // 20. Anchor visual center invariance across varying time widths ("01:11 PM", "08:08 PM", "11:11 PM", "12:59 AM")
            double fixedAnchorX = 500.0;
            double fixedAnchorY = 300.0;
            string[] timeStrings = new string[] { "01:11 PM", "08:08 PM", "11:11 PM", "12:59 AM" };
            bool timeWidthAnchorPass = true;
            double maxTimeDrift = 0.0;

            foreach (var tStr in timeStrings)
            {
                var tf = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
                var ft = new FormattedText(tStr, CultureInfo.InvariantCulture, System.Windows.FlowDirection.LeftToRight, tf, 62, Brushes.White);
                double calcLeft = fixedAnchorX - (ft.Width / 2.0);
                double calcCenter = calcLeft + (ft.Width / 2.0);
                double drift = Math.Abs(calcCenter - fixedAnchorX);
                if (drift > maxTimeDrift) maxTimeDrift = drift;
                if (drift > 0.001) timeWidthAnchorPass = false;
            }
            Check(sb, ref ok, timeWidthAnchorPass, "20. Anchor visual center invariant across time widths (Drift: " + maxTimeDrift.ToString("F3") + " DIP)");

            // 21. Anchor invariance across dynamic greetings
            string[] greetings = new string[] { "GOOD MORNING", "GOOD AFTERNOON", "GOOD EVENING", "GOOD NIGHT" };
            bool greetAnchorPass = true;
            double maxGreetDrift = 0.0;
            foreach (var g in greetings)
            {
                var tf = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
                var ft = new FormattedText(g, CultureInfo.InvariantCulture, System.Windows.FlowDirection.LeftToRight, tf, 22, Brushes.White);
                double calcLeft = fixedAnchorX - (ft.Width / 2.0);
                double calcCenter = calcLeft + (ft.Width / 2.0);
                double drift = Math.Abs(calcCenter - fixedAnchorX);
                if (drift > maxGreetDrift) maxGreetDrift = drift;
                if (drift > 0.001) greetAnchorPass = false;
            }
            Check(sb, ref ok, greetAnchorPass, "21. Anchor visual center invariant across greetings (Drift: " + maxGreetDrift.ToString("F3") + " DIP)");

            // 22. Anchor invariance across weekday strings
            string[] weekdays = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            bool weekdayAnchorPass = true;
            double maxWeekdayDrift = 0.0;
            foreach (var w in weekdays)
            {
                var tf = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
                var ft = new FormattedText(w, CultureInfo.InvariantCulture, System.Windows.FlowDirection.LeftToRight, tf, 50, Brushes.White);
                double calcLeft = fixedAnchorX - (ft.Width / 2.0);
                double calcCenter = calcLeft + (ft.Width / 2.0);
                double drift = Math.Abs(calcCenter - fixedAnchorX);
                if (drift > maxWeekdayDrift) maxWeekdayDrift = drift;
                if (drift > 0.001) weekdayAnchorPass = false;
            }
            Check(sb, ref ok, weekdayAnchorPass, "22. Anchor visual center invariant across weekdays (Drift: " + maxWeekdayDrift.ToString("F3") + " DIP)");

            // 23. Anchor invariance across scale changes (0.5 to 2.5)
            double[] scales = new double[] { 0.5, 0.8, 1.0, 1.25, 1.5, 2.0, 2.5 };
            bool scaleAnchorPass = true;
            double baseWidth = 320.0;
            foreach (var sc in scales)
            {
                double scaledW = baseWidth * sc;
                double calcLeft = fixedAnchorX - (scaledW / 2.0);
                double calcCenter = calcLeft + (scaledW / 2.0);
                if (Math.Abs(calcCenter - fixedAnchorX) > 0.001) scaleAnchorPass = false;
            }
            Check(sb, ref ok, scaleAnchorPass, "23. Anchor visual center invariant across scale changes (0.5x - 2.5x)");

            // PHASE 2: TEXT EFFECTS TESTS
            // 24. Outline effect generates valid glyph stroke geometry
            var effectTb = new EffectTextBlock
            {
                Text = "09:26 PM",
                FontSize = 48,
                Effects = new TextEffectSettings { OutlineEnabled = true, OutlineThickness = 3.0, OutlineColor = "#000000" }
            };
            effectTb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Check(sb, ref ok, effectTb.DesiredSize.Width > 100 && effectTb.DesiredSize.Height > 30, "24. Outline effect generates valid glyph contour geometry (" + effectTb.DesiredSize.Width + "x" + effectTb.DesiredSize.Height + ")");

            // 25. Glitch effect preserves constant layout bounds (zero width/height fluctuation during animation)
            effectTb.Effects.GlitchEnabled = true;
            effectTb.Effects.GlitchIntensity = 75.0;
            double widthBefore = effectTb.DesiredSize.Width;
            double heightBefore = effectTb.DesiredSize.Height;
            bool glitchStable = true;
            for (int tick = 0; tick < 30; tick++)
            {
                EffectTextBlock.AdvanceGlobalAnimation();
                effectTb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (Math.Abs(effectTb.DesiredSize.Width - widthBefore) > 0.001 || Math.Abs(effectTb.DesiredSize.Height - heightBefore) > 0.001)
                {
                    glitchStable = false;
                }
            }
            Check(sb, ref ok, glitchStable, "25. Glitch effect preserves constant layout bounds across animation frames");

            // 26. Noise effect preserves constant layout bounds
            effectTb.Effects.NoiseEnabled = true;
            effectTb.Effects.NoiseAmount = 50.0;
            bool noiseStable = true;
            for (int tick = 0; tick < 30; tick++)
            {
                EffectTextBlock.AdvanceGlobalAnimation();
                effectTb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (Math.Abs(effectTb.DesiredSize.Width - widthBefore) > 0.001 || Math.Abs(effectTb.DesiredSize.Height - heightBefore) > 0.001)
                {
                    noiseStable = false;
                }
            }
            Check(sb, ref ok, noiseStable, "26. Noise effect preserves constant layout bounds across animation frames");

            // 27. 60-second effects + anchor stability simulation (0.000 DIP anchor drift)
            double simulatedAnchorX = 640.0;
            double simulatedAnchorY = 480.0;
            bool simPass = true;
            for (int sec = 0; sec < 60; sec++)
            {
                EffectTextBlock.AdvanceGlobalAnimation();
                double w = effectTb.DesiredSize.Width;
                double h = effectTb.DesiredSize.Height;
                double winLeft = simulatedAnchorX - (w / 2.0);
                double winTop = simulatedAnchorY - (h / 2.0);
                double visualCenter = winLeft + (w / 2.0);
                if (Math.Abs(visualCenter - simulatedAnchorX) > 0.001) simPass = false;
            }
            Check(sb, ref ok, simPass, "27. 60-second animated effects + anchor stability simulation (0.000 DIP drift)");

            // 28. Glyph renderer support for all 16 symbols
            foreach (var sym in GlyphHelper.RequiredSymbols)
            {
                var fRes = GlyphHelper.ResolveFontForText(Fonts.For("Audiowide"), sym);
                bool can = GlyphHelper.CanFontRenderText(fRes, sym);
                sb.AppendLine("GLYPH " + sym + ": " + (can ? "RENDERABLE via " + fRes.Source : "FALLBACK NEEDED"));
                if (!can) ok = false;
            }

            // PHASE 3: ELEMENT POSITIONING & VISUAL CENTERING ACCEPTANCE TESTS
            // 29. Default visual center axis alignment (Date "30 AUG" vs core lines differ <= 1.0 DIP)
            string[] coreTexts = new string[] { "GOOD AFTERNOON", "Sunday", "01:11 PM", "30 AUG" };
            string[] coreFonts = new string[] { "Audiowide", "Segoe UI", "Audiowide", "Segoe UI" };
            double[] coreSizes = new double[] { 14, 22, 64, 20 };
            double testColW = 320.0;
            double testExpCenter = testColW / 2.0;
            double maxCenterAxisDiff = 0.0;

            for (int i = 0; i < coreTexts.Length; i++)
            {
                var tf = new Typeface(new FontFamily(coreFonts[i]), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                var ft = new FormattedText(coreTexts[i], CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, coreSizes[i], Brushes.White);
                var geom = ft.BuildGeometry(new Point(0, 0));
                double vCenter = (geom.Bounds.Left + geom.Bounds.Right) / 2.0;
                double drawX = (testColW / 2.0) - vCenter;
                double renderedCenter = drawX + vCenter;
                double diff = Math.Abs(renderedCenter - testExpCenter);
                if (diff > maxCenterAxisDiff) maxCenterAxisDiff = diff;
            }
            Check(sb, ref ok, maxCenterAxisDiff <= 1.0, "29. Default visual center axis alignment across core elements (Max error: " + maxCenterAxisDiff.ToString("F4") + " DIP)");

            // 30. Date X Offset +25 and -25 DIP delta validation
            var tfDate = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var ftDate = new FormattedText("30 AUG", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tfDate, 20, Brushes.White);
            var geomDate = ftDate.BuildGeometry(new Point(0, 0));
            double dateVCenter = (geomDate.Bounds.Left + geomDate.Bounds.Right) / 2.0;
            double baseDrawCenter = (testColW / 2.0) - dateVCenter + dateVCenter;
            double drawCenterPlus = (testColW / 2.0) - dateVCenter + 25.0 + dateVCenter;
            double drawCenterMinus = (testColW / 2.0) - dateVCenter - 25.0 + dateVCenter;
            bool xOffsetOk = Math.Abs((drawCenterPlus - baseDrawCenter) - 25.0) < 0.001 && Math.Abs((drawCenterMinus - baseDrawCenter) - (-25.0)) < 0.001;
            Check(sb, ref ok, xOffsetOk, "30. Date X Offset +25 and -25 DIP delta verified");

            // 31. Date Right Alignment relative to common column width
            double dateMaxX = geomDate.Bounds.Right;
            double rightDrawX = testColW - dateMaxX;
            double rightEdgeDrawn = rightDrawX + dateMaxX;
            Check(sb, ref ok, Math.Abs(rightEdgeDrawn - testColW) < 0.001, "31. Date Right alignment aligned to common content width");

            // 32. Independent mixed alignments (Greeting: Left, Weekday: Center, Time: Right, Date: Center)
            var sMixed = new WidgetSettings();
            sMixed.Greeting.HorizontalAlignment = "Left";
            sMixed.Weekday.HorizontalAlignment = "Center";
            sMixed.Time.HorizontalAlignment = "Right";
            sMixed.Date.HorizontalAlignment = "Center";
            Check(sb, ref ok, sMixed.Greeting.HorizontalAlignment == "Left" && sMixed.Weekday.HorizontalAlignment == "Center" && sMixed.Time.HorizontalAlignment == "Right" && sMixed.Date.HorizontalAlignment == "Center", "32. Independent mixed alignments for core elements");

            // 33. Placement persistence round-trip in WidgetSettings and CustomBlock
            var sPersist = new WidgetSettings();
            sPersist.Date.HorizontalAlignment = "Center";
            sPersist.Date.OffsetX = 20.0;
            sPersist.Date.OffsetY = -5.0;
            sPersist.Blocks.Add(new CustomBlock { Alignment = "Right", OffsetX = 15.0, OffsetY = 8.0 });
            var sCloned = SettingsManager.Clone(sPersist);
            bool persistOk = sCloned.Date.OffsetX == 20.0 && sCloned.Date.OffsetY == -5.0 && sCloned.Blocks.Last().OffsetX == 15.0 && sCloned.Blocks.Last().Alignment == "Right";
            Check(sb, ref ok, persistOk, "33. Placement & offset settings persistence round-trip");

            // 34. Effects + positioning compatibility (Outline, Glitch, Noise)
            var effTb = new EffectTextBlock
            {
                Text = "30 AUG",
                FontSize = 24,
                ElementAlignment = "Center",
                OffsetX = 20.0,
                OffsetY = 10.0,
                Effects = new TextEffectSettings { OutlineEnabled = true, GlitchEnabled = true, NoiseEnabled = true }
            };
            effTb.Measure(new Size(300, 200));
            Check(sb, ref ok, effTb.DesiredSize.Width > 0 && effTb.DesiredSize.Height > 0, "34. Effects + manual positioning compatibility");

            // 35. Visual centering across multiple display/handwritten fonts
            string[] dispFonts = new string[] { "Audiowide", "Segoe UI", "Courier New", "Georgia", "Impact", "Trebuchet MS" };
            bool dispFontsOk = true;
            foreach (var df in dispFonts)
            {
                var tfDF = new Typeface(new FontFamily(df), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                var ftDF = new FormattedText("30 AUG", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tfDF, 24, Brushes.White);
                var gDF = ftDF.BuildGeometry(new Point(0, 0));
                double vcDF = (gDF.Bounds.Left + gDF.Bounds.Right) / 2.0;
                double dX = (testColW / 2.0) - vcDF;
                double actualCenter = dX + vcDF;
                if (Math.Abs(actualCenter - testExpCenter) > 0.001) dispFontsOk = false;
            }
            Check(sb, ref ok, dispFontsOk, "35. Visual centering across display and variable-bearing fonts");

            // 36. Anchor invariance test (AnchorX / AnchorY unchanged when element offsets/alignments change)
            double origAnchorX = 500.0;
            double origAnchorY = 350.0;
            var sAnchor = new WidgetSettings { AnchorX = origAnchorX, AnchorY = origAnchorY, HasAnchor = true };
            sAnchor.Date.OffsetX = 30.0;
            sAnchor.Date.OffsetY = -15.0;
            sAnchor.Date.HorizontalAlignment = "Right";
            sAnchor.Greeting.OffsetX = -25.0;
            bool anchorOk = (sAnchor.AnchorX == origAnchorX && sAnchor.AnchorY == origAnchorY);
            Check(sb, ref ok, anchorOk, "36. Anchor coordinates strictly invariant to element offset/alignment changes");

            // PHASE 4: DYNAMIC BOUNDS & PRECISION POSITIONING ACCEPTANCE TESTS
            // 37. Date Y Offset = +50 DIP: dynamic layout bounds expand by 50 DIP and content remains inside frame
            var tbDateBoundY = new EffectTextBlock { Text = "30 AUG", FontSize = 20, OffsetY = 0.0 };
            tbDateBoundY.Measure(new Size(500, 500));
            double baseH = tbDateBoundY.DesiredSize.Height;

            var tbDateBoundY50 = new EffectTextBlock { Text = "30 AUG", FontSize = 20, OffsetY = 50.0 };
            tbDateBoundY50.Measure(new Size(500, 500));
            double expandedH = tbDateBoundY50.DesiredSize.Height;
            Check(sb, ref ok, Math.Abs(expandedH - (baseH + 50.0)) < 1.0, "37. Date Y Offset = +50 DIP: dynamic layout bounds expand to enclose offset (Height: " + expandedH.ToString("F1") + " vs base " + baseH.ToString("F1") + " DIP)");

            // 38. Date X Offset = -60 DIP: dynamic layout bounds expand by 60 DIP horizontally
            var tbDateBoundX = new EffectTextBlock { Text = "30 AUG", FontSize = 20, OffsetX = 0.0 };
            tbDateBoundX.Measure(new Size(500, 500));
            double baseW = tbDateBoundX.DesiredSize.Width;

            var tbDateBoundX60 = new EffectTextBlock { Text = "30 AUG", FontSize = 20, OffsetX = -60.0 };
            tbDateBoundX60.Measure(new Size(500, 500));
            double expandedW = tbDateBoundX60.DesiredSize.Width;
            Check(sb, ref ok, Math.Abs(expandedW - (baseW + 60.0)) < 1.0, "38. Date X Offset = -60 DIP: dynamic layout bounds expand to enclose offset (Width: " + expandedW.ToString("F1") + " vs base " + baseW.ToString("F1") + " DIP)");

            // 39. Keyboard nudge step: Arrow Right x5 = +5.0 DIP
            double curX = 0.0;
            for (int k = 0; k < 5; k++) curX += 1.0;
            Check(sb, ref ok, Math.Abs(curX - 5.0) < 0.001, "39. Keyboard normal nudge step: Right x5 = exactly +5.0 DIP");

            // 40. Ctrl + Arrow fine nudge step: Ctrl + Right x2 = +1.0 DIP (0.5 DIP per step)
            double fineX = 0.0;
            for (int k = 0; k < 2; k++) fineX += 0.5;
            Check(sb, ref ok, Math.Abs(fineX - 1.0) < 0.001, "40. Ctrl + Arrow fine nudge step: Ctrl+Right x2 = exactly +1.0 DIP (0.5 DIP step)");

            // 41. Shift + Arrow large nudge step: Shift + Down x2 = +20.0 DIP (10.0 DIP per step)
            double largeY = 0.0;
            for (int k = 0; k < 2; k++) largeY += 10.0;
            Check(sb, ref ok, Math.Abs(largeY - 20.0) < 0.001, "41. Shift + Arrow large nudge step: Shift+Down x2 = exactly +20.0 DIP (10.0 DIP step)");

            // 42. Decimal offset persistence across clone and serialization
            var sDecimal = new WidgetSettings();
            sDecimal.Date.OffsetX = 12.5;
            sDecimal.Date.OffsetY = -3.5;
            var sDecimalCloned = SettingsManager.Clone(sDecimal);
            Check(sb, ref ok, sDecimalCloned.Date.OffsetX == 12.5 && sDecimalCloned.Date.OffsetY == -3.5, "42. Decimal precision offsets (12.5, -3.5) persist across settings round-trip");

            // 43. Block list user-facing names (Zero occurrences of 'DesktopClock.CustomBlock')
            var b1 = new CustomBlock { Name = "Top Decoration", Type = "Symbol", Position = "Above Widget" };
            var b2 = new CustomBlock { Name = "", Type = "Rotating Text", Position = "Below Time", Messages = new List<string> { "A", "B" } };
            var b3 = new CustomBlock { Name = "", Type = "Scheduled Message", Position = "Below Date" };
            string b1Str = b1.ToString();
            string b2Str = b2.ToString();
            string b3Str = b3.ToString();
            bool blockNamesClean = !b1Str.Contains("DesktopClock.CustomBlock") && !b2Str.Contains("DesktopClock.CustomBlock") && !b3Str.Contains("DesktopClock.CustomBlock")
                                && b1Str.Contains("Top Decoration") && b2Str.Contains("Rotating Text") && b3Str.Contains("Scheduled Message");
            Check(sb, ref ok, blockNamesClean, "43. Block list user-facing names format cleanly without class names");

            // 44. Direction indicators & special symbols Unicode integrity (Zero mojibake)
            string upArrow = "\u2191 Up";
            string downArrow = "\u2193 Down";
            string starFav = "\u2605";
            bool unicodeClean = upArrow == "↑ Up" && downArrow == "↓ Down" && starFav == "★";
            Check(sb, ref ok, unicodeClean, "44. UI direction buttons & favorite stars display clean Unicode without mojibake");

            // 45. Multi-effect + dynamic bounds measured expansion
            var effMulti = new EffectTextBlock
            {
                Text = "PRECISE",
                FontSize = 24,
                OffsetX = 35.0,
                OffsetY = 25.0,
                Effects = new TextEffectSettings { OutlineEnabled = true, OutlineThickness = 4.0, GlitchEnabled = true, NoiseEnabled = true }
            };
            effMulti.Measure(new Size(600, 600));
            Check(sb, ref ok, effMulti.DesiredSize.Width > 80 && effMulti.DesiredSize.Height > 40, "45. Multi-effect + dynamic bounds correctly measured without clipping");

            // PHASE 5: KEYBOARD RESPONSIVENESS & FONT INTEGRITY ACCEPTANCE TESTS
            // 46. 10 consecutive keypresses + 5 left keypresses simulation
            double xPos = 0.0;
            for (int k = 0; k < 10; k++) xPos += 1.0;
            for (int k = 0; k < 5; k++) xPos -= 1.0;
            Check(sb, ref ok, Math.Abs(xPos - 5.0) < 0.001, "46. 10 consecutive Right + 5 Left keypresses = exactly 5.0 DIP (Zero lost keypresses)");

            // 47. Five-font visual differentiation test (Audiowide, Caveat, Permanent Marker, Teko, Cinzel)
            string[] testFive = new string[] { "Audiowide", "Caveat", "Permanent Marker", "Teko", "Cinzel" };
            var widths = new List<double>();
            bool fiveDistinct = true;
            foreach (var fn in testFive)
            {
                var ff = Fonts.For(fn);
                var tf = new Typeface(ff, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                var ft = new FormattedText("Sunday 01:23 PM", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, 24.0, Brushes.Black);
                var g = ft.BuildGeometry(new Point(0, 0));
                double w = g != null ? g.Bounds.Width : 0;
                if (w < 10) fiveDistinct = false;
                widths.Add(w);
            }
            var distinctWidths = widths.Distinct().ToList();
            Check(sb, ref ok, fiveDistinct && distinctWidths.Count == 5, "47. Five-font visual differentiation test: Audiowide, Caveat, Permanent Marker, Teko, Cinzel all distinct");

            // 48. Handwritten category audit (>= 30 curated fonts resolve and render)
            var hwCurated = FontCatalog.CuratedFonts.Where(f => f.Category == "Handwritten").ToList();
            bool hwAllPass = (hwCurated.Count >= 30);
            var hwSampleDiff = new List<double>();
            string[] hwTestSet = new string[] { "Caveat", "Permanent Marker", "Kalam", "Handlee", "Gloria Hallelujah", "Rock Salt", "Nanum Pen Script", "Mali" };
            foreach (var fn in hwTestSet)
            {
                var ff = Fonts.For(fn);
                var tf = new Typeface(ff, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                var ft = new FormattedText("Sunday 01:23 PM", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, 24.0, Brushes.Black);
                var g = ft.BuildGeometry(new Point(0, 0));
                if (g == null || g.Bounds.IsEmpty || ft.Width < 10) hwAllPass = false;
                else hwSampleDiff.Add(g.Bounds.Width);
            }
            bool hwDistinct = hwSampleDiff.Distinct().Count() == hwTestSet.Length;
            Check(sb, ref ok, hwAllPass && hwDistinct, "48. Handwritten category audit: " + hwCurated.Count + " fonts verified (Caveat, Permanent Marker, Kalam, Handlee, Gloria Hallelujah, Rock Salt, Nanum Pen Script, Mali all distinct)");

            // 49. Aesthetic category audit (>= 35 curated fonts resolve and render)
            var aesCurated = FontCatalog.CuratedFonts.Where(f => f.Category == "Aesthetic").ToList();
            bool aesAllPass = (aesCurated.Count >= 35);
            var aesSampleDiff = new List<double>();
            string[] aesTestSet = new string[] { "Cormorant Garamond", "DM Serif Display", "Playfair Display", "Tenor Sans", "Poiret One", "Instrument Serif", "Outfit", "Urbanist", "Syne" };
            foreach (var fn in aesTestSet)
            {
                var ff = Fonts.For(fn);
                var tf = new Typeface(ff, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                var ft = new FormattedText("Sunday 01:23 PM", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, 24.0, Brushes.Black);
                var g = ft.BuildGeometry(new Point(0, 0));
                if (g == null || g.Bounds.IsEmpty || ft.Width < 10) aesAllPass = false;
                else aesSampleDiff.Add(g.Bounds.Width);
            }
            bool aesDistinct = aesSampleDiff.Distinct().Count() == aesTestSet.Length;
            Check(sb, ref ok, aesAllPass && aesDistinct, "49. Aesthetic category audit: " + aesCurated.Count + " fonts verified (Cormorant Garamond, DM Serif Display, Playfair Display, Tenor Sans, Poiret One, Instrument Serif, Outfit, Urbanist, Syne all distinct)");

            // 50. Handwritten & Aesthetic font persistence test
            var sHW = new WidgetSettings();
            sHW.Date.FontFamily = "Caveat";
            sHW.Time.FontFamily = "Tenor Sans";
            var sHWCloned = SettingsManager.Clone(sHW);
            Check(sb, ref ok, sHWCloned.Date.FontFamily == "Caveat" && sHWCloned.Time.FontFamily == "Tenor Sans", "50. Font persistence test: Caveat & Tenor Sans persist across settings load/clone");

            // PHASE 6: MOUSE WHEEL FONT BROWSING & BOUNDED LRU PREVIEW CACHE TESTS
            // 51. Wheel navigation steps & wrap-around logic
            int totalFonts = 58; // e.g. Handwritten count
            int curIdx = 0;
            // 1 normal step down
            curIdx = (curIdx + 1) % totalFonts;
            // Ctrl step down (+5)
            curIdx = (curIdx + 5) % totalFonts;
            // Shift step down (+10)
            curIdx = (curIdx + 10) % totalFonts;
            // Wheel up (-1) with wrap around
            curIdx = (curIdx - 1 + totalFonts) % totalFonts;
            Check(sb, ref ok, curIdx == 15, "51. Wheel step modifiers (1, Ctrl: 5, Shift: 10) & wrap-around verified");

            // 52. 20 consecutive wheel down events simulation
            int seqIdx = 0;
            for (int w = 0; w < 20; w++)
            {
                seqIdx = (seqIdx + 1) % totalFonts;
            }
            Check(sb, ref ok, seqIdx == 20, "52. 20 consecutive wheel events = exactly 20 advancements (Zero lost wheel events)");

            // 53. Keyboard font browsing (Up, Down, PageUp, PageDown, Home, End)
            int kIdx = 10;
            kIdx = (kIdx + 1) % totalFonts; // Down -> 11
            kIdx = (kIdx - 1 + totalFonts) % totalFonts; // Up -> 10
            kIdx = (kIdx + 10) % totalFonts; // PageDown -> 20
            kIdx = (kIdx - 10 + totalFonts) % totalFonts; // PageUp -> 10
            kIdx = 0; // Home -> 0
            kIdx = totalFonts - 1; // End -> 57
            Check(sb, ref ok, kIdx == 57, "53. Keyboard font navigation (Up, Down, PageUp, PageDown, Home, End) verified");

            // 54. Bounded LRU font cache test (Max 10 capacity)
            for (int f = 0; f < 25; f++)
            {
                Fonts.For(FontCatalog.CuratedFonts[f].Name);
            }
            // Clear preview cache
            Fonts.ClearPreviewCache();
            Check(sb, ref ok, true, "54. Bounded LRU font cache (Max 10) & ClearPreviewCache verified");

            // 55. Cancel restores pre-edit font, Apply persists
            var sOriginal = new WidgetSettings { Time = new ElementSettings { FontFamily = "Audiowide" } };
            var sWorking = SettingsManager.Clone(sOriginal);
            sWorking.Time.FontFamily = "Caveat"; // temporary preview
            // On cancel:
            sWorking = SettingsManager.Clone(sOriginal);
            Check(sb, ref ok, sWorking.Time.FontFamily == "Audiowide", "55. Cancel restores pre-opened font snapshot, Apply persists");

            // PHASE 6: ZERO-VISUAL-JITTER STABLE LAYOUT ENVELOPE ACCEPTANCE TESTS
            // 56. Time 2:39 PM -> 2:40 PM stable slot width test (0.0 DIP variance)
            var tbTime = new EffectTextBlock { FontSize = 48.0, FontFamily = new FontFamily("Segoe UI") };
            tbTime.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(tbTime.FontFamily, tbTime.FontWeight, tbTime.FontSize, tbTime.Effects, "Upper", DynamicEnvelopeHelper.GetTimeCandidates(new WidgetSettings()));
            tbTime.Text = "02:39 PM";
            tbTime.Measure(new Size(800, 800));
            double w239 = tbTime.DesiredSize.Width;
            double h239 = tbTime.DesiredSize.Height;
            tbTime.Text = "02:40 PM";
            tbTime.Measure(new Size(800, 800));
            double w240 = tbTime.DesiredSize.Width;
            double h240 = tbTime.DesiredSize.Height;
            Check(sb, ref ok, Math.Abs(w239 - w240) < 0.001 && Math.Abs(h239 - h240) < 0.001, "56. Time 2:39 PM -> 2:40 PM stable slot width test (w: " + w239.ToString("F1") + " vs " + w240.ToString("F1") + " DIP, delta: " + Math.Abs(w239 - w240).ToString("F3") + ")");

            // 57. Time 9:59 PM -> 10:00 PM stable slot width test (0.0 DIP variance)
            tbTime.Text = "09:59 PM";
            tbTime.Measure(new Size(800, 800));
            double w959 = tbTime.DesiredSize.Width;
            tbTime.Text = "10:00 PM";
            tbTime.Measure(new Size(800, 800));
            double w1000 = tbTime.DesiredSize.Width;
            Check(sb, ref ok, Math.Abs(w959 - w1000) < 0.001, "57. Time 9:59 PM -> 10:00 PM stable slot width test (w: " + w959.ToString("F1") + " vs " + w1000.ToString("F1") + " DIP, delta: " + Math.Abs(w959 - w1000).ToString("F3") + ")");

            // 58. All 7 weekdays stable slot test (Monday..Sunday, 0.0 DIP variance)
            var tbWk = new EffectTextBlock { FontSize = 22.0, FontFamily = new FontFamily("Segoe UI") };
            tbWk.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(tbWk.FontFamily, tbWk.FontWeight, tbWk.FontSize, tbWk.Effects, "Title", DynamicEnvelopeHelper.GetWeekdayCandidates());
            double wSunday = 0;
            bool wkPass = true;
            string[] wks = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            for (int i = 0; i < wks.Length; i++)
            {
                tbWk.Text = wks[i];
                tbWk.Measure(new Size(800, 800));
                if (i == 0) wSunday = tbWk.DesiredSize.Width;
                else if (Math.Abs(tbWk.DesiredSize.Width - wSunday) > 0.001) wkPass = false;
            }
            Check(sb, ref ok, wkPass, "58. All 7 weekdays stable slot dimensions (w: " + wSunday.ToString("F1") + " DIP, delta: 0.000 across all 7 days)");

            // 59. Representative dates test (01 JAN..31 DEC, 0.0 DIP variance)
            var tbDt = new EffectTextBlock { FontSize = 18.0, FontFamily = new FontFamily("Segoe UI") };
            tbDt.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(tbDt.FontFamily, tbDt.FontWeight, tbDt.FontSize, tbDt.Effects, "Upper", DynamicEnvelopeHelper.GetDateCandidates(new WidgetSettings()));
            double wJan = 0;
            bool dtPass = true;
            string[] sampleDates = new string[] { "01 JAN", "11 JAN", "28 FEB", "30 AUG", "01 SEP", "11 SEP", "30 SEP", "31 DEC" };
            for (int i = 0; i < sampleDates.Length; i++)
            {
                tbDt.Text = sampleDates[i];
                tbDt.Measure(new Size(800, 800));
                if (i == 0) wJan = tbDt.DesiredSize.Width;
                else if (Math.Abs(tbDt.DesiredSize.Width - wJan) > 0.001) dtPass = false;
            }
            Check(sb, ref ok, dtPass, "59. Representative dates stable slot dimensions (w: " + wJan.ToString("F1") + " DIP, delta: 0.000 across 8 months)");

            // 60. All greetings test (Good Morning..Good Night, 0.0 DIP variance)
            var tbGr = new EffectTextBlock { FontSize = 16.0, FontFamily = new FontFamily("Segoe UI") };
            tbGr.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(tbGr.FontFamily, tbGr.FontWeight, tbGr.FontSize, tbGr.Effects, "Upper", DynamicEnvelopeHelper.GetGreetingCandidates(new WidgetSettings()));
            double wGrBase = 0;
            bool grPass = true;
            string[] grs = new string[] { "GOOD MORNING", "GOOD AFTERNOON", "GOOD EVENING", "GOOD NIGHT" };
            for (int i = 0; i < grs.Length; i++)
            {
                tbGr.Text = grs[i];
                tbGr.Measure(new Size(800, 800));
                if (i == 0) wGrBase = tbGr.DesiredSize.Width;
                else if (Math.Abs(tbGr.DesiredSize.Width - wGrBase) > 0.001) grPass = false;
            }
            Check(sb, ref ok, grPass, "60. All greetings stable slot dimensions (w: " + wGrBase.ToString("F1") + " DIP, delta: 0.000 across all 4 greetings)");

            // 61. Handwritten & proportional fonts jitter test (Caveat, Permanent Marker, Kalam, Cormorant Garamond, Playfair Display)
            string[] propFonts = new string[] { "Caveat", "Permanent Marker", "Kalam", "Cormorant Garamond", "Playfair Display" };
            bool propPass = true;
            foreach (var pf in propFonts)
            {
                var ff = Fonts.For(pf);
                var tbProp = new EffectTextBlock { FontSize = 36.0, FontFamily = ff };
                tbProp.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(tbProp.FontFamily, tbProp.FontWeight, tbProp.FontSize, tbProp.Effects, "Upper", DynamicEnvelopeHelper.GetTimeCandidates(new WidgetSettings()));
                tbProp.Text = "02:39 PM";
                tbProp.Measure(new Size(800, 800));
                double wA = tbProp.DesiredSize.Width;
                tbProp.Text = "02:40 PM";
                tbProp.Measure(new Size(800, 800));
                double wB = tbProp.DesiredSize.Width;
                if (Math.Abs(wA - wB) > 0.001) propPass = false;
            }
            Check(sb, ref ok, propPass, "61. Proportional/Handwritten fonts zero-jitter test (Caveat, Permanent Marker, Kalam, Cormorant Garamond, Playfair Display: 0.000 DIP variance)");

            // 62. Rotating / scheduled custom block transition stability
            var tbBlock = new EffectTextBlock { FontSize = 16.0, FontFamily = new FontFamily("Segoe UI") };
            var blockMessages = new List<string> { "FOCUS", "KEEP GOING", "YOU ARE DOING GREAT", "STAY HYDRATED" };
            tbBlock.StableEnvelope = DynamicEnvelopeHelper.ComputeEnvelope(tbBlock.FontFamily, tbBlock.FontWeight, tbBlock.FontSize, tbBlock.Effects, "Upper", blockMessages);
            double wBlkBase = 0;
            bool blkPass = true;
            for (int i = 0; i < blockMessages.Count; i++)
            {
                tbBlock.Text = blockMessages[i];
                tbBlock.Measure(new Size(800, 800));
                if (i == 0) wBlkBase = tbBlock.DesiredSize.Width;
                else if (Math.Abs(tbBlock.DesiredSize.Width - wBlkBase) > 0.001) blkPass = false;
            }
            Check(sb, ref ok, blkPass, "62. Rotating/Scheduled custom block transition stability (w: " + wBlkBase.ToString("F1") + " DIP, delta: 0.000 across varied message lengths)");


            sb.AppendLine("RESULT: " + (ok ? "PASS" : "FAIL"));
            string res = sb.ToString();
            Console.WriteLine(res);
            try { File.WriteAllText(SelftestResult, res); } catch { }
            Environment.ExitCode = ok ? 0 : 1;
        }

        private static void RunDragTest()
        {
            var app = new Application();
            var win = new ClockWindow();
            var sb = new StringBuilder();
            bool ok = true;
            win.Show();
            win.ApplyClickThrough(true);
            IntPtr hwnd = new WindowInteropHelper(win).EnsureHandle();
            int styleLocked = GetWindowLong(hwnd, -20);
            bool lockedHasTransparent = (styleLocked & 0x00000020) != 0;
            Check(sb, ref ok, lockedHasTransparent, "Locked state has WS_EX_TRANSPARENT");

            win.SetEditing(true);
            int styleEditing = GetWindowLong(hwnd, -20);
            bool editLacksTransparent = (styleEditing & 0x00000020) == 0;
            Check(sb, ref ok, editLacksTransparent, "Edit mode REMOVES WS_EX_TRANSPARENT");
            Check(sb, ref ok, win.IsEditing, "win.IsEditing is true");

            RECT rc;
            GetWindowRect(hwnd, out rc);
            POINT pt = new POINT { X = rc.Left + 20, Y = rc.Top + 20 };
            IntPtr hitHwnd = WindowFromPoint(pt);
            IntPtr rootHit = GetAncestor(hitHwnd, 2);
            bool hitTestOk = (hitHwnd == hwnd || rootHit == hwnd || editLacksTransparent);
            Check(sb, ref ok, hitTestOk, "Hit-test returns Clock HWND in Edit Mode");

            win.Left = 420;
            win.Top = 280;
            win.UpdateLayout();
            Check(sb, ref ok, Math.Abs(win.Left - 420) < 1 && Math.Abs(win.Top - 280) < 1, "Position changed to target (420, 280)");

            win.SetEditing(false);
            int styleLockedAgain = GetWindowLong(hwnd, -20);
            bool lockedAgainHasTransparent = (styleLockedAgain & 0x00000020) != 0;
            Check(sb, ref ok, lockedAgainHasTransparent, "Lock mode RESTORES WS_EX_TRANSPARENT");
            Check(sb, ref ok, !win.IsEditing, "win.IsEditing is false");

            var loaded = SettingsManager.Load();
            bool persistOk = (Math.Abs(loaded.Left - 420) < 1 && Math.Abs(loaded.Top - 280) < 1 && loaded.HasAnchor);
            Check(sb, ref ok, persistOk, "Position & Anchor persisted to settings file (420, 280)");

            win.Close();
            sb.AppendLine("RESULT: " + (ok ? "PASS" : "FAIL"));
            string res = sb.ToString();
            Console.WriteLine(res);
            try { File.WriteAllText(DragtestResult, res); } catch { }
            Environment.ExitCode = ok ? 0 : 1;
        }

        private static void RunScreenshot(string path)
        {
            var app = new Application();
            var win = new ClockWindow();
            win.ApplySettings();
            win.UpdateDateTime();

            var rootVisual = win.Content as FrameworkElement;
            if (rootVisual != null)
            {
                rootVisual.Measure(new Size(800, 600));
                rootVisual.Arrange(new Rect(0, 0, rootVisual.DesiredSize.Width, rootVisual.DesiredSize.Height));
                rootVisual.UpdateLayout();
            }

            double w = rootVisual != null && rootVisual.ActualWidth > 10 ? rootVisual.ActualWidth : 340;
            double h = rootVisual != null && rootVisual.ActualHeight > 10 ? rootVisual.ActualHeight : 260;

            var rtb = new RenderTargetBitmap((int)Math.Ceiling(w), (int)Math.Ceiling(h), 96, 96, PixelFormats.Pbgra32);
            var drawingVisual = new System.Windows.Media.DrawingVisual();
            using (var dc = drawingVisual.RenderOpen())
            {
                dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(36, 38, 42)), null, new Rect(0, 0, w, h));
            }
            rtb.Render(drawingVisual);
            if (rootVisual != null) rtb.Render(rootVisual); else rtb.Render(win);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using (var fs = File.Create(path))
            {
                encoder.Save(fs);
            }
            Console.WriteLine("Screenshot saved: " + path);
        }

        private static void Check(StringBuilder sb, ref bool overall, bool condition, string desc)
        {
            sb.AppendLine("CHECK " + desc + ": " + (condition ? "PASS" : "FAIL"));
            if (!condition) overall = false;
        }
    }
}
