using GF_Gereric;
using GKG.SubMM;
using GKG.UI;
using GKG;
using Griffins;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using Avalonia.Threading;
using Avalonia.Media;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage.ViewModels
{
    public class AxisDebugWindowViewModel : ReactiveObject
    {
        private const string ParamKey_CardGuid = "CardGuid";
        private const string ParamKey_AxisName = "AxisName";
        private const string ParamKey_AxisIndex = "AxisIndex";
        private const string ParamKey_Axis = "Axis";
        private const string ParamKey_IoPortNo = "IOPortNo";

        private const string ResultKey_Data = "data";
        private const string ResultKey_Result = "Result";

        private const string ParamKey_IoChannel = "IOChannel";
        private const string ParamKey_Value = "Value";

        private const string ParamKey_SelectedAxisIndex = "SelectedAxisIndex";
        private const string ParamKey_MotionMode = "MotionMode";
        private const string ParamKey_RotationDirection = "RotationDirection";
        private const string ParamKey_Velocity = "Velocity";
        private const string ParamKey_Acceleration = "Acceleration";
        private const string ParamKey_Distance = "Distance";

        private readonly ICompUIRunTimeCallBack _callBack;
        private readonly IReadOnlyList<AxisDescriptor> _configuredAxes;
        private readonly IReadOnlyList<IoDescriptor> _configuredIos;

        private DispatcherTimer? _ioInRefreshTimer;
        private DispatcherTimer? _axisStatusRefreshTimer;
        private bool _disposed;
        private bool _pollingActive;
        private int _attachedViewCount;
        private readonly AxisStatusItemViewModel _axisSelectionPlaceholder = new AxisStatusItemViewModel("[请选择轴]", string.Empty);

        public Guid CardGuid { get; }
        public int AxisCount { get; }

        private ObservableCollection<AxisStatusItemViewModel> _axisStatusList = new ObservableCollection<AxisStatusItemViewModel>();
        public ObservableCollection<AxisStatusItemViewModel> AxisStatusList
        {
            get => _axisStatusList;
            set
            {
                if (ReferenceEquals(_axisStatusList, value))
                    return;
                this.RaiseAndSetIfChanged(ref _axisStatusList, value);
            }

        }
        public ObservableCollection<AxisStatusItemViewModel> AxisSelectionList { get; } = new ObservableCollection<AxisStatusItemViewModel>();

        private static string getResultData(GFBaseTypeParamValueList? result)
        {
            var s = TryGetParamString(result, ResultKey_Data);
            return string.IsNullOrWhiteSpace(s) ? string.Empty : s;
        }

        private AxisStatusItemViewModel? _selectedAxisStatus;
        public AxisStatusItemViewModel? SelectedAxisStatus
        {
            get => _selectedAxisStatus;
            set
            {
                JogStop();
                if (_selectedAxisStatus != null)
                    _selectedAxisStatus.PropertyChanged -= SelectedAxisStatus_PropertyChanged;

                this.RaiseAndSetIfChanged(ref _selectedAxisStatus, value);

                if (_selectedAxisStatus != null)
                    _selectedAxisStatus.PropertyChanged += SelectedAxisStatus_PropertyChanged;

                foreach (var axis in AxisStatusList)
                    axis.IsSelected = ReferenceEquals(axis, value);
                refreshIoItemsForSelectedAxis();
                refreshSingleAxisRuntime(value);
                this.RaisePropertyChanged(nameof(CanMove));
                this.RaisePropertyChanged(nameof(HasSelectedAxis));
                this.RaisePropertyChanged(nameof(CanAxisCommand));
                this.RaisePropertyChanged(nameof(AxisSelectionHint));
            }
        }

        private void SelectedAxisStatus_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxisStatusItemViewModel.IsEnabled) ||
                e.PropertyName == nameof(AxisStatusItemViewModel.IsAlarm))
            {
                this.RaisePropertyChanged(nameof(CanMove));
                this.RaisePropertyChanged(nameof(CanAxisCommand));
            }
        }

        private MotionMode _motionMode = MotionMode.Jog;
        public bool IsJogMode
        {
            get => _motionMode == MotionMode.Jog;
            set
            {
                if (!value) return;
                JogStop();
                _motionMode = MotionMode.Jog;
                this.RaisePropertyChanged(nameof(IsJogMode));
                this.RaisePropertyChanged(nameof(IsPointMode));
            }
        }

        public bool IsPointMode
        {
            get => _motionMode == MotionMode.Point;
            set
            {
                if (!value) return;
                JogStop();
                _motionMode = MotionMode.Point;
                this.RaisePropertyChanged(nameof(IsJogMode));
                this.RaisePropertyChanged(nameof(IsPointMode));
            }
        }

        private RotationDirection _rotationDirection = RotationDirection.CW;
        public bool IsCw
        {
            get => _rotationDirection == RotationDirection.CW;
            set
            {
                if (!value) return;
                _rotationDirection = RotationDirection.CW;
                this.RaisePropertyChanged(nameof(IsCw));
                this.RaisePropertyChanged(nameof(IsCcw));
            }
        }

        public bool IsCcw
        {
            get => _rotationDirection == RotationDirection.CCW;
            set
            {
                if (!value) return;
                _rotationDirection = RotationDirection.CCW;
                this.RaisePropertyChanged(nameof(IsCw));
                this.RaisePropertyChanged(nameof(IsCcw));
            }
        }

        private string _velocity = "1";
        public string Velocity
        {
            get => _velocity;
            set => this.RaiseAndSetIfChanged(ref _velocity, value);
        }

        private string _acceleration = "1000";
        public string Acceleration
        {
            get => _acceleration;
            set => this.RaiseAndSetIfChanged(ref _acceleration, value);
        }

        private string _distance = "1";
        public string Distance
        {
            get => _distance;
            set => this.RaiseAndSetIfChanged(ref _distance, value);
        }

        public TextInputViewModel VelocityInputViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel AccelerationInputViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel DistanceInputViewModel { get; } = new TextInputViewModel();
        public ComboxViewModel AxisSelectionComboxViewModel { get; } = new ComboxViewModel();

        private bool _readOnly;
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                JogStop();
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                this.RaisePropertyChanged(nameof(CanMove));
                this.RaisePropertyChanged(nameof(CanAxisCommand));
            }
        }

        public bool CanMove
        {
            get
            {
                if (ReadOnly) return false;
                var axis = getSelectedAxis();
                if (axis == null) return false;
                if (axis.IsAlarm) return false;
                return axis.IsEnabled;
            }
        }

        public bool HasSelectedAxis => SelectedAxisStatus != null;

        public bool HasAvailableAxis => AxisStatusList.Count > 0;

        public bool CanAxisCommand => HasSelectedAxis && !ReadOnly;

        public string AxisSelectionHint => HasSelectedAxis ? string.Empty : "请先选择轴";

        public ReactiveCommand<Unit, Unit> MoveCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
        public ReactiveCommand<Unit, Unit> EnableCommand { get; }
        public ReactiveCommand<Unit, Unit> DisableCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearStatusCommand { get; }
        public ReactiveCommand<Unit, Unit> HomeCommand { get; }
        public ReactiveCommand<Unit, Unit> JogToggleCommand { get; }
        private DispatcherTimer? _jogTimer;
        private bool _isJogging;

        public bool IsJogging => _isJogging;
        public string JogButtonText => _isJogging ? "停止" : "运动";

        public AxisDebugWindowViewModel(
            ICompUIRunTimeCallBack callBack,
            Guid cardGuid,
            int axisCount,
            int initialAxisIndex,
            IReadOnlyList<AxisDescriptor>? configuredAxes = null,
            IReadOnlyList<IoDescriptor>? configuredIos = null)
        {
            _callBack = callBack;
            CardGuid = cardGuid;
            _configuredAxes = configuredAxes ?? Array.Empty<AxisDescriptor>();
            _configuredIos = configuredIos ?? Array.Empty<IoDescriptor>();

            AxisCount = axisCount > 0 ? axisCount : _configuredAxes.Count;

            MoveCommand = ReactiveCommand.Create(move, this.WhenAnyValue(x => x.CanMove));
            StopCommand = ReactiveCommand.Create(stop, this.WhenAnyValue(x => x.CanAxisCommand));
            EnableCommand = ReactiveCommand.Create(enableServo, this.WhenAnyValue(x => x.CanAxisCommand));
            DisableCommand = ReactiveCommand.Create(disableServo, this.WhenAnyValue(x => x.CanAxisCommand));
            ClearStatusCommand = ReactiveCommand.Create(clearStatus, this.WhenAnyValue(x => x.CanAxisCommand));
            HomeCommand = ReactiveCommand.Create(home, this.WhenAnyValue(x => x.CanMove));
            JogToggleCommand = ReactiveCommand.Create(toggleJog, this.WhenAnyValue(x => x.CanMove));

            RefreshIOInCommand = ReactiveCommand.Create(refreshIoIn);
            initAxisStatusList(initialAxisIndex);
            wireAxisAndMotionInputs();

            initIoInItems();
            initIoOutItems();
        }

        public void OnViewAttached()
        {
            if (_disposed)
            {
                return;
            }

            if (++_attachedViewCount == 1)
            {
                StartPolling();
            }
        }

        public void OnViewDetached()
        {
            if (_attachedViewCount > 0 && --_attachedViewCount == 0)
            {
                Cleanup();
            }
        }

        public void StartPolling()
        {
            if (_disposed || _pollingActive)
            {
                return;
            }

            _pollingActive = true;
            startIoInAutoRefresh();
            refreshIoIn();
            startAxisStatusAutoRefresh();
        }

        public void Cleanup()
        {
            if (!_pollingActive)
            {
                return;
            }

            _pollingActive = false;
            stopIoInAutoRefresh();
            stopAxisStatusAutoRefresh();
            JogStop();
        }

        private void wireAxisAndMotionInputs()
        {
            VelocityInputViewModel.Text = Velocity;
            VelocityInputViewModel.ValueChanged += (_, __) => Velocity = VelocityInputViewModel.Text ?? string.Empty;
            this.WhenAnyValue(x => x.Velocity).Subscribe(v =>
            {
                var t = v ?? string.Empty;
                if (VelocityInputViewModel.Text != t)
                    VelocityInputViewModel.Text = t;
            });

            AccelerationInputViewModel.Text = Acceleration;
            AccelerationInputViewModel.ValueChanged += (_, __) => Acceleration = AccelerationInputViewModel.Text ?? string.Empty;
            this.WhenAnyValue(x => x.Acceleration).Subscribe(v =>
            {
                var t = v ?? string.Empty;
                if (AccelerationInputViewModel.Text != t)
                    AccelerationInputViewModel.Text = t;
            });

            DistanceInputViewModel.Text = Distance;
            DistanceInputViewModel.ValueChanged += (_, __) => Distance = DistanceInputViewModel.Text ?? string.Empty;
            this.WhenAnyValue(x => x.Distance).Subscribe(v =>
            {
                var t = v ?? string.Empty;
                if (DistanceInputViewModel.Text != t)
                    DistanceInputViewModel.Text = t;
            });

            AxisSelectionComboxViewModel.DisplayMemberPath = nameof(AxisStatusItemViewModel.AxisName);
            AxisSelectionComboxViewModel.ItemsSource = AxisSelectionList;
            AxisSelectionComboxViewModel.ValueChanged += (_, e) =>
            {
                if (e.NewValue is AxisStatusItemViewModel ax)
                {
                    if (ReferenceEquals(ax, _axisSelectionPlaceholder))
                    {
                        SelectedAxisStatus = null;
                        return;
                    }

                    SelectedAxisStatus = ax;
                }
            };
            this.WhenAnyValue(x => x.SelectedAxisStatus).Subscribe(ax =>
            {
                if (ax != null && !ReferenceEquals(AxisSelectionComboxViewModel.SelectedItem, ax))
                    AxisSelectionComboxViewModel.SelectedItem = ax;
            });
            AxisSelectionComboxViewModel.SelectedItem = SelectedAxisStatus ?? _axisSelectionPlaceholder;
        }

        private Guid GetActiveCardGuid()
        {
            return getSelectedAxis()?.CardGuid ?? CardGuid;
        }

        public ReactiveCommand<Unit, Unit> RefreshIOInCommand { get; }

        private bool _isTrackBVisible;
        public bool IsTrackBVisible
        {
            get => _isTrackBVisible;
            set => this.RaiseAndSetIfChanged(ref _isTrackBVisible, value);
        }

        public ObservableCollection<IOInItemViewModel> CommonSensors { get; } = new ObservableCollection<IOInItemViewModel>();
        public ObservableCollection<IOInItemViewModel> TrackASensors { get; } = new ObservableCollection<IOInItemViewModel>();
        public ObservableCollection<IOInItemViewModel> TrackBSensors { get; } = new ObservableCollection<IOInItemViewModel>();

        public ObservableCollection<IOOutItemViewModel> IOOutColumn1Items { get; } = new ObservableCollection<IOOutItemViewModel>();
        public ObservableCollection<IOOutItemViewModel> IOOutColumn2Items { get; } = new ObservableCollection<IOOutItemViewModel>();
        public ObservableCollection<IOOutItemViewModel> IOOutColumn3Items { get; } = new ObservableCollection<IOOutItemViewModel>();

        private void initIoInItems()
        {
            IsTrackBVisible = false;

            CommonSensors.Clear();
            TrackASensors.Clear();
            TrackBSensors.Clear();

            var activeCardGuid = GetActiveCardGuid();
            var configuredNames = _configuredIos
                .Where(x => x.CardGuid == Guid.Empty || activeCardGuid == Guid.Empty || x.CardGuid == activeCardGuid)
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .ToList();

            if (configuredNames.Count == 0)
                return;

            for (int i = 0; i < configuredNames.Count; i++)
            {
                var item = new IOInItemViewModel(configuredNames[i].ChannelNo, configuredNames[i].Name, false, configuredNames[i].CanWrite, configuredNames[i].ChannelNo);
                item.AfterToggled += (_, __) => onIoInToggled(item);
                if (IsTrackAName(item.Name))
                {
                    TrackASensors.Add(item);
                }
                else if (IsTrackBName(item.Name))
                {
                    TrackBSensors.Add(item);
                }
                else
                {
                    CommonSensors.Add(item);
                }
            }

            IsTrackBVisible = TrackBSensors.Count > 0;
        }

        private void refreshIoItemsForSelectedAxis()
        {
            initIoInItems();
            initIoOutItems();
        }

        private void startIoInAutoRefresh()
        {
            if (_disposed || !_pollingActive)
            {
                return;
            }

            if (_ioInRefreshTimer == null)
            {
                _ioInRefreshTimer = new DispatcherTimer();
                _ioInRefreshTimer.Interval = TimeSpan.FromMilliseconds(500);
                _ioInRefreshTimer.Tick += (_, __) =>
                {
                    if (!_pollingActive || _disposed)
                    {
                        return;
                    }

                    refreshIoIn();
                };
            }

            if (!_ioInRefreshTimer.IsEnabled)
                _ioInRefreshTimer.Start();
        }

        private void startAxisStatusAutoRefresh()
        {
            if (_disposed || !_pollingActive)
            {
                return;
            }

            if (_axisStatusRefreshTimer == null)
            {
                _axisStatusRefreshTimer = new DispatcherTimer();
                _axisStatusRefreshTimer.Interval = TimeSpan.FromMilliseconds(500);
                _axisStatusRefreshTimer.Tick += (_, __) =>
                {
                    if (!_pollingActive || _disposed)
                    {
                        return;
                    }

                    refreshAllAxisRuntime();
                };
            }

            _axisStatusRefreshTimer.Start();
            refreshAllAxisRuntime();
        }

        private void stopIoInAutoRefresh()
        {
            _ioInRefreshTimer?.Stop();
        }

        private void stopAxisStatusAutoRefresh()
        {
            _axisStatusRefreshTimer?.Stop();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _attachedViewCount = 0;
            Cleanup();

            if (_selectedAxisStatus != null)
            {
                _selectedAxisStatus.PropertyChanged -= SelectedAxisStatus_PropertyChanged;
            }
        }

        private void refreshIoIn()
        {
            if (!_pollingActive || _disposed)
            {
                return;
            }

            refreshIoInList(CommonSensors);
            refreshIoInList(TrackASensors);
            refreshIoInList(TrackBSensors);
        }

        private void refreshIoInList(ObservableCollection<IOInItemViewModel> list)
        {
            foreach (var item in list)
            {
                refreshSingleIoState(item);
            }
        }

        private void applyIoInResult(ObservableCollection<IOInItemViewModel> list, GFBaseTypeParamValueList result)
        {
            foreach (var item in list)
            {
                var key = $"Ch{item.Channel}";
                var s = TryGetParamString(result, key);
                if (string.IsNullOrWhiteSpace(s))
                    continue;

                if (bool.TryParse(s, out var b))
                    item.UpdateStateFromBackend(b);
                else if (int.TryParse(s, out var i))
                    item.UpdateStateFromBackend(i != 0);
            }
        }

        private void onIoInToggled(IOInItemViewModel item)
        {
            if (item == null || !item.CanWrite)
                return;

            var axis = getSelectedAxis();
            var axisIndex = getSelectedAxisIndex();
            if (axis == null || axisIndex < 0)
                return;

            var p = buildCommonCardAxisParam(axis, axisIndex);
            p[ParamKey_AxisIndex] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            p[ParamKey_Axis] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            p[ParamKey_IoChannel] = new GriffinsBaseValue((item.IOChannel ?? item.Channel).ToString(CultureInfo.InvariantCulture));
            p[ParamKey_Value] = new GriffinsBaseValue(item.IsOn.ToString());
            Exec(EletronicManagerSubMachineModulesConst.RtCmdSetOutputState, p);
            refreshSingleIoState(item);
        }

        private void refreshSingleIoState(IOInItemViewModel item)
        {
            if (item == null)
                return;

            var axis = getSelectedAxis();
            var axisIndex = getSelectedAxisIndex();
            if (axis == null || axisIndex < 0)
                return;

            var p = buildCommonCardAxisParam(axis, axisIndex);
            p[ParamKey_AxisIndex] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            p[ParamKey_Axis] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            var ioCh = item.IOChannel ?? item.Channel;
            p[ParamKey_IoChannel] = new GriffinsBaseValue(ioCh.ToString(CultureInfo.InvariantCulture));

            var result = ExecWithResult(EletronicManagerSubMachineModulesConst.RtCmdGetInOutputState, p);
            if (result == null)
                return;

            var dataText = getResultData(result);
            if (string.IsNullOrWhiteSpace(dataText))
                return;

            if (bool.TryParse(dataText, out var b))
                item.UpdateStateFromBackend(b);
            else if (int.TryParse(dataText, out var i))
                item.UpdateStateFromBackend(i != 0);
        }

        private static string? TryGetParamString(GFBaseTypeParamValueList param, string key)
        {
            if (param == null || string.IsNullOrWhiteSpace(key))
                return null;

            var v = param[key];
            return v?.ToString();
        }

        private void initIoOutItems()
        {
            IOOutColumn1Items.Clear();
            IOOutColumn2Items.Clear();
            IOOutColumn3Items.Clear();

            var names = new[]
            {
                "红灯",
                "蜂鸣器",
                "黄灯",
                "绿灯",
                "(前门)安全门气缸锁",
                "(后门)安全门气缸锁",
                "驱动器电源",
                "胶水气压",
                "右阀喷胶",
                "左阀喷胶",
                "胶阀气压",
                "真空",
                "右阀",
                "左阀",
                "机器运行灯",

                "(A轨)左挡板气缸",
                "(A轨)右挡板气缸",
                "(A轨)出料口挡板气缸",
                "(A轨)顶板气缸",
                "(A轨)顶板气缸2",
                "(A轨)顶板气缸3",
                "(A轨)本机真空阀",
                "(A轨)本机右挡板",
                "(A轨)左加热吹气",
                "(A轨)中加热吹气",
                "(A轨)右加热吹气",
                "(A轨)侧夹气缸",
                "(A轨)X向盖板保护信号",
                "(A轨)Z向盖板保护信号",
                "(A轨)平台真空阀",
                "(A轨)等待位顶板气缸",

                "(B轨)左挡板气缸",
                "(B轨)右挡板气缸",
                "(B轨)出料口挡板气缸",
                "(B轨)顶板气缸",
                "(B轨)顶板气缸2",
                "(B轨)顶板气缸3",
                "(B轨)本机真空阀",
                "(B轨)本机右挡板",
                "(B轨)左加热吹气",
                "(B轨)中加热吹气",
                "(B轨)右加热吹气",
                "(B轨)侧夹气缸",
                "(B轨)X向盖板保护信号",
                "(B轨)Z向盖板保护信号",
                "(B轨)平台真空阀",
                "(B轨)等待位顶板气缸",
            };

            for (int i = 0; i < names.Length; i++)
            {
                var id = $"DO{i + 1}";
                var name = names[i];
                int? ioChannel = string.Equals(name, "红灯", StringComparison.OrdinalIgnoreCase) ? 0 : null;
                var itemVm = new IOOutItemViewModel(id, name, false, true, IOChannel: ioChannel);
                itemVm.AfterToggled += (_, __) => onIoOutToggled(itemVm);

                if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("(A轨)", StringComparison.OrdinalIgnoreCase))
                {
                    IOOutColumn2Items.Add(itemVm);
                }
                else if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("(B轨)", StringComparison.OrdinalIgnoreCase))
                {
                    IOOutColumn3Items.Add(itemVm);
                }
                else
                {
                    IOOutColumn1Items.Add(itemVm);
                }
            }
        }

        private void onIoOutToggled(IOOutItemViewModel item)
        {
            if (item == null)
                return;

            var axis = getSelectedAxis();
            var axisIndex = getSelectedAxisIndex();
            if (axis == null || axisIndex < 0)
                return;

            var p = buildCommonCardAxisParam(axis, axisIndex);
            p[ParamKey_AxisIndex] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            p[ParamKey_Axis] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            p[ParamKey_IoChannel] = new GriffinsBaseValue(getIoOutChannel(item).ToString(CultureInfo.InvariantCulture));
            p[ParamKey_Value] = new GriffinsBaseValue(item.IsOn.ToString());
            Exec(EletronicManagerSubMachineModulesConst.RtCmdSetOutputState, p);
        }

        /// <summary>硬件 IO 通道：显式 IOChannel 优先，否则由 DO 编号推导（DO1->0）。</summary>
        private static int getIoOutChannel(IOOutItemViewModel item)
        {
            if (item.IOChannel.HasValue)
                return item.IOChannel.Value;
            var id = item.Id ?? string.Empty;
            if (id.Length >= 3 && id.StartsWith("DO", StringComparison.OrdinalIgnoreCase))
            {
                var suffix = id.Substring(2);
                if (int.TryParse(suffix, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) && n >= 1)
                    return n - 1;
            }
            return 0;
        }

        private void initAxisStatusList(int initialAxisIndex)
        {
            AxisStatusList = new ObservableCollection<AxisStatusItemViewModel>();
            if (_configuredAxes.Count > 0)
            {
                foreach (var info in _configuredAxes)
                {
                    AxisStatusList.Add(new AxisStatusItemViewModel(info.Name, info.Name)
                    {
                        CardGuid = info.CardGuid,
                        BackendAxisNo = info.AxisNo,
                        Position = 0,
                        IsEnabled = false,
                    });
                }
            }
            if (initialAxisIndex < 0)
                initialAxisIndex = 0;
            AxisSelectionList.Clear();
            AxisSelectionList.Add(_axisSelectionPlaceholder);
            foreach (var axis in AxisStatusList)
                AxisSelectionList.Add(axis);

            this.RaisePropertyChanged(nameof(HasAvailableAxis));
            SelectedAxisStatus = ResolveInitialAxis(initialAxisIndex);
        }

        private AxisStatusItemViewModel? ResolveInitialAxis(int initialAxisIndex)
        {
            if (AxisStatusList.Count == 0)
                return null;

            var matchedAxis = AxisStatusList.FirstOrDefault(x => x.BackendAxisNo == initialAxisIndex);
            return matchedAxis ?? AxisStatusList.FirstOrDefault();
        }

        private AxisStatusItemViewModel? getSelectedAxis()
        {
            return SelectedAxisStatus;
        }

        private int getSelectedAxisIndex()
        {
            var axis = getSelectedAxis();
            if (axis == null) return -1;
            return axis.BackendAxisNo >= 0 ? axis.BackendAxisNo : AxisStatusList.IndexOf(axis);
        }

        private string getSelectedAxisKey()
        {
            var axis = getSelectedAxis();
            return axis?.AxisKey ?? string.Empty;
        }

        private void move()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;

            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;

            var dir = _rotationDirection == RotationDirection.CW ? 1 : -1;
            var dist = parseDoubleOrDefault(Distance, 0);
            var maxSpeed = parseDoubleOrDefault(Velocity, 1);
            refreshSingleAxisPosition(axis);

            Exec(EletronicManagerSubMachineModulesConst.RtCmdAbsoluteMove, buildAbsoluteMoveParam(axisIndex, axis.Position + dir * dist, maxSpeed));
            refreshSingleAxisRuntime(axis);
            this.RaisePropertyChanged(nameof(CanMove));
        }

        private void stop()
        {
            JogStop();
            var axis = getSelectedAxis();
            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;
            Exec(EletronicManagerSubMachineModulesConst.RtCmdAxisStop, buildAxisStopParam(axisIndex));
            refreshSingleAxisRuntime(axis);
        }

        private void enableServo()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;

            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;

            Exec(EletronicManagerSubMachineModulesConst.RtCmdServoOn, buildServoOnParam(axisIndex, true));
            refreshSingleAxisRuntime(axis);
            this.RaisePropertyChanged(nameof(CanMove));
        }

        private void disableServo()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;

            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;

            Exec(EletronicManagerSubMachineModulesConst.RtCmdServoOn, buildServoOnParam(axisIndex, false));
            refreshSingleAxisRuntime(axis);
            this.RaisePropertyChanged(nameof(CanMove));
        }

        private void clearStatus()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;
            JogStop();

            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;
            Exec(EletronicManagerSubMachineModulesConst.RtCmdClearAxisAlarm, buildCommonCardAxisParam(axis, axisIndex));
            refreshSingleAxisRuntime(axis);
            this.RaisePropertyChanged(nameof(CanMove));
        }

        private void home()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;
            JogStop();

            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;
            Exec(EletronicManagerSubMachineModulesConst.RtCmdHomed, buildCommonCardAxisParam(axis, axisIndex));
            refreshSingleAxisRuntime(axis);
        }

        private void toggleJog()
        {
            if (!IsJogMode) return;
            var axis = getSelectedAxis();
            if (axis == null) return;

            if (_isJogging)
            {
                JogStop();
                return;
            }

            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;

            var dir = _rotationDirection == RotationDirection.CW ? 1 : -1;
            var maxSpeed = parseDoubleOrDefault(Velocity, 1) * dir;
            Exec(EletronicManagerSubMachineModulesConst.RtCmdJogMove, buildJogMoveParam(axisIndex, maxSpeed));

            _isJogging = true;
            this.RaisePropertyChanged(nameof(IsJogging));
            this.RaisePropertyChanged(nameof(JogButtonText));

            if (_jogTimer == null)
            {
                _jogTimer = new DispatcherTimer();
                _jogTimer.Interval = TimeSpan.FromMilliseconds(50);
                _jogTimer.Tick += (_, __) => jogTick();
            }
            _jogTimer.Start();
        }

        private void jogTick()
        {
            if (!_isJogging) return;
            var axis = getSelectedAxis();
            if (axis == null) return;
            refreshSingleAxisRuntime(axis);
        }

        public void JogStop()
        {
            var axis = getSelectedAxis();
            if (_isJogging)
            {
                var axisIndex = getSelectedAxisIndex();
                if (axisIndex >= 0)
                {
                    Exec(EletronicManagerSubMachineModulesConst.RtCmdAxisStop, buildAxisStopParam(axisIndex));
                }
            }

            _isJogging = false;
            this.RaisePropertyChanged(nameof(IsJogging));
            this.RaisePropertyChanged(nameof(JogButtonText));
            if (_jogTimer != null)
            {
                _jogTimer.Stop();
            }
            refreshSingleAxisRuntime(axis);
        }

        private void refreshSingleAxisPosition(AxisStatusItemViewModel? axis)
        {
            if (axis == null)
                return;

            var axisIndex = axis.BackendAxisNo >= 0 ? axis.BackendAxisNo : AxisStatusList.IndexOf(axis);
            if (axisIndex < 0)
                return;

            var p = buildCommonCardAxisParam(axis, axisIndex);
            p["PositionType"] = new GriffinsBaseValue("Command");

            var result = ExecWithResult(EletronicManagerSubMachineModulesConst.RtCmdGetAxisPos, p);
            if (result == null)
                return;

            var dataText = getResultData(result);
            if (string.IsNullOrWhiteSpace(dataText))
                return;

            if (double.TryParse(dataText, NumberStyles.Float, CultureInfo.InvariantCulture, out var pos) ||
                double.TryParse(dataText, out pos))
            {
                axis.Position = pos;
            }
        }

        private void refreshAllAxisRuntime()
        {
            if (!_pollingActive || _disposed)
            {
                return;
            }

            foreach (var axis in AxisStatusList)
                refreshSingleAxisRuntime(axis);
        }

        private void refreshSingleAxisRuntime(AxisStatusItemViewModel? axis)
        {
            if (axis == null)
                return;

            refreshSingleAxisPosition(axis);
            refreshSingleAxisStates(axis);
        }

        private void refreshSingleAxisStates(AxisStatusItemViewModel? axis)
        {
            if (axis == null)
                return;

            var axisIndex = axis.BackendAxisNo >= 0 ? axis.BackendAxisNo : AxisStatusList.IndexOf(axis);
            if (axisIndex < 0)
                return;

            var p = buildCommonCardAxisParam(axis, axisIndex);
            var result = ExecWithResult(EletronicManagerSubMachineModulesConst.RtCmdGetAxisStates, p);
            if (!isRuntimeCmdSuccess(result))
                return;

            var dataText = getResultData(result);
            if (string.IsNullOrWhiteSpace(dataText))
                return;

            var parts = dataText.Split(',');
            if (parts.Length < 5)
                return;

            axis.IsPosLimit = parseAxisStateValue(parts[0]);
            axis.IsHome = parseAxisStateValue(parts[1]);
            axis.IsNegLimit = parseAxisStateValue(parts[2]);
            axis.IsAlarm = parseAxisStateValue(parts[3]);
            axis.IsEnabled = parseAxisStateValue(parts[4]);
        }

        private static bool parseAxisStateValue(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (bool.TryParse(text, out var boolValue))
                return boolValue;

            return int.TryParse(text, out var intValue) && intValue != 0;
        }

        private static bool isRuntimeCmdSuccess(GFBaseTypeParamValueList? result)
        {
            if (result == null)
                return false;

            var code = TryGetParamString(result, ResultKey_Result);
            return string.IsNullOrWhiteSpace(code) ||
                   string.Equals(code, "0", StringComparison.OrdinalIgnoreCase);
        }

        private void Exec(string cmdId, GFBaseTypeParamValueList cmdParam)
        {
            cmdParam = EnrichCmdParamWithUiSnapshot(cmdParam);
            _callBack.ExecConfigSvrCtlCmd(cmdId, cmdParam);
        }

        private GFBaseTypeParamValueList? ExecWithResult(string cmdId, GFBaseTypeParamValueList cmdParam)
        {
            cmdParam = EnrichCmdParamWithUiSnapshot(cmdParam);
            return _callBack.ExecConfigSvrCtlCmd(cmdId, cmdParam);
        }

        private GFBaseTypeParamValueList EnrichCmdParamWithUiSnapshot(GFBaseTypeParamValueList cmdParam)
        {
            if (cmdParam == null)
                cmdParam = new GFBaseTypeParamValueList();

            var axisIndex = getSelectedAxisIndex();
            var axisKey = getSelectedAxisKey();

            if (TryGetParamString(cmdParam, ParamKey_AxisIndex) == null)
            {
                var backendAxisIndex = getBackendAxisIndexForCmd();
                cmdParam[ParamKey_AxisIndex] = new GriffinsBaseValue(backendAxisIndex.ToString(CultureInfo.InvariantCulture));
                cmdParam[ParamKey_Axis] = new GriffinsBaseValue(backendAxisIndex.ToString(CultureInfo.InvariantCulture));
            }

            cmdParam[ParamKey_SelectedAxisIndex] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            cmdParam[ParamKey_AxisName] = new GriffinsBaseValue(axisKey);
            cmdParam[ParamKey_IoPortNo] = new GriffinsBaseValue("0");
            cmdParam[ParamKey_MotionMode] = new GriffinsBaseValue(IsJogMode ? "Jog" : "Point");
            cmdParam[ParamKey_RotationDirection] = new GriffinsBaseValue(_rotationDirection == RotationDirection.CW ? "CW" : "CCW");
            cmdParam[ParamKey_Velocity] = new GriffinsBaseValue(Velocity ?? string.Empty);
            cmdParam[ParamKey_Acceleration] = new GriffinsBaseValue(Acceleration ?? string.Empty);
            cmdParam[ParamKey_Distance] = new GriffinsBaseValue(Distance ?? string.Empty);

            return cmdParam;
        }

        private int getBackendAxisIndexForCmd()
        {
            return getSelectedAxisIndex();
        }

        private GFBaseTypeParamValueList buildCommonCardAxisParam(AxisStatusItemViewModel? axis, int axisIndex)
        {
            var p = new GFBaseTypeParamValueList();
            p[ParamKey_CardGuid] = new GriffinsBaseValue((axis?.CardGuid ?? CardGuid).ToString());
            p[ParamKey_AxisName] = new GriffinsBaseValue(axis?.AxisKey ?? string.Empty);
            p[ParamKey_AxisIndex] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            p[ParamKey_Axis] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            p[ParamKey_IoPortNo] = new GriffinsBaseValue("0");
            return p;
        }

        private GFBaseTypeParamValueList buildAbsoluteMoveParam(int axisIndex, double pos, double maxSpeed)
        {
            var p = buildCommonCardAxisParam(getSelectedAxis(), axisIndex);
            p["MotionType"] = new GriffinsBaseValue("0");
            p["Pos"] = new GriffinsBaseValue(pos.ToString(CultureInfo.InvariantCulture));
            p["StartSpeed"] = new GriffinsBaseValue("0");
            p["MaxSpeed"] = new GriffinsBaseValue(maxSpeed.ToString(CultureInfo.InvariantCulture));
            p["AccTimeT"] = new GriffinsBaseValue("0");
            p["DecTimeT"] = new GriffinsBaseValue("0");
            p["AccTimeS"] = new GriffinsBaseValue("0");
            p["DecTimeS"] = new GriffinsBaseValue("0");
            return p;
        }

        private GFBaseTypeParamValueList buildJogMoveParam(int axisIndex, double maxSpeedSigned)
        {
            var p = buildCommonCardAxisParam(getSelectedAxis(), axisIndex);
            p["MotionType"] = new GriffinsBaseValue("0");
            p["StartSpeed"] = new GriffinsBaseValue("0");
            p["MaxSpeed"] = new GriffinsBaseValue(maxSpeedSigned.ToString(CultureInfo.InvariantCulture));
            p["AccTimeT"] = new GriffinsBaseValue("0");
            p["DecTimeT"] = new GriffinsBaseValue("0");
            p["AccTimeS"] = new GriffinsBaseValue("0");
            p["DecTimeS"] = new GriffinsBaseValue("0");
            return p;
        }

        private GFBaseTypeParamValueList buildAxisStopParam(int axisIndex)
        {
            var p = buildCommonCardAxisParam(getSelectedAxis(), axisIndex);
            p["StopType"] = new GriffinsBaseValue(((int)GKG.MotionControlAxisStopTypeConstants.DecelerationStop).ToString(CultureInfo.InvariantCulture));
            return p;
        }

        private GFBaseTypeParamValueList buildServoOnParam(int axisIndex, bool isEnabled)
        {
            var cmd = buildCommonCardAxisParam(getSelectedAxis(), axisIndex);
            cmd["IsEnabled"] = new GriffinsBaseValue(isEnabled.ToString());
            return cmd;
        }

        private double parseDoubleOrDefault(string text, double defaultValue)
        {
            if (double.TryParse(text?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                return v;
            if (double.TryParse(text?.Trim(), out v))
                return v;
            return defaultValue;
        }

        private static bool IsTrackAName(string? name)
        {
            return !string.IsNullOrWhiteSpace(name) &&
                   (name.StartsWith("(A轨)", StringComparison.OrdinalIgnoreCase) ||
                    name.StartsWith("A轨", StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsTrackBName(string? name)
        {
            return !string.IsNullOrWhiteSpace(name) &&
                   (name.StartsWith("(B轨)", StringComparison.OrdinalIgnoreCase) ||
                    name.StartsWith("B轨", StringComparison.OrdinalIgnoreCase));
        }

        public class IOInItemViewModel : ReactiveObject
        {
            private bool _suppressToggleEvent;

            public string Channel { get; }
            public string Name { get; }
            public bool CanWrite { get; }

            public string? IOChannel { get; }

            private bool _isOn;
            public bool IsOn
            {
                get => _isOn;
                set
                {
                    this.RaiseAndSetIfChanged(ref _isOn, value);
                    this.RaisePropertyChanged(nameof(StatusColor));
                    if (!_suppressToggleEvent)
                        AfterToggled?.Invoke(this, EventArgs.Empty);
                }
            }

            public IBrush StatusColor => IsOn ? Brushes.LightGreen : Brushes.LightGray;

            public event EventHandler? AfterToggled;

            public IOInItemViewModel(string channel, string name, bool isOn, bool canWrite, string? IOChannel = null)
            {
                Channel = channel;
                Name = name;
                CanWrite = canWrite;
                this.IOChannel = IOChannel;
                _isOn = isOn;
            }

            public void UpdateStateFromBackend(bool isOn)
            {
                _suppressToggleEvent = true;
                try
                {
                    IsOn = isOn;
                }
                finally
                {
                    _suppressToggleEvent = false;
                }
            }
        }

        public class IOOutItemViewModel : ReactiveObject
        {
            private bool _isOn;
            private bool _isEnabled = true;
            private bool _isSelected;

            public string Id { get; }
            public string Name { get; }

            public bool IsOn
            {
                get => _isOn;
                set
                {
                    if (_isOn == value)
                        return;

                    _isOn = value;
                    this.RaisePropertyChanged(nameof(IsOn));
                    AfterToggled?.Invoke(this, EventArgs.Empty);
                }
            }

            public bool IsEnabled
            {
                get => _isEnabled;
                set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
            }

            public bool IsSelected
            {
                get => _isSelected;
                set => this.RaiseAndSetIfChanged(ref _isSelected, value);
            }

            public event EventHandler? AfterToggled;

            public int? IOChannel { get; }

            public IOOutItemViewModel(string id, string name, bool isOn, bool isEnabled, int? IOChannel = null)
            {
                Id = id;
                Name = name;
                _isOn = isOn;
                _isEnabled = isEnabled;
                this.IOChannel = IOChannel;
            }
        }

        private enum MotionMode
        {
            Jog,
            Point,
        }

        private enum RotationDirection
        {
            CW,
            CCW,
        }

        public sealed class AxisDescriptor
        {
            public AxisDescriptor(Guid cardGuid, int axisNo, string name)
            {
                CardGuid = cardGuid;
                AxisNo = axisNo;
                Name = name ?? string.Empty;
            }

            public Guid CardGuid { get; }
            public int AxisNo { get; }
            public string Name { get; }
        }

        public sealed class IoDescriptor
        {
            public IoDescriptor(Guid cardGuid, string channelNo, string name, bool canWrite)
            {
                CardGuid = cardGuid;
                ChannelNo = channelNo;
                Name = name ?? string.Empty;
                CanWrite = canWrite;
            }

            public Guid CardGuid { get; }
            public string ChannelNo { get; }
            public string Name { get; }
            public bool CanWrite { get; }
        }
    }
}
