using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using GF_Gereric;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.Objects;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using AControls = Avalonia.Controls;
using APrimitives = Avalonia.Controls.Primitives;

namespace GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.MapOprtCellParamCfgView
{
    internal static class DeviceStatusOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));
        internal static T FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    // ViewModels
    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private decimal? _currentIndex;
        public decimal? CurrentIndex { get => _currentIndex; set => this.RaiseAndSetIfChanged(ref _currentIndex, value); }
        private string _statusName;
        public string StatusName { get => _statusName; set => this.RaiseAndSetIfChanged(ref _statusName, value); }
        private string _deviceStatusValue;
        public string DeviceStatusValue { get => _deviceStatusValue; set => this.RaiseAndSetIfChanged(ref _deviceStatusValue, value); }

        public void FromBytes(byte[] data)
        {
            var temp = DeviceStatusOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data);
            if (temp == null) return;
            CurrentIndex = temp.CurrentIndex;
            StatusName = temp.StatusName;
            DeviceStatusValue = temp.DeviceStatusValue;
        }
        public byte[] ToBytes() => DeviceStatusOprtCellCfgJson.ToBytes(this);
    }

    // Views
    internal abstract class DeviceStatusOprtCellParamViewBase : AControls.UserControl
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
        protected static AControls.NumericUpDown CreateNumericUpDown(string bindingPath, double width = 120)
        {
            var nud = new AControls.NumericUpDown { Width = width, Minimum = 0, Increment = 1, FormatString = "0" };
            nud.Bind(AControls.NumericUpDown.ValueProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay });
            return nud;
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

    internal sealed class CommonInfoMapOprtCellParamView : DeviceStatusOprtCellParamViewBase
    {
        public CommonInfoMapOprtCellParamView()
        {
            var root = CreateRoot();
            root.Children.Add(CreateRow("当前索引", CreateNumericUpDown(nameof(CommonInfoMapOprtCellParamViewModel.CurrentIndex), 120)));
            root.Children.Add(CreateRow("状态名称", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.StatusName), 120)));
            root.Children.Add(CreateRow("设备状态", CreateTextBox(nameof(CommonInfoMapOprtCellParamViewModel.DeviceStatusValue), 120)));
            Content = root;
        }
    }

    // Wrappers
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
}
