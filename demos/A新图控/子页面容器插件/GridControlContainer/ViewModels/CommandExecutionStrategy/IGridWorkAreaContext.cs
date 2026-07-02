namespace GKG.Map.Page.UIContainer.GridContainer.ViewModels;

/// <summary>
/// 网格工作区上下文接口 (为策略提供底层数据查询能力)
/// </summary>
public interface IGridWorkAreaContext
{

    /// <summary>
    /// 根据 SubID 获取对应的网格数据模型
    /// </summary>
    GridWorkAreaModel? GetModelBySubID(Guid subID);
}
