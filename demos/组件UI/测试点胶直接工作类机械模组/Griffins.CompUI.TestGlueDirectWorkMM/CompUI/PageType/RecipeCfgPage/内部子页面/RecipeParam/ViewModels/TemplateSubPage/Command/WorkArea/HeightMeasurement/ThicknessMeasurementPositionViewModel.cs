using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 厚度测试位置 视图模型
    /// </summary>
    public class ThicknessMeasurementPositionViewModel : MeasurementPositionViewModel
    {
        /// <summary>
        /// 下限（mm）
        /// </summary>
        public NumericViewModel LowerLimitViewModel { get; }

        /// <summary>
        /// 上限（mm）
        /// </summary>
        public NumericViewModel UpperLimitViewModel { get; }

        /// <summary>
        /// 厚度（mm）
        /// </summary>
        public NumericViewModel ThicknessViewModel { get; }
        /// <summary>
        /// 下限（mm）
        /// </summary>
        public decimal LowerLimit
        {
            get => LowerLimitViewModel.Value;
            set => LowerLimitViewModel.Value = value;
        }

        /// <summary>
        /// 上限（mm）
        /// </summary>
        public decimal UpperLimit
        {
            get => UpperLimitViewModel.Value;
            set => UpperLimitViewModel.Value = value;
        }

        /// <summary>
        /// 厚度（mm）
        /// </summary>
        public decimal Thickness
        {
            get => ThicknessViewModel.Value;
            set => ThicknessViewModel.Value = value;
        }
       
        public ThicknessMeasurementPositionViewModel()
        {
            // 初始化下限（mm）：默认0.000，精度3位小数，步长0.001
            LowerLimitViewModel = new NumericViewModel
            {
                Increment = 0.001m,
                DecimalPlaces = 3,
                Minimum = 0.000m,
                Maximum = 100.000m,
                Value = 0.000m
            };

            // 初始化上限（mm）：默认50.000，精度3位小数，步长0.001
            UpperLimitViewModel = new NumericViewModel
            {
                Increment = 0.001m,
                DecimalPlaces = 3,
                Minimum = 0.001m,
                Maximum = 100.000m,
                Value = 50.000m
            };
            ThicknessViewModel = new NumericViewModel();
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型加载数据
        /// </summary>
        public void CopyFrom(ThicknessPositionInfo info)
        {
            if (info == null) return;
            base.CopyFrom(info); 

            this.UpperLimit=info.UpperLimit;
            this.LowerLimit=info.LowerLimit;
            this.Thickness=info.Thickness;
          
        }

        /// <summary>
        /// 将数据回写到数据模型
        /// </summary>
        public void CopyTo(ThicknessPositionInfo info)
        {
            if (info == null) return;
            base.CopyTo(info);
            info.UpperLimit = this.UpperLimit;
            info.LowerLimit = this.LowerLimit;
            info.Thickness = this.Thickness;

        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            LowerLimitViewModel.ValueChanged += onValueChanged;
            UpperLimitViewModel.ValueChanged += onValueChanged;
            ThicknessViewModel.ValueChanged += onValueChanged;
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