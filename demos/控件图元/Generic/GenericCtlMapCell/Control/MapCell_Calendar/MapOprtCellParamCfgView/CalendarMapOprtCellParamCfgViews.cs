using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects;
using GKG.Map.MapCell.Generic.Control.Shared;
using ReactiveUI;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar.MapOprtCellParamCfgView
{
    internal static class CalendarOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));

        internal static T? FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0)
                return default;
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    internal abstract class CalendarOprtCellParamViewBase : UserControl
    {
        protected static StackPanel CreateRoot() => new() { Margin = new Thickness(20), Spacing = 12 };

        protected static StackPanel CreateRow(string label, Avalonia.Controls.Control editor)
        {
            StackPanel row = new() { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
            row.Children.Add(new TextBlock { Text = label, Width = 150, VerticalAlignment = VerticalAlignment.Center });
            row.Children.Add(editor);
            return row;
        }

        protected static TextBox CreateTextBox(string bindingPath, double width = 180)
        {
            TextBox textBox = new() { Width = width };
            textBox.Bind(TextBox.TextProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return textBox;
        }

        protected static Avalonia.Controls.CheckBox CreateCheckBox(string content, string bindingPath)
        {
            Avalonia.Controls.CheckBox checkBox = new() { Content = content };
            checkBox.Bind(Avalonia.Controls.Primitives.ToggleButton.IsCheckedProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return checkBox;
        }

        protected static ComboBox CreateEnumComboBox<TEnum>(string bindingPath, double width = 220) where TEnum : struct, Enum
        {
            ComboBox comboBox = new() { Width = width, ItemsSource = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList() };
            comboBox.Bind(Avalonia.Controls.Primitives.SelectingItemsControl.SelectedItemProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return comboBox;
        }

        protected static ColorPickerEditor CreateColorPicker(string bindingPath)
        {
            ColorPickerEditor editor = new();
            editor.Bind(ColorPickerEditor.ColorStringProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return editor;
        }
    }

    internal sealed class BrushInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _backgroundColor = "#FFFFFFFF";
        private string _borderColor = "#FF808080";
        private string _foregroundColor = "#FF000000";
        public string BackgroundColor { get => _backgroundColor; set => this.RaiseAndSetIfChanged(ref _backgroundColor, value); }
        public string BorderColor { get => _borderColor; set => this.RaiseAndSetIfChanged(ref _borderColor, value); }
        public string ForegroundColor { get => _foregroundColor; set => this.RaiseAndSetIfChanged(ref _foregroundColor, value); }
        public void FromBytes(byte[] data) { var temp = CalendarOprtCellCfgJson.FromBytes<BrushInfoMapOprtCellParamViewModel>(data); if (temp == null) return; BackgroundColor = temp.BackgroundColor ?? BackgroundColor; BorderColor = temp.BorderColor ?? BorderColor; ForegroundColor = temp.ForegroundColor ?? ForegroundColor; }
        public byte[] ToBytes() => CalendarOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class BrushInfoMapOprtCellParamCfgView : CalendarOprtCellParamViewBase
    {
        public BrushInfoMapOprtCellParamCfgView()
        {
            StackPanel root = CreateRoot();
            root.Children.Add(CreateRow("背景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BackgroundColor))));
            root.Children.Add(CreateRow("边框颜色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BorderColor))));
            root.Children.Add(CreateRow("前景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ForegroundColor))));
            Content = root;
        }
    }

    internal sealed class AppearanceInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _opacity = "0";
        private string _borderThicknessLeft = "1";
        private string _borderThicknessTop = "1";
        private string _borderThicknessRight = "1";
        private string _borderThicknessBottom = "1";
        public string Opacity { get => _opacity; set => this.RaiseAndSetIfChanged(ref _opacity, value); }
        public string BorderThicknessLeft { get => _borderThicknessLeft; set => this.RaiseAndSetIfChanged(ref _borderThicknessLeft, value); }
        public string BorderThicknessTop { get => _borderThicknessTop; set => this.RaiseAndSetIfChanged(ref _borderThicknessTop, value); }
        public string BorderThicknessRight { get => _borderThicknessRight; set => this.RaiseAndSetIfChanged(ref _borderThicknessRight, value); }
        public string BorderThicknessBottom { get => _borderThicknessBottom; set => this.RaiseAndSetIfChanged(ref _borderThicknessBottom, value); }
        public void FromBytes(byte[] data) { var temp = CalendarOprtCellCfgJson.FromBytes<AppearanceInfoMapOprtCellParamViewModel>(data); if (temp == null) return; Opacity = temp.Opacity ?? Opacity; BorderThicknessLeft = temp.BorderThicknessLeft ?? BorderThicknessLeft; BorderThicknessTop = temp.BorderThicknessTop ?? BorderThicknessTop; BorderThicknessRight = temp.BorderThicknessRight ?? BorderThicknessRight; BorderThicknessBottom = temp.BorderThicknessBottom ?? BorderThicknessBottom; }
        public byte[] ToBytes() => CalendarOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class AppearanceInfoMapOprtCellParamCfgView : CalendarOprtCellParamViewBase
    {
        public AppearanceInfoMapOprtCellParamCfgView()
        {
            StackPanel root = CreateRoot();
            root.Children.Add(CreateRow("透明度(0-100)", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.Opacity))));
            root.Children.Add(CreateRow("左边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessLeft), 120)));
            root.Children.Add(CreateRow("上边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessTop), 120)));
            root.Children.Add(CreateRow("右边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessRight), 120)));
            root.Children.Add(CreateRow("下边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessBottom), 120)));
            Content = root;
        }
    }

    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private CalendarCursorType _hoverCursor = CalendarCursorType.Arrow;
        private bool _enabled = true;
        private bool _showButtonPanel = true;
        public CalendarCursorType HoverCursor { get => _hoverCursor; set => this.RaiseAndSetIfChanged(ref _hoverCursor, value); }
        public bool Enabled { get => _enabled; set => this.RaiseAndSetIfChanged(ref _enabled, value); }
        public bool ShowButtonPanel { get => _showButtonPanel; set => this.RaiseAndSetIfChanged(ref _showButtonPanel, value); }
        public void FromBytes(byte[] data) { var temp = CalendarOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data); if (temp == null) return; HoverCursor = temp.HoverCursor; Enabled = temp.Enabled; ShowButtonPanel = temp.ShowButtonPanel; }
        public byte[] ToBytes() => CalendarOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class CommonInfoMapOprtCellParamCfgView : CalendarOprtCellParamViewBase
    {
        public CommonInfoMapOprtCellParamCfgView()
        {
            StackPanel root = CreateRoot();
            root.Children.Add(CreateRow("悬停光标", CreateEnumComboBox<CalendarCursorType>(nameof(CommonInfoMapOprtCellParamViewModel.HoverCursor))));
            root.Children.Add(CreateRow("是否启用", CreateCheckBox("启用", nameof(CommonInfoMapOprtCellParamViewModel.Enabled))));
            root.Children.Add(CreateRow("ShowButtonPanel", CreateCheckBox("显示底部按钮", nameof(CommonInfoMapOprtCellParamViewModel.ShowButtonPanel))));
            Content = root;
        }
    }

    internal sealed class LayoutInfoMapOprtCellParamViewModel : ReactiveObject
    {
        public CalendarHorizontalAlignmentType HorizontalAlignment { get => _horizontalAlignment; set => this.RaiseAndSetIfChanged(ref _horizontalAlignment, value); } private CalendarHorizontalAlignmentType _horizontalAlignment = CalendarHorizontalAlignmentType.Stretch;
        public CalendarVerticalAlignmentType VerticalAlignment { get => _verticalAlignment; set => this.RaiseAndSetIfChanged(ref _verticalAlignment, value); } private CalendarVerticalAlignmentType _verticalAlignment = CalendarVerticalAlignmentType.Stretch;
        public string MarginLeft { get => _marginLeft; set => this.RaiseAndSetIfChanged(ref _marginLeft, value); } private string _marginLeft = "0";
        public string MarginTop { get => _marginTop; set => this.RaiseAndSetIfChanged(ref _marginTop, value); } private string _marginTop = "0";
        public string MarginRight { get => _marginRight; set => this.RaiseAndSetIfChanged(ref _marginRight, value); } private string _marginRight = "0";
        public string MarginBottom { get => _marginBottom; set => this.RaiseAndSetIfChanged(ref _marginBottom, value); } private string _marginBottom = "0";
        public string MinWidth { get => _minWidth; set => this.RaiseAndSetIfChanged(ref _minWidth, value); } private string _minWidth = "0";
        public string MaxWidth { get => _maxWidth; set => this.RaiseAndSetIfChanged(ref _maxWidth, value); } private string _maxWidth = "10000";
        public string MinHeight { get => _minHeight; set => this.RaiseAndSetIfChanged(ref _minHeight, value); } private string _minHeight = "0";
        public string MaxHeight { get => _maxHeight; set => this.RaiseAndSetIfChanged(ref _maxHeight, value); } private string _maxHeight = "10000";
        public void FromBytes(byte[] data) { var temp = CalendarOprtCellCfgJson.FromBytes<LayoutInfoMapOprtCellParamViewModel>(data); if (temp == null) return; HorizontalAlignment = temp.HorizontalAlignment; VerticalAlignment = temp.VerticalAlignment; MarginLeft = temp.MarginLeft ?? MarginLeft; MarginTop = temp.MarginTop ?? MarginTop; MarginRight = temp.MarginRight ?? MarginRight; MarginBottom = temp.MarginBottom ?? MarginBottom; MinWidth = temp.MinWidth ?? MinWidth; MaxWidth = temp.MaxWidth ?? MaxWidth; MinHeight = temp.MinHeight ?? MinHeight; MaxHeight = temp.MaxHeight ?? MaxHeight; }
        public byte[] ToBytes() => CalendarOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class LayoutInfoMapOprtCellParamCfgView : CalendarOprtCellParamViewBase
    {
        public LayoutInfoMapOprtCellParamCfgView()
        {
            StackPanel root = CreateRoot();
            root.Children.Add(CreateRow("水平对齐", CreateEnumComboBox<CalendarHorizontalAlignmentType>(nameof(LayoutInfoMapOprtCellParamViewModel.HorizontalAlignment))));
            root.Children.Add(CreateRow("垂直对齐", CreateEnumComboBox<CalendarVerticalAlignmentType>(nameof(LayoutInfoMapOprtCellParamViewModel.VerticalAlignment))));
            root.Children.Add(CreateRow("左外边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginLeft), 120)));
            root.Children.Add(CreateRow("上外边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginTop), 120)));
            root.Children.Add(CreateRow("右外边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginRight), 120)));
            root.Children.Add(CreateRow("下外边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginBottom), 120)));
            root.Children.Add(CreateRow("最小宽度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MinWidth), 120)));
            root.Children.Add(CreateRow("最大宽度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MaxWidth), 120)));
            root.Children.Add(CreateRow("最小高度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MinHeight), 120)));
            root.Children.Add(CreateRow("最大高度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MaxHeight), 120)));
            Content = root;
        }
    }

    internal sealed class TextInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private CalendarFontFamilyType _fontFamily = CalendarFontFamilyType.YaHei;
        private string _fontColor = "#FF000000";
        private string _fontSize = "14";
        private bool _isItalic;
        private bool _isBold;
        public CalendarFontFamilyType FontFamily { get => _fontFamily; set => this.RaiseAndSetIfChanged(ref _fontFamily, value); }
        public string FontColor { get => _fontColor; set => this.RaiseAndSetIfChanged(ref _fontColor, value); }
        public string FontSize { get => _fontSize; set => this.RaiseAndSetIfChanged(ref _fontSize, value); }
        public bool IsItalic { get => _isItalic; set => this.RaiseAndSetIfChanged(ref _isItalic, value); }
        public bool IsBold { get => _isBold; set => this.RaiseAndSetIfChanged(ref _isBold, value); }
        public void FromBytes(byte[] data) { var temp = CalendarOprtCellCfgJson.FromBytes<TextInfoMapOprtCellParamViewModel>(data); if (temp == null) return; FontFamily = temp.FontFamily; FontColor = temp.FontColor ?? FontColor; FontSize = temp.FontSize ?? FontSize; IsItalic = temp.IsItalic; IsBold = temp.IsBold; }
        public byte[] ToBytes() => CalendarOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class TextInfoMapOprtCellParamCfgView : CalendarOprtCellParamViewBase
    {
        public TextInfoMapOprtCellParamCfgView()
        {
            StackPanel root = CreateRoot();
            root.Children.Add(CreateRow("字体簇", CreateEnumComboBox<CalendarFontFamilyType>(nameof(TextInfoMapOprtCellParamViewModel.FontFamily))));
            root.Children.Add(CreateRow("字体颜色", CreateColorPicker(nameof(TextInfoMapOprtCellParamViewModel.FontColor))));
            root.Children.Add(CreateRow("字体大小", CreateTextBox(nameof(TextInfoMapOprtCellParamViewModel.FontSize), 120)));
            root.Children.Add(CreateRow("是否斜体", CreateCheckBox("斜体", nameof(TextInfoMapOprtCellParamViewModel.IsItalic))));
            root.Children.Add(CreateRow("是否加粗", CreateCheckBox("加粗", nameof(TextInfoMapOprtCellParamViewModel.IsBold))));
            Content = root;
        }
    }

    internal sealed class MiscInfoMapOprtCellParamViewModel : ReactiveObject
    {
        public string SelectedDate { get => _selectedDate; set => this.RaiseAndSetIfChanged(ref _selectedDate, value); } private string _selectedDate = string.Empty;
        public string SelectedDates { get => _selectedDates; set => this.RaiseAndSetIfChanged(ref _selectedDates, value); } private string _selectedDates = "[]";
        public string DisplayDate { get => _displayDate; set => this.RaiseAndSetIfChanged(ref _displayDate, value); } private string _displayDate = CalendarValueHelpers.FormatDate(DateTime.Today);
        public string DisplayDateStart { get => _displayDateStart; set => this.RaiseAndSetIfChanged(ref _displayDateStart, value); } private string _displayDateStart = string.Empty;
        public string DisplayDateEnd { get => _displayDateEnd; set => this.RaiseAndSetIfChanged(ref _displayDateEnd, value); } private string _displayDateEnd = string.Empty;
        public CalendarFirstDayOfWeekType FirstDayOfWeek { get => _firstDayOfWeek; set => this.RaiseAndSetIfChanged(ref _firstDayOfWeek, value); } private CalendarFirstDayOfWeekType _firstDayOfWeek = CalendarFirstDayOfWeekType.Sunday;
        public CalendarSelectionModeType SelectionMode { get => _selectionMode; set => this.RaiseAndSetIfChanged(ref _selectionMode, value); } private CalendarSelectionModeType _selectionMode = CalendarSelectionModeType.SingleDate;
        public bool IsTodayHighlighted { get => _isTodayHighlighted; set => this.RaiseAndSetIfChanged(ref _isTodayHighlighted, value); } private bool _isTodayHighlighted = true;
        public CalendarDisplayModeType DisplayMode { get => _displayMode; set => this.RaiseAndSetIfChanged(ref _displayMode, value); } private CalendarDisplayModeType _displayMode = CalendarDisplayModeType.Day;
        public string BlackoutDates { get => _blackoutDates; set => this.RaiseAndSetIfChanged(ref _blackoutDates, value); } private string _blackoutDates = "[]";
        public void FromBytes(byte[] data) { var temp = CalendarOprtCellCfgJson.FromBytes<MiscInfoMapOprtCellParamViewModel>(data); if (temp == null) return; SelectedDate = temp.SelectedDate ?? SelectedDate; SelectedDates = temp.SelectedDates ?? SelectedDates; DisplayDate = temp.DisplayDate ?? DisplayDate; DisplayDateStart = temp.DisplayDateStart ?? DisplayDateStart; DisplayDateEnd = temp.DisplayDateEnd ?? DisplayDateEnd; FirstDayOfWeek = temp.FirstDayOfWeek; SelectionMode = temp.SelectionMode; IsTodayHighlighted = temp.IsTodayHighlighted; DisplayMode = temp.DisplayMode; BlackoutDates = temp.BlackoutDates ?? BlackoutDates; }
        public byte[] ToBytes() => CalendarOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class MiscInfoMapOprtCellParamCfgView : CalendarOprtCellParamViewBase
    {
        public MiscInfoMapOprtCellParamCfgView()
        {
            StackPanel root = CreateRoot();
            root.Children.Add(CreateRow("SelectedDate", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.SelectedDate), 220)));
            root.Children.Add(CreateRow("SelectedDates", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.SelectedDates), 360)));
            root.Children.Add(CreateRow("DisplayDate", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.DisplayDate), 220)));
            root.Children.Add(CreateRow("显示开始日期", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.DisplayDateStart), 220)));
            root.Children.Add(CreateRow("显示结束日期", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.DisplayDateEnd), 220)));
            root.Children.Add(CreateRow("FirstDayOfWeek", CreateEnumComboBox<CalendarFirstDayOfWeekType>(nameof(MiscInfoMapOprtCellParamViewModel.FirstDayOfWeek))));
            root.Children.Add(CreateRow("SelectionMode", CreateEnumComboBox<CalendarSelectionModeType>(nameof(MiscInfoMapOprtCellParamViewModel.SelectionMode))));
            root.Children.Add(CreateRow("IsTodayHighlighted", CreateCheckBox("高亮今天", nameof(MiscInfoMapOprtCellParamViewModel.IsTodayHighlighted))));
            root.Children.Add(CreateRow("DisplayMode", CreateEnumComboBox<CalendarDisplayModeType>(nameof(MiscInfoMapOprtCellParamViewModel.DisplayMode))));
            var blackoutTextBox = CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.BlackoutDates), 360);
            blackoutTextBox.AcceptsReturn = true;
            blackoutTextBox.MinHeight = 90;
            root.Children.Add(CreateRow("BlackoutDates", blackoutTextBox));
            Content = root;
        }
    }
}
