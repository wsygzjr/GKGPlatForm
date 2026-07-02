using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using ReactiveUI;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using GKG.Map.MapCell.Generic.Control.Shared;

using AControls = Avalonia.Controls;
using APrimitives = Avalonia.Controls.Primitives;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox.MapOprtCellParamCfgView
{
    internal static class TextBoxOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));

        internal static T? FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    internal abstract class TextBoxOprtCellParamViewBase : AControls.UserControl
    {
        protected static AControls.StackPanel CreateRoot() => new AControls.StackPanel { Margin = new Thickness(20), Spacing = 12 };

        protected static AControls.StackPanel CreateRow(string label, AControls.Control editor)
        {
            var row = new AControls.StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
            row.Children.Add(new AControls.TextBlock { Text = label, Width = 150, VerticalAlignment = VerticalAlignment.Center });
            row.Children.Add(editor);
            return row;
        }

        protected static AControls.TextBox CreateTextBox(string bindingPath, double width = 160)
        {
            var tb = new AControls.TextBox { Width = width };
            tb.Bind(AControls.TextBox.TextProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return tb;
        }

        protected static AControls.CheckBox CreateCheckBox(string content, string bindingPath)
        {
            var cb = new AControls.CheckBox { Content = content };
            cb.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return cb;
        }

        protected static AControls.ComboBox CreateEnumComboBox<TEnum>(string bindingPath, double width = 220) where TEnum : struct, Enum
        {
            var cb = new AControls.ComboBox { Width = width, ItemsSource = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList() };
            cb.Bind(APrimitives.SelectingItemsControl.SelectedItemProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return cb;
        }

        protected static ColorPickerEditor CreateColorPicker(string bindingPath)
        {
            var picker = new ColorPickerEditor();
            picker.Bind(ColorPickerEditor.ColorStringProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return picker;
        }
    }

    internal sealed class BrushInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _backgroundColor = "#FFFFFFFF";
        public string BackgroundColor { get => _backgroundColor; set => this.RaiseAndSetIfChanged(ref _backgroundColor, value); }

        private string _borderColor = "#FF808080";
        public string BorderColor { get => _borderColor; set => this.RaiseAndSetIfChanged(ref _borderColor, value); }

        private string _foregroundColor = "#FF000000";
        public string ForegroundColor { get => _foregroundColor; set => this.RaiseAndSetIfChanged(ref _foregroundColor, value); }

        private string _selectedBorderColor = "#FF1E90FF";
        public string SelectedBorderColor { get => _selectedBorderColor; set => this.RaiseAndSetIfChanged(ref _selectedBorderColor, value); }

        public void FromBytes(byte[] data)
        {
            var temp = TextBoxOprtCellCfgJson.FromBytes<BrushInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            BackgroundColor = temp.BackgroundColor ?? BackgroundColor;
            BorderColor = temp.BorderColor ?? BorderColor;
            ForegroundColor = temp.ForegroundColor ?? ForegroundColor;
            SelectedBorderColor = temp.SelectedBorderColor ?? SelectedBorderColor;
        }

        public byte[] ToBytes() => TextBoxOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class BrushInfoMapOprtCellParamView : TextBoxOprtCellParamViewBase
    {
        public BrushInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("背景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BackgroundColor))));
            root.Children.Add(CreateRow("边框色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BorderColor))));
            root.Children.Add(CreateRow("前景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ForegroundColor))));
            root.Children.Add(CreateRow("选中边框色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.SelectedBorderColor))));
            Content = root;
        }
    }

    internal sealed class AppearanceInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _opacity = "0";
        public string Opacity { get => _opacity; set => this.RaiseAndSetIfChanged(ref _opacity, value); }

        private string _borderThicknessLeft = "1";
        public string BorderThicknessLeft { get => _borderThicknessLeft; set => this.RaiseAndSetIfChanged(ref _borderThicknessLeft, value); }

        private string _borderThicknessTop = "1";
        public string BorderThicknessTop { get => _borderThicknessTop; set => this.RaiseAndSetIfChanged(ref _borderThicknessTop, value); }

        private string _borderThicknessRight = "1";
        public string BorderThicknessRight { get => _borderThicknessRight; set => this.RaiseAndSetIfChanged(ref _borderThicknessRight, value); }

        private string _borderThicknessBottom = "1";
        public string BorderThicknessBottom { get => _borderThicknessBottom; set => this.RaiseAndSetIfChanged(ref _borderThicknessBottom, value); }

        public void FromBytes(byte[] data)
        {
            var temp = TextBoxOprtCellCfgJson.FromBytes<AppearanceInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            Opacity = temp.Opacity ?? Opacity;
            BorderThicknessLeft = temp.BorderThicknessLeft ?? BorderThicknessLeft;
            BorderThicknessTop = temp.BorderThicknessTop ?? BorderThicknessTop;
            BorderThicknessRight = temp.BorderThicknessRight ?? BorderThicknessRight;
            BorderThicknessBottom = temp.BorderThicknessBottom ?? BorderThicknessBottom;
        }

        public byte[] ToBytes() => TextBoxOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class AppearanceInfoMapOprtCellParamView : TextBoxOprtCellParamViewBase
    {
        public AppearanceInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("透明度(0-100)", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.Opacity), 160)));
            root.Children.Add(CreateRow("边框左", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessLeft), 120)));
            root.Children.Add(CreateRow("边框上", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessTop), 120)));
            root.Children.Add(CreateRow("边框右", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessRight), 120)));
            root.Children.Add(CreateRow("边框下", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessBottom), 120)));
            Content = root;
        }
    }

    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _text = "";
        public string Text { get => _text; set => this.RaiseAndSetIfChanged(ref _text, value); }

        private string _tooltipText = "";
        public string TooltipText { get => _tooltipText; set => this.RaiseAndSetIfChanged(ref _tooltipText, value); }

        private bool _enabled = true;
        public bool Enabled { get => _enabled; set => this.RaiseAndSetIfChanged(ref _enabled, value); }

        private bool _isReadOnly;
        public bool IsReadOnly { get => _isReadOnly; set => this.RaiseAndSetIfChanged(ref _isReadOnly, value); }

        private CommonCursorType _hoverCursor = CommonCursorType.文本输入;
        public CommonCursorType HoverCursor { get => _hoverCursor; set => this.RaiseAndSetIfChanged(ref _hoverCursor, value); }

        private string _selectedTextOpacity = "100";
        public string SelectedTextOpacity { get => _selectedTextOpacity; set => this.RaiseAndSetIfChanged(ref _selectedTextOpacity, value); }

        private bool _enableSpellCheck;
        public bool EnableSpellCheck { get => _enableSpellCheck; set => this.RaiseAndSetIfChanged(ref _enableSpellCheck, value); }

        public void FromBytes(byte[] data)
        {
            var temp = TextBoxOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            Text = temp.Text ?? Text;
            TooltipText = temp.TooltipText ?? TooltipText;
            Enabled = temp.Enabled;
            IsReadOnly = temp.IsReadOnly;
            HoverCursor = temp.HoverCursor;
            SelectedTextOpacity = temp.SelectedTextOpacity ?? SelectedTextOpacity;
            EnableSpellCheck = temp.EnableSpellCheck;
        }

        public byte[] ToBytes() => TextBoxOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class CommonInfoMapOprtCellParamView : TextBoxOprtCellParamViewBase
    {
        public CommonInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("文本内容", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.Text), 360)));
            root.Children.Add(CreateRow("提示文本", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.TooltipText), 260)));
            root.Children.Add(CreateRow("启用", CreateCheckBox("启用", nameof(CommonInfoMapOprtCellParamViewModel.Enabled))));
            root.Children.Add(CreateRow("只读", CreateCheckBox("只读", nameof(CommonInfoMapOprtCellParamViewModel.IsReadOnly))));
            root.Children.Add(CreateRow("悬停光标", CreateEnumComboBox<CommonCursorType>(nameof(CommonInfoMapOprtCellParamViewModel.HoverCursor), 220)));
            root.Children.Add(CreateRow("选中文本不透明度(0-100)", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.SelectedTextOpacity), 160)));
            root.Children.Add(CreateRow("拼写检查", CreateCheckBox("启用", nameof(CommonInfoMapOprtCellParamViewModel.EnableSpellCheck))));
            Content = root;
        }
    }

    internal sealed class LayoutInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private TextBoxLayoutInfo.HorizontalAlignmentEnum _horizontalAlignment = TextBoxLayoutInfo.HorizontalAlignmentEnum.Stretch;
        public TextBoxLayoutInfo.HorizontalAlignmentEnum HorizontalAlignment { get => _horizontalAlignment; set => this.RaiseAndSetIfChanged(ref _horizontalAlignment, value); }

        private TextBoxLayoutInfo.VerticalAlignmentEnum _verticalAlignment = TextBoxLayoutInfo.VerticalAlignmentEnum.Stretch;
        public TextBoxLayoutInfo.VerticalAlignmentEnum VerticalAlignment { get => _verticalAlignment; set => this.RaiseAndSetIfChanged(ref _verticalAlignment, value); }

        private string _marginLeft = "0";
        public string MarginLeft { get => _marginLeft; set => this.RaiseAndSetIfChanged(ref _marginLeft, value); }

        private string _marginTop = "0";
        public string MarginTop { get => _marginTop; set => this.RaiseAndSetIfChanged(ref _marginTop, value); }

        private string _marginRight = "0";
        public string MarginRight { get => _marginRight; set => this.RaiseAndSetIfChanged(ref _marginRight, value); }

        private string _marginBottom = "0";
        public string MarginBottom { get => _marginBottom; set => this.RaiseAndSetIfChanged(ref _marginBottom, value); }

        private string _minWidth = "0";
        public string MinWidth { get => _minWidth; set => this.RaiseAndSetIfChanged(ref _minWidth, value); }

        private string _maxWidth = "2147483647";
        public string MaxWidth { get => _maxWidth; set => this.RaiseAndSetIfChanged(ref _maxWidth, value); }

        private string _minHeight = "0";
        public string MinHeight { get => _minHeight; set => this.RaiseAndSetIfChanged(ref _minHeight, value); }

        private string _maxHeight = "2147483647";
        public string MaxHeight { get => _maxHeight; set => this.RaiseAndSetIfChanged(ref _maxHeight, value); }

        public void FromBytes(byte[] data)
        {
            var temp = TextBoxOprtCellCfgJson.FromBytes<LayoutInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            HorizontalAlignment = temp.HorizontalAlignment;
            VerticalAlignment = temp.VerticalAlignment;
            MarginLeft = temp.MarginLeft ?? MarginLeft;
            MarginTop = temp.MarginTop ?? MarginTop;
            MarginRight = temp.MarginRight ?? MarginRight;
            MarginBottom = temp.MarginBottom ?? MarginBottom;
            MinWidth = temp.MinWidth ?? MinWidth;
            MaxWidth = temp.MaxWidth ?? MaxWidth;
            MinHeight = temp.MinHeight ?? MinHeight;
            MaxHeight = temp.MaxHeight ?? MaxHeight;
        }

        public byte[] ToBytes() => TextBoxOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class LayoutInfoMapOprtCellParamView : TextBoxOprtCellParamViewBase
    {
        public LayoutInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("水平对齐", CreateEnumComboBox<TextBoxLayoutInfo.HorizontalAlignmentEnum>(nameof(LayoutInfoMapOprtCellParamViewModel.HorizontalAlignment), 220)));
            root.Children.Add(CreateRow("垂直对齐", CreateEnumComboBox<TextBoxLayoutInfo.VerticalAlignmentEnum>(nameof(LayoutInfoMapOprtCellParamViewModel.VerticalAlignment), 220)));
            root.Children.Add(CreateRow("外边距左", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginLeft), 120)));
            root.Children.Add(CreateRow("外边距上", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginTop), 120)));
            root.Children.Add(CreateRow("外边距右", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginRight), 120)));
            root.Children.Add(CreateRow("外边距下", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginBottom), 120)));
            root.Children.Add(CreateRow("最小宽度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MinWidth), 120)));
            root.Children.Add(CreateRow("最大宽度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MaxWidth), 160)));
            root.Children.Add(CreateRow("最小高度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MinHeight), 120)));
            root.Children.Add(CreateRow("最大高度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MaxHeight), 160)));
            Content = root;
        }
    }

    internal sealed class TextInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private TextBoxFontFamilyType _fontFamily = TextBoxFontFamilyType.微软雅黑;
        public TextBoxFontFamilyType FontFamily { get => _fontFamily; set => this.RaiseAndSetIfChanged(ref _fontFamily, value); }

        private string _fontColor = "#FF000000";
        public string FontColor { get => _fontColor; set => this.RaiseAndSetIfChanged(ref _fontColor, value); }

        private string _fontSize = "14";
        public string FontSize { get => _fontSize; set => this.RaiseAndSetIfChanged(ref _fontSize, value); }

        private bool _isItalic;
        public bool IsItalic { get => _isItalic; set => this.RaiseAndSetIfChanged(ref _isItalic, value); }

        private bool _isBold;
        public bool IsBold { get => _isBold; set => this.RaiseAndSetIfChanged(ref _isBold, value); }

        private TextBoxTextAlignmentType _textAlignment = TextBoxTextAlignmentType.Left;
        public TextBoxTextAlignmentType TextAlignment { get => _textAlignment; set => this.RaiseAndSetIfChanged(ref _textAlignment, value); }

        private TextBoxVerticalTextAlignmentType _verticalTextAlignment = TextBoxVerticalTextAlignmentType.Center;
        public TextBoxVerticalTextAlignmentType VerticalTextAlignment { get => _verticalTextAlignment; set => this.RaiseAndSetIfChanged(ref _verticalTextAlignment, value); }

        public void FromBytes(byte[] data)
        {
            var temp = TextBoxOprtCellCfgJson.FromBytes<TextInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            FontFamily = temp.FontFamily;
            FontColor = temp.FontColor ?? FontColor;
            FontSize = temp.FontSize ?? FontSize;
            IsItalic = temp.IsItalic;
            IsBold = temp.IsBold;
            TextAlignment = temp.TextAlignment;
            VerticalTextAlignment = temp.VerticalTextAlignment;
        }

        public byte[] ToBytes() => TextBoxOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class TextInfoMapOprtCellParamView : TextBoxOprtCellParamViewBase
    {
        public TextInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("字体族", CreateEnumComboBox<TextBoxFontFamilyType>(nameof(TextInfoMapOprtCellParamViewModel.FontFamily), 220)));
            root.Children.Add(CreateRow("字体颜色", CreateTextBox(nameof(TextInfoMapOprtCellParamViewModel.FontColor), 220)));
            root.Children.Add(CreateRow("字体大小", CreateTextBox(nameof(TextInfoMapOprtCellParamViewModel.FontSize), 120)));
            root.Children.Add(CreateRow("斜体", CreateCheckBox("斜体", nameof(TextInfoMapOprtCellParamViewModel.IsItalic))));
            root.Children.Add(CreateRow("加粗", CreateCheckBox("加粗", nameof(TextInfoMapOprtCellParamViewModel.IsBold))));
            root.Children.Add(CreateRow("水平文本对齐", CreateEnumComboBox<TextBoxTextAlignmentType>(nameof(TextInfoMapOprtCellParamViewModel.TextAlignment), 220)));
            root.Children.Add(CreateRow("垂直文本对齐", CreateEnumComboBox<TextBoxVerticalTextAlignmentType>(nameof(TextInfoMapOprtCellParamViewModel.VerticalTextAlignment), 220)));
            Content = root;
        }
    }

    #region CfgView wrappers (implement IMapOprtCellParamCfgView)

    internal sealed class BrushInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly BrushInfoMapOprtCellParamView _view;
        private readonly BrushInfoMapOprtCellParamViewModel _vm;
        public BrushInfoMapOprtCellParamCfgView() { _vm = new BrushInfoMapOprtCellParamViewModel(); _view = new BrushInfoMapOprtCellParamView { DataContext = _vm }; }
        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    internal sealed class AppearanceInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly AppearanceInfoMapOprtCellParamView _view;
        private readonly AppearanceInfoMapOprtCellParamViewModel _vm;
        public AppearanceInfoMapOprtCellParamCfgView() { _vm = new AppearanceInfoMapOprtCellParamViewModel(); _view = new AppearanceInfoMapOprtCellParamView { DataContext = _vm }; }
        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    internal sealed class CommonInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly CommonInfoMapOprtCellParamView _view;
        private readonly CommonInfoMapOprtCellParamViewModel _vm;
        public CommonInfoMapOprtCellParamCfgView() { _vm = new CommonInfoMapOprtCellParamViewModel(); _view = new CommonInfoMapOprtCellParamView { DataContext = _vm }; }
        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    internal sealed class LayoutInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly LayoutInfoMapOprtCellParamView _view;
        private readonly LayoutInfoMapOprtCellParamViewModel _vm;
        public LayoutInfoMapOprtCellParamCfgView() { _vm = new LayoutInfoMapOprtCellParamViewModel(); _view = new LayoutInfoMapOprtCellParamView { DataContext = _vm }; }
        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    internal sealed class TextInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly TextInfoMapOprtCellParamView _view;
        private readonly TextInfoMapOprtCellParamViewModel _vm;
        public TextInfoMapOprtCellParamCfgView() { _vm = new TextInfoMapOprtCellParamViewModel(); _view = new TextInfoMapOprtCellParamView { DataContext = _vm }; }
        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    #endregion
}
