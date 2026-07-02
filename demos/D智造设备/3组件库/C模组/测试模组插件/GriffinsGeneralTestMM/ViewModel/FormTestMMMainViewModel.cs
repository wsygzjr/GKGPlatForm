using Avalonia;

using ReactiveUI;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive; 
using System.Windows.Input;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    /// 主窗口ViewModel：包含所有业务逻辑和状态
    /// </summary>
    public class FormTestMMMainViewModel : ReactiveObject
    {
        // 状态字段 
        private bool _isTestMode;
        private bool _closAllMsgShow;
        private int _execPercent = 5;  

        /// <summary>
        /// 参数变更事件 
        /// </summary>
        public event EventHandler? AfterParamChanged;
         
        /// <summary>
        /// 确认命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ButtonMMCommand { get; }
        /// <summary>
        /// 取消修改命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ButtonSubMMCommand { get; }

        public FormTestMMMainViewModel()
        {
            // 初始化命令
            ButtonMMCommand = ReactiveCommand.Create(OnButtonMMClicked);
            ButtonSubMMCommand = ReactiveCommand.Create(OnButtonSubMMClicked); 

            // 初始化状态逻辑 
            DoAfterTestModeChanged();
        }

        #region 绑定属性（供View绑定 ）
        /// <summary>
        /// 是否为测试模式（绑定到单选按钮）
        /// </summary>
        public bool IsTestMode
        {
            get => _isTestMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _isTestMode, value); 
                DoAfterTestModeChanged(); // 状态变更后执行业务逻辑
            } 
        }

        /// <summary>
        /// 关闭所有信息显示（绑定到复选框）
        /// </summary>
        public bool ClosAllMsgShow
        {
            get => _closAllMsgShow;
            set
            {
                this.RaiseAndSetIfChanged(ref _closAllMsgShow, value);
                DoAfterParamChanged(); // 状态变更后执行业务逻辑
            } 
        }

        /// <summary>
        /// 执行延迟时间百分比（绑定到数值输入框）
        /// </summary>
        public int ExecPercent
        {
            get => _execPercent;
            set
            {
                this.RaiseAndSetIfChanged(ref _execPercent, value);
                DoAfterParamChanged(); // 属性变更触发全局事件
            } 
        }

        /// <summary>
        /// 演示面板是否启用（计算属性：绑定到Panel的IsEnabled）
        /// </summary>
        public bool IsDemoPanelEnabled => !_isTestMode;
        #endregion

        #region 业务逻辑方法
        /// <summary>
        /// 触发参数变更事件 
        /// </summary>
        private void DoAfterParamChanged()
        {
            AfterParamChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 测试模式变更后的业务逻辑 
        /// </summary>
        private void DoAfterTestModeChanged()
        {
            // 通知UI更新演示面板状态
            this.RaisePropertyChanged(nameof(IsDemoPanelEnabled)); 
            if (_isTestMode)
            {
                // 测试模式：强制重置参数
                ExecPercent = 0;
                ClosAllMsgShow = true;
            }
            else
            {
                // 演示模式：同步参数并触发事件
                DoAfterParamChanged();
            }
        }

        /// <summary>
        /// 机械模组监视按钮点击逻辑
        /// </summary>
        private void OnButtonMMClicked()
        {
            TestMMMain.Show();
        }

        /// <summary>
        /// 子机械模组监视按钮点击逻辑
        /// </summary>
        private void OnButtonSubMMClicked()
        { 
            TestSubMMMain.Show();
        }
        #endregion
         
    }
}