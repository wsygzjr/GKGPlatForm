using Avalonia.Controls;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using NonMainFrameViewModel.ViewModels.Comon;
using NonMainFrameViewModel.ViewModels.ToolbarMenu;
using NonMainFrameViewModel.ViewModels.TopMenu;
using SixLabors.ImageSharp;
using TabControlContainerViewModel.ViewModels.Comon;

namespace NonMainFrameViewModel.ViewModels
{
    /// <summary>
    /// 设计时命令执行策略（执行设计时业务）
    /// </summary>
    public class DesignTimeCommandStrategy : ICommandExecutionStrategy
    {
        private readonly IWorkAreaContentUpdater _contentUpdater;

        public DesignTimeCommandStrategy(IWorkAreaContentUpdater contentUpdater)
        {
            _contentUpdater = contentUpdater ?? throw new ArgumentNullException(nameof(contentUpdater));
        }
        /// <summary>
        /// 显示工作区
        /// </summary>
        /// <param name="workAreaInfo"></param>
        public void ShowWorkAreaInfo(WorkAreaInfo workAreaInfo)
        {
            switch (workAreaInfo.WorkAreaKind)
            {
                case WorkAreaKind.SubPage:
                    gotoSubPage(new SubPageID(workAreaInfo.SubID));
                    break;
                case WorkAreaKind.SubPageContainer:
                    gotoSubPageContainer(workAreaInfo.SubID);
                    break;
                case WorkAreaKind.Dynamic:
                    if (workAreaInfo.IsDefaultSubPage)
                        gotoSubPage(new SubPageID(workAreaInfo.SubID));
                    else
                        gotoSubPageContainer(workAreaInfo.SubID);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 设计时工具栏命令处理
        /// </summary>
        public void HandleToolbarCommand(PageToolBarButtonInfo toolItem)
        {
            if (toolItem == null)
                throw new Exception("无效的工具栏项");

            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"执行工具栏按钮:【{toolItem.ButtonName}】" });
        }

        /// <summary>
        /// 处理顶部菜单命令
        /// </summary>
        /// <param name="topItem">顶部菜单命令种类</param>
        public void HandleTopMenuCommand(TopMenuCmdKind topMenuCmdKind)
        {
            switch (topMenuCmdKind)
            {
                case TopMenuCmdKind.GotoParentPage:
                    gotoParentPage();
                    break;
                case TopMenuCmdKind.GotoRootPage:
                    gotoRootPage();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 设置指定的子页面为当前活动子页面
        /// </summary>
        /// <param name="subPageID">子页面实例ID（不可为null）</param>
        /// <returns>true:切换成功；false:子页面不存在或切换失败</returns>
        public bool ActivateSubPage(SubPageID subPageID)
        {
            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"设置指定的子页面【{subPageID}】为当前活动子页面" });
            return true;
        }



        /// <summary>
        /// 跳转到子页面
        /// </summary>
        /// <param name="subPageID"></param>
        /// <exception cref="Exception"></exception>
        private void gotoSubPage(SubPageID subPageID)
        {
            if (subPageID == SubPageID.Empty)
                throw new Exception("未配置子页面");

            var subPageInfoes = GlobleCallBack.GetSubPageInfoes()
                 ?? throw new Exception("获取子页面信息失败");

            var subPageInfo = subPageInfoes.Find(o => o.SubPageID == subPageID)
                ?? throw new Exception($"未找到ID为 {subPageID} 的子页面");

            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"跳转到子页面:【{subPageInfo.SubPageName}】" });
        }
        
        /// <summary>
        /// 跳转到子页面容器
        /// </summary>
        /// <param name="containerInstanceID"></param>
        /// <exception cref="Exception"></exception>
        private void gotoSubPageContainer(Guid containerInstanceID)
        {
            if (containerInstanceID == Guid.Empty)
                throw new Exception("未配置子页面容器");

            var containerTypeInfoes = GlobleCallBack.GetSubPageContainerInstInfoes()
               ?? throw new Exception("子页面容器信息获取失败");

            var containerTypeInfo = containerTypeInfoes.Find(o => o.InstanceID == containerInstanceID)
                ?? throw new Exception($"未找到容器实例ID为 {containerInstanceID} 的容器");

            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"跳转到子页面容器:【{containerTypeInfo.SubPageContainerName}】" });
        }
        /// <summary>
        /// 跳转到首页
        /// </summary>
        private void gotoRootPage()
        {
            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"跳转到首页" });
        }
        /// <summary>
        /// 跳转到上级页面
        /// </summary>
        private void gotoParentPage()
        {
            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"跳转到上级页面" });
        }
        /// <summary>
        /// 执行窗口关闭命令
        /// </summary>
        /// <param name="topItem"></param>
        private void closeCommand()
        {
            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"执行窗口关闭命令" });
        }
    }
}
