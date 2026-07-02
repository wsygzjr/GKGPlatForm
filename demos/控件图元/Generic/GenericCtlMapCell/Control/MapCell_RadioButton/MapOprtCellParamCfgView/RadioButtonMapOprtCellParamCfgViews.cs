using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using GKG.Map.MapCell.Generic.GroupPanel;
using GKG.Map.MapCell.Generic.Control.Lable;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;

using AControls = Avalonia.Controls;
using APrimitives = Avalonia.Controls.Primitives;

namespace GKG.Map.MapCell.Generic.RadioButton.MapOprtCellParamCfgView
{
    internal static class RadioButtonOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));
        internal static T? FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    #region 颜色选择器控件

    internal sealed class ColorPickerEditor : AControls.UserControl
    {
        private readonly AControls.TextBox _textBox;
        private readonly Border _colorPreview;
        private readonly CompositeDisposable _disposables = new();

        public static readonly StyledProperty<string> ColorStringProperty =
            AvaloniaProperty.Register<ColorPickerEditor, string>(nameof(ColorString), "#FF000000");

        public string ColorString
        {
            get => GetValue(ColorStringProperty);
            set => SetValue(ColorStringProperty, value);
        }

        public ColorPickerEditor()
        {
            var panel = new AControls.StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };

            _colorPreview = new Border
            {
                Width = 24, Height = 24, CornerRadius = new CornerRadius(4),
                BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1),
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
                Background = new SolidColorBrush(Colors.Black),
            };
            _colorPreview.PointerPressed += OnColorPreviewClick;

            _textBox = new AControls.TextBox { Width = 100, VerticalAlignment = VerticalAlignment.Center };
            _textBox.Bind(AControls.TextBox.TextProperty, new Binding(nameof(ColorString)) { Source = this, Mode = BindingMode.TwoWay });

            panel.Children.Add(_colorPreview);
            panel.Children.Add(_textBox);
            Content = panel;

            this.GetObservable(ColorStringProperty).Subscribe(UpdateColorPreview).DisposeWith(_disposables);
        }

        private void UpdateColorPreview(string colorStr)
        {
            try { if (Color.TryParse(colorStr, out var color)) _colorPreview.Background = new SolidColorBrush(color); } catch { }
        }

        private async void OnColorPreviewClick(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            try
            {
                var currentColor = Colors.Black;
                if (Color.TryParse(ColorString, out var parsed)) currentColor = parsed;

                var colorPicker = new AControls.ColorPicker { Color = currentColor, Width = 300, Height = 400 };
                var dialog = new Window
                {
                    Title = "选择颜色", Width = 340, Height = 480, WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new AControls.StackPanel
                    {
                        Margin = new Thickness(10), Spacing = 10,
                        Children = { colorPicker, new AControls.StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 10, Children = { new AControls.Button { Content = "确定", Tag = "OK", Width = 80 }, new AControls.Button { Content = "取消", Tag = "Cancel", Width = 80 } } } }
                    }
                };

                Color? selectedColor = null;
                var buttons = ((AControls.StackPanel)((AControls.StackPanel)dialog.Content).Children[1]).Children;
                ((AControls.Button)buttons[0]).Click += (s, args) => { selectedColor = colorPicker.Color; dialog.Close(); };
                ((AControls.Button)buttons[1]).Click += (s, args) => dialog.Close();

                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel is Window parentWindow) await dialog.ShowDialog(parentWindow);
                else { dialog.Show(); return; }

                if (selectedColor.HasValue)
                    ColorString = $"#{selectedColor.Value.A:X2}{selectedColor.Value.R:X2}{selectedColor.Value.G:X2}{selectedColor.Value.B:X2}";
            }
            catch { }
        }

        protected override void OnUnloaded(Avalonia.Interactivity.RoutedEventArgs e) { base.OnUnloaded(e); _disposables.Dispose(); }
    }

    #endregion

    #region ViewModels

    internal sealed class BrushInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _backColor = "#00FFFFFF";
        public string BackColor { get => _backColor; set => this.RaiseAndSetIfChanged(ref _backColor, value); }

        private string _borderColor = "#FF646464";
        public string BorderColor { get => _borderColor; set => this.RaiseAndSetIfChanged(ref _borderColor, value); }

        private string _foreColor = "#FF000000";
        public string ForeColor { get => _foreColor; set => this.RaiseAndSetIfChanged(ref _foreColor, value); }

        public void FromBytes(byte[] data)
        {
            var temp = RadioButtonOprtCellCfgJson.FromBytes<BrushInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            BackColor = temp.BackColor ?? BackColor;
            BorderColor = temp.BorderColor ?? BorderColor;
            ForeColor = temp.ForeColor ?? ForeColor;
        }
        public byte[] ToBytes() => RadioButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class AppearanceInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _opacity = "1";
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
            var temp = RadioButtonOprtCellCfgJson.FromBytes<AppearanceInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            Opacity = temp.Opacity ?? Opacity;
            BorderThicknessLeft = temp.BorderThicknessLeft ?? BorderThicknessLeft;
            BorderThicknessTop = temp.BorderThicknessTop ?? BorderThicknessTop;
            BorderThicknessRight = temp.BorderThicknessRight ?? BorderThicknessRight;
            BorderThicknessBottom = temp.BorderThicknessBottom ?? BorderThicknessBottom;
        }
        public byte[] ToBytes() => RadioButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class LayoutInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private HorizontalAlignType _horizontalAlign = HorizontalAlignType.Left;
        public HorizontalAlignType HorizontalAlign { get => _horizontalAlign; set => this.RaiseAndSetIfChanged(ref _horizontalAlign, value); }

        private VerticalAlignType _verticalAlign = VerticalAlignType.Center;
        public VerticalAlignType VerticalAlign { get => _verticalAlign; set => this.RaiseAndSetIfChanged(ref _verticalAlign, value); }

        private string _marginTop = "0";
        public string MarginTop { get => _marginTop; set => this.RaiseAndSetIfChanged(ref _marginTop, value); }

        private string _marginLeft = "0";
        public string MarginLeft { get => _marginLeft; set => this.RaiseAndSetIfChanged(ref _marginLeft, value); }

        private string _marginBottom = "0";
        public string MarginBottom { get => _marginBottom; set => this.RaiseAndSetIfChanged(ref _marginBottom, value); }

        private string _marginRight = "0";
        public string MarginRight { get => _marginRight; set => this.RaiseAndSetIfChanged(ref _marginRight, value); }

        private string _minWidth = "0";
        public string MinWidth { get => _minWidth; set => this.RaiseAndSetIfChanged(ref _minWidth, value); }

        private string _maxWidth = "0";
        public string MaxWidth { get => _maxWidth; set => this.RaiseAndSetIfChanged(ref _maxWidth, value); }

        private string _minHeight = "0";
        public string MinHeight { get => _minHeight; set => this.RaiseAndSetIfChanged(ref _minHeight, value); }

        private string _maxHeight = "0";
        public string MaxHeight { get => _maxHeight; set => this.RaiseAndSetIfChanged(ref _maxHeight, value); }

        public void FromBytes(byte[] data)
        {
            var temp = RadioButtonOprtCellCfgJson.FromBytes<LayoutInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            HorizontalAlign = temp.HorizontalAlign;
            VerticalAlign = temp.VerticalAlign;
            MarginTop = temp.MarginTop ?? MarginTop;
            MarginLeft = temp.MarginLeft ?? MarginLeft;
            MarginBottom = temp.MarginBottom ?? MarginBottom;
            MarginRight = temp.MarginRight ?? MarginRight;
            MinWidth = temp.MinWidth ?? MinWidth;
            MaxWidth = temp.MaxWidth ?? MaxWidth;
            MinHeight = temp.MinHeight ?? MinHeight;
            MaxHeight = temp.MaxHeight ?? MaxHeight;
        }
        public byte[] ToBytes() => RadioButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _text = "单选框";
        public string Text { get => _text; set => this.RaiseAndSetIfChanged(ref _text, value); }

        private string _groupName = "";
        public string GroupName { get => _groupName; set => this.RaiseAndSetIfChanged(ref _groupName, value); }

        private bool? _isChecked = false;
        public bool? IsChecked { get => _isChecked; set => this.RaiseAndSetIfChanged(ref _isChecked, value); }

        private bool _isThreeState = false;
        public bool IsThreeState { get => _isThreeState; set => this.RaiseAndSetIfChanged(ref _isThreeState, value); }

        private CursorType _cursorType = CursorType.Arrow;
        public CursorType CursorType { get => _cursorType; set => this.RaiseAndSetIfChanged(ref _cursorType, value); }

        private bool _isEnabled = true;
        public bool IsEnabled { get => _isEnabled; set => this.RaiseAndSetIfChanged(ref _isEnabled, value); }

        private string _toolTip = "";
        public string ToolTip { get => _toolTip; set => this.RaiseAndSetIfChanged(ref _toolTip, value); }

        public void FromBytes(byte[] data)
        {
            var temp = RadioButtonOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            Text = temp.Text ?? Text;
            GroupName = temp.GroupName ?? GroupName;
            IsChecked = temp.IsChecked;
            IsThreeState = temp.IsThreeState;
            CursorType = temp.CursorType;
            IsEnabled = temp.IsEnabled;
            ToolTip = temp.ToolTip ?? ToolTip;
        }
        public byte[] ToBytes() => RadioButtonOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class TextInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private FontFamilyType _fontFamilyType = FontFamilyType.MicrosoftYaHei;
        public FontFamilyType FontFamilyType { get => _fontFamilyType; set => this.RaiseAndSetIfChanged(ref _fontFamilyType, value); }

        private string _fontColor = "#FF000000";
        public string FontColor { get => _fontColor; set => this.RaiseAndSetIfChanged(ref _fontColor, value); }

        private string _fontSize = "14";
        public string FontSize { get => _fontSize; set => this.RaiseAndSetIfChanged(ref _fontSize, value); }

        private bool _isItalic = false;
        public bool IsItalic { get => _isItalic; set => this.RaiseAndSetIfChanged(ref _isItalic, value); }

        private bool _isBold = false;
        public bool IsBold { get => _isBold; set => this.RaiseAndSetIfChanged(ref _isBold, value); }

        public void FromBytes(byte[] data)
        {
            var temp = RadioButtonOprtCellCfgJson.FromBytes<TextInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            FontFamilyType = temp.FontFamilyType;
            FontColor = temp.FontColor ?? FontColor;
            FontSize = temp.FontSize ?? FontSize;
            IsItalic = temp.IsItalic;
            IsBold = temp.IsBold;
        }
        public byte[] ToBytes() => RadioButtonOprtCellCfgJson.ToBytes(this);
    }

    #endregion

    #region Views

    internal abstract class RadioButtonOprtCellParamViewBase : AControls.UserControl
    {
        protected static AControls.StackPanel CreateRoot() => new AControls.StackPanel { Margin = new Thickness(20), Spacing = 12 };

        protected static AControls.StackPanel CreateRow(string label, AControls.Control editor)
        {
            var row = new AControls.StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
            row.Children.Add(new AControls.TextBlock { Text = label, Width = 140, VerticalAlignment = VerticalAlignment.Center });
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

        protected static AControls.ComboBox CreateEnumComboBox<TEnum>(string bindingPath, double width = 160) where TEnum : struct, Enum
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

    internal sealed class BrushInfoMapOprtCellParamView : RadioButtonOprtCellParamViewBase
    {
        public BrushInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("背景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BackColor))));
            root.Children.Add(CreateRow("边框色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BorderColor))));
            root.Children.Add(CreateRow("前景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ForeColor))));
            Content = root;
        }
    }

    internal sealed class AppearanceInfoMapOprtCellParamView : RadioButtonOprtCellParamViewBase
    {
        public AppearanceInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("透明度(0~1)", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.Opacity), 80)));
            root.Children.Add(CreateRow("左边框线宽", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessLeft), 80)));
            root.Children.Add(CreateRow("上边框线宽", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessTop), 80)));
            root.Children.Add(CreateRow("右边框线宽", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessRight), 80)));
            root.Children.Add(CreateRow("下边框线宽", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.BorderThicknessBottom), 80)));
            Content = root;
        }
    }

    internal sealed class LayoutInfoMapOprtCellParamView : RadioButtonOprtCellParamViewBase
    {
        public LayoutInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("水平对齐", CreateEnumComboBox<HorizontalAlignType>(nameof(LayoutInfoMapOprtCellParamViewModel.HorizontalAlign))));
            root.Children.Add(CreateRow("垂直对齐", CreateEnumComboBox<VerticalAlignType>(nameof(LayoutInfoMapOprtCellParamViewModel.VerticalAlign))));
            root.Children.Add(CreateRow("上边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginTop), 80)));
            root.Children.Add(CreateRow("左边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginLeft), 80)));
            root.Children.Add(CreateRow("下边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginBottom), 80)));
            root.Children.Add(CreateRow("右边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginRight), 80)));
            root.Children.Add(CreateRow("最小宽度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MinWidth), 80)));
            root.Children.Add(CreateRow("最大宽度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MaxWidth), 80)));
            root.Children.Add(CreateRow("最小高度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MinHeight), 80)));
            root.Children.Add(CreateRow("最大高度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MaxHeight), 80)));
            Content = root;
        }
    }

    internal sealed class CommonInfoMapOprtCellParamView : RadioButtonOprtCellParamViewBase
    {
        public CommonInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("文本", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.Text), 200)));
            root.Children.Add(CreateRow("组名", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.GroupName), 200)));
            root.Children.Add(CreateCheckBox("选中", nameof(CommonInfoMapOprtCellParamViewModel.IsChecked)));
            root.Children.Add(CreateCheckBox("支持三态", nameof(CommonInfoMapOprtCellParamViewModel.IsThreeState)));
            root.Children.Add(CreateRow("鼠标样式", CreateEnumComboBox<CursorType>(nameof(CommonInfoMapOprtCellParamViewModel.CursorType), 200)));
            root.Children.Add(CreateCheckBox("启用", nameof(CommonInfoMapOprtCellParamViewModel.IsEnabled)));
            root.Children.Add(CreateRow("提示文字", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.ToolTip), 200)));
            Content = root;
        }
    }

    internal sealed class TextInfoMapOprtCellParamView : RadioButtonOprtCellParamViewBase
    {
        public TextInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("字体", CreateEnumComboBox<FontFamilyType>(nameof(TextInfoMapOprtCellParamViewModel.FontFamilyType), 200)));
            root.Children.Add(CreateRow("字体颜色", CreateColorPicker(nameof(TextInfoMapOprtCellParamViewModel.FontColor))));
            root.Children.Add(CreateRow("字体大小", CreateTextBox(nameof(TextInfoMapOprtCellParamViewModel.FontSize), 80)));
            root.Children.Add(CreateCheckBox("斜体", nameof(TextInfoMapOprtCellParamViewModel.IsItalic)));
            root.Children.Add(CreateCheckBox("加粗", nameof(TextInfoMapOprtCellParamViewModel.IsBold)));
            Content = root;
        }
    }

    #endregion

    #region CfgView wrappers

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

    internal sealed class TextInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly TextInfoMapOprtCellParamView _view;
        private readonly TextInfoMapOprtCellParamViewModel _vm;

        public TextInfoMapOprtCellParamCfgView()
        {
            _vm = new TextInfoMapOprtCellParamViewModel();
            _view = new TextInfoMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    #endregion
}
