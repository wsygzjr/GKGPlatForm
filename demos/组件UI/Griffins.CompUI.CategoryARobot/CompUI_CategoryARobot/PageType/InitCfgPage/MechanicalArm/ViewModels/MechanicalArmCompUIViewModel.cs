using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace Griffins.CompUI.CategoryARobot.CompUI_CategoryARobot.PageType.InitCfgPage.MechanicalArm.ViewModels
{
    /// <summary>
    /// A类扩展运动控制机械手组件-视图模型
    /// </summary>
    public class MechanicalArmCompUIViewModel : ReactiveObject
    {
        #region 私有字段

        private readonly bool isDesign;

        private bool readOnly;

        private object viewTag;

        #endregion

        #region UI组件模型

        /// <summary>
        /// 点位运动模式-下拉框
        /// </summary>
        public ComboxViewModel PointMoveModeViewModel { get; }

        /// <summary>
        /// 轴号-数字输入框
        /// </summary>
        public NumericViewModel AxisNumberViewModel { get; }

        /// <summary>
        /// 分段速度模式-下拉框
        /// </summary>
        public ComboxViewModel SegmentSpeedModeViewModel { get; }

        /// <summary>
        /// 2D位置比较参数（Json文本）
        /// </summary>
        public string PositionComparison2DParametersJson
        {
            get => positionComparison2DParametersJson;
            set
            {
                this.RaiseAndSetIfChanged(ref positionComparison2DParametersJson, value);
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
        }
        private string positionComparison2DParametersJson = string.Empty;

        /// <summary>
        /// 运控前瞻参数（Json文本）
        /// </summary>
        public string ArcFeedForwardParametersJson
        {
            get => arcFeedForwardParametersJson;
            set
            {
                this.RaiseAndSetIfChanged(ref arcFeedForwardParametersJson, value);
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
        }
        private string arcFeedForwardParametersJson = string.Empty;

        /// <summary>
        /// 运动轨迹参数（Json文本）
        /// </summary>
        public string MotionTrajectoryJson
        {
            get => motionTrajectoryJson;
            set
            {
                this.RaiseAndSetIfChanged(ref motionTrajectoryJson, value);
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
        }
        private string motionTrajectoryJson = string.Empty;

        /// <summary>
        /// 2D位置比较参数（结构化编辑）
        /// </summary>
        public MotionControlPositionComparison2DParametersViewModel PositionComparison2DParametersViewModel { get; }

        /// <summary>
        /// 运控前瞻参数（结构化编辑）
        /// </summary>
        public MotionControlArcFeedForwardParametersViewModel ArcFeedForwardParametersViewModel { get; }

        #endregion

        #region 指令列表

        /// <summary>
        /// 指令列表
        /// </summary>
        public ObservableCollection<MechanicalArmCommandItemViewModel> Commands { get; }

        /// <summary>
        /// 添加指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        /// <summary>
        /// 删除指令
        /// </summary>
        public ReactiveCommand<MechanicalArmCommandItemViewModel, Unit> RemoveCommand { get; }

        #endregion

        #region 事件

        /// <summary>
        /// 事件（通知外部数据变更）
        /// </summary>
        public event EventHandler AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 对应View的Tag属性（支持双向绑定）
        /// </summary>
        public object ViewTag
        {
            get => viewTag;
            set => this.RaiseAndSetIfChanged(ref viewTag, value);
        }

        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                updateEnableStates();
            }
        }

        #endregion

        public MechanicalArmCompUIViewModel(bool isDesign)
        {
            this.isDesign = isDesign;

            // 左侧参数
            PointMoveModeViewModel = new();
            var pointMoveModeDisplayNames = new Dictionary<MotionMode, string>
            {
                { MotionMode.UnionControl, "联动" },
                { MotionMode.NotUnionControl, "不联动" },
            };
            PointMoveModeViewModel.ItemsSource = EnumExtensions.ToEnumItems(pointMoveModeDisplayNames);
            PointMoveModeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            AxisNumberViewModel = new()
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 1,
                Increment = 1,
            };

            SegmentSpeedModeViewModel = new();
            var segmentSpeedModeDisplayNames = new Dictionary<SegmentedSpeedMode, string>
            {
                { SegmentedSpeedMode.HightSpeed, "高速" },
                { SegmentedSpeedMode.MiddleSpeed, "中速" },
                { SegmentedSpeedMode.LowSpeed, "低速" },
            };
            SegmentSpeedModeViewModel.ItemsSource = EnumExtensions.ToEnumItems(segmentSpeedModeDisplayNames);
            SegmentSpeedModeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            PositionComparison2DParametersViewModel = new MotionControlPositionComparison2DParametersViewModel();
            ArcFeedForwardParametersViewModel = new MotionControlArcFeedForwardParametersViewModel();

            // 右侧列表
            Commands = new ObservableCollection<MechanicalArmCommandItemViewModel>();

            AddCommand = ReactiveCommand.Create(addCommandItem);
            RemoveCommand = ReactiveCommand.Create<MechanicalArmCommandItemViewModel>(removeCommandItem);

            subscribeValueChanges();
            updateEnableStates();
        }

        private void subscribeValueChanges()
        {
            PointMoveModeViewModel.ValueChanged += onValueChanged;
            AxisNumberViewModel.ValueChanged += onValueChanged;
            SegmentSpeedModeViewModel.ValueChanged += onValueChanged;

            PositionComparison2DParametersViewModel.AfterModified += onChildValueChanged;
            ArcFeedForwardParametersViewModel.AfterModified += onChildValueChanged;

            Commands.CollectionChanged += (_, __) =>
            {
                updateCommandIndex();
                subscribeCommandValueChanges();
                AfterModified?.Invoke(this, EventArgs.Empty);
            };
        }

        private void subscribeCommandValueChanges()
        {
            foreach (var item in Commands)
            {
                item.AfterModified -= onChildValueChanged;
                item.AfterModified += onChildValueChanged;
            }
        }

        private void onChildValueChanged(object sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        private void onValueChanged(object sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, EventArgs.Empty);
        }

        private void addCommandItem()
        {
            if (ReadOnly)
            {
                return;
            }

            var item = new MechanicalArmCommandItemViewModel();
            Commands.Add(item);
        }

        private void removeCommandItem(MechanicalArmCommandItemViewModel item)
        {
            if (ReadOnly)
            {
                return;
            }

            if (item == null)
            {
                return;
            }

            Commands.Remove(item);
        }

        private void updateCommandIndex()
        {
            for (int i = 0; i < Commands.Count; i++)
            {
                Commands[i].Index = i + 1;
            }
        }

        public void SetData(CategoryARobotSubMachineModulesInitCfg model)
        {
            model ??= new CategoryARobotSubMachineModulesInitCfg();

            PositionComparison2DParametersViewModel.SetData(model.PositionComparison2DParameters);
            ArcFeedForwardParametersViewModel.SetData(model.ArcFeedForwardParameters);

            // 兼容旧Json字段：保持可序列化/可回显
            PositionComparison2DParametersJson = ToJsonStringOrEmpty(PositionComparison2DParametersViewModel.GetData());
            ArcFeedForwardParametersJson = ToJsonStringOrEmpty(ArcFeedForwardParametersViewModel.GetData());
            MotionTrajectoryJson = ExtractMotionTrajectoryJson(PositionComparison2DParametersJson);

            if (PointMoveModeViewModel.ItemsSource != null)
            {
                var target = PointMoveModeViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault(o => o.Value is MotionMode v && v == model.motionMode);
                if (target != null)
                {
                    PointMoveModeViewModel.SelectedItem = target;
                }
            }

            var firstSpeed = (model.SegmentationSpeedList != null && model.SegmentationSpeedList.Length > 0)
                ? model.SegmentationSpeedList[0]
                : new SegmentationSpeed();

            AxisNumberViewModel.Value = firstSpeed.AxisNo;

            if (SegmentSpeedModeViewModel.ItemsSource != null)
            {
                var target = SegmentSpeedModeViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault(o => o.Value is SegmentedSpeedMode v && v == firstSpeed.Mode);
                if (target != null)
                {
                    SegmentSpeedModeViewModel.SelectedItem = target;
                }
            }

            Commands.Clear();
            if (firstSpeed.SegmentedSpeedRangeList != null)
            {
                foreach (var range in firstSpeed.SegmentedSpeedRangeList)
                {
                    var vm = new MechanicalArmCommandItemViewModel();
                    vm.CopyFrom(range);
                    Commands.Add(vm);
                }
            }

            updateCommandIndex();
        }

        public CategoryARobotSubMachineModulesInitCfg GetData()
        {
            var model = new CategoryARobotSubMachineModulesInitCfg();

            model.motionMode = (PointMoveModeViewModel.SelectedItem as ComBoxItem)?.Value is MotionMode pm ? pm : MotionMode.UnionControl;

            // 结构化数据优先
            model.PositionComparison2DParameters = PositionComparison2DParametersViewModel.GetData();
            model.ArcFeedForwardParameters = ArcFeedForwardParametersViewModel.GetData();

            // 兼容：将运动轨迹json回注入到PositionComparison2DParametersJson后再反序列化（如果用户仍编辑了Json文本）
            var mergedPositionComparison2DJson = MergeMotionTrajectoryJson(ToJsonStringOrEmpty(model.PositionComparison2DParameters), MotionTrajectoryJson);
            var mergedObj = FromJsonStringOrNull<MotionControlPositionComparison2DParameters>(mergedPositionComparison2DJson);
            if (mergedObj != null)
            {
                model.PositionComparison2DParameters = mergedObj;
            }

            var ranges = new List<SegmentedSpeedRange>();
            foreach (var cmdVm in Commands)
            {
                var range = new SegmentedSpeedRange();
                cmdVm.CopyTo(ref range);
                ranges.Add(range);
            }

            model.SegmentationSpeedList =
            [
                new SegmentationSpeed
                {
                    AxisNo = (int)AxisNumberViewModel.Value,
                    Mode = (SegmentSpeedModeViewModel.SelectedItem as ComBoxItem)?.Value is SegmentedSpeedMode sm ? sm : SegmentedSpeedMode.HightSpeed,
                    SegmentedSpeedRangeList = ranges.ToArray(),
                },
            ];

            return model;
        }

        private void updateEnableStates()
        {
            var enabled = !ReadOnly;

            PointMoveModeViewModel.IsEnabled = enabled;
            AxisNumberViewModel.IsEnabled = enabled;
            SegmentSpeedModeViewModel.IsEnabled = enabled;

            PositionComparison2DParametersViewModel.SetEnabled(enabled);
            ArcFeedForwardParametersViewModel.SetEnabled(enabled);
        }

        private static string ToJsonStringOrEmpty(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            try
            {
                var json = JsonSerializer.Serialize(obj);
                return json;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static T FromJsonStringOrNull<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                var obj = JsonSerializer.Deserialize<T>(json);
                return obj;
            }
            catch
            {
                return null;
            }
        }

        private static string MergeMotionTrajectoryJson(string positionComparison2DJson, string motionTrajectoryJson)
        {
            if (string.IsNullOrWhiteSpace(motionTrajectoryJson))
            {
                return positionComparison2DJson;
            }

            JsonObject root;
            try
            {
                root = string.IsNullOrWhiteSpace(positionComparison2DJson)
                    ? new JsonObject()
                    : JsonNode.Parse(positionComparison2DJson)?.AsObject() ?? new JsonObject();
            }
            catch
            {
                root = new JsonObject();
            }

            JsonNode trajectoryNode;
            try
            {
                trajectoryNode = JsonNode.Parse(motionTrajectoryJson);
            }
            catch
            {
                trajectoryNode = JsonValue.Create(motionTrajectoryJson);
            }

            // 尝试注入常见字段名（不同库/版本可能不同命名）
            SetOrCreate(root, "motionTrajectory", trajectoryNode);
            SetOrCreate(root, "motionTrajectoryList", trajectoryNode);
            SetOrCreate(root, "MotionTrajectory", trajectoryNode);
            SetOrCreate(root, "MotionTrajectoryList", trajectoryNode);

            return root.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = false,
            });
        }

        private static string ExtractMotionTrajectoryJson(string positionComparison2DJson)
        {
            if (string.IsNullOrWhiteSpace(positionComparison2DJson))
            {
                return string.Empty;
            }

            try
            {
                var root = JsonNode.Parse(positionComparison2DJson)?.AsObject();
                if (root == null)
                {
                    return string.Empty;
                }
                var keys = new[]
                {
                    "motionTrajectory",
                    "motionTrajectoryList",
                    "MotionTrajectory",
                    "MotionTrajectoryList",
                };

                foreach (var key in keys)
                {
                    if (TryGetPropertyIgnoreCase(root, key, out var token) && token != null)
                    {
                        return token.ToJsonString(new JsonSerializerOptions
                        {
                            WriteIndented = true,
                        });
                    }
                }
            }
            catch
            {
                // ignore
            }

            return string.Empty;
        }

        private static bool TryGetPropertyIgnoreCase(JsonObject obj, string key, out JsonNode token)
        {
            token = null;

            if (obj == null || string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            foreach (var kv in obj)
            {
                if (string.Equals(kv.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    token = kv.Value;
                    return true;
                }
            }

            return false;
        }

        private static void SetOrCreate(JsonObject obj, string key, JsonNode value)
        {
            if (obj == null || string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (TryGetPropertyIgnoreCase(obj, key, out var existingPropToken) && existingPropToken != null)
            {
                obj[key] = value;
                return;
            }

            obj[key] = value;
        }
    }

    /// <summary>
    /// 2D位置比较参数-ViewModel
    /// </summary>
    public class MotionControlPositionComparison2DParametersViewModel : ReactiveObject
    {
        public event EventHandler AfterModified;

        public bool IsEnabled
        {
            get => isEnabled;
            private set => this.RaiseAndSetIfChanged(ref isEnabled, value);
        }
        private bool isEnabled = true;

        /// <summary>
        /// 位置比较模式-下拉框
        /// </summary>
        public ComboxViewModel ComparisonModeViewModel { get; }

        public TextInputViewModel AxisListViewModel { get; }
        public TextInputViewModel ChannelListViewModel { get; }
        public NumericUpDownViewModel GpoChannel0ViewModel { get; }
        public NumericUpDownViewModel GpoChannel1ViewModel { get; }
        public NumericUpDownViewModel StartLevelViewModel { get; }
        public NumericUpDownViewModel OutputModeViewModel { get; }
        public NumericUpDownViewModel PulseWidthViewModel { get; }
        public NumericUpDownViewModel PulseOffTimeViewModel { get; }
        public NumericUpDownViewModel MaxPositionErrorViewModel { get; }
        public NumericUpDownViewModel ComparisonSourceViewModel { get; }
        public NumericUpDownViewModel CoordinateSystemIdViewModel { get; }
        public NumericUpDownViewModel AnticipationCorneringTimeViewModel { get; }
        public NumericUpDownViewModel MaxAccelerationViewModel { get; }

        /// <summary>启用比较（开关，与界面 ToggleSwitchControl 绑定）</summary>
        public ToggleSwitchViewModel ComparisonEnabledToggleViewModel { get; }

        public bool IsComparisonEnabled
        {
            get => ComparisonEnabledToggleViewModel.IsChecked;
            set => ComparisonEnabledToggleViewModel.IsChecked = value;
        }

        public MotionControlPositionComparisonMode ComparisonMode
        {
            get => comparisonMode;
            set
            {
                this.RaiseAndSetIfChanged(ref comparisonMode, value);
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
        }
        private MotionControlPositionComparisonMode comparisonMode = MotionControlPositionComparisonMode.None;

        public MotionControlPositionComparison2DParametersViewModel()
        {
            ComparisonModeViewModel = new ComboxViewModel();
            var compareModeDisplayNames = new Dictionary<MotionControlPositionComparisonMode, string>
            {
                { MotionControlPositionComparisonMode.None, "不启用" },
                { MotionControlPositionComparisonMode.OneDimensional, "1维比较" },
                { MotionControlPositionComparisonMode.TwoDimensionalSingleAxis, "2维单轴" },
                { MotionControlPositionComparisonMode.TwoDimensionalDualAxis, "2维双轴" },
            };
            ComparisonModeViewModel.ItemsSource = EnumExtensions.ToEnumItems(compareModeDisplayNames);
            ComparisonModeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            AxisListViewModel = new TextInputViewModel();
            ChannelListViewModel = new TextInputViewModel();

            GpoChannel0ViewModel = CreateIntNumeric(0, 0, 999);
            GpoChannel1ViewModel = CreateIntNumeric(1, 0, 999);
            StartLevelViewModel = CreateIntNumeric(0, 0, 1);
            OutputModeViewModel = CreateIntNumeric(0, 0, 1);
            PulseWidthViewModel = CreateIntNumeric(0, 0, int.MaxValue);
            PulseOffTimeViewModel = CreateIntNumeric(0, 0, int.MaxValue);
            MaxPositionErrorViewModel = CreateIntNumeric(100, 0, 511);
            ComparisonSourceViewModel = CreateIntNumeric(0, 0, 1);
            CoordinateSystemIdViewModel = CreateIntNumeric(0, 0, 1);
            AnticipationCorneringTimeViewModel = CreateIntNumeric(0, 0, 10);
            MaxAccelerationViewModel = CreateDoubleNumeric(0, 0, 999999);

            ComparisonEnabledToggleViewModel = new ToggleSwitchViewModel();
            ComparisonEnabledToggleViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);

            AxisListViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            ChannelListViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);

            ComparisonModeViewModel.ValueChanged += (_, __) =>
            {
                if ((ComparisonModeViewModel.SelectedItem as ComBoxItem)?.Value is MotionControlPositionComparisonMode mode)
                {
                    ComparisonMode = mode;
                }

                AfterModified?.Invoke(this, EventArgs.Empty);
            };

            GpoChannel0ViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            GpoChannel1ViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            StartLevelViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            OutputModeViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            PulseWidthViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            PulseOffTimeViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            MaxPositionErrorViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            ComparisonSourceViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            CoordinateSystemIdViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            AnticipationCorneringTimeViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            MaxAccelerationViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
        }

        public void SetEnabled(bool enabled)
        {
            IsEnabled = enabled;
            ComparisonModeViewModel.IsEnabled = enabled;
            AxisListViewModel.IsEnabled = enabled;
            ChannelListViewModel.IsEnabled = enabled;
            GpoChannel0ViewModel.IsEnabled = enabled;
            GpoChannel1ViewModel.IsEnabled = enabled;
            StartLevelViewModel.IsEnabled = enabled;
            OutputModeViewModel.IsEnabled = enabled;
            PulseWidthViewModel.IsEnabled = enabled;
            PulseOffTimeViewModel.IsEnabled = enabled;
            MaxPositionErrorViewModel.IsEnabled = enabled;
            ComparisonSourceViewModel.IsEnabled = enabled;
            CoordinateSystemIdViewModel.IsEnabled = enabled;
            AnticipationCorneringTimeViewModel.IsEnabled = enabled;
            MaxAccelerationViewModel.IsEnabled = enabled;
            ComparisonEnabledToggleViewModel.IsEnabled = enabled;
        }

        public void SetData(MotionControlPositionComparison2DParameters model)
        {
            model ??= new MotionControlPositionComparison2DParameters();

            AxisListViewModel.Text = JoinArray(model.AxisList);
            ChannelListViewModel.Text = JoinArray(model.ChannelList);

            GpoChannel0ViewModel.Value = (decimal)(model.GpoChannel0 ?? 0);
            GpoChannel1ViewModel.Value = (decimal)(model.GpoChannel1 ?? 1);
            StartLevelViewModel.Value = model.StartLevel;
            OutputModeViewModel.Value = model.OutputMode;
            PulseWidthViewModel.Value = model.PulseWidth;
            PulseOffTimeViewModel.Value = model.PulseOffTime;
            MaxPositionErrorViewModel.Value = model.MaxPositionError;
            ComparisonSourceViewModel.Value = model.ComparisonSource;
            CoordinateSystemIdViewModel.Value = (decimal)(model.CoordinateSystemId ?? 0);
            AnticipationCorneringTimeViewModel.Value = (decimal)(model.AnticipationCorneringTime ?? 0);
            MaxAccelerationViewModel.Value = (decimal)model.MaxAcceleration;

                ComparisonEnabledToggleViewModel.IsChecked = model.IsComparisonEnabled;
            ComparisonMode = model.ComparisonMode;

            if (ComparisonModeViewModel.ItemsSource != null)
            {
                var target = ComparisonModeViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault(o => o.Value is MotionControlPositionComparisonMode v && v == ComparisonMode);
                if (target != null)
                {
                    ComparisonModeViewModel.SelectedItem = target;
                }
            }
        }

        public MotionControlPositionComparison2DParameters GetData()
        {
            return new MotionControlPositionComparison2DParameters
            {
                AxisList = ParseIntArray(AxisListViewModel.Text),
                ChannelList = ParseIntArray(ChannelListViewModel.Text),
                GpoChannel0 = (int)GpoChannel0ViewModel.Value,
                GpoChannel1 = (int)GpoChannel1ViewModel.Value,
                StartLevel = (int)StartLevelViewModel.Value,
                OutputMode = (int)OutputModeViewModel.Value,
                PulseWidth = (int)PulseWidthViewModel.Value,
                PulseOffTime = (int)PulseOffTimeViewModel.Value,
                MaxPositionError = (int)MaxPositionErrorViewModel.Value,
                ComparisonSource = (int)ComparisonSourceViewModel.Value,
                CoordinateSystemId = (int)CoordinateSystemIdViewModel.Value,
                AnticipationCorneringTime = (int)AnticipationCorneringTimeViewModel.Value,
                MaxAcceleration = (double)MaxAccelerationViewModel.Value,
                IsComparisonEnabled = ComparisonEnabledToggleViewModel.IsChecked,
                ComparisonMode = ComparisonMode,
                CoordinateSystem = null,
                ChannelEnableStatus = null,
                FreeFIFO = null,
            };
        }

        private static NumericUpDownViewModel CreateIntNumeric(int value, int min, int max)
        {
            return new NumericUpDownViewModel
            {
                Minimum = min,
                Maximum = max,
                DecimalPlaces = 0,
                Increment = 1,
                Value = value,
                IsEnabled = true,
            };
        }

        private static NumericUpDownViewModel CreateDoubleNumeric(double value, double min, double max)
        {
            return new NumericUpDownViewModel
            {
                Minimum = (decimal)min,
                Maximum = (decimal)max,
                DecimalPlaces = 0,
                Increment = 1m,
                Value = (decimal)value,
                IsEnabled = true,
            };
        }

        private static string JoinArray<T>(IEnumerable<T> items)
        {
            if (items == null)
            {
                return string.Empty;
            }

            return string.Join(",", items);
        }

        private static int[] ParseIntArray(string text)
        {
            var list = new List<int>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return list.ToArray();
            }

            var parts = text.Split(new[] { ',', '，', ';', '；', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
            {
                if (int.TryParse(p.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)
                    || int.TryParse(p.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out v))
                {
                    list.Add(v);
                }
            }
            return list.ToArray();
        }
    }

    /// <summary>
    /// 运控前瞻参数-ViewModel
    /// </summary>
    public class MotionControlArcFeedForwardParametersViewModel : ReactiveObject
    {
        public event EventHandler AfterModified;

        public bool IsEnabled
        {
            get => isEnabled;
            private set => this.RaiseAndSetIfChanged(ref isEnabled, value);
        }
        private bool isEnabled = true;

        public NumericUpDownViewModel FeedForwardSegmentsViewModel { get; }
        public NumericUpDownViewModel FeedForwardTimeConstantViewModel { get; }
        public NumericUpDownViewModel RadiusRatioSquaredViewModel { get; }

        public TextInputViewModel VMaxViewModel { get; }
        public TextInputViewModel AMaxViewModel { get; }
        public TextInputViewModel VVariationMaxViewModel { get; }
        public TextInputViewModel PulseViewModel { get; }
        public TextInputViewModel AxisRelationViewModel { get; }

        public TextInputViewModel MachineConfigFileNameViewModel { get; }

        /// <summary>轨迹预处理开关</summary>
        public ToggleSwitchViewModel TrajectoryPreprocessingToggleViewModel { get; }

        public bool TrajectoryPreprocessingSwitch
        {
            get => TrajectoryPreprocessingToggleViewModel.IsChecked;
            set => TrajectoryPreprocessingToggleViewModel.IsChecked = value;
        }

        public NumericUpDownViewModel TrajectoryPreprocessingMinAngleViewModel { get; }
        public NumericUpDownViewModel TrajectoryPreprocessingMaxAngleViewModel { get; }
        public NumericUpDownViewModel TrajectoryPreprocessingErrorViewModel { get; }

        public NumericUpDownViewModel CentripetalAccelerationSwitchViewModel { get; }
        public NumericUpDownViewModel CentripetalAccelerationViewModel { get; }

        /// <summary>S 平滑段开关</summary>
        public ToggleSwitchViewModel SmoothingSegmentToggleViewModel { get; }

        public bool SmoothingSegmentSwitch
        {
            get => SmoothingSegmentToggleViewModel.IsChecked;
            set => SmoothingSegmentToggleViewModel.IsChecked = value;
        }

        public NumericUpDownViewModel SmoothingSegmentAccelerationViewModel { get; }
        public NumericUpDownViewModel SmoothingSegmentRatioViewModel { get; }
        public NumericUpDownViewModel SmoothingSegmentMinLengthViewModel { get; }

        public MotionControlArcFeedForwardParametersViewModel()
        {
            FeedForwardSegmentsViewModel = new NumericUpDownViewModel
            {
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 0,
                Increment = 1,
                Value = 200,
            };
            FeedForwardTimeConstantViewModel = CreateDoubleNumeric(0.01, 0, 999999);
            RadiusRatioSquaredViewModel = CreateDoubleNumeric(0, 0, 999999);

            VMaxViewModel = new TextInputViewModel();
            AMaxViewModel = new TextInputViewModel();
            VVariationMaxViewModel = new TextInputViewModel();
            PulseViewModel = new TextInputViewModel();
            AxisRelationViewModel = new TextInputViewModel();
            MachineConfigFileNameViewModel = new TextInputViewModel();

            TrajectoryPreprocessingToggleViewModel = new ToggleSwitchViewModel();
            SmoothingSegmentToggleViewModel = new ToggleSwitchViewModel();

            TrajectoryPreprocessingMinAngleViewModel = CreateDoubleNumeric(0, 0, 999999);
            TrajectoryPreprocessingMaxAngleViewModel = CreateDoubleNumeric(0, 0, 999999);
            TrajectoryPreprocessingErrorViewModel = CreateDoubleNumeric(0, 0, 999999);

            CentripetalAccelerationSwitchViewModel = new NumericUpDownViewModel
            {
                Minimum = 0,
                Maximum = 1,
                DecimalPlaces = 0,
                Increment = 1,
                Value = 0,
            };

            CentripetalAccelerationViewModel = new NumericUpDownViewModel
            {
                Minimum = 0,
                Maximum = 32767,
                DecimalPlaces = 0,
                Increment = 1,
                Value = 10,
            };

            SmoothingSegmentAccelerationViewModel = CreateDoubleNumeric(0, 0, 999999);
            SmoothingSegmentRatioViewModel = CreateDoubleNumeric(0, 0, 1);
            SmoothingSegmentMinLengthViewModel = CreateDoubleNumeric(0, 0, 999999);

            TrajectoryPreprocessingToggleViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            SmoothingSegmentToggleViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);

            subscribe();
        }

        public void SetEnabled(bool enabled)
        {
            IsEnabled = enabled;
            FeedForwardSegmentsViewModel.IsEnabled = enabled;
            FeedForwardTimeConstantViewModel.IsEnabled = enabled;
            RadiusRatioSquaredViewModel.IsEnabled = enabled;

            VMaxViewModel.IsEnabled = enabled;
            AMaxViewModel.IsEnabled = enabled;
            VVariationMaxViewModel.IsEnabled = enabled;
            PulseViewModel.IsEnabled = enabled;
            AxisRelationViewModel.IsEnabled = enabled;
            MachineConfigFileNameViewModel.IsEnabled = enabled;

            TrajectoryPreprocessingMinAngleViewModel.IsEnabled = enabled;
            TrajectoryPreprocessingMaxAngleViewModel.IsEnabled = enabled;
            TrajectoryPreprocessingErrorViewModel.IsEnabled = enabled;

            CentripetalAccelerationSwitchViewModel.IsEnabled = enabled;
            CentripetalAccelerationViewModel.IsEnabled = enabled;

            SmoothingSegmentAccelerationViewModel.IsEnabled = enabled;
            SmoothingSegmentRatioViewModel.IsEnabled = enabled;
            SmoothingSegmentMinLengthViewModel.IsEnabled = enabled;

            TrajectoryPreprocessingToggleViewModel.IsEnabled = enabled;
            SmoothingSegmentToggleViewModel.IsEnabled = enabled;
        }

        public void SetData(MotionControlArcFeedForwardParameters model)
        {
            model ??= new MotionControlArcFeedForwardParameters();

            FeedForwardSegmentsViewModel.Value = model.FeedForwardSegments;
            FeedForwardTimeConstantViewModel.Value = (decimal)model.FeedForwardTimeConstant;
            RadiusRatioSquaredViewModel.Value = (decimal)model.RadiusRatioSquared;

            VMaxViewModel.Text = JoinArray(model.VMax);
            AMaxViewModel.Text = JoinArray(model.AMax);
            VVariationMaxViewModel.Text = JoinArray(model.VVariationMax);
            PulseViewModel.Text = JoinArray(model.PULSE);
            AxisRelationViewModel.Text = JoinArray(model.AxisRelation);
            MachineConfigFileNameViewModel.Text = model.MachineConfigFileName ?? string.Empty;

            TrajectoryPreprocessingToggleViewModel.IsChecked = model.TrajectoryPreprocessingSwitch ?? false;
            TrajectoryPreprocessingMinAngleViewModel.Value = (decimal)model.TrajectoryPreprocessingMinAngle;
            TrajectoryPreprocessingMaxAngleViewModel.Value = (decimal)model.TrajectoryPreprocessingMaxAngle;
            TrajectoryPreprocessingErrorViewModel.Value = (decimal)model.TrajectoryPreprocessingError;

            CentripetalAccelerationSwitchViewModel.Value = (decimal)(model.CentripetalAccelerationSwitch ?? 0);
            CentripetalAccelerationViewModel.Value = model.CentripetalAcceleration;

            SmoothingSegmentToggleViewModel.IsChecked = model.SmoothingSegmentSwitch ?? false;
            SmoothingSegmentAccelerationViewModel.Value = (decimal)model.SmoothingSegmentAcceleration;
            SmoothingSegmentRatioViewModel.Value = (decimal)model.SmoothingSegmentRatio;
            SmoothingSegmentMinLengthViewModel.Value = (decimal)model.SmoothingSegmentMinLength;
        }

        public MotionControlArcFeedForwardParameters GetData()
        {
            return new MotionControlArcFeedForwardParameters
            {
                FeedForwardSegments = (int)FeedForwardSegmentsViewModel.Value,
                FeedForwardTimeConstant = (double)FeedForwardTimeConstantViewModel.Value,
                RadiusRatioSquared = (double)RadiusRatioSquaredViewModel.Value,
                VMax = ParseDoubleArray(VMaxViewModel.Text),
                AMax = ParseDoubleArray(AMaxViewModel.Text),
                VVariationMax = ParseDoubleArray(VVariationMaxViewModel.Text),
                PULSE = ParseDoubleArray(PulseViewModel.Text),
                AxisRelation = ParseShortArray(AxisRelationViewModel.Text),
                MachineConfigFileName = string.IsNullOrWhiteSpace(MachineConfigFileNameViewModel.Text) ? null : MachineConfigFileNameViewModel.Text,
                TrajectoryPreprocessingSwitch = TrajectoryPreprocessingToggleViewModel.IsChecked,
                TrajectoryPreprocessingMinAngle = (float)TrajectoryPreprocessingMinAngleViewModel.Value,
                TrajectoryPreprocessingMaxAngle = (float)TrajectoryPreprocessingMaxAngleViewModel.Value,
                TrajectoryPreprocessingError = (float)TrajectoryPreprocessingErrorViewModel.Value,
                CentripetalAccelerationSwitch = (short)CentripetalAccelerationSwitchViewModel.Value,
                CentripetalAcceleration = (short)CentripetalAccelerationViewModel.Value,
                SmoothingSegmentSwitch = SmoothingSegmentToggleViewModel.IsChecked,
                SmoothingSegmentAcceleration = (float)SmoothingSegmentAccelerationViewModel.Value,
                SmoothingSegmentRatio = (float)SmoothingSegmentRatioViewModel.Value,
                SmoothingSegmentMinLength = (float)SmoothingSegmentMinLengthViewModel.Value,
            };
        }

        private void subscribe()
        {
            FeedForwardSegmentsViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            FeedForwardTimeConstantViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            RadiusRatioSquaredViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);

            VMaxViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            AMaxViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            VVariationMaxViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            PulseViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            AxisRelationViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            MachineConfigFileNameViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);

            TrajectoryPreprocessingMinAngleViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            TrajectoryPreprocessingMaxAngleViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            TrajectoryPreprocessingErrorViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);

            CentripetalAccelerationSwitchViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            CentripetalAccelerationViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);

            SmoothingSegmentAccelerationViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            SmoothingSegmentRatioViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            SmoothingSegmentMinLengthViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private static NumericUpDownViewModel CreateDoubleNumeric(double value, double min, double max)
        {
            return new NumericUpDownViewModel
            {
                Minimum = (decimal)min,
                Maximum = (decimal)max,
                DecimalPlaces = 0,
                Increment = 1m,
                Value = (decimal)value,
                IsEnabled = true,
            };
        }

        private static string JoinArray<T>(IEnumerable<T> items)
        {
            if (items == null)
            {
                return string.Empty;
            }
            return string.Join(",", items);
        }

        private static double[] ParseDoubleArray(string text)
        {
            var list = new List<double>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return list.ToArray();
            }

            var parts = text.Split(new[] { ',', '，', ';', '；', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
            {
                if (double.TryParse(p.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v)
                    || double.TryParse(p.Trim(), NumberStyles.Float, CultureInfo.CurrentCulture, out v))
                {
                    list.Add(v);
                }
            }
            return list.ToArray();
        }

        private static short[] ParseShortArray(string text)
        {
            var list = new List<short>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return list.ToArray();
            }

            var parts = text.Split(new[] { ',', '，', ';', '；', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
            {
                if (short.TryParse(p.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)
                    || short.TryParse(p.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out v))
                {
                    list.Add(v);
                }
            }
            return list.ToArray();
        }
    }

    /// <summary>
    /// 机械手-指令项ViewModel
    /// </summary>
    public class MechanicalArmCommandItemViewModel : ReactiveObject
    {
        public int Index
        {
            get => index;
            set => this.RaiseAndSetIfChanged(ref index, value);
        }
        private int index;

        public TextInputViewModel RangeViewModel { get; }

        public NumericUpDownViewModel SpeedViewModel { get; }

        public NumericUpDownViewModel AccelerationViewModel { get; }

        public event EventHandler AfterModified;

        public double Range
        {
            get
            {
                if (double.TryParse(RangeViewModel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                {
                    return v;
                }

                if (double.TryParse(RangeViewModel.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out v))
                {
                    return v;
                }

                return 0;
            }
            set
            {
                RangeViewModel.Text = value.ToString(CultureInfo.InvariantCulture);
                this.RaisePropertyChanged(nameof(Range));
            }
        }

        public double Speed
        {
            get => (double)SpeedViewModel.Value;
            set
            {
                SpeedViewModel.Value = (decimal)value;
                this.RaisePropertyChanged(nameof(Speed));
            }
        }

        public double Acceleration
        {
            get => (double)AccelerationViewModel.Value;
            set
            {
                AccelerationViewModel.Value = (decimal)value;
                this.RaisePropertyChanged(nameof(Acceleration));
            }
        }

        public MechanicalArmCommandItemViewModel()
        {
            RangeViewModel = new TextInputViewModel();

            SpeedViewModel = new NumericUpDownViewModel
            {
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 3,
                Increment = 0.001m,
                Value = 0,
            };

            AccelerationViewModel = new NumericUpDownViewModel
            {
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 3,
                Increment = 0.001m,
                Value = 0,
            };

            RangeViewModel.ValueChanged += onValueChanged;
            SpeedViewModel.ValueChanged += onValueChanged;
            AccelerationViewModel.ValueChanged += onValueChanged;
        }

        private void onValueChanged(object sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, EventArgs.Empty);
        }

        public void CopyFrom(SegmentedSpeedRange model)
        {
            Range = model.Range;
            Speed = model.Speed;
            Acceleration = model.Accleration;
        }

        public void CopyTo(ref SegmentedSpeedRange model)
        {
            model.Range = Range;
            model.Speed = Speed;
            model.Accleration = Acceleration;
        }
    }
}
