using Avalonia.Controls;
using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// 测高串口调试-视图模型
    /// </summary>
    public class HeightSerialPortViewModel : ReactiveObject
    {
        #region 私有字段

        private Control? _viewReference;

        #endregion

        #region UI属性

        /// <summary>
        /// 串口号-下拉框视图模型
        /// </summary>
        public ComboxViewModel PortNameViewModel { get; }

        /// <summary>
        /// 波特率-下拉框视图模型
        /// </summary>
        public ComboxViewModel BaudRateViewModel { get; }

        /// <summary>
        /// 数据位-下拉框视图模型
        /// </summary>
        public ComboxViewModel DataBitsViewModel { get; }

        /// <summary>
        /// 停止位-下拉框视图模型
        /// </summary>
        public ComboxViewModel StopBitsViewModel { get; }

        /// <summary>
        /// 奇偶校验类型-下拉框视图模型
        /// </summary>
        public ComboxViewModel ParityViewModel { get; }

        /// <summary>
        /// 是否启用CRC16-开关按钮视图模型
        /// </summary>
        public ToggleSwitchViewModel EnableCRC16ViewModel { get; }

        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 选中的串口号
        /// </summary>
        public string SelectedPortName
        {
            get => (string)((PortNameViewModel.SelectedItem as ComBoxItem)?.Value ?? "Com1");
            set
            {
                if (PortNameViewModel.ItemsSource != null)
                {
                    var targetItem = PortNameViewModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (string)o.Value == value);
                    if (targetItem != null)
                        PortNameViewModel.SelectedItem = targetItem;

                    this.RaisePropertyChanged(nameof(SelectedPortName));
                }
            }
        }

        /// <summary>
        /// 选中的波特率
        /// </summary>
        public SerialPortCfgTypes.BaudRate SelectedBaudRate
        {
            get => (SerialPortCfgTypes.BaudRate)((BaudRateViewModel.SelectedItem as ComBoxItem)?.Value ?? SerialPortCfgTypes.BaudRate.BaudRate9600);
            set
            {
                if (BaudRateViewModel.ItemsSource != null)
                {
                    var targetItem = BaudRateViewModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (SerialPortCfgTypes.BaudRate)o.Value == value);
                    if (targetItem != null)
                        BaudRateViewModel.SelectedItem = targetItem;

                    this.RaisePropertyChanged(nameof(SelectedBaudRate));
                }
            }
        }

        /// <summary>
        /// 选中的数据位
        /// </summary>
        public SerialPortCfgTypes.DataBits SelectedDataBits
        {
            get => (SerialPortCfgTypes.DataBits)((DataBitsViewModel.SelectedItem as ComBoxItem)?.Value ?? SerialPortCfgTypes.DataBits.Eight);
            set
            {
                if (DataBitsViewModel.ItemsSource != null)
                {
                    var targetItem = DataBitsViewModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (SerialPortCfgTypes.DataBits)o.Value == value);
                    if (targetItem != null)
                        DataBitsViewModel.SelectedItem = targetItem;

                    this.RaisePropertyChanged(nameof(SelectedDataBits));
                }
            }
        }

        /// <summary>
        /// 选中的停止位
        /// </summary>
        public SerialPortCfgTypes.StopBits SelectedStopBits
        {
            get => (SerialPortCfgTypes.StopBits)((StopBitsViewModel.SelectedItem as ComBoxItem)?.Value ?? SerialPortCfgTypes.StopBits.One);
            set
            {
                if (StopBitsViewModel.ItemsSource != null)
                {
                    var targetItem = StopBitsViewModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (SerialPortCfgTypes.StopBits)o.Value == value);
                    if (targetItem != null)
                        StopBitsViewModel.SelectedItem = targetItem;

                    this.RaisePropertyChanged(nameof(SelectedStopBits));
                }
            }
        }

        /// <summary>
        /// 选中的串口校验类型
        /// </summary>
        public SerialPortCfgTypes.Parity SelectedParity
        {
            get => (SerialPortCfgTypes.Parity)((ParityViewModel.SelectedItem as ComBoxItem)?.Value ?? SerialPortCfgTypes.Parity.None);
            set
            {
                if (ParityViewModel.ItemsSource != null)
                {
                    var targetItem = ParityViewModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (SerialPortCfgTypes.Parity)o.Value == value);
                    if (targetItem != null)
                        ParityViewModel.SelectedItem = targetItem;

                    this.RaisePropertyChanged(nameof(SelectedParity));
                }
            }
        }

        /// <summary>
        /// 是否启用CRC16
        /// </summary>
        public bool EnableCRC16
        {
            get => EnableCRC16ViewModel.IsChecked;
            set
            {
                EnableCRC16ViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(EnableCRC16));
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public HeightSerialPortViewModel()
        {
            #region 初始化UI属性

            // 初始化开关按钮
            EnableCRC16ViewModel = new();

            PortNameViewModel = new();
            // 初始化选泵数据
            PortNameViewModel.ItemsSource = new List<ComBoxItem>
            {
                new ComBoxItem { Value = "COM1", DisplayName = "COM1" },
                new ComBoxItem { Value = "COM2", DisplayName = "COM2" },
                new ComBoxItem { Value = "COM3", DisplayName = "COM3" },
            };
            PortNameViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            // 初始化枚举数据
            BaudRateViewModel = new();
            DataBitsViewModel = new();
            StopBitsViewModel = new();
            ParityViewModel = new();

            var baudRateDisplayNames = new Dictionary<SerialPortCfgTypes.BaudRate, string>
            {
                { SerialPortCfgTypes.BaudRate.BaudRate4800, "4800" },
                { SerialPortCfgTypes.BaudRate.BaudRate9600, "9600" },
                { SerialPortCfgTypes.BaudRate.BaudRate19200, "19200" },
                { SerialPortCfgTypes.BaudRate.BaudRate38400, "38400" },
                { SerialPortCfgTypes.BaudRate.BaudRate57600, "57600" },
                { SerialPortCfgTypes.BaudRate.BaudRate115200, "115200" }
            };
            BaudRateViewModel.ItemsSource = EnumExtensions.ToEnumItems(baudRateDisplayNames);
            BaudRateViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            var dataBitsDisplayNames = new Dictionary<SerialPortCfgTypes.DataBits, string>
            {
                { SerialPortCfgTypes.DataBits.Five, "5" },
                { SerialPortCfgTypes.DataBits.Six, "6" },
                { SerialPortCfgTypes.DataBits.Seven, "7" },
                { SerialPortCfgTypes.DataBits.Eight, "8" }
            };
            DataBitsViewModel.ItemsSource = EnumExtensions.ToEnumItems(dataBitsDisplayNames);
            DataBitsViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            var stopBitsDisplayNames = new Dictionary<SerialPortCfgTypes.StopBits, string>
            {
                { SerialPortCfgTypes.StopBits.None, "None" },
                { SerialPortCfgTypes.StopBits.One, "1" },
                { SerialPortCfgTypes.StopBits.Two, "2" },
                { SerialPortCfgTypes.StopBits.OnePointFive, "1.5" }
            };
            StopBitsViewModel.ItemsSource = EnumExtensions.ToEnumItems(stopBitsDisplayNames);
            StopBitsViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            var parityDisplayNames = new Dictionary<SerialPortCfgTypes.Parity, string>
            {
                { SerialPortCfgTypes.Parity.None, "None" },
                { SerialPortCfgTypes.Parity.Odd, "Odd" },
                { SerialPortCfgTypes.Parity.Even, "Even" },
                { SerialPortCfgTypes.Parity.Mark, "Mark" },
                { SerialPortCfgTypes.Parity.Space, "Space" }
            };
            ParityViewModel.ItemsSource = EnumExtensions.ToEnumItems(parityDisplayNames);
            ParityViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            #endregion

            subscribeValueChanges();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 从配置模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(HeightSerialPortCfg model)
        {
            if (model == null) return;
            SelectedPortName = model.PortName;
            SelectedBaudRate = model.BaudRate;
            SelectedDataBits = model.DataBits;
            SelectedStopBits = model.StopBits;
            SelectedParity = model.Parity;
            EnableCRC16 = model.EnableCRC16;
        }

        /// <summary>
        /// 从ViewModel复制数据到配置模型
        /// </summary>
        public void CopyTo(HeightSerialPortCfg model)
        {
            if (model == null) return;
            model.PortName = SelectedPortName;
            model.BaudRate = SelectedBaudRate;
            model.DataBits = SelectedDataBits;
            model.StopBits = SelectedStopBits;
            model.Parity = SelectedParity;
            model.EnableCRC16 = EnableCRC16;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 订阅值改变
        /// </summary>
        private void subscribeValueChanges()
        {
            PortNameViewModel.ValueChanged += onValueChanged;
            BaudRateViewModel.ValueChanged += onValueChanged;
            DataBitsViewModel.ValueChanged += onValueChanged;
            StopBitsViewModel.ValueChanged += onValueChanged;
            ParityViewModel.ValueChanged += onValueChanged;
            EnableCRC16ViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值改变
        /// </summary>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        #endregion
    }
}
