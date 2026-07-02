using Avalonia.Controls;
using Avalonia.Layout;
using Griffins.Map;
using Griffins.Map.UI;
using NonMainFrameViewModel.Models;
using NonMainFrameViewModel.ViewModels.Comon;
using NonMainFrameViewModel.ViewModels.ToolbarMenu;
using NonMainFrameViewModel.ViewModels.TopMenu;
using ReactiveUI;
using TabControlContainerViewModel.ViewModels.Comon;

namespace NonMainFrameViewModel.ViewModels;

/// <summary>
/// 主框架预览视图模型（支持设计时和运行时不同命令流程）
/// </summary>
public class PreviewViewModel : ReactiveObject, IWorkAreaContentUpdater
{
    private byte[]? _cfgInfo;

    /// <summary>
    /// 顶部栏上下文对象
    /// </summary>
    public TopMenuViewModel TopMenuViewModel { get; }

    ///// <summary>
    ///// 页面工具栏上下文对象
    ///// </summary>
    public PageToolBarButtonViewModel PageToolBarButtonViewModel { get; }

    /// <summary>
    /// 工作区信息
    /// </summary>
    private WorkAreaInfoList? _workAreaInfoes;
    ///// <summary>
    ///// 工作区显示上下文对象
    ///// </summary>
    //public WorkAreaViewModel WorkAreaViewModel { get; }

    private object? _currentContent;
    /// <summary>
    /// 工作区显示的内容
    /// </summary>
    public object? WorkAreaContent
    {
        get => _currentContent;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentContent, value);
        }
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
    /// 命令执行策略（根据环境自动切换）
    /// </summary>
    private ICommandExecutionStrategy _commandStrategy;

    /// <summary>
    /// 构造函数（默认使用设计时策略）
    /// </summary>
    public PreviewViewModel()
    {
        // 初始化子ViewModel
        TopMenuViewModel = new TopMenuViewModel(onTopMenu);
        PageToolBarButtonViewModel = new PageToolBarButtonViewModel(OnToolbarCommand);

        // 初始化设计时策略并订阅异常事件
        _commandStrategy = new DesignTimeCommandStrategy(this);
    }

    private void onTopMenu(TopMenuCmdKind kind)
    {
        _commandStrategy.HandleTopMenuCommand(kind);
    }

    /// <summary>
    /// 工具栏命令处理（委托给当前策略）
    /// </summary>
    private void OnToolbarCommand(string toolItemId)
    {
        if (string.IsNullOrWhiteSpace(toolItemId))
        {
            WorkAreaContent = new TextBlock { Text = "工具项ID不能为空" };
            return;
        }

        var toolItem = PageToolBarButtonViewModel
            .ToolbarMenuItems
            .FirstOrDefault(o => o.ButtonID == toolItemId);

        if (toolItem == null)
        {
            WorkAreaContent = new TextBlock { Text = $"未找到ID为 {toolItemId} 的工具项" };
            return;
        }

        try
        {
            _commandStrategy.HandleToolbarCommand(toolItem);
        }
        catch (Exception ex)
        {
            WorkAreaContent = new TextBlock { Text = $"工具栏命令失败: {ex.Message}" };
        }
    }


    #region 运行时实现的方法
    /// <summary>
    /// 设置运行时回调并切换到运行时策略
    /// </summary>
    /// <param name="runtimeCallback">运行时回调接口（不可为null）</param>
    public void SetRuntimeCallback(IPageFrameTemplateRunTimeCallBack runtimeCallback, WorkAreaInfoList? workAreaInfoes, byte[]? cfgInfo)
    {
        if (runtimeCallback == null)
            throw new Exception($"运行时回调接口不能为空");
        this._cfgInfo = cfgInfo;
        this._workAreaInfoes = workAreaInfoes;

        if (cfgInfo != null)
        {
            var mainPageCfgInfo = new NonMainPageFrameTemplateCfgInfo();
            mainPageCfgInfo.FromJsonBytes(cfgInfo);
            TopMenuViewModel.FillToVM(mainPageCfgInfo.GeneralConfigInfo);
        }
        PageToolBarButtonViewModel.FillToVM(runtimeCallback.GetPageToolBarButtonInfoes());
        // 切换到运行时策略并传递工作区更新器
        _commandStrategy = new RuntimeCommandStrategy(this, runtimeCallback, _cfgInfo, _workAreaInfoes);
        if (_workAreaInfoes?.Count != 1)
            throw new Exception("工作区信息配置错误");

        var workAreaInfo = _workAreaInfoes[0];
        var workAreaCfgInfo = WorkAreaCfgInfo.FromJSonBytes(workAreaInfo.CfgInfo);
        HorizontalContentAlignment = workAreaCfgInfo.HorizontalContentAlignment;
        VerticalContentAlignment = workAreaCfgInfo.VerticalContentAlignment;
        //_commandStrategy.ShowWorkAreaInfo(workAreaInfo);
    }

    /// <summary>
    /// 加载配置信息
    /// </summary>
    public void LoadConfiguration(
        WorkAreaInfoList? workAreaInfoes,
        byte[] cfgInfo)
    {
        this._cfgInfo = cfgInfo;
        this._workAreaInfoes = workAreaInfoes;

        if (cfgInfo!=null)
        {
            var mainPageCfgInfo = new NonMainPageFrameTemplateCfgInfo();
            mainPageCfgInfo.FromJsonBytes(cfgInfo);
            TopMenuViewModel.FillToVM(mainPageCfgInfo.GeneralConfigInfo);
        }
        PageToolBarButtonViewModel.FillToVM(GlobleCallBack.GetPageToolBarButtonInfoes());

        if (_workAreaInfoes?.Count != 1)
            throw new Exception("工作区信息配置错误");
        _commandStrategy = new DesignTimeCommandStrategy(this);
        _commandStrategy.ShowWorkAreaInfo(_workAreaInfoes[0]);
    }

    /// <summary>
    /// 设置指定的子页面为当前活动子页面
    /// </summary>
    /// <param name="subPageID">子页面实例ID（不可为null）</param>
    /// <returns>true:切换成功；false:子页面不存在或切换失败</returns>
    public bool ActivateSubPage(SubPageID subPageID)
    {
        if (subPageID == SubPageID.Empty)
        {
            _commandStrategy.ShowWorkAreaInfo(_workAreaInfoes[0]);
            return true;
        }
        else
            return _commandStrategy.ActivateSubPage(subPageID);
    }
    #endregion

    #region 工作区更新
    /// <summary>
    /// 实现IWorkAreaContentUpdater接口，供策略更新工作区内容
    /// </summary>
    void IWorkAreaContentUpdater.UpdateWorkAreaContent(object? content)
    {
        WorkAreaContent = content;
    }
    #endregion
}
