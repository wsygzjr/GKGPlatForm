using GF_Gereric;
using GKG;
using GKG.ElectronicControl.General;
using GKG.SubMM.Dispenser;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage.WeighingBalance.ViewModels
{
    /// <summary>
    /// 称重初始化配置视图模型
    /// </summary>
    public class WeighingBalanceInitCfgCompUIViewModel : ReactiveObject, IDisposable
    {
        private readonly List<AxisInformation> axisInformations = new();
        private WeighingBalanceSubMachineModulesInitCfg loadedData = new();
        private bool readOnly;
        private bool isApplyingData;
        private string selectedPortName = string.Empty;
        private ComBoxItem selectedPortItem;

        /// <summary>
        /// 修改后事件
        /// </summary>
        public event EventHandler AfterModified;

        /// <summary>
        /// 可用串口列表
        /// </summary>
        public ObservableCollection<ComBoxItem> PortNameItems { get; } = new();

        /// <summary>
        /// 阀实例ID列表
        /// </summary>
        public ObservableCollection<ValveBindingItemViewModel> ValveItems { get; } = new();

        /// <summary>
        /// 轴绑定选项
        /// </summary>
        public ObservableCollection<AxisBindingItemViewModel> AxisBindingItems { get; } = new();

        /// <summary>
        /// 串口号控件模型
        /// </summary>
        public ComboxViewModel PortNameViewModel { get; } = CreateComboViewModel();

        /// <summary>
        /// 波特率控件模型
        /// </summary>
        public ComboxViewModel BaudRateViewModel { get; } = CreateComboViewModel();

        /// <summary>
        /// 数据位控件模型
        /// </summary>
        public ComboxViewModel DataBitsViewModel { get; } = CreateComboViewModel();

        /// <summary>
        /// 停止位控件模型
        /// </summary>
        public ComboxViewModel StopBitsViewModel { get; } = CreateComboViewModel();

        /// <summary>
        /// 校验位控件模型
        /// </summary>
        public ComboxViewModel ParityViewModel { get; } = CreateComboViewModel();

        /// <summary>
        /// Modbus类型控件模型
        /// </summary>
        public ComboxViewModel ModbusTypeViewModel { get; } = CreateComboViewModel();

        /// <summary>
        /// 启用CRC16控件模型
        /// </summary>
        public ToggleSwitchViewModel IsEnableCRC16ViewModel { get; } = new();

        /// <summary>
        /// 启用回抹控件模型
        /// </summary>
        public ToggleSwitchViewModel BackRollingEnabledViewModel { get; } = new();

        /// <summary>
        /// 回抹次数控件模型
        /// </summary>
        public NumericUpDownViewModel BackRollingCountViewModel { get; } = CreateNumericViewModel(0, 999999, 1, 0);

        /// <summary>
        /// 回抹距离控件模型，单位 mm
        /// </summary>
        public NumericUpDownViewModel BackRollingDistanceViewModel { get; } = CreateNumericViewModel(0, 999999, 0.01m, 2);

        /// <summary>
        /// 称重位置 X 控件模型，允许负坐标
        /// </summary>
        public NumericUpDownViewModel WeighingPositionXViewModel { get; } = CreateNumericViewModel(-999999, 999999, 0.01m, 2);

        /// <summary>
        /// 称重位置 Y 控件模型，允许负坐标
        /// </summary>
        public NumericUpDownViewModel WeighingPositionYViewModel { get; } = CreateNumericViewModel(-999999, 999999, 0.01m, 2);

        /// <summary>
        /// 称重位置 Z 控件模型，允许负坐标
        /// </summary>
        public NumericUpDownViewModel WeighingPositionZViewModel { get; } = CreateNumericViewModel(-999999, 999999, 0.01m, 2);

        /// <summary>
        /// 回抹速度控件模型，单位 mm/s
        /// </summary>
        public NumericUpDownViewModel BackRollingSpeedViewModel { get; } = CreateNumericViewModel(0, 999999, 0.01m, 2);

        /// <summary>
        /// 回抹加速度控件模型，单位 mm/s2
        /// </summary>
        public NumericUpDownViewModel BackRollingAccViewModel { get; } = CreateNumericViewModel(0, 999999, 0.01m, 2);

        /// <summary>
        /// X轴绑定控件模型
        /// </summary>
        public ComboxViewModel XAxisBindingViewModel { get; } = CreateComboViewModel();

        /// <summary>
        /// Y轴绑定控件模型
        /// </summary>
        public ComboxViewModel YAxisBindingViewModel { get; } = CreateComboViewModel();

        /// <summary>
        /// Z轴绑定控件模型
        /// </summary>
        public ComboxViewModel ZAxisBindingViewModel { get; } = CreateComboViewModel();

        /// <summary>
        /// 波特率选项
        /// </summary>
        public ObservableCollection<decimal> BaudRateOptions { get; } = new()
        {
            50, 75, 110, 150, 300, 600, 1200, 1800, 2400,
            4800, 9600, 14400, 19200, 28800, 38400,
            57600, 115200, 230400, 460800, 921600,
        };

        /// <summary>
        /// 数据位选项
        /// </summary>
        public ObservableCollection<decimal> DataBitsOptions { get; } = new()
        {
            5, 6, 7, 8,
        };

        /// <summary>
        /// 新增阀实例ID命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddValveCommand { get; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                this.RaisePropertyChanged(nameof(CanEdit));
            }
        }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool CanEdit => !ReadOnly;

        /// <summary>
        /// 串口号
        /// </summary>
        public string SelectedPortName
        {
            get => selectedPortName;
            set => SetSelectedPortName(value ?? string.Empty);
        }

        /// <summary>
        /// 选中的串口项
        /// </summary>
        public ComBoxItem SelectedPortItem
        {
            get => selectedPortItem;
            set => SetSelectedPortItem(value);
        }

        /// <summary>
        /// 波特率
        /// </summary>
        public decimal BaudRate
        {
            get => GetSelectedValue(BaudRateViewModel, 38400m);
            set => SetSelectedValue(BaudRateViewModel, value);
        }

        /// <summary>
        /// 数据位
        /// </summary>
        public decimal DataBits
        {
            get => GetSelectedValue(DataBitsViewModel, 8m);
            set => SetSelectedValue(DataBitsViewModel, value);
        }

        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits SelectedStopBits
        {
            get => GetSelectedValue(StopBitsViewModel, StopBits.One);
            set => SetSelectedValue(StopBitsViewModel, value);
        }

        /// <summary>
        /// 校验位
        /// </summary>
        public Parity SelectedParity
        {
            get => GetSelectedValue(ParityViewModel, Parity.None);
            set => SetSelectedValue(ParityViewModel, value);
        }

        /// <summary>
        /// 是否启用CRC16
        /// </summary>
        public bool IsEnableCRC16
        {
            get => IsEnableCRC16ViewModel.IsChecked;
            set => IsEnableCRC16ViewModel.IsChecked = value;
        }

        /// <summary>
        /// Modbus类型
        /// </summary>
        public EModbusType SelectedModbusType
        {
            get => GetSelectedValue(ModbusTypeViewModel, EModbusType.RS232);
            set => SetSelectedValue(ModbusTypeViewModel, value);
        }

        /// <summary>
        /// 回抹开关
        /// </summary>
        public bool BackRollingEnabled
        {
            get => BackRollingEnabledViewModel.IsChecked;
            set => BackRollingEnabledViewModel.IsChecked = value;
        }

        /// <summary>
        /// 称重位置X
        /// </summary>
        public decimal WeighingPositionX
        {
            get => WeighingPositionXViewModel.Value;
            set => WeighingPositionXViewModel.Value = value;
        }

        /// <summary>
        /// 称重位置Y
        /// </summary>
        public decimal WeighingPositionY
        {
            get => WeighingPositionYViewModel.Value;
            set => WeighingPositionYViewModel.Value = value;
        }

        /// <summary>
        /// 称重位置Z
        /// </summary>
        public decimal WeighingPositionZ
        {
            get => WeighingPositionZViewModel.Value;
            set => WeighingPositionZViewModel.Value = value;
        }

        /// <summary>
        /// 回抹次数
        /// </summary>
        public decimal BackRollingCount
        {
            get => BackRollingCountViewModel.Value;
            set => BackRollingCountViewModel.Value = value;
        }

        /// <summary>
        /// 回抹距离
        /// </summary>
        public decimal BackRollingDistance
        {
            get => BackRollingDistanceViewModel.Value;
            set => BackRollingDistanceViewModel.Value = value;
        }

        /// <summary>
        /// 回抹速度
        /// </summary>
        public decimal BackRollingSpeed
        {
            get => BackRollingSpeedViewModel.Value;
            set => BackRollingSpeedViewModel.Value = value;
        }

        /// <summary>
        /// 回抹加速度
        /// </summary>
        public decimal BackRollingAcc
        {
            get => BackRollingAccViewModel.Value;
            set => BackRollingAccViewModel.Value = value;
        }

        /// <summary>
        /// X轴绑定
        /// </summary>
        public AxisBindingItemViewModel SelectedXAxisBinding
        {
            get => XAxisBindingViewModel.SelectedItem as AxisBindingItemViewModel;
            set => XAxisBindingViewModel.SelectedItem = value;
        }

        /// <summary>
        /// Y轴绑定
        /// </summary>
        public AxisBindingItemViewModel SelectedYAxisBinding
        {
            get => YAxisBindingViewModel.SelectedItem as AxisBindingItemViewModel;
            set => YAxisBindingViewModel.SelectedItem = value;
        }

        /// <summary>
        /// Z轴绑定
        /// </summary>
        public AxisBindingItemViewModel SelectedZAxisBinding
        {
            get => ZAxisBindingViewModel.SelectedItem as AxisBindingItemViewModel;
            set => ZAxisBindingViewModel.SelectedItem = value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WeighingBalanceInitCfgCompUIViewModel()
        {
            AddValveCommand = ReactiveCommand.Create(AddValveItem);
            InitializeControlViewModels();
            SubscribeControlViewModels();
            RefreshPortNames(string.Empty);
            ApplySerialConfig(CreateDefaultSerialConfig());
            ApplyActionParameters(new WeighingActionParameters());
            ApplyAxisBindings(new WeighingBalanceSubMachineModulesInitCfg());
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isDesign">是否设计模式</param>
        /// <param name="callBack">回调</param>
        public WeighingBalanceInitCfgCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
            : this()
        {
            if (!isDesign)
            {
                LoadAxisInfos(callBack);
                ApplyAxisBindings(loadedData);
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="model">初始化配置</param>
        public void SetData(WeighingBalanceSubMachineModulesInitCfg model)
        {
            ApplyWithoutModified(() =>
            {
                loadedData = CloneData(model);
                var serialConfig = ParseSerialConfig(loadedData.WeighingBalanceInitParams);
                RefreshPortNames(serialConfig.PortName);
                ApplySerialConfig(serialConfig);
                ApplyActionParameters(loadedData.WeighingActionParameters);
                ApplyAxisBindings(loadedData);
                ApplyValveItems(loadedData.ValveID);
            });
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>初始化配置</returns>
        public WeighingBalanceSubMachineModulesInitCfg GetData()
        {
            var result = CloneData(loadedData);
            var serialConfig = new SerialConfig
            {
                PortName = SelectedPortName ?? string.Empty,
                BaudRate = (int)BaudRate,
                DataBits = (int)DataBits,
                StopBits = SelectedStopBits,
                Parity = SelectedParity,
                IsEnableCRC16 = IsEnableCRC16,
                ModbusType = SelectedModbusType,
            };

            var initParams = new WeighingBalanceApwInitParams
            {
                SerialConfig = JsonObjConvert.ToJSonBytes(serialConfig),
            };

            result.WeighingBalanceInitParams = JsonObjConvert.ToJSonBytes(initParams);
            result.WeighingActionParameters = BuildActionParameters();
            result.XAxisBindingGuid = SelectedXAxisBinding?.AxisGuid ?? Guid.Empty;
            result.YAxisBindingGuid = SelectedYAxisBinding?.AxisGuid ?? Guid.Empty;
            result.ZAxisBindingGuid = SelectedZAxisBinding?.AxisGuid ?? Guid.Empty;
            result.ValveID = BuildValveDictionary();
            loadedData = CloneData(result);
            return result;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (var valveItem in ValveItems)
            {
                valveItem.AfterModified -= OnValveItemModified;
            }

            foreach (var controlViewModel in GetControlViewModels())
            {
                controlViewModel.ValueChanged -= OnControlValueChanged;
            }
        }

        private void InitializeControlViewModels()
        {
            PortNameViewModel.ItemsSource = PortNameItems;
            PortNameViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            BaudRateViewModel.ItemsSource = BaudRateOptions.Select(value => CreateComboItem(value, value.ToString())).ToArray();
            BaudRateViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            DataBitsViewModel.ItemsSource = DataBitsOptions.Select(value => CreateComboItem(value, value.ToString())).ToArray();
            DataBitsViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            StopBitsViewModel.ItemsSource = new[]
            {
                CreateComboItem(StopBits.One, "One"),
                CreateComboItem(StopBits.One5, "One5"),
                CreateComboItem(StopBits.Two, "Two"),
            };
            StopBitsViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);


            
            ParityViewModel.ItemsSource = new[]
            {
                CreateComboItem(Parity.None, "None"),
                CreateComboItem(Parity.Odd, "Odd"),
                CreateComboItem(Parity.Even, "Even"),
                CreateComboItem(Parity.Mark, "Mark"),
                CreateComboItem(Parity.Space, "Space"),
            };
            ParityViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            ModbusTypeViewModel.ItemsSource = new[]
            {
                CreateComboItem(EModbusType.RS232, "RS232"),
                CreateComboItem(EModbusType.RS485, "RS485"),
            };
            ModbusTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            XAxisBindingViewModel.ItemsSource = AxisBindingItems;
            XAxisBindingViewModel.DisplayMemberPath = nameof(AxisBindingItemViewModel.DisplayName);
            YAxisBindingViewModel.ItemsSource = AxisBindingItems;
            YAxisBindingViewModel.DisplayMemberPath = nameof(AxisBindingItemViewModel.DisplayName);
            ZAxisBindingViewModel.ItemsSource = AxisBindingItems;
            ZAxisBindingViewModel.DisplayMemberPath = nameof(AxisBindingItemViewModel.DisplayName);
        }

        private void SubscribeControlViewModels()
        {
            foreach (var controlViewModel in GetControlViewModels())
            {
                controlViewModel.ValueChanged += OnControlValueChanged;
            }
        }

        private IEnumerable<BasicControlViewModel> GetControlViewModels()
        {
            yield return PortNameViewModel;
            yield return BaudRateViewModel;
            yield return DataBitsViewModel;
            yield return StopBitsViewModel;
            yield return ParityViewModel;
            yield return ModbusTypeViewModel;
            yield return IsEnableCRC16ViewModel;
            yield return BackRollingEnabledViewModel;
            yield return BackRollingCountViewModel;
            yield return BackRollingDistanceViewModel;
            yield return WeighingPositionXViewModel;
            yield return WeighingPositionYViewModel;
            yield return WeighingPositionZViewModel;
            yield return BackRollingSpeedViewModel;
            yield return BackRollingAccViewModel;
            yield return XAxisBindingViewModel;
            yield return YAxisBindingViewModel;
            yield return ZAxisBindingViewModel;
        }

        private void OnControlValueChanged(object sender, EventArgs e)
        {
            if (sender == PortNameViewModel)
            {
                var portItem = PortNameViewModel.SelectedItem as ComBoxItem;
                selectedPortItem = portItem;
                selectedPortName = portItem?.Value as string ?? string.Empty;
                this.RaisePropertyChanged(nameof(SelectedPortItem));
                this.RaisePropertyChanged(nameof(SelectedPortName));
            }

            NotifyModified();
        }

        private void ApplySerialConfig(SerialConfig serialConfig)
        {
            SetSelectedPortName(ResolveSelectedPortName(serialConfig.PortName));
            BaudRate = serialConfig.BaudRate == 0 ? 38400 : serialConfig.BaudRate;
            DataBits = serialConfig.DataBits == 0 ? 8 : serialConfig.DataBits;
            SelectedStopBits = serialConfig.StopBits;
            SelectedParity = serialConfig.Parity;
            IsEnableCRC16 = serialConfig.IsEnableCRC16;
            SelectedModbusType = serialConfig.ModbusType;
        }

        private void ApplyActionParameters(WeighingActionParameters parameters)
        {
            parameters ??= new WeighingActionParameters();
            var position = parameters.WeighingPosition ?? new Point3D();

            BackRollingEnabled = parameters.BackRollingEnabled;
            WeighingPositionX = (decimal)position.X;
            WeighingPositionY = (decimal)position.Y;
            WeighingPositionZ = (decimal)position.Z;
            BackRollingCount = parameters.BackRollingCount;
            BackRollingDistance = (decimal)parameters.BackRollingDistance;
            BackRollingSpeed = (decimal)parameters.BackRollingSpeed;
            BackRollingAcc = (decimal)parameters.BackRollingAcc;
        }

        private WeighingActionParameters BuildActionParameters()
        {
            return new WeighingActionParameters
            {
                BackRollingEnabled = BackRollingEnabled,
                WeighingPosition = new Point3D((double)WeighingPositionX, (double)WeighingPositionY, (double)WeighingPositionZ),
                BackRollingCount = (int)BackRollingCount,
                BackRollingDistance = (double)BackRollingDistance,
                BackRollingSpeed = (double)BackRollingSpeed,
                BackRollingAcc = (double)BackRollingAcc,
            };
        }

        private void ApplyAxisBindings(WeighingBalanceSubMachineModulesInitCfg model)
        {
            model ??= new WeighingBalanceSubMachineModulesInitCfg();

            RefreshAxisBindingItems(model.XAxisBindingGuid, model.YAxisBindingGuid, model.ZAxisBindingGuid);
            SelectedXAxisBinding = FindAxisBindingItem(model.XAxisBindingGuid);
            SelectedYAxisBinding = FindAxisBindingItem(model.YAxisBindingGuid);
            SelectedZAxisBinding = FindAxisBindingItem(model.ZAxisBindingGuid);
        }

        private void ApplyValveItems(Dictionary<string, string> valveIDs)
        {
            foreach (var valveItem in ValveItems)
            {
                valveItem.AfterModified -= OnValveItemModified;
            }

            ValveItems.Clear();

            if (valveIDs == null)
            {
                return;
            }

            foreach (var pair in valveIDs)
            {
                ValveItems.Add(CreateValveBindingItem(pair.Key, pair.Value));
            }
        }

        private ValveBindingItemViewModel CreateValveBindingItem(string valveID, string alias)
        {
            var item = new ValveBindingItemViewModel(RemoveValveItem)
            {
                ValveID = valveID ?? string.Empty,
                Alias = alias ?? string.Empty,
            };
            item.AfterModified += OnValveItemModified;
            return item;
        }

        private void AddValveItem()
        {
            var valveID = CreateNextValveID();
            ValveItems.Add(CreateValveBindingItem(valveID, valveID));
            NotifyModified();
        }

        private void RemoveValveItem(ValveBindingItemViewModel item)
        {
            if (item == null)
            {
                return;
            }

            item.AfterModified -= OnValveItemModified;
            if (ValveItems.Remove(item))
            {
                NotifyModified();
            }
        }

        private void OnValveItemModified(object sender, EventArgs e)
        {
            NotifyModified();
        }

        private Dictionary<string, string> BuildValveDictionary()
        {
            var result = new Dictionary<string, string>();

            foreach (var item in ValveItems)
            {
                var valveID = item?.ValveID?.Trim();
                if (string.IsNullOrWhiteSpace(valveID))
                {
                    continue;
                }

                var alias = string.IsNullOrWhiteSpace(item.Alias) ? valveID : item.Alias.Trim();
                result[valveID] = alias;
            }

            return result;
        }

        private string CreateNextValveID()
        {
            var index = 1;
            while (ValveItems.Any(item => string.Equals(item.ValveID, $"Valve{index}", StringComparison.OrdinalIgnoreCase)))
            {
                index++;
            }

            return $"Valve{index}";
        }

        private void LoadAxisInfos(ICompUIRunTimeCallBack callBack)
        {
            axisInformations.Clear();
            if (callBack == null)
            {
                return;
            }

            try
            {
                var result = callBack.ExecConfigSvrCtlCmd("GetAxisOptions", new GFBaseTypeParamValueList());
                var raw = result?["data"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    raw = result?["Result"]?.ToStringVal();
                }

                var axisInfos = string.IsNullOrWhiteSpace(raw)
                    ? null
                    : JsonObjConvert.FromJSon<List<AxisInformation>>(raw);

                if (axisInfos != null)
                {
                    axisInformations.AddRange(axisInfos.Where(item => item != null && item.AxisGuid != Guid.Empty));
                    foreach (var item in axisInformations)
                    {
                        AxisBindingItems.Add(new AxisBindingItemViewModel(item.AxisGuid, item.AxisName));
                    }
                }
            }
            catch
            {
            }
        }

        private void RefreshAxisBindingItems(Guid xAxisGuid, Guid yAxisGuid, Guid zAxisGuid)
        {
            AxisBindingItems.Clear();
           

            foreach (var axisInfo in axisInformations
                         .Where(item => item != null && item.AxisGuid != Guid.Empty)
                         .GroupBy(item => item.AxisGuid)
                         .Select(group => group.First()))
            {
                AxisBindingItems.Add(new AxisBindingItemViewModel(
                    axisInfo.AxisGuid,
                    string.IsNullOrWhiteSpace(axisInfo.AxisName) ? axisInfo.AxisGuid.ToString() : axisInfo.AxisName));
            }

            AddSavedAxisBinding(xAxisGuid);
            AddSavedAxisBinding(yAxisGuid);
            AddSavedAxisBinding(zAxisGuid);
        }

        private void AddSavedAxisBinding(Guid axisGuid)
        {
            if (axisGuid == Guid.Empty || AxisBindingItems.Any(item => item.AxisGuid == axisGuid))
            {
                return;
            }

            AxisBindingItems.Add(new AxisBindingItemViewModel(axisGuid, axisGuid.ToString()));
        }

        private AxisBindingItemViewModel FindAxisBindingItem(Guid axisGuid)
        {
            return AxisBindingItems.FirstOrDefault(item => item.AxisGuid == axisGuid)
                ?? AxisBindingItems.FirstOrDefault();
        }

        private void RefreshPortNames(string savedPortName)
        {
            PortNameItems.Clear();

            foreach (var portName in GetLocalPortNames())
            {
                PortNameItems.Add(CreatePortItem(portName));
            }

            if (!string.IsNullOrWhiteSpace(savedPortName) &&
                PortNameItems.All(item => !IsPortItem(item, savedPortName)))
            {
                PortNameItems.Add(CreatePortItem(savedPortName));
            }
        }

        private string ResolveSelectedPortName(string portName)
        {
            if (!string.IsNullOrWhiteSpace(portName))
            {
                var savedPort = FindPortItem(portName)?.Value as string;
                if (!string.IsNullOrWhiteSpace(savedPort))
                {
                    return savedPort;
                }

                return portName;
            }

            return PortNameItems.FirstOrDefault()?.Value as string ?? string.Empty;
        }

        private void SetSelectedPortName(string portName)
        {
            var portItem = FindPortItem(portName);
            if (portItem == null && !string.IsNullOrWhiteSpace(portName))
            {
                portItem = CreatePortItem(portName);
                PortNameItems.Add(portItem);
            }

            SetSelectedPortItem(portItem);
        }

        private void SetSelectedPortItem(ComBoxItem portItem)
        {
            if (portItem != null)
            {
                PortNameViewModel.SelectedItem = portItem;
            }

            this.RaiseAndSetIfChanged(ref selectedPortItem, portItem);
            this.RaiseAndSetIfChanged(ref selectedPortName, portItem?.Value as string ?? string.Empty);
            NotifyModified();
        }

        private ComBoxItem FindPortItem(string portName)
        {
            return PortNameItems.FirstOrDefault(item => IsPortItem(item, portName));
        }

        private static bool IsPortItem(ComBoxItem item, string portName)
        {
            return item?.Value is string value &&
                string.Equals(value, portName, StringComparison.OrdinalIgnoreCase);
        }

        private static ComBoxItem CreatePortItem(string portName)
        {
            return new ComBoxItem { Value = portName, DisplayName = portName };
        }

        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                PlaceholderText = string.Empty,
            };
        }

        private static NumericUpDownViewModel CreateNumericViewModel(decimal minimum, decimal maximum, decimal increment, int decimalPlaces)
        {
            return new NumericUpDownViewModel
            {
                Minimum = minimum,
                Maximum = maximum,
                Increment = increment,
                DecimalPlaces = decimalPlaces,
            };
        }

        private static ComBoxItem CreateComboItem(object value, string displayName)
        {
            return new ComBoxItem { Value = value, DisplayName = displayName };
        }

        private static T GetSelectedValue<T>(ComboxViewModel viewModel, T defaultValue)
        {
            return viewModel.SelectedItem is ComBoxItem item && item.Value is T value
                ? value
                : defaultValue;
        }

        private static void SetSelectedValue<T>(ComboxViewModel viewModel, T value)
        {
            if (viewModel.ItemsSource == null)
            {
                return;
            }

            foreach (var item in viewModel.ItemsSource)
            {
                if (item is ComBoxItem comboItem && Equals(comboItem.Value, value))
                {
                    viewModel.SelectedItem = comboItem;
                    return;
                }
            }
        }

        private static IEnumerable<string> GetLocalPortNames()
        {
            var portNames = GetRjcpPortNames();
            if (portNames.Length == 0)
            {
                portNames = GetSystemPortNames();
            }

            return portNames
                .OrderBy(GetPortSortOrder)
                .ThenBy(portName => portName, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string[] GetRjcpPortNames()
        {
            try
            {
                return new SerialPortStream().GetPortNames()
                    .ToArray();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        private static string[] GetSystemPortNames()
        {
            if (!OperatingSystem.IsWindows())
            {
                return Array.Empty<string>();
            }

            return GetWindowsRegistryPortNames();
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static string[] GetWindowsRegistryPortNames()
        {
            try
            {
                using var serialCommKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM");
                return serialCommKey?.GetValueNames()
                    .Select(name => serialCommKey.GetValue(name)?.ToString())
                    .Where(portName => !string.IsNullOrWhiteSpace(portName))
                    .ToArray() ?? Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        private static int GetPortSortOrder(string portName)
        {
            if (!string.IsNullOrWhiteSpace(portName) &&
                portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(portName.Substring(3), out var portNumber))
            {
                return portNumber;
            }

            return int.MaxValue;
        }

        private static SerialConfig ParseSerialConfig(byte[] initParamsBytes)
        {
            if (initParamsBytes == null || initParamsBytes.Length == 0)
            {
                return CreateDefaultSerialConfig();
            }

            try
            {
                var initParams = JsonObjConvert.FromJSonBytes<WeighingBalanceApwInitParams>(initParamsBytes);
                if (initParams?.SerialConfig == null || initParams.SerialConfig.Length == 0)
                {
                    return CreateDefaultSerialConfig();
                }

                return JsonObjConvert.FromJSonBytes<SerialConfig>(initParams.SerialConfig);
            }
            catch
            {
                return CreateDefaultSerialConfig();
            }
        }

        private static SerialConfig CreateDefaultSerialConfig()
        {
            return new SerialConfig
            {
                PortName = string.Empty,
                BaudRate = 38400,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                IsEnableCRC16 = false,
                ModbusType = EModbusType.RS232,
            };
        }

        private static WeighingBalanceSubMachineModulesInitCfg CloneData(WeighingBalanceSubMachineModulesInitCfg model)
        {
            if (model == null)
            {
                return new WeighingBalanceSubMachineModulesInitCfg
                {
                    ValveID = new Dictionary<string, string>(),
                    WeighingActionParameters = new WeighingActionParameters(),
                    WeighingBalanceInitParams = Array.Empty<byte>(),
                };
            }

            return new WeighingBalanceSubMachineModulesInitCfg
            {
                WeighingActionParameters = CloneActionParameters(model.WeighingActionParameters),
                XAxisBindingGuid = model.XAxisBindingGuid,
                YAxisBindingGuid = model.YAxisBindingGuid,
                ZAxisBindingGuid = model.ZAxisBindingGuid,
                ValveID = model.ValveID == null ? new Dictionary<string, string>() : new Dictionary<string, string>(model.ValveID),
                WeighingBalanceInitParams = model.WeighingBalanceInitParams == null
                    ? Array.Empty<byte>()
                    : (byte[])model.WeighingBalanceInitParams.Clone(),
            };
        }

        private static WeighingActionParameters CloneActionParameters(WeighingActionParameters source)
        {
            if (source == null)
            {
                return new WeighingActionParameters();
            }

            return new WeighingActionParameters
            {
                BackRollingEnabled = source.BackRollingEnabled,
                WeighingPosition = source.WeighingPosition == null ? new Point3D() : new Point3D(source.WeighingPosition),
                BackRollingCount = source.BackRollingCount,
                BackRollingDistance = source.BackRollingDistance,
                BackRollingSpeed = source.BackRollingSpeed,
                BackRollingAcc = source.BackRollingAcc,
            };
        }

        private void NotifyModified()
        {
            if (isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyWithoutModified(Action action)
        {
            isApplyingData = true;
            try
            {
                action();
            }
            finally
            {
                isApplyingData = false;
            }
        }
    }

    /// <summary>
    /// 轴绑定选项
    /// </summary>
    public class AxisBindingItemViewModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="axisGuid">轴Guid</param>
        /// <param name="displayName">显示名称</param>
        public AxisBindingItemViewModel(Guid axisGuid, string displayName)
        {
            AxisGuid = axisGuid;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? "未绑定" : displayName;
        }

        /// <summary>
        /// 轴Guid
        /// </summary>
        public Guid AxisGuid { get; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; }
    }

    /// <summary>
    /// 阀实例ID编辑项
    /// </summary>
    public class ValveBindingItemViewModel : ReactiveObject
    {
        private readonly Action<ValveBindingItemViewModel> removeAction;

        /// <summary>
        /// 修改后事件
        /// </summary>
        public event EventHandler AfterModified;

        /// <summary>
        /// 删除命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; }

        /// <summary>
        /// 阀实例ID控件模型
        /// </summary>
        public TextInputViewModel ValveIDViewModel { get; } = new();

        /// <summary>
        /// 别名控件模型
        /// </summary>
        public TextInputViewModel AliasViewModel { get; } = new();

        /// <summary>
        /// 构造函数
        /// </summary>
        public ValveBindingItemViewModel()
            : this(null)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="removeAction">删除动作</param>
        public ValveBindingItemViewModel(Action<ValveBindingItemViewModel> removeAction)
        {
            this.removeAction = removeAction;
            RemoveCommand = ReactiveCommand.Create(Remove);
            ValveIDViewModel.ValueChanged += OnChildValueChanged;
            AliasViewModel.ValueChanged += OnChildValueChanged;
        }

        /// <summary>
        /// 阀实例ID
        /// </summary>
        public string ValveID
        {
            get => ValveIDViewModel.Text;
            set => ValveIDViewModel.Text = value ?? string.Empty;
        }

        /// <summary>
        /// 阀别名
        /// </summary>
        public string Alias
        {
            get => AliasViewModel.Text;
            set => AliasViewModel.Text = value ?? string.Empty;
        }

        private void Remove()
        {
            removeAction?.Invoke(this);
        }

        private void OnChildValueChanged(object sender, EventArgs e)
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }
    }
}
