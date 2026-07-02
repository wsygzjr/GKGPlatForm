using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Griffins;
using Griffins.ImeIOT;
using Griffins.UI;
using Griffins.UI.General;
using Newtonsoft.JsonG.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using static GriffinsGeneralTestMM.UctlCabilityEventesViewModel; 

namespace GriffinsGeneralTestMM
{
    // 普通事件控件的ViewModel
    public class UctlNormalEventesViewModel : ReactiveObject
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
        private GenNormalEventDefInfoList _eventDefInfoes = new();
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
                this.RaisePropertyChanged(nameof(IsExecButtonEnabled));  // 同步按钮状态
            } 
        }

        // 只读状态
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                this.RaisePropertyChanged(nameof(IsExecButtonEnabled));  // 同步按钮状态
            } 
        }

        // 执行按钮是否启用（只读+选中项双重判断）
        public bool IsExecButtonEnabled => !_readOnly && SelectedEvent != null;
        #endregion

        #region 命令（绑定执行按钮的Click）

        /// <summary>
        /// 执行按钮命令  
        /// </summary>
        public ReactiveCommand<Unit, Unit> ExecCommand { get; }
         
        #endregion

        #region 事件定义（转发原有业务事件）
        public event ImeGenNormalEventHandler? GenNormalEvent;
        #endregion

        #region 构造函数
        public UctlNormalEventesViewModel()
        {
            // 初始化按钮状态
            this.RaisePropertyChanged(nameof(IsExecButtonEnabled));

            // CanExecute 动态观测（选中项/只读变化时自动更新按钮状态）
            ExecCommand = ReactiveCommand.CreateFromTask(
                OnExecClicked,
                this.WhenAnyValue(vm => vm.IsExecButtonEnabled)
            );
        }
        #endregion

        #region 核心业务方法
        // 初始化事件列表
        public void Init(GenNormalEventDefInfoList? eventDefInfoes)
        {
            _eventDefInfoes = eventDefInfoes ?? new GenNormalEventDefInfoList();
            _events.CopyFrom(_eventDefInfoes);

            // 线程安全更新UI列表 
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

        /// <summary>
        /// 执行按钮点击逻辑 
        /// </summary>
        private async Task OnExecClicked()
        {
            try
            {
                if (SelectedEvent == null) 
                    return;

                //  调用异步方法，接收自定义返回对象 
                var paramResult = await getEventParamAsync(SelectedEvent.EventParamDefInfoes);
                //  通过返回对象的Success判断是否成功，EventParam获取参数
                if (paramResult.Success)
                {
                    // 触发能力事件 
                    GenNormalEvent?.Invoke(this, new ImeGenNormalEventArgs(SelectedEvent.EventKind, paramResult.EventParam));
                } 
            }
            catch (Exception ex)
            {
                await Griffins.UI.General.MessageBox.ShowErrorDialog("错误", ex.Message, _viewReference);
            }
        }

        /// <summary>
        /// 获取事件参数
        /// </summary>
        /// <param name="eventParamDefInfoes">参数定义列表</param>
        /// <returns>封装了「是否成功」和「事件参数」的对象</returns>  
        private async Task<EventParamGetResult> getEventParamAsync(GFParamDefInfoList eventParamDefInfoes)
        {
            var result = new EventParamGetResult { Success = false, EventParam = null };
            var parentWindow = _viewReference?.GetVisualRoot() as Window;

            try
            {
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

                bool dialogResult = await editWindow.ShowDialog<bool>(parentWindow);
                if (dialogResult)
                {
                    result.Success = true;
                    result.EventParam = editViewModel.EventParam;
                }
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", ex.Message, _viewReference);
            }

            return result;
        } 
        #endregion
         
        #region 内部类型 
        // 内部事件定义信息（对应原InnerEventDefInfo）
        public class InnerEventDefInfo
        {
            /// <summary>事件种类</summary>
            public int EventKind { get; set; }
            /// <summary>事件名称</summary>
            public string EventName { get; set; } = string.Empty;
            /// <summary>事件参数属性列表</summary>
            public GFParamDefInfoList EventParamDefInfoes { get; set; } = new();

            public override string ToString()
            {
                return string.IsNullOrEmpty(EventName) ? EventKind.ToString() : EventName;
            }

            public void CopyFrom(GenNormalEventDefInfo imeEventDefInfo)
            {
                EventKind = imeEventDefInfo.EventKind;
                EventName = imeEventDefInfo.EventName;
                EventParamDefInfoes = imeEventDefInfo.GetEventParamObjPropes();
            }
        }

        // 内部事件列表（对应原InnerEventDefInfoList）
        public class InnerEventDefInfoList : List<InnerEventDefInfo>
        {
            public void CopyFrom(GenNormalEventDefInfoList imeEventDefInfoes)
            {
                Clear();
                foreach (var genEvent in imeEventDefInfoes)
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