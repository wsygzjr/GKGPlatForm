using GF_Gereric;
using GKG;
using GKG.ElectronicControl.Dispenser;
using GKG.ElectronicControl.General;
using GKG.SubMM;
using GKG.SubMM.Dispenser;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage.DispensingFunctionHeadInit.ViewModels
{
    /// <summary>
    /// 点胶机功能头初始化配置页面ViewModel
    /// 管理38个配置参数，包含4个主要配置区域：
    /// 1. 阀参数配置（11个字段：包含阀报警IO参数）
    /// 2. 阀通讯参数配置（7个字段）
    /// 3. 阀气压控制参数配置（9个字段）
    /// 4. 供胶装置气压控制参数配置（9个字段）
    /// 
    /// 支持多层嵌套的byte[]数据序列化/反序列化：
    /// DispensingFunctionHeadSubMachineModulesInitCfg
    ///   ├── ValveInitParams (byte[]) → GKGPiezoValveInitParams
    ///   │   ├── CommunicatorParams (byte[]) → SerialConfig
    ///   │   ├── PressureControlParams (byte[]) → PressureControlSerialPortInitParams
    ///   │   │   └── SerialConfig (byte[]) → SerialConfig
    ///   │   └── ValveAlarmIOParams (Guid) → IO点位
    ///   └── GlueDispensingDeviceInitParams (byte[]) → GlueDispensingDeviceGKGInitParams
    ///       └── PressureControlInitParams (byte[]) → PressureControlSerialPortInitParams
    /// </summary>
    internal class DispensingFunctionHeadInitCompUIViewModel : ReactiveObject
    {
        /// <summary>
        /// 轴信息列表，从后端动态加载
        /// </summary>
        private readonly List<AxisInformation> _axisInformations = new();

        /// <summary>
        /// IO状态信息列表，从后端动态加载
        /// </summary>
        private readonly List<IOStateInformation> _ioStateInformations = new();

        /// <summary>
        /// 初始化配置数据对象
        /// </summary>
        private DispensingFunctionHeadSubMachineModulesInitCfg _data = new();

        private object _viewTag;
        private bool _readOnly;
        private bool _isSettingData; // 标志：正在设置数据，防止触发AfterModified

        /// <summary>
        /// 数据修改后触发的事件
        /// </summary>
        public event EventHandler AfterModified;

        /// <summary>
        /// 视图标签，用于视图和ViewModel之间的关联
        /// </summary>
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        /// <summary>
        /// 只读模式标志
        /// 控制所有输入控件的启用/禁用状态
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                UpdateEnabledState(!_readOnly);
            }
        }

        #region 阀参数配置 ViewModel属性（11个字段）

        /// <summary>阀绑定的轴</summary>
        public ComboxViewModel XAxisBindingViewModel { get; }
        
        /// <summary>阀名称</summary>
        public TextInputViewModel ValveNameViewModel { get; }
        
        /// <summary>阀ID</summary>
        public TextInputViewModel ValveIDViewModel { get; }
        
        /// <summary>阀报警IO参数</summary>
        public IOSensorConfigViewModel ValveAlarmIOViewModel { get; }
        
        /// <summary>触发通道</summary>
        public TextInputViewModel ChannelViewModel { get; }
        
        /// <summary>开阀时间(ms)</summary>
        public TextInputViewModel OpenValveTimeMsViewModel { get; }
        
        /// <summary>关阀时间(ms)</summary>
        public TextInputViewModel CloseValveTimeMsViewModel { get; }
        
        /// <summary>报警次数检测</summary>
        public TextInputViewModel AlarmCountDetectViewModel { get; }
        
        /// <summary>是否开启报警次数检测</summary>
        public ToggleSwitchViewModel IsDetectAlarmCountViewModel { get; }
        
        /// <summary>人工清洗喷嘴间隔时间(s)</summary>
        public TextInputViewModel ManualCleaningNozzleTimeSViewModel { get; }
        
        /// <summary>是否开启人工清洗喷嘴提示功能</summary>
        public ToggleSwitchViewModel IsManualCleaningViewModel { get; }

        #endregion

        #region 阀通讯参数配置 ViewModel属性

        /// <summary>阀通讯参数（串口配置）</summary>
        public SerialPortViewModel ValveCommunicatorViewModel { get; }

        #endregion

        #region 阀气压控制参数配置 ViewModel属性

        /// <summary>阀气压控制参数（串口配置）</summary>
        public SerialPortViewModel ValvePressureControlViewModel { get; }
        
        /// <summary>气压控制站号</summary>
        public TextInputViewModel ValvePressureStationIdViewModel { get; }
        
        /// <summary>气压控制通道号</summary>
        public TextInputViewModel ValvePressureChannelIdViewModel { get; }

        #endregion

        #region 供胶装置气压控制参数配置 ViewModel属性

        /// <summary>供胶气压控制参数（串口配置）</summary>
        public SerialPortViewModel GluePressureControlViewModel { get; }
        
        /// <summary>气压控制站号</summary>
        public TextInputViewModel GluePressureStationIdViewModel { get; }
        
        /// <summary>气压控制通道号</summary>
        public TextInputViewModel GluePressureChannelIdViewModel { get; }
        
        /// <summary>胶量感应IO参数</summary>
        public IOSensorConfigViewModel GlueAmountStateIOViewModel { get; }

        #endregion

        /// <summary>
        /// 构造函数
        /// 初始化所有ViewModel、枚举下拉框选项、订阅事件、加载轴信息和IO信息
        /// </summary>
        /// <param name="callBack">CompUI运行时回调接口</param>
        public DispensingFunctionHeadInitCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            // Initialize Valve ViewModels
            XAxisBindingViewModel = CreateComboViewModel();
            ValveNameViewModel = new TextInputViewModel();
            ValveIDViewModel = new TextInputViewModel();
            ValveAlarmIOViewModel = new IOSensorConfigViewModel();
            ChannelViewModel = new TextInputViewModel();
            OpenValveTimeMsViewModel = new TextInputViewModel();
            CloseValveTimeMsViewModel = new TextInputViewModel();
            AlarmCountDetectViewModel = new TextInputViewModel();
            IsDetectAlarmCountViewModel = new ToggleSwitchViewModel();
            ManualCleaningNozzleTimeSViewModel = new TextInputViewModel();
            IsManualCleaningViewModel = new ToggleSwitchViewModel();

            // Initialize Valve Communicator ViewModels
            ValveCommunicatorViewModel = new SerialPortViewModel();

            // Initialize Valve Pressure Control ViewModels
            ValvePressureControlViewModel = new SerialPortViewModel();
            ValvePressureStationIdViewModel = new TextInputViewModel();
            ValvePressureChannelIdViewModel = new TextInputViewModel();

            // Initialize Glue Pressure Control ViewModels
            GluePressureControlViewModel = new SerialPortViewModel();
            GluePressureStationIdViewModel = new TextInputViewModel();
            GluePressureChannelIdViewModel = new TextInputViewModel();
            GlueAmountStateIOViewModel = new IOSensorConfigViewModel();

            // Subscribe to change events
            SubscribeToChangeEvents();

            // Initialize enum options
            InitializeEnumOptions();

            LoadAxisInfos(callBack);
            ApplyAxisOptions(Guid.Empty);
            LoadIOStateInfos(callBack);
            RefreshIOOptions();
            ReadOnly = false;
        }

        private void SubscribeToChangeEvents()
        {
            XAxisBindingViewModel.ValueChanged += OnValueChanged;
            ValveNameViewModel.ValueChanged += OnValueChanged;
            ValveIDViewModel.ValueChanged += OnValueChanged;
            ValveAlarmIOViewModel.AfterModified += OnValueChanged;
            ChannelViewModel.ValueChanged += OnValueChanged;
            OpenValveTimeMsViewModel.ValueChanged += OnValueChanged;
            CloseValveTimeMsViewModel.ValueChanged += OnValueChanged;
            AlarmCountDetectViewModel.ValueChanged += OnValueChanged;
            IsDetectAlarmCountViewModel.ValueChanged += OnValueChanged;
            ManualCleaningNozzleTimeSViewModel.ValueChanged += OnValueChanged;
            IsManualCleaningViewModel.ValueChanged += OnValueChanged;

            ValveCommunicatorViewModel.AfterModified += OnValueChanged;

            ValvePressureControlViewModel.AfterModified += OnValueChanged;
            ValvePressureStationIdViewModel.ValueChanged += OnValueChanged;
            ValvePressureChannelIdViewModel.ValueChanged += OnValueChanged;

            GluePressureControlViewModel.AfterModified += OnValueChanged;
            GluePressureStationIdViewModel.ValueChanged += OnValueChanged;
            GluePressureChannelIdViewModel.ValueChanged += OnValueChanged;
            GlueAmountStateIOViewModel.AfterModified += OnValueChanged;
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            // 如果正在SetData，不触发AfterModified
            if (_isSettingData)
            {
                return;
            }

            AfterModified?.Invoke(sender, e);
        }

        private void InitializeEnumOptions()
        {
            // ToggleSwitchViewModel does not require items initialization.
        }

        public void SetData(DispensingFunctionHeadSubMachineModulesInitCfg data)
        {
            _isSettingData = true; // 开始设置数据
            try
            {
                _data = CloneData(data);

                // Deserialize ValveInitParams
                if (_data.ValveInitParams != null && _data.ValveInitParams.Length > 0)
                {
                    var valveParams = JsonObjConvert.FromJSonBytes<GKGPiezoValveInitParams>(_data.ValveInitParams);
                    if (valveParams != null)
                    {
                        ApplyAxisOptions(valveParams.XAxisBindingObjID);
                        ValveNameViewModel.Text = valveParams.Name ?? "";
                        ValveIDViewModel.Text = valveParams.ID ?? "";
                        RefreshIOOptions();
                        ValveAlarmIOViewModel.SetOptions(_ioStateInformations, valveParams.ValveAlarmIOParams);
                        ChannelViewModel.Text = valveParams.Channel.ToString(CultureInfo.InvariantCulture);
                        OpenValveTimeMsViewModel.Text = valveParams.OpenValveTimeMs.ToString(CultureInfo.InvariantCulture);
                        CloseValveTimeMsViewModel.Text = valveParams.CloseValveTimeMs.ToString(CultureInfo.InvariantCulture);
                        AlarmCountDetectViewModel.Text = valveParams.AlarmCountDetect.ToString(CultureInfo.InvariantCulture);
                        IsDetectAlarmCountViewModel.IsChecked = valveParams.IsDetectAlarmCount;
                        ManualCleaningNozzleTimeSViewModel.Text = valveParams.ManualCleaningOfTheNozzleTimeS.ToString(CultureInfo.InvariantCulture);
                        IsManualCleaningViewModel.IsChecked = valveParams.IsManualCleaning;

                        // Deserialize CommunicatorParams
                        if (valveParams.CommunicatorParams != null && valveParams.CommunicatorParams.Length > 0)
                        {
                            var serialConfig = JsonObjConvert.FromJSonBytes<SerialConfig>(valveParams.CommunicatorParams);
                            ValveCommunicatorViewModel.CopyFrom(serialConfig);
                        }

                        // Deserialize PressureControlParams
                        if (valveParams.PressureControlParams != null && valveParams.PressureControlParams.Length > 0)
                        {
                            var pressureParams = JsonObjConvert.FromJSonBytes<PressureControlSerialPortInitParams>(valveParams.PressureControlParams);
                            if (pressureParams != null)
                            {
                                ValvePressureStationIdViewModel.Text = pressureParams.StationId.ToString(CultureInfo.InvariantCulture);
                                ValvePressureChannelIdViewModel.Text = pressureParams.ChannelId.ToString(CultureInfo.InvariantCulture);

                                if (pressureParams.SerialConfig != null && pressureParams.SerialConfig.Length > 0)
                                {
                                    var pressureSerialConfig = JsonObjConvert.FromJSonBytes<SerialConfig>(pressureParams.SerialConfig);
                                    ValvePressureControlViewModel.CopyFrom(pressureSerialConfig);
                                }
                            }
                        }
                    }
                }

                // Deserialize GlueDispensingDeviceInitParams
                GlueDispensingDeviceGKGInitParams glueParams = null;
                if (_data.GlueDispensingDeviceInitParams != null && _data.GlueDispensingDeviceInitParams.Length > 0)
                {
                    glueParams = JsonObjConvert.FromJSonBytes<GlueDispensingDeviceGKGInitParams>(_data.GlueDispensingDeviceInitParams);
                }

                if (glueParams != null)
                {
                    // Load GlueAmountStateInitParams (胶量感应IO参数)
                    GlueAmountStateIOViewModel.SetOptions(_ioStateInformations, glueParams.GlueAmountStateInitParams);

                    // Deserialize PressureControlInitParams
                    PressureControlSerialPortInitParams gluePressureParams = null;
                    if (glueParams.PressureControlInitParams != null && glueParams.PressureControlInitParams.Length > 0)
                    {
                        gluePressureParams = JsonObjConvert.FromJSonBytes<PressureControlSerialPortInitParams>(glueParams.PressureControlInitParams);
                    }

                    if (gluePressureParams != null)
                    {
                        GluePressureStationIdViewModel.Text = gluePressureParams.StationId.ToString(CultureInfo.InvariantCulture);
                        GluePressureChannelIdViewModel.Text = gluePressureParams.ChannelId.ToString(CultureInfo.InvariantCulture);

                        SerialConfig? glueSerialConfig = null;
                        if (gluePressureParams.SerialConfig != null && gluePressureParams.SerialConfig.Length > 0)
                        {
                            glueSerialConfig = JsonObjConvert.FromJSonBytes<SerialConfig>(gluePressureParams.SerialConfig);
                        }
                        GluePressureControlViewModel.CopyFrom(glueSerialConfig);
                    }
                    else
                    {
                        // 如果压力控制参数为空，设置默认值
                        GluePressureStationIdViewModel.Text = "0";
                        GluePressureChannelIdViewModel.Text = "0";
                        GluePressureControlViewModel.CopyFrom(null);
                    }
                }
                else
                {
                    // 如果供胶参数为空，设置默认值
                    GlueAmountStateIOViewModel.SetOptions(_ioStateInformations, Guid.Empty);
                    GluePressureStationIdViewModel.Text = "0";
                    GluePressureChannelIdViewModel.Text = "0";
                    GluePressureControlViewModel.CopyFrom(null);
                }
            }
            finally
            {
                _isSettingData = false; // 结束设置数据
            }
        }

        public DispensingFunctionHeadSubMachineModulesInitCfg GetData()
        {
            _data ??= new DispensingFunctionHeadSubMachineModulesInitCfg();

            // Build CommunicatorParams using SerialPortViewModel
            var valveCommunicatorConfig = new SerialConfig();
            ValveCommunicatorViewModel.CopyTo(ref valveCommunicatorConfig);

            // Build PressureControlParams using SerialPortViewModel
            var valvePressureSerialConfig = new SerialConfig();
            ValvePressureControlViewModel.CopyTo(ref valvePressureSerialConfig);

            var valvePressureParams = new PressureControlSerialPortInitParams
            {
                SerialConfig = JsonObjConvert.ToJSonBytes(valvePressureSerialConfig),
                StationId = ParseIntOrDefault(ValvePressureStationIdViewModel.Text, 0),
                ChannelId = ParseIntOrDefault(ValvePressureChannelIdViewModel.Text, 0)
            };

            // Build ValveInitParams
            var valveParams = new GKGPiezoValveInitParams
            {
                XAxisBindingObjID = GetSelectedAxisGuid(XAxisBindingViewModel),
                Name = ValveNameViewModel.Text,
                ID = ValveIDViewModel.Text,
                ValveAlarmIOParams = ValveAlarmIOViewModel.GetSelectedGuid(),
                Channel = ParseIntOrDefault(ChannelViewModel.Text, 0),
                OpenValveTimeMs = ParseDoubleOrDefault(OpenValveTimeMsViewModel.Text, 0),
                CloseValveTimeMs = ParseDoubleOrDefault(CloseValveTimeMsViewModel.Text, 0),
                AlarmCountDetect = ParseIntOrDefault(AlarmCountDetectViewModel.Text, 0),
                IsDetectAlarmCount = IsDetectAlarmCountViewModel.IsChecked,
                ManualCleaningOfTheNozzleTimeS = ParseIntOrDefault(ManualCleaningNozzleTimeSViewModel.Text, 0),
                IsManualCleaning = IsManualCleaningViewModel.IsChecked,
                CommunicatorParams = JsonObjConvert.ToJSonBytes(valveCommunicatorConfig),
                PressureControlParams = JsonObjConvert.ToJSonBytes(valvePressureParams)
            };

            _data.ValveInitParams = JsonObjConvert.ToJSonBytes(valveParams);

            // Build GlueDispensingDeviceInitParams - 完全参照ValveInitParams的模式
            var gluePressureSerialConfig = new SerialConfig();
            GluePressureControlViewModel.CopyTo(ref gluePressureSerialConfig);

            var gluePressureParams = new PressureControlSerialPortInitParams
            {
                SerialConfig = JsonObjConvert.ToJSonBytes(gluePressureSerialConfig),
                StationId = ParseIntOrDefault(GluePressureStationIdViewModel.Text, 0),
                ChannelId = ParseIntOrDefault(GluePressureChannelIdViewModel.Text, 0)
            };

            var glueParams = new GlueDispensingDeviceGKGInitParams
            {
                PressureControlInitParams = JsonObjConvert.ToJSonBytes(gluePressureParams),
                GlueAmountStateInitParams = GlueAmountStateIOViewModel.GetSelectedGuid()
            };

            _data.GlueDispensingDeviceInitParams = JsonObjConvert.ToJSonBytes(glueParams);

            return CloneData(_data);
        }

        private void UpdateEnabledState(bool enabled)
        {
            XAxisBindingViewModel.IsEnabled = enabled;
            ValveNameViewModel.IsEnabled = enabled;
            ValveIDViewModel.IsEnabled = enabled;
            ValveAlarmIOViewModel.IOChannelViewModel.IsEnabled = enabled;
            ChannelViewModel.IsEnabled = enabled;
            OpenValveTimeMsViewModel.IsEnabled = enabled;
            CloseValveTimeMsViewModel.IsEnabled = enabled;
            AlarmCountDetectViewModel.IsEnabled = enabled;
            IsDetectAlarmCountViewModel.IsEnabled = enabled;
            ManualCleaningNozzleTimeSViewModel.IsEnabled = enabled;
            IsManualCleaningViewModel.IsEnabled = enabled;

            ValveCommunicatorViewModel.PortNameViewModel.IsEnabled = enabled;
            ValveCommunicatorViewModel.BaudRateViewModel.IsEnabled = enabled;
            ValveCommunicatorViewModel.DataBitsViewModel.IsEnabled = enabled;
            ValveCommunicatorViewModel.StopBitsViewModel.IsEnabled = enabled;
            ValveCommunicatorViewModel.ParityViewModel.IsEnabled = enabled;
            ValveCommunicatorViewModel.EnableCRC16ViewModel.IsEnabled = enabled;
            ValveCommunicatorViewModel.ModbusTypeViewModel.IsEnabled = enabled;

            ValvePressureControlViewModel.PortNameViewModel.IsEnabled = enabled;
            ValvePressureControlViewModel.BaudRateViewModel.IsEnabled = enabled;
            ValvePressureControlViewModel.DataBitsViewModel.IsEnabled = enabled;
            ValvePressureControlViewModel.StopBitsViewModel.IsEnabled = enabled;
            ValvePressureControlViewModel.ParityViewModel.IsEnabled = enabled;
            ValvePressureControlViewModel.EnableCRC16ViewModel.IsEnabled = enabled;
            ValvePressureControlViewModel.ModbusTypeViewModel.IsEnabled = enabled;
            ValvePressureStationIdViewModel.IsEnabled = enabled;
            ValvePressureChannelIdViewModel.IsEnabled = enabled;

            GluePressureControlViewModel.PortNameViewModel.IsEnabled = enabled;
            GluePressureControlViewModel.BaudRateViewModel.IsEnabled = enabled;
            GluePressureControlViewModel.DataBitsViewModel.IsEnabled = enabled;
            GluePressureControlViewModel.StopBitsViewModel.IsEnabled = enabled;
            GluePressureControlViewModel.ParityViewModel.IsEnabled = enabled;
            GluePressureControlViewModel.EnableCRC16ViewModel.IsEnabled = enabled;
            GluePressureControlViewModel.ModbusTypeViewModel.IsEnabled = enabled;
            GluePressureStationIdViewModel.IsEnabled = enabled;
            GluePressureChannelIdViewModel.IsEnabled = enabled;
            GlueAmountStateIOViewModel.IOChannelViewModel.IsEnabled = enabled;
        }

        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>(),
            };
        }

        private void LoadAxisInfos(ICompUIRunTimeCallBack callBack)
        {
            _axisInformations.Clear();
            try
            {
                var result = callBack?.ExecConfigSvrCtlCmd("GetAxisOptions", new GFBaseTypeParamValueList());
                var raw = result?["data"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                    raw = result?["Result"]?.ToStringVal();

                var axisInfos = string.IsNullOrWhiteSpace(raw)
                    ? null
                    : JsonObjConvert.FromJSon<List<AxisInformation>>(raw);

                if (axisInfos != null)
                    _axisInformations.AddRange(axisInfos.Where(item => item != null && item.AxisGuid != Guid.Empty));
            }
            catch
            {
            }
        }

        /// <summary>
        /// 从后端加载IO状态信息列表
        /// </summary>
        /// <param name="callBack">回调接口</param>
        private void LoadIOStateInfos(ICompUIRunTimeCallBack callBack)
        {
            _ioStateInformations.Clear();
            if (callBack == null)
            {
                return;
            }

            try
            {
                var result = callBack.ExecConfigSvrCtlCmd("GetIOInfos", new GFBaseTypeParamValueList());
                var raw = result?["data"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    raw = result?["Result"]?.ToStringVal();
                }

                var ioInfos = string.IsNullOrWhiteSpace(raw)
                    ? null
                    : JsonObjConvert.FromJSon<List<IOStateInformation>>(raw);

                if (ioInfos != null)
                {
                    _ioStateInformations.AddRange(ioInfos.Where(item => item != null && item.IOGuid != Guid.Empty));
                }
            }
            catch
            {
                // 加载失败时保持空列表
            }
        }

        /// <summary>
        /// 刷新IO选项
        /// </summary>
        private void RefreshIOOptions()
        {
            ValveAlarmIOViewModel.SetOptions(_ioStateInformations, ValveAlarmIOViewModel.GetSelectedGuid());
            GlueAmountStateIOViewModel.SetOptions(_ioStateInformations, GlueAmountStateIOViewModel.GetSelectedGuid());
        }

        private void ApplyAxisOptions(Guid selectedAxisGuid)
        {
            var items = XAxisBindingViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();
            if (items == null) return;

            foreach (var axisInfo in _axisInformations
                .Where(item => item != null && item.AxisGuid != Guid.Empty)
                .GroupBy(item => item.AxisGuid)
                .Select(group => group.First()))
            {
                items.Add(new ComBoxItem
                {
                    Value = axisInfo,
                    DisplayName = string.IsNullOrWhiteSpace(axisInfo.AxisName) ? axisInfo.AxisGuid.ToString() : axisInfo.AxisName,
                });
            }

            if (items.Count == 0)
            {
                items.Add(new ComBoxItem
                {
                    Value = null,
                    DisplayName = "等待从后端获取列表",
                });
            }

            if (selectedAxisGuid != Guid.Empty &&
                items.All(item => (item.Value as AxisInformation)?.AxisGuid != selectedAxisGuid))
            {
                items.Add(new ComBoxItem
                {
                    Value = new AxisInformation
                    {
                        AxisGuid = selectedAxisGuid,
                        AxisName = selectedAxisGuid.ToString(),
                    },
                    DisplayName = selectedAxisGuid.ToString(),
                });
            }

            XAxisBindingViewModel.SelectedItem = items[0];
            SetSelectedAxis(XAxisBindingViewModel, items, selectedAxisGuid);
        }

        private static void SetSelectedAxis(ComboxViewModel viewModel, IEnumerable<ComBoxItem> items, Guid axisGuid)
        {
            var target = items.FirstOrDefault(item => (item.Value as AxisInformation)?.AxisGuid == axisGuid);
            if (target != null)
                viewModel.SelectedItem = target;
        }

        private static Guid GetSelectedAxisGuid(ComboxViewModel viewModel)
            => ((viewModel.SelectedItem as ComBoxItem)?.Value as AxisInformation)?.AxisGuid ?? Guid.Empty;

        private static DispensingFunctionHeadSubMachineModulesInitCfg CloneData(DispensingFunctionHeadSubMachineModulesInitCfg data)
        {
            if (data == null)
                return new DispensingFunctionHeadSubMachineModulesInitCfg();

            return JsonObjConvert.FromJSonBytes<DispensingFunctionHeadSubMachineModulesInitCfg>(JsonObjConvert.ToJSonBytes(data))
                ?? new DispensingFunctionHeadSubMachineModulesInitCfg();
        }

        private static double ParseDoubleOrDefault(string text, double defaultValue)
        {
            return double.TryParse(
                text,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var value)
                ? value
                : defaultValue;
        }

        private static int ParseIntOrDefault(string text, int defaultValue)
        {
            return int.TryParse(
                text,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var value)
                ? value
                : defaultValue;
        }
    }
}
