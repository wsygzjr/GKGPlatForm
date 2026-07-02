using Griffins.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels.Axis
{
    /// <summary>
    /// 脉冲输入模式配置-视图模型
    /// </summary>
    public class PulseInputModeParamViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 感应信后反应方式-下拉框数据模型
        /// </summary>
        public ComboxViewModel PulseInputModeModel { get; }

        /// <summary>
        /// 选中的脉冲输入模式
        /// </summary>
        public PulseInputMode SelectedPulseInputMode
        {
           
            get => (PulseInputMode)((PulseInputModeModel.SelectedItem as ComBoxItem)?.Value ?? PulseInputMode.Times2ABPhase);
            set
            {
                if (PulseInputModeModel.ItemsSource != null)
                {
                    var targetItem = PulseInputModeModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (PulseInputMode)o.Value == value);
                    if (targetItem != null)
                        PulseInputModeModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedPulseInputMode));
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PulseInputModeParamViewModel()
        {
            PulseInputModeModel = new ComboxViewModel();
            var reactionDisplayNames = new Dictionary<PulseInputMode, string>
            {
                { PulseInputMode.Times4ABPhase, "4倍AB相" },
                { PulseInputMode.Times2ABPhase, "2倍AB相" },
                { PulseInputMode.Times1ABPhase, "1倍AB相" },
                { PulseInputMode.DoublePulse, "双脉冲" },
                { PulseInputMode.PulseDirection, "脉冲+方向" },
            };
            PulseInputModeModel.ItemsSource = EnumExtensions.ToEnumItems<PulseInputMode>(reactionDisplayNames);
            PulseInputModeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="pulseInputModeParamInfo"></param>
        public void CopyFrom(PulseInputModeParamInfo pulseInputModeParamInfo)
        {
            this.SelectedPulseInputMode = pulseInputModeParamInfo.PulseInputMode;

        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="pulseInputModeParamInfo"></param>
        public void CopyTo(PulseInputModeParamInfo pulseInputModeParamInfo)
        {
            pulseInputModeParamInfo.PulseInputMode = this.SelectedPulseInputMode;

        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            PulseInputModeModel.ValueChanged += onValueChanged;
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
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion
    }
}