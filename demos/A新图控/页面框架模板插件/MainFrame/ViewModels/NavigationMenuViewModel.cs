using Avalonia.Layout;
using Griffins.Map.UI;
using ReactiveUI;

namespace MainFrame.ViewModels
{
    /// <summary>
    /// 导航菜单VM
    /// </summary>
    public class NavigationMenuViewModel : ReactiveObject, IWorkAreaContentUpdater
    {
   
        private WorkAreaInfo? _workAreaInfo;
        private ICommandExecutionStrategy _commandStrategy;

        private object? _currentContent = null!;
        public object? CurrentContent { get => _currentContent; set => this.RaiseAndSetIfChanged(ref _currentContent, value); }

        private HorizontalAlignment _horizontalContentAlignment;
        public HorizontalAlignment HorizontalContentAlignment{ get => _horizontalContentAlignment; set => _horizontalContentAlignment = value; }

        private VerticalAlignment _verticalContentAlignment;
        public VerticalAlignment VerticalContentAlignment{ get => _verticalContentAlignment; set => _verticalContentAlignment = value; }

        public NavigationMenuViewModel()
        {
            _commandStrategy = new DesignTimeCommandStrategy(this);
        }

        /// <summary>
        /// 设置运行时回调和策略
        /// </summary>
        /// <param name="runtimeCallback">运行时回调接口（不可为null）</param>
        public void SetRuntimeCallback(IPageFrameTemplateRunTimeCallBack runtimeCallback)
        {
            // 运行时策略并传递工作区更新器
            _commandStrategy = new RuntimeCommandStrategy(this, runtimeCallback, null);
            if (_workAreaInfo != null)
                _commandStrategy.ShowWorkAreaInfo(_workAreaInfo);
        }

        /// <summary>
        /// 从配置信息填充到VM
        /// </summary>
        /// <param name="workAreaInfo">左边导航栏配置信息</param>
        public void FillToVM(WorkAreaInfo workAreaInfo, bool isRunTime = false)
        {
            this._workAreaInfo = workAreaInfo;
            var workAreaCfgInfo = WorkAreaCfgInfo.FromJSonBytes(workAreaInfo.CfgInfo);
            HorizontalContentAlignment = workAreaCfgInfo.HorizontalContentAlignment;
            VerticalContentAlignment = workAreaCfgInfo.VerticalContentAlignment;

            //默认加载设计时配置的区域：显示文本
            if (!isRunTime)
                _commandStrategy.ShowWorkAreaInfo(workAreaInfo);
        }

        /// <summary>
        /// 实现IWorkAreaContentUpdater接口，供策略更新工作区内容
        /// </summary>
        void IWorkAreaContentUpdater.UpdateWorkAreaContent(object? content)
        {
            CurrentContent = content;
        }

    }
}
