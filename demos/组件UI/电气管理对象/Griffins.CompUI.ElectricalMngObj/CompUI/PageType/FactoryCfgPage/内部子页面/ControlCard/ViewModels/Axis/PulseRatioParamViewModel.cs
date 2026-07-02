using Griffins.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels.Axis
{
    /// <summary>
    /// 脉冲比参数配置-视图模型
    /// </summary>
    public class PulseRatioParamViewModel : ReactiveObject
    {
        private double _pulseRatio;
        private int _pulsesPerRevolution;

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 脉冲比-数据模型
        /// </summary>
        public NumericWithLableViewModel PulseRatioViewModel { get; }
        /// <summary>
        /// 每转脉冲-数据模型
        /// </summary>
        public NumericWithLableViewModel PulsesPerRevolutionViewModel { get; }
        /// <summary>
        /// 脉冲比（数字输入框）
        /// </summary>
        public double PulseRatio
        {
            get
            {
                _pulseRatio = (double)PulseRatioViewModel.Value;
                return _pulseRatio;
            }
            set
            {
                _pulseRatio = value;
                PulseRatioViewModel.Value = (decimal)_pulseRatio;
            }
        }
        /// <summary>
        /// 每转脉冲（数字输入框）
        /// </summary>
        public int PulsesPerRevolution
        {
            get
            {
                _pulsesPerRevolution = (int)PulsesPerRevolutionViewModel.Value;
                return _pulsesPerRevolution;
            }
            set
            {
                _pulsesPerRevolution = value;
                PulsesPerRevolutionViewModel.Value = _pulsesPerRevolution;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PulseRatioParamViewModel()
        {
            PulseRatioViewModel = new NumericWithLableViewModel()
            {
                DecimalPlaces=1,
                Increment=(decimal)0.1 ,
                LableText = "mm/s"
            };
            PulsesPerRevolutionViewModel = new NumericWithLableViewModel()
            {
                DecimalPlaces = 1,
                Increment = (decimal)0.1 ,
                LableText = "mm/s"
            };
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="pulseRatioParamInfo"></param>
        public void CopyFrom(PulseRatioParamInfo pulseRatioParamInfo)
        {
            this.PulseRatio = pulseRatioParamInfo.PulseRatio;
            this.PulsesPerRevolution = pulseRatioParamInfo.PulsesPerRevolution;

        }
        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="pulseRatioParamInfo"></param>
        public void CopyTo(PulseRatioParamInfo pulseRatioParamInfo)
        {
            pulseRatioParamInfo.PulseRatio = this.PulseRatio;
            pulseRatioParamInfo.PulsesPerRevolution = this.PulsesPerRevolution;

        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            PulseRatioViewModel.ValueChanged += onValueChanged;
            PulsesPerRevolutionViewModel.ValueChanged += onValueChanged;
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