using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.BasicParameters;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.BasicParameters
{
    /// <summary>
    /// 基础信息参数配置-视图模型
    /// </summary>
    public class BasicParametersViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        #region 子ViewModel（对应UI控件）
        /// <summary>
        /// 切换配方时是否弹窗向导校准页面-开关按钮ViewModel
        /// </summary>
        public ToggleSwitchViewModel IsShowCalibrationPageViewModel { get; }

        /// <summary>
        /// 暂停后恢复是否排胶称重-开关按钮ViewModel
        /// </summary>
        public ToggleSwitchViewModel IsDischargeGlueWhenResumeViewModel { get; }

        /// <summary>
        /// 启用拐角平滑-开关按钮ViewModel
        /// </summary>
        public ToggleSwitchViewModel EnableCornerSmoothingViewModel { get; }

        /// <summary>
        /// 暂停后回待机位-开关按钮ViewModel
        /// </summary>
        public ToggleSwitchViewModel IsReturnToStandbyViewModel { get; }

        /// <summary>
        /// 定时提醒校准时间间隔-数字输入框ViewModel
        /// </summary>
        public NumericViewModel CalibrationReminderIntervalViewModel { get; }

        /// <summary>
        /// 胶量报警后继续点胶板数-数字输入框ViewModel
        /// </summary>
        public NumericViewModel ContinueDispensingBoardsViewModel { get; }

        /// <summary>
        /// 拐角平滑系数-数字输入框ViewModel
        /// </summary>
        public NumericViewModel CornerSmoothingCoefficientViewModel { get; }

        /// <summary>
        /// 点胶头待机位置-视图模型
        /// </summary>

        public CamreaPositionViewModel GlueDspensingPositionViewModel { get; }

        ///// <summary>
        ///// 预点胶功能参数配置-视图模型
        ///// </summary>

        //public PreDispensingViewModel PreDispensingViewModel { get; }

        #endregion

        #region 绑定属性
        /// <summary>
        /// 切换配方时是否弹窗向导校准页面
        /// </summary>
        public bool IsShowCalibrationPageWhenSwitchingFormulas
        {
            get => IsShowCalibrationPageViewModel.IsChecked;
            set => IsShowCalibrationPageViewModel.IsChecked = value;
        }

        /// <summary>
        /// 暂停后恢复是否排胶称重
        /// </summary>
        public bool IsDischargeGlueAndWeighWhenResumePause
        {
            get => IsDischargeGlueWhenResumeViewModel.IsChecked;
            set => IsDischargeGlueWhenResumeViewModel.IsChecked = value;
        }

        /// <summary>
        /// 启用拐角平滑
        /// </summary>
        public bool EnableCornerSmoothing
        {
            get => EnableCornerSmoothingViewModel.IsChecked;
            set => EnableCornerSmoothingViewModel.IsChecked = value;
        }

        /// <summary>
        /// 暂停后回待机位
        /// </summary>
        public bool IsReturnToStandbyPositionAfterPause
        {
            get => IsReturnToStandbyViewModel.IsChecked;
            set => IsReturnToStandbyViewModel.IsChecked = value;
        }

        /// <summary>
        /// 定时提醒校准时间间隔
        /// </summary>
        public int CalibrationReminderInterval
        {
            get => (int)CalibrationReminderIntervalViewModel.Value;
            set => CalibrationReminderIntervalViewModel.Value = value;
        }

        /// <summary>
        /// 胶量报警后继续点胶板数
        /// </summary>
        public int ContinueDispensingBoardsAfterGlueAlarm
        {
            get => (int)ContinueDispensingBoardsViewModel.Value;
            set => ContinueDispensingBoardsViewModel.Value = value;
        }

        /// <summary>
        /// 拐角平滑系数
        /// </summary>
        public decimal CornerSmoothingCoefficient
        {
            get => CornerSmoothingCoefficientViewModel.Value;
            set => CornerSmoothingCoefficientViewModel.Value = value;
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public BasicParametersViewModel()
        {
            // 初始化所有子ViewModel
            IsShowCalibrationPageViewModel = new ToggleSwitchViewModel();
            IsDischargeGlueWhenResumeViewModel = new ToggleSwitchViewModel();
            EnableCornerSmoothingViewModel = new ToggleSwitchViewModel();
            IsReturnToStandbyViewModel = new ToggleSwitchViewModel();
            CalibrationReminderIntervalViewModel = new NumericViewModel()
            {
                DecimalPlaces = 0,
                Increment = 1m,
                //LableText = "h"
            };
            ContinueDispensingBoardsViewModel = new NumericViewModel()
            {
                DecimalPlaces =0,
                Increment = 1m,
            };
            CornerSmoothingCoefficientViewModel = new NumericViewModel()
            {
                DecimalPlaces = 0,
                Increment = 1m,
            };

            GlueDspensingPositionViewModel = new CamreaPositionViewModel();
            //PreDispensingViewModel = new PreDispensingViewModel();
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="cfgInfo">基础参数配置模型</param>
        public void CopyFrom(BasicParametersCfgInfo cfgInfo)
        {
            IsShowCalibrationPageWhenSwitchingFormulas = cfgInfo.IsShowCalibrationPageWhenSwitchingFormulas;
            IsDischargeGlueAndWeighWhenResumePause = cfgInfo.IsDischargeGlueAndWeighWhenResumePause;
            EnableCornerSmoothing = cfgInfo.EnableCornerSmoothing;
            IsReturnToStandbyPositionAfterPause = cfgInfo.IsReturnToStandbyPositionAfterPause;
            CalibrationReminderInterval = cfgInfo.CalibrationReminderInterval;
            ContinueDispensingBoardsAfterGlueAlarm = cfgInfo.ContinueDispensingBoardsAfterGlueAlarm;
            CornerSmoothingCoefficient = cfgInfo.CornerSmoothingCoefficient;

            GlueDspensingPositionViewModel.CopyFrom(cfgInfo.GlueDspensingPositionInfo);
            //PreDispensingViewModel.CopyFrom(cfgInfo.PreDispensingCfgInfo);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="cfgInfo">待填充的基础参数配置模型</param>
        public void CopyTo(BasicParametersCfgInfo cfgInfo)
        {
            cfgInfo.IsShowCalibrationPageWhenSwitchingFormulas = IsShowCalibrationPageWhenSwitchingFormulas;
            cfgInfo.IsDischargeGlueAndWeighWhenResumePause = IsDischargeGlueAndWeighWhenResumePause;
            cfgInfo.EnableCornerSmoothing = EnableCornerSmoothing;
            cfgInfo.IsReturnToStandbyPositionAfterPause = IsReturnToStandbyPositionAfterPause;
            cfgInfo.CalibrationReminderInterval = CalibrationReminderInterval;
            cfgInfo.ContinueDispensingBoardsAfterGlueAlarm = ContinueDispensingBoardsAfterGlueAlarm;
            cfgInfo.CornerSmoothingCoefficient = CornerSmoothingCoefficient;

            GlueDspensingPositionViewModel.CopyTo(cfgInfo.GlueDspensingPositionInfo);
            //PreDispensingViewModel.CopyTo(cfgInfo.PreDispensingCfgInfo);

        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            //GlobalVisionViewModel.CameraShowViewModel.SetViewReference(view);
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            //PreDispensingViewModel.AfterModified += onAfterModified;
            GlueDspensingPositionViewModel.AfterModified += onAfterModified; 

            IsShowCalibrationPageViewModel.ValueChanged += onValueChanged;
            IsDischargeGlueWhenResumeViewModel.ValueChanged += onValueChanged;
            EnableCornerSmoothingViewModel.ValueChanged += onValueChanged;
            IsReturnToStandbyViewModel.ValueChanged += onValueChanged;
            CalibrationReminderIntervalViewModel.ValueChanged += onValueChanged;
            ContinueDispensingBoardsViewModel.ValueChanged += onValueChanged;
            CornerSmoothingCoefficientViewModel.ValueChanged += onValueChanged;
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