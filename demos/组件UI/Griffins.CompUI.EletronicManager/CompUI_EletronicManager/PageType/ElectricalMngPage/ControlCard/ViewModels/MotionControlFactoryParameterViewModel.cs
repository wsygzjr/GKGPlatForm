using GKG.UI;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.Converters;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels
{
    public class MotionControlFactoryParameterViewModel : ReactiveObject
    {
        private static long s_revisionSeed;
        private bool _isSuppressingRevision;
        public TextInputViewModel HomingAccelerationTimeViewModel { get; } = new();
        public TextInputViewModel HomingDecelerationTimeViewModel { get; } = new();
        public TextInputViewModel AxisNameViewModel { get; } = new();
        public TextInputViewModel HomingInitialSpeedViewModel { get; } = new();
        public TextInputViewModel HomingMinimumSpeedViewModel { get; } = new();
        public TextInputViewModel HomingMaximumSpeedViewModel { get; } = new();
        public TextInputViewModel HomingRetractDistanceViewModel { get; } = new();
        public TextInputViewModel StopReactionViewModel { get; } = new();
        public TextInputViewModel PulseRatioViewModel { get; } = new();
        public TextInputViewModel PulsesPerRevolutionViewModel { get; } = new();
        public TextInputViewModel PositiveLimitViewModel { get; } = new();
        public TextInputViewModel NegativeLimitViewModel { get; } = new();
        public TextInputViewModel PulseOutputModeViewModel { get; } = new();

        public ComboxViewModel HomingModeComboxViewModel { get; } = new();
        public ComboxViewModel HomingDirectionComboxViewModel { get; } = new();
        public ComboxViewModel EncoderTypeComboxViewModel { get; } = new();
        public ComboxViewModel PulseInputModeComboxViewModel { get; } = new();
        public ComboxViewModel PulseOutputModeComboxViewModel { get; } = new();
        public IReadOnlyList<ComBoxItem> EncoderTypeItems { get; } = CloneComBoxItems(BuildEncoderTypeItems());
        public IReadOnlyList<ComBoxItem> PulseInputModeItems { get; } = CloneComBoxItems(BuildPulseInputModeItems());
        public IReadOnlyList<ComBoxItem> PulseOutputModeItems { get; } = CloneComBoxItems(BuildPulseOutputModeItems());

        private int _axisNo;
        public int AxisNo
        {
            get => _axisNo;
            set
            {
                this.RaiseAndSetIfChanged(ref _axisNo, value);
                this.RaisePropertyChanged(nameof(AxisSequence));
                this.RaisePropertyChanged(nameof(AxisSelectorName));
                this.RaisePropertyChanged(nameof(DefaultAxisName));
                this.RaisePropertyChanged(nameof(AxisDisplayName));
                this.RaisePropertyChanged(nameof(AxisNameEditor));
            }
        }

        public int AxisSequence => Math.Max(0, AxisNo - 1);
        public string AxisSelectorName => $"\u8F74{AxisNo}";

        private string _cardDisplayName = string.Empty;
        public string CardDisplayName
        {
            get => _cardDisplayName;
            set
            {
                this.RaiseAndSetIfChanged(ref _cardDisplayName, value);
                this.RaisePropertyChanged(nameof(DefaultAxisName));
                this.RaisePropertyChanged(nameof(AxisDisplayName));
                this.RaisePropertyChanged(nameof(AxisNameEditor));
            }
        }

        public string DefaultAxisName => $"{(string.IsNullOrWhiteSpace(CardDisplayName) ? "\u8FD0\u63A7\u5361" : CardDisplayName)}-{RandomSuffix}";

        private string _randomSuffix = BuildRandomSuffix();
        public string RandomSuffix
        {
            get => _randomSuffix;
            set
            {
                this.RaiseAndSetIfChanged(ref _randomSuffix, value);
                this.RaisePropertyChanged(nameof(DefaultAxisName));
                this.RaisePropertyChanged(nameof(AxisDisplayName));
                this.RaisePropertyChanged(nameof(AxisNameEditor));
            }
        }

        private string _axisName = string.Empty;
        public string AxisName
        {
            get => _axisName;
            set
            {
                this.RaiseAndSetIfChanged(ref _axisName, value);
                this.RaisePropertyChanged(nameof(AxisDisplayName));
                this.RaisePropertyChanged(nameof(AxisNameEditor));
            }
        }

        public string AxisDisplayName => string.IsNullOrWhiteSpace(AxisName) ? DefaultAxisName : AxisName;

        public string AxisNameEditor
        {
            // Editor should bind to the raw name so clearing text won't be
            // immediately replaced by DefaultAxisName during typing.
            get => AxisName;
            set
            {
                AxisName = value ?? string.Empty;
                AxisNameViewModel.Text = AxisName;
            }
        }

        private GKG.MotionControlAxisHomingMode _homingMode;
        public GKG.MotionControlAxisHomingMode HomingMode
        {
            get => _homingMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _homingMode, value);
                var current = (HomingModeComboxViewModel.SelectedItem as ComBoxItem)?.Value;
                if (!Equals(current, value))
                {
                    HomingModeComboxViewModel.SelectedItem = FindByValue(HomingModeComboxViewModel.ItemsSource, value);
                }
            }
        }

        private int _homingDirection;
        public int HomingDirection
        {
            get => _homingDirection;
            set
            {
                this.RaiseAndSetIfChanged(ref _homingDirection, value);
                var current = (HomingDirectionComboxViewModel.SelectedItem as ComBoxItem)?.Value;
                if (!Equals(current, value))
                    HomingDirectionComboxViewModel.SelectedItem = FindByValue(HomingDirectionComboxViewModel.ItemsSource, value);
            }
        }

        private double _homingAccelerationTime;
        public double HomingAccelerationTime
        {
            get => _homingAccelerationTime;
            set => this.RaiseAndSetIfChanged(ref _homingAccelerationTime, value);
        }

        private double _homingDecelerationTime;
        public double HomingDecelerationTime
        {
            get => _homingDecelerationTime;
            set => this.RaiseAndSetIfChanged(ref _homingDecelerationTime, value);
        }

        private double _homingInitialSpeed;
        public double HomingInitialSpeed
        {
            get => _homingInitialSpeed;
            set => this.RaiseAndSetIfChanged(ref _homingInitialSpeed, value);
        }

        private double _homingMinimumSpeed;
        public double HomingMinimumSpeed
        {
            get => _homingMinimumSpeed;
            set => this.RaiseAndSetIfChanged(ref _homingMinimumSpeed, value);
        }

        private double _homingMaximumSpeed;
        public double HomingMaximumSpeed
        {
            get => _homingMaximumSpeed;
            set => this.RaiseAndSetIfChanged(ref _homingMaximumSpeed, value);
        }

        private double _homingRetractDistance;
        public double HomingRetractDistance
        {
            get => _homingRetractDistance;
            set => this.RaiseAndSetIfChanged(ref _homingRetractDistance, value);
        }

        private bool _stateEnable;
        public bool StateEnable
        {
            get => _stateEnable;
            set => this.RaiseAndSetIfChanged(ref _stateEnable, value);
        }

        private bool _stateReverse;
        public bool StateReverse
        {
            get => _stateReverse;
            set => this.RaiseAndSetIfChanged(ref _stateReverse, value);
        }

        private int _stopReaction;
        public int StopReaction
        {
            get => _stopReaction;
            set => this.RaiseAndSetIfChanged(ref _stopReaction, value);
        }

        private double _pulseRatio;
        public double PulseRatio
        {
            get => _pulseRatio;
            set => this.RaiseAndSetIfChanged(ref _pulseRatio, value);
        }

        private double _pulsesPerRevolution;
        public double PulsesPerRevolution
        {
            get => _pulsesPerRevolution;
            set => this.RaiseAndSetIfChanged(ref _pulsesPerRevolution, value);
        }

        private double _positiveLimit;
        public double PositiveLimit
        {
            get => _positiveLimit;
            set => this.RaiseAndSetIfChanged(ref _positiveLimit, value);
        }

        private double _negativeLimit;
        public double NegativeLimit
        {
            get => _negativeLimit;
            set => this.RaiseAndSetIfChanged(ref _negativeLimit, value);
        }

        private GKG.EncoderType? _encoderType;
        public GKG.EncoderType? EncoderType
        {
            get => _encoderType;
            set
            {
                this.RaiseAndSetIfChanged(ref _encoderType, value);
                var target = value ?? GKG.EncoderType.None;
                var current = (EncoderTypeComboxViewModel.SelectedItem as ComBoxItem)?.Value;
                if (!Equals(current, target))
                    EncoderTypeComboxViewModel.SelectedItem = FindByValue(EncoderTypeComboxViewModel.ItemsSource, target);
                this.RaisePropertyChanged(nameof(SelectedEncoderTypeItem));
            }
        }

        public object? SelectedEncoderTypeItem
        {
            get => FindByValue(EncoderTypeComboxViewModel.ItemsSource, EncoderType ?? GKG.EncoderType.None);
            set
            {
                if (value is ComBoxItem item && item.Value is GKG.EncoderType encoderType)
                    EncoderType = encoderType;
            }
        }

        private GKG.MotionControlPulseInputMode? _pulseInputMode;
        public GKG.MotionControlPulseInputMode? PulseInputMode
        {
            get => _pulseInputMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _pulseInputMode, value);
                var target = value ?? GKG.MotionControlPulseInputMode.PulseDirection;
                var current = (PulseInputModeComboxViewModel.SelectedItem as ComBoxItem)?.Value;
                if (!Equals(current, target))
                    PulseInputModeComboxViewModel.SelectedItem = FindByValue(PulseInputModeComboxViewModel.ItemsSource, target);
                this.RaisePropertyChanged(nameof(SelectedPulseInputModeItem));
            }
        }

        public object? SelectedPulseInputModeItem
        {
            get => FindByValue(PulseInputModeComboxViewModel.ItemsSource, PulseInputMode ?? GKG.MotionControlPulseInputMode.PulseDirection);
            set
            {
                if (value is ComBoxItem item && item.Value is GKG.MotionControlPulseInputMode pulseInputMode)
                    PulseInputMode = pulseInputMode;
            }
        }

        private int? _pulseOutputMode;
        public int? PulseOutputMode
        {
            get => _pulseOutputMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _pulseOutputMode, value);
                var target = value ?? GKG.MotionControlPulseOutputModeConstants.Pulse;
                var current = (PulseOutputModeComboxViewModel.SelectedItem as ComBoxItem)?.Value;
                if (!Equals(current, target))
                    PulseOutputModeComboxViewModel.SelectedItem = FindByValue(PulseOutputModeComboxViewModel.ItemsSource, target);
            }
        }

        public long Revision { get; private set; }

        public MotionControlFactoryParameterViewModel()
        {
            HomingModeComboxViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            HomingModeComboxViewModel.ItemsSource = CloneComBoxItems(BuildHomingModeItems());
            HomingModeComboxViewModel.ValueChanged += (_, e) => SetHomingModeFromSelection(e.NewValue);

            HomingDirectionComboxViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            HomingDirectionComboxViewModel.ItemsSource = CloneComBoxItems(BuildHomingDirectionItems());
            HomingDirectionComboxViewModel.ValueChanged += (_, e) => SetHomingDirectionFromSelection(e.NewValue);

            EncoderTypeComboxViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            EncoderTypeComboxViewModel.ItemsSource = CloneComBoxItems(EncoderTypeItems);
            EncoderTypeComboxViewModel.ValueChanged += (_, e) => SetEncoderTypeFromSelection(e.NewValue);

            PulseInputModeComboxViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            PulseInputModeComboxViewModel.ItemsSource = CloneComBoxItems(PulseInputModeItems);
            PulseInputModeComboxViewModel.ValueChanged += (_, e) => SetPulseInputModeFromSelection(e.NewValue);

            PulseOutputModeComboxViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            PulseOutputModeComboxViewModel.ItemsSource = CloneComBoxItems(PulseOutputModeItems);
            PulseOutputModeComboxViewModel.ValueChanged += (_, e) => SetPulseOutputModeFromSelection(e.NewValue);

            AxisNameViewModel.ValueChanged += (_, _) => AxisName = AxisNameViewModel.Text?.Trim() ?? string.Empty;
            HomingAccelerationTimeViewModel.ValueChanged += (_, _) => TrySetDouble(HomingAccelerationTimeViewModel, v => HomingAccelerationTime = v);
            HomingDecelerationTimeViewModel.ValueChanged += (_, _) => TrySetDouble(HomingDecelerationTimeViewModel, v => HomingDecelerationTime = v);
            HomingInitialSpeedViewModel.ValueChanged += (_, _) => TrySetDouble(HomingInitialSpeedViewModel, v => HomingInitialSpeed = v);
            HomingMinimumSpeedViewModel.ValueChanged += (_, _) => TrySetDouble(HomingMinimumSpeedViewModel, v => HomingMinimumSpeed = v);
            HomingMaximumSpeedViewModel.ValueChanged += (_, _) => TrySetDouble(HomingMaximumSpeedViewModel, v => HomingMaximumSpeed = v);
            HomingRetractDistanceViewModel.ValueChanged += (_, _) => TrySetDouble(HomingRetractDistanceViewModel, v => HomingRetractDistance = v);
            StopReactionViewModel.ValueChanged += (_, _) => TrySetInt(StopReactionViewModel, v => StopReaction = v);
            PulseRatioViewModel.ValueChanged += (_, _) => TrySetDouble(PulseRatioViewModel, v => PulseRatio = v);
            PulsesPerRevolutionViewModel.ValueChanged += (_, _) => TrySetDouble(PulsesPerRevolutionViewModel, v => PulsesPerRevolution = v);
            PositiveLimitViewModel.ValueChanged += (_, _) => TrySetDouble(PositiveLimitViewModel, v => PositiveLimit = v);
            NegativeLimitViewModel.ValueChanged += (_, _) => TrySetDouble(NegativeLimitViewModel, v => NegativeLimit = v);
            PulseOutputModeViewModel.ValueChanged += (_, _) => TrySetInt(PulseOutputModeViewModel, v => PulseOutputMode = v);

            // 默认回零模式设为“原点回零(一次)”
            HomingMode = GKG.MotionControlAxisHomingMode.OnceOriginGoHome;
            PropertyChanged += MotionControlFactoryParameterViewModel_PropertyChanged;
            ResetRevision();
        }

        public void BindHomingModeItems(IEnumerable items)
        {
            // Clone item objects to avoid cross-card/cross-axis shared selection state.
            var clonedItems = CloneComBoxItems(items.OfType<ComBoxItem>());

            HomingModeComboxViewModel.ItemsSource = clonedItems;
            HomingModeComboxViewModel.SelectedItem = FindByValue(clonedItems, HomingMode);
        }

        public static MotionControlFactoryParameterViewModel FromModel(GKG.MotionControlFactoryParameter model)
        {
            var vm = new MotionControlFactoryParameterViewModel();
            vm.CopyFrom(model);
            return vm;
        }

        public MotionControlFactoryParameterViewModel Clone()
        {
            var vm = new MotionControlFactoryParameterViewModel();
            vm.CopyFrom(this);
            return vm;
        }

        public void CopyFrom(GKG.MotionControlFactoryParameter model)
        {
            _isSuppressingRevision = true;
            try
            {
                AxisNo = model.AxisNo;
                AxisName = string.Empty;
                AxisNameViewModel.Text = AxisName;

                HomingMode = model.AxisHomingParameters?.HomingMode ?? default;
                HomingDirection = model.AxisHomingParameters?.HomingDirection ?? GKG.MotionControlAxisHomingDirectionConstants.NegativeDirection;
                HomingAccelerationTime = model.AxisHomingParameters?.HomingAccelerationTime ?? 0;
                HomingDecelerationTime = 0;
                HomingInitialSpeed = model.AxisHomingParameters?.HomingInitialSpeed ?? 0;
                HomingMinimumSpeed = model.AxisHomingParameters?.HomingMinimumSpeed ?? 0;
                HomingMaximumSpeed = model.AxisHomingParameters?.HomingMaximumSpeed ?? 0;
                HomingRetractDistance = model.AxisHomingParameters?.HomingRetractDistance ?? 0;

                HomingDirectionComboxViewModel.SelectedItem = FindByValue(HomingDirectionComboxViewModel.ItemsSource, HomingDirection);
                HomingAccelerationTimeViewModel.Text = HomingAccelerationTime.ToString(CultureInfo.InvariantCulture);
                HomingDecelerationTimeViewModel.Text = HomingDecelerationTime.ToString(CultureInfo.InvariantCulture);
                HomingInitialSpeedViewModel.Text = HomingInitialSpeed.ToString(CultureInfo.InvariantCulture);
                HomingMinimumSpeedViewModel.Text = HomingMinimumSpeed.ToString(CultureInfo.InvariantCulture);
                HomingMaximumSpeedViewModel.Text = HomingMaximumSpeed.ToString(CultureInfo.InvariantCulture);
                HomingRetractDistanceViewModel.Text = HomingRetractDistance.ToString(CultureInfo.InvariantCulture);

                StateEnable = model.AxisStateLogicParameters?.StateEnable ?? false;
                StateReverse = model.AxisStateLogicParameters?.StateReverse ?? false;
                StopReaction = model.AxisStateLogicParameters?.StopReaction ?? 0;
                StopReactionViewModel.Text = StopReaction.ToString(CultureInfo.InvariantCulture);

                PulseRatio = model.PulseRatioParameters?.PulseRatio ?? 0;
                PulsesPerRevolution = model.PulseRatioParameters?.PulsesPerRevolution ?? 0;
                PulseRatioViewModel.Text = PulseRatio.ToString(CultureInfo.InvariantCulture);
                PulsesPerRevolutionViewModel.Text = PulsesPerRevolution.ToString(CultureInfo.InvariantCulture);

                PositiveLimit = model.AxisSoftLimit?.PositiveLimit ?? 0;
                NegativeLimit = model.AxisSoftLimit?.NegativeLimit ?? 0;
                PositiveLimitViewModel.Text = PositiveLimit.ToString(CultureInfo.InvariantCulture);
                NegativeLimitViewModel.Text = NegativeLimit.ToString(CultureInfo.InvariantCulture);

                EncoderType = model.EncoderType;
                PulseInputMode = model.PulseInputMode;
                PulseOutputMode = model.PulseOutputMode;
                PulseOutputModeViewModel.Text = (PulseOutputMode ?? GKG.MotionControlPulseOutputModeConstants.Pulse).ToString(CultureInfo.InvariantCulture);
            }
            finally
            {
                _isSuppressingRevision = false;
            }

            ResetRevision();
        }

        public void CopyFrom(MotionControlFactoryParameterViewModel source)
        {
            _isSuppressingRevision = true;
            try
            {
                AxisNo = source.AxisNo;
                CardDisplayName = source.CardDisplayName;
                RandomSuffix = source.RandomSuffix;
                AxisName = source.AxisName;
                AxisNameViewModel.Text = source.AxisNameViewModel.Text;

                HomingMode = source.HomingMode;
                HomingDirection = source.HomingDirection;
                HomingAccelerationTime = source.HomingAccelerationTime;
                HomingDecelerationTime = source.HomingDecelerationTime;
                HomingInitialSpeed = source.HomingInitialSpeed;
                HomingMinimumSpeed = source.HomingMinimumSpeed;
                HomingMaximumSpeed = source.HomingMaximumSpeed;
                HomingRetractDistance = source.HomingRetractDistance;

                StateEnable = source.StateEnable;
                StateReverse = source.StateReverse;
                StopReaction = source.StopReaction;

                PulseRatio = source.PulseRatio;
                PulsesPerRevolution = source.PulsesPerRevolution;
                PositiveLimit = source.PositiveLimit;
                NegativeLimit = source.NegativeLimit;

                EncoderType = source.EncoderType;
                PulseInputMode = source.PulseInputMode;
                PulseOutputMode = source.PulseOutputMode;

                HomingAccelerationTimeViewModel.Text = source.HomingAccelerationTimeViewModel.Text;
                HomingDecelerationTimeViewModel.Text = source.HomingDecelerationTimeViewModel.Text;
                HomingInitialSpeedViewModel.Text = source.HomingInitialSpeedViewModel.Text;
                HomingMinimumSpeedViewModel.Text = source.HomingMinimumSpeedViewModel.Text;
                HomingMaximumSpeedViewModel.Text = source.HomingMaximumSpeedViewModel.Text;
                HomingRetractDistanceViewModel.Text = source.HomingRetractDistanceViewModel.Text;
                StopReactionViewModel.Text = source.StopReactionViewModel.Text;
                PulseRatioViewModel.Text = source.PulseRatioViewModel.Text;
                PulsesPerRevolutionViewModel.Text = source.PulsesPerRevolutionViewModel.Text;
                PositiveLimitViewModel.Text = source.PositiveLimitViewModel.Text;
                NegativeLimitViewModel.Text = source.NegativeLimitViewModel.Text;
                PulseOutputModeViewModel.Text = source.PulseOutputModeViewModel.Text;
            }
            finally
            {
                _isSuppressingRevision = false;
            }

            Revision = source.Revision;
        }

        public GKG.MotionControlFactoryParameter ToModel()
        {
            CommitPendingEditorChanges();

            var model = new GKG.MotionControlFactoryParameter();
            model.AxisNo = AxisNo;

            model.AxisHomingParameters.HomingMode = HomingMode;
            model.AxisHomingParameters.HomingDirection = HomingDirection;
            model.AxisHomingParameters.HomingAccelerationTime = HomingAccelerationTime;
            model.AxisHomingParameters.HomingInitialSpeed = HomingInitialSpeed;
            model.AxisHomingParameters.HomingMinimumSpeed = HomingMinimumSpeed;
            model.AxisHomingParameters.HomingMaximumSpeed = HomingMaximumSpeed;
            model.AxisHomingParameters.HomingRetractDistance = HomingRetractDistance;

            model.AxisStateLogicParameters.StateEnable = StateEnable;
            model.AxisStateLogicParameters.StateReverse = StateReverse;
            model.AxisStateLogicParameters.StopReaction = StopReaction;

            model.PulseRatioParameters.PulseRatio = PulseRatio;
            model.PulseRatioParameters.PulsesPerRevolution = PulsesPerRevolution;

            model.AxisSoftLimit.PositiveLimit = PositiveLimit;
            model.AxisSoftLimit.NegativeLimit = NegativeLimit;

            model.EncoderType = EncoderType ?? GKG.EncoderType.None;
            model.PulseInputMode = PulseInputMode ?? GKG.MotionControlPulseInputMode.PulseDirection;
            model.PulseOutputMode = PulseOutputMode ?? GKG.MotionControlPulseOutputModeConstants.Pulse;
            return model;
        }

        public void CommitPendingEditorChanges()
        {
            AxisName = AxisNameViewModel.Text?.Trim() ?? AxisName;

            SetHomingModeFromSelection(HomingModeComboxViewModel.SelectedItem);
            SetHomingDirectionFromSelection(HomingDirectionComboxViewModel.SelectedItem);
            SetEncoderTypeFromSelection(EncoderTypeComboxViewModel.SelectedItem);
            SetPulseInputModeFromSelection(PulseInputModeComboxViewModel.SelectedItem);
            SetPulseOutputModeFromSelection(PulseOutputModeComboxViewModel.SelectedItem);

            TrySetDouble(HomingAccelerationTimeViewModel, v => HomingAccelerationTime = v);
            TrySetDouble(HomingDecelerationTimeViewModel, v => HomingDecelerationTime = v);
            TrySetDouble(HomingInitialSpeedViewModel, v => HomingInitialSpeed = v);
            TrySetDouble(HomingMinimumSpeedViewModel, v => HomingMinimumSpeed = v);
            TrySetDouble(HomingMaximumSpeedViewModel, v => HomingMaximumSpeed = v);
            TrySetDouble(HomingRetractDistanceViewModel, v => HomingRetractDistance = v);
            TrySetInt(StopReactionViewModel, v => StopReaction = v);
            TrySetDouble(PulseRatioViewModel, v => PulseRatio = v);
            TrySetDouble(PulsesPerRevolutionViewModel, v => PulsesPerRevolution = v);
            TrySetDouble(PositiveLimitViewModel, v => PositiveLimit = v);
            TrySetDouble(NegativeLimitViewModel, v => NegativeLimit = v);
            TrySetInt(PulseOutputModeViewModel, v => PulseOutputMode = v);
        }

        public void EnsureDefaultAxisName()
        {
            if (!string.IsNullOrWhiteSpace(AxisName))
                return;

            AxisName = DefaultAxisName;
            AxisNameViewModel.Text = AxisName;
        }

        private static object? FindByValue(IEnumerable? items, object value)
        {
            return items?.OfType<ComBoxItem>().FirstOrDefault(i => Equals(i.Value, value));
        }

        private static List<ComBoxItem> CloneComBoxItems(IEnumerable<ComBoxItem> source)
        {
            return source
                .Select(x => new ComBoxItem
                {
                    Value = x.Value,
                    DisplayName = x.DisplayName,
                })
                .ToList();
        }

        private void SetHomingModeFromSelection(object? selection)
        {
            if (selection is ComBoxItem item && item.Value is GKG.MotionControlAxisHomingMode mode)
            {
                if (_homingMode != mode)
                    HomingMode = mode;
            }
        }

        private void SetHomingDirectionFromSelection(object? selection)
        {
            if (selection is ComBoxItem item && item.Value is int direction)
            {
                if (_homingDirection != direction)
                    HomingDirection = direction;
            }
        }

        private void SetEncoderTypeFromSelection(object? selection)
        {
            if (selection is ComBoxItem item && item.Value is GKG.EncoderType encoderType)
            {
                if (_encoderType != encoderType)
                    EncoderType = encoderType;
            }
        }

        private void SetPulseInputModeFromSelection(object? selection)
        {
            if (selection is ComBoxItem item && item.Value is GKG.MotionControlPulseInputMode pulseInputMode)
            {
                if (_pulseInputMode != pulseInputMode)
                    PulseInputMode = pulseInputMode;
            }
        }

        private void SetPulseOutputModeFromSelection(object? selection)
        {
            if (selection is ComBoxItem item && item.Value is int pulseOutputMode)
            {
                if (_pulseOutputMode != pulseOutputMode)
                    PulseOutputMode = pulseOutputMode;
            }
        }

        private static void TrySetInt(TextInputViewModel vm, Action<int> setter)
        {
            if (int.TryParse(vm.Text?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                setter(value);
        }

        private static void TrySetDouble(TextInputViewModel vm, Action<double> setter)
        {
            var text = vm.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                setter(value);
        }

        private static IReadOnlyList<ComBoxItem> BuildHomingModeItems()
        {
            var converter = new MotionControlAxisHomingModeToCnConverter();
            var list = new List<ComBoxItem>();
            foreach (var mode in Enum.GetValues<GKG.MotionControlAxisHomingMode>())
            {
                var display = converter.Convert(mode, typeof(string), null, CultureInfo.CurrentCulture)?.ToString() ?? mode.ToString();
                list.Add(new ComBoxItem
                {
                    Value = mode,
                    DisplayName = display,
                });
            }

            return list;
        }

        private static IReadOnlyList<ComBoxItem> BuildHomingDirectionItems()
        {
            return new List<ComBoxItem>
            {
                new ComBoxItem
                {
                    Value = GKG.MotionControlAxisHomingDirectionConstants.PositiveDirection,
                    DisplayName = "正向",
                },
                new ComBoxItem
                {
                    Value = GKG.MotionControlAxisHomingDirectionConstants.NegativeDirection,
                    DisplayName = "反向",
                },
            };
        }

        private static IReadOnlyList<ComBoxItem> BuildEncoderTypeItems()
        {
            return new List<ComBoxItem>
            {
                new ComBoxItem
                {
                    Value = GKG.EncoderType.None,
                    DisplayName = "\u65E0\u5916\u90E8\u7F16\u7801\u5668",
                },
                new ComBoxItem
                {
                    Value = GKG.EncoderType.External,
                    DisplayName = "\u6709\u5916\u90E8\u7F16\u7801\u5668",
                },
            };
        }

        private static IReadOnlyList<ComBoxItem> BuildPulseInputModeItems()
        {
            return new List<ComBoxItem>
            {
                new ComBoxItem
                {
                    Value = GKG.MotionControlPulseInputMode.QuadraAB,
                    DisplayName = "4\u500DAB\u76F8",
                },
                new ComBoxItem
                {
                    Value = GKG.MotionControlPulseInputMode.DoubleAB,
                    DisplayName = "2\u500DAB\u76F8",
                },
                new ComBoxItem
                {
                    Value = GKG.MotionControlPulseInputMode.SingleAB,
                    DisplayName = "1\u500DAB\u76F8",
                },
                new ComBoxItem
                {
                    Value = GKG.MotionControlPulseInputMode.DoublePulse,
                    DisplayName = "\u53CC\u8109\u51B2",
                },
                new ComBoxItem
                {
                    Value = GKG.MotionControlPulseInputMode.PulseDirection,
                    DisplayName = "\u8109\u51B2+\u65B9\u5411",
                },
            };
        }

        private static IReadOnlyList<ComBoxItem> BuildPulseOutputModeItems()
        {
            return new List<ComBoxItem>
            {
                new ComBoxItem
                {
                    Value = GKG.MotionControlPulseOutputModeConstants.Pulse,
                    DisplayName = "\u8109\u51B2",
                },
                new ComBoxItem
                {
                    Value = GKG.MotionControlPulseOutputModeConstants.Level,
                    DisplayName = "\u7535\u5E73",
                },
            };
        }

        private static string BuildRandomSuffix()
        {
            return Random.Shared.Next(1000, 10000).ToString();
        }

        private void MotionControlFactoryParameterViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isSuppressingRevision || !ShouldTrackRevision(e.PropertyName))
                return;

            Revision = System.Threading.Interlocked.Increment(ref s_revisionSeed);
        }

        private void ResetRevision()
        {
            Revision = 0;
        }

        private static bool ShouldTrackRevision(string? propertyName)
        {
            return propertyName == nameof(AxisName) ||
                   propertyName == nameof(HomingMode) ||
                   propertyName == nameof(HomingDirection) ||
                   propertyName == nameof(HomingAccelerationTime) ||
                   propertyName == nameof(HomingDecelerationTime) ||
                   propertyName == nameof(HomingInitialSpeed) ||
                   propertyName == nameof(HomingMinimumSpeed) ||
                   propertyName == nameof(HomingMaximumSpeed) ||
                   propertyName == nameof(HomingRetractDistance) ||
                   propertyName == nameof(StateEnable) ||
                   propertyName == nameof(StateReverse) ||
                   propertyName == nameof(StopReaction) ||
                   propertyName == nameof(PulseRatio) ||
                   propertyName == nameof(PulsesPerRevolution) ||
                   propertyName == nameof(PositiveLimit) ||
                   propertyName == nameof(NegativeLimit) ||
                   propertyName == nameof(EncoderType) ||
                   propertyName == nameof(PulseInputMode) ||
                   propertyName == nameof(PulseOutputMode);
        }
    }
}
