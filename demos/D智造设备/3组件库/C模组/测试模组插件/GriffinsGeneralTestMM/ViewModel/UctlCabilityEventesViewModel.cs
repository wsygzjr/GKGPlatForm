using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Griffins;
using Griffins.ImeIOT;
using Griffins.UI;
using Griffins.UI.General;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;


namespace GriffinsGeneralTestMM
{
    /// <summary>
    /// 能力事件控件ViewModel：包含所有业务逻辑和状态
    /// </summary>
    public class UctlCabilityEventesViewModel : ReactiveObject
    {
        // 原控件核心字段
        private GenCabilityEventDefInfoList _eventDefInfoes = new();
        private InnerEventDefInfoList _events = new();
        private bool _readOnly;
        private InnerEventDefInfo? _selectedEvent;

        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Avalonia.Controls.Control view)
        {
            _viewReference = view;
            // 引用赋值后，可触发一次属性变更（可选）
            this.RaisePropertyChanged(nameof(IsButtonExecEnabled));
        }

        /// <summary>
        /// 能力事件触发 
        /// </summary>
        public event ImeCabilityEventHandler? CabilityEvent;

        /// <summary>
        /// 执行按钮命令  
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExecCommand { get; }

        public UctlCabilityEventesViewModel()
        {
            _events = new InnerEventDefInfoList();

            // CanExecute 动态观测（选中项/只读变化时自动更新按钮状态）
            ExecCommand = ReactiveCommand.CreateFromTask(
                OnExecClicked,
                this.WhenAnyValue(vm => vm.IsButtonExecEnabled)
            );
            SetButtonExecState();
        }

        #region 绑定属性（供View绑定）
        /// <summary>
        /// 事件列表（绑定到ComboBox的ItemsSource）
        /// </summary>
        public InnerEventDefInfoList Events
        {
            get => _events;
            set => this.RaiseAndSetIfChanged(ref _events, value);
        }

        /// <summary>
        /// 选中的事件（双向绑定ComboBox的SelectedItem）
        /// </summary>
        public InnerEventDefInfo? SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedEvent, value);
                SetButtonExecState();
            }
        }

        /// <summary>
        /// 只读状态（绑定到控件禁用逻辑）
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                SetReadOnly();
            }
        }

        /// <summary>
        /// 执行按钮是否启用（计算属性：只读/无选中项则禁用）
        /// </summary>
        public bool IsButtonExecEnabled
        {
            get
            {
                if (_readOnly) return false;
                return _selectedEvent != null;
            }
        }
        #endregion

        #region 外部接口 
        /// <summary>
        /// 初始化事件列表（和原Init方法逻辑一致）
        /// </summary>
        public void Init(GenCabilityEventDefInfoList eventDefInfoes)
        {
            _eventDefInfoes = eventDefInfoes ?? new GenCabilityEventDefInfoList();
            _events.CopyFrom(_eventDefInfoes);
            ShowEvents();
        }
        #endregion

        #region 核心业务逻辑
        /// <summary>
        /// 显示事件列表 
        /// </summary>
        private void ShowEvents()
        {
            this.RaisePropertyChanged(nameof(Events));
            // 默认选中第一个事件
            SelectedEvent = _events.Count > 0 ? _events[0] : null;
            SetButtonExecState();
        }

        /// <summary>
        /// 设置只读状态 
        /// </summary>
        private void SetReadOnly() => SetButtonExecState();

        /// <summary>
        /// 更新按钮启用状态 
        /// </summary>
        private void SetButtonExecState() => this.RaisePropertyChanged(nameof(IsButtonExecEnabled));

        /// <summary>
        /// 执行按钮点击逻辑 
        /// </summary>
        private async Task OnExecClicked()
        {
            try
            {
                if (SelectedEvent == null) return;

                //  调用异步方法，接收自定义返回对象 
                var paramResult = await getEventParamAsync(SelectedEvent.EventParamDefInfoes);
                //  通过返回对象的Success判断是否成功，EventParam获取参数
                if (paramResult.Success)
                {
                    // 触发能力事件 
                    CabilityEvent?.Invoke(this, new ImeCabilityEventArgs(SelectedEvent.EventID, paramResult.EventParam));
                }
            }
            catch (Exception ex)
            {
                // 关键修改：即使无View引用，也能显示错误提示 
                await MessageBox.ShowErrorDialog("错误", ex.Message , _viewReference);
            }
        }

        /// <summary>
        /// 获取事件参数
        /// </summary>
        /// <param name="eventParamDefInfoes">参数定义列表</param>
        /// <returns>封装了「是否成功」和「事件参数」的对象</returns>
        private async Task<EventParamGetResult> getEventParamAsync(GFParamDefInfoList eventParamDefInfoes)
        {
            // 初始化返回结果（默认失败，参数为空）
            var result = new EventParamGetResult { Success = false, EventParam = null };

            // 关键修改2：多层级获取父窗口 
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            // 无窗口上下文时的兜底处理
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，将以无窗口模式打开参数编辑界面" , _viewReference); 
            } 
            try
            {
                // 无参数定义，直接返回成功+空参数
                if (eventParamDefInfoes == null || eventParamDefInfoes.Count == 0)
                {
                    result.Success = true;
                    result.EventParam = null;
                    return result;
                }


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

                // 关键修改3：适配有无父窗口的场景
                if (parentWindow != null)
                {
                    // 有父窗口：以Dialog形式打开（模态）
                    var dialogResult = await editWindow.ShowDialog<bool>(parentWindow);
                    if (dialogResult)
                    {
                        result.Success = true;
                        result.EventParam = editViewModel.EventParam;
                    }
                }
                else
                {
                    // 无父窗口：以普通窗口打开（非模态）
                    editWindow.Show();
                    // 非模态窗口需等待用户操作（可选：根据你的窗口逻辑调整） 
                    result.Success = true;
                    result.EventParam = editViewModel.EventParam;
                }
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", ex.Message , _viewReference); 
            }

            return result;
        }
         
        #endregion

        #region 内部辅助类/数据模型 
        /// <summary>
        ///  获取事件参数的返回结果封装 
        /// </summary>
        public class EventParamGetResult
        {
            /// <summary>是否成功获取/编辑参数</summary>
            public bool Success { get; set; }
            /// <summary>获取/编辑后的事件参数</summary>
            public GFBaseTypeParamValueList? EventParam { get; set; }
        }

        /// <summary>
        /// 内部事件定义信息（数据模型，无业务逻辑）
        /// </summary>
        public class InnerEventDefInfo
        {
            public string EventID { get; set; } = string.Empty;
            public string EventName { get; set; } = string.Empty;
            public GFParamDefInfoList EventParamDefInfoes { get; set; } = new();

            /// <summary>
            /// 下拉框显示文本 
            /// </summary>
            public override string ToString() => string.IsNullOrEmpty(EventName) ? EventID : EventName;

            /// <summary>
            /// 从GenCabilityEventDefInfo复制数据 
            /// </summary>
            public void CopyFrom(GenCabilityEventDefInfo genCabilityEventDefInfo)
            {
                EventID = genCabilityEventDefInfo.EventID;
                EventName = genCabilityEventDefInfo.EventName;
                EventParamDefInfoes = genCabilityEventDefInfo.GetEventParamObjPropes();
            }
        }

        /// <summary>
        /// 内部事件定义列表（数据模型）
        /// </summary>
        public class InnerEventDefInfoList : List<InnerEventDefInfo>
        {
            /// <summary>
            /// 从GenCabilityEventDefInfoList复制数据 
            /// </summary>
            public void CopyFrom(GenCabilityEventDefInfoList genCabilityEventDefInfoes)
            {
                Clear();
                foreach (GenCabilityEventDefInfo info in genCabilityEventDefInfoes)
                {
                    var innerInfo = new InnerEventDefInfo();
                    innerInfo.CopyFrom(info);
                    Add(innerInfo);
                }
            }
        }
        #endregion
    }
}