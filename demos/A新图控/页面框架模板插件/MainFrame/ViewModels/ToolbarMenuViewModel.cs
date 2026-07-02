using Avalonia.Layout;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using ReactiveUI;

namespace MainFrame.ViewModels;

/// <summary>
/// 快捷菜单栏ViewModel
/// </summary>
public class ToolbarMenuViewModel : ReactiveObject, IWorkAreaContentUpdater
{
    private object? _currentContent = null!;
    //顶部工作区的页面跳转配置信息
    private WorkAreaInfo? _workAreaInfo;
    /// <summary>
    /// 命令执行策略
    /// </summary>
    private ICommandExecutionStrategy _commandStrategy;
    /// <summary>
    /// 工作区内容
    /// </summary>
    public object? CurrentContent
    {
        get => _currentContent;
        set => this.RaiseAndSetIfChanged(ref _currentContent, value);
    }

    private HorizontalAlignment _horizontalContentAlignment;
    /// <summary>
    ///工作区内部子内容在自身的水平对齐
    /// </summary>
    public HorizontalAlignment HorizontalContentAlignment
    {
        get => _horizontalContentAlignment;
        set => _horizontalContentAlignment = value;
    }
    private VerticalAlignment _verticalContentAlignment;
    /// <summary>
    /// 工作区内部子内容在自身的垂直对齐
    /// </summary>
    public VerticalAlignment VerticalContentAlignment
    {
        get => _verticalContentAlignment;
        set => _verticalContentAlignment = value;
    }
    /// <summary>
    /// 无参构造函数
    /// </summary>
    public ToolbarMenuViewModel()
    {
        // 初始化设计时策略
        _commandStrategy = new DesignTimeCommandStrategy(this);
    }
    /// <summary>
    /// 设置运行时回调并切换到运行时策略
    /// </summary>
    /// <param name="runtimeCallback">运行时回调接口</param>
    public void SetRuntimeCallback(IPageFrameTemplateRunTimeCallBack runtimeCallback)
    {
        // 切换到运行时策略并传递工作区更新器
        _commandStrategy = new RuntimeCommandStrategy(this, runtimeCallback, null);
        if (_workAreaInfo != null)
            _commandStrategy.ShowWorkAreaInfo(_workAreaInfo);
    }
    /// <summary>
    /// 从配置信息填充到VM
    /// </summary>
    /// <param name="workAreaInfo">顶部菜单配置信息</param>
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
