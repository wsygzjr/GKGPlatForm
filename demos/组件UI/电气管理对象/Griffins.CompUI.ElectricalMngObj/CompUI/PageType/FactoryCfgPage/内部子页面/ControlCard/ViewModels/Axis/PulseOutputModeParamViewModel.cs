using Griffins.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels.Axis
{
    /// <summary>
    /// 脉冲输出模式配置-视图模型
    /// </summary>
    public class PulseOutputModeParamViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 脉冲输出模式-下拉框数据模型
        /// </summary>
        public ComboxViewModel PulseOutputModeModel { get; }

        /// <summary>
        /// 选中的脉冲输出模式
        /// </summary>
        public PulseOutputMode SelectedPulseOutputMode
        {
            get => (PulseOutputMode)((PulseOutputModeModel.SelectedItem as ComBoxItem)?.Value ?? PulseOutputMode.DoublePulse_Low);
            set
            {
                if (PulseOutputModeModel.ItemsSource != null)
                {
                    var targetItem = PulseOutputModeModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (PulseOutputMode)o.Value == value);
                    if (targetItem != null)
                        PulseOutputModeModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedPulseOutputMode));
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PulseOutputModeParamViewModel()
        {
            PulseOutputModeModel = new ComboxViewModel();
            var reactionDisplayNames = new Dictionary<PulseOutputMode, string>
            {
                { PulseOutputMode.lowPulse_LowDirection, "脉冲低+方向低" },
                { PulseOutputMode.HighPulse_HighDirection, "脉冲高+方向高" },
                { PulseOutputMode.HighPulse_LowDirection, "脉冲高+方向低" },
                { PulseOutputMode.LowPulse_HighDirection, "脉冲低+方向高" },
                { PulseOutputMode.DoublePulse_High, "双脉冲高" },
                { PulseOutputMode.DoublePulse_Low, "双脉冲低" },
                { PulseOutputMode.AnalogQuantity, "模拟量" },
            };
            PulseOutputModeModel.ItemsSource = EnumExtensions.ToEnumItems<PulseOutputMode>(reactionDisplayNames) ;
            PulseOutputModeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="pulseOutputModeParamInfo"></param>
        public void CopyFrom(PulseOutputModeParamInfo pulseOutputModeParamInfo)
        {
            this.SelectedPulseOutputMode = pulseOutputModeParamInfo.PulseOutputMode;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="pulseOutputModeParamInfo"></param>
        public void CopyTo(PulseOutputModeParamInfo pulseOutputModeParamInfo)
        {
            pulseOutputModeParamInfo.PulseOutputMode = this.SelectedPulseOutputMode;
        }

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            PulseOutputModeModel.ValueChanged += onValueChanged;
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