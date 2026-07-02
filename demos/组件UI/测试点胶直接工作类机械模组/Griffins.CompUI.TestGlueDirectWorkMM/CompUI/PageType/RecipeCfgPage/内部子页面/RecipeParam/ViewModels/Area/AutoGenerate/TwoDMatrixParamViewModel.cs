using Avalonia.Controls;
using Griffins.UI;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.AutoGenerate;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 二维矩阵参数-视图模型
    /// </summary>
    public class TwoDMatrixParamViewModel : OneDMatrixParamViewModel
    {
        #region 私有字段（数据源）

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;
        #endregion

        #region 响应式属性
        /// <summary>
        /// 二维矩阵参数
        /// </summary>
        public DMatrixParamViewModel TowOfDMatrixParamViewModel { get; }
        #endregion


        /// <summary>
        /// 构造函数（初始化组件模型、数据源、事件订阅）
        /// </summary>
        public TwoDMatrixParamViewModel()
        {
            TowOfDMatrixParamViewModel = new DMatrixParamViewModel();
            // 订阅值变更事件
            subscribeValueChanges();
        }

        #region 数据同步方法
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(TwoDMatrixParamCfgInfo matrixParamCfgInfo)
        {
            if (matrixParamCfgInfo == null)
            {
                resetToDefault();
                return;
            }
            base.CopyFrom(matrixParamCfgInfo);
            
            TowOfDMatrixParamViewModel.CopyFrom(matrixParamCfgInfo.TwoOfDMatrixParamCfgInfo);
            
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(TwoDMatrixParamCfgInfo matrixParamCfgInfo)
        {
            if (matrixParamCfgInfo == null) return;
            base.CopyTo(matrixParamCfgInfo);
            TowOfDMatrixParamViewModel.CopyTo(matrixParamCfgInfo.TwoOfDMatrixParamCfgInfo);
        }

        /// <summary>
        /// 设置视图引用（用于弹窗等UI操作）
        /// </summary>
        public new void SetViewReference(Control view)
        {
            _viewReference = view;
            base.SetViewReference(view);
            TowOfDMatrixParamViewModel.SetViewReference(view);
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 重置为默认值
        /// </summary>
        public new void resetToDefault()
        {
            TowOfDMatrixParamViewModel.resetToDefault();
        }

        /// <summary>
        /// 联动抬针相关配置的启用状态
        /// </summary>
        private void updateLiftNeedleRelatedControlEnabled(bool isEnabled)
        {
            LiftNeedleHeightViewModel.IsEnabled = isEnabled;
        }
        #endregion

        #region 事件订阅与联动逻辑
        /// <summary>
        /// 订阅子组件变更事件
        /// </summary>
        private void subscribeChildEvents()
        {
            // 下拉框事件
            SelectedTemplateModel.ValueChanged += onChildValueChanged;

            // 开关按钮事件
            EnableLiftNeedleOnNewLineViewModel.ValueChanged += onChildValueChanged;
            EnableCleanOnNewLineViewModel.ValueChanged += onChildValueChanged;

            // 数字输入框事件
            LiftNeedleHeightViewModel.ValueChanged += onChildValueChanged;
            NewLineFirstPointStableTimeViewModel.ValueChanged += onChildValueChanged;
        }

        /// <summary>
        /// 子组件值变更统一处理
        /// </summary>
        private void onChildValueChanged(object? sender, ValueChangedEventArgs e)
        {
            // 触发响应式属性通知（确保UI同步）
            this.RaisePropertyChanged(nameof(SelectedTemplateId));
            this.RaisePropertyChanged(nameof(EnableLiftNeedleOnNewLine));
            this.RaisePropertyChanged(nameof(EnableCleanOnNewLine));
            this.RaisePropertyChanged(nameof(LiftNeedleHeight));
            this.RaisePropertyChanged(nameof(NewLineFirstPointStableTime));
        }
        #endregion

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            TowOfDMatrixParamViewModel.AfterModified += onAfterModified;
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