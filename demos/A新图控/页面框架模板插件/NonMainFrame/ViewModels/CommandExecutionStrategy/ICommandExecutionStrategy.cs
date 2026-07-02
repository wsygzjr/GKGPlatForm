using Griffins.Map;
using Griffins.Map.UI;
using NonMainFrameViewModel.ViewModels.ToolbarMenu;
using NonMainFrameViewModel.ViewModels.TopMenu;

namespace NonMainFrameViewModel.ViewModels.Comon
{
    /// <summary>
    /// 命令执行策略接口（设计时/运行时命令处理契约）
    /// </summary>
    public interface ICommandExecutionStrategy
    {
        /// <summary>
        /// 显示工作区
        /// </summary>
        /// <param name="workAreaInfo"></param>
        void ShowWorkAreaInfo(WorkAreaInfo workAreaInfo);
        /// <summary>
        /// 处理工具栏命令
        /// </summary>
        /// <param name="toolItem">工具栏按钮对象</param>
        void HandleToolbarCommand(PageToolBarButtonInfo toolItem);

        /// <summary>
        /// 处理顶部菜单命令
        /// </summary>
        /// <param name="topItem">顶部菜单命令种类</param>
        void HandleTopMenuCommand(TopMenuCmdKind topMenuCmdKind);
        /// <summary>
        /// 设置指定的子页面为当前活动子页面
        /// </summary>
        /// <param name="subPageID">子页面实例ID（不可为null）</param>
        /// <returns>true:切换成功；false:子页面不存在或切换失败</returns>
        bool ActivateSubPage(SubPageID subPageID);
    }
    /// <summary>
    /// 顶部菜单命令种类
    /// </summary>
    public enum TopMenuCmdKind
    {
        /// <summary>
        /// 返回上一页
        /// </summary>
        GotoParentPage,
        /// <summary>
        /// 跳转到首页
        /// </summary>
        GotoRootPage
    }

    /// <summary>
    /// 工作区内容更新接口（策略与ViewModel的通信契约）
    /// </summary>
    public interface IWorkAreaContentUpdater
    {
        /// <summary>
        /// 更新工作区当前内容
        /// </summary>
        /// <param name="content">要显示的内容（UserControl/TextBlock等）</param>
        void UpdateWorkAreaContent(object? content);
    }
}
