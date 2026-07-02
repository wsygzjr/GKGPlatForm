using Griffins.Map;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;

namespace MainFrame.ViewModels
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
        /// 最小化窗口
        /// </summary>
        MinimizeWinow,
        /// <summary>
        /// 关闭窗口
        /// </summary>
        CloseWindow
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
