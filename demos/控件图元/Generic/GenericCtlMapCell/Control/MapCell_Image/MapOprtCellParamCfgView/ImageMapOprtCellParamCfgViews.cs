using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using GKG.Map.MapCell.Generic.Control.MapCell_Image.Objects;
using GKG.Map.MapCell.Generic.Control.Lable;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

using AControls = Avalonia.Controls;
using APrimitives = Avalonia.Controls.Primitives;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Image.MapOprtCellParamCfgView
{
    internal static class ImageOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));
        internal static T FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    #region ViewModels

    internal sealed class AppearanceInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _opacity = "1";
        public string Opacity { get => _opacity; set => this.RaiseAndSetIfChanged(ref _opacity, value); }

        public void FromBytes(byte[] data)
        {
            var temp = ImageOprtCellCfgJson.FromBytes<AppearanceInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            Opacity = temp.Opacity ?? Opacity;
        }
        public byte[] ToBytes() => ImageOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private ImageStretchMode? _stretchMode;
        public ImageStretchMode? StretchMode { get => _stretchMode; set => this.RaiseAndSetIfChanged(ref _stretchMode, value); }

        private bool? _isEnabled;
        public bool? IsEnabled { get => _isEnabled; set => this.RaiseAndSetIfChanged(ref _isEnabled, value); }

        private string _toolTip = "";
        public string ToolTip { get => _toolTip; set => this.RaiseAndSetIfChanged(ref _toolTip, value); }

        public void FromBytes(byte[] data)
        {
            var temp = ImageOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            StretchMode = temp.StretchMode;
            IsEnabled = temp.IsEnabled;
            ToolTip = temp.ToolTip ?? ToolTip;
        }
        public byte[] ToBytes() => ImageOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class LayoutInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private HorizontalAlignType? _horizontalAlign;
        public HorizontalAlignType? HorizontalAlign { get => _horizontalAlign; set => this.RaiseAndSetIfChanged(ref _horizontalAlign, value); }
        private VerticalAlignType? _verticalAlign;
        public VerticalAlignType? VerticalAlign { get => _verticalAlign; set => this.RaiseAndSetIfChanged(ref _verticalAlign, value); }
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
            var temp = ImageOprtCellCfgJson.FromBytes<LayoutInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            HorizontalAlign = temp.HorizontalAlign; VerticalAlign = temp.VerticalAlign;
            MarginTop = temp.MarginTop ?? MarginTop; MarginLeft = temp.MarginLeft ?? MarginLeft;
            MarginBottom = temp.MarginBottom ?? MarginBottom; MarginRight = temp.MarginRight ?? MarginRight;
            MinWidth = temp.MinWidth ?? MinWidth; MaxWidth = temp.MaxWidth ?? MaxWidth;
            MinHeight = temp.MinHeight ?? MinHeight; MaxHeight = temp.MaxHeight ?? MaxHeight;
        }
        public byte[] ToBytes() => ImageOprtCellCfgJson.ToBytes(this);
    }

    #endregion

    #region Views

    internal abstract class ImageOprtCellParamViewBase : AControls.UserControl
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
        protected static AControls.ComboBox CreateNullableEnumComboBox<TEnum>(string bindingPath, double width = 160) where TEnum : struct, Enum
        {
            var items = new List<TEnum?> { null };
            items.AddRange(Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(e => (TEnum?)e));
            var cb = new AControls.ComboBox { Width = width, ItemsSource = items };
            cb.Bind(APrimitives.SelectingItemsControl.SelectedItemProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return cb;
        }
    }

    internal sealed class AppearanceInfoMapOprtCellParamView : ImageOprtCellParamViewBase
    {
        public AppearanceInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("透明度(0~1)", CreateTextBox(nameof(AppearanceInfoMapOprtCellParamViewModel.Opacity), 80)));
            Content = root;
        }
    }

    internal sealed class CommonInfoMapOprtCellParamView : ImageOprtCellParamViewBase
    {
        public CommonInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("填充方式", CreateNullableEnumComboBox<ImageStretchMode>(nameof(CommonInfoMapOprtCellParamViewModel.StretchMode))));
            root.Children.Add(CreateCheckBox("启用", nameof(CommonInfoMapOprtCellParamViewModel.IsEnabled)));
            root.Children.Add(CreateRow("提示文字", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.ToolTip), 240)));
            Content = root;
        }
    }

    internal sealed class LayoutInfoMapOprtCellParamView : ImageOprtCellParamViewBase
    {
        public LayoutInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("水平对齐", CreateNullableEnumComboBox<HorizontalAlignType>(nameof(LayoutInfoMapOprtCellParamViewModel.HorizontalAlign))));
            root.Children.Add(CreateRow("垂直对齐", CreateNullableEnumComboBox<VerticalAlignType>(nameof(LayoutInfoMapOprtCellParamViewModel.VerticalAlign))));
            root.Children.Add(CreateRow("上边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginTop), 80)));
            root.Children.Add(CreateRow("左边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginLeft), 80)));
            root.Children.Add(CreateRow("下边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginBottom), 80)));
            root.Children.Add(CreateRow("右边距", CreateTextBox(nameof(LayoutInfoMapOprtCellParamViewModel.MarginRight), 80)));
            Content = root;
        }
    }

    #endregion

    #region CfgView wrappers

    internal sealed class AppearanceInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly AppearanceInfoMapOprtCellParamView _view;
        private readonly AppearanceInfoMapOprtCellParamViewModel _vm;
        public AppearanceInfoMapOprtCellParamCfgView() { _vm = new(); _view = new() { DataContext = _vm }; }
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
        public CommonInfoMapOprtCellParamCfgView() { _vm = new(); _view = new() { DataContext = _vm }; }
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
        public LayoutInfoMapOprtCellParamCfgView() { _vm = new(); _view = new() { DataContext = _vm }; }
        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();
        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    #endregion
}
