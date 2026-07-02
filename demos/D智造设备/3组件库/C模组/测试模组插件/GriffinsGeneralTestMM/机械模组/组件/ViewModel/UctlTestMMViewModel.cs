using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.Threading;
using DynamicData;
using Griffins;
using Griffins.ImeIOT;
using Griffins.UI;
using Griffins.UI.Test;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace GriffinsGeneralTestMM
{
    public class UctlTestMMViewModel : ReactiveObject
    { 
        private GenMMInfo _genMMInfo = new();
        private MMAlias _alias;
        private string _title = "";
        private string _workState = "停止";
        private bool _readOnly;
        private int _execCount;
        private string _curMethod = "";
        private bool _paused;
        private DateTime _lastDT = DateTime.MinValue;
        private IMMCmdExecutorCallBack _callBack;
        private InnerControlPanelDefInfo? _selecteControlPanelDefInfo; 
        private ControlPanelDefInfoList _controlPanelDefInfos = new();
        private readonly InnerControlPanelDefInfoList _innerControlPanelDefInfos = new();
        private GFBaseTypePropValueList propValueInfoes=new();
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        #region 绑定属性

        // 消息列表（绑定到ListBox）
        private ObservableCollection<string> _messages = new();
        public ObservableCollection<string> Messages => _messages;

        /// <summary>
        /// 参数值信息列表VM
        /// </summary>
        public UIPropValueListViewModel CompPropValueListViewModel { get; }


        // 事件列表（绑定ComboBox的ItemsSource）
        internal ObservableCollection<InnerControlPanelDefInfo> ControlPanelDefInfos { get; } = new();

        // 选中的事件（绑定ComboBox的SelectedItem）
        internal InnerControlPanelDefInfo? SelectedControlPanelDefInfo
        {
            get => _selecteControlPanelDefInfo;
            set
            {
                this.RaiseAndSetIfChanged(ref _selecteControlPanelDefInfo, value);
                this.RaisePropertyChanged(nameof(IsExecButtonEnabled));  // 同步按钮状态
            }
        }  

        // 执行按钮是否启用（只读+选中项双重判断）
        public bool IsExecButtonEnabled => !_readOnly && ControlPanelDefInfos != null && ControlPanelDefInfos.Count > 0;

        /// <summary>
        /// 标题（包含模组名称和工作状态）
        /// </summary>
        public string Title
        {
            get => $"{_title}({_workState})";
            set
            {
                this.RaiseAndSetIfChanged(ref _title, value);
            }
        }

        /// <summary>
        /// 执行计数
        /// </summary>
        public int ExecCount
        {
            get => _execCount;
            set
            {
                this.RaiseAndSetIfChanged(ref _execCount, value);
            }
        }

        /// <summary>
        /// 当前执行方法
        /// </summary>
        public string CurMethod
        {
            get => _curMethod;
            set
            {
                this.RaiseAndSetIfChanged(ref _curMethod, value);
                this.RaisePropertyChanged(nameof(TitleBackgroundColor)); // 更新标题背景色
            }
        }

        /// <summary>
        /// 只读状态
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                //  设置只读时同步子控件 
                setReadOnly();
                this.RaisePropertyChanged(nameof(IsExecButtonEnabled));  // 同步按钮状态
            }
        }

        /// <summary>
        /// 暂停状态
        /// </summary>
        public bool Paused
        {
            get => _paused;
            set
            {
                this.RaiseAndSetIfChanged(ref _paused, value);
                this.RaisePropertyChanged(nameof(PauseMenuItemText)); // 更新菜单文本
            }
        }

        /// <summary>
        /// 暂停菜单文本（计算属性：暂停(&P)/恢复(&R)）
        /// </summary>
        public string PauseMenuItemText => _paused ? "恢复(&R)" : "暂停(&P)";

        /// <summary>
        /// 标题背景色（绑定到Panel背景：执行中红色，否则蓝色）
        /// </summary>
        public string TitleBackgroundColor => string.IsNullOrEmpty(_curMethod) ? "#4169E1" : "#FF0000";
        public event ImeCabilityEventHandler? CabilityEvent;
        public event ImeGenNormalEventHandler? GenNormalEvent;
        /// <summary>
        /// 界面数据对象属性值改变事件
        /// </summary>
        public event GFBaseTypeObjPropPathValueChangedEventHandler? AfterPropValueChanged;
        public UctlCabilityEventesViewModel UctlCabilityEventesViewModel { get; } = new();
        public UctlNormalEventesViewModel UctlNormalEventesViewModel { get; } = new();
        #endregion

        // 命令
        /// <summary>
        /// 执行命令：清空消息
        /// </summary>
        public ReactiveCommand<Unit, Unit> ClearMessagesCommand { get; }

        /// <summary>
        /// 执行命令：显示消息详情（修正泛型定义）
        /// TInput = string（传入选中的消息），TOutput = Unit（无返回值）
        /// </summary>
        public ReactiveCommand<string, Unit> ShowMessageDetailCommand { get; }

        /// <summary>
        /// 执行命令：切换暂停状态
        /// </summary>
        public ReactiveCommand<Unit, Unit> TogglePauseCommand { get; }

		/// <summary>
		/// 显示控制面板命令
		/// </summary>
		public ReactiveCommand<string, Unit> ShowControlPanelCommand { get; }
		/// <summary>
		/// 显示消息确认框命令
		/// </summary>
		public ReactiveCommand<string, Unit> ShowMessageBoxCommand { get; }

        public UctlTestMMViewModel(Control view)
        {
            _viewReference = view;
            // 初始化命令 
            ClearMessagesCommand = ReactiveCommand.Create(clearMessages); 
            ShowMessageDetailCommand = ReactiveCommand.Create<string>(ShowMessageDetail); 
            TogglePauseCommand = ReactiveCommand.Create(togglePause);
            ShowMessageBoxCommand = ReactiveCommand.Create<string>(showMessageBox);
            // 控制面板按钮状态
            this.RaisePropertyChanged(nameof(IsExecButtonEnabled));
            ShowControlPanelCommand = ReactiveCommand.Create<string>(showControlPanel, this.WhenAnyValue(vm => vm.IsExecButtonEnabled));
            
			// 订阅子控件事件
			UctlCabilityEventesViewModel.CabilityEvent += OnCabilityEvent;
            UctlNormalEventesViewModel.GenNormalEvent += OnGenNormalEvent;

            CompPropValueListViewModel = new UIPropValueListViewModel();
            CompPropValueListViewModel.CanEdit = false;
            CompPropValueListViewModel.AfterPropValueChanged += onAfterPropValueChanged;

            ExecCount = 0;
            CurMethod = "";
        }

        #region 公共方法 
        /// <summary>
        /// 初始化模组信息
        /// </summary>
        public void Init(GenMMInfo genMMInfo, MMAlias alias)
        {
            _genMMInfo = genMMInfo;
            _alias = alias;
            Title = $"{genMMInfo.MMName}[{alias.ToString()}]";

            CompPropValueListViewModel.SetViewReference(_viewReference);
            CompPropValueListViewModel.GetSubUIProObjItemNamesFunc = onGetSubUIProObjItemNamesOfRunTime;

            UctlCabilityEventesViewModel.Init(genMMInfo.CabilityEvents);
            UctlNormalEventesViewModel.Init(genMMInfo.NormalEvents);
            initControlPanelDefInfos(genMMInfo.ControlPanels);
        }
        /// <summary>
        /// 初始化测试面板的界面数据对象属性值设置界面
        /// </summary>
        public void AfterInit()
        {
            if (_genMMInfo.UIDataObjProps != null)
                propValueInfoes = CommFuncObj.CreateDefaultGFBaseTypeObjPropPathValues(_genMMInfo.UIDataObjProps.Props, onGetSubUIProObjItemNamesOfRunTime);
            CompPropValueListViewModel.Init(propValueInfoes, _genMMInfo.UIDataObjProps?.Props);
            //延时自动推送所有属性值改变
            relaySendChanged();
        }
        /// <summary>
        /// 设置实例名称
        /// </summary>
        public void SetInstanceName(string instanceName)
        {
            Title = string.IsNullOrEmpty(instanceName)
                ? $"{_genMMInfo.MMName}[{_alias.ToString()}]"
                : $"{_genMMInfo.MMName}[{instanceName}]";
        }

        /// <summary>
        /// 调整执行延迟时间
        /// </summary>
        public void AdjustCurDelyTime(int execPercent)
        {
            _genMMInfo.AdjustCurDelyTime(execPercent);
        }

        /// <summary>
        /// 获取界面属性值
        /// </summary>
        public GriffinsBaseValue GetUIDataObjPropValue(MPPropertyID propertyID)
        {
            return CompPropValueListViewModel.GetParamValue(propertyID.ToString());
        }

        /// <summary>
        /// 设置界面属性值
        /// </summary>
        public void SetUIDataObjPropValue(MPPropertyID propertyID, GriffinsBaseValue value)
        {
            CompPropValueListViewModel.SetParamValue(propertyID.ToString(), value);
        }

        /// <summary>
        /// 获取所有属性值
        /// </summary>
        public GFBaseTypePropValueList GetAllUIDataObjPropValues()
        {
            var values = CompPropValueListViewModel.GetUIPropValues();
            return values;
        }

        /// <summary>
        /// 设置工作状态
        /// </summary>
        public void SetWorkState(string workState)
        {
            _workState = workState;
            this.RaisePropertyChanged(nameof(Title));
        }

        /// <summary>
        /// 显示执行计数
        /// </summary>
        public void ShowExecCount()
        {
            ExecCount++;
            var ts = DateTime.Now - _lastDT;
            if (ts.TotalMilliseconds > 500)
            {
                _lastDT = DateTime.Now;
                this.RaisePropertyChanged(nameof(ExecCount));
            }
        }

        /// <summary>
        /// 设置执行方法
        /// </summary>
        public void SetExecMethod(bool execing, string curMethod = "")
        {
            CurMethod = execing ? curMethod : "";
        }

        /// <summary>
        /// 显示普通消息
        /// </summary>
        public void ShowMessage(string msg)
        {
            showMessageInternal(msg);
        }

        /// <summary>
        /// 显示错误消息
        /// </summary>
        public void ShowErrorMessage(string msg)
        {
            showMessageInternal(msg);
        }

        public void SetMMCmdExecutorCallBack(IMMCmdExecutorCallBack callBack) 
        {
            _callBack = callBack;
		}
        #endregion

        #region 私有方法 
        /// <summary>
        /// 在运行时下获取指定路径的属性的对象名称信息字典
        /// </summary>
        /// <param name="objInstPropPath">指定路径的属性</param>
        /// <returns></returns>
        private Dictionary<string, string> onGetSubUIProObjItemNamesOfRunTime(ObjInstPropPath objInstPropPath)
        {
            return _genMMInfo.IUIDataObjPropExChange?.GetSubUIProObjItemNamesOfRunTime(objInstPropPath);
        }

        // 初始化事件列表
        private void initControlPanelDefInfos(ControlPanelDefInfoList? defInfoes)
        {
            _controlPanelDefInfos = defInfoes ?? new ControlPanelDefInfoList();
            _innerControlPanelDefInfos.CopyFrom(_controlPanelDefInfos);

            // 线程安全更新UI列表（替代WinForms的Invoke）
            Dispatcher.UIThread.Post(() =>
            {
                ControlPanelDefInfos.Clear();
                foreach (var evt in _innerControlPanelDefInfos)
                {
                    ControlPanelDefInfos.Add(evt);
                }
                if (ControlPanelDefInfos.Count > 0)
                {
                    SelectedControlPanelDefInfo = ControlPanelDefInfos[0];
                }
            });
        }
        /// <summary>
        /// 显示消息到列表 
        /// </summary>
        private void showMessageInternal(string msg)
        {
            if (_paused) return;
             
			Dispatcher.UIThread.Post(() =>
			{
				var dt = DateTime.Now;
				// 修正毫秒格式化：0000 会显示4位，Millisecond最大999，改为000即可
				var formattedMsg = $"{dt:HH:mm:ss} {dt.Millisecond:000}=>{msg}";

				// 限制消息数量为1000条
				if (_messages.Count >= 1000)
					_messages.RemoveAt(0);

				// ObservableCollection 添加元素会自动触发UI更新，无需额外 RaisePropertyChanged
				_messages.Add(formattedMsg);
				this.RaisePropertyChanged(nameof(Messages));// 通知UI刷新 
			});
		}

        /// <summary>
        /// 设置只读状态
        /// </summary>
        private void setReadOnly()
        {
            CompPropValueListViewModel.CanEdit = !_readOnly;
            UctlCabilityEventesViewModel.ReadOnly = _readOnly;
            UctlNormalEventesViewModel.ReadOnly = _readOnly;
        }

        /// <summary>
        /// 清空消息列表
        /// </summary>
        private void clearMessages()
        {
            _messages.Clear();
        }

        /// <summary>
        /// 显示消息详情（命令逻辑）
        /// </summary>
        private void ShowMessageDetail(string selectedMsg)
        {
            if (string.IsNullOrEmpty(selectedMsg)) return;
            // 替换为Avalonia的消息详情窗口
            var detailWindow = new FormMessageDetailWindow(selectedMsg); 
            // 通过交互服务实现窗口弹出，避免ViewModel直接依赖View
            detailWindow.Show();
        }

        /// <summary>
        /// 切换暂停状态
        /// </summary>
        private void togglePause()
        {
            Paused = !_paused;
        }

        /// <summary>
        /// 显示控制面板
        /// </summary>
        private void showControlPanel(string controlPanelID)
        {
            if (string.IsNullOrWhiteSpace(controlPanelID))
            {
                return;
            }
            Task.Run(() =>
             {
                 try
                 {
                     _callBack?.ShowControlPanel(controlPanelID, Timeout.Infinite);
                 }
                 catch (Exception ex)
                 {
                     System.Diagnostics.Debug.WriteLine("显示控制面板出错：" + ex.Message);
                 }
             });
        }

		/// <summary>
		/// 显示消息框
		/// </summary>
		private void showMessageBox(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
            try
            {
				_callBack?.ShowMessageBox(text, "提示", Griffins.Map.ImeMessageBoxButtons.YesNo, Griffins.Map.ImeMessageBoxIcon.Question, 20 * 1000);
			}
            catch (Exception ex)
            {
				System.Diagnostics.Debug.WriteLine("显示消息框出错：" + ex.Message);
			}
		}
        /// <summary>
        /// 带路径的属性ID的值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterPropValueChanged(object sender, GFBaseTypeObjPropPathValueChangedEventArgs e)
        {
            AfterPropValueChanged?.Invoke(this, e);
        }
        private void OnCabilityEvent(object? sender, ImeCabilityEventArgs e)
        {
            CabilityEvent?.Invoke(this, e);
        }

        private void OnGenNormalEvent(object? sender, ImeGenNormalEventArgs e)
        {
            GenNormalEvent?.Invoke(this, e);
        }
        /// <summary>
        /// 延时自动推送所有属性值改变
        /// 正式服务下模拟插件自动推送所有属性值，用于在模拟终端显示所有属性值
        /// </summary>
        private void relaySendChanged()
        {
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                GFBaseTypePropValueList gfBaseTypePropValues = propValueInfoes;
                GFBaseTypeObjPropPathValueList gFBaseTypeObjPropPathValues = gfBaseTypePropValues.GetLeafGFBaseTypeObjPropPathValues(); ;
                onAfterPropValueChanged(this, new GFBaseTypeObjPropPathValueChangedEventArgs(gFBaseTypeObjPropPathValues));

            });

        }
        #endregion


    }
}