using Griffins.UI;
using ReactiveUI;
using System.Reactive.Linq;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels.Axis
{
    /// <summary>
    /// 轴状态逻辑参数配置-视图模型
    /// </summary>
    public class AxisStateLogicParamViewModel : ReactiveObject
    {
        private bool _isStateEnabled;
        private bool _isStateInverted;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 感应信后反应方式-下拉框数据模型
        /// </summary>
        public ComboxViewModel SignalReactionModel { get; }

        /// <summary>
        /// 运控卡轴状态类型-下拉框数据模型
        /// </summary>
        public ComboxViewModel AxisStateTypeModel { get; }

        /// <summary>
        /// 状态是否启用-数据模型
        /// </summary>
        public ToggleSwitchViewModel IsStateEnabledViewModel { get; }
        /// <summary>
        /// 状态是否取反-数据模型
        /// </summary>
        public ToggleSwitchViewModel IsStateInvertedViewModel { get; }

        /// <summary>
        /// 选中的感应信号后反应方式
        /// </summary>
        public SignalReaction SelectedSignalReaction
        {

            get => (SignalReaction)((SignalReactionModel.SelectedItem as ComBoxItem)?.Value ?? SignalReaction.StopImmediately);
            set
            {
                if (SignalReactionModel.ItemsSource != null)
                {
                    var targetItem = SignalReactionModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (SignalReaction)o.Value == value);
                    if (targetItem != null)
                        SignalReactionModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedSignalReaction));
                }
            }
        }
        /// <summary>
        /// 运控卡轴状态类型（标签显示）
        /// </summary>
        public AxisStateType MotionControlCardAxisStateType
        {
            get => (AxisStateType)((AxisStateTypeModel.SelectedItem as ComBoxItem)?.Value ?? AxisStateType.EnableSignal);
            set
            {
                if (AxisStateTypeModel.ItemsSource != null)
                {
                    var targetItem = AxisStateTypeModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (AxisStateType)o.Value == value);
                    if (targetItem != null)
                        AxisStateTypeModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(MotionControlCardAxisStateType));
                }
            }
        }

        /// <summary>
        /// 状态是否启用（开关按钮）
        /// </summary>
        public bool IsStateEnabled
        {
            get
            {
                _isStateEnabled = IsStateEnabledViewModel.IsChecked;
                return _isStateEnabled;
            }
            set
            {
                _isStateEnabled = value;
                IsStateEnabledViewModel.IsChecked = _isStateEnabled;
            }
        }
        /// <summary>
        /// 状态是否取反（开关按钮）
        /// </summary>
        public bool IsStateInverted
        {
            get
            {
                _isStateInverted = IsStateInvertedViewModel.IsChecked;
                return _isStateInverted;
            }
            set
            {
                _isStateInverted = value;
                IsStateInvertedViewModel.IsChecked = _isStateInverted;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AxisStateLogicParamViewModel()
        {
            IsStateEnabledViewModel = new ToggleSwitchViewModel();
            IsStateInvertedViewModel = new ToggleSwitchViewModel();
            SignalReactionModel = new ComboxViewModel();
            var reactionDisplayNames = new Dictionary<SignalReaction, string>
            {
                { SignalReaction.StopImmediately, "立即停止" },
                { SignalReaction.StopSlow, "减速停止" }
            };
            SignalReactionModel.ItemsSource = EnumExtensions.ToEnumItems<SignalReaction>(reactionDisplayNames);
            SignalReactionModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            AxisStateTypeModel = new ComboxViewModel();
            var axisStateTypeDisplayNames = new Dictionary<AxisStateType, string>
            {
                { AxisStateType.OriginSignal, "原点信号" },
                { AxisStateType.AlarmSignal, "报警信号" },
                { AxisStateType.PositiveLimitSignal, "正限位信号" },
                { AxisStateType.NegativeLimitSignal, "负限位信号" },
                { AxisStateType.InpiSignal, "Inpi信号" }, 
                { AxisStateType.EZSignal, "EZ信号" },     
                { AxisStateType.EnableSignal, "使能信号" },
                { AxisStateType.PrepareCompleteSignal, "准备完成信号" }
            };
            AxisStateTypeModel.ItemsSource = EnumExtensions.ToEnumItems<AxisStateType>(axisStateTypeDisplayNames);
            AxisStateTypeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            // 订阅值变更事件
            subscribeValueChanges();
        }


        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="axisStateLogicParamInfo"></param>
        public void CopyFrom(AxisStateLogicParamInfo axisStateLogicParamInfo)
        {
            this.SelectedSignalReaction = axisStateLogicParamInfo.SignalReaction;
            this.MotionControlCardAxisStateType = axisStateLogicParamInfo.MotionControlCardAxisStateType;
            this.IsStateEnabled = axisStateLogicParamInfo.IsStateEnabled;
            this.IsStateInverted = axisStateLogicParamInfo.IsStateInverted;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="axisStateLogicParamInfo"></param>
        public void CopyTo(AxisStateLogicParamInfo axisStateLogicParamInfo)
        {
            axisStateLogicParamInfo.SignalReaction = this.SelectedSignalReaction;
            axisStateLogicParamInfo.MotionControlCardAxisStateType = this.MotionControlCardAxisStateType;
            axisStateLogicParamInfo.IsStateEnabled = this.IsStateEnabled;
            axisStateLogicParamInfo.IsStateInverted = this.IsStateInverted;

        }

        /// <summary>
        /// 订阅值变更事件
        /// </summary>
        private void subscribeValueChanges()
        {
            SignalReactionModel.ValueChanged += onValueChanged;
            AxisStateTypeModel.ValueChanged += onValueChanged;
            IsStateEnabledViewModel.ValueChanged += onValueChanged;
            IsStateInvertedViewModel.ValueChanged += onValueChanged;
        }
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
    }
}