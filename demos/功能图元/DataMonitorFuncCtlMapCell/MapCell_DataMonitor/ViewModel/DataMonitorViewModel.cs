using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.Objects;
using Avalonia;
using Avalonia.Media;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.ComponentModel;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.ViewModels;

namespace GKG.Map.DataMonitorFuncCtlMapCell.ViewModel
{
    /// <summary>
    /// 数据监控功能图元视图模型类
    /// 负责管理数据监控图元的数据和业务逻辑，包括字体、颜色、供阀气压、供胶气压、喷嘴加热、监控信息等属性
    /// </summary>
    public class DataMonitorViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段
        private readonly DataMonitorPropertyModelEdit _model;
        #endregion

        #region 顶部设备状态栏
        // 说明：DataMonitor的属性面板仍由DataMonitorPropertyModelEdit提供权威数据(DeviceStatusCommonInfo)。
        // 这里额外维护三个DeviceStatusViewModel，仅用于驱动DeviceStatusView控件显示。

        public DeviceStatusViewModel SupplyValvePressureDeviceStatusVm { get; } = new();
        public DeviceStatusViewModel SupplyGluePressureDeviceStatusVm { get; } = new();
        public DeviceStatusViewModel NozzleHeatingDeviceStatusVm { get; } = new();

        private DeviceStatusCommonInfo _prevSupplyValvePressureInfo;
        private DeviceStatusCommonInfo _prevSupplyGluePressureInfo;
        private DeviceStatusCommonInfo _prevNozzleHeatingInfo;
        #endregion

        #region 响应式属性
        /// <summary>
        /// 字体信息
        /// </summary>
        private FontInfo _textFont;
        public FontInfo TextFont
        {
            get { return _textFont; }
            set
            {
                this.RaiseAndSetIfChanged(ref _textFont, value);
            }
        }

        public void SetTextFontFromModel(FontInfo value)
        {
            this.RaiseAndSetIfChanged(ref _textFont, value);
        }

        /// <summary>
        /// 文本颜色
        /// </summary>
        private Color _textColor;
        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                this.RaiseAndSetIfChanged(ref _textColor, value);
            }
        }

        /// <summary>
        /// 背景颜色
        /// </summary>
        private Color _backColor;
        public Color BackColor
        {
            get { return _backColor; }
            set
            {
                this.RaiseAndSetIfChanged(ref _backColor, value);
            }
        }

        #region 供阀气压栏属性
        /// <summary>
        /// 供阀气压
        /// </summary>
        private string _supplyValvePressure = "供阀气压";
        public string SupplyValvePressure
        {
            get { return _supplyValvePressure; }
            set { this.RaiseAndSetIfChanged(ref _supplyValvePressure, value); }
        }

        /// <summary>
        /// 供阀气压值
        /// </summary>
        private string _supplyValvePressureValue;
        public string SupplyValvePressureValue
        {
            get
            {
                // 确保返回3位小数格式
                if (string.IsNullOrEmpty(_supplyValvePressureValue))
                    return "0.000";

                if (decimal.TryParse(_supplyValvePressureValue, out decimal value))
                    return value.ToString("F3");

                return _supplyValvePressureValue;
            }
            set { this.RaiseAndSetIfChanged(ref _supplyValvePressureValue, value); }
        }

        /// <summary>
        /// 供阀气压状态（true表示开启，false表示关闭）
        /// </summary>
        private bool _supplyValvePressureStatus;
        public bool SupplyValvePressureStatus
        {
            get { return _supplyValvePressureStatus; }
            set { this.RaiseAndSetIfChanged(ref _supplyValvePressureStatus, value); }
        }

        /// <summary>
        /// 供阀气压设备状态
        /// </summary>
        private DeviceStatusCommonInfo _supplyValvePressureDeviceStatus;
        public DeviceStatusCommonInfo SupplyValvePressureDeviceStatus
        {
            get { return _supplyValvePressureDeviceStatus; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _supplyValvePressureDeviceStatus, value) != null)
                {
                    if (_model != null && !ReferenceEquals(_model.SupplyValvePressureDeviceStatus, value))
                        _model.SupplyValvePressureDeviceStatus = value;

                    AttachDeviceStatusVmHandlers();
                    UpdateDeviceStatusVmFromInfo(SupplyValvePressureDeviceStatusVm, _supplyValvePressureDeviceStatus);
                }
            }
        }
        #endregion

        #region 供胶气压栏属性
        /// <summary>
        /// 供胶气压
        /// </summary>
        private string _supplyGluePressure = "供胶气压";
        public string SupplyGluePressure
        {
            get { return _supplyGluePressure; }
            set { this.RaiseAndSetIfChanged(ref _supplyGluePressure, value); }
        }

        /// <summary>
        /// 供胶气压值
        /// </summary>
        private string _supplyGluePressureValue;
        public string SupplyGluePressureValue
        {
            get
            {
                // 确保返回3位小数格式
                if (string.IsNullOrEmpty(_supplyGluePressureValue))
                    return "0.000";

                if (decimal.TryParse(_supplyGluePressureValue, out decimal value))
                    return value.ToString("F3");

                return _supplyGluePressureValue;
            }
            set { this.RaiseAndSetIfChanged(ref _supplyGluePressureValue, value); }
        }

        /// <summary>
        /// 供胶气压状态（true表示开启，false表示关闭）
        /// </summary>
        private bool _supplyGluePressureStatus;
        public bool SupplyGluePressureStatus
        {
            get { return _supplyGluePressureStatus; }
            set { this.RaiseAndSetIfChanged(ref _supplyGluePressureStatus, value); }
        }

        /// <summary>
        /// 供胶气压设备状态
        /// </summary>
        private DeviceStatusCommonInfo _supplyGluePressureDeviceStatus;
        public DeviceStatusCommonInfo SupplyGluePressureDeviceStatus
        {
            get { return _supplyGluePressureDeviceStatus; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _supplyGluePressureDeviceStatus, value) != null)
                {
                    if (_model != null && !ReferenceEquals(_model.SupplyGluePressureDeviceStatus, value))
                        _model.SupplyGluePressureDeviceStatus = value;

                    AttachDeviceStatusVmHandlers();
                    UpdateDeviceStatusVmFromInfo(SupplyGluePressureDeviceStatusVm, _supplyGluePressureDeviceStatus);
                }
            }
        }
        #endregion

        #region 喷嘴加热栏属性
        /// <summary>
        /// 喷嘴加热
        /// </summary>
        private string _nozzleHeating = "喷嘴加热";
        public string NozzleHeating
        {
            get { return _nozzleHeating; }
            set { this.RaiseAndSetIfChanged(ref _nozzleHeating, value); }
        }

        /// <summary>
        /// 喷嘴加热值
        /// </summary>
        private string _nozzleHeatingValue;
        public string NozzleHeatingValue
        {
            get
            {
                // 确保返回1位小数格式
                if (string.IsNullOrEmpty(_nozzleHeatingValue))
                    return "0.0";

                if (decimal.TryParse(_nozzleHeatingValue, out decimal value))
                    return value.ToString("F1");

                return _nozzleHeatingValue;
            }
            set { this.RaiseAndSetIfChanged(ref _nozzleHeatingValue, value); }
        }

        /// <summary>
        /// 喷嘴加热状态（true表示开启，false表示关闭）
        /// </summary>
        private bool _nozzleHeatingStatus;
        public bool NozzleHeatingStatus
        {
            get { return _nozzleHeatingStatus; }
            set { this.RaiseAndSetIfChanged(ref _nozzleHeatingStatus, value); }
        }

        /// <summary>
        /// 喷嘴加热设备状态
        /// </summary>
        private DeviceStatusCommonInfo _nozzleHeatingDeviceStatus;
        public DeviceStatusCommonInfo NozzleHeatingDeviceStatus
        {
            get { return _nozzleHeatingDeviceStatus; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _nozzleHeatingDeviceStatus, value) != null)
                {
                    if (_model != null && !ReferenceEquals(_model.NozzleHeatingDeviceStatus, value))
                        _model.NozzleHeatingDeviceStatus = value;

                    AttachDeviceStatusVmHandlers();
                    UpdateDeviceStatusVmFromInfo(NozzleHeatingDeviceStatusVm, _nozzleHeatingDeviceStatus);
                }
            }
        }
        #endregion

        #region 监控信息栏扩展属性
        /// <summary>
        /// 安全门状态
        /// </summary>
        private bool _safetyDoorStatus = true;
        public bool SafetyDoorStatus
        {
            get { return _safetyDoorStatus; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _safetyDoorStatus, value) != value)
                {
                    // 同步到Model（避免循环，只在值真正改变时同步）
                    if (_model != null && _model.SafetyDoorStatus != value)
                    {
                        _model.SafetyDoorStatus = value;
                    }
                }
            }
        }
        /// <summary>
        /// 总气压状态
        /// </summary>
        private bool _totalPressureStatus = true;
        public bool TotalPressureStatus
        {
            get { return _totalPressureStatus; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _totalPressureStatus, value) != value)
                {
                    // 同步到Model
                    if (_model != null && _model.TotalPressureStatus != value)
                        _model.TotalPressureStatus = value;
                }
            }
        }

        /// <summary>
        /// 清洁布状态
        /// </summary>
        private bool _cleaningClothStatus = true;
        public bool CleaningClothStatus
        {
            get { return _cleaningClothStatus; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _cleaningClothStatus, value) != value)
                {
                    // 同步到Model
                    if (_model != null && _model.CleaningClothStatus != value)
                        _model.CleaningClothStatus = value;
                }
            }
        }

        private bool _isDualValve;
        public bool IsDualValve
        {
            get { return _isDualValve; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isDualValve, value) != value)
                {
                    if (_model != null && _model.IsDualValve != value)
                        _model.IsDualValve = value;
                }
            }
        }

        /// <summary>
        /// 胶水余量（mg）
        /// </summary>
        private string _glueRemaining = "50%";
        public string GlueRemaining
        {
            get { return _glueRemaining; }
            set
            {
                this.RaiseAndSetIfChanged(ref _glueRemaining, value);
                // 同步到Model（避免循环，只在值真正改变时同步）
                if (_model != null && _model.GlueRemaining != value)
                {
                    _model.GlueRemaining = value;
                }
            }
        }

        private string _leftGlueRemaining = "50%";
        public string LeftGlueRemaining
        {
            get { return _leftGlueRemaining; }
            set
            {
                this.RaiseAndSetIfChanged(ref _leftGlueRemaining, value);
                if (_model != null && _model.LeftGlueRemaining != value)
                {
                    _model.LeftGlueRemaining = value;
                }
            }
        }

        /// <summary>
        /// 校正
        /// </summary>
        private string _calibration = "校正";
        public string Calibration
        {
            get { return _calibration; }
            set
            {
                this.RaiseAndSetIfChanged(ref _calibration, value);
                // 同步到Model
                if (_model != null && _model.Calibration != value)
                    _model.Calibration = value;
            }
        }

        private string _leftCalibration = "校正";
        public string LeftCalibration
        {
            get { return _leftCalibration; }
            set
            {
                this.RaiseAndSetIfChanged(ref _leftCalibration, value);
                if (_model != null && _model.LeftCalibration != value)
                    _model.LeftCalibration = value;
            }
        }

        /// <summary>
        /// 阀体值
        /// </summary>
        private string _valveBodyValue = "正常";
        public string ValveBodyValue
        {
            get { return _valveBodyValue; }
            set
            {
                this.RaiseAndSetIfChanged(ref _valveBodyValue, value);
                // 同步到Model
                if (_model != null && _model.ValveBodyValue != value)
                    _model.ValveBodyValue = value;
            }
        }

        private string _leftValveBodyValue = "正常";
        public string LeftValveBodyValue
        {
            get { return _leftValveBodyValue; }
            set
            {
                this.RaiseAndSetIfChanged(ref _leftValveBodyValue, value);
                if (_model != null && _model.LeftValveBodyValue != value)
                    _model.LeftValveBodyValue = value;
            }
        }

        /// <summary>
        /// 阀体自定义图标
        /// </summary>
        private BitmapData _valveBodyIcon;
        public BitmapData ValveBodyIcon
        {
            get { return _valveBodyIcon; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _valveBodyIcon, value) != null)
                {
                    // 同步到Model
                    if (_model != null && _model.ValveBodyIcon != value)
                        _model.ValveBodyIcon = value;
                }
            }
        }

        private BitmapData _leftValveBodyIcon;
        public BitmapData LeftValveBodyIcon
        {
            get { return _leftValveBodyIcon; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _leftValveBodyIcon, value) != null)
                {
                    if (_model != null && _model.LeftValveBodyIcon != value)
                        _model.LeftValveBodyIcon = value;
                }
            }
        }

        /// <summary>
        /// 密封圈值
        /// </summary>
        private string _sealingRingValue = "正常";
        public string SealingRingValue
        {
            get { return _sealingRingValue; }
            set
            {
                this.RaiseAndSetIfChanged(ref _sealingRingValue, value);
                // 同步到Model
                if (_model != null && _model.SealingRingValue != value)
                    _model.SealingRingValue = value;
            }
        }

        private string _leftSealingRingValue = "正常";
        public string LeftSealingRingValue
        {
            get { return _leftSealingRingValue; }
            set
            {
                this.RaiseAndSetIfChanged(ref _leftSealingRingValue, value);
                if (_model != null && _model.LeftSealingRingValue != value)
                    _model.LeftSealingRingValue = value;
            }
        }

        /// <summary>
        /// 密封圈自定义图标
        /// </summary>
        private BitmapData _sealingRingIcon;
        public BitmapData SealingRingIcon
        {
            get { return _sealingRingIcon; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _sealingRingIcon, value) != null)
                {
                    // 同步到Model
                    if (_model != null && _model.SealingRingIcon != value)
                        _model.SealingRingIcon = value;
                }
            }
        }

        private BitmapData _leftSealingRingIcon;
        public BitmapData LeftSealingRingIcon
        {
            get { return _leftSealingRingIcon; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _leftSealingRingIcon, value) != null)
                {
                    if (_model != null && _model.LeftSealingRingIcon != value)
                        _model.LeftSealingRingIcon = value;
                }
            }
        }

        /// <summary>
        /// 状态正常时显示的图标（Green）
        /// </summary>
        private Bitmap _statusOkImage;
        public Bitmap StatusOkImage
        {
            get { return _statusOkImage; }
            set { this.RaiseAndSetIfChanged(ref _statusOkImage, value); }
        }

        /// <summary>
        /// 状态为false时显示的图标（Gray）
        /// </summary>
        private Bitmap _statusNgImage;
        public Bitmap StatusNgImage
        {
            get { return _statusNgImage; }
            set { this.RaiseAndSetIfChanged(ref _statusNgImage, value); }
        }
        #endregion

        #endregion

        public ReactiveCommand<Point, Unit> PointerPressedCommand { get; }

        private Action _clickAction;

        #region 构造函数
        /// <summary>
        /// 初始化数据监控视图模型
        /// </summary>
        /// <param name="model">属性模型</param>
        public DataMonitorViewModel(DataMonitorPropertyModelEdit model, Action clickAction)
        {
            // ViewModel持有PropertyModelEdit（_model）作为权威数据源：
            // - 初始化时从_model读取一次，填充到VM属性
            // - VM属性在set时会反写到_model（双向同步）
            _model = model ?? throw new ArgumentNullException(nameof(model));

            // 初始化图片
            StatusOkImage = TryLoadBitmap("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Green.png");
            StatusNgImage = TryLoadBitmap("avares://Griffins.Map.DataMonitorFuncCtlMapCell/Assets/Images/Gray.png");

            // 初始化属性值
            InitializeProperties();

            // 订阅模型属性变更
            SubscribeToModelChanges();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 将设备状态信息DeviceStatusCommonInfo同步到用于显示的DeviceStatusViewModel
        /// </summary>
        private static void UpdateDeviceStatusVmFromInfo(DeviceStatusViewModel vm, DeviceStatusCommonInfo info)
        {
            if (vm == null || info == null) return;
            vm.ImageSources = info.ImageSources ?? new System.Collections.Generic.List<BitmapData>();
            vm.CurrentIndex = info.CurrentIndex;
            vm.StatusName = info.StatusName ?? "";
            vm.DeviceStatusValue = info.DeviceStatusValue ?? "";
            vm.DeviceStatusUnit = info.DeviceStatusUnit ?? "";
        }

        /// <summary>
        /// 当三个 DeviceStatusCommonInfo 被替换时，重新挂接它们的变更事件，保证UI能刷新。
        /// </summary>
        private void AttachDeviceStatusVmHandlers()
        {
            // DeviceStatusCommonInfo 是可变对象：当被整体替换时，需要重新挂接事件，确保内部字段变化能驱动UI刷新。
            void Detach(DeviceStatusCommonInfo oldInfo, PropertyChangedEventHandler handler)
            {
                if (oldInfo == null) return;
                oldInfo.PropertyChanged -= handler;
            }

            void Attach(DeviceStatusCommonInfo newInfo, PropertyChangedEventHandler handler)
            {
                if (newInfo == null) return;
                newInfo.PropertyChanged += handler;
            }

            if (!ReferenceEquals(_prevSupplyValvePressureInfo, _supplyValvePressureDeviceStatus))
            {
                Detach(_prevSupplyValvePressureInfo, SupplyValvePressureInfo_PropertyChanged);
                _prevSupplyValvePressureInfo = _supplyValvePressureDeviceStatus;
                Attach(_prevSupplyValvePressureInfo, SupplyValvePressureInfo_PropertyChanged);
            }

            if (!ReferenceEquals(_prevSupplyGluePressureInfo, _supplyGluePressureDeviceStatus))
            {
                Detach(_prevSupplyGluePressureInfo, SupplyGluePressureInfo_PropertyChanged);
                _prevSupplyGluePressureInfo = _supplyGluePressureDeviceStatus;
                Attach(_prevSupplyGluePressureInfo, SupplyGluePressureInfo_PropertyChanged);
            }

            if (!ReferenceEquals(_prevNozzleHeatingInfo, _nozzleHeatingDeviceStatus))
            {
                Detach(_prevNozzleHeatingInfo, NozzleHeatingInfo_PropertyChanged);
                _prevNozzleHeatingInfo = _nozzleHeatingDeviceStatus;
                Attach(_prevNozzleHeatingInfo, NozzleHeatingInfo_PropertyChanged);
            }
        }

        private void SupplyValvePressureInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateDeviceStatusVmFromInfo(SupplyValvePressureDeviceStatusVm, _supplyValvePressureDeviceStatus);
        }

        private void SupplyGluePressureInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateDeviceStatusVmFromInfo(SupplyGluePressureDeviceStatusVm, _supplyGluePressureDeviceStatus);
        }

        private void NozzleHeatingInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateDeviceStatusVmFromInfo(NozzleHeatingDeviceStatusVm, _nozzleHeatingDeviceStatus);
        }

        /// <summary>
        /// 初始化属性值
        /// </summary>
        private void InitializeProperties()
        {
            // 将模型中的初始值同步到VM字段，确保View第一次显示即为正确状态。
            // 注意：这里是“模型 -> VM”的一次性初始化。
            TextFont = _model.TextFont ?? FontInfo.DefaultFont;
            TextColor = _model.TextColor;
            BackColor = _model.BackColor;

            // 供阀气压设备状态栏
            SupplyValvePressureDeviceStatus = _model.SupplyValvePressureDeviceStatus;

            // 供胶气压设备状态栏
            SupplyGluePressureDeviceStatus = _model.SupplyGluePressureDeviceStatus;

            // 喷嘴加热设备状态栏
            NozzleHeatingDeviceStatus = _model.NozzleHeatingDeviceStatus;

            // 监控信息栏扩展属性
            SafetyDoorStatus = _model.SafetyDoorStatus;
            TotalPressureStatus = _model.TotalPressureStatus;
            CleaningClothStatus = _model.CleaningClothStatus;
            IsDualValve = _model.IsDualValve;
            GlueRemaining = _model.GlueRemaining ?? "50%";
            LeftGlueRemaining = _model.LeftGlueRemaining ?? "50%";
            Calibration = _model.Calibration ?? "校正";
            LeftCalibration = _model.LeftCalibration ?? "校正";
            ValveBodyValue = _model.ValveBodyValue ?? "正常";
            SealingRingValue = _model.SealingRingValue ?? "正常";
            LeftValveBodyValue = _model.LeftValveBodyValue ?? "正常";
            LeftSealingRingValue = _model.LeftSealingRingValue ?? "正常";

            // 设置自定义图标
            ValveBodyIcon = _model.ValveBodyIcon;
            SealingRingIcon = _model.SealingRingIcon;
            LeftValveBodyIcon = _model.LeftValveBodyIcon;
            LeftSealingRingIcon = _model.LeftSealingRingIcon;
        }

        /// <summary>
        /// 订阅模型属性变更
        /// </summary>
        private void SubscribeToModelChanges()
        {
            // 监听模型变更（通常来自属性面板、点位订阅、跨实例同步等），并更新VM属性驱动UI刷新。
            _model.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_model.TextFont):
                        TextFont = _model.TextFont;
                        break;
                    case nameof(_model.TextColor):
                        TextColor = _model.TextColor;
                        break;
                    case nameof(_model.BackColor):
                        BackColor = _model.BackColor;
                        break;
                    case nameof(_model.SupplyValvePressureDeviceStatus):
                        SupplyValvePressureDeviceStatus = _model.SupplyValvePressureDeviceStatus;
                        break;
                    case nameof(_model.SupplyGluePressureDeviceStatus):
                        SupplyGluePressureDeviceStatus = _model.SupplyGluePressureDeviceStatus;
                        break;
                    case nameof(_model.NozzleHeatingDeviceStatus):
                        NozzleHeatingDeviceStatus = _model.NozzleHeatingDeviceStatus;
                        break;
                    case nameof(_model.SafetyDoorStatus):
                        SafetyDoorStatus = _model.SafetyDoorStatus;
                        break;
                    case nameof(_model.TotalPressureStatus):
                        TotalPressureStatus = _model.TotalPressureStatus;
                        break;
                    case nameof(_model.CleaningClothStatus):
                        CleaningClothStatus = _model.CleaningClothStatus;
                        break;
                    case nameof(_model.IsDualValve):
                        IsDualValve = _model.IsDualValve;
                        break;
                    case nameof(_model.GlueRemaining):
                        GlueRemaining = _model.GlueRemaining;
                        break;
                    case nameof(_model.LeftGlueRemaining):
                        LeftGlueRemaining = _model.LeftGlueRemaining;
                        break;
                    case nameof(_model.Calibration):
                        Calibration = _model.Calibration;
                        break;
                    case nameof(_model.LeftCalibration):
                        LeftCalibration = _model.LeftCalibration;
                        break;
                    case nameof(_model.ValveBodyValue):
                        ValveBodyValue = _model.ValveBodyValue;
                        break;
                    case nameof(_model.ValveBodyIcon):
                        ValveBodyIcon = _model.ValveBodyIcon;
                        break;
                    case nameof(_model.LeftValveBodyValue):
                        LeftValveBodyValue = _model.LeftValveBodyValue;
                        break;
                    case nameof(_model.LeftValveBodyIcon):
                        LeftValveBodyIcon = _model.LeftValveBodyIcon;
                        break;
                    case nameof(_model.SealingRingValue):
                        SealingRingValue = _model.SealingRingValue;
                        break;
                    case nameof(_model.SealingRingIcon):
                        SealingRingIcon = _model.SealingRingIcon;
                        break;
                    case nameof(_model.LeftSealingRingValue):
                        LeftSealingRingValue = _model.LeftSealingRingValue;
                        break;
                    case nameof(_model.LeftSealingRingIcon):
                        LeftSealingRingIcon = _model.LeftSealingRingIcon;
                        break;
                }
            };
        }

        /// <summary>
        /// 从Avalonia资源(avares)加载图片；失败返回null。
        /// </summary>
        private static Bitmap TryLoadBitmap(string uriText)
        {
            // 从Avalonia资源中加载图片（用于状态图标等）。
            // 加载失败时返回null，由上层决定如何降级显示。
            try
            {
                var uri = new Uri(uriText);
                using (var stream = AssetLoader.Open(uri))
                {
                    var bitmap = new Bitmap(stream);
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        private void OnPointerPressed(Point screenP)
        {
            _clickAction?.Invoke();
        }

        public void Dispose()
        {

        }
    }
}
