using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using GF_Gereric;
using GKG.SubMM;
using Griffins;
using Griffins.Map.UI;

namespace Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.AxisDebugging.ViewModels
{
    public class AxisDebuggingCompUIViewModel : ReactiveObject
    {
        public event EventHandler? AfterModified;

        private ObservableCollection<AxisStatusItemViewModel> _axisStatusList = new ObservableCollection<AxisStatusItemViewModel>();
        public ObservableCollection<AxisStatusItemViewModel> AxisStatusList
        {
            get => _axisStatusList;
            set
            {
                if (ReferenceEquals(_axisStatusList, value))
                    return;

                _axisStatusList.CollectionChanged -= axisStatusList_CollectionChanged;
                this.RaiseAndSetIfChanged(ref _axisStatusList, value);
                _axisStatusList.CollectionChanged += axisStatusList_CollectionChanged;
                rebuildAxisOptions();
            }
        }

        private ObservableCollection<string> _axisOptions = new ObservableCollection<string>();
        public ObservableCollection<string> AxisOptions
        {
            get => _axisOptions;
            set => this.RaiseAndSetIfChanged(ref _axisOptions, value);
        }

        private string? _selectedAxis;
        public string? SelectedAxis
        {
            get => _selectedAxis;
            set
            {
                JogStop();
                this.RaiseAndSetIfChanged(ref _selectedAxis, value);
                onAfterModified();
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
                onAfterModified();
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
                onAfterModified();
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
                onAfterModified();
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
                onAfterModified();
            }
        }

        private string _velocity = "1";
        public string Velocity
        {
            get => _velocity;
            set
            {
                this.RaiseAndSetIfChanged(ref _velocity, value);
                onAfterModified();
            }
        }

        private string _acceleration = "1000";
        public string Acceleration
        {
            get => _acceleration;
            set
            {
                this.RaiseAndSetIfChanged(ref _acceleration, value);
                onAfterModified();
            }
        }

        private string _distance = "1";
        public string Distance
        {
            get => _distance;
            set
            {
                this.RaiseAndSetIfChanged(ref _distance, value);
                onAfterModified();
            }
        }

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

        public ReactiveCommand<Unit, Unit> MoveCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
        public ReactiveCommand<Unit, Unit> EnableCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearStatusCommand { get; }
        public ReactiveCommand<Unit, Unit> HomeCommand { get; }
        public ReactiveCommand<Unit, Unit> JogToggleCommand { get; }

        private DispatcherTimer? _jogTimer;
        private bool _isJogging;

        private const double SimulatedPosLimitPosition = 100;
        private const double SimulatedNegLimitPosition = -100;
        private const double SimulatedHomeTolerance = 0.0001;

        public bool IsJogging => _isJogging;

        public string JogButtonText => _isJogging ? "停止" : "运动";

        private readonly ICompUIRunTimeCallBack? _callBack;

        public AxisDebuggingCompUIViewModel(ICompUIRunTimeCallBack? callBack = null)
        {
            _callBack = callBack;
            var canMove = this.WhenAnyValue(x => x.CanMove);

            MoveCommand = ReactiveCommand.Create(move, canMove);
            StopCommand = ReactiveCommand.Create(stop);
            EnableCommand = ReactiveCommand.Create(toggleEnable);
            ClearStatusCommand = ReactiveCommand.Create(clearStatus);
            HomeCommand = ReactiveCommand.Create(home, canMove);

            JogToggleCommand = ReactiveCommand.Create(toggleJog);

            initAxisStatusList();
        }

        private void initAxisStatusList()
        {
            AxisStatusList = new ObservableCollection<AxisStatusItemViewModel>
            {
                new AxisStatusItemViewModel("X轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("Y轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("Z轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("Vx轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("Vy轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("前导轨调宽轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("中导轨调宽轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("后导轨调宽轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("A轨_运输皮带轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("B轨_运输皮带轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("清洗轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("清洗轴2", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("中间顶升平台轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("倾斜轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("回流轨运轴", 0.000, false, true, false, false, false),
                new AxisStatusItemViewModel("回流调宽轴", 0.000, false, true, false, false, false)
            };

            foreach (var axis in AxisStatusList)
            {
                axis.AfterModified += (_, __) =>
                {
                    this.RaisePropertyChanged(nameof(CanMove));
                    onAfterModified();
                };

                axis.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(AxisStatusItemViewModel.AxisName))
                    {
                        rebuildAxisOptions();
                    }
                };
            }

            rebuildAxisOptions();
        }

        private void axisStatusList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            rebuildAxisOptions();
        }

        private void rebuildAxisOptions()
        {
            AxisOptions = new ObservableCollection<string>(AxisStatusList.Select(o => o.AxisName));

            if (AxisOptions.Count == 0)
            {
                SelectedAxis = null;
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedAxis) || !AxisOptions.Contains(SelectedAxis))
            {
                SelectedAxis = AxisOptions.FirstOrDefault();
            }
        }

        private AxisStatusItemViewModel? getSelectedAxis()
        {
            if (string.IsNullOrWhiteSpace(SelectedAxis)) return null;
            return AxisStatusList.FirstOrDefault(o => o.AxisName == SelectedAxis);
        }

        private int getSelectedLogicalAxis(AxisStatusItemViewModel axis)
        {
            return AxisStatusList.IndexOf(axis);
        }

        private void sendNormalCmd(string cmdId, string paramKey, object paramObj)
        {
            if (_callBack == null)
            {
                return;
            }

            try
            {
                var param = new GFBaseTypeParamValueList();
                var json = JsonObjConvert.ToJSon(paramObj);
                param[paramKey] = new GriffinsBaseValue(json);
                _callBack.ExecConfigSvrCtlCmd(cmdId, param);
            }
            catch
            {
            }
        }

        private void move()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;

            var moveParam = new AxisDebugRelativeMoveParameters
            {
                LogicalAxis = getSelectedLogicalAxis(axis),
                RelativeDistance = parseDoubleOrDefault(Distance, 0),
                Speed = parseDoubleOrDefault(Velocity, 1),
                Direction = _rotationDirection == RotationDirection.CW,
            };
            sendNormalCmd(AxisDebugSubMachineModulesConst.MoveRelativeMethodID,
                AxisDebugSubMachineModulesConst.MoveRelativeParamKey,
                moveParam);

            // 本地模拟逻辑保留作为回显（实际应由后端状态推送更新）
            axis.Position = axis.Position + (_rotationDirection == RotationDirection.CW ? 1 : -1) * parseDoubleOrDefault(Distance, 1);
            updateHomeFlag(axis);
            updateLimitAndAlarm(axis);
            onAfterModified();
        }

        private void stop()
        {
            JogStop();
            var axis = getSelectedAxis();

            if (axis != null)
            {
                var stopParam = new AxisDebugStopParameters
                {
                    LogicalAxis = getSelectedLogicalAxis(axis),
                };
                sendNormalCmd(AxisDebugSubMachineModulesConst.StopMethodID,
                    AxisDebugSubMachineModulesConst.StopParamKey,
                    stopParam);
            }
            onAfterModified();
        }

        private void toggleEnable()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;
            JogStop();

            var enableParam = new AxisDebugEnableParameters
            {
                LogicalAxis = getSelectedLogicalAxis(axis),
                Enable = !axis.IsEnabled,
            };
            sendNormalCmd(AxisDebugSubMachineModulesConst.EnableMethodID,
                AxisDebugSubMachineModulesConst.EnableParamKey,
                enableParam);

            axis.IsEnabled = !axis.IsEnabled;
            this.RaisePropertyChanged(nameof(CanMove));
            onAfterModified();
        }

        private void clearStatus()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;
            JogStop();

            var clearParam = new AxisDebugClearStatusParameters
            {
                LogicalAxis = getSelectedLogicalAxis(axis),
            };
            sendNormalCmd(AxisDebugSubMachineModulesConst.ClearStatusMethodID,
                AxisDebugSubMachineModulesConst.ClearStatusParamKey,
                clearParam);

            axis.IsAlarm = false;
            axis.IsPosLimit = false;
            axis.IsNegLimit = false;
            updateHomeFlag(axis);
            this.RaisePropertyChanged(nameof(CanMove));
            onAfterModified();
        }

        private void home()
        {
            var axis = getSelectedAxis();
            if (axis == null) return;
            JogStop();

            var homeParam = new AxisDebugHomeParameters
            {
                LogicalAxis = getSelectedLogicalAxis(axis),
            };
            sendNormalCmd(AxisDebugSubMachineModulesConst.HomeMethodID,
                AxisDebugSubMachineModulesConst.HomeParamKey,
                homeParam);

            axis.Position = 0;
            updateHomeFlag(axis);
            updateLimitAndAlarm(axis);
            onAfterModified();
        }

        private void toggleJog()
        {
            if (!IsJogMode) return;

            var axis = getSelectedAxis();
            if (axis == null) return;

            if (_isJogging)
            {
                JogStop();
                onAfterModified();
                return;
            }

            var jogParam = new AxisDebugJogParameters
            {
                LogicalAxis = getSelectedLogicalAxis(axis),
                Speed = parseDoubleOrDefault(Velocity, 1),
                Direction = _rotationDirection == RotationDirection.CW,
                Start = true,
            };
            sendNormalCmd(AxisDebugSubMachineModulesConst.JogMethodID,
                AxisDebugSubMachineModulesConst.JogParamKey,
                jogParam);

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
            onAfterModified();
        }

        public void JogStop()
        {
            if (_isJogging)
            {
                var axis = getSelectedAxis();
                if (axis != null)
                {
                    var jogParam = new AxisDebugJogParameters
                    {
                        LogicalAxis = getSelectedLogicalAxis(axis),
                        Start = false,
                    };
                    sendNormalCmd(AxisDebugSubMachineModulesConst.JogMethodID,
                        AxisDebugSubMachineModulesConst.JogParamKey,
                        jogParam);
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

        private void jogTick()
        {
            if (!_isJogging)
            {
                JogStop();
                return;
            }

            var axis = getSelectedAxis();
            if (axis == null)
            {
                JogStop();
                return;
            }

            if (!CanMove)
            {
                JogStop();
                return;
            }

            var vel = parseDoubleOrDefault(Velocity, 1);
            var dt = _jogTimer?.Interval.TotalSeconds ?? 0.05;
            var dir = _rotationDirection == RotationDirection.CW ? 1 : -1;

            axis.Position = axis.Position + dir * vel * dt;
            updateHomeFlag(axis);
            updateLimitAndAlarm(axis);
            onAfterModified();
        }

        private void updateHomeFlag(AxisStatusItemViewModel axis)
        {
            axis.IsHome = Math.Abs(axis.Position) <= SimulatedHomeTolerance;
        }

        private void updateLimitAndAlarm(AxisStatusItemViewModel axis)
        {
            axis.IsPosLimit = axis.Position >= SimulatedPosLimitPosition;
            axis.IsNegLimit = axis.Position <= SimulatedNegLimitPosition;

            if (axis.IsPosLimit || axis.IsNegLimit)
            {
                axis.IsAlarm = true;
                JogStop();
            }
        }

        private double parseDoubleOrDefault(string? text, double defaultValue)
        {
            if (double.TryParse(text, out var value)) return value;
            return defaultValue;
        }

        private void onAfterModified()
        {
            this.RaisePropertyChanged(nameof(CanMove));
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        public enum MotionMode
        {
            Jog,
            Point
        }

        public enum RotationDirection
        {
            CW,
            CCW
        }

        public class AxisStatusItemViewModel : ReactiveObject
        {
            public event EventHandler? AfterModified;

            private string _axisName;
            public string AxisName
            {
                get => _axisName;
                set
                {
                    this.RaiseAndSetIfChanged(ref _axisName, value);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            }

            private double _position;
            public double Position
            {
                get => _position;
                set
                {
                    this.RaiseAndSetIfChanged(ref _position, value);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            }

            private bool _isPosLimit;
            public bool IsPosLimit
            {
                get => _isPosLimit;
                set
                {
                    this.RaiseAndSetIfChanged(ref _isPosLimit, value);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            }

            private bool _isHome;
            public bool IsHome
            {
                get => _isHome;
                set
                {
                    this.RaiseAndSetIfChanged(ref _isHome, value);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            }

            private bool _isNegLimit;
            public bool IsNegLimit
            {
                get => _isNegLimit;
                set
                {
                    this.RaiseAndSetIfChanged(ref _isNegLimit, value);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            }

            private bool _isAlarm;
            public bool IsAlarm
            {
                get => _isAlarm;
                set
                {
                    this.RaiseAndSetIfChanged(ref _isAlarm, value);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            }

            private bool _isEnabled;
            public bool IsEnabled
            {
                get => _isEnabled;
                set
                {
                    this.RaiseAndSetIfChanged(ref _isEnabled, value);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            }

            public AxisStatusItemViewModel(
                string axisName,
                double position,
                bool isPosLimit,
                bool isHome,
                bool isNegLimit,
                bool isAlarm,
                bool isEnabled)
            {
                _axisName = axisName;
                _position = position;
                _isPosLimit = isPosLimit;
                _isHome = isHome;
                _isNegLimit = isNegLimit;
                _isAlarm = isAlarm;
                _isEnabled = isEnabled;
            }
        }
    }
}
