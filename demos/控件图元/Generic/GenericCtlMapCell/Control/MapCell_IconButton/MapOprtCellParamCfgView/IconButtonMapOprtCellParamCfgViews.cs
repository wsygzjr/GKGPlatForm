using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using ReactiveUI;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox;
using GKG.Map.MapCell.Generic.Control.Shared;
using Griffins;

using AControls = Avalonia.Controls;
using APrimitives = Avalonia.Controls.Primitives;

namespace GKG.Map.MapCell.Generic.IconButton.MapOprtCellParamCfgView
{
    internal static class IconButtonOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));

        internal static T? FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    internal abstract class IconButtonOprtCellParamViewBase : AControls.UserControl
    {
        protected static AControls.StackPanel CreateRoot() => new AControls.StackPanel { Margin = new Thickness(20), Spacing = 12 };

        protected static AControls.StackPanel CreateRow(string label, AControls.Control editor)
        {
            var row = new AControls.StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
            row.Children.Add(new AControls.TextBlock { Text = label, Width = 150, VerticalAlignment = VerticalAlignment.Center });
            row.Children.Add(editor);
            return row;
        }

        protected static AControls.TextBox CreateTextBox(string bindingPath, double width = 120)
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

        protected static AControls.ComboBox CreateEnumComboBox<TEnum>(string bindingPath, double width = 200) where TEnum : struct, Enum
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
        private string _backgroundColor = "#FF00FF00";
        public string BackgroundColor { get => _backgroundColor; set => this.RaiseAndSetIfChanged(ref _backgroundColor, value); }

        private string _borderColor = "#FF00FF00";
        public string BorderColor { get => _borderColor; set => this.RaiseAndSetIfChanged(ref _borderColor, value); }

        private string _foregroundColor = "#FF000000";
        public string ForegroundColor { get => _foregroundColor; set => this.RaiseAndSetIfChanged(ref _foregroundColor, value); }

        private string _hoverBackgroundColor = "#00000000";
        public string HoverBackgroundColor { get => _hoverBackgroundColor; set => this.RaiseAndSetIfChanged(ref _hoverBackgroundColor, value); }

        private string _hoverForegroundColor = "#00000000";
        public string HoverForegroundColor { get => _hoverForegroundColor; set => this.RaiseAndSetIfChanged(ref _hoverForegroundColor, value); }

        private string _clickForegroundColor = "#00000000";
        public string ClickForegroundColor { get => _clickForegroundColor; set => this.RaiseAndSetIfChanged(ref _clickForegroundColor, value); }

        private string _clickBackgroundColor = "#00000000";
        public string ClickBackgroundColor { get => _clickBackgroundColor; set => this.RaiseAndSetIfChanged(ref _clickBackgroundColor, value); }

        private string _clickBorderColorLeft = "#FF00FF00";
        public string ClickBorderColorLeft { get => _clickBorderColorLeft; set => this.RaiseAndSetIfChanged(ref _clickBorderColorLeft, value); }

        private string _clickBorderColorTop = "#FF00FF00";
        public string ClickBorderColorTop { get => _clickBorderColorTop; set => this.RaiseAndSetIfChanged(ref _clickBorderColorTop, value); }

        private string _clickBorderColorRight = "#FF00FF00";
        public string ClickBorderColorRight { get => _clickBorderColorRight; set => this.RaiseAndSetIfChanged(ref _clickBorderColorRight, value); }

        private string _clickBorderColorBottom = "#FF00FF00";
        public string ClickBorderColorBottom { get => _clickBorderColorBottom; set => this.RaiseAndSetIfChanged(ref _clickBorderColorBottom, value); }

        public void FromBytes(byte[] data)
        {
            var temp = IconButtonOprtCellCfgJson.FromBytes<BrushInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            BackgroundColor = temp.BackgroundColor ?? BackgroundColor;
            BorderColor = temp.BorderColor ?? BorderColor;
            ForegroundColor = temp.ForegroundColor ?? ForegroundColor;
            HoverBackgroundColor = temp.HoverBackgroundColor ?? HoverBackgroundColor;
            HoverForegroundColor = temp.HoverForegroundColor ?? HoverForegroundColor;
            ClickForegroundColor = temp.ClickForegroundColor ?? ClickForegroundColor;
            ClickBackgroundColor = temp.ClickBackgroundColor ?? ClickBackgroundColor;
            ClickBorderColorLeft = temp.ClickBorderColorLeft ?? ClickBorderColorLeft;
            ClickBorderColorTop = temp.ClickBorderColorTop ?? ClickBorderColorTop;
            ClickBorderColorRight = temp.ClickBorderColorRight ?? ClickBorderColorRight;
            ClickBorderColorBottom = temp.ClickBorderColorBottom ?? ClickBorderColorBottom;
        }

        public byte[] ToBytes() => IconButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class AppearanceInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _opacity = "0";
        public string Opacity { get => _opacity; set => this.RaiseAndSetIfChanged(ref _opacity, value); }

        private string _borderThicknessLeft = "0";
        public string BorderThicknessLeft { get => _borderThicknessLeft; set => this.RaiseAndSetIfChanged(ref _borderThicknessLeft, value); }

        private string _borderThicknessTop = "0";
        public string BorderThicknessTop { get => _borderThicknessTop; set => this.RaiseAndSetIfChanged(ref _borderThicknessTop, value); }

        private string _borderThicknessRight = "0";
        public string BorderThicknessRight { get => _borderThicknessRight; set => this.RaiseAndSetIfChanged(ref _borderThicknessRight, value); }

        private string _borderThicknessBottom = "0";
        public string BorderThicknessBottom { get => _borderThicknessBottom; set => this.RaiseAndSetIfChanged(ref _borderThicknessBottom, value); }

        private string _clickBorderThicknessLeft = "0";
        public string ClickBorderThicknessLeft { get => _clickBorderThicknessLeft; set => this.RaiseAndSetIfChanged(ref _clickBorderThicknessLeft, value); }

        private string _clickBorderThicknessTop = "0";
        public string ClickBorderThicknessTop { get => _clickBorderThicknessTop; set => this.RaiseAndSetIfChanged(ref _clickBorderThicknessTop, value); }

        private string _clickBorderThicknessRight = "0";
        public string ClickBorderThicknessRight { get => _clickBorderThicknessRight; set => this.RaiseAndSetIfChanged(ref _clickBorderThicknessRight, value); }

        private string _clickBorderThicknessBottom = "0";
        public string ClickBorderThicknessBottom { get => _clickBorderThicknessBottom; set => this.RaiseAndSetIfChanged(ref _clickBorderThicknessBottom, value); }

        public void FromBytes(byte[] data)
        {
            var temp = IconButtonOprtCellCfgJson.FromBytes<AppearanceInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            Opacity = temp.Opacity ?? Opacity;
            BorderThicknessLeft = temp.BorderThicknessLeft ?? BorderThicknessLeft;
            BorderThicknessTop = temp.BorderThicknessTop ?? BorderThicknessTop;
            BorderThicknessRight = temp.BorderThicknessRight ?? BorderThicknessRight;
            BorderThicknessBottom = temp.BorderThicknessBottom ?? BorderThicknessBottom;
            ClickBorderThicknessLeft = temp.ClickBorderThicknessLeft ?? ClickBorderThicknessLeft;
            ClickBorderThicknessTop = temp.ClickBorderThicknessTop ?? ClickBorderThicknessTop;
            ClickBorderThicknessRight = temp.ClickBorderThicknessRight ?? ClickBorderThicknessRight;
            ClickBorderThicknessBottom = temp.ClickBorderThicknessBottom ?? ClickBorderThicknessBottom;
        }

        public byte[] ToBytes() => IconButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class LayoutInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private IconButtonLayoutInfo.HorizontalAlignmentEnum _horizontalAlignment = IconButtonLayoutInfo.HorizontalAlignmentEnum.Stretch;
        public IconButtonLayoutInfo.HorizontalAlignmentEnum HorizontalAlignment { get => _horizontalAlignment; set => this.RaiseAndSetIfChanged(ref _horizontalAlignment, value); }

        private IconButtonLayoutInfo.VerticalAlignmentEnum _verticalAlignment = IconButtonLayoutInfo.VerticalAlignmentEnum.Stretch;
        public IconButtonLayoutInfo.VerticalAlignmentEnum VerticalAlignment { get => _verticalAlignment; set => this.RaiseAndSetIfChanged(ref _verticalAlignment, value); }

        private string _marginTop = "0";
        public string MarginTop { get => _marginTop; set => this.RaiseAndSetIfChanged(ref _marginTop, value); }

        private string _marginLeft = "0";
        public string MarginLeft { get => _marginLeft; set => this.RaiseAndSetIfChanged(ref _marginLeft, value); }

        private string _marginBottom = "0";
        public string MarginBottom { get => _marginBottom; set => this.RaiseAndSetIfChanged(ref _marginBottom, value); }

        private string _marginRight = "0";
        public string MarginRight { get => _marginRight; set => this.RaiseAndSetIfChanged(ref _marginRight, value); }

        public void FromBytes(byte[] data)
        {
            var temp = IconButtonOprtCellCfgJson.FromBytes<LayoutInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            HorizontalAlignment = temp.HorizontalAlignment;
            VerticalAlignment = temp.VerticalAlignment;
            MarginTop = temp.MarginTop ?? MarginTop;
            MarginLeft = temp.MarginLeft ?? MarginLeft;
            MarginBottom = temp.MarginBottom ?? MarginBottom;
            MarginRight = temp.MarginRight ?? MarginRight;
        }

        public byte[] ToBytes() => IconButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _buttonText = "";
        public string ButtonText { get => _buttonText; set => this.RaiseAndSetIfChanged(ref _buttonText, value); }

        private CommonCursorType _hoverCursor = CommonCursorType.手型;
        public CommonCursorType HoverCursor { get => _hoverCursor; set => this.RaiseAndSetIfChanged(ref _hoverCursor, value); }

        private bool _enabled = true;
        public bool Enabled { get => _enabled; set => this.RaiseAndSetIfChanged(ref _enabled, value); }

        private string _tooltipText = "";
        public string TooltipText { get => _tooltipText; set => this.RaiseAndSetIfChanged(ref _tooltipText, value); }

        private string _groupId = "";
        public string GroupId { get => _groupId; set => this.RaiseAndSetIfChanged(ref _groupId, value); }

        public void FromBytes(byte[] data)
        {
            var temp = IconButtonOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            ButtonText = temp.ButtonText ?? ButtonText;
            HoverCursor = temp.HoverCursor;
            Enabled = temp.Enabled;
            TooltipText = temp.TooltipText ?? TooltipText;
            GroupId = temp.GroupId ?? GroupId;
        }

        public byte[] ToBytes() => IconButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class FontInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _fontColor = "#FF000000";
        public string FontColor { get => _fontColor; set => this.RaiseAndSetIfChanged(ref _fontColor, value); }

        private string _fontSize = "16";
        public string FontSize { get => _fontSize; set => this.RaiseAndSetIfChanged(ref _fontSize, value); }

        private bool _isBold = false;
        public bool IsBold { get => _isBold; set => this.RaiseAndSetIfChanged(ref _isBold, value); }

        private bool _isItalic = false;
        public bool IsItalic { get => _isItalic; set => this.RaiseAndSetIfChanged(ref _isItalic, value); }

        private bool _isUnderline = false;
        public bool IsUnderline { get => _isUnderline; set => this.RaiseAndSetIfChanged(ref _isUnderline, value); }

        public void FromBytes(byte[] data)
        {
            var temp = IconButtonOprtCellCfgJson.FromBytes<FontInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            FontColor = temp.FontColor ?? FontColor;
            FontSize = temp.FontSize ?? FontSize;
            IsBold = temp.IsBold;
            IsItalic = temp.IsItalic;
            IsUnderline = temp.IsUnderline;
        }

        public byte[] ToBytes() => IconButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class ParagraphInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _lineHeight = "23";
        public string LineHeight { get => _lineHeight; set => this.RaiseAndSetIfChanged(ref _lineHeight, value); }

        private string _paragraphSpacingBefore = "0";
        public string ParagraphSpacingBefore { get => _paragraphSpacingBefore; set => this.RaiseAndSetIfChanged(ref _paragraphSpacingBefore, value); }

        private string _paragraphSpacingAfter = "0";
        public string ParagraphSpacingAfter { get => _paragraphSpacingAfter; set => this.RaiseAndSetIfChanged(ref _paragraphSpacingAfter, value); }

        private IconButtonParagraphInfo.TextAlignmentEnum _textAlignment = IconButtonParagraphInfo.TextAlignmentEnum.Center;
        public IconButtonParagraphInfo.TextAlignmentEnum TextAlignment { get => _textAlignment; set => this.RaiseAndSetIfChanged(ref _textAlignment, value); }

        public void FromBytes(byte[] data)
        {
            var temp = IconButtonOprtCellCfgJson.FromBytes<ParagraphInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            LineHeight = temp.LineHeight ?? LineHeight;
            ParagraphSpacingBefore = temp.ParagraphSpacingBefore ?? ParagraphSpacingBefore;
            ParagraphSpacingAfter = temp.ParagraphSpacingAfter ?? ParagraphSpacingAfter;
            TextAlignment = temp.TextAlignment;
        }

        public byte[] ToBytes() => IconButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class MiscInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _iconWidth = "24";
        public string IconWidth { get => _iconWidth; set => this.RaiseAndSetIfChanged(ref _iconWidth, value); }

        private string _iconHeight = "24";
        public string IconHeight { get => _iconHeight; set => this.RaiseAndSetIfChanged(ref _iconHeight, value); }

        private string _iconMarginTop = "0";
        public string IconMarginTop { get => _iconMarginTop; set => this.RaiseAndSetIfChanged(ref _iconMarginTop, value); }

        private string _iconMarginLeft = "0";
        public string IconMarginLeft { get => _iconMarginLeft; set => this.RaiseAndSetIfChanged(ref _iconMarginLeft, value); }

        private string _iconMarginBottom = "0";
        public string IconMarginBottom { get => _iconMarginBottom; set => this.RaiseAndSetIfChanged(ref _iconMarginBottom, value); }

        private string _iconMarginRight = "0";
        public string IconMarginRight { get => _iconMarginRight; set => this.RaiseAndSetIfChanged(ref _iconMarginRight, value); }

        private IconButtonMiscInfo.IconPositionEnum _iconPosition = IconButtonMiscInfo.IconPositionEnum.Left;
        public IconButtonMiscInfo.IconPositionEnum IconPosition { get => _iconPosition; set => this.RaiseAndSetIfChanged(ref _iconPosition, value); }

        public void FromBytes(byte[] data)
        {
            var temp = IconButtonOprtCellCfgJson.FromBytes<MiscInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            IconWidth = temp.IconWidth ?? IconWidth;
            IconHeight = temp.IconHeight ?? IconHeight;
            IconMarginTop = temp.IconMarginTop ?? IconMarginTop;
            IconMarginLeft = temp.IconMarginLeft ?? IconMarginLeft;
            IconMarginBottom = temp.IconMarginBottom ?? IconMarginBottom;
            IconMarginRight = temp.IconMarginRight ?? IconMarginRight;
            IconPosition = temp.IconPosition;
        }

        public byte[] ToBytes() => IconButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class BrushInfoMapOprtCellParamView : IconButtonOprtCellParamViewBase
    {
        public BrushInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("背景色(#AARRGGBB)", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BackgroundColor))));
            root.Children.Add(CreateRow("边框色(#AARRGGBB)", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BorderColor))));
            root.Children.Add(CreateRow("前景色(#AARRGGBB)", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ForegroundColor))));
            root.Children.Add(CreateRow("悬停背景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.HoverBackgroundColor))));
            root.Children.Add(CreateRow("悬停前景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.HoverForegroundColor))));
            root.Children.Add(CreateRow("点击背景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ClickBackgroundColor))));
            root.Children.Add(CreateRow("点击前景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ClickForegroundColor))));
            root.Children.Add(CreateRow("点击左边框颜色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ClickBorderColorLeft))));
            root.Children.Add(CreateRow("点击上边框颜色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ClickBorderColorTop))));
            root.Children.Add(CreateRow("点击右边框颜色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ClickBorderColorRight))));
            root.Children.Add(CreateRow("点击下边框颜色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ClickBorderColorBottom))));
            Content = root;
        }
    }

    internal sealed class AppearanceInfoMapOprtCellParamView : IconButtonOprtCellParamViewBase
    {
        public AppearanceInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("透明度(0-100)", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.Opacity), 80)));
            root.Children.Add(CreateRow("左边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessLeft), 80)));
            root.Children.Add(CreateRow("上边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessTop), 80)));
            root.Children.Add(CreateRow("右边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessRight), 80)));
            root.Children.Add(CreateRow("下边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessBottom), 80)));
            root.Children.Add(CreateRow("点击左边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.ClickBorderThicknessLeft), 80)));
            root.Children.Add(CreateRow("点击上边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.ClickBorderThicknessTop), 80)));
            root.Children.Add(CreateRow("点击右边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.ClickBorderThicknessRight), 80)));
            root.Children.Add(CreateRow("点击下边框", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.ClickBorderThicknessBottom), 80)));
            Content = root;
        }
    }

    internal sealed class LayoutInfoMapOprtCellParamView : IconButtonOprtCellParamViewBase
    {
        public LayoutInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("水平对齐", CreateEnumComboBox<IconButtonLayoutInfo.HorizontalAlignmentEnum>(nameof(LayoutInfoMapOprtCellParamViewModel.HorizontalAlignment), 220)));
            root.Children.Add(CreateRow("垂直对齐", CreateEnumComboBox<IconButtonLayoutInfo.VerticalAlignmentEnum>(nameof(LayoutInfoMapOprtCellParamViewModel.VerticalAlignment), 220)));
            root.Children.Add(CreateRow("上边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginTop), 80)));
            root.Children.Add(CreateRow("左边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginLeft), 80)));
            root.Children.Add(CreateRow("下边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginBottom), 80)));
            root.Children.Add(CreateRow("右边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginRight), 80)));
            Content = root;
        }
    }

    internal sealed class CommonInfoMapOprtCellParamView : IconButtonOprtCellParamViewBase
    {
        public CommonInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("按钮文本", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.ButtonText), 220)));
            root.Children.Add(CreateRow("悬停光标", CreateEnumComboBox<CommonCursorType>(nameof(CommonInfoMapOprtCellParamViewModel.HoverCursor), 220)));
            root.Children.Add(CreateCheckBox("启用", nameof(CommonInfoMapOprtCellParamViewModel.Enabled)));
            root.Children.Add(CreateRow("提示文本", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.TooltipText), 220)));
            root.Children.Add(CreateRow("组ID", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.GroupId), 220)));
            Content = root;
        }
    }

    internal sealed class FontInfoMapOprtCellParamView : IconButtonOprtCellParamViewBase
    {
        public FontInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("字体颜色(#AARRGGBB)", CreateTextBox(nameof(FontInfoMapOprtCellParamViewModel.FontColor), 180)));
            root.Children.Add(CreateRow("字体大小", CreateTextBox(nameof(FontInfoMapOprtCellParamViewModel.FontSize), 80)));
            root.Children.Add(CreateCheckBox("加粗", nameof(FontInfoMapOprtCellParamViewModel.IsBold)));
            root.Children.Add(CreateCheckBox("斜体", nameof(FontInfoMapOprtCellParamViewModel.IsItalic)));
            root.Children.Add(CreateCheckBox("下划线", nameof(FontInfoMapOprtCellParamViewModel.IsUnderline)));
            Content = root;
        }
    }

    internal sealed class ParagraphInfoMapOprtCellParamView : IconButtonOprtCellParamViewBase
    {
        public ParagraphInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("行高", CreateTextBox(nameof(ParagraphInfoMapOprtCellParamViewModel.LineHeight), 80)));
            root.Children.Add(CreateRow("段前间距", CreateTextBox(nameof(ParagraphInfoMapOprtCellParamViewModel.ParagraphSpacingBefore), 80)));
            root.Children.Add(CreateRow("段后间距", CreateTextBox(nameof(ParagraphInfoMapOprtCellParamViewModel.ParagraphSpacingAfter), 80)));
            root.Children.Add(CreateRow("文本对齐", CreateEnumComboBox<IconButtonParagraphInfo.TextAlignmentEnum>(nameof(ParagraphInfoMapOprtCellParamViewModel.TextAlignment), 220)));
            Content = root;
        }
    }

    internal sealed class MiscInfoMapOprtCellParamView : IconButtonOprtCellParamViewBase
    {
        public MiscInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("Icon宽度", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.IconWidth), 80)));
            root.Children.Add(CreateRow("Icon高度", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.IconHeight), 80)));
            root.Children.Add(CreateRow("Icon位置", CreateEnumComboBox<IconButtonMiscInfo.IconPositionEnum>(nameof(MiscInfoMapOprtCellParamViewModel.IconPosition), 220)));
            root.Children.Add(CreateRow("Icon上外边距", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.IconMarginTop), 80)));
            root.Children.Add(CreateRow("Icon左外边距", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.IconMarginLeft), 80)));
            root.Children.Add(CreateRow("Icon下外边距", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.IconMarginBottom), 80)));
            root.Children.Add(CreateRow("Icon右外边距", CreateTextBox(nameof(MiscInfoMapOprtCellParamViewModel.IconMarginRight), 80)));
            Content = root;
        }
    }

    internal sealed class BrushInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly BrushInfoMapOprtCellParamView _view;
        private readonly BrushInfoMapOprtCellParamViewModel _vm;

        public BrushInfoMapOprtCellParamCfgView()
        {
            _vm = new BrushInfoMapOprtCellParamViewModel();
            _view = new BrushInfoMapOprtCellParamView { DataContext = _vm };
        }

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

        public AppearanceInfoMapOprtCellParamCfgView()
        {
            _vm = new AppearanceInfoMapOprtCellParamViewModel();
            _view = new AppearanceInfoMapOprtCellParamView { DataContext = _vm };
        }

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

        public LayoutInfoMapOprtCellParamCfgView()
        {
            _vm = new LayoutInfoMapOprtCellParamViewModel();
            _view = new LayoutInfoMapOprtCellParamView { DataContext = _vm };
        }

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

        public CommonInfoMapOprtCellParamCfgView()
        {
            _vm = new CommonInfoMapOprtCellParamViewModel();
            _view = new CommonInfoMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    internal sealed class FontInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly FontInfoMapOprtCellParamView _view;
        private readonly FontInfoMapOprtCellParamViewModel _vm;

        public FontInfoMapOprtCellParamCfgView()
        {
            _vm = new FontInfoMapOprtCellParamViewModel();
            _view = new FontInfoMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    internal sealed class ParagraphInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly ParagraphInfoMapOprtCellParamView _view;
        private readonly ParagraphInfoMapOprtCellParamViewModel _vm;

        public ParagraphInfoMapOprtCellParamCfgView()
        {
            _vm = new ParagraphInfoMapOprtCellParamViewModel();
            _view = new ParagraphInfoMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    internal sealed class MiscInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly MiscInfoMapOprtCellParamView _view;
        private readonly MiscInfoMapOprtCellParamViewModel _vm;

        public MiscInfoMapOprtCellParamCfgView()
        {
            _vm = new MiscInfoMapOprtCellParamViewModel();
            _view = new MiscInfoMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }
}


