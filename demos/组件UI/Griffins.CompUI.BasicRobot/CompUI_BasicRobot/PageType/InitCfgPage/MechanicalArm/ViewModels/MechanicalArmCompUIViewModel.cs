using GF_Gereric;
using GKG.SubMM;
using GKG.UI;

using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;

namespace Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage.MechanicalArm.ViewModels
{
    /// <summary>
    /// 基础运动机械手组件-视图模型
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
            set => this.RaiseAndSetIfChanged(ref readOnly, value);
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

            // 右侧列表
            Commands = new ObservableCollection<MechanicalArmCommandItemViewModel>();

            AddCommand = ReactiveCommand.Create(addCommandItem);
            RemoveCommand = ReactiveCommand.Create<MechanicalArmCommandItemViewModel>(removeCommandItem);

            subscribeValueChanges();
        }

        private void subscribeValueChanges()
        {
            PointMoveModeViewModel.ValueChanged += onValueChanged;
            AxisNumberViewModel.ValueChanged += onValueChanged;
            SegmentSpeedModeViewModel.ValueChanged += onValueChanged;

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

        public void SetData(BasicRobotSubMachineModulesInitCfg model)
        {
            model ??= new BasicRobotSubMachineModulesInitCfg();

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

        public BasicRobotSubMachineModulesInitCfg GetData()
        {
            var model = new BasicRobotSubMachineModulesInitCfg();

            model.motionMode = (PointMoveModeViewModel.SelectedItem as ComBoxItem)?.Value is MotionMode pm ? pm : MotionMode.UnionControl;

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
