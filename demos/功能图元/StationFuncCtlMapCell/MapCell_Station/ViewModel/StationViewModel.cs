using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GKG.Map.StationFuncCtlMapCell.ViewModel
{
    /// <summary>
    /// 工位图元核心视图模型 (ViewModel)
    /// 遵循标准的 MVVM 与响应式 (Reactive) 设计原则，纯粹地向 UI 暴露数据状态和交互命令。
    /// </summary>
    public class StationViewModel : ReactiveObject, IActivatableViewModel, IDisposable
    {
        private readonly StationPropertyModelEdit _model;

        #region 事件与命令 (Event & Commands)

        /// <summary>
        /// UI 触发左侧挡板点击时触发的事件，供图元控制器 (MapCellStationCtlObj) 订阅处理。
        /// 采用 event 关键字保护委托，防止被外部类意外覆盖或清空。
        /// </summary>
        public event Action? OnLeftJackingClicked;

        /// <summary>
        /// UI 触发右侧挡板点击时触发的事件，供图元控制器订阅处理。
        /// </summary>
        public event Action? OnRightJackingClicked;

        /// <summary>
        /// 左侧气缸点击命令 (绑定到 UI 按钮)
        /// </summary>
        public ICommand LeftJackingClickCommand { get; }

        /// <summary>
        /// 右侧气缸点击命令 (绑定到 UI 按钮)
        /// </summary>
        public ICommand RightJackingClickCommand { get; }

        /// <summary>
        /// ReactiveUI 的视图模型激活器，负责管理响应式绑定的生命周期，防止内存泄漏
        /// </summary>
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        #endregion

        #region 构造与生命周期管理

        /// <summary>
        /// 实例化工位视图模型
        /// </summary>
        /// <param name="model">底层图元数据编辑模型</param>
        /// <exception cref="ArgumentNullException">模型实例为空时抛出异常</exception>
        public StationViewModel(StationPropertyModelEdit model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));

            // 初始化点击命令：当 UI 按钮触发 Command 时，向外发出 Action 广播
            LeftJackingClickCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                OnLeftJackingClicked?.Invoke();
                await Task.Delay(500); // 500ms 的绝对防抖窗口
            });

            RightJackingClickCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                OnRightJackingClicked?.Invoke();
                await Task.Delay(500); // 500ms 的绝对防抖窗口
            });

            // 响应式按需监听：当控件显示在屏幕上时激活订阅，隐藏或销毁时自动注销
            this.WhenActivated(disposables =>
            {
                // 精准订阅底层 Model 的属性变更，并通知前端 UI 进行局部刷新，彻底消灭全量重绘
                _model.WhenAnyValue(x => x.HasLeftJacking).Subscribe(_ => this.RaisePropertyChanged(nameof(HasLeftJacking))).DisposeWith(disposables);
                _model.WhenAnyValue(x => x.HasRightJacking).Subscribe(_ => this.RaisePropertyChanged(nameof(HasRightJacking))).DisposeWith(disposables);
                _model.WhenAnyValue(x => x.LeftJackingState).Subscribe(_ => this.RaisePropertyChanged(nameof(LeftJackingState))).DisposeWith(disposables);
                _model.WhenAnyValue(x => x.RightJackingState).Subscribe(_ => this.RaisePropertyChanged(nameof(RightJackingState))).DisposeWith(disposables);
                _model.WhenAnyValue(x => x.LeftSensorStatus).Subscribe(_ => this.RaisePropertyChanged(nameof(LeftSensorStatus))).DisposeWith(disposables);
                _model.WhenAnyValue(x => x.RightSensorStatus).Subscribe(_ => this.RaisePropertyChanged(nameof(RightSensorStatus))).DisposeWith(disposables);
                _model.WhenAnyValue(x => x.HasBoard).Subscribe(_ => this.RaisePropertyChanged(nameof(HasBoard))).DisposeWith(disposables);
            });
        }

        #endregion

        #region 1. 映射的被动 UI 属性 (单一数据源)

        /// <summary>配置：是否包含左侧挡板</summary>
        public bool HasLeftJacking => _model.HasLeftJacking;

        /// <summary>配置：是否包含右侧挡板</summary>
        public bool HasRightJacking => _model.HasRightJacking;

        /// <summary>运行时：左侧挡板/气缸状态</summary>
        public int LeftJackingState => _model.LeftJackingState;

        /// <summary>运行时：右侧挡板/气缸状态</summary>
        public int RightJackingState => _model.RightJackingState;

        /// <summary>运行时：左侧感应器状态</summary>
        public bool LeftSensorStatus => _model.LeftSensorStatus;

        /// <summary>运行时：右侧感应器状态</summary>
        public bool RightSensorStatus => _model.RightSensorStatus;

        /// <summary>运行时：工位是否有料 (料板在位)</summary>
        public bool HasBoard => _model.HasBoard;

        #endregion

        #region 2. 视图派生属性 (剔除 UI 框架依赖，为 View 提供渲染参考值)

        /// <summary>
        /// 动态计算左侧挡板的列宽比例。有挡板时为 1.2，无挡板时为 0
        /// (UI 端通过绑定到静态单例 RatioToGridLengthConverter 来进行转换)
        /// </summary>
        public double LeftJackingWidthRatio => HasLeftJacking ? 1.2 : 0;

        /// <summary>
        /// 动态计算右侧挡板的列宽比例。有挡板时为 1.2，无挡板时为 0
        /// </summary>
        public double RightJackingWidthRatio => HasRightJacking ? 1.2 : 0;

        /// <summary>
        /// 根据气缸状态控制左挡板透明度。缩回状态下呈半透明 (0.2)，其他状态不透明 (1.0)
        /// </summary>
        public double LeftJackingOpacity => LeftJackingState == StationPropertyModelEdit.STATE_RETRACT ? 0.2 : 1.0;

        /// <summary>
        /// 根据气缸状态控制右挡板透明度。缩回状态下呈半透明 (0.2)，其他状态不透明 (1.0)
        /// </summary>
        public double RightJackingOpacity => RightJackingState == StationPropertyModelEdit.STATE_RETRACT ? 0.2 : 1.0;

        /// <summary>
        /// 左气缸是否处于异常状态 (交由 UI 端 Animations 动画样式接管闪烁控制)
        /// </summary>
        public bool IsLeftJackingUnNormal => LeftJackingState == StationPropertyModelEdit.STATE_UNNORMAL;

        /// <summary>
        /// 右气缸是否处于异常状态
        /// </summary>
        public bool IsRightJackingUnNormal => RightJackingState == StationPropertyModelEdit.STATE_UNNORMAL;

        #endregion

        /// <summary>
        /// 释放资源。当前 ViewModel 的生命周期及资源由 ReactiveUI.WhenActivated 安全托管，此处预留接口。
        /// </summary>
        public void Dispose()
        {
            // 若未来增加非托管资源或独立定时器，请在此处释放。
        }
    }
}