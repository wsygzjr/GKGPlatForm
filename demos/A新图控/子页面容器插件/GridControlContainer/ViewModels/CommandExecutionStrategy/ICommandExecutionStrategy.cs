using Griffins.Map;

namespace GKG.Map.Page.UIContainer.GridContainer.ViewModels
{
    /// <summary>
    /// 命令执行策略接口（设计时/运行时命令处理契约）
    /// </summary>
    public interface ICommandExecutionStrategy
    {
        /// <summary>
        /// 加载工作区网格内容
        /// </summary>
        /// <param name="workAreaItem">工作区数据模型</param>
        void LoadWorkAreaContent(GridWorkAreaModel workAreaItem);

        /// <summary>
        /// 设置指定的子页面为当前活动子页面
        /// </summary>
        /// <param name="subPageID">子页面实例ID（不可为null）</param>
        /// <returns>true:切换成功；false:子页面不存在或切换失败</returns>
        bool ActivateSubPage(SubPageID subPageID);
    }
}