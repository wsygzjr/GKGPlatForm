using Avalonia.Controls;
using Griffins.Map;
using Griffins.Map.UI;
using MainFrame.Models;
using ReactiveUI;

namespace MainFrame.ViewModels;

/// <summary>
/// 主框架预览视图模型（设计时和运行时不同命令流程）
/// </summary>
public class PreviewViewModel : ReactiveObject, IWorkAreaContentUpdater
{
    private byte[]? _frameTemplateCfgInfo;
    /// <summary>
    /// 主工作区信息
    /// </summary>
    private WorkAreaInfo? _mainWorkAreaInfo;

    /// <summary>
    /// 导航菜单上下文对象
    /// </summary>
    public NavigationMenuViewModel NavigationMenuViewModel { get; }

    /// <summary>
    /// 顶部栏上下文对象
    /// </summary>
    public TopMenuViewModel TopMenuViewModel { get; }

    /// <summary>
    /// 快捷工具栏上下文对象
    /// </summary>
    public ToolbarMenuViewModel ToolbarMenuViewModel { get; }

    /// <summary>
    /// 底部栏上下文对象
    /// </summary>
    public ButtomColumnViewModel ButtomColumnViewModel { get; }

    /// <summary>
    /// 左下信息块上下文对象
    /// </summary>
    public LeftBottomInfoBlockViewModel LeftBottomInfoBlockViewModel { get; }


    private object? _mainWorkAreaContent;
    /// <summary>
    /// 主工作区显示的内容
    /// </summary>
    public object? MainWorkAreaContent
    {
        get => _mainWorkAreaContent;
        set
        {
            this.RaiseAndSetIfChanged(ref _mainWorkAreaContent, value);
        }
    }

    /// <summary>
    /// 命令执行策略
    /// </summary>
    private ICommandExecutionStrategy _commandStrategy;

    /// <summary>
    /// 构造函数
    /// </summary>
    public PreviewViewModel()
    {
        // 初始化子ViewModel
        NavigationMenuViewModel = new NavigationMenuViewModel();
        TopMenuViewModel = new TopMenuViewModel();
        ToolbarMenuViewModel = new ToolbarMenuViewModel();
        ButtomColumnViewModel = new ButtomColumnViewModel();
        LeftBottomInfoBlockViewModel = new LeftBottomInfoBlockViewModel();
        // 初始化设计时策略
        _commandStrategy = new DesignTimeCommandStrategy(this);
    }

    #region 运行时实现的方法

    /// <summary>
    /// 设置运行时回调并切换到运行时策略
    /// </summary>
    /// <param name="runtimeCallback">运行时回调接口（不可为null）</param>
    public void SetRuntimeCallback(IPageFrameTemplateRunTimeCallBack runtimeCallback)
    {
        if (runtimeCallback == null)
            throw new Exception($"运行时回调接口不能为空");
        // 切换到运行时策略并传递工作区更新器
        _commandStrategy = new RuntimeCommandStrategy(this, runtimeCallback, _frameTemplateCfgInfo);
        NavigationMenuViewModel.SetRuntimeCallback(runtimeCallback);
        TopMenuViewModel.SetRuntimeCallback(runtimeCallback);
        ToolbarMenuViewModel.SetRuntimeCallback(runtimeCallback);
        ButtomColumnViewModel.SetRuntimeCallback(runtimeCallback);
        LeftBottomInfoBlockViewModel.SetRuntimeCallback(runtimeCallback);
    }

    /// <summary>
    /// 加载主页面框架模板配置信息
    /// </summary>
    /// <param name="workAreaInfoes">主页面框架模板工作区</param>
    /// <param name="cfgInfo">主页面框架模板配置信息</param>
    /// <param name="isRunTime">是否为运行时</param>
    public void LoadConfiguration(WorkAreaInfoList? workAreaInfoes,byte[]? cfgInfo,bool isRunTime=false)
    {
        MainPageFrameTemplateCfgInfo mainPageCfgInfo = new MainPageFrameTemplateCfgInfo();
        if (cfgInfo!=null)
		{
			mainPageCfgInfo.FromJsonBytes(cfgInfo);
			_frameTemplateCfgInfo = cfgInfo;
		}
        NavigationMenuViewModel.FillToVM(mainPageCfgInfo.NavigationMenu, isRunTime);
        TopMenuViewModel.FillToVM(mainPageCfgInfo.TopMenuCfgInfo, mainPageCfgInfo.GeneralConfigInfo, isRunTime);
        ToolbarMenuViewModel.FillToVM(mainPageCfgInfo.ToolbarButton, isRunTime);
        ButtomColumnViewModel.FillToVM(mainPageCfgInfo.ButtomColumn, isRunTime);
        LeftBottomInfoBlockViewModel.FillToVM(mainPageCfgInfo.LeftBottomInfoBlock, isRunTime);

        if (workAreaInfoes != null && workAreaInfoes.Count != 0)
            _mainWorkAreaInfo = workAreaInfoes[0];
    }

    /// <summary>
    /// 设置指定的子页面为当前活动子页面
    /// </summary>
    /// <param name="subPageID">子页面实例ID</param>
    /// <returns>true:切换成功；false:子页面不存在或切换失败</returns>
    public bool ActivateSubPage(SubPageID subPageID)
    {
        //如果为空，则设置主工作区的内容为配置的默认的子页面或子容器
        if (subPageID == SubPageID.Empty)
        {
            if (_mainWorkAreaInfo != null)
                _commandStrategy.ShowWorkAreaInfo(_mainWorkAreaInfo);
            return true;
        }
        return _commandStrategy.ActivateSubPage(subPageID);
    }

    #endregion

    #region 工作区更新

    /// <summary>
    /// 实现IWorkAreaContentUpdater接口，供策略更新工作区内容
    /// </summary>
    void IWorkAreaContentUpdater.UpdateWorkAreaContent(object? content)
    {
        MainWorkAreaContent = content;
    }

    #endregion
}
