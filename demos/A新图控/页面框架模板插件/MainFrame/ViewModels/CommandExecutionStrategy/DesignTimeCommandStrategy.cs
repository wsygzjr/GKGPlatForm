using Avalonia.Controls;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using System;

namespace MainFrame.ViewModels
{
    /// <summary>
    /// 设计时命令执行策略
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
        /// 处理顶部菜单命令
        /// </summary>
        /// <param name="topItem">顶部菜单命令种类</param>
        public void HandleTopMenuCommand(TopMenuCmdKind topMenuCmdKind)
        {
            switch (topMenuCmdKind)
            {
                case TopMenuCmdKind.MinimizeWinow:
                    minimizeCommand();
                    break;
                case TopMenuCmdKind.CloseWindow:
                    closeCommand();
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
            if(subPageID== SubPageID.Empty)
                throw new Exception(ResourceString.NoCfgSubPageID);

            var subPageInfoes = GlobleCallBack.GetSubPageInfoes()
                 ?? throw new Exception(ResourceString.NothingSubPageInfo);

            var subPageInfo = subPageInfoes.Find(o => o.SubPageID == subPageID)
                ?? throw new Exception(ResourceString.NoubPageInfo+ subPageID);

            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"跳转到子页面:【{subPageInfo.SubPageName}】" });
        }

        /// <summary>
        /// 跳转到页面
        /// </summary>
        /// <param name="pageID"></param>
        /// <exception cref="Exception"></exception>
        private void gotoPage(PageID pageID)
        {
            if (pageID == PageID.Empty)
                throw new Exception(ResourceString.NoPageID);

            var pageInfoes = GlobleCallBack.GetPageInfoes()
                 ?? throw new Exception(ResourceString.NothingPageInfo);

            var pageInfo = pageInfoes.Find(o => o.PageID == pageID)
                ?? throw new Exception(ResourceString.NoPageInfo+ pageID);

            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"跳转到页面:【{pageInfo.PageName}】" });
        }

        /// <summary>
        /// 跳转到页面
        /// </summary>
        /// <param name="pageID">页面ID</param>
        /// <param name="subPageID">子页面ID</param>
        /// <exception cref="Exception"></exception>
        private void gotoPageOfSubPage(PageID pageID, SubPageID subPageID)
        {
            if (pageID == PageID.Empty)
                throw new Exception(ResourceString.NoPageID);

            var pageInfoes = GlobleCallBack.GetPageInfoes()
                 ?? throw new Exception(ResourceString.NothingPageInfo);

            var pageInfo = pageInfoes.Find(o => o.PageID == pageID)
                ?? throw new Exception(ResourceString.NoPageInfo + pageID);

            var subPageInfo = GlobleCallBack.GetSubPageInfoesByPageID(pageID).Find(o => o.SubPageID == subPageID)
                ?? throw new Exception(ResourceString.NothingSubPageInfo + subPageID);

            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"跳转到页面:【{pageInfo.PageName}的子页面{subPageInfo.SubPageName}】" });
        }

		/// <summary>
		/// 跳转到子页面容器
		/// </summary>
		/// <param name="item"></param>
		/// <exception cref="Exception"></exception>
		private void gotoSubPageContainer(Guid containerInstanceID)
        {
            if (containerInstanceID == Guid.Empty)
                throw new Exception(ResourceString.NoContainerInstanceID);

            var containerTypeInfoes = GlobleCallBack.GetSubPageContainerInstInfoes()
               ?? throw new Exception(ResourceString.NothingSubPageContainerInstance);

            var containerTypeInfo = containerTypeInfoes.Find(o => o.InstanceID == containerInstanceID)
                ?? throw new Exception($"{ResourceString.NoSubPageContainerInstance} {containerInstanceID}");

            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"跳转到子页面容器:【{containerTypeInfo.SubPageContainerName}】" });
        }


        /// <summary>
        /// 执行窗口最小化命令
        /// </summary>
        /// <param name="topItem"></param>
        private void minimizeCommand()
        {
            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"执行窗口最小化命令" });
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
