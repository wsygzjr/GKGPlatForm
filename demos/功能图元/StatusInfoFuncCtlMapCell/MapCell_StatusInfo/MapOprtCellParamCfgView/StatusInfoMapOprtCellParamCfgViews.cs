using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using GF_Gereric;
using GKG.Map.StatusInfoFuncCtlMapCell;
using Griffins.Map.UI;
using ReactiveUI;
using System.Text;
using System.Text.Json;
using APrimitives = Avalonia.Controls.Primitives;

namespace GKG.Map.StatusInfoFuncCtlMapCell.MapCell_StatusInfo.MapOprtCellParamCfgView
{
    /// <summary>
    /// 状态信息操作单元格配置序列化器
    /// 提供序列化和反序列化功能
    /// </summary>
    internal static class StatusInfoOprtCellCfgSerializer
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

    /// <summary>
    /// 状态信息状态操作单元格参数视图模型
    /// 用于配置状态信息的状态参数
    /// </summary>
    public sealed class StatusInfoStateMapOprtCellParamViewModel : ReactiveObject
    {
        private bool _leftValveGlueMonitorState;
        public bool LeftValveGlueMonitorState
        {
            get => _leftValveGlueMonitorState;
            set => this.RaiseAndSetIfChanged(ref _leftValveGlueMonitorState, value);
        }

        private bool _rightValveGlueMonitorState;
        public bool RightValveGlueMonitorState
        {
            get => _rightValveGlueMonitorState;
            set => this.RaiseAndSetIfChanged(ref _rightValveGlueMonitorState, value);
        }

        private bool _rightPressureCyclesAlarmState;
        public bool RightPressureCyclesAlarmState
        {
            get => _rightPressureCyclesAlarmState;
            set => this.RaiseAndSetIfChanged(ref _rightPressureCyclesAlarmState, value);
        }

        /// <summary>
        /// 从字节数组加载配置
        /// </summary>
        /// <param name="data">配置数据</param>
        public void FromBytes(byte[] data)
        {
            var temp = StatusInfoOprtCellCfgSerializer.FromBytes<StatusInfoStateMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            LeftValveGlueMonitorState = temp.LeftValveGlueMonitorState;
            RightValveGlueMonitorState = temp.RightValveGlueMonitorState;
            RightPressureCyclesAlarmState = temp.RightPressureCyclesAlarmState;
        }

        /// <summary>
        /// 将配置转换为字节数组
        /// </summary>
        /// <returns>配置数据的字节数组</returns>
        public byte[] ToBytes() => StatusInfoOprtCellCfgSerializer.ToBytes(this);
    }

    /// <summary>
    /// 状态信息状态操作单元格参数视图
    /// 用于配置状态信息的状态参数的UI界面
    /// </summary>
    internal sealed class StatusInfoStateMapOprtCellParamView : UserControl
    {
        /// <summary>
        /// 初始化状态信息状态操作单元格参数视图
        /// </summary>
        public StatusInfoStateMapOprtCellParamView()
        {
            var root = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 8,
                Margin = new Thickness(10)
            };

            var left = new CheckBox { Content = ResourceA.LeftValveGlueMonitorState };
            left.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(nameof(StatusInfoStateMapOprtCellParamViewModel.LeftValveGlueMonitorState)) { Mode = BindingMode.TwoWay });

            var right = new CheckBox { Content = ResourceA.RightValveGlueMonitorState };
            right.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(nameof(StatusInfoStateMapOprtCellParamViewModel.RightValveGlueMonitorState)) { Mode = BindingMode.TwoWay });

            var cycles = new CheckBox { Content = ResourceA.RightPressureCyclesAlarmState };
            cycles.Bind(APrimitives.ToggleButton.IsCheckedProperty, new Binding(nameof(StatusInfoStateMapOprtCellParamViewModel.RightPressureCyclesAlarmState)) { Mode = BindingMode.TwoWay });

            root.Children.Add(left);
            root.Children.Add(right);
            root.Children.Add(cycles);

            Content = root;
        }
    }

    /// <summary>
    /// 状态信息状态操作单元格参数配置视图
    /// 实现IMapOprtCellParamCfgView接口，提供参数配置功能
    /// </summary>
    public sealed class StatusInfoStateMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly StatusInfoStateMapOprtCellParamView _view;
        private readonly StatusInfoStateMapOprtCellParamViewModel _vm;

        /// <summary>
        /// 初始化状态信息状态操作单元格参数配置视图
        /// </summary>
        public StatusInfoStateMapOprtCellParamCfgView()
        {
            _vm = new StatusInfoStateMapOprtCellParamViewModel();
            _view = new StatusInfoStateMapOprtCellParamView { DataContext = _vm };
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
    /// 状态信息时间操作单元格参数视图模型
    /// 用于配置状态信息的时间参数
    /// </summary>
    public sealed class StatusInfoTimeMapOprtCellParamViewModel : ReactiveObject
    {
        private string _aWaitingAddGlueTime = "00:00";
        public string AWaitingAddGlueTime
        {
            get => _aWaitingAddGlueTime;
            set => this.RaiseAndSetIfChanged(ref _aWaitingAddGlueTime, string.IsNullOrWhiteSpace(value) ? "00:00" : value);
        }

        private string _bWaitingAddGlueTime = "00:00";
        public string BWaitingAddGlueTime
        {
            get => _bWaitingAddGlueTime;
            set => this.RaiseAndSetIfChanged(ref _bWaitingAddGlueTime, string.IsNullOrWhiteSpace(value) ? "00:00" : value);
        }

        public void FromBytes(byte[] data)
        {
            var temp = StatusInfoOprtCellCfgSerializer.FromBytes<StatusInfoTimeMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            AWaitingAddGlueTime = temp.AWaitingAddGlueTime;
            BWaitingAddGlueTime = temp.BWaitingAddGlueTime;
        }

        public byte[] ToBytes() => StatusInfoOprtCellCfgSerializer.ToBytes(this);
    }

    internal sealed class StatusInfoTimeMapOprtCellParamView : UserControl
    {
        public StatusInfoTimeMapOprtCellParamView()
        {
            var root = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 8,
                Margin = new Thickness(10)
            };

            var aTime = new TextBox { Width = 160 };
            aTime.Bind(TextBox.TextProperty, new Binding(nameof(StatusInfoTimeMapOprtCellParamViewModel.AWaitingAddGlueTime)) { Mode = BindingMode.TwoWay });

            var bTime = new TextBox { Width = 160 };
            bTime.Bind(TextBox.TextProperty, new Binding(nameof(StatusInfoTimeMapOprtCellParamViewModel.BWaitingAddGlueTime)) { Mode = BindingMode.TwoWay });

            root.Children.Add(new TextBlock { Text = ResourceA.AWaitingAddGlueTime });
            root.Children.Add(aTime);
            root.Children.Add(new TextBlock { Text = ResourceA.BWaitingAddGlueTime });
            root.Children.Add(bTime);

            Content = root;
        }
    }

    public sealed class StatusInfoTimeMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly StatusInfoTimeMapOprtCellParamView _view;
        private readonly StatusInfoTimeMapOprtCellParamViewModel _vm;

        public StatusInfoTimeMapOprtCellParamCfgView()
        {
            _vm = new StatusInfoTimeMapOprtCellParamViewModel();
            _view = new StatusInfoTimeMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();

        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }
}
