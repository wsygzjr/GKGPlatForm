using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
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

namespace GKG.Map.MapCell.Generic.PasswordBox.MapOprtCellParamCfgView
{
    internal static class PasswordBoxOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));
        internal static T? FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    #region 颜色选择器控件 (复用逻辑，但为了独立性重新定义)

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
        private string _backColor = "#FFFFFFFF";
        public string BackColorStr { get => _backColor; set => this.RaiseAndSetIfChanged(ref _backColor, value); }

        private string _borderColor = "#FF808080";
        public string BorderColorStr { get => _borderColor; set => this.RaiseAndSetIfChanged(ref _borderColor, value); }

        private string _foreColor = "#FF000000";
        public string ForeColorStr { get => _foreColor; set => this.RaiseAndSetIfChanged(ref _foreColor, value); }

        private string _focusBorderColor = "#FF0000FF";
        public string FocusBorderColorStr { get => _focusBorderColor; set => this.RaiseAndSetIfChanged(ref _focusBorderColor, value); }

        public void FromBytes(byte[] data)
        {
            var temp = PasswordBoxOprtCellCfgJson.FromBytes<BrushInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            BackColorStr = temp.BackColorStr ?? BackColorStr;
            BorderColorStr = temp.BorderColorStr ?? BorderColorStr;
            ForeColorStr = temp.ForeColorStr ?? ForeColorStr;
            FocusBorderColorStr = temp.FocusBorderColorStr ?? FocusBorderColorStr;
        }
        public byte[] ToBytes() => PasswordBoxOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class AppearanceInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private double _opacity = 1.0;
        public double Opacity { get => _opacity; set => this.RaiseAndSetIfChanged(ref _opacity, value); }

        private double _borderThicknessLeft = 1;
        public double BorderThicknessLeft { get => _borderThicknessLeft; set => this.RaiseAndSetIfChanged(ref _borderThicknessLeft, value); }

        private double _borderThicknessTop = 1;
        public double BorderThicknessTop { get => _borderThicknessTop; set => this.RaiseAndSetIfChanged(ref _borderThicknessTop, value); }

        private double _borderThicknessRight = 1;
        public double BorderThicknessRight { get => _borderThicknessRight; set => this.RaiseAndSetIfChanged(ref _borderThicknessRight, value); }

        private double _borderThicknessBottom = 1;
        public double BorderThicknessBottom { get => _borderThicknessBottom; set => this.RaiseAndSetIfChanged(ref _borderThicknessBottom, value); }

        public void FromBytes(byte[] data)
        {
            var temp = PasswordBoxOprtCellCfgJson.FromBytes<AppearanceInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            Opacity = temp.Opacity;
            BorderThicknessLeft = temp.BorderThicknessLeft;
            BorderThicknessTop = temp.BorderThicknessTop;
            BorderThicknessRight = temp.BorderThicknessRight;
            BorderThicknessBottom = temp.BorderThicknessBottom;
        }
        public byte[] ToBytes() => PasswordBoxOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class LayoutInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private double _width = 120;
        public double Width { get => _width; set => this.RaiseAndSetIfChanged(ref _width, value); }

        private double _height = 50;
        public double Height { get => _height; set => this.RaiseAndSetIfChanged(ref _height, value); }

        private HorizontalAlignType _horizontalAlignment = HorizontalAlignType.Stretch;
        public HorizontalAlignType HorizontalAlignment { get => _horizontalAlignment; set => this.RaiseAndSetIfChanged(ref _horizontalAlignment, value); }

        private VerticalAlignType _verticalAlignment = VerticalAlignType.Center;
        public VerticalAlignType VerticalAlignment { get => _verticalAlignment; set => this.RaiseAndSetIfChanged(ref _verticalAlignment, value); }

        private double _marginTop = 0;
        public double MarginTop { get => _marginTop; set => this.RaiseAndSetIfChanged(ref _marginTop, value); }

        private double _marginLeft = 0;
        public double MarginLeft { get => _marginLeft; set => this.RaiseAndSetIfChanged(ref _marginLeft, value); }

        private double _marginBottom = 0;
        public double MarginBottom { get => _marginBottom; set => this.RaiseAndSetIfChanged(ref _marginBottom, value); }

        private double _marginRight = 0;
        public double MarginRight { get => _marginRight; set => this.RaiseAndSetIfChanged(ref _marginRight, value); }

        private double _minWidth = 0;
        public double MinWidth { get => _minWidth; set => this.RaiseAndSetIfChanged(ref _minWidth, value); }

        private double _maxWidth = 10000;
        public double MaxWidth { get => _maxWidth; set => this.RaiseAndSetIfChanged(ref _maxWidth, value); }

        private double _minHeight = 0;
        public double MinHeight { get => _minHeight; set => this.RaiseAndSetIfChanged(ref _minHeight, value); }

        private double _maxHeight = 10000;
        public double MaxHeight { get => _maxHeight; set => this.RaiseAndSetIfChanged(ref _maxHeight, value); }

        public void FromBytes(byte[] data)
        {
            var temp = PasswordBoxOprtCellCfgJson.FromBytes<LayoutInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            Width = temp.Width;
            Height = temp.Height;
            HorizontalAlignment = temp.HorizontalAlignment;
            VerticalAlignment = temp.VerticalAlignment;
            MarginTop = temp.MarginTop;
            MarginLeft = temp.MarginLeft;
            MarginBottom = temp.MarginBottom;
            MarginRight = temp.MarginRight;
            MinWidth = temp.MinWidth;
            MaxWidth = temp.MaxWidth;
            MinHeight = temp.MinHeight;
            MaxHeight = temp.MaxHeight;
        }
        public byte[] ToBytes() => PasswordBoxOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _passwordValue = "";
        public string PasswordValue { get => _passwordValue; set => this.RaiseAndSetIfChanged(ref _passwordValue, value); }

        private CursorType _cursorType = CursorType.Ibeam;
        public CursorType CursorType { get => _cursorType; set => this.RaiseAndSetIfChanged(ref _cursorType, value); }

        private bool _enabled = true;
        public bool Enabled { get => _enabled; set => this.RaiseAndSetIfChanged(ref _enabled, value); }

        private string _placeholderText = "";
        public string PlaceholderText { get => _placeholderText; set => this.RaiseAndSetIfChanged(ref _placeholderText, value); }

        private bool _passwordVisible = false;
        public bool PasswordVisible { get => _passwordVisible; set => this.RaiseAndSetIfChanged(ref _passwordVisible, value); }

        public void FromBytes(byte[] data)
        {
            var temp = PasswordBoxOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            PasswordValue = temp.PasswordValue ?? PasswordValue;
            CursorType = temp.CursorType;
            Enabled = temp.Enabled;
            PlaceholderText = temp.PlaceholderText ?? PlaceholderText;
            PasswordVisible = temp.PasswordVisible;
        }
        public byte[] ToBytes() => PasswordBoxOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class TextInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private FontFamilyType _fontFamily = FontFamilyType.MicrosoftYaHei;
        public FontFamilyType FontFamily { get => _fontFamily; set => this.RaiseAndSetIfChanged(ref _fontFamily, value); }

        private double _fontSize = 14;
        public double FontSize { get => _fontSize; set => this.RaiseAndSetIfChanged(ref _fontSize, value); }

        private bool _isItalic = false;
        public bool IsItalic { get => _isItalic; set => this.RaiseAndSetIfChanged(ref _isItalic, value); }

        private bool _isBold = false;
        public bool IsBold { get => _isBold; set => this.RaiseAndSetIfChanged(ref _isBold, value); }

        public void FromBytes(byte[] data)
        {
            var temp = PasswordBoxOprtCellCfgJson.FromBytes<TextInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            FontFamily = temp.FontFamily;
            FontSize = temp.FontSize;
            IsItalic = temp.IsItalic;
            IsBold = temp.IsBold;
        }
        public byte[] ToBytes() => PasswordBoxOprtCellCfgJson.ToBytes(this);
    }

    #endregion

    #region Views

    internal abstract class PasswordBoxOprtCellParamViewBase : AControls.UserControl
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

    internal sealed class BrushInfoMapOprtCellParamView : PasswordBoxOprtCellParamViewBase
    {
        public BrushInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("背景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BackColorStr))));
            root.Children.Add(CreateRow("边框色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BorderColorStr))));
            root.Children.Add(CreateRow("前景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.ForeColorStr))));
            root.Children.Add(CreateRow("聚焦边框色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.FocusBorderColorStr))));
            Content = root;
        }
    }

    internal sealed class AppearanceInfoMapOprtCellParamView : PasswordBoxOprtCellParamViewBase
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

    internal sealed class LayoutInfoMapOprtCellParamView : PasswordBoxOprtCellParamViewBase
    {
        public LayoutInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("水平对齐", CreateEnumComboBox<HorizontalAlignType>(nameof(LayoutInfoMapOprtCellParamViewModel.HorizontalAlignment))));
            root.Children.Add(CreateRow("垂直对齐", CreateEnumComboBox<VerticalAlignType>(nameof(LayoutInfoMapOprtCellParamViewModel.VerticalAlignment))));
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

    internal sealed class CommonInfoMapOprtCellParamView : PasswordBoxOprtCellParamViewBase
    {
        public CommonInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("密码值", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.PasswordValue), 200)));
            root.Children.Add(CreateRow("提示文本", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.PlaceholderText), 200)));
            root.Children.Add(CreateCheckBox("密码可见", nameof(CommonInfoMapOprtCellParamViewModel.PasswordVisible)));
            root.Children.Add(CreateRow("鼠标样式", CreateEnumComboBox<CursorType>(nameof(CommonInfoMapOprtCellParamViewModel.CursorType), 200)));
            root.Children.Add(CreateCheckBox("启用", nameof(CommonInfoMapOprtCellParamViewModel.Enabled)));
            Content = root;
        }
    }

    internal sealed class TextInfoMapOprtCellParamView : PasswordBoxOprtCellParamViewBase
    {
        public TextInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("字体", CreateEnumComboBox<FontFamilyType>(nameof(TextInfoMapOprtCellParamViewModel.FontFamily), 200)));
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
