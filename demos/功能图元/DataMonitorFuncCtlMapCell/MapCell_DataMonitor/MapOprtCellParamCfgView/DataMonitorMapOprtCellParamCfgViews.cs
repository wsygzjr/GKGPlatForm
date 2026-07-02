using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Styling;
using GF_Gereric;
using GKG.Map.DataMonitorFuncCtlMapCell;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using APrimitives = Avalonia.Controls.Primitives;

namespace GKG.Map.DataMonitorFuncCtlMapCell.MapCell_DataMonitor.MapOprtCellParamCfgView
{
    /// <summary>
    /// 数据监控操作单元格配置序列化器
    /// 提供序列化和反序列化功能
    /// </summary>
    internal static class DataMonitorOprtCellCfgSerializer
    {
        /// <summary>
        /// 将对象序列化为字节数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="vm">要序列化的对象</param>
        /// <returns>序列化后的字节数组</returns>
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));

        /// <summary>
        /// 从字节数组反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">字节数组</param>
        /// <returns>反序列化后的对象</returns>
        internal static T? FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0)
                return default;
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    internal sealed class StringToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string s)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return Colors.Transparent;

                try
                {
                    return Color.Parse(s);
                }
                catch
                {
                    return Colors.Transparent;
                }
            }

            return Colors.Transparent;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color c)
                return c.ToString();

            return string.Empty;
        }
    }

    /// <summary>
    /// 布尔值到字符串转换器
    /// </summary>
    internal class BoolToStringConverter : IValueConverter
    {
        private readonly string _trueValue;
        private readonly string _falseValue;

        public BoolToStringConverter(string trueValue, string falseValue)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
        }

        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? _trueValue : _falseValue;
            }
            return _falseValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue == _trueValue;
            }
            return false;
        }
    }

    /// <summary>
    /// 双精度浮点数到十进制数转换器
    /// </summary>
    internal class DoubleToDecimalConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return (decimal)doubleValue;
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return (double)decimalValue;
            }
            return null;
        }
    }

    /// <summary>
    /// 字符串到画笔转换器
    /// </summary>
    internal class StringToBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string colorString)
            {
                try
                {
                    var color = Color.Parse(colorString);
                    return new SolidColorBrush(color);
                }
                catch
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color.ToString();
            }
            return "#000000";
        }
    }

    public sealed class SystemStatusMapOprtCellParamViewModel : ReactiveObject
    {
        private bool _safetyDoorStatus = true;
        public bool SafetyDoorStatus
        {
            get => _safetyDoorStatus;
            set => this.RaiseAndSetIfChanged(ref _safetyDoorStatus, value);
        }

        private bool _totalPressureStatus = true;
        public bool TotalPressureStatus
        {
            get => _totalPressureStatus;
            set => this.RaiseAndSetIfChanged(ref _totalPressureStatus, value);
        }

        private bool _cleaningClothStatus = true;
        public bool CleaningClothStatus
        {
            get => _cleaningClothStatus;
            set => this.RaiseAndSetIfChanged(ref _cleaningClothStatus, value);
        }

        private bool _isDualValve = false;
        public bool IsDualValve
        {
            get => _isDualValve;
            set => this.RaiseAndSetIfChanged(ref _isDualValve, value);
        }

        private string _glueRemaining = "50%";
        public string GlueRemaining
        {
            get => _glueRemaining;
            set => this.RaiseAndSetIfChanged(ref _glueRemaining, value ?? string.Empty);
        }

        private string _leftGlueRemaining = "50%";
        public string LeftGlueRemaining
        {
            get => _leftGlueRemaining;
            set => this.RaiseAndSetIfChanged(ref _leftGlueRemaining, value ?? string.Empty);
        }

        private string _calibration = "**h|**mg|**pcs";
        public string Calibration
        {
            get => _calibration;
            set => this.RaiseAndSetIfChanged(ref _calibration, value ?? string.Empty);
        }

        private string _leftCalibration = "**h|**mg|**pcs";
        public string LeftCalibration
        {
            get => _leftCalibration;
            set => this.RaiseAndSetIfChanged(ref _leftCalibration, value ?? string.Empty);
        }

        private string _valveBodyValue = "**h";
        public string ValveBodyValue
        {
            get => _valveBodyValue;
            set => this.RaiseAndSetIfChanged(ref _valveBodyValue, value ?? string.Empty);
        }

        private string _leftValveBodyValue = "**h";
        public string LeftValveBodyValue
        {
            get => _leftValveBodyValue;
            set => this.RaiseAndSetIfChanged(ref _leftValveBodyValue, value ?? string.Empty);
        }

        private string _sealingRingValue = "**h";
        public string SealingRingValue
        {
            get => _sealingRingValue;
            set => this.RaiseAndSetIfChanged(ref _sealingRingValue, value ?? string.Empty);
        }

        private string _leftSealingRingValue = "**h";
        public string LeftSealingRingValue
        {
            get => _leftSealingRingValue;
            set => this.RaiseAndSetIfChanged(ref _leftSealingRingValue, value ?? string.Empty);
        }

        public void FromBytes(byte[] data)
        {
            var temp = DataMonitorOprtCellCfgSerializer.FromBytes<SystemStatusMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            SafetyDoorStatus = temp.SafetyDoorStatus;
            TotalPressureStatus = temp.TotalPressureStatus;
            CleaningClothStatus = temp.CleaningClothStatus;
            IsDualValve = temp.IsDualValve;
            GlueRemaining = temp.GlueRemaining;
            LeftGlueRemaining = temp.LeftGlueRemaining;
            Calibration = temp.Calibration;
            LeftCalibration = temp.LeftCalibration;
            ValveBodyValue = temp.ValveBodyValue;
            LeftValveBodyValue = temp.LeftValveBodyValue;
            SealingRingValue = temp.SealingRingValue;
            LeftSealingRingValue = temp.LeftSealingRingValue;
        }

        public byte[] ToBytes() => DataMonitorOprtCellCfgSerializer.ToBytes(this);
    }

    internal sealed class SystemStatusMapOprtCellParamView : UserControl
    {
        public SystemStatusMapOprtCellParamView()
        {
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("Auto,*"),
                RowDefinitions = new RowDefinitions(),
                Margin = new Thickness(10)
            };

            void AddRow(string label, Control editor)
            {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                var rowIndex = grid.RowDefinitions.Count - 1;

                var text = new TextBlock
                {
                    Text = label,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 10, 0)
                };
                Grid.SetRow(text, rowIndex);
                Grid.SetColumn(text, 0);

                editor.VerticalAlignment = VerticalAlignment.Center;
                editor.HorizontalAlignment = HorizontalAlignment.Stretch;
                Grid.SetRow(editor, rowIndex);
                Grid.SetColumn(editor, 1);

                grid.Children.Add(text);
                grid.Children.Add(editor);
            }

            var safety = new CheckBox { Content = string.Empty };
            safety.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.SafetyDoorStatus)) { Mode = BindingMode.TwoWay });
            AddRow("安全门状态", safety);

            var pressure = new CheckBox { Content = string.Empty };
            pressure.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.TotalPressureStatus)) { Mode = BindingMode.TwoWay });
            AddRow("总气压状态", pressure);

            var cloth = new CheckBox { Content = string.Empty };
            cloth.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.CleaningClothStatus)) { Mode = BindingMode.TwoWay });
            AddRow("清洁布状态", cloth);

            var dual = new CheckBox { Content = string.Empty };
            dual.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.IsDualValve)) { Mode = BindingMode.TwoWay });
            AddRow("双阀模式", dual);

            var glueRemaining = new TextBox { Width = 260 };
            glueRemaining.Bind(TextBox.TextProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.GlueRemaining)) { Mode = BindingMode.TwoWay });
            AddRow("胶水余量", glueRemaining);

            var leftGlueRemaining = new TextBox { Width = 260 };
            leftGlueRemaining.Bind(TextBox.TextProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.LeftGlueRemaining)) { Mode = BindingMode.TwoWay });
            AddRow("左阀胶水余量", leftGlueRemaining);

            var calibration = new TextBox { Width = 260 };
            calibration.Bind(TextBox.TextProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.Calibration)) { Mode = BindingMode.TwoWay });
            AddRow("校正", calibration);

            var leftCalibration = new TextBox { Width = 260 };
            leftCalibration.Bind(TextBox.TextProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.LeftCalibration)) { Mode = BindingMode.TwoWay });
            AddRow("左阀校正", leftCalibration);

            var valveBodyValue = new TextBox { Width = 260 };
            valveBodyValue.Bind(TextBox.TextProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.ValveBodyValue)) { Mode = BindingMode.TwoWay });
            AddRow("阀体值", valveBodyValue);

            var leftValveBodyValue = new TextBox { Width = 260 };
            leftValveBodyValue.Bind(TextBox.TextProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.LeftValveBodyValue)) { Mode = BindingMode.TwoWay });
            AddRow("左阀阀体值", leftValveBodyValue);

            var sealingRingValue = new TextBox { Width = 260 };
            sealingRingValue.Bind(TextBox.TextProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.SealingRingValue)) { Mode = BindingMode.TwoWay });
            AddRow("密封圈值", sealingRingValue);

            var leftSealingRingValue = new TextBox { Width = 260 };
            leftSealingRingValue.Bind(TextBox.TextProperty, new Binding(nameof(SystemStatusMapOprtCellParamViewModel.LeftSealingRingValue)) { Mode = BindingMode.TwoWay });
            AddRow("左阀密封圈值", leftSealingRingValue);

            Content = grid;
        }
    }

    public sealed class SystemStatusMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly SystemStatusMapOprtCellParamView _view;
        private readonly SystemStatusMapOprtCellParamViewModel _vm;

        public SystemStatusMapOprtCellParamCfgView()
        {
            _vm = new SystemStatusMapOprtCellParamViewModel();
            _view = new SystemStatusMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();

        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    /// <summary>
    /// 供阀气压操作单元格参数视图模型
    /// 用于配置供阀气压的详细属性
    /// </summary>
    public sealed class SupplyValvePressureMapOprtCellParamViewModel : ReactiveObject
    {
        private string _statusName = "供阀气压";
        public string StatusName
        {
            get => _statusName;
            set => this.RaiseAndSetIfChanged(ref _statusName, value);
        }

        private string _statusValue = "0.000";
        public string StatusValue
        {
            get => _statusValue;
            set => this.RaiseAndSetIfChanged(ref _statusValue, value);
        }

        private string _statusUnit = "kgf/cm²";
        public string StatusUnit
        {
            get => _statusUnit;
            set => this.RaiseAndSetIfChanged(ref _statusUnit, value);
        }

        private bool _statusSwitch = true;
        public bool StatusSwitch
        {
            get => _statusSwitch;
            set => this.RaiseAndSetIfChanged(ref _statusSwitch, value);
        }

        /// <summary>
        /// 从字节数组加载配置
        /// </summary>
        /// <param name="data">配置数据</param>
        public void FromBytes(byte[] data)
        {
            var temp = DataMonitorOprtCellCfgSerializer.FromBytes<SupplyValvePressureMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            StatusName = temp.StatusName;
            StatusValue = temp.StatusValue;
            StatusUnit = temp.StatusUnit;
            StatusSwitch = temp.StatusSwitch;
        }

        /// <summary>
        /// 将配置转换为字节数组
        /// </summary>
        /// <returns>配置数据的字节数组</returns>
        public byte[] ToBytes() => DataMonitorOprtCellCfgSerializer.ToBytes(this);
    }

    /// <summary>
    /// 供阀气压操作单元格参数视图
    /// 用于配置供阀气压的详细属性的UI界面
    /// </summary>
    internal sealed class SupplyValvePressureMapOprtCellParamView : UserControl
    {
        /// <summary>
        /// 初始化供阀气压操作单元格参数视图
        /// </summary>
        public SupplyValvePressureMapOprtCellParamView()
        {
            var root = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 8,
                Margin = new Thickness(10)
            };

            var nameLabel = new TextBlock { Text = "供阀气压名称:" };
            var nameTextBox = new TextBox { Width = 200 };
            nameTextBox.Bind(TextBox.TextProperty, new Binding(nameof(SupplyValvePressureMapOprtCellParamViewModel.StatusName)) { Mode = BindingMode.TwoWay });

            var valueLabel = new TextBlock { Text = "供阀气压状态值:" };
            var valueTextBox = new TextBox { Width = 200 };
            valueTextBox.Bind(TextBox.TextProperty, new Binding(nameof(SupplyValvePressureMapOprtCellParamViewModel.StatusValue)) { Mode = BindingMode.TwoWay });

            var unitLabel = new TextBlock { Text = "供阀气压单位:" };
            var unitTextBox = new TextBox { Width = 200 };
            unitTextBox.Bind(TextBox.TextProperty, new Binding(nameof(SupplyValvePressureMapOprtCellParamViewModel.StatusUnit)) { Mode = BindingMode.TwoWay });

            var switchCheckBox = new CheckBox { Content = "供阀气压开关" }; 
            switchCheckBox.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(nameof(SupplyValvePressureMapOprtCellParamViewModel.StatusSwitch)) { Mode = BindingMode.TwoWay });

            root.Children.Add(nameLabel);
            root.Children.Add(nameTextBox);
            root.Children.Add(valueLabel);
            root.Children.Add(valueTextBox);
            root.Children.Add(unitLabel);
            root.Children.Add(unitTextBox);
            root.Children.Add(switchCheckBox);

            Content = root;
        }
    }

    /// <summary>
    /// 供阀气压操作单元格参数配置视图
    /// 实现IMapOprtCellParamCfgView接口，提供参数配置功能
    /// </summary>
    public sealed class SupplyValvePressureMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly SupplyValvePressureMapOprtCellParamView _view;
        private readonly SupplyValvePressureMapOprtCellParamViewModel _vm;

        /// <summary>
        /// 初始化供阀气压操作单元格参数配置视图
        /// </summary>
        public SupplyValvePressureMapOprtCellParamCfgView()
        {
            _vm = new SupplyValvePressureMapOprtCellParamViewModel();
            _view = new SupplyValvePressureMapOprtCellParamView { DataContext = _vm };
        }

        /// <summary>
        /// 获取视图对象
        /// </summary>
        public object View => _view;
        
        /// <summary>
        /// 设置配置数据
        /// </summary>
        /// <param name="data">配置数据</param>
        public void SetData(byte[] data) => _vm.FromBytes(data);
        
        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <returns>配置数据</returns>
        public byte[] GetData() => _vm.ToBytes();

        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    /// <summary>
    /// 供胶气压操作单元格参数视图模型
    /// 用于配置供胶气压的详细属性
    /// </summary>
    public sealed class SupplyGluePressureMapOprtCellParamViewModel : ReactiveObject
    {
        private string _statusName = "供胶气压";
        public string StatusName
        {
            get => _statusName;
            set => this.RaiseAndSetIfChanged(ref _statusName, value);
        }

        private string _statusValue = "0.000";
        public string StatusValue
        {
            get => _statusValue;
            set => this.RaiseAndSetIfChanged(ref _statusValue, value);
        }

        private string _statusUnit = "kgf/cm²";
        public string StatusUnit
        {
            get => _statusUnit;
            set => this.RaiseAndSetIfChanged(ref _statusUnit, value);
        }

        private bool _statusSwitch = true;
        public bool StatusSwitch
        {
            get => _statusSwitch;
            set => this.RaiseAndSetIfChanged(ref _statusSwitch, value);
        }
        
        /// <summary>
        /// 从字节数组加载配置
        /// </summary>
        /// <param name="data">配置数据</param>
        public void FromBytes(byte[] data)
        {
            var temp = DataMonitorOprtCellCfgSerializer.FromBytes<SupplyGluePressureMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            StatusName = temp.StatusName;
            StatusValue = temp.StatusValue;
            StatusUnit = temp.StatusUnit;
            StatusSwitch = temp.StatusSwitch;
        }

        /// <summary>
        /// 将配置转换为字节数组
        /// </summary>
        /// <returns>配置数据的字节数组</returns>
        public byte[] ToBytes() => DataMonitorOprtCellCfgSerializer.ToBytes(this);
    }

    /// <summary>
    /// 供胶气压操作单元格参数视图
    /// 用于配置供胶气压的详细属性的UI界面
    /// </summary>
    internal sealed class SupplyGluePressureMapOprtCellParamView : UserControl
    {
        /// <summary>
        /// 初始化供胶气压操作单元格参数视图
        /// </summary>
        public SupplyGluePressureMapOprtCellParamView()
        {
            var root = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 8,
                Margin = new Thickness(10)
            };

            var nameLabel = new TextBlock { Text = "供胶气压名称:" };
            var nameTextBox = new TextBox { Width = 200 };
            nameTextBox.Bind(TextBox.TextProperty, new Binding(nameof(SupplyGluePressureMapOprtCellParamViewModel.StatusName)) { Mode = BindingMode.TwoWay });

            var valueLabel = new TextBlock { Text = "供胶气压状态值:" };
            var valueTextBox = new TextBox { Width = 200 };
            valueTextBox.Bind(TextBox.TextProperty, new Binding(nameof(SupplyGluePressureMapOprtCellParamViewModel.StatusValue)) { Mode = BindingMode.TwoWay });

            var unitLabel = new TextBlock { Text = "供胶气压单位:" };
            var unitTextBox = new TextBox { Width = 200 };
            unitTextBox.Bind(TextBox.TextProperty, new Binding(nameof(SupplyGluePressureMapOprtCellParamViewModel.StatusUnit)) { Mode = BindingMode.TwoWay });

            var switchCheckBox = new CheckBox { Content = "供胶气压开关" };
            switchCheckBox.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(nameof(SupplyGluePressureMapOprtCellParamViewModel.StatusSwitch)) { Mode = BindingMode.TwoWay });

            root.Children.Add(nameLabel);
            root.Children.Add(nameTextBox);
            root.Children.Add(valueLabel);
            root.Children.Add(valueTextBox);
            root.Children.Add(unitLabel);
            root.Children.Add(unitTextBox);
            root.Children.Add(switchCheckBox);

            Content = root;
        }
    }

    /// <summary>
    /// 供胶气压操作单元格参数配置视图
    /// 实现IMapOprtCellParamCfgView接口，提供参数配置功能
    /// </summary>
    public sealed class SupplyGluePressureMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly SupplyGluePressureMapOprtCellParamView _view;
        private readonly SupplyGluePressureMapOprtCellParamViewModel _vm;

        /// <summary>
        /// 初始化供胶气压操作单元格参数配置视图
        /// </summary>
        public SupplyGluePressureMapOprtCellParamCfgView()
        {
            _vm = new SupplyGluePressureMapOprtCellParamViewModel();
            _view = new SupplyGluePressureMapOprtCellParamView { DataContext = _vm };
        }

        /// <summary>
        /// 获取视图对象
        /// </summary>
        public object View => _view;
        
        /// <summary>
        /// 设置配置数据
        /// </summary>
        /// <param name="data">配置数据</param>
        public void SetData(byte[] data) => _vm.FromBytes(data);
        
        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <returns>配置数据</returns>
        public byte[] GetData() => _vm.ToBytes();

        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    /// <summary>
    /// 喷嘴加热操作单元格参数视图模型
    /// 用于配置喷嘴加热的详细属性
    /// </summary>
    public sealed class NozzleHeatingMapOprtCellParamViewModel : ReactiveObject
    {
        private string _statusName = "喷嘴加热";
        public string StatusName
        {
            get => _statusName;
            set => this.RaiseAndSetIfChanged(ref _statusName, value);
        }

        private string _statusValue = "0.0";
        public string StatusValue
        {
            get => _statusValue;
            set => this.RaiseAndSetIfChanged(ref _statusValue, value);
        }

        private string _statusUnit = "°C";
        public string StatusUnit
        {
            get => _statusUnit;
            set => this.RaiseAndSetIfChanged(ref _statusUnit, value);
        }

        private bool _statusSwitch = true;
        public bool StatusSwitch
        {
            get => _statusSwitch;
            set => this.RaiseAndSetIfChanged(ref _statusSwitch, value);
        }

        /// <summary>
        /// 从字节数组加载配置
        /// </summary>
        /// <param name="data">配置数据</param>
        public void FromBytes(byte[] data)
        {
            var temp = DataMonitorOprtCellCfgSerializer.FromBytes<NozzleHeatingMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            StatusName = temp.StatusName;
            StatusValue = temp.StatusValue;
            StatusUnit = temp.StatusUnit;
            StatusSwitch = temp.StatusSwitch;
        }

        /// <summary>
        /// 将配置转换为字节数组
        /// </summary>
        /// <returns>配置数据的字节数组</returns>
        public byte[] ToBytes() => DataMonitorOprtCellCfgSerializer.ToBytes(this);
    }

    /// <summary>
    /// 喷嘴加热操作单元格参数视图
    /// 用于配置喷嘴加热的详细属性的UI界面
    /// </summary>
    internal sealed class NozzleHeatingMapOprtCellParamView : UserControl
    {
        /// <summary>
        /// 初始化喷嘴加热操作单元格参数视图
        /// </summary>
        public NozzleHeatingMapOprtCellParamView()
        {
            var root = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 8,
                Margin = new Thickness(10)
            };

            var nameLabel = new TextBlock { Text = "喷嘴加热名称:" };
            var nameTextBox = new TextBox { Width = 200 };
            nameTextBox.Bind(TextBox.TextProperty, new Binding(nameof(NozzleHeatingMapOprtCellParamViewModel.StatusName)) { Mode = BindingMode.TwoWay });

            var valueLabel = new TextBlock { Text = "喷嘴加热温度值:" };
            var valueTextBox = new TextBox { Width = 200 };
            valueTextBox.Bind(TextBox.TextProperty, new Binding(nameof(NozzleHeatingMapOprtCellParamViewModel.StatusValue)) { Mode = BindingMode.TwoWay });

            var unitLabel = new TextBlock { Text = "喷嘴加热单位:" };
            var unitTextBox = new TextBox { Width = 200 };
            unitTextBox.Bind(TextBox.TextProperty, new Binding(nameof(NozzleHeatingMapOprtCellParamViewModel.StatusUnit)) { Mode = BindingMode.TwoWay });

            var switchCheckBox = new CheckBox { Content = "喷嘴加热开关" };
            switchCheckBox.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(nameof(NozzleHeatingMapOprtCellParamViewModel.StatusSwitch)) { Mode = BindingMode.TwoWay });

            root.Children.Add(nameLabel);
            root.Children.Add(nameTextBox);
            root.Children.Add(valueLabel);
            root.Children.Add(valueTextBox);
            root.Children.Add(unitLabel);
            root.Children.Add(unitTextBox);
            root.Children.Add(switchCheckBox);

            Content = root;
        }
    }

    /// <summary>
    /// 喷嘴加热操作单元格参数配置视图
    /// 实现IMapOprtCellParamCfgView接口，提供参数配置功能
    /// </summary>
    public sealed class NozzleHeatingMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly NozzleHeatingMapOprtCellParamView _view;
        private readonly NozzleHeatingMapOprtCellParamViewModel _vm;

        /// <summary>
        /// 初始化喷嘴加热操作单元格参数配置视图
        /// </summary>
        public NozzleHeatingMapOprtCellParamCfgView()
        {
            _vm = new NozzleHeatingMapOprtCellParamViewModel();
            _view = new NozzleHeatingMapOprtCellParamView { DataContext = _vm };
        }

        /// <summary>
        /// 获取视图对象
        /// </summary>
        public object View => _view;
        
        /// <summary>
        /// 设置配置数据
        /// </summary>
        /// <param name="data">配置数据</param>
        public void SetData(byte[] data) => _vm.FromBytes(data);
        
        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <returns>配置数据</returns>
        public byte[] GetData() => _vm.ToBytes();

        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    /// <summary>
    /// 颜色属性操作单元格参数视图模型
    /// 用于配置文本颜色和背景颜色
    /// </summary>
    public sealed class ColorPropertyMapOprtCellParamViewModel : ReactiveObject
    {
        private string _color = "#000000";
        public string Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        /// <summary>
        /// 从字节数组加载配置
        /// </summary>
        /// <param name="data">配置数据</param>
        public void FromBytes(byte[] data)
        {
            var temp = DataMonitorOprtCellCfgSerializer.FromBytes<ColorPropertyMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            Color = temp.Color;
        }

        /// <summary>
        /// 将配置转换为字节数组
        /// </summary>
        /// <returns>配置数据的字节数组</returns>
        public byte[] ToBytes() => DataMonitorOprtCellCfgSerializer.ToBytes(this);
    }

    /// <summary>
    /// 颜色属性操作单元格参数视图
    /// 用于配置颜色属性的UI界面
    /// </summary>
    internal sealed class ColorPropertyMapOprtCellParamView : UserControl
    {
        private static bool _colorPickerStylesInjected;

        /// <summary>
        /// 初始化颜色属性操作单元格参数视图
        /// </summary>
        public ColorPropertyMapOprtCellParamView()
        {
            EnsureColorPickerStyles();

            var root = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 8,
                Margin = new Thickness(10)
            };

            var colorView = new ColorView
            {
                Width = 360,
                Height = 360,
                IsAlphaEnabled = true,
                IsAlphaVisible = true,
                IsHexInputVisible = true,
                IsColorSpectrumVisible = true,
                IsColorPaletteVisible = false,
                IsColorComponentsVisible = false,
                SelectedIndex = 0
            };
            colorView.Bind(ColorView.ColorProperty, new Binding(nameof(ColorPropertyMapOprtCellParamViewModel.Color))
            {
                Mode = BindingMode.TwoWay,
                Converter = new StringToColorConverter()
            });

            root.Children.Add(colorView);
            Content = root;
        }

        private static void EnsureColorPickerStyles()
        {
            if (_colorPickerStylesInjected)
                return;

            var app = Application.Current;
            if (app?.Styles == null)
                return;

            var source = new Uri("avares://Avalonia.Controls.ColorPicker/Themes/Fluent/Fluent.xaml");
            foreach (var style in app.Styles)
            {
                if (style is StyleInclude include && include.Source == source)
                {
                    _colorPickerStylesInjected = true;
                    return;
                }
            }

            var baseUri = new Uri("avares://Griffins.Map.DataMonitorFuncCtlMapCell/");
            app.Styles.Add(new StyleInclude(baseUri) { Source = source });
            _colorPickerStylesInjected = true;
        }
    }

    /// <summary>
    /// 颜色属性操作单元格参数配置视图
    /// 实现IMapOprtCellParamCfgView接口，提供参数配置功能
    /// </summary>
    public sealed class ColorPropertyMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly ColorPropertyMapOprtCellParamView _view;
        private readonly ColorPropertyMapOprtCellParamViewModel _vm;

        public ColorPropertyMapOprtCellParamCfgView()
        {
            _vm = new ColorPropertyMapOprtCellParamViewModel();
            _view = new ColorPropertyMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();

        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    /// <summary>
    /// 字体属性操作单元格参数视图模型
    /// 用于配置文本字体
    /// </summary>
    public sealed class FontPropertyMapOprtCellParamViewModel : ReactiveObject
    {
        private string _fontFamily = "Arial";
        public string FontFamily
        {
            get => _fontFamily;
            set => this.RaiseAndSetIfChanged(ref _fontFamily, value);
        }

        private double _fontSize = 12;
        public double FontSize
        {
            get => _fontSize;
            set => this.RaiseAndSetIfChanged(ref _fontSize, value);
        }

        private bool _isBold;
        public bool IsBold
        {
            get => _isBold;
            set => this.RaiseAndSetIfChanged(ref _isBold, value);
        }

        private bool _isItalic;
        public bool IsItalic
        {
            get => _isItalic;
            set => this.RaiseAndSetIfChanged(ref _isItalic, value);
        }

        /// <summary>
        /// 从字节数组加载配置
        /// </summary>
        /// <param name="data">配置数据</param>
        public void FromBytes(byte[] data)
        {
            var temp = DataMonitorOprtCellCfgSerializer.FromBytes<FontPropertyMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            FontFamily = temp.FontFamily;
            FontSize = temp.FontSize;
            IsBold = temp.IsBold;
            IsItalic = temp.IsItalic;
        }

        /// <summary>
        /// 将配置转换为字节数组
        /// </summary>
        /// <returns>配置数据的字节数组</returns>
        public byte[] ToBytes() => DataMonitorOprtCellCfgSerializer.ToBytes(this);
    }

    /// <summary>
    /// 字体属性操作单元格参数视图
    /// 用于配置字体属性的UI界面
    /// </summary>
    internal sealed class FontPropertyMapOprtCellParamView : UserControl
    {
        /// <summary>
        /// 初始化字体属性操作单元格参数视图
        /// </summary>
        public FontPropertyMapOprtCellParamView()
        {
            var root = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("Auto,*"),
                RowDefinitions = new RowDefinitions(),
                Margin = new Thickness(10)
            };

            void AddRow(Control label, Control editor)
            {
                root.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                var rowIndex = root.RowDefinitions.Count - 1;

                label.VerticalAlignment = VerticalAlignment.Center;
                label.Margin = new Thickness(0, 0, 10, 0);
                Grid.SetRow(label, rowIndex);
                Grid.SetColumn(label, 0);

                editor.VerticalAlignment = VerticalAlignment.Center;
                editor.HorizontalAlignment = HorizontalAlignment.Stretch;
                Grid.SetRow(editor, rowIndex);
                Grid.SetColumn(editor, 1);

                root.Children.Add(label);
                root.Children.Add(editor);
            }

            // 字体族下拉框
            var fontFamilyLabel = new TextBlock { Text = "字体族:" };
            var fontFamilyComboBox = new ComboBox { MinWidth = 200 };
            var fontFamilyItems = new List<string> { "Arial", "Segoe UI", "Times New Roman", "Calibri", "Microsoft YaHei" };
            foreach (var item in fontFamilyItems)
            {
                fontFamilyComboBox.Items.Add(item);
            }
            // 设置默认选中项
            fontFamilyComboBox.SelectedItem = "Segoe UI";
            fontFamilyComboBox.Bind(ComboBox.SelectedItemProperty, new Binding(nameof(FontPropertyMapOprtCellParamViewModel.FontFamily)) { Mode = BindingMode.TwoWay });

            // 字体大小数字输入框
            var fontSizeLabel = new TextBlock { Text = "大小:" };
            var fontSizeNumericUpDown = new NumericUpDown { MinWidth = 200, Value = 14m };
            // 创建一个转换器，将double转换为decimal?
            var doubleToDecimalConverter = new DoubleToDecimalConverter();
            fontSizeNumericUpDown.Bind(NumericUpDown.ValueProperty, new Binding(nameof(FontPropertyMapOprtCellParamViewModel.FontSize)) 
            {
                Mode = BindingMode.TwoWay,
                Converter = doubleToDecimalConverter
            });

            // 字体粗细下拉框
            var fontWeightLabel = new TextBlock { Text = "粗细:" };
            var fontWeightComboBox = new ComboBox { MinWidth = 200 };
            var fontWeightItems = new List<string> { "Normal", "Bold" };
            foreach (var item in fontWeightItems)
            {
                fontWeightComboBox.Items.Add(item);
            }
            // 设置默认选中项
            fontWeightComboBox.SelectedItem = "Normal";
            // 创建一个转换器，将IsBold布尔值转换为字符串
            var boolToStringConverter = new BoolToStringConverter("Bold", "Normal");
            fontWeightComboBox.Bind(ComboBox.SelectedItemProperty, new Binding(nameof(FontPropertyMapOprtCellParamViewModel.IsBold)) 
            {
                Mode = BindingMode.TwoWay,
                Converter = boolToStringConverter
            });

            // 字体样式下拉框
            var fontStyleLabel = new TextBlock { Text = "样式:" };
            var fontStyleComboBox = new ComboBox { MinWidth = 200 };
            var fontStyleItems = new List<string> { "Normal", "Italic" };
            foreach (var item in fontStyleItems)
            {
                fontStyleComboBox.Items.Add(item);
            }
            // 设置默认选中项
            fontStyleComboBox.SelectedItem = "Normal";
            // 创建一个转换器，将IsItalic布尔值转换为字符串
            var italicToStringConverter = new BoolToStringConverter("Italic", "Normal");
            fontStyleComboBox.Bind(ComboBox.SelectedItemProperty, new Binding(nameof(FontPropertyMapOprtCellParamViewModel.IsItalic)) 
            {
                Mode = BindingMode.TwoWay,
                Converter = italicToStringConverter
            });

            AddRow(fontFamilyLabel, fontFamilyComboBox);
            AddRow(fontSizeLabel, fontSizeNumericUpDown);
            AddRow(fontWeightLabel, fontWeightComboBox);
            AddRow(fontStyleLabel, fontStyleComboBox);

            Content = root;
        }
    }

    /// <summary>
    /// 字体属性操作单元格参数配置视图
    /// 实现IMapOprtCellParamCfgView接口，提供参数配置功能
    /// </summary>
    public sealed class FontPropertyMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly FontPropertyMapOprtCellParamView _view;
        private readonly FontPropertyMapOprtCellParamViewModel _vm;

        /// <summary>
        /// 初始化字体属性操作单元格参数配置视图
        /// </summary>
        public FontPropertyMapOprtCellParamCfgView()
        {
            _vm = new FontPropertyMapOprtCellParamViewModel();
            _view = new FontPropertyMapOprtCellParamView { DataContext = _vm };
        }

        /// <summary>
        /// 获取视图对象
        /// </summary>
        public object View => _view;
        
        /// <summary>
        /// 设置配置数据
        /// </summary>
        /// <param name="data">配置数据</param>
        public void SetData(byte[] data) => _vm.FromBytes(data);
        
        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <returns>配置数据</returns>
        public byte[] GetData() => _vm.ToBytes();

        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }
}
