using Avalonia.Controls;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.Map.Page.UIContainer.TabControlContainer.ViewModels
{
    /// <summary>
    /// 设计时命令执行策略（执行设计时业务）
    /// </summary>
    public class DesignTimeCommandStrategy : ICommandExecutionStrategy
    {
        private readonly IWorkAreaContentUpdater _contentUpdater;
        private byte[]? _cfgInfo;
        private WorkAreaInfoList? _workAreaInfoes;

        public DesignTimeCommandStrategy(IWorkAreaContentUpdater contentUpdater, 
            byte[]? cfgInfo, 
            WorkAreaInfoList? workAreaInfoes)
        {
            _contentUpdater = contentUpdater;
            _cfgInfo = cfgInfo;
            _workAreaInfoes = workAreaInfoes;
        }


        /// <summary>
        /// 选项卡改变
        /// </summary>
        /// <param name="tabWorkAreaItem">选项卡项</param>
        public void TabChanged(TabWorkAreaModel tabWorkAreaItem)
        {
            var workAreaInfo = _workAreaInfoes?.Find(o => o.WorkAreaID == tabWorkAreaItem.Id);
            if (workAreaInfo == null)
                throw new Exception("未找到指定工作区对应的选项卡");
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
        /// 设置指定的子页面为当前活动子页面
        /// </summary>
        /// <param name="subPageID">子页面实例ID（不可为null）</param>
        /// <returns>true:切换成功；false:子页面不存在或切换失败</returns>
        public bool ActivateSubPage(SubPageID subPageID)
        {
            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"设置指定的子页面【{subPageID}】为当前活动子页面" }, subPageID.ToGuid());
            return true;
        }

        /// <summary>
        /// 跳转到子页面
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="Exception"></exception>
        private void gotoSubPage(SubPageID subPageID)
        {
            if(subPageID == SubPageID.Empty)
                throw new Exception("未配置子页面");

            var subPageInfoes = GlobleCallBack.GetSubPageInfoes()
                 ?? throw new Exception("获取子页面信息失败");

            var subPageInfo = subPageInfoes.Find(o => o.SubPageID == subPageID)
                ?? throw new Exception($"未找到ID为 {subPageID} 的子页面");

            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"跳转到子页面:【{subPageInfo.SubPageName}】" },subPageID.ToGuid());
        }
        
        /// <summary>
        /// 跳转到子页面容器
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="Exception"></exception>
        private void gotoSubPageContainer(Guid containerInstanceID)
        {
            if (containerInstanceID == Guid.Empty)
                throw new Exception("未配置子页面容器");

            var containerTypeInfoes = GlobleCallBack.GetSubPageContainerInstInfoes()
               ?? throw new Exception("子页面容器信息获取失败");

            var containerTypeInfo = containerTypeInfoes.Find(o => o.InstanceID == containerInstanceID)
                ?? throw new Exception($"未找到容器实例ID为 {containerInstanceID} 的容器");

            _contentUpdater.UpdateWorkAreaContent(new TextBlock { Text = $"跳转到子页面容器:【{containerTypeInfo.SubPageContainerName}】" }, containerInstanceID);
        }
       
    }
}
