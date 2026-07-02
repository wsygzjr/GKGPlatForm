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

using AControls = Avalonia.Controls;
using APrimitives = Avalonia.Controls.Primitives;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider.MapOprtCellParamCfgView
{
    internal static class SliderOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));

        internal static T? FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    internal abstract class SliderOprtCellParamViewBase : AControls.UserControl
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
        private string _backgroundColor = "#FF0000FF";
        public string BackgroundColor { get => _backgroundColor; set => this.RaiseAndSetIfChanged(ref _backgroundColor, value); }

        public void FromBytes(byte[] data)
        {
            var temp = SliderOprtCellCfgJson.FromBytes<BrushInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            BackgroundColor = temp.BackgroundColor ?? BackgroundColor;
        }

        public byte[] ToBytes() => SliderOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class BrushInfoMapOprtCellParamView : SliderOprtCellParamViewBase
    {
        public BrushInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("背景色", CreateColorPicker(nameof(BrushInfoMapOprtCellParamViewModel.BackgroundColor))));
            Content = root;
        }
    }

    internal sealed class AppearanceInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _opacity = "0";
        public string Opacity { get => _opacity; set => this.RaiseAndSetIfChanged(ref _opacity, value); }

        public void FromBytes(byte[] data)
        {
            var temp = SliderOprtCellCfgJson.FromBytes<AppearanceInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            Opacity = temp.Opacity ?? Opacity;
        }

        public byte[] ToBytes() => SliderOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class AppearanceInfoMapOprtCellParamView : SliderOprtCellParamViewBase
    {
        public AppearanceInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("透明度(0-100)", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.Opacity), 160)));
            Content = root;
        }
    }

    internal sealed class LayoutInfoMapOprtCellParamViewModel : ReactiveObject
    {
        // 参数面板默认值和滑块自身默认布局保持一致，避免面板重置时又回到 Stretch。
        private SliderLayoutInfo.HorizontalAlignmentEnum _horizontalAlignment = SliderLayoutInfo.HorizontalAlignmentEnum.Center;
        public SliderLayoutInfo.HorizontalAlignmentEnum HorizontalAlignment { get => _horizontalAlignment; set => this.RaiseAndSetIfChanged(ref _horizontalAlignment, value); }

        private SliderLayoutInfo.VerticalAlignmentEnum _verticalAlignment = SliderLayoutInfo.VerticalAlignmentEnum.Center;
        public SliderLayoutInfo.VerticalAlignmentEnum VerticalAlignment { get => _verticalAlignment; set => this.RaiseAndSetIfChanged(ref _verticalAlignment, value); }

        private string _marginTop = "0";
        public string MarginTop { get => _marginTop; set => this.RaiseAndSetIfChanged(ref _marginTop, value); }

        private string _marginLeft = "0";
        public string MarginLeft { get => _marginLeft; set => this.RaiseAndSetIfChanged(ref _marginLeft, value); }

        private string _marginBottom = "0";
        public string MarginBottom { get => _marginBottom; set => this.RaiseAndSetIfChanged(ref _marginBottom, value); }

        private string _marginRight = "0";
        public string MarginRight { get => _marginRight; set => this.RaiseAndSetIfChanged(ref _marginRight, value); }

        private string _minWidth = "50";
        public string MinWidth { get => _minWidth; set => this.RaiseAndSetIfChanged(ref _minWidth, value); }

        private string _maxWidth = "2147483647";
        public string MaxWidth { get => _maxWidth; set => this.RaiseAndSetIfChanged(ref _maxWidth, value); }

        private string _minHeight = "20";
        public string MinHeight { get => _minHeight; set => this.RaiseAndSetIfChanged(ref _minHeight, value); }

        private string _maxHeight = "2147483647";
        public string MaxHeight { get => _maxHeight; set => this.RaiseAndSetIfChanged(ref _maxHeight, value); }

        public void FromBytes(byte[] data)
        {
            var temp = SliderOprtCellCfgJson.FromBytes<LayoutInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            HorizontalAlignment = temp.HorizontalAlignment;
            VerticalAlignment = temp.VerticalAlignment;
            MarginTop = temp.MarginTop ?? MarginTop;
            MarginLeft = temp.MarginLeft ?? MarginLeft;
            MarginBottom = temp.MarginBottom ?? MarginBottom;
            MarginRight = temp.MarginRight ?? MarginRight;
            MinWidth = temp.MinWidth ?? MinWidth;
            MaxWidth = temp.MaxWidth ?? MaxWidth;
            MinHeight = temp.MinHeight ?? MinHeight;
            MaxHeight = temp.MaxHeight ?? MaxHeight;
        }

        public byte[] ToBytes() => SliderOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class LayoutInfoMapOprtCellParamView : SliderOprtCellParamViewBase
    {
        public LayoutInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("水平对齐", CreateEnumComboBox<SliderLayoutInfo.HorizontalAlignmentEnum>(nameof(LayoutInfoMapOprtCellParamViewModel.HorizontalAlignment), 220)));
            root.Children.Add(CreateRow("垂直对齐", CreateEnumComboBox<SliderLayoutInfo.VerticalAlignmentEnum>(nameof(LayoutInfoMapOprtCellParamViewModel.VerticalAlignment), 220)));
            root.Children.Add(CreateRow("外边距上", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginTop), 120)));
            root.Children.Add(CreateRow("外边距左", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginLeft), 120)));
            root.Children.Add(CreateRow("外边距下", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginBottom), 120)));
            root.Children.Add(CreateRow("外边距右", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginRight), 120)));
            root.Children.Add(CreateRow("最小宽度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MinWidth), 120)));
            root.Children.Add(CreateRow("最大宽度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MaxWidth), 160)));
            root.Children.Add(CreateRow("最小高度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MinHeight), 120)));
            root.Children.Add(CreateRow("最大高度", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MaxHeight), 160)));
            Content = root;
        }
    }

    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _maximum = "100";
        public string Maximum { get => _maximum; set => this.RaiseAndSetIfChanged(ref _maximum, value); }

        private string _minimum = "0";
        public string Minimum { get => _minimum; set => this.RaiseAndSetIfChanged(ref _minimum, value); }

        private SliderCommonInfo.DirectionEnum _direction = SliderCommonInfo.DirectionEnum.水平;
        public SliderCommonInfo.DirectionEnum Direction { get => _direction; set => this.RaiseAndSetIfChanged(ref _direction, value); }

        private string _smallChange = "1";
        public string SmallChange { get => _smallChange; set => this.RaiseAndSetIfChanged(ref _smallChange, value); }

        private string _value = "50";
        public string Value { get => _value; set => this.RaiseAndSetIfChanged(ref _value, value); }

        private string _tickFrequency = "10";
        public string TickFrequency { get => _tickFrequency; set => this.RaiseAndSetIfChanged(ref _tickFrequency, value); }

        private TickPlacement _tickPlacement = TickPlacement.None;
        public TickPlacement TickPlacement { get => _tickPlacement; set => this.RaiseAndSetIfChanged(ref _tickPlacement, value); }

        private CommonCursorType _hoverCursor = CommonCursorType.手型;
        public CommonCursorType HoverCursor { get => _hoverCursor; set => this.RaiseAndSetIfChanged(ref _hoverCursor, value); }

        private bool _enabled = true;
        public bool Enabled { get => _enabled; set => this.RaiseAndSetIfChanged(ref _enabled, value); }

        private string _tooltipText = "";
        public string TooltipText { get => _tooltipText; set => this.RaiseAndSetIfChanged(ref _tooltipText, value); }

        public void FromBytes(byte[] data)
        {
            var temp = SliderOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            Maximum = temp.Maximum ?? Maximum;
            Minimum = temp.Minimum ?? Minimum;
            Direction = temp.Direction;
            SmallChange = temp.SmallChange ?? SmallChange;
            Value = temp.Value ?? Value;
            TickFrequency = temp.TickFrequency ?? TickFrequency;
            TickPlacement = temp.TickPlacement;
            HoverCursor = temp.HoverCursor;
            Enabled = temp.Enabled;
            TooltipText = temp.TooltipText ?? TooltipText;
        }

        public byte[] ToBytes() => SliderOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class CommonInfoMapOprtCellParamView : SliderOprtCellParamViewBase
    {
        public CommonInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("最大值", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.Maximum), 120)));
            root.Children.Add(CreateRow("最小值", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.Minimum), 120)));
            root.Children.Add(CreateRow("方向", CreateEnumComboBox<SliderCommonInfo.DirectionEnum>(nameof(CommonInfoMapOprtCellParamViewModel.Direction), 220)));
            root.Children.Add(CreateRow("步长", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.SmallChange), 120)));
            root.Children.Add(CreateRow("当前值", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.Value), 120)));
            root.Children.Add(CreateRow("刻度线间隔", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.TickFrequency), 120)));
            root.Children.Add(CreateRow("刻度线位置", CreateEnumComboBox<TickPlacement>(nameof(CommonInfoMapOprtCellParamViewModel.TickPlacement), 220)));
            root.Children.Add(CreateRow("悬停光标", CreateEnumComboBox<CommonCursorType>(nameof(CommonInfoMapOprtCellParamViewModel.HoverCursor), 220)));
            root.Children.Add(CreateRow("启用", CreateCheckBox("启用", nameof(CommonInfoMapOprtCellParamViewModel.Enabled))));
            root.Children.Add(CreateRow("提示文本", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.TooltipText), 260)));
            Content = root;
        }
    }

    #region CfgView wrappers (implement IMapOprtCellParamCfgView)

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

    #endregion
}
