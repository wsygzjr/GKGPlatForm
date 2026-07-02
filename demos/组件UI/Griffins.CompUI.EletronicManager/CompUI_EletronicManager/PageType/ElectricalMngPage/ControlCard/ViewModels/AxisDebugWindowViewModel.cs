using GF_Gereric;
using GKG.SubMM;
using GKG.UI;
using Griffins;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using Avalonia.Threading;
using Avalonia.Media;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels
{
    public class AxisDebugWindowViewModel : ReactiveObject
    {
        private const string ParamKey_CardGuid = "CardGuid";
        private const string ParamKey_AxisName = "AxisName";
        private const string ParamKey_AxisIndex = "AxisIndex";
        private const string ParamKey_Axis = "Axis";
        private const string ParamKey_IoPortNo = "IOPortNo";

        private const string ResultKey_Data = "data";

        private const string ParamKey_IoChannel = "IOChannel";
        private const string ParamKey_Value = "Value";

        private const string ParamKey_SelectedAxisIndex = "SelectedAxisIndex";
        private const string ParamKey_MotionMode = "MotionMode";
        private const string ParamKey_RotationDirection = "RotationDirection";
        private const string ParamKey_Velocity = "Velocity";
        private const string ParamKey_Acceleration = "Acceleration";
        private const string ParamKey_Distance = "Distance";

        private readonly ICompUIRunTimeCallBack _callBack;

        private DispatcherTimer? _ioInRefreshTimer;
        private readonly AxisStatusItemViewModel _axisSelectionPlaceholder = new AxisStatusItemViewModel("[请选择轴]", string.Empty);

        public Guid CardGuid { get; }
        public int AxisCount { get; }

        public string CardGuidText => CardGuid.ToString();

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

                this.RaisePropertyChanged(nameof(CanMove));
                this.RaisePropertyChanged(nameof(HasSelectedAxis));
                this.RaisePropertyChanged(nameof(AxisSelectionHint));
            }
        }

        private void SelectedAxisStatus_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxisStatusItemViewModel.IsEnabled) ||
                e.PropertyName == nameof(AxisStatusItemViewModel.IsAlarm))
            {
                this.RaisePropertyChanged(nameof(CanMove));
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
        public TextInputViewModel CardGuidDisplayViewModel { get; } = new TextInputViewModel();
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

        public AxisDebugWindowViewModel(ICompUIRunTimeCallBack callBack, Guid cardGuid, int axisCount, int initialAxisIndex)
        {
            _callBack = callBack;
            CardGuid = cardGuid;

            AxisCount = axisCount > 0 ? axisCount : 0;

            MoveCommand = ReactiveCommand.Create(move, this.WhenAnyValue(x => x.CanMove));
            StopCommand = ReactiveCommand.Create(stop);
            EnableCommand = ReactiveCommand.Create(enableServo);
            DisableCommand = ReactiveCommand.Create(disableServo);
            ClearStatusCommand = ReactiveCommand.Create(clearStatus);
            HomeCommand = ReactiveCommand.Create(home, this.WhenAnyValue(x => x.CanMove));
            JogToggleCommand = ReactiveCommand.Create(toggleJog);

            RefreshIOInCommand = ReactiveCommand.Create(refreshIoIn);
            initAxisStatusList(initialAxisIndex);
            wireAxisAndMotionInputs();

            initIoInItems();
            initIoOutItems();

            wireCardGuidDisplay();

            IsIOInAutoRefreshEnabled = false;
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
            AxisSelectionComboxViewModel.SelectedItem = _axisSelectionPlaceholder;
        }

        private void wireCardGuidDisplay()
        {
            CardGuidDisplayViewModel.Text = CardGuidText;
            CardGuidDisplayViewModel.IsEnabled = false;
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

            const int ioInChannelDefault = 6;
            int ch = 1;
            CommonSensors.Add(new IOInItemViewModel(ch++, "启动按钮", true, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "复位按钮", false, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "急停按钮", true, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "门开关", false, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "(前门)安全门气缸锁", false, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "(后门)安全门气缸锁", false, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "气压", true, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "测高", false, IOChannel: 6));
            CommonSensors.Add(new IOInItemViewModel(ch++, "清洗1", false, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "清洗圈数1", false, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "清洗2", false, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "清洗圈数2", false, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "右胶盖检测", false, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "右胶液高液位检测", false, IOChannel: ioInChannelDefault));
            CommonSensors.Add(new IOInItemViewModel(ch++, "左胶盖检测", false, IOChannel: ioInChannelDefault));

            TrackASensors.Add(new IOInItemViewModel(ch++, "进板检测", true, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "出板检测", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "左到板检测", true, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "右到板检测", true, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "左待料信号", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "右待料信号", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "左挡板气缸上限位", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "左挡板气缸下限位", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "右挡板气缸上限位", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "右挡板气缸下限位", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "顶料气缸下限位", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "接近感应器", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "上位机右板信号", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "下位机左板信号", false, IOChannel: ioInChannelDefault));
            TrackASensors.Add(new IOInItemViewModel(ch++, "提前减速信号", false, IOChannel: ioInChannelDefault));

            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)进板检测", true, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)出板检测", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)左到板检测", true, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)右到板检测", true, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)左待料信号", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)右待料信号", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)左挡板气缸上限位", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)左挡板气缸下限位", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)右挡板气缸上限位", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)右挡板气缸下限位", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)顶料气缸下限位", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)接近感应器", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)上位机右板信号", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)下位机左板信号", false, IOChannel: ioInChannelDefault));
            TrackBSensors.Add(new IOInItemViewModel(ch++, "(B轨)提前减速信号", false, IOChannel: ioInChannelDefault));
        }

        private void startIoInAutoRefresh()
        {
            if (_ioInRefreshTimer == null)
            {
                _ioInRefreshTimer = new DispatcherTimer();
                _ioInRefreshTimer.Interval = TimeSpan.FromMilliseconds(500);
                _ioInRefreshTimer.Tick += (_, __) => refreshIoIn();
            }
            _ioInRefreshTimer.Start();
        }

        private void stopIoInAutoRefresh()
        {
            _ioInRefreshTimer?.Stop();
        }

        private bool _isIOInAutoRefreshEnabled;
        public bool IsIOInAutoRefreshEnabled
        {
            get => _isIOInAutoRefreshEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref _isIOInAutoRefreshEnabled, value);
                if (value) startIoInAutoRefresh(); else stopIoInAutoRefresh();
            }
        }

        private void refreshIoIn()
        {
            try
            {
                refreshIoInList(CommonSensors);
                refreshIoInList(TrackASensors);
                refreshIoInList(TrackBSensors);
            }
            catch
            {
            }
        }

        private void refreshIoInList(ObservableCollection<IOInItemViewModel> list)
        {
            foreach (var item in list)
            {
                var p = buildCommonCardAxisParam(getSelectedAxisIndex());
                p[ParamKey_AxisIndex] = new GriffinsBaseValue(getSelectedAxisIndex().ToString(CultureInfo.InvariantCulture));
                p[ParamKey_Axis] = new GriffinsBaseValue(getSelectedAxisIndex().ToString(CultureInfo.InvariantCulture));
                var ioCh = item.IOChannel ?? item.Channel;
                p[ParamKey_IoChannel] = new GriffinsBaseValue(ioCh.ToString(CultureInfo.InvariantCulture));

                var result = ExecWithResult(EletronicManagerSubMachineModulesConst.RtCmdGetInOutputState, p);
                if (result == null)
                    continue;

                var dataText = getResultData(result);
                if (string.IsNullOrWhiteSpace(dataText))
                    continue;

                if (bool.TryParse(dataText, out var b))
                    item.IsOn = b;
                else if (int.TryParse(dataText, out var i))
                    item.IsOn = i != 0;
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
                    item.IsOn = b;
                else if (int.TryParse(s, out var i))
                    item.IsOn = i != 0;
            }
        }

        private static string? TryGetParamString(GFBaseTypeParamValueList param, string key)
        {
            if (param == null || string.IsNullOrWhiteSpace(key))
                return null;

            try
            {
                var v = param[key];
                return v?.ToString();
            }
            catch
            {
                return null;
            }
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

            var p = buildCommonCardAxisParam(getSelectedAxisIndex());
            p[ParamKey_AxisIndex] = new GriffinsBaseValue(getSelectedAxisIndex().ToString(CultureInfo.InvariantCulture));
            p[ParamKey_Axis] = new GriffinsBaseValue(getSelectedAxisIndex().ToString(CultureInfo.InvariantCulture));
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

            if (initialAxisIndex < 0)
                initialAxisIndex = 0;
            AxisSelectionList.Clear();
            AxisSelectionList.Add(_axisSelectionPlaceholder);
            foreach (var axis in AxisStatusList)
                AxisSelectionList.Add(axis);

            SelectedAxisStatus = null;
        }

        private AxisStatusItemViewModel? getSelectedAxis()
        {
            return SelectedAxisStatus;
        }

        private int getSelectedAxisIndex()
        {
            var axis = getSelectedAxis();
            if (axis == null) return -1;
            return AxisStatusList.IndexOf(axis);
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

            Exec(EletronicManagerSubMachineModulesConst.RtCmdAbsoluteMove, buildAbsoluteMoveParam(axisIndex, axis.Position + dir * dist, maxSpeed));

            axis.Position = axis.Position + dir * dist;
            this.RaisePropertyChanged(nameof(CanMove));
        }

        private void stop()
        {
            JogStop();
            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;
            Exec(EletronicManagerSubMachineModulesConst.RtCmdAxisStop, buildAxisStopParam(axisIndex));
        }

        private void enableServo()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;

            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;

            Exec(EletronicManagerSubMachineModulesConst.RtCmdServoOn, buildServoOnParam(axisIndex, true));
            axis.IsEnabled = true;
            this.RaisePropertyChanged(nameof(CanMove));
        }

        private void disableServo()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;

            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;

            Exec(EletronicManagerSubMachineModulesConst.RtCmdServoOn, buildServoOnParam(axisIndex, false));
            axis.IsEnabled = false;
            this.RaisePropertyChanged(nameof(CanMove));
        }

        private void clearStatus()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;
            JogStop();

            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;
            Exec(EletronicManagerSubMachineModulesConst.RtCmdClearAxisAlarm, buildCommonCardAxisParam(axisIndex));

            axis.IsAlarm = false;
            axis.IsPosLimit = false;
            axis.IsNegLimit = false;
            this.RaisePropertyChanged(nameof(CanMove));
        }

        private void home()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;
            JogStop();

            var axisIndex = getSelectedAxisIndex();
            if (axisIndex < 0) return;
            Exec(EletronicManagerSubMachineModulesConst.RtCmdHomed, buildCommonCardAxisParam(axisIndex));

            axis.Position = 0;
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

            var dir = _rotationDirection == RotationDirection.CW ? 1 : -1;
            var speed = parseDoubleOrDefault(Velocity, 1);
            axis.Position += dir * speed * 0.05;
        }

        public void JogStop()
        {
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
        }

        private void Exec(string cmdId, GFBaseTypeParamValueList cmdParam)
        {
            try
            {
                cmdParam = EnrichCmdParamWithUiSnapshot(cmdParam);
                _callBack.ExecConfigSvrCtlCmd(cmdId, cmdParam);
            }
            catch
            {
            }
        }

        private GFBaseTypeParamValueList? ExecWithResult(string cmdId, GFBaseTypeParamValueList cmdParam)
        {
            try
            {
                cmdParam = EnrichCmdParamWithUiSnapshot(cmdParam);
                return _callBack.ExecConfigSvrCtlCmd(cmdId, cmdParam);
            }
            catch
            {
                return null;
            }
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

        private GFBaseTypeParamValueList buildCommonCardAxisParam(int axisIndex)
        {
            var p = new GFBaseTypeParamValueList();
            p[ParamKey_CardGuid] = new GriffinsBaseValue(CardGuid.ToString());
            p[ParamKey_AxisName] = new GriffinsBaseValue(getSelectedAxisKey());
            p[ParamKey_AxisIndex] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            p[ParamKey_Axis] = new GriffinsBaseValue(axisIndex.ToString(CultureInfo.InvariantCulture));
            p[ParamKey_IoPortNo] = new GriffinsBaseValue("0");
            return p;
        }

        private GFBaseTypeParamValueList buildAbsoluteMoveParam(int axisIndex, double pos, double maxSpeed)
        {
            var p = buildCommonCardAxisParam(axisIndex);
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
            var p = buildCommonCardAxisParam(axisIndex);
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
            var p = buildCommonCardAxisParam(axisIndex);
            p["StopType"] = new GriffinsBaseValue(((int)GKG.MotionControlAxisStopTypeConstants.DecelerationStop).ToString(CultureInfo.InvariantCulture));
            return p;
        }

        private GFBaseTypeParamValueList buildServoOnParam(int axisIndex, bool isEnabled)
        {
            var cmd = buildCommonCardAxisParam(axisIndex);
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

        public class IOInItemViewModel : ReactiveObject
        {
            public int Channel { get; }
            public string Name { get; }

            public int? IOChannel { get; }

            private bool _isOn;
            public bool IsOn
            {
                get => _isOn;
                set
                {
                    this.RaiseAndSetIfChanged(ref _isOn, value);
                    this.RaisePropertyChanged(nameof(StatusColor));
                }
            }

            public IBrush StatusColor => IsOn ? Brushes.LightGreen : Brushes.LightGray;

            public IOInItemViewModel(int channel, string name, bool isOn, int? IOChannel = null)
            {
                Channel = channel;
                Name = name;
                this.IOChannel = IOChannel;
                _isOn = isOn;
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
    }
}
