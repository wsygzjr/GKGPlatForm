using Griffins.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels.Axis
{
    /// <summary>
    /// 软限位参数配置-视图模型
    /// </summary>
    public class SoftLimitParamViewModel : ReactiveObject
    {
        private double _positiveLimitPosition;
        private double _negativeLimitPosition;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 正极限位置-数据模型
        /// </summary>
        public NumericWithLableViewModel PositiveLimitPositionViewModel { get; }
        /// <summary>
        /// 负极限位置-数据模型
        /// </summary>
        public NumericWithLableViewModel NegativeLimitPositionViewModel { get; }
        /// <summary>
        /// 正极限位置（数字输入框）
        /// </summary>
        public double PositiveLimitPosition
        {
            get
            {
                _positiveLimitPosition = (double)PositiveLimitPositionViewModel.Value;
                return _positiveLimitPosition;
            }
            set
            {
                _positiveLimitPosition = value;
                PositiveLimitPositionViewModel.Value = (decimal)_positiveLimitPosition;
            }
        }
        /// <summary>
        /// 负极限位置（数字输入框）
        /// </summary>
        public double NegativeLimitPosition
        {
            get
            {
                _negativeLimitPosition = (double)NegativeLimitPositionViewModel.Value;
                return _negativeLimitPosition;
            }
            set
            {
                _negativeLimitPosition = value;
                NegativeLimitPositionViewModel.Value = (decimal)_negativeLimitPosition;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SoftLimitParamViewModel()
        {
            PositiveLimitPositionViewModel = new NumericWithLableViewModel() { DecimalPlaces = 1, Increment = (decimal)0.1 , LableText="mm" };
            NegativeLimitPositionViewModel = new NumericWithLableViewModel() { DecimalPlaces = 1, Increment = (decimal)0.1, LableText = "mm" };
            PositiveLimitPosition = 0;
            NegativeLimitPosition = 0;
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="softLimitParamInfo"></param>
        public void CopyFrom(SoftLimitParamInfo softLimitParamInfo)
        {
            this.PositiveLimitPosition = softLimitParamInfo.PositiveLimitPosition;
            this.NegativeLimitPosition = softLimitParamInfo.NegativeLimitPosition;

        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="softLimitParamInfo"></param>
        public void CopyTo(SoftLimitParamInfo softLimitParamInfo)
        {
            softLimitParamInfo.PositiveLimitPosition = this.PositiveLimitPosition;
            softLimitParamInfo.NegativeLimitPosition = this.NegativeLimitPosition;

        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            PositiveLimitPositionViewModel.ValueChanged += onValueChanged;
            NegativeLimitPositionViewModel.ValueChanged += onValueChanged;
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