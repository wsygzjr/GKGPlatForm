namespace GKG.Map.Page.UIContainer.TabControlContainer.ViewModels;

/// <summary>
/// 工作区内容更新接口（策略与ViewModel的通信契约）
/// </summary>
public interface IWorkAreaContentUpdater
{

    /// <summary>
    /// 更新工作区当前内容
    /// </summary>
    /// <param name="content">要显示的内容（UserControl/TextBlock等）</param>
    /// <param name="subID">子页面ID或容器ID,可为空</param>
    void UpdateWorkAreaContent(object content, Guid subID);
}
