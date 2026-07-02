using Avalonia.Controls;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using NonMainFrameViewModel.Models;
using NonMainFrameViewModel.ViewModels.Comon;
using NonMainFrameViewModel.ViewModels.ToolbarMenu;
using NonMainFrameViewModel.ViewModels.TopMenu;

namespace NonMainFrameViewModel.ViewModels
{
    /// <summary>
    /// 运行时命令执行策略（执行实际业务逻辑）
    /// </summary>
    public class RuntimeCommandStrategy : ICommandExecutionStrategy
    {
        private readonly IWorkAreaContentUpdater _contentUpdater;
        private readonly IPageFrameTemplateRunTimeCallBack _runtimeCallback;
        private byte[]? _cfgInfo;

        /// <summary>
        /// 工作区信息
        /// </summary>
        private readonly WorkAreaInfoList? _workAreaInfoes;

        public RuntimeCommandStrategy(IWorkAreaContentUpdater contentUpdater, 
            IPageFrameTemplateRunTimeCallBack runtimeCallback,
            byte[]? cfgInfo,
            WorkAreaInfoList? workAreaInfoes)
        {
            _contentUpdater = contentUpdater;
            _runtimeCallback = runtimeCallback ;
            _workAreaInfoes = workAreaInfoes;
            _cfgInfo = cfgInfo;
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
        /// 运行时工具栏命令处理
        /// </summary>
        public void HandleToolbarCommand(PageToolBarButtonInfo toolItem)
        {
            if (toolItem == null)
                throw new Exception("无效的工具栏项");

            //调用框架回调执行实际工具操作
            _runtimeCallback.ExecButtonCmd(toolItem.ButtonID);
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
            //1.获取该框架页面自己内部的子页面ID,存在则设置并返回true
            List<SubPageID> subPageIDs = getOwnerSubPageIDs();
            if (subPageIDs.Contains(subPageID))
            {
                gotoSubPage(subPageID);
                return true;
            }
            //2.获取该框架页面关联的子页面容器实例,循环调用子页面容器实例的SetCurSubPaage,该方法返回true时则返回.
            SubPageContainerInstInfoList SubPageContainerInstInfoes = _runtimeCallback.GetSubPageContainerInstInfoes();
            foreach (var SubPageContainerInstInfo in SubPageContainerInstInfoes)
            {
                ISubPageContainerRunTime iSubPageContainerRunTime = _runtimeCallback.GetSubPageContainerRunTime(SubPageContainerInstInfo.InstanceID);
                bool hasSubPage = iSubPageContainerRunTime.SetCurSubPaage(subPageID);
                if (hasSubPage)
                {
                    _contentUpdater.UpdateWorkAreaContent(iSubPageContainerRunTime.View);
                    return hasSubPage;
                }
            }
            return false;
        }

        /// <summary>
        /// 跳转到子页面
        /// 注:切换到子容器也有两个地方触发
        /// </summary>
        /// <param name="subPageID">子页面ID</param>
        /// <exception cref="Exception"></exception>
        private void gotoSubPage(SubPageID subPageID)
        {
            if (subPageID == SubPageID.Empty)
                throw new Exception("未配置子页面");

            var subPageInfoes = _runtimeCallback.GetSubPageInfoes()
                 ?? throw new Exception("获取子页面信息失败");

            var subPageInfo = subPageInfoes.Find(o => o.SubPageID == subPageID)
                ?? throw new Exception($"未找到ID为 {subPageID} 的子页面");

            var view = _runtimeCallback.GetSubPageView(subPageID)
                ?? throw new Exception($"无法加载子页面视图: {subPageInfo.SubPageName}");

            _contentUpdater.UpdateWorkAreaContent(view);
        }
       
        /// <summary>
        /// 切换到子页面容器
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="Exception"></exception>
        private void gotoSubPageContainer(Guid containerInstanceID)
        {
            if (containerInstanceID == Guid.Empty)
                throw new Exception("未配置子页面容器");

            var subPageContainerRunTime = _runtimeCallback.GetSubPageContainerRunTime(containerInstanceID)
               ?? throw new Exception("子页面容器实例信息获取失败");

            _contentUpdater.UpdateWorkAreaContent(subPageContainerRunTime.View);
        }

        /// <summary>
        /// 跳转到首页
        /// </summary>
        private void gotoRootPage()
        {
            _runtimeCallback.GotoRootPage();
        }
        /// <summary>
        /// 跳转到上级页面
        /// </summary>
        private void gotoParentPage()
        {
            _runtimeCallback.GotoParentPage();
        }
        /// <summary>
        /// 执行窗口关闭命令
        /// </summary>
        /// <param name="topItem"></param>
        private void closeCommand()
        {
            _runtimeCallback.CloseWindow();
        }

        /// <summary>
        /// 获取框架页面自己的子页面ID列表
        /// </summary>
        /// <returns>当前页面的子页面ID列表</returns>
        List<SubPageID> getOwnerSubPageIDs()
        {
            if (_cfgInfo == null)
                throw new Exception("未配置框架页面的子页面");
            return _runtimeCallback.GetSubPageInfoes().Select(info => info.SubPageID).ToList();
            //var mainPageCfgInfo = new NonMainPageFrameTemplateCfgInfo();
            //mainPageCfgInfo.FromJsonBytes(_cfgInfo);
            //List<SubPageID> subPageIDs = new List<SubPageID>();
            //subPageIDs.AddRange(mainPageCfgInfo.ToolbarButtons.Where(o => o.NavgMenuCmdKind == MenuCmdKind.GotoSubPage).Select(p => p.SubPageID).ToList());
            //return subPageIDs;
        }
    }
}
