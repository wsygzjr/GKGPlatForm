using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Griffins;
using Griffins.ImeIOT;
using Griffins.UI;
using Griffins.UI.General;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
//using System.Windows.Input;

namespace GriffinsGeneralTestMM
{
    // 能力事件控件的ViewModel
    public class CabilityEventesViewModel : ReactiveObject
    {
        #region 私有字段
        private GenCabilityEventDefInfoList _eventDefInfoes = new();
        private readonly InnerEventDefInfoList _events = new();
        private bool _readOnly;
        private InnerEventDefInfo? _selectedEvent;
        #endregion

        #region 绑定到View的属性
        // 事件列表（绑定ComboBox的ItemsSource）
        public ObservableCollection<InnerEventDefInfo> Events { get; } = new();

        // 选中的事件（绑定ComboBox的SelectedItem）
        public InnerEventDefInfo? SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedEvent, value);
                this.RaisePropertyChanged(nameof(IsExecButtonEnabled)); // 同步按钮状态 
            }
        }

        // 只读状态
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                this.RaisePropertyChanged(nameof(IsExecButtonEnabled)); // 同步按钮状态 
            }
        }

        // 执行按钮是否启用（只读+选中项双重判断）
        public bool IsExecButtonEnabled => !_readOnly && SelectedEvent != null;
        #endregion

        #region 命令（绑定执行按钮的Click）
        /// <summary>
        /// 执行命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExecCommand { get; } 
        #endregion

        #region 事件定义（转发原有业务事件）
        public event ImeCabilityEventHandler? CabilityEvent;
        #endregion

        #region 构造函数
        public CabilityEventesViewModel()
        {
            // 初始化按钮状态 
            this.RaisePropertyChanged(nameof(IsExecButtonEnabled));
            //监听当前属性值的变化，与原始值对比,只要任意一个属性的当前值 ≠ 原始值，就允许保存（canSave = true）
            var canMove = this.WhenAnyValue(x => x.IsExecButtonEnabled);  
            ExecCommand = ReactiveCommand.Create(ExecuteEvent,canMove);
        }
        #endregion

        #region 核心业务方法
        // 初始化事件列表
        public void Init(GenCabilityEventDefInfoList? eventDefInfoes)
        {
            _eventDefInfoes = eventDefInfoes ?? new GenCabilityEventDefInfoList();
            _events.CopyFrom(_eventDefInfoes);

            // 线程安全更新UI列表（替代WinForms的Invoke）
            Dispatcher.UIThread.Post(() =>
            {
                Events.Clear();
                foreach (var evt in _events)
                {
                    Events.Add(evt);
                }
                if (Events.Count > 0)
                {
                    SelectedEvent = Events[0];
                }
            });
        }
        #endregion

        #region 私有方法
        // 执行选中的事件
        private void ExecuteEvent()
        {
            if (SelectedEvent == null) return;

            // 获取事件参数（保留原有逻辑，注释部分可按需恢复）
            if (!GetEventParam(SelectedEvent.EventParamDefInfoes, out var eventParam))
            {
                return;
            }

            // 触发能力事件
            CabilityEvent?.Invoke(this, new ImeCabilityEventArgs(SelectedEvent.EventID, eventParam));
        }

        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;

        // 获取事件参数 （zgl该代码不完整）
        private bool GetEventParam(GFParamDefInfoList? eventParamDefInfoes, out GFBaseTypeParamValueList? eventParam)
        {
            eventParam = null;
            if (eventParamDefInfoes == null || eventParamDefInfoes.Count == 0)
            {
                eventParam = null;
                return true;
            }

            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            { 
                    MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference); 
                return false;
            }
            try
            { 
                // 将eventParamDefInfoes 转换为 List<PropertyValueInfo>
                List<PropertyValueInfo> propertyValueInfoes = Eventes.ConvertGFParamDefInfoListToPropertyValueInfoList(eventParamDefInfoes);
                // 传入有效值
                var editViewModel = new FormEventParamWindowViewModel(propertyValueInfoes, _viewReference);

                var editWindow = new FormEventParamWindow
                {
                    DataContext = editViewModel,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                editViewModel.SetViewReference(editWindow);

                // 无父窗口：以普通窗口打开（非模态）
                editWindow.Show();
                // 非模态窗口需等待用户操作（可选：根据你的窗口逻辑调整） 
                eventParam = editViewModel.EventParam;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.ShowErrorDialog("错误", ex.Message, _viewReference);
                return false;
            }
        } 

        #endregion

        #region 内部类型（保留原有业务逻辑）
        // 内部事件定义信息（对应原InnerEventDefInfo）
        public class InnerEventDefInfo
        {
            /// <summary>事件ID</summary>
            public string EventID { get; set; } = string.Empty;
            /// <summary>事件名称</summary>
            public string EventName { get; set; } = string.Empty;
            /// <summary>事件参数属性列表</summary>
            public GFParamDefInfoList EventParamDefInfoes { get; set; } = new();

            public override string ToString()
            {
                return string.IsNullOrEmpty(EventName) ? EventID : EventName;
            }

            public void CopyFrom(GenCabilityEventDefInfo genCabilityEventDefInfo)
            {
                EventID = genCabilityEventDefInfo.EventID;
                EventName = genCabilityEventDefInfo.EventName;
                EventParamDefInfoes = genCabilityEventDefInfo.GetEventParamObjPropes();
            }
        }

        // 内部事件列表（对应原InnerEventDefInfoList）
        public class InnerEventDefInfoList : List<InnerEventDefInfo>
        {
            public void CopyFrom(GenCabilityEventDefInfoList genCabilityEventDefInfoes)
            {
                Clear();
                foreach (var genEvent in genCabilityEventDefInfoes)
                {
                    var innerEvent = new InnerEventDefInfo();
                    innerEvent.CopyFrom(genEvent);
                    Add(innerEvent);
                }
            }
        }
        #endregion
    }
}