using System.Collections.ObjectModel;

namespace Griffins.CompUI.SL.InitCfgPage.Models
{
    /// <summary>
    /// 标准直接工作类子机械模组数据模型
    /// </summary>
    public class StandardDirectWorkCompUIModel
    {
        /// <summary>
        /// 扫码器
        /// </summary>
        public BarcodeScannerConfig BarcodeScanner { get; set; } = new();

        /// <summary>
        /// 测高
        /// </summary>
        public HeightMeasurementConfig HeightMeasurement { get; set; } = new();

        /// <summary>
        /// 顶升固定时间参数
        /// </summary>
        public LiftFixedTimeConfig LiftFixedTime { get; set; } = new();

        /// <summary>
        /// 温度控制
        /// </summary>
        public TemperatureControlConfig TemperatureControl { get; set; } = new();

        /// <summary>
        /// 工位自定义感应器列表
        /// </summary>
        public ObservableCollection<CustomStationSensor> CustomStationSensors { get; set; } = new();
    }

    /// <summary>
    /// 扫码器配置
    /// </summary>
    public class BarcodeScannerConfig
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 条码检验字符
        /// </summary>
        public string BarcodeCheckChar { get; set; } = string.Empty;

        /// <summary>
        /// 条码类型
        /// </summary>
        public BarcodeType BarcodeType { get; set; } = BarcodeType.Code39;

        /// <summary>
        /// 扫码器类型
        /// </summary>
        public string ScannerType { get; set; } = string.Empty;
    }

    /// <summary>
    /// 测高配置
    /// </summary>
    public class HeightMeasurementConfig
    {
        /// <summary>
        /// 测高延时
        /// </summary>
        public int DelayMs { get; set; }
    }

    /// <summary>
    /// 顶升固定时间配置
    /// </summary>
    public class LiftFixedTimeConfig
    {
        /// <summary>
        /// 固定时间
        /// </summary>
        public int FixedTimeMs { get; set; }

        /// <summary>
        /// 解除固定时间
        /// </summary>
        public int ReleaseFixedTimeMs { get; set; }
    }

    /// <summary>
    /// 温度控制配置
    /// </summary>
    public class TemperatureControlConfig
    {
        /// <summary>
        /// 是否检测开启
        /// </summary>
        public bool DetectEnabled { get; set; }

        /// <summary>
        /// 是否工作开启
        /// </summary>
        public bool WorkEnabled { get; set; }

        /// <summary>
        /// 是否预加热
        /// </summary>
        public bool PreheatEnabled { get; set; }
    }

    /// <summary>
    /// 工位自定义感应器对象
    /// </summary>
    public class CustomStationSensor : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        private bool _isSelected;
        /// <summary>
        /// 是否选中（用于列表勾选）
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 感应器id
        /// </summary>
        public string SensorId { get; set; }

        /// <summary>
        /// 运控卡id
        /// </summary>
        public string MotionCardId { get; set; }

        /// <summary>
        /// io通道
        /// </summary>
        public string IoChannel { get; set; }
    }
}
