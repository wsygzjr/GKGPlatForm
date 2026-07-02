using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;

namespace GKG.Map.Page.UIContainer.TabControlContainer.ViewModels;

/// <summary>
/// 选项卡控件子页面容器预览视图模型（支持设计时和运行时不同命令流程）
/// </summary>
public class PreviewViewModel : ReactiveObject, IWorkAreaContentUpdater
{
    /// <summary>
    /// 配置信息
    /// </summary>
    private byte[]? _cfgInfo;

    /// <summary>
    /// 工作区信息
    /// </summary>
    private WorkAreaInfoList? _workAreaInfoes;

    /// <summary>
    /// 命令执行策略（根据环境自动切换）
    /// </summary>
    private ICommandExecutionStrategy? _commandStrategy;

    /// <summary>
    /// 工作区显示上下文对象
    /// </summary>
    public WorkAreaViewModel WorkAreaViewModel { get; }

    /// <summary>
    /// 构造函数（默认使用设计时策略）
    /// </summary>
    public PreviewViewModel()
    {
        // 初始化子ViewModel
        WorkAreaViewModel = new WorkAreaViewModel(onTabChanged);
    }
  
    /// <summary>
    /// 选项卡点击事件 委托
    /// </summary>
    /// <param name="tabWorkAreaItem"></param>
    private void onTabChanged(TabWorkAreaModel tabWorkAreaItem)
    {
       _commandStrategy?.TabChanged(tabWorkAreaItem);
    }

    /// <summary>
    /// 加载配置信息
    /// </summary>
    /// <param name="workAreaInfoes">工作区 ***预览时有多个工作区时怎么显示?</param>
    /// <param name="cfgInfo"></param>
    public void LoadConfiguration(WorkAreaInfoList workAreaInfoes, byte[] cfgInfo)
    {
        this._workAreaInfoes = workAreaInfoes;
        this._cfgInfo = cfgInfo;

        WorkAreaViewModel.LoadConfiguration(workAreaInfoes);
    }

    /// <summary>
    /// 设置设计时命令执行策略
    /// </summary>
    public void SetDesignTimeCommandStrategy()
    {
        _commandStrategy = new DesignTimeCommandStrategy(this, this._cfgInfo, this._workAreaInfoes);
        WorkAreaViewModel.SetDefaultTab();
    }

    #region 运行时实现的方法

    /// <summary>
    /// 设置运行时回调并切换到运行时策略
    /// </summary>
    /// <param name="runtimeCallback">运行时回调接口（不可为null）</param>
    public void SetRuntimeCallback(ISubPageContainerRunTimeCallBack runtimeCallback)
    {
        if (runtimeCallback == null)
            throw new Exception($"运行时回调接口不能为空");

        // 设置运行时策略并传递工作区更新器
        _commandStrategy = new RuntimeCommandStrategy(this, runtimeCallback, this._cfgInfo, this._workAreaInfoes);
        WorkAreaViewModel.SetDefaultTab();
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
            WorkAreaViewModel.SetDefaultTab();
            return true;
        }
        else
            return _commandStrategy.ActivateSubPage(subPageID);
    }

    #endregion

    #region 工作区更新

    /// <summary>
    /// 更新工作区当前内容
    /// </summary>
    /// <param name="content">要显示的内容（UserControl/TextBlock等）</param>
    /// <param name="subID">子页面ID或容器ID,可为空</param>
    void IWorkAreaContentUpdater.UpdateWorkAreaContent(object content, Guid subID)
    {
        WorkAreaViewModel.ChangeCurrentContent(content, subID);
    }

    #endregion
}
