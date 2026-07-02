using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Griffins;
using Griffins.ImeIOT;
using Griffins.UI.General;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
    // 告警事件控件的ViewModel
    public class UctlAlarmEventesViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        { 
            _viewReference = view; 
        }

        #region 私有字段
        private bool _readOnly;
        #endregion

        #region 绑定到View的属性

        private int _eventID;
        public int EventID
        {
            get => _eventID;
            set
            {
                this.RaiseAndSetIfChanged(ref _eventID, value);
            } 
        }
        private int _alarmKind;
        public int AlarmKind
        {
            get => _alarmKind;
            set
            {
                this.RaiseAndSetIfChanged(ref _alarmKind, value);
            }
        }
        public bool IsExecButtonEnabled => EventID!=0 && AlarmKind != 0;
        #endregion

        #region 命令（绑定执行按钮的Click）

        /// <summary>
        /// 执行按钮命令  
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExecCommand { get; }
         
        #endregion

        #region 事件定义（转发原有业务事件）
        public event ImeAlarmEventHandler? ImeAlarmEvent;
        #endregion

        #region 构造函数
        public UctlAlarmEventesViewModel()
        {
            this.RaisePropertyChanged(nameof(IsExecButtonEnabled));

            ExecCommand = ReactiveCommand.CreateFromTask(
                OnExecClicked,
                this.WhenAnyValue(vm => vm.IsExecButtonEnabled)
            );
        }
        #endregion

        #region 核心业务方法
        // 初始化事件列表
        public void Init()
        {
           
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 执行按钮点击逻辑 
        /// </summary>
        private async Task OnExecClicked()
        {
            try
            {
                ImeAlarmEvent?.Invoke(this, new ImeAlarmEventArgs(EventID, AlarmKind,DateTime.Now));
            }
            catch (Exception ex)
            {
                await Griffins.UI.General.MessageBox.ShowErrorDialog("错误", ex.Message, _viewReference);
            }
        } 
        #endregion
    }
}