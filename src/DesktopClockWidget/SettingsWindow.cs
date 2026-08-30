using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using Color = System.Windows.Media.Color;
using ComboBox = System.Windows.Controls.ComboBox;
using GroupBox = System.Windows.Controls.GroupBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListBox = System.Windows.Controls.ListBox;
using MessageBox = System.Windows.MessageBox;
using Orientation = System.Windows.Controls.Orientation;
using Rectangle = System.Windows.Shapes.Rectangle;
using TabControl = System.Windows.Controls.TabControl;
using TextBox = System.Windows.Controls.TextBox;

namespace DesktopClock
{
    public class SettingsWindow : Window
    {
        private readonly ISettingsHost _host;
        private WidgetSettings _preview;
        private readonly WidgetSettings _original;
        private bool _applied = false;
        private bool _isUpdatingUi = false;

        // General Tab
        private CheckBox _chkUseGlobalFont;
        private ComboBox _cmbGlobalFont;
        private CheckBox _chkUseGlobalColor;
        private Rectangle _rectGlobalColorSwatch;
        private TextBlock _lblGlobalColorHex;
        private Slider _sliderMasterScale;
        private TextBlock _lblMasterScale;
        private ComboBox _cmbGreetingMode;
        private TextBox _txtCustomGreeting;
        private TextBox _txtMorningStart;
        private TextBox _txtAfternoonStart;
        private TextBox _txtEveningStart;
        private TextBox _txtNightStart;

        // Core Elements Tab
        private ComboBox _cmbCoreElementSelector;
        private CheckBox _chkCoreElemVisible;
        private ComboBox _cmbCoreElemAlign;
        private TextBox _txtCoreElemOffsetX;
        private Slider _sliderCoreElemOffsetX;
        private TextBox _txtCoreElemOffsetY;
        private Slider _sliderCoreElemOffsetY;
        private ComboBox _cmbCoreElemSource;
        private ComboBox _cmbCoreElemCategory;
        private TextBox _txtCoreElemFontSearch;
        private ComboBox _cmbCoreElemFont;
        private Button _btnCoreElemFontFav;
        private TextBlock _lblCoreElemFontMeta;
        private TextBlock _lblCoreElemFontPreview;
        private ComboBox _cmbCoreElemWeight;
        private TextBox _txtCoreElemFontSize;
        private Slider _sliderCoreElemFontSize;
        private TextBlock _lblCoreElemColorHex;
        private Rectangle _rectCoreElemColorSwatch;
        private Slider _sliderCoreElemOpacity;
        private TextBlock _lblCoreElemOpacity;
        private ComboBox _cmbCoreElemCase;

        // Core Element Effects
        private ComboBox _cmbCoreElemEffectPreset;
        private CheckBox _chkCoreElemOutline;
        private Rectangle _rectCoreElemOutlineSwatch;
        private TextBlock _lblCoreElemOutlineHex;
        private Slider _sliderCoreElemOutlineThick;
        private TextBlock _lblCoreElemOutlineThick;
        private Slider _sliderCoreElemOutlineOpacity;
        private TextBlock _lblCoreElemOutlineOpacity;

        private CheckBox _chkCoreElemGlitch;
        private Slider _sliderCoreElemGlitchInt;
        private TextBlock _lblCoreElemGlitchInt;
        private ComboBox _cmbCoreElemGlitchSpeed;
        private Rectangle _rectCoreElemGlitchC1Swatch;
        private TextBlock _lblCoreElemGlitchC1Hex;
        private Rectangle _rectCoreElemGlitchC2Swatch;
        private TextBlock _lblCoreElemGlitchC2Hex;

        private CheckBox _chkCoreElemNoise;
        private Slider _sliderCoreElemNoiseAmt;
        private TextBlock _lblCoreElemNoiseAmt;
        private ComboBox _cmbCoreElemNoiseSpeed;

        // Custom Blocks Tab
        private ListBox _lstBlocks;
        private Button _btnAddBlock;
        private Button _btnDupBlock;
        private Button _btnDelBlock;
        private Button _btnMoveUpBlock;
        private Button _btnMoveDownBlock;

        private Border _blockInspectorPanel;
        private CheckBox _chkBlockEnabled;
        private TextBox _txtBlockName;
        private ComboBox _cmbBlockType;
        private ComboBox _cmbBlockPosition;
        private TextBox _txtBlockOrder;

        // Block Content: Symbol
        private StackPanel _panelBlockSymbol;
        private ComboBox _cmbBlockPresetSymbol;
        private TextBox _txtBlockCustomSymbol;

        // Block Content: Static
        private StackPanel _panelBlockStatic;
        private TextBox _txtBlockStaticText;

        // Block Content: Rotating
        private StackPanel _panelBlockRotating;
        private ComboBox _cmbBlockRotationMode;
        private StackPanel _panelBlockInterval;
        private TextBox _txtBlockIntervalValue;
        private ComboBox _cmbBlockIntervalUnit;
        private ListBox _lstBlockMessages;
        private Button _btnAddBlockMsg;
        private Button _btnDelBlockMsg;
        private StackPanel _panelBlockSchedule;
        private ListBox _lstBlockSchedules;
        private Button _btnAddSchedule;
        private Button _btnDelSchedule;

        // Block Appearance
        private ComboBox _cmbBlockFont;
        private Button _btnBlockFontFav;
        private TextBlock _lblBlockFontMeta;
        private TextBlock _lblBlockFontPreview;
        private ComboBox _cmbBlockFontWeight;
        private TextBox _txtBlockFontSize;
        private Slider _sliderBlockFontSize;
        private TextBlock _lblBlockColorHex;
        private Rectangle _rectBlockColorSwatch;
        private Slider _sliderBlockOpacity;
        private TextBlock _lblBlockOpacity;
        private ComboBox _cmbBlockAlignment;
        private TextBox _txtBlockOffsetX;
        private Slider _sliderBlockOffsetX;
        private TextBox _txtBlockOffsetY;
        private Slider _sliderBlockOffsetY;
        private ComboBox _cmbBlockCase;
        private CheckBox _chkBlockItalic;
        private CheckBox _chkBlockUnderline;

        // Block Effects
        private ComboBox _cmbBlockEffectPreset;
        private CheckBox _chkBlockOutline;
        private Rectangle _rectBlockOutlineSwatch;
        private TextBlock _lblBlockOutlineHex;
        private Slider _sliderBlockOutlineThick;
        private TextBlock _lblBlockOutlineThick;
        private Slider _sliderBlockOutlineOpacity;
        private TextBlock _lblBlockOutlineOpacity;

        private CheckBox _chkBlockGlitch;
        private Slider _sliderBlockGlitchInt;
        private TextBlock _lblBlockGlitchInt;
        private ComboBox _cmbBlockGlitchSpeed;
        private Rectangle _rectBlockGlitchC1Swatch;
        private TextBlock _lblBlockGlitchC1Hex;
        private Rectangle _rectBlockGlitchC2Swatch;
        private TextBlock _lblBlockGlitchC2Hex;

        private CheckBox _chkBlockNoise;
        private Slider _sliderBlockNoiseAmt;
        private TextBlock _lblBlockNoiseAmt;
        private ComboBox _cmbBlockNoiseSpeed;

        // Font Catalog Tab
        private ComboBox _cmbCatalogSource;
        private ComboBox _cmbCatalogCategory;
        private TextBox _txtCatalogSearch;
        private ListBox _lstCatalogFonts;
        private TextBlock _lblCatalogFontMeta;
        private TextBlock _lblCatalogSample;
        private TextBlock _lblCatalogStats;

        // Position Tab
        private Button _btnEditPos;
        private Button _btnLockPos;
        private Button _btnCenter;
        private CheckBox _chkRunOnStartup;
        private TextBlock _lblCoordinates;

        public SettingsWindow(ISettingsHost host, WidgetSettings currentSettings)
        {
            _host = host;
            _original = SettingsManager.Clone(currentSettings);
            _preview = SettingsManager.Clone(currentSettings);

            Title = "Desktop Clock Settings";
            Width = 660;
            Height = 720;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = new SolidColorBrush(Color.FromRgb(32, 34, 37));
            Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            ResizeMode = ResizeMode.CanResize;
            MinWidth = 580;
            MinHeight = 560;

            InitializeComponents();
            LoadValues();
        }

        private void InitializeComponents()
        {
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var tabs = new TabControl
            {
                Background = new SolidColorBrush(Color.FromRgb(36, 38, 42)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 52, 58)),
                Margin = new Thickness(10)
            };

            var tabGeneral = new TabItem { Header = "  GENERAL  " };
            tabGeneral.Content = CreateGeneralTab();
            tabs.Items.Add(tabGeneral);

            var tabCore = new TabItem { Header = "  CORE ELEMENTS  " };
            tabCore.Content = CreateCoreElementsTab();
            tabs.Items.Add(tabCore);

            var tabBlocks = new TabItem { Header = "  CUSTOM BLOCKS  " };
            tabBlocks.Content = CreateBlocksTab();
            tabs.Items.Add(tabBlocks);

            var tabFonts = new TabItem { Header = "  FONT CATALOG  " };
            tabFonts.Content = CreateFontCatalogTab();
            tabs.Items.Add(tabFonts);

            var tabPos = new TabItem { Header = "  POSITION  " };
            tabPos.Content = CreatePositionTab();
            tabs.Items.Add(tabPos);

            Grid.SetRow(tabs, 0);
            mainGrid.Children.Add(tabs);

            var bottomBar = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 0, 10, 10)
            };

            var btnApply = CreateStyledButton("Apply", 90);
            btnApply.Click += (s, e) =>
            {
                _applied = true;
                _host.CommitSettings(_preview);
            };

            var btnOk = CreateStyledButton("OK", 80);
            btnOk.Click += (s, e) =>
            {
                _applied = true;
                _host.CommitSettings(_preview);
                Close();
            };

            var btnCancel = CreateStyledButton("Cancel", 80);
            btnCancel.Click += (s, e) =>
            {
                if (!_applied) _host.ApplyPreview(_original);
                Close();
            };

            bottomBar.Children.Add(btnApply);
            bottomBar.Children.Add(btnOk);
            bottomBar.Children.Add(btnCancel);

            Grid.SetRow(bottomBar, 1);
            mainGrid.Children.Add(bottomBar);

            Content = mainGrid;
            Closing += (s, e) =>
            {
                if (!_applied) _host.ApplyPreview(_original);
            };
        }

        private UIElement CreateGeneralTab()
        {
            var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            var root = new StackPanel { Margin = new Thickness(12) };

            var grpFont = CreateGroupBox("Global Font Override");
            var stackFont = new StackPanel { Margin = new Thickness(8) };
            _chkUseGlobalFont = new CheckBox { Content = "Override all text elements with Global Font", FontWeight = FontWeights.Medium, Margin = new Thickness(0, 0, 0, 8) };
            _chkUseGlobalFont.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                _preview.UseGlobalFont = _chkUseGlobalFont.IsChecked == true;
                _cmbGlobalFont.IsEnabled = _preview.UseGlobalFont;
                ApplyPreviewLive();
            };
            stackFont.Children.Add(_chkUseGlobalFont);

            var fontRow = new StackPanel { Orientation = Orientation.Horizontal };
            fontRow.Children.Add(new TextBlock { Text = "Global Font:", Width = 90, VerticalAlignment = VerticalAlignment.Center });
            _cmbGlobalFont = CreateComboBox(200);
            foreach (var f in FontCatalog.GetAllFonts()) _cmbGlobalFont.Items.Add(f.Name);
            _cmbGlobalFont.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbGlobalFont.SelectedItem == null) return;
                _preview.GlobalFont = _cmbGlobalFont.SelectedItem.ToString();
                ApplyPreviewLive();
            };
            fontRow.Children.Add(_cmbGlobalFont);
            stackFont.Children.Add(fontRow);
            grpFont.Content = stackFont;
            root.Children.Add(grpFont);

            var grpColor = CreateGroupBox("Global Color Override");
            var stackColor = new StackPanel { Margin = new Thickness(8) };
            _chkUseGlobalColor = new CheckBox { Content = "Override all text colors with Global Color", FontWeight = FontWeights.Medium, Margin = new Thickness(0, 0, 0, 8) };
            _chkUseGlobalColor.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                _preview.UseGlobalColor = _chkUseGlobalColor.IsChecked == true;
                ApplyPreviewLive();
            };
            stackColor.Children.Add(_chkUseGlobalColor);

            var colorRow = new StackPanel { Orientation = Orientation.Horizontal };
            colorRow.Children.Add(new TextBlock { Text = "Global Color:", Width = 90, VerticalAlignment = VerticalAlignment.Center });
            _rectGlobalColorSwatch = new Rectangle { Width = 24, Height = 24, Stroke = Brushes.Gray, StrokeThickness = 1, Margin = new Thickness(0, 0, 8, 0) };
            colorRow.Children.Add(_rectGlobalColorSwatch);
            _lblGlobalColorHex = new TextBlock { Text = "#D6D3D0", Width = 75, VerticalAlignment = VerticalAlignment.Center };
            colorRow.Children.Add(_lblGlobalColorHex);

            var btnPickGlobalColor = CreateStyledButton("Choose...", 80);
            btnPickGlobalColor.Click += (s, e) =>
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string hex = string.Format("#{0:X2}{1:X2}{2:X2}", dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    _preview.GlobalColor = hex;
                    _lblGlobalColorHex.Text = hex;
                    _rectGlobalColorSwatch.Fill = new SolidColorBrush(ParseColor(hex));
                    ApplyPreviewLive();
                }
            };
            colorRow.Children.Add(btnPickGlobalColor);
            stackColor.Children.Add(colorRow);
            grpColor.Content = stackColor;
            root.Children.Add(grpColor);

            var grpScale = CreateGroupBox("Master Scale");
            var stackScale = new StackPanel { Margin = new Thickness(8) };
            var scaleRow = new StackPanel { Orientation = Orientation.Horizontal };
            scaleRow.Children.Add(new TextBlock { Text = "Scale:", Width = 90, VerticalAlignment = VerticalAlignment.Center });
            _sliderMasterScale = new Slider { Minimum = 40, Maximum = 300, Value = 100, Width = 220, VerticalAlignment = VerticalAlignment.Center };
            _sliderMasterScale.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                _preview.Scale = Math.Round(_sliderMasterScale.Value) / 100.0;
                _lblMasterScale.Text = ((int)Math.Round(_sliderMasterScale.Value)) + "%";
                ApplyPreviewLive();
            };
            scaleRow.Children.Add(_sliderMasterScale);
            _lblMasterScale = new TextBlock { Text = "100%", Width = 50, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            scaleRow.Children.Add(_lblMasterScale);
            stackScale.Children.Add(scaleRow);
            grpScale.Content = stackScale;
            root.Children.Add(grpScale);

            var grpGreeting = CreateGroupBox("Dynamic Greeting Schedule");
            var stackGreeting = new StackPanel { Margin = new Thickness(8) };

            var greetRow1 = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            greetRow1.Children.Add(new TextBlock { Text = "Mode:", Width = 90, VerticalAlignment = VerticalAlignment.Center });
            _cmbGreetingMode = CreateComboBox(160);
            _cmbGreetingMode.Items.Add("Auto Schedule");
            _cmbGreetingMode.Items.Add("Custom Text");
            _cmbGreetingMode.Items.Add("Hidden");
            _cmbGreetingMode.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                _preview.GreetingMode = _cmbGreetingMode.SelectedIndex;
                _txtCustomGreeting.IsEnabled = (_preview.GreetingMode == 1);
                ApplyPreviewLive();
            };
            greetRow1.Children.Add(_cmbGreetingMode);
            stackGreeting.Children.Add(greetRow1);

            var customRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            customRow.Children.Add(new TextBlock { Text = "Custom Text:", Width = 90, VerticalAlignment = VerticalAlignment.Center });
            _txtCustomGreeting = CreateTextBox(160);
            _txtCustomGreeting.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                _preview.CustomGreeting = _txtCustomGreeting.Text;
                ApplyPreviewLive();
            };
            customRow.Children.Add(_txtCustomGreeting);
            stackGreeting.Children.Add(customRow);

            var hourRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 0) };
            hourRow.Children.Add(new TextBlock { Text = "Hours (0-23):", Width = 90, VerticalAlignment = VerticalAlignment.Center });
            hourRow.Children.Add(new TextBlock { Text = "Morn:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) });
            _txtMorningStart = CreateTextBox(32);
            hourRow.Children.Add(_txtMorningStart);
            hourRow.Children.Add(new TextBlock { Text = "Aft:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 4, 0) });
            _txtAfternoonStart = CreateTextBox(32);
            hourRow.Children.Add(_txtAfternoonStart);
            hourRow.Children.Add(new TextBlock { Text = "Eve:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 4, 0) });
            _txtEveningStart = CreateTextBox(32);
            hourRow.Children.Add(_txtEveningStart);
            hourRow.Children.Add(new TextBlock { Text = "Ngt:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 4, 0) });
            _txtNightStart = CreateTextBox(32);
            hourRow.Children.Add(_txtNightStart);

            _txtMorningStart.TextChanged += OnHourChanged;
            _txtAfternoonStart.TextChanged += OnHourChanged;
            _txtEveningStart.TextChanged += OnHourChanged;
            _txtNightStart.TextChanged += OnHourChanged;

            stackGreeting.Children.Add(hourRow);
            grpGreeting.Content = stackGreeting;
            root.Children.Add(grpGreeting);

            var btnResetAll = CreateStyledButton("Reset Appearance to Defaults", 220);
            btnResetAll.Margin = new Thickness(0, 12, 0, 0);
            btnResetAll.Click += (s, e) =>
            {
                if (MessageBox.Show("Reset all appearance settings and blocks to default?", "Confirm Reset", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var def = SettingsManager.Defaults();
                    _preview.Scale = def.Scale;
                    _preview.UseGlobalColor = false;
                    _preview.UseGlobalFont = false;
                    _preview.Greeting = def.Greeting;
                    _preview.Weekday = def.Weekday;
                    _preview.Time = def.Time;
                    _preview.Date = def.Date;
                    _preview.Blocks = def.Blocks;
                    LoadValues();
                    ApplyPreviewLive();
                }
            };
            root.Children.Add(btnResetAll);

            scroll.Content = root;
            return scroll;
        }

        private void OnHourChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUi) return;
            int m, a, ev, n;
            if (int.TryParse(_txtMorningStart.Text, out m)) _preview.MorningStart = m;
            if (int.TryParse(_txtAfternoonStart.Text, out a)) _preview.AfternoonStart = a;
            if (int.TryParse(_txtEveningStart.Text, out ev)) _preview.EveningStart = ev;
            if (int.TryParse(_txtNightStart.Text, out n)) _preview.NightStart = n;
            ApplyPreviewLive();
        }

        private UIElement CreateCoreElementsTab()
        {
            var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            var root = new StackPanel { Margin = new Thickness(12) };

            var selRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
            selRow.Children.Add(new TextBlock { Text = "Clock Element:", Width = 110, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center });
            _cmbCoreElementSelector = CreateComboBox(200);
            _cmbCoreElementSelector.Items.Add("GREETING");
            _cmbCoreElementSelector.Items.Add("WEEKDAY");
            _cmbCoreElementSelector.Items.Add("TIME (HERO)");
            _cmbCoreElementSelector.Items.Add("DATE");
            _cmbCoreElementSelector.SelectedIndex = 2;
            _cmbCoreElementSelector.SelectionChanged += (s, e) => LoadSelectedCoreElementValues();
            selRow.Children.Add(_cmbCoreElementSelector);
            root.Children.Add(selRow);

            var grp = CreateGroupBox("Typography & Colors");
            var stack = new StackPanel { Margin = new Thickness(8) };

            _chkCoreElemVisible = new CheckBox { Content = "Visible", FontWeight = FontWeights.Medium, Margin = new Thickness(0, 0, 0, 10) };
            _chkCoreElemVisible.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var elem = GetSelectedCoreElement();
                if (elem != null) { elem.Visible = _chkCoreElemVisible.IsChecked == true; ApplyPreviewLive(); }
            };
            stack.Children.Add(_chkCoreElemVisible);

            var srcRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            srcRow.Children.Add(new TextBlock { Text = "Source:", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _cmbCoreElemSource = CreateComboBox(150);
            foreach (var s in FontCatalog.Sources) _cmbCoreElemSource.Items.Add(s);
            _cmbCoreElemSource.SelectedIndex = 0;
            _cmbCoreElemSource.SelectionChanged += (s, e) => PopulateCoreFontList();
            srcRow.Children.Add(_cmbCoreElemSource);
            stack.Children.Add(srcRow);

            var catRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            catRow.Children.Add(new TextBlock { Text = "Category:", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _cmbCoreElemCategory = CreateComboBox(150);
            foreach (var c in FontCatalog.Categories) _cmbCoreElemCategory.Items.Add(c);
            _cmbCoreElemCategory.SelectedIndex = 0;
            _cmbCoreElemCategory.SelectionChanged += (s, e) => PopulateCoreFontList();
            catRow.Children.Add(_cmbCoreElemCategory);
            stack.Children.Add(catRow);

            var searchRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            searchRow.Children.Add(new TextBlock { Text = "Search Font:", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _txtCoreElemFontSearch = CreateTextBox(150);
            _txtCoreElemFontSearch.TextChanged += (s, e) => PopulateCoreFontList();
            searchRow.Children.Add(_txtCoreElemFontSearch);
            stack.Children.Add(searchRow);

            var fontRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
            fontRow.Children.Add(new TextBlock { Text = "Font Family:", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _cmbCoreElemFont = CreateComboBox(180);
            _cmbCoreElemFont.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbCoreElemFont.SelectedItem == null) return;
                var elem = GetSelectedCoreElement();
                if (elem != null)
                {
                    elem.FontFamily = _cmbCoreElemFont.SelectedItem.ToString();
                    UpdateCoreFontFavButton();
                    UpdateCoreFontPreview();
                    UpdateCoreFontMetadata();
                    ApplyPreviewLive();
                }
            };
            fontRow.Children.Add(_cmbCoreElemFont);

            _btnCoreElemFontFav = CreateStyledButton("â˜†", 36);
            _btnCoreElemFontFav.Click += (s, e) =>
            {
                var elem = GetSelectedCoreElement();
                if (elem == null || string.IsNullOrEmpty(elem.FontFamily)) return;
                if (_preview.FavoriteFonts.Contains(elem.FontFamily))
                    _preview.FavoriteFonts.Remove(elem.FontFamily);
                else
                    _preview.FavoriteFonts.Add(elem.FontFamily);
                UpdateCoreFontFavButton();
                PopulateCoreFontList();
            };
            fontRow.Children.Add(_btnCoreElemFontFav);
            stack.Children.Add(fontRow);

            _lblCoreElemFontMeta = new TextBlock
            {
                Text = "Source: App Font | Category: Futuristic",
                Foreground = Brushes.Gray,
                FontSize = 11,
                Margin = new Thickness(110, 0, 0, 6)
            };
            stack.Children.Add(_lblCoreElemFontMeta);

            _lblCoreElemFontPreview = new TextBlock
            {
                Text = "Saturday  09:26 PM  GOOD EVENING",
                FontSize = 14,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)),
                Padding = new Thickness(6),
                Margin = new Thickness(0, 0, 0, 10),
                TextAlignment = TextAlignment.Center
            };
            stack.Children.Add(_lblCoreElemFontPreview);

            var weightRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            weightRow.Children.Add(new TextBlock { Text = "Font Weight:", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _cmbCoreElemWeight = CreateComboBox(140);
            foreach (var w in Fonts.Weights) _cmbCoreElemWeight.Items.Add(w);
            _cmbCoreElemWeight.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbCoreElemWeight.SelectedItem == null) return;
                var elem = GetSelectedCoreElement();
                if (elem != null) { elem.FontWeight = _cmbCoreElemWeight.SelectedItem.ToString(); ApplyPreviewLive(); }
            };
            weightRow.Children.Add(_cmbCoreElemWeight);
            stack.Children.Add(weightRow);

            var szRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            szRow.Children.Add(new TextBlock { Text = "Font Size:", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _txtCoreElemFontSize = CreateTextBox(45);
            _txtCoreElemFontSize.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val;
                if (double.TryParse(_txtCoreElemFontSize.Text, out val) && val >= 6 && val <= 240)
                {
                    var elem = GetSelectedCoreElement();
                    if (elem != null) { elem.FontSize = val; _sliderCoreElemFontSize.Value = val; ApplyPreviewLive(); }
                }
            };
            szRow.Children.Add(_txtCoreElemFontSize);
            _sliderCoreElemFontSize = new Slider { Minimum = 8, Maximum = 160, Value = 40, Width = 160, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            _sliderCoreElemFontSize.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val = Math.Round(_sliderCoreElemFontSize.Value);
                var elem = GetSelectedCoreElement();
                if (elem != null) { elem.FontSize = val; _txtCoreElemFontSize.Text = val.ToString(); ApplyPreviewLive(); }
            };
            szRow.Children.Add(_sliderCoreElemFontSize);
            stack.Children.Add(szRow);

            var colorRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            colorRow.Children.Add(new TextBlock { Text = "Color:", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _rectCoreElemColorSwatch = new Rectangle { Width = 22, Height = 22, Stroke = Brushes.Gray, StrokeThickness = 1, Margin = new Thickness(0, 0, 8, 0) };
            colorRow.Children.Add(_rectCoreElemColorSwatch);
            _lblCoreElemColorHex = new TextBlock { Text = "#D6D3D0", Width = 70, VerticalAlignment = VerticalAlignment.Center };
            colorRow.Children.Add(_lblCoreElemColorHex);
            var btnColor = CreateStyledButton("Choose...", 75);
            btnColor.Click += (s, e) =>
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string hex = string.Format("#{0:X2}{1:X2}{2:X2}", dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    var elem = GetSelectedCoreElement();
                    if (elem != null)
                    {
                        elem.Color = hex;
                        _lblCoreElemColorHex.Text = hex;
                        _rectCoreElemColorSwatch.Fill = new SolidColorBrush(ParseColor(hex));
                        ApplyPreviewLive();
                    }
                }
            };
            colorRow.Children.Add(btnColor);
            stack.Children.Add(colorRow);

            var opRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            opRow.Children.Add(new TextBlock { Text = "Opacity:", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _sliderCoreElemOpacity = new Slider { Minimum = 0, Maximum = 100, Value = 100, Width = 160, VerticalAlignment = VerticalAlignment.Center };
            _sliderCoreElemOpacity.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var elem = GetSelectedCoreElement();
                if (elem != null)
                {
                    elem.Opacity = Math.Round(_sliderCoreElemOpacity.Value) / 100.0;
                    _lblCoreElemOpacity.Text = ((int)Math.Round(_sliderCoreElemOpacity.Value)) + "%";
                    ApplyPreviewLive();
                }
            };
            opRow.Children.Add(_sliderCoreElemOpacity);
            _lblCoreElemOpacity = new TextBlock { Text = "100%", Width = 45, Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            opRow.Children.Add(_lblCoreElemOpacity);
            stack.Children.Add(opRow);

            var caseRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
            caseRow.Children.Add(new TextBlock { Text = "Text Case:", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _cmbCoreElemCase = CreateComboBox(140);
            _cmbCoreElemCase.Items.Add("None");
            _cmbCoreElemCase.Items.Add("Title");
            _cmbCoreElemCase.Items.Add("Upper");
            _cmbCoreElemCase.Items.Add("Lower");
            _cmbCoreElemCase.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbCoreElemCase.SelectedItem == null) return;
                var elem = GetSelectedCoreElement();
                if (elem != null) { elem.Case = _cmbCoreElemCase.SelectedItem.ToString(); ApplyPreviewLive(); }
            };
            caseRow.Children.Add(_cmbCoreElemCase);
            stack.Children.Add(caseRow);

            grp.Content = stack;
            root.Children.Add(grp);

            // POSITION & ALIGNMENT SECTION
            var grpPos = CreateGroupBox("Position & Alignment");
            var stackPos = new StackPanel { Margin = new Thickness(8) };

            var alignRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            alignRow.Children.Add(new TextBlock { Text = "Horizontal:", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _cmbCoreElemAlign = CreateComboBox(140);
            _cmbCoreElemAlign.Items.Add("Left");
            _cmbCoreElemAlign.Items.Add("Center");
            _cmbCoreElemAlign.Items.Add("Right");
            _cmbCoreElemAlign.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbCoreElemAlign.SelectedItem == null) return;
                var elem = GetSelectedCoreElement();
                if (elem != null) { elem.HorizontalAlignment = _cmbCoreElemAlign.SelectedItem.ToString(); ApplyPreviewLive(); }
            };
            alignRow.Children.Add(_cmbCoreElemAlign);
            stackPos.Children.Add(alignRow);

            var xRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            xRow.Children.Add(new TextBlock { Text = "X Offset (DIP):", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _txtCoreElemOffsetX = CreateTextBox(45);
            _txtCoreElemOffsetX.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val;
                if (double.TryParse(_txtCoreElemOffsetX.Text, out val) && val >= -120 && val <= 120)
                {
                    var elem = GetSelectedCoreElement();
                    if (elem != null) { elem.OffsetX = val; _sliderCoreElemOffsetX.Value = val; ApplyPreviewLive(); }
                }
            };
            xRow.Children.Add(_txtCoreElemOffsetX);
            _sliderCoreElemOffsetX = new Slider { Minimum = -120, Maximum = 120, Value = 0, Width = 160, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            _sliderCoreElemOffsetX.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val = Math.Round(_sliderCoreElemOffsetX.Value);
                var elem = GetSelectedCoreElement();
                if (elem != null) { elem.OffsetX = val; _txtCoreElemOffsetX.Text = val.ToString(); ApplyPreviewLive(); }
            };
            xRow.Children.Add(_sliderCoreElemOffsetX);
            stackPos.Children.Add(xRow);

            var yRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            yRow.Children.Add(new TextBlock { Text = "Y Offset (DIP):", Width = 110, VerticalAlignment = VerticalAlignment.Center });
            _txtCoreElemOffsetY = CreateTextBox(45);
            _txtCoreElemOffsetY.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val;
                if (double.TryParse(_txtCoreElemOffsetY.Text, out val) && val >= -80 && val <= 80)
                {
                    var elem = GetSelectedCoreElement();
                    if (elem != null) { elem.OffsetY = val; _sliderCoreElemOffsetY.Value = val; ApplyPreviewLive(); }
                }
            };
            yRow.Children.Add(_txtCoreElemOffsetY);
            _sliderCoreElemOffsetY = new Slider { Minimum = -80, Maximum = 80, Value = 0, Width = 160, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            _sliderCoreElemOffsetY.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val = Math.Round(_sliderCoreElemOffsetY.Value);
                var elem = GetSelectedCoreElement();
                if (elem != null) { elem.OffsetY = val; _txtCoreElemOffsetY.Text = val.ToString(); ApplyPreviewLive(); }
            };
            yRow.Children.Add(_sliderCoreElemOffsetY);
            stackPos.Children.Add(yRow);

            var btnResetPos = CreateStyledButton("Reset Position", 120);
            btnResetPos.Margin = new Thickness(110, 4, 0, 0);
            btnResetPos.Click += (s, e) =>
            {
                var elem = GetSelectedCoreElement();
                if (elem != null)
                {
                    elem.HorizontalAlignment = "Center";
                    elem.OffsetX = 0.0;
                    elem.OffsetY = 0.0;
                    LoadSelectedCoreElementValues();
                    ApplyPreviewLive();
                }
            };
            stackPos.Children.Add(btnResetPos);

            grpPos.Content = stackPos;
            root.Children.Add(grpPos);

            // EFFECTS SECTION
            var grpFx = CreateGroupBox("Visual Effects (Outline / Glitch / Noise)");
            var stackFx = new StackPanel { Margin = new Thickness(8) };

            var presetRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
            presetRow.Children.Add(new TextBlock { Text = "Effect Preset:", Width = 110, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center });
            _cmbCoreElemEffectPreset = CreateComboBox(160);
            _cmbCoreElemEffectPreset.Items.Add("Custom");
            _cmbCoreElemEffectPreset.Items.Add("Clean (None)");
            _cmbCoreElemEffectPreset.Items.Add("Outlined");
            _cmbCoreElemEffectPreset.Items.Add("Cyber Glitch");
            _cmbCoreElemEffectPreset.Items.Add("Heavy Glitch");
            _cmbCoreElemEffectPreset.Items.Add("Subtle Noise");
            _cmbCoreElemEffectPreset.Items.Add("Digital Distortion");
            _cmbCoreElemEffectPreset.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbCoreElemEffectPreset.SelectedItem == null) return;
                ApplyPresetToElement(GetSelectedCoreElement(), _cmbCoreElemEffectPreset.SelectedItem.ToString());
                LoadSelectedCoreElementValues();
                ApplyPreviewLive();
            };
            presetRow.Children.Add(_cmbCoreElemEffectPreset);
            stackFx.Children.Add(presetRow);

            // Outline
            _chkCoreElemOutline = new CheckBox { Content = "Enable Outline (Glyph Contour Stroke)", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 4, 0, 6) };
            _chkCoreElemOutline.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var elem = GetSelectedCoreElement();
                if (elem != null && elem.Effects != null) { elem.Effects.OutlineEnabled = _chkCoreElemOutline.IsChecked == true; ApplyPreviewLive(); }
            };
            stackFx.Children.Add(_chkCoreElemOutline);

            var outColRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 0, 0, 6) };
            outColRow.Children.Add(new TextBlock { Text = "Outline Color:", Width = 95, VerticalAlignment = VerticalAlignment.Center });
            _rectCoreElemOutlineSwatch = new Rectangle { Width = 20, Height = 20, Stroke = Brushes.Gray, StrokeThickness = 1, Margin = new Thickness(0, 0, 6, 0) };
            outColRow.Children.Add(_rectCoreElemOutlineSwatch);
            _lblCoreElemOutlineHex = new TextBlock { Text = "#000000", Width = 65, VerticalAlignment = VerticalAlignment.Center };
            outColRow.Children.Add(_lblCoreElemOutlineHex);
            var btnOutCol = CreateStyledButton("Choose...", 70);
            btnOutCol.Click += (s, e) =>
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string hex = string.Format("#{0:X2}{1:X2}{2:X2}", dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    var elem = GetSelectedCoreElement();
                    if (elem != null && elem.Effects != null)
                    {
                        elem.Effects.OutlineColor = hex;
                        _lblCoreElemOutlineHex.Text = hex;
                        _rectCoreElemOutlineSwatch.Fill = new SolidColorBrush(ParseColor(hex));
                        ApplyPreviewLive();
                    }
                }
            };
            outColRow.Children.Add(btnOutCol);
            stackFx.Children.Add(outColRow);

            var outThickRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 0, 0, 6) };
            outThickRow.Children.Add(new TextBlock { Text = "Thickness:", Width = 95, VerticalAlignment = VerticalAlignment.Center });
            _sliderCoreElemOutlineThick = new Slider { Minimum = 5, Maximum = 80, Value = 20, Width = 140, VerticalAlignment = VerticalAlignment.Center };
            _sliderCoreElemOutlineThick.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val = Math.Round(_sliderCoreElemOutlineThick.Value) / 10.0;
                var elem = GetSelectedCoreElement();
                if (elem != null && elem.Effects != null)
                {
                    elem.Effects.OutlineThickness = val;
                    _lblCoreElemOutlineThick.Text = val.ToString("F1") + " DIP";
                    ApplyPreviewLive();
                }
            };
            outThickRow.Children.Add(_sliderCoreElemOutlineThick);
            _lblCoreElemOutlineThick = new TextBlock { Text = "2.0 DIP", Width = 55, Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            outThickRow.Children.Add(_lblCoreElemOutlineThick);
            stackFx.Children.Add(outThickRow);

            var outOpRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 0, 0, 10) };
            outOpRow.Children.Add(new TextBlock { Text = "Opacity:", Width = 95, VerticalAlignment = VerticalAlignment.Center });
            _sliderCoreElemOutlineOpacity = new Slider { Minimum = 0, Maximum = 100, Value = 100, Width = 140, VerticalAlignment = VerticalAlignment.Center };
            _sliderCoreElemOutlineOpacity.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val = Math.Round(_sliderCoreElemOutlineOpacity.Value) / 100.0;
                var elem = GetSelectedCoreElement();
                if (elem != null && elem.Effects != null)
                {
                    elem.Effects.OutlineOpacity = val;
                    _lblCoreElemOutlineOpacity.Text = ((int)Math.Round(_sliderCoreElemOutlineOpacity.Value)) + "%";
                    ApplyPreviewLive();
                }
            };
            outOpRow.Children.Add(_sliderCoreElemOutlineOpacity);
            _lblCoreElemOutlineOpacity = new TextBlock { Text = "100%", Width = 45, Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            outOpRow.Children.Add(_lblCoreElemOutlineOpacity);
            stackFx.Children.Add(outOpRow);

            // Glitch
            _chkCoreElemGlitch = new CheckBox { Content = "Enable Cyber Glitch (Ghost Channels & Displacement)", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 4, 0, 6) };
            _chkCoreElemGlitch.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var elem = GetSelectedCoreElement();
                if (elem != null && elem.Effects != null) { elem.Effects.GlitchEnabled = _chkCoreElemGlitch.IsChecked == true; ApplyPreviewLive(); }
            };
            stackFx.Children.Add(_chkCoreElemGlitch);

            var gIntRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 0, 0, 6) };
            gIntRow.Children.Add(new TextBlock { Text = "Intensity:", Width = 95, VerticalAlignment = VerticalAlignment.Center });
            _sliderCoreElemGlitchInt = new Slider { Minimum = 0, Maximum = 100, Value = 35, Width = 140, VerticalAlignment = VerticalAlignment.Center };
            _sliderCoreElemGlitchInt.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var elem = GetSelectedCoreElement();
                if (elem != null && elem.Effects != null)
                {
                    elem.Effects.GlitchIntensity = Math.Round(_sliderCoreElemGlitchInt.Value);
                    _lblCoreElemGlitchInt.Text = elem.Effects.GlitchIntensity + "%";
                    ApplyPreviewLive();
                }
            };
            gIntRow.Children.Add(_sliderCoreElemGlitchInt);
            _lblCoreElemGlitchInt = new TextBlock { Text = "35%", Width = 45, Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            gIntRow.Children.Add(_lblCoreElemGlitchInt);
            stackFx.Children.Add(gIntRow);

            var gSpeedRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 0, 0, 6) };
            gSpeedRow.Children.Add(new TextBlock { Text = "Speed:", Width = 95, VerticalAlignment = VerticalAlignment.Center });
            _cmbCoreElemGlitchSpeed = CreateComboBox(110);
            _cmbCoreElemGlitchSpeed.Items.Add("Slow");
            _cmbCoreElemGlitchSpeed.Items.Add("Medium");
            _cmbCoreElemGlitchSpeed.Items.Add("Fast");
            _cmbCoreElemGlitchSpeed.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbCoreElemGlitchSpeed.SelectedItem == null) return;
                var elem = GetSelectedCoreElement();
                if (elem != null && elem.Effects != null) { elem.Effects.GlitchSpeed = _cmbCoreElemGlitchSpeed.SelectedItem.ToString(); ApplyPreviewLive(); }
            };
            gSpeedRow.Children.Add(_cmbCoreElemGlitchSpeed);
            stackFx.Children.Add(gSpeedRow);

            var gGhostRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 0, 0, 10) };
            gGhostRow.Children.Add(new TextBlock { Text = "Ghost 1:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) });
            _rectCoreElemGlitchC1Swatch = new Rectangle { Width = 18, Height = 18, Stroke = Brushes.Gray, StrokeThickness = 1, Margin = new Thickness(0, 0, 4, 0) };
            gGhostRow.Children.Add(_rectCoreElemGlitchC1Swatch);
            _lblCoreElemGlitchC1Hex = new TextBlock { Text = "#00FFFF", Width = 55, VerticalAlignment = VerticalAlignment.Center };
            gGhostRow.Children.Add(_lblCoreElemGlitchC1Hex);
            var btnC1 = CreateStyledButton("...", 30);
            btnC1.Click += (s, e) =>
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string hex = string.Format("#{0:X2}{1:X2}{2:X2}", dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    var elem = GetSelectedCoreElement();
                    if (elem != null && elem.Effects != null)
                    {
                        elem.Effects.GlitchColor1 = hex;
                        _lblCoreElemGlitchC1Hex.Text = hex;
                        _rectCoreElemGlitchC1Swatch.Fill = new SolidColorBrush(ParseColor(hex));
                        ApplyPreviewLive();
                    }
                }
            };
            gGhostRow.Children.Add(btnC1);

            gGhostRow.Children.Add(new TextBlock { Text = "Ghost 2:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 4, 0) });
            _rectCoreElemGlitchC2Swatch = new Rectangle { Width = 18, Height = 18, Stroke = Brushes.Gray, StrokeThickness = 1, Margin = new Thickness(0, 0, 4, 0) };
            gGhostRow.Children.Add(_rectCoreElemGlitchC2Swatch);
            _lblCoreElemGlitchC2Hex = new TextBlock { Text = "#FF0055", Width = 55, VerticalAlignment = VerticalAlignment.Center };
            gGhostRow.Children.Add(_lblCoreElemGlitchC2Hex);
            var btnC2 = CreateStyledButton("...", 30);
            btnC2.Click += (s, e) =>
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string hex = string.Format("#{0:X2}{1:X2}{2:X2}", dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    var elem = GetSelectedCoreElement();
                    if (elem != null && elem.Effects != null)
                    {
                        elem.Effects.GlitchColor2 = hex;
                        _lblCoreElemGlitchC2Hex.Text = hex;
                        _rectCoreElemGlitchC2Swatch.Fill = new SolidColorBrush(ParseColor(hex));
                        ApplyPreviewLive();
                    }
                }
            };
            gGhostRow.Children.Add(btnC2);
            stackFx.Children.Add(gGhostRow);

            // Noise
            _chkCoreElemNoise = new CheckBox { Content = "Enable Digital Noise / Scanline Grain", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 4, 0, 6) };
            _chkCoreElemNoise.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var elem = GetSelectedCoreElement();
                if (elem != null && elem.Effects != null) { elem.Effects.NoiseEnabled = _chkCoreElemNoise.IsChecked == true; ApplyPreviewLive(); }
            };
            stackFx.Children.Add(_chkCoreElemNoise);

            var nAmtRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 0, 0, 6) };
            nAmtRow.Children.Add(new TextBlock { Text = "Amount:", Width = 95, VerticalAlignment = VerticalAlignment.Center });
            _sliderCoreElemNoiseAmt = new Slider { Minimum = 0, Maximum = 100, Value = 25, Width = 140, VerticalAlignment = VerticalAlignment.Center };
            _sliderCoreElemNoiseAmt.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var elem = GetSelectedCoreElement();
                if (elem != null && elem.Effects != null)
                {
                    elem.Effects.NoiseAmount = Math.Round(_sliderCoreElemNoiseAmt.Value);
                    _lblCoreElemNoiseAmt.Text = elem.Effects.NoiseAmount + "%";
                    ApplyPreviewLive();
                }
            };
            nAmtRow.Children.Add(_sliderCoreElemNoiseAmt);
            _lblCoreElemNoiseAmt = new TextBlock { Text = "25%", Width = 45, Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            nAmtRow.Children.Add(_lblCoreElemNoiseAmt);
            stackFx.Children.Add(nAmtRow);

            var nSpeedRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 0, 0, 10) };
            nSpeedRow.Children.Add(new TextBlock { Text = "Speed:", Width = 95, VerticalAlignment = VerticalAlignment.Center });
            _cmbCoreElemNoiseSpeed = CreateComboBox(110);
            _cmbCoreElemNoiseSpeed.Items.Add("Slow");
            _cmbCoreElemNoiseSpeed.Items.Add("Medium");
            _cmbCoreElemNoiseSpeed.Items.Add("Fast");
            _cmbCoreElemNoiseSpeed.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbCoreElemNoiseSpeed.SelectedItem == null) return;
                var elem = GetSelectedCoreElement();
                if (elem != null && elem.Effects != null) { elem.Effects.NoiseSpeed = _cmbCoreElemNoiseSpeed.SelectedItem.ToString(); ApplyPreviewLive(); }
            };
            nSpeedRow.Children.Add(_cmbCoreElemNoiseSpeed);
            stackFx.Children.Add(nSpeedRow);

            grpFx.Content = stackFx;
            root.Children.Add(grpFx);

            scroll.Content = root;
            return scroll;
        }

        private ElementSettings GetSelectedCoreElement()
        {
            if (_preview == null) return null;
            switch (_cmbCoreElementSelector.SelectedIndex)
            {
                case 0: return _preview.Greeting;
                case 1: return _preview.Weekday;
                case 2: return _preview.Time;
                case 3: return _preview.Date;
                default: return _preview.Time;
            }
        }

        private void ApplyPresetToElement(ElementSettings elem, string preset)
        {
            if (elem == null) return;
            if (elem.Effects == null) elem.Effects = new TextEffectSettings();

            switch (preset)
            {
                case "Clean (None)":
                    elem.Effects.OutlineEnabled = false;
                    elem.Effects.GlitchEnabled = false;
                    elem.Effects.NoiseEnabled = false;
                    break;
                case "Outlined":
                    elem.Effects.OutlineEnabled = true;
                    elem.Effects.OutlineColor = "#000000";
                    elem.Effects.OutlineThickness = 2.0;
                    elem.Effects.OutlineOpacity = 1.0;
                    elem.Effects.GlitchEnabled = false;
                    elem.Effects.NoiseEnabled = false;
                    break;
                case "Cyber Glitch":
                    elem.Effects.OutlineEnabled = true;
                    elem.Effects.OutlineColor = "#000000";
                    elem.Effects.OutlineThickness = 1.5;
                    elem.Effects.OutlineOpacity = 0.8;
                    elem.Effects.GlitchEnabled = true;
                    elem.Effects.GlitchIntensity = 40.0;
                    elem.Effects.GlitchSpeed = "Medium";
                    elem.Effects.GlitchColor1 = "#00FFFF";
                    elem.Effects.GlitchColor2 = "#FF0055";
                    elem.Effects.NoiseEnabled = false;
                    break;
                case "Heavy Glitch":
                    elem.Effects.OutlineEnabled = true;
                    elem.Effects.OutlineColor = "#000000";
                    elem.Effects.OutlineThickness = 2.0;
                    elem.Effects.OutlineOpacity = 0.9;
                    elem.Effects.GlitchEnabled = true;
                    elem.Effects.GlitchIntensity = 80.0;
                    elem.Effects.GlitchSpeed = "Fast";
                    elem.Effects.GlitchColor1 = "#00FFFF";
                    elem.Effects.GlitchColor2 = "#FF0055";
                    elem.Effects.NoiseEnabled = true;
                    elem.Effects.NoiseAmount = 30.0;
                    break;
                case "Subtle Noise":
                    elem.Effects.OutlineEnabled = false;
                    elem.Effects.GlitchEnabled = false;
                    elem.Effects.NoiseEnabled = true;
                    elem.Effects.NoiseAmount = 25.0;
                    elem.Effects.NoiseSpeed = "Medium";
                    break;
                case "Digital Distortion":
                    elem.Effects.OutlineEnabled = true;
                    elem.Effects.OutlineColor = "#000000";
                    elem.Effects.OutlineThickness = 2.0;
                    elem.Effects.OutlineOpacity = 1.0;
                    elem.Effects.GlitchEnabled = true;
                    elem.Effects.GlitchIntensity = 55.0;
                    elem.Effects.GlitchSpeed = "Medium";
                    elem.Effects.GlitchColor1 = "#00FFFF";
                    elem.Effects.GlitchColor2 = "#FF0055";
                    elem.Effects.NoiseEnabled = true;
                    elem.Effects.NoiseAmount = 45.0;
                    elem.Effects.NoiseSpeed = "Fast";
                    break;
            }
        }

        private void ApplyPresetToBlock(CustomBlock b, string preset)
        {
            if (b == null) return;
            if (b.Effects == null) b.Effects = new TextEffectSettings();

            switch (preset)
            {
                case "Clean (None)":
                    b.Effects.OutlineEnabled = false;
                    b.Effects.GlitchEnabled = false;
                    b.Effects.NoiseEnabled = false;
                    break;
                case "Outlined":
                    b.Effects.OutlineEnabled = true;
                    b.Effects.OutlineColor = "#000000";
                    b.Effects.OutlineThickness = 2.0;
                    b.Effects.OutlineOpacity = 1.0;
                    b.Effects.GlitchEnabled = false;
                    b.Effects.NoiseEnabled = false;
                    break;
                case "Cyber Glitch":
                    b.Effects.OutlineEnabled = true;
                    b.Effects.OutlineColor = "#000000";
                    b.Effects.OutlineThickness = 1.5;
                    b.Effects.OutlineOpacity = 0.8;
                    b.Effects.GlitchEnabled = true;
                    b.Effects.GlitchIntensity = 40.0;
                    b.Effects.GlitchSpeed = "Medium";
                    b.Effects.GlitchColor1 = "#00FFFF";
                    b.Effects.GlitchColor2 = "#FF0055";
                    b.Effects.NoiseEnabled = false;
                    break;
                case "Heavy Glitch":
                    b.Effects.OutlineEnabled = true;
                    b.Effects.OutlineColor = "#000000";
                    b.Effects.OutlineThickness = 2.0;
                    b.Effects.OutlineOpacity = 0.9;
                    b.Effects.GlitchEnabled = true;
                    b.Effects.GlitchIntensity = 80.0;
                    b.Effects.GlitchSpeed = "Fast";
                    b.Effects.GlitchColor1 = "#00FFFF";
                    b.Effects.GlitchColor2 = "#FF0055";
                    b.Effects.NoiseEnabled = true;
                    b.Effects.NoiseAmount = 30.0;
                    break;
                case "Subtle Noise":
                    b.Effects.OutlineEnabled = false;
                    b.Effects.GlitchEnabled = false;
                    b.Effects.NoiseEnabled = true;
                    b.Effects.NoiseAmount = 25.0;
                    b.Effects.NoiseSpeed = "Medium";
                    break;
                case "Digital Distortion":
                    b.Effects.OutlineEnabled = true;
                    b.Effects.OutlineColor = "#000000";
                    b.Effects.OutlineThickness = 2.0;
                    b.Effects.OutlineOpacity = 1.0;
                    b.Effects.GlitchEnabled = true;
                    b.Effects.GlitchIntensity = 55.0;
                    b.Effects.GlitchSpeed = "Medium";
                    b.Effects.GlitchColor1 = "#00FFFF";
                    b.Effects.GlitchColor2 = "#FF0055";
                    b.Effects.NoiseEnabled = true;
                    b.Effects.NoiseAmount = 45.0;
                    b.Effects.NoiseSpeed = "Fast";
                    break;
            }
        }

        private void PopulateCoreFontList()
        {
            if (_cmbCoreElemCategory == null || _cmbCoreElemFont == null) return;
            string src = _cmbCoreElemSource != null && _cmbCoreElemSource.SelectedItem != null ? _cmbCoreElemSource.SelectedItem.ToString() : "All";
            string cat = _cmbCoreElemCategory.SelectedItem != null ? _cmbCoreElemCategory.SelectedItem.ToString() : "All";
            string search = _txtCoreElemFontSearch != null ? _txtCoreElemFontSearch.Text : "";

            var filtered = FontCatalog.Filter(src, cat, search, _preview.FavoriteFonts);
            string cur = _cmbCoreElemFont.SelectedItem != null ? _cmbCoreElemFont.SelectedItem.ToString() : "";

            _cmbCoreElemFont.Items.Clear();
            foreach (var f in filtered) _cmbCoreElemFont.Items.Add(f.Name);

            if (_cmbCoreElemFont.Items.Contains(cur)) _cmbCoreElemFont.SelectedItem = cur;
            else if (_cmbCoreElemFont.Items.Count > 0) _cmbCoreElemFont.SelectedIndex = 0;

            UpdateCoreFontMetadata();
        }

        private void UpdateCoreFontFavButton()
        {
            var elem = GetSelectedCoreElement();
            if (elem == null || string.IsNullOrEmpty(elem.FontFamily)) return;
            bool isFav = _preview.FavoriteFonts.Contains(elem.FontFamily);
            _btnCoreElemFontFav.Content = isFav ? "â˜…" : "â˜†";
            _btnCoreElemFontFav.Foreground = isFav ? Brushes.Gold : Brushes.White;
        }

        private void UpdateCoreFontPreview()
        {
            var elem = GetSelectedCoreElement();
            if (elem == null || string.IsNullOrEmpty(elem.FontFamily)) return;
            _lblCoreElemFontPreview.FontFamily = Fonts.For(elem.FontFamily);
        }

        private void UpdateCoreFontMetadata()
        {
            if (_lblCoreElemFontMeta == null) return;
            var elem = GetSelectedCoreElement();
            if (elem == null || string.IsNullOrEmpty(elem.FontFamily)) return;

            var curated = FontCatalog.FindCurated(elem.FontFamily);
            if (curated != null)
            {
                _lblCoreElemFontMeta.Text = string.Format("Source: App Font | Category: {0}", curated.Category);
            }
            else
            {
                _lblCoreElemFontMeta.Text = "Source: System Font";
            }
        }

        private void LoadSelectedCoreElementValues()
        {
            _isUpdatingUi = true;
            try
            {
                var elem = GetSelectedCoreElement();
                if (elem == null) return;
                if (elem.Effects == null) elem.Effects = new TextEffectSettings();

                _chkCoreElemVisible.IsChecked = elem.Visible;
                PopulateCoreFontList();
                _cmbCoreElemFont.SelectedItem = elem.FontFamily ?? "Audiowide";
                _cmbCoreElemWeight.SelectedItem = elem.FontWeight ?? "Regular";
                _txtCoreElemFontSize.Text = elem.FontSize.ToString();
                _sliderCoreElemFontSize.Value = elem.FontSize;
                _lblCoreElemColorHex.Text = elem.Color ?? "#D6D3D0";
                _rectCoreElemColorSwatch.Fill = new SolidColorBrush(ParseColor(elem.Color));
                _sliderCoreElemOpacity.Value = Math.Round(elem.Opacity * 100.0);
                _lblCoreElemOpacity.Text = ((int)Math.Round(_sliderCoreElemOpacity.Value)) + "%";
                _cmbCoreElemCase.SelectedItem = elem.Case ?? "Title";

                // Position & Alignment
                _cmbCoreElemAlign.SelectedItem = !string.IsNullOrEmpty(elem.HorizontalAlignment) ? elem.HorizontalAlignment : "Center";
                _txtCoreElemOffsetX.Text = elem.OffsetX.ToString();
                _sliderCoreElemOffsetX.Value = elem.OffsetX;
                _txtCoreElemOffsetY.Text = elem.OffsetY.ToString();
                _sliderCoreElemOffsetY.Value = elem.OffsetY;

                // Effects
                var fx = elem.Effects;
                _chkCoreElemOutline.IsChecked = fx.OutlineEnabled;
                _lblCoreElemOutlineHex.Text = fx.OutlineColor ?? "#000000";
                _rectCoreElemOutlineSwatch.Fill = new SolidColorBrush(ParseColor(fx.OutlineColor));
                _sliderCoreElemOutlineThick.Value = fx.OutlineThickness * 10.0;
                _lblCoreElemOutlineThick.Text = fx.OutlineThickness.ToString("F1") + " DIP";
                _sliderCoreElemOutlineOpacity.Value = Math.Round(fx.OutlineOpacity * 100.0);
                _lblCoreElemOutlineOpacity.Text = ((int)Math.Round(fx.OutlineOpacity * 100.0)) + "%";

                _chkCoreElemGlitch.IsChecked = fx.GlitchEnabled;
                _sliderCoreElemGlitchInt.Value = fx.GlitchIntensity;
                _lblCoreElemGlitchInt.Text = ((int)fx.GlitchIntensity) + "%";
                _cmbCoreElemGlitchSpeed.SelectedItem = fx.GlitchSpeed ?? "Medium";
                _lblCoreElemGlitchC1Hex.Text = fx.GlitchColor1 ?? "#00FFFF";
                _rectCoreElemGlitchC1Swatch.Fill = new SolidColorBrush(ParseColor(fx.GlitchColor1));
                _lblCoreElemGlitchC2Hex.Text = fx.GlitchColor2 ?? "#FF0055";
                _rectCoreElemGlitchC2Swatch.Fill = new SolidColorBrush(ParseColor(fx.GlitchColor2));

                _chkCoreElemNoise.IsChecked = fx.NoiseEnabled;
                _sliderCoreElemNoiseAmt.Value = fx.NoiseAmount;
                _lblCoreElemNoiseAmt.Text = ((int)fx.NoiseAmount) + "%";
                _cmbCoreElemNoiseSpeed.SelectedItem = fx.NoiseSpeed ?? "Medium";

                UpdateCoreFontFavButton();
                UpdateCoreFontPreview();
                UpdateCoreFontMetadata();
            }
            finally
            {
                _isUpdatingUi = false;
            }
        }

        private UIElement CreateBlocksTab()
        {
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(135) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            _lstBlocks = new ListBox
            {
                Background = new SolidColorBrush(Color.FromRgb(28, 30, 33)),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 52, 58)),
                Margin = new Thickness(8)
            };
            _lstBlocks.SelectionChanged += (s, e) => LoadSelectedBlockValues();
            Grid.SetRow(_lstBlocks, 0);
            mainGrid.Children.Add(_lstBlocks);

            var toolBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(8, 0, 8, 8) };
            _btnAddBlock = CreateStyledButton("+ Add Block", 95);
            _btnAddBlock.Click += (s, e) =>
            {
                var b = new CustomBlock { Name = "Block " + (_preview.Blocks.Count + 1), Order = _preview.Blocks.Count };
                _preview.Blocks.Add(b);
                RefreshBlocksList();
                _lstBlocks.SelectedItem = b;
                ApplyPreviewLive();
            };

            _btnDupBlock = CreateStyledButton("Duplicate", 85);
            _btnDupBlock.Click += (s, e) =>
            {
                var b = _lstBlocks.SelectedItem as CustomBlock;
                if (b == null) return;
                var dup = b.Clone();
                _preview.Blocks.Add(dup);
                RefreshBlocksList();
                _lstBlocks.SelectedItem = dup;
                ApplyPreviewLive();
            };

            _btnDelBlock = CreateStyledButton("Delete", 75);
            _btnDelBlock.Click += (s, e) =>
            {
                var b = _lstBlocks.SelectedItem as CustomBlock;
                if (b == null) return;
                _preview.Blocks.Remove(b);
                RefreshBlocksList();
                if (_lstBlocks.Items.Count > 0) _lstBlocks.SelectedIndex = 0;
                ApplyPreviewLive();
            };

            _btnMoveUpBlock = CreateStyledButton("â–² Up", 65);
            _btnMoveUpBlock.Click += (s, e) =>
            {
                int idx = _lstBlocks.SelectedIndex;
                if (idx > 0)
                {
                    var item = _preview.Blocks[idx];
                    _preview.Blocks.RemoveAt(idx);
                    _preview.Blocks.Insert(idx - 1, item);
                    for (int i = 0; i < _preview.Blocks.Count; i++) _preview.Blocks[i].Order = i;
                    RefreshBlocksList();
                    _lstBlocks.SelectedIndex = idx - 1;
                    ApplyPreviewLive();
                }
            };

            _btnMoveDownBlock = CreateStyledButton("â–¼ Down", 75);
            _btnMoveDownBlock.Click += (s, e) =>
            {
                int idx = _lstBlocks.SelectedIndex;
                if (idx >= 0 && idx < _preview.Blocks.Count - 1)
                {
                    var item = _preview.Blocks[idx];
                    _preview.Blocks.RemoveAt(idx);
                    _preview.Blocks.Insert(idx + 1, item);
                    for (int i = 0; i < _preview.Blocks.Count; i++) _preview.Blocks[i].Order = i;
                    RefreshBlocksList();
                    _lstBlocks.SelectedIndex = idx + 1;
                    ApplyPreviewLive();
                }
            };

            toolBar.Children.Add(_btnAddBlock);
            toolBar.Children.Add(_btnDupBlock);
            toolBar.Children.Add(_btnDelBlock);
            toolBar.Children.Add(_btnMoveUpBlock);
            toolBar.Children.Add(_btnMoveDownBlock);

            Grid.SetRow(toolBar, 1);
            mainGrid.Children.Add(toolBar);

            _blockInspectorPanel = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 52, 58)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(8),
                Padding = new Thickness(6)
            };

            var inspectorScroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            var insStack = new StackPanel();

            var topRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            _chkBlockEnabled = new CheckBox { Content = "Enabled", FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 14, 0) };
            _chkBlockEnabled.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null) { b.Enabled = _chkBlockEnabled.IsChecked == true; RefreshBlocksList(); ApplyPreviewLive(); }
            };
            topRow.Children.Add(_chkBlockEnabled);

            topRow.Children.Add(new TextBlock { Text = "Name:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) });
            _txtBlockName = CreateTextBox(130);
            _txtBlockName.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null) { b.Name = _txtBlockName.Text; RefreshBlocksList(); }
            };
            topRow.Children.Add(_txtBlockName);

            topRow.Children.Add(new TextBlock { Text = "Order:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 4, 0) });
            _txtBlockOrder = CreateTextBox(40);
            _txtBlockOrder.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                int ord;
                if (int.TryParse(_txtBlockOrder.Text, out ord))
                {
                    var b = GetSelectedBlock();
                    if (b != null) { b.Order = ord; ApplyPreviewLive(); }
                }
            };
            topRow.Children.Add(_txtBlockOrder);
            insStack.Children.Add(topRow);

            var row2 = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            row2.Children.Add(new TextBlock { Text = "Type:", Width = 60, VerticalAlignment = VerticalAlignment.Center });
            _cmbBlockType = CreateComboBox(120);
            _cmbBlockType.Items.Add("Symbol");
            _cmbBlockType.Items.Add("Static Text");
            _cmbBlockType.Items.Add("Rotating Text");
            _cmbBlockType.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbBlockType.SelectedItem == null) return;
                var b = GetSelectedBlock();
                if (b != null)
                {
                    b.Type = _cmbBlockType.SelectedItem.ToString();
                    UpdateBlockTypeVisibility();
                    RefreshBlocksList();
                    ApplyPreviewLive();
                }
            };
            row2.Children.Add(_cmbBlockType);

            row2.Children.Add(new TextBlock { Text = "Position:", Width = 65, Margin = new Thickness(12, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center });
            _cmbBlockPosition = CreateComboBox(150);
            string[] positions = new string[]
            {
                "Above Widget", "Below Widget", "Above Greeting", "Below Greeting",
                "Above Weekday", "Below Weekday", "Above Time", "Below Time",
                "Above Date", "Below Date", "Left of Widget", "Right of Widget"
            };
            foreach (var p in positions) _cmbBlockPosition.Items.Add(p);
            _cmbBlockPosition.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbBlockPosition.SelectedItem == null) return;
                var b = GetSelectedBlock();
                if (b != null)
                {
                    b.Position = _cmbBlockPosition.SelectedItem.ToString();
                    RefreshBlocksList();
                    ApplyPreviewLive();
                }
            };
            row2.Children.Add(_cmbBlockPosition);
            insStack.Children.Add(row2);

            _panelBlockSymbol = new StackPanel { Margin = new Thickness(0, 0, 0, 8) };
            var symRow = new StackPanel { Orientation = Orientation.Horizontal };
            symRow.Children.Add(new TextBlock { Text = "Symbol:", Width = 60, VerticalAlignment = VerticalAlignment.Center });
            _cmbBlockPresetSymbol = CreateComboBox(70);
            foreach (var sym in GlyphHelper.GetValidSymbols()) _cmbBlockPresetSymbol.Items.Add(sym);
            _cmbBlockPresetSymbol.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbBlockPresetSymbol.SelectedItem == null) return;
                var b = GetSelectedBlock();
                if (b != null)
                {
                    b.SymbolContent = _cmbBlockPresetSymbol.SelectedItem.ToString();
                    _txtBlockCustomSymbol.Text = b.SymbolContent;
                    ApplyPreviewLive();
                }
            };
            symRow.Children.Add(_cmbBlockPresetSymbol);

            symRow.Children.Add(new TextBlock { Text = "Custom Symbol:", Margin = new Thickness(12, 0, 6, 0), VerticalAlignment = VerticalAlignment.Center });
            _txtBlockCustomSymbol = CreateTextBox(70);
            _txtBlockCustomSymbol.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null && !string.IsNullOrEmpty(_txtBlockCustomSymbol.Text))
                {
                    b.SymbolContent = _txtBlockCustomSymbol.Text;
                    ApplyPreviewLive();
                }
            };
            symRow.Children.Add(_txtBlockCustomSymbol);
            _panelBlockSymbol.Children.Add(symRow);
            insStack.Children.Add(_panelBlockSymbol);

            _panelBlockStatic = new StackPanel { Margin = new Thickness(0, 0, 0, 8) };
            var statRow = new StackPanel { Orientation = Orientation.Horizontal };
            statRow.Children.Add(new TextBlock { Text = "Text:", Width = 60, VerticalAlignment = VerticalAlignment.Center });
            _txtBlockStaticText = CreateTextBox(240);
            _txtBlockStaticText.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null) { b.StaticContent = _txtBlockStaticText.Text; ApplyPreviewLive(); }
            };
            statRow.Children.Add(_txtBlockStaticText);
            _panelBlockStatic.Children.Add(statRow);
            insStack.Children.Add(_panelBlockStatic);

            _panelBlockRotating = new StackPanel { Margin = new Thickness(0, 0, 0, 8) };
            var rotModeRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            rotModeRow.Children.Add(new TextBlock { Text = "Rotation:", Width = 60, VerticalAlignment = VerticalAlignment.Center });
            _cmbBlockRotationMode = CreateComboBox(120);
            _cmbBlockRotationMode.Items.Add("Sequential");
            _cmbBlockRotationMode.Items.Add("Random");
            _cmbBlockRotationMode.Items.Add("Schedule");
            _cmbBlockRotationMode.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbBlockRotationMode.SelectedItem == null) return;
                var b = GetSelectedBlock();
                if (b != null)
                {
                    b.RotationMode = _cmbBlockRotationMode.SelectedItem.ToString();
                    UpdateBlockRotationModePanels();
                    ApplyPreviewLive();
                }
            };
            rotModeRow.Children.Add(_cmbBlockRotationMode);
            _panelBlockRotating.Children.Add(rotModeRow);

            _panelBlockInterval = new StackPanel { Margin = new Thickness(0, 0, 0, 6) };
            var intRow = new StackPanel { Orientation = Orientation.Horizontal };
            intRow.Children.Add(new TextBlock { Text = "Interval:", Width = 60, VerticalAlignment = VerticalAlignment.Center });
            _txtBlockIntervalValue = CreateTextBox(45);
            _txtBlockIntervalValue.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                int val;
                if (int.TryParse(_txtBlockIntervalValue.Text, out val) && val >= 1)
                {
                    var b = GetSelectedBlock();
                    if (b != null)
                    {
                        b.IntervalValue = val;
                        b.IntervalMinutes = b.IntervalUnit == "Hours" ? val * 60 : val;
                        ApplyPreviewLive();
                    }
                }
            };
            intRow.Children.Add(_txtBlockIntervalValue);

            _cmbBlockIntervalUnit = CreateComboBox(90);
            _cmbBlockIntervalUnit.Items.Add("Minutes");
            _cmbBlockIntervalUnit.Items.Add("Hours");
            _cmbBlockIntervalUnit.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbBlockIntervalUnit.SelectedItem == null) return;
                var b = GetSelectedBlock();
                if (b != null)
                {
                    b.IntervalUnit = _cmbBlockIntervalUnit.SelectedItem.ToString();
                    b.IntervalMinutes = b.IntervalUnit == "Hours" ? b.IntervalValue * 60 : b.IntervalValue;
                    ApplyPreviewLive();
                }
            };
            intRow.Children.Add(_cmbBlockIntervalUnit);
            _panelBlockInterval.Children.Add(intRow);

            _panelBlockInterval.Children.Add(new TextBlock { Text = "Rotating Messages:", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 4, 0, 2) });
            _lstBlockMessages = new ListBox
            {
                Height = 70,
                Background = new SolidColorBrush(Color.FromRgb(24, 26, 28)),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 52, 58))
            };
            _panelBlockInterval.Children.Add(_lstBlockMessages);

            var msgTools = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 0) };
            _btnAddBlockMsg = CreateStyledButton("+ Msg", 60);
            _btnAddBlockMsg.Click += (s, e) =>
            {
                var b = GetSelectedBlock();
                if (b == null) return;
                b.Messages.Add("NEW MESSAGE " + (b.Messages.Count + 1));
                RefreshBlockMessagesList();
                ApplyPreviewLive();
            };
            _btnDelBlockMsg = CreateStyledButton("Delete", 60);
            _btnDelBlockMsg.Click += (s, e) =>
            {
                var b = GetSelectedBlock();
                if (b == null || _lstBlockMessages.SelectedIndex < 0) return;
                b.Messages.RemoveAt(_lstBlockMessages.SelectedIndex);
                RefreshBlockMessagesList();
                ApplyPreviewLive();
            };
            msgTools.Children.Add(_btnAddBlockMsg);
            msgTools.Children.Add(_btnDelBlockMsg);
            _panelBlockInterval.Children.Add(msgTools);
            _panelBlockRotating.Children.Add(_panelBlockInterval);

            _panelBlockSchedule = new StackPanel { Margin = new Thickness(0, 0, 0, 6) };
            _panelBlockSchedule.Children.Add(new TextBlock { Text = "Fixed-Time Schedules (HH:mm -> Text):", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 4, 0, 2) });
            _lstBlockSchedules = new ListBox
            {
                Height = 70,
                Background = new SolidColorBrush(Color.FromRgb(24, 26, 28)),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 52, 58))
            };
            _panelBlockSchedule.Children.Add(_lstBlockSchedules);

            var schedTools = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 0) };
            _btnAddSchedule = CreateStyledButton("+ Schedule", 85);
            _btnAddSchedule.Click += (s, e) =>
            {
                var b = GetSelectedBlock();
                if (b == null) return;
                b.ScheduledMessages.Add(new ScheduledMessage("12:00", "SCHEDULED MESSAGE"));
                RefreshBlockScheduleList();
                ApplyPreviewLive();
            };
            _btnDelSchedule = CreateStyledButton("Delete", 60);
            _btnDelSchedule.Click += (s, e) =>
            {
                var b = GetSelectedBlock();
                if (b == null || _lstBlockSchedules.SelectedIndex < 0) return;
                b.ScheduledMessages.RemoveAt(_lstBlockSchedules.SelectedIndex);
                RefreshBlockScheduleList();
                ApplyPreviewLive();
            };
            schedTools.Children.Add(_btnAddSchedule);
            schedTools.Children.Add(_btnDelSchedule);
            _panelBlockSchedule.Children.Add(schedTools);
            _panelBlockRotating.Children.Add(_panelBlockSchedule);

            insStack.Children.Add(_panelBlockRotating);

            // Styling Section
            var grpApp = CreateGroupBox("Block Styling");
            var appStack = new StackPanel { Margin = new Thickness(6) };

            var bFontRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
            bFontRow.Children.Add(new TextBlock { Text = "Font:", Width = 70, VerticalAlignment = VerticalAlignment.Center });
            _cmbBlockFont = CreateComboBox(170);
            _cmbBlockFont.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbBlockFont.SelectedItem == null) return;
                var b = GetSelectedBlock();
                if (b != null)
                {
                    b.FontFamily = _cmbBlockFont.SelectedItem.ToString();
                    UpdateBlockFontFavButton();
                    UpdateBlockFontPreview();
                    UpdateBlockFontMetadata();
                    ApplyPreviewLive();
                }
            };
            bFontRow.Children.Add(_cmbBlockFont);

            _btnBlockFontFav = CreateStyledButton("â˜†", 34);
            _btnBlockFontFav.Click += (s, e) =>
            {
                var b = GetSelectedBlock();
                if (b == null || string.IsNullOrEmpty(b.FontFamily)) return;
                if (_preview.FavoriteFonts.Contains(b.FontFamily)) _preview.FavoriteFonts.Remove(b.FontFamily);
                else _preview.FavoriteFonts.Add(b.FontFamily);
                UpdateBlockFontFavButton();
            };
            bFontRow.Children.Add(_btnBlockFontFav);
            appStack.Children.Add(bFontRow);

            _lblBlockFontMeta = new TextBlock
            {
                Text = "Source: App Font | Category: Futuristic",
                Foreground = Brushes.Gray,
                FontSize = 11,
                Margin = new Thickness(70, 0, 0, 6)
            };
            appStack.Children.Add(_lblBlockFontMeta);

            _lblBlockFontPreview = new TextBlock
            {
                Text = "SAMPLE TEXT 123",
                FontSize = 13,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)),
                Padding = new Thickness(4),
                Margin = new Thickness(0, 0, 0, 6),
                TextAlignment = TextAlignment.Center
            };
            appStack.Children.Add(_lblBlockFontPreview);

            var rowWz = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            rowWz.Children.Add(new TextBlock { Text = "Weight:", Width = 70, VerticalAlignment = VerticalAlignment.Center });
            _cmbBlockFontWeight = CreateComboBox(110);
            foreach (var w in Fonts.Weights) _cmbBlockFontWeight.Items.Add(w);
            _cmbBlockFontWeight.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbBlockFontWeight.SelectedItem == null) return;
                var b = GetSelectedBlock();
                if (b != null) { b.FontWeight = _cmbBlockFontWeight.SelectedItem.ToString(); ApplyPreviewLive(); }
            };
            rowWz.Children.Add(_cmbBlockFontWeight);

            rowWz.Children.Add(new TextBlock { Text = "Size:", Width = 40, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center });
            _txtBlockFontSize = CreateTextBox(38);
            _txtBlockFontSize.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val;
                if (double.TryParse(_txtBlockFontSize.Text, out val) && val >= 6 && val <= 120)
                {
                    var b = GetSelectedBlock();
                    if (b != null) { b.FontSize = val; _sliderBlockFontSize.Value = val; ApplyPreviewLive(); }
                }
            };
            rowWz.Children.Add(_txtBlockFontSize);

            _sliderBlockFontSize = new Slider { Minimum = 8, Maximum = 100, Value = 16, Width = 110, Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            _sliderBlockFontSize.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val = Math.Round(_sliderBlockFontSize.Value);
                var b = GetSelectedBlock();
                if (b != null) { b.FontSize = val; _txtBlockFontSize.Text = val.ToString(); ApplyPreviewLive(); }
            };
            rowWz.Children.Add(_sliderBlockFontSize);
            appStack.Children.Add(rowWz);

            var rowCo = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            rowCo.Children.Add(new TextBlock { Text = "Color:", Width = 70, VerticalAlignment = VerticalAlignment.Center });
            _rectBlockColorSwatch = new Rectangle { Width = 20, Height = 20, Stroke = Brushes.Gray, StrokeThickness = 1, Margin = new Thickness(0, 0, 6, 0) };
            rowCo.Children.Add(_rectBlockColorSwatch);
            _lblBlockColorHex = new TextBlock { Text = "#D6D3D0", Width = 65, VerticalAlignment = VerticalAlignment.Center };
            rowCo.Children.Add(_lblBlockColorHex);

            var btnBColor = CreateStyledButton("Choose...", 70);
            btnBColor.Click += (s, e) =>
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string hex = string.Format("#{0:X2}{1:X2}{2:X2}", dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    var b = GetSelectedBlock();
                    if (b != null)
                    {
                        b.Color = hex;
                        _lblBlockColorHex.Text = hex;
                        _rectBlockColorSwatch.Fill = new SolidColorBrush(ParseColor(hex));
                        ApplyPreviewLive();
                    }
                }
            };
            rowCo.Children.Add(btnBColor);
            appStack.Children.Add(rowCo);

            var rowOp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            rowOp.Children.Add(new TextBlock { Text = "Opacity:", Width = 70, VerticalAlignment = VerticalAlignment.Center });
            _sliderBlockOpacity = new Slider { Minimum = 0, Maximum = 100, Value = 80, Width = 140, VerticalAlignment = VerticalAlignment.Center };
            _sliderBlockOpacity.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null)
                {
                    b.Opacity = Math.Round(_sliderBlockOpacity.Value) / 100.0;
                    _lblBlockOpacity.Text = ((int)Math.Round(_sliderBlockOpacity.Value)) + "%";
                    ApplyPreviewLive();
                }
            };
            rowOp.Children.Add(_sliderBlockOpacity);
            _lblBlockOpacity = new TextBlock { Text = "80%", Width = 45, Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            rowOp.Children.Add(_lblBlockOpacity);
            appStack.Children.Add(rowOp);

            var rowAc = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            rowAc.Children.Add(new TextBlock { Text = "Align:", Width = 70, VerticalAlignment = VerticalAlignment.Center });
            _cmbBlockAlignment = CreateComboBox(85);
            _cmbBlockAlignment.Items.Add("Left");
            _cmbBlockAlignment.Items.Add("Center");
            _cmbBlockAlignment.Items.Add("Right");
            _cmbBlockAlignment.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbBlockAlignment.SelectedItem == null) return;
                var b = GetSelectedBlock();
                if (b != null) { b.Alignment = _cmbBlockAlignment.SelectedItem.ToString(); ApplyPreviewLive(); }
            };
            rowAc.Children.Add(_cmbBlockAlignment);

            rowAc.Children.Add(new TextBlock { Text = "Case:", Width = 40, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center });
            _cmbBlockCase = CreateComboBox(80);
            _cmbBlockCase.Items.Add("None");
            _cmbBlockCase.Items.Add("Title");
            _cmbBlockCase.Items.Add("Upper");
            _cmbBlockCase.Items.Add("Lower");
            _cmbBlockCase.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbBlockCase.SelectedItem == null) return;
                var b = GetSelectedBlock();
                if (b != null) { b.Case = _cmbBlockCase.SelectedItem.ToString(); ApplyPreviewLive(); }
            };
            rowAc.Children.Add(_cmbBlockCase);
            appStack.Children.Add(rowAc);

            var rowBx = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            rowBx.Children.Add(new TextBlock { Text = "X Offset:", Width = 70, VerticalAlignment = VerticalAlignment.Center });
            _txtBlockOffsetX = CreateTextBox(40);
            _txtBlockOffsetX.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val;
                if (double.TryParse(_txtBlockOffsetX.Text, out val) && val >= -120 && val <= 120)
                {
                    var b = GetSelectedBlock();
                    if (b != null) { b.OffsetX = val; _sliderBlockOffsetX.Value = val; ApplyPreviewLive(); }
                }
            };
            rowBx.Children.Add(_txtBlockOffsetX);
            _sliderBlockOffsetX = new Slider { Minimum = -120, Maximum = 120, Value = 0, Width = 110, Margin = new Thickness(6, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            _sliderBlockOffsetX.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val = Math.Round(_sliderBlockOffsetX.Value);
                var b = GetSelectedBlock();
                if (b != null) { b.OffsetX = val; _txtBlockOffsetX.Text = val.ToString(); ApplyPreviewLive(); }
            };
            rowBx.Children.Add(_sliderBlockOffsetX);
            appStack.Children.Add(rowBx);

            var rowBy = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            rowBy.Children.Add(new TextBlock { Text = "Y Offset:", Width = 70, VerticalAlignment = VerticalAlignment.Center });
            _txtBlockOffsetY = CreateTextBox(40);
            _txtBlockOffsetY.TextChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val;
                if (double.TryParse(_txtBlockOffsetY.Text, out val) && val >= -80 && val <= 80)
                {
                    var b = GetSelectedBlock();
                    if (b != null) { b.OffsetY = val; _sliderBlockOffsetY.Value = val; ApplyPreviewLive(); }
                }
            };
            rowBy.Children.Add(_txtBlockOffsetY);
            _sliderBlockOffsetY = new Slider { Minimum = -80, Maximum = 80, Value = 0, Width = 110, Margin = new Thickness(6, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            _sliderBlockOffsetY.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val = Math.Round(_sliderBlockOffsetY.Value);
                var b = GetSelectedBlock();
                if (b != null) { b.OffsetY = val; _txtBlockOffsetY.Text = val.ToString(); ApplyPreviewLive(); }
            };
            rowBy.Children.Add(_sliderBlockOffsetY);

            var btnResetBPos = CreateStyledButton("Reset Pos", 70);
            btnResetBPos.Margin = new Thickness(8, 0, 0, 0);
            btnResetBPos.Click += (s, e) =>
            {
                var b = GetSelectedBlock();
                if (b != null)
                {
                    b.Alignment = "Center";
                    b.OffsetX = 0.0;
                    b.OffsetY = 0.0;
                    LoadSelectedBlockValues();
                    ApplyPreviewLive();
                }
            };
            rowBy.Children.Add(btnResetBPos);
            appStack.Children.Add(rowBy);

            var rowStyle = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
            _chkBlockItalic = new CheckBox { Content = "Italic", FontWeight = FontWeights.Medium, Margin = new Thickness(70, 0, 16, 0) };
            _chkBlockItalic.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null) { b.Italic = _chkBlockItalic.IsChecked == true; ApplyPreviewLive(); }
            };
            rowStyle.Children.Add(_chkBlockItalic);

            _chkBlockUnderline = new CheckBox { Content = "Underline", FontWeight = FontWeights.Medium };
            _chkBlockUnderline.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null) { b.Underline = _chkBlockUnderline.IsChecked == true; ApplyPreviewLive(); }
            };
            rowStyle.Children.Add(_chkBlockUnderline);
            appStack.Children.Add(rowStyle);

            grpApp.Content = appStack;
            insStack.Children.Add(grpApp);

            // Block Effects Section
            var grpBlockFx = CreateGroupBox("Block Effects");
            var blockFxStack = new StackPanel { Margin = new Thickness(6) };

            var bPresetRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            bPresetRow.Children.Add(new TextBlock { Text = "Effect Preset:", Width = 95, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center });
            _cmbBlockEffectPreset = CreateComboBox(150);
            _cmbBlockEffectPreset.Items.Add("Custom");
            _cmbBlockEffectPreset.Items.Add("Clean (None)");
            _cmbBlockEffectPreset.Items.Add("Outlined");
            _cmbBlockEffectPreset.Items.Add("Cyber Glitch");
            _cmbBlockEffectPreset.Items.Add("Heavy Glitch");
            _cmbBlockEffectPreset.Items.Add("Subtle Noise");
            _cmbBlockEffectPreset.Items.Add("Digital Distortion");
            _cmbBlockEffectPreset.SelectionChanged += (s, e) =>
            {
                if (_isUpdatingUi || _cmbBlockEffectPreset.SelectedItem == null) return;
                ApplyPresetToBlock(GetSelectedBlock(), _cmbBlockEffectPreset.SelectedItem.ToString());
                LoadSelectedBlockValues();
                ApplyPreviewLive();
            };
            bPresetRow.Children.Add(_cmbBlockEffectPreset);
            blockFxStack.Children.Add(bPresetRow);

            _chkBlockOutline = new CheckBox { Content = "Outline Effect", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 2, 0, 4) };
            _chkBlockOutline.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null && b.Effects != null) { b.Effects.OutlineEnabled = _chkBlockOutline.IsChecked == true; ApplyPreviewLive(); }
            };
            blockFxStack.Children.Add(_chkBlockOutline);

            var bOutRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(16, 0, 0, 6) };
            bOutRow.Children.Add(new TextBlock { Text = "Color:", Width = 55, VerticalAlignment = VerticalAlignment.Center });
            _rectBlockOutlineSwatch = new Rectangle { Width = 18, Height = 18, Stroke = Brushes.Gray, StrokeThickness = 1, Margin = new Thickness(0, 0, 4, 0) };
            bOutRow.Children.Add(_rectBlockOutlineSwatch);
            _lblBlockOutlineHex = new TextBlock { Text = "#000000", Width = 60, VerticalAlignment = VerticalAlignment.Center };
            bOutRow.Children.Add(_lblBlockOutlineHex);
            var btnBOutCol = CreateStyledButton("...", 30);
            btnBOutCol.Click += (s, e) =>
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string hex = string.Format("#{0:X2}{1:X2}{2:X2}", dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    var b = GetSelectedBlock();
                    if (b != null && b.Effects != null)
                    {
                        b.Effects.OutlineColor = hex;
                        _lblBlockOutlineHex.Text = hex;
                        _rectBlockOutlineSwatch.Fill = new SolidColorBrush(ParseColor(hex));
                        ApplyPreviewLive();
                    }
                }
            };
            bOutRow.Children.Add(btnBOutCol);

            bOutRow.Children.Add(new TextBlock { Text = "Thick:", Width = 40, Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center });
            _sliderBlockOutlineThick = new Slider { Minimum = 5, Maximum = 60, Value = 20, Width = 80, VerticalAlignment = VerticalAlignment.Center };
            _sliderBlockOutlineThick.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                double val = Math.Round(_sliderBlockOutlineThick.Value) / 10.0;
                var b = GetSelectedBlock();
                if (b != null && b.Effects != null)
                {
                    b.Effects.OutlineThickness = val;
                    _lblBlockOutlineThick.Text = val.ToString("F1");
                    ApplyPreviewLive();
                }
            };
            bOutRow.Children.Add(_sliderBlockOutlineThick);
            _lblBlockOutlineThick = new TextBlock { Text = "2.0", Width = 30, Margin = new Thickness(4, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            bOutRow.Children.Add(_lblBlockOutlineThick);
            blockFxStack.Children.Add(bOutRow);

            _chkBlockGlitch = new CheckBox { Content = "Glitch Effect", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 2, 0, 4) };
            _chkBlockGlitch.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null && b.Effects != null) { b.Effects.GlitchEnabled = _chkBlockGlitch.IsChecked == true; ApplyPreviewLive(); }
            };
            blockFxStack.Children.Add(_chkBlockGlitch);

            var bGRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(16, 0, 0, 6) };
            bGRow.Children.Add(new TextBlock { Text = "Intensity:", Width = 60, VerticalAlignment = VerticalAlignment.Center });
            _sliderBlockGlitchInt = new Slider { Minimum = 0, Maximum = 100, Value = 35, Width = 100, VerticalAlignment = VerticalAlignment.Center };
            _sliderBlockGlitchInt.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null && b.Effects != null)
                {
                    b.Effects.GlitchIntensity = Math.Round(_sliderBlockGlitchInt.Value);
                    _lblBlockGlitchInt.Text = b.Effects.GlitchIntensity + "%";
                    ApplyPreviewLive();
                }
            };
            bGRow.Children.Add(_sliderBlockGlitchInt);
            _lblBlockGlitchInt = new TextBlock { Text = "35%", Width = 40, Margin = new Thickness(4, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            bGRow.Children.Add(_lblBlockGlitchInt);
            blockFxStack.Children.Add(bGRow);

            _chkBlockNoise = new CheckBox { Content = "Noise Effect", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 2, 0, 4) };
            _chkBlockNoise.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null && b.Effects != null) { b.Effects.NoiseEnabled = _chkBlockNoise.IsChecked == true; ApplyPreviewLive(); }
            };
            blockFxStack.Children.Add(_chkBlockNoise);

            var bNRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(16, 0, 0, 6) };
            bNRow.Children.Add(new TextBlock { Text = "Amount:", Width = 60, VerticalAlignment = VerticalAlignment.Center });
            _sliderBlockNoiseAmt = new Slider { Minimum = 0, Maximum = 100, Value = 25, Width = 100, VerticalAlignment = VerticalAlignment.Center };
            _sliderBlockNoiseAmt.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                var b = GetSelectedBlock();
                if (b != null && b.Effects != null)
                {
                    b.Effects.NoiseAmount = Math.Round(_sliderBlockNoiseAmt.Value);
                    _lblBlockNoiseAmt.Text = b.Effects.NoiseAmount + "%";
                    ApplyPreviewLive();
                }
            };
            bNRow.Children.Add(_sliderBlockNoiseAmt);
            _lblBlockNoiseAmt = new TextBlock { Text = "25%", Width = 40, Margin = new Thickness(4, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            bNRow.Children.Add(_lblBlockNoiseAmt);
            blockFxStack.Children.Add(bNRow);

            grpBlockFx.Content = blockFxStack;
            insStack.Children.Add(grpBlockFx);

            inspectorScroll.Content = insStack;
            _blockInspectorPanel.Child = inspectorScroll;

            Grid.SetRow(_blockInspectorPanel, 2);
            mainGrid.Children.Add(_blockInspectorPanel);

            return mainGrid;
        }

        private CustomBlock GetSelectedBlock()
        {
            return _lstBlocks != null ? _lstBlocks.SelectedItem as CustomBlock : null;
        }

        private void RefreshBlocksList()
        {
            if (_lstBlocks == null || _preview.Blocks == null) return;
            var cur = _lstBlocks.SelectedItem;
            _lstBlocks.Items.Clear();
            foreach (var b in _preview.Blocks.OrderBy(x => x.Order))
            {
                _lstBlocks.Items.Add(b);
            }
            if (cur != null && _lstBlocks.Items.Contains(cur)) _lstBlocks.SelectedItem = cur;
            else if (_lstBlocks.Items.Count > 0) _lstBlocks.SelectedIndex = 0;
        }

        private void LoadSelectedBlockValues()
        {
            var b = GetSelectedBlock();
            if (b == null)
            {
                _blockInspectorPanel.Visibility = Visibility.Collapsed;
                return;
            }
            _blockInspectorPanel.Visibility = Visibility.Visible;

            _isUpdatingUi = true;
            try
            {
                if (b.Effects == null) b.Effects = new TextEffectSettings();

                _chkBlockEnabled.IsChecked = b.Enabled;
                _txtBlockName.Text = b.Name ?? "";
                _txtBlockOrder.Text = b.Order.ToString();
                _cmbBlockType.SelectedItem = b.Type ?? "Symbol";
                _cmbBlockPosition.SelectedItem = b.Position ?? "Above Widget";

                _cmbBlockPresetSymbol.SelectedItem = b.SymbolContent ?? "âœ¦";
                _txtBlockCustomSymbol.Text = b.SymbolContent ?? "âœ¦";
                _txtBlockStaticText.Text = b.StaticContent ?? "";

                _cmbBlockRotationMode.SelectedItem = b.RotationMode ?? "Sequential";
                _txtBlockIntervalValue.Text = b.IntervalValue > 0 ? b.IntervalValue.ToString() : "30";
                _cmbBlockIntervalUnit.SelectedItem = b.IntervalUnit ?? "Minutes";

                RefreshBlockMessagesList();
                RefreshBlockScheduleList();

                PopulateBlockFontList();
                _cmbBlockFont.SelectedItem = b.FontFamily ?? "Segoe UI Symbol";
                _cmbBlockFontWeight.SelectedItem = b.FontWeight ?? "Regular";
                _txtBlockFontSize.Text = b.FontSize.ToString();
                _sliderBlockFontSize.Value = b.FontSize;
                _lblBlockColorHex.Text = b.Color ?? "#D6D3D0";
                _rectBlockColorSwatch.Fill = new SolidColorBrush(ParseColor(b.Color));
                _sliderBlockOpacity.Value = Math.Round(b.Opacity * 100.0);
                _lblBlockOpacity.Text = ((int)Math.Round(_sliderBlockOpacity.Value)) + "%";
                _cmbBlockAlignment.SelectedItem = !string.IsNullOrEmpty(b.Alignment) ? b.Alignment : "Center";
                _txtBlockOffsetX.Text = b.OffsetX.ToString();
                _sliderBlockOffsetX.Value = b.OffsetX;
                _txtBlockOffsetY.Text = b.OffsetY.ToString();
                _sliderBlockOffsetY.Value = b.OffsetY;
                _cmbBlockCase.SelectedItem = b.Case ?? "None";
                _chkBlockItalic.IsChecked = b.Italic;
                _chkBlockUnderline.IsChecked = b.Underline;

                // Block Effects
                var fx = b.Effects;
                _chkBlockOutline.IsChecked = fx.OutlineEnabled;
                _lblBlockOutlineHex.Text = fx.OutlineColor ?? "#000000";
                _rectBlockOutlineSwatch.Fill = new SolidColorBrush(ParseColor(fx.OutlineColor));
                _sliderBlockOutlineThick.Value = fx.OutlineThickness * 10.0;
                _lblBlockOutlineThick.Text = fx.OutlineThickness.ToString("F1");

                _chkBlockGlitch.IsChecked = fx.GlitchEnabled;
                _sliderBlockGlitchInt.Value = fx.GlitchIntensity;
                _lblBlockGlitchInt.Text = ((int)fx.GlitchIntensity) + "%";

                _chkBlockNoise.IsChecked = fx.NoiseEnabled;
                _sliderBlockNoiseAmt.Value = fx.NoiseAmount;
                _lblBlockNoiseAmt.Text = ((int)fx.NoiseAmount) + "%";

                UpdateBlockTypeVisibility();
                UpdateBlockRotationModePanels();
                UpdateBlockFontFavButton();
                UpdateBlockFontPreview();
                UpdateBlockFontMetadata();
            }
            finally
            {
                _isUpdatingUi = false;
            }
        }

        private void PopulateBlockFontList()
        {
            if (_cmbBlockFont == null) return;
            _cmbBlockFont.Items.Clear();
            foreach (var f in FontCatalog.GetAllFonts())
            {
                _cmbBlockFont.Items.Add(f.Name);
            }
        }

        private void UpdateBlockFontFavButton()
        {
            var b = GetSelectedBlock();
            if (b == null || string.IsNullOrEmpty(b.FontFamily)) return;
            bool isFav = _preview.FavoriteFonts.Contains(b.FontFamily);
            _btnBlockFontFav.Content = isFav ? "â˜…" : "â˜†";
            _btnBlockFontFav.Foreground = isFav ? Brushes.Gold : Brushes.White;
        }

        private void UpdateBlockFontPreview()
        {
            var b = GetSelectedBlock();
            if (b == null || string.IsNullOrEmpty(b.FontFamily)) return;
            _lblBlockFontPreview.FontFamily = Fonts.For(b.FontFamily);
        }

        private void UpdateBlockFontMetadata()
        {
            if (_lblBlockFontMeta == null) return;
            var b = GetSelectedBlock();
            if (b == null || string.IsNullOrEmpty(b.FontFamily)) return;

            var curated = FontCatalog.FindCurated(b.FontFamily);
            if (curated != null)
            {
                _lblBlockFontMeta.Text = string.Format("Source: App Font | Category: {0}", curated.Category);
            }
            else
            {
                _lblBlockFontMeta.Text = "Source: System Font";
            }
        }

        private void UpdateBlockTypeVisibility()
        {
            var b = GetSelectedBlock();
            if (b == null) return;

            string t = b.Type != null ? b.Type.ToLowerInvariant() : "symbol";
            _panelBlockSymbol.Visibility = (t == "symbol") ? Visibility.Visible : Visibility.Collapsed;
            _panelBlockStatic.Visibility = (t.Contains("static")) ? Visibility.Visible : Visibility.Collapsed;
            _panelBlockRotating.Visibility = (t.Contains("rotating")) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateBlockRotationModePanels()
        {
            var b = GetSelectedBlock();
            if (b == null) return;

            string m = b.RotationMode != null ? b.RotationMode.ToLowerInvariant() : "sequential";
            _panelBlockInterval.Visibility = (m != "schedule") ? Visibility.Visible : Visibility.Collapsed;
            _panelBlockSchedule.Visibility = (m == "schedule") ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RefreshBlockMessagesList()
        {
            if (_lstBlockMessages == null) return;
            _lstBlockMessages.Items.Clear();
            var b = GetSelectedBlock();
            if (b != null && b.Messages != null)
            {
                foreach (var msg in b.Messages) _lstBlockMessages.Items.Add(msg);
            }
        }

        private void RefreshBlockScheduleList()
        {
            if (_lstBlockSchedules == null) return;
            _lstBlockSchedules.Items.Clear();
            var b = GetSelectedBlock();
            if (b != null && b.ScheduledMessages != null)
            {
                foreach (var s in b.ScheduledMessages) _lstBlockSchedules.Items.Add(s.Time + " -> " + s.Text);
            }
        }

        private UIElement CreateFontCatalogTab()
        {
            var root = new Grid { Margin = new Thickness(10) };
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var topBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };

            topBar.Children.Add(new TextBlock { Text = "Source:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) });
            _cmbCatalogSource = CreateComboBox(110);
            foreach (var s in FontCatalog.Sources) _cmbCatalogSource.Items.Add(s);
            _cmbCatalogSource.SelectedIndex = 0;
            _cmbCatalogSource.SelectionChanged += (s, e) => PopulateCatalogList();
            topBar.Children.Add(_cmbCatalogSource);

            topBar.Children.Add(new TextBlock { Text = "Category:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 4, 0) });
            _cmbCatalogCategory = CreateComboBox(115);
            foreach (var c in FontCatalog.Categories) _cmbCatalogCategory.Items.Add(c);
            _cmbCatalogCategory.SelectedIndex = 0;
            _cmbCatalogCategory.SelectionChanged += (s, e) => PopulateCatalogList();
            topBar.Children.Add(_cmbCatalogCategory);

            topBar.Children.Add(new TextBlock { Text = "Search:", Margin = new Thickness(8, 0, 4, 0), VerticalAlignment = VerticalAlignment.Center });
            _txtCatalogSearch = CreateTextBox(115);
            _txtCatalogSearch.TextChanged += (s, e) => PopulateCatalogList();
            topBar.Children.Add(_txtCatalogSearch);

            Grid.SetRow(topBar, 0);
            root.Children.Add(topBar);

            _lstCatalogFonts = new ListBox
            {
                Background = new SolidColorBrush(Color.FromRgb(28, 30, 33)),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 52, 58)),
                Margin = new Thickness(0, 0, 0, 8)
            };
            _lstCatalogFonts.SelectionChanged += (s, e) => UpdateCatalogSample();
            Grid.SetRow(_lstCatalogFonts, 1);
            root.Children.Add(_lstCatalogFonts);

            var sampleBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 52, 58)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8),
                Background = new SolidColorBrush(Color.FromRgb(22, 24, 26)),
                Margin = new Thickness(0, 0, 0, 6)
            };

            var sampleStack = new StackPanel();
            _lblCatalogFontMeta = new TextBlock
            {
                Text = "Source: App Font | Category: Futuristic",
                Foreground = Brushes.LightGray,
                FontSize = 11,
                Margin = new Thickness(0, 0, 0, 4),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            sampleStack.Children.Add(_lblCatalogFontMeta);

            _lblCatalogSample = new TextBlock
            {
                Text = "Saturday\n09:26 PM\nGOOD EVENING",
                FontSize = 18,
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center
            };
            sampleStack.Children.Add(_lblCatalogSample);
            sampleBorder.Child = sampleStack;
            Grid.SetRow(sampleBorder, 2);
            root.Children.Add(sampleBorder);

            _lblCatalogStats = new TextBlock
            {
                Text = "Curated App Fonts: 146 | System Fonts: 245 | Total: 391",
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetRow(_lblCatalogStats, 3);
            root.Children.Add(_lblCatalogStats);

            return root;
        }

        private void PopulateCatalogList()
        {
            if (_cmbCatalogCategory == null || _lstCatalogFonts == null) return;
            string src = _cmbCatalogSource != null && _cmbCatalogSource.SelectedItem != null ? _cmbCatalogSource.SelectedItem.ToString() : "All";
            string cat = _cmbCatalogCategory.SelectedItem != null ? _cmbCatalogCategory.SelectedItem.ToString() : "All";
            string search = _txtCatalogSearch != null ? _txtCatalogSearch.Text : "";

            var filtered = FontCatalog.Filter(src, cat, search, _preview.FavoriteFonts);
            _lstCatalogFonts.Items.Clear();
            foreach (var f in filtered)
            {
                bool isFav = _preview.FavoriteFonts.Contains(f.Name);
                string star = isFav ? "â˜… " : "   ";
                string tag = f.IsAppFont ? "[App]" : "[System]";
                _lstCatalogFonts.Items.Add(star + f.Name + " " + tag + " (" + f.Category + ")");
            }

            if (_lblCatalogStats != null)
            {
                int appCount = FontCatalog.CuratedAppFontCount;
                int total = FontCatalog.GetAllFonts().Count;
                int sysCount = total - appCount;
                _lblCatalogStats.Text = string.Format("Curated App Fonts: {0} | System Fonts: {1} | Total: {2} | Showing: {3}", appCount, sysCount, total, filtered.Count);
            }

            if (_lstCatalogFonts.Items.Count > 0) _lstCatalogFonts.SelectedIndex = 0;
        }

        private void UpdateCatalogSample()
        {
            if (_lstCatalogFonts.SelectedItem == null) return;
            string item = _lstCatalogFonts.SelectedItem.ToString().Trim();
            if (item.StartsWith("â˜… ")) item = item.Substring(2).Trim();
            int tagIdx = item.IndexOf(" [");
            if (tagIdx > 0) item = item.Substring(0, tagIdx).Trim();

            _lblCatalogSample.FontFamily = Fonts.For(item);

            var curated = FontCatalog.FindCurated(item);
            if (curated != null)
            {
                _lblCatalogFontMeta.Text = string.Format("Font: {0} | Source: App Font | Category: {1}", item, curated.Category);
            }
            else
            {
                _lblCatalogFontMeta.Text = string.Format("Font: {0} | Source: System Font", item);
            }
        }

        private UIElement CreatePositionTab()
        {
            var root = new StackPanel { Margin = new Thickness(12) };

            var grpPos = CreateGroupBox("Window Placement & Stable Anchor");
            var stackPos = new StackPanel { Margin = new Thickness(8) };

            _btnEditPos = CreateStyledButton("Edit Position (Ctrl+Alt+C)", 200);
            _btnEditPos.Click += (s, e) =>
            {
                _host.SetEditing(true);
                UpdatePositionButtons();
            };
            stackPos.Children.Add(_btnEditPos);

            _btnLockPos = CreateStyledButton("Lock Position", 200);
            _btnLockPos.Margin = new Thickness(0, 8, 0, 0);
            _btnLockPos.Click += (s, e) =>
            {
                _host.SetEditing(false);
                UpdatePositionButtons();
            };
            stackPos.Children.Add(_btnLockPos);

            _btnCenter = CreateStyledButton("Center on Screen", 200);
            _btnCenter.Margin = new Thickness(0, 8, 0, 0);
            _btnCenter.Click += (s, e) =>
            {
                _host.CenterOnScreen();
                UpdatePositionDisplay();
            };
            stackPos.Children.Add(_btnCenter);

            _lblCoordinates = new TextBlock
            {
                Text = "Current Anchor: (0, 0)",
                Foreground = Brushes.LightGray,
                Margin = new Thickness(0, 12, 0, 0)
            };
            stackPos.Children.Add(_lblCoordinates);

            _chkRunOnStartup = new CheckBox { Content = "Run on Windows Startup", FontWeight = FontWeights.Medium, Margin = new Thickness(0, 12, 0, 0) };
            _chkRunOnStartup.Click += (s, e) =>
            {
                if (_isUpdatingUi) return;
                _preview.RunOnStartup = _chkRunOnStartup.IsChecked == true;
                ApplyPreviewLive();
            };
            stackPos.Children.Add(_chkRunOnStartup);

            grpPos.Content = stackPos;
            root.Children.Add(grpPos);

            return root;
        }

        private void UpdatePositionButtons()
        {
            if (_btnEditPos != null) _btnEditPos.IsEnabled = !_host.IsEditing;
            if (_btnLockPos != null) _btnLockPos.IsEnabled = _host.IsEditing;
        }

        private void UpdatePositionDisplay()
        {
            if (_lblCoordinates != null)
            {
                _lblCoordinates.Text = string.Format("Anchor Center: X = {0:F1}, Y = {1:F1} | Left = {2:F1}, Top = {3:F1}", _host.AnchorX, _host.AnchorY, _host.Left, _host.Top);
            }
        }

        private void LoadValues()
        {
            _isUpdatingUi = true;
            try
            {
                _preview = _preview ?? SettingsManager.Defaults();

                _chkUseGlobalFont.IsChecked = _preview.UseGlobalFont;
                _cmbGlobalFont.SelectedItem = _preview.GlobalFont ?? "Audiowide";
                _cmbGlobalFont.IsEnabled = _preview.UseGlobalFont;

                _chkUseGlobalColor.IsChecked = _preview.UseGlobalColor;
                _lblGlobalColorHex.Text = _preview.GlobalColor ?? "#D6D3D0";
                _rectGlobalColorSwatch.Fill = new SolidColorBrush(ParseColor(_preview.GlobalColor));

                _sliderMasterScale.Value = Math.Round(_preview.Scale * 100.0);
                _lblMasterScale.Text = ((int)Math.Round(_sliderMasterScale.Value)) + "%";

                _cmbGreetingMode.SelectedIndex = _preview.GreetingMode;
                _txtCustomGreeting.Text = _preview.CustomGreeting ?? "";
                _txtCustomGreeting.IsEnabled = (_preview.GreetingMode == 1);

                _txtMorningStart.Text = _preview.MorningStart.ToString();
                _txtAfternoonStart.Text = _preview.AfternoonStart.ToString();
                _txtEveningStart.Text = _preview.EveningStart.ToString();
                _txtNightStart.Text = _preview.NightStart.ToString();

                LoadSelectedCoreElementValues();
                RefreshBlocksList();
                PopulateCatalogList();

                _chkRunOnStartup.IsChecked = _preview.RunOnStartup;
                UpdatePositionButtons();
                UpdatePositionDisplay();
            }
            finally
            {
                _isUpdatingUi = false;
            }
        }

        private void ApplyPreviewLive()
        {
            if (_host != null && _preview != null)
            {
                _host.ApplyPreview(_preview);
            }
        }

        private static Color ParseColor(string hex)
        {
            try { return (Color)ColorConverter.ConvertFromString(hex); }
            catch { return Color.FromRgb(0xD6, 0xD3, 0xD0); }
        }

        private static GroupBox CreateGroupBox(string header)
        {
            return new GroupBox
            {
                Header = " " + header + " ",
                Foreground = Brushes.LightGray,
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 52, 58)),
                Margin = new Thickness(0, 0, 0, 10)
            };
        }

        private static TextBox CreateTextBox(double width)
        {
            return new TextBox
            {
                Width = width,
                Background = new SolidColorBrush(Color.FromRgb(24, 26, 28)),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(60, 62, 68)),
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(3, 2, 3, 2)
            };
        }

        private static ComboBox CreateComboBox(double width)
        {
            return new ComboBox
            {
                Width = width,
                Background = new SolidColorBrush(Color.FromRgb(24, 26, 28)),
                Foreground = Brushes.Black,
                BorderBrush = new SolidColorBrush(Color.FromRgb(60, 62, 68)),
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private static Button CreateStyledButton(string text, double width)
        {
            return new Button
            {
                Content = text,
                Width = width,
                Height = 26,
                Background = new SolidColorBrush(Color.FromRgb(45, 48, 54)),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(70, 72, 80)),
                Margin = new Thickness(0, 0, 6, 0)
            };
        }
    }
}