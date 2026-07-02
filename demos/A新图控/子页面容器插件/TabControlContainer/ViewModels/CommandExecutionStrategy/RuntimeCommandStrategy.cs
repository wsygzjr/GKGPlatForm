using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.Map.Page.UIContainer.TabControlContainer.ViewModels
{
    /// <summary>
    /// 运行时命令执行策略（执行实际业务逻辑）
    /// </summary>
    public class RuntimeCommandStrategy : ICommandExecutionStrategy
    {
        private readonly IWorkAreaContentUpdater _contentUpdater;
        private readonly ISubPageContainerRunTimeCallBack _runtimeCallback;
        private byte[]? _cfgInfo;
        private WorkAreaInfoList? _workAreaInfoes;

        public RuntimeCommandStrategy(IWorkAreaContentUpdater contentUpdater, 
            ISubPageContainerRunTimeCallBack runtimeCallback, 
            byte[]? cfgInfo, 
            WorkAreaInfoList? workAreaInfoes)
        {
            _contentUpdater = contentUpdater!;
            _runtimeCallback = runtimeCallback!;
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
            //1.获取该框架页面自己内部的子页面ID,存在则设置并返回true
            List<SubPageID> subPageIDs = GetOwnerSubPageIDs();
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
                    _contentUpdater.UpdateWorkAreaContent(iSubPageContainerRunTime.View, SubPageContainerInstInfo.InstanceID);
                    return hasSubPage;
                }
            }
            return false;
        }

        /// <summary>
        /// 跳转到子页面
        /// </summary>
        /// <param name="subPageID"></param>
        /// <exception cref="Exception"></exception>
        private void gotoSubPage(SubPageID subPageID)
        {
            try
            {
                if (subPageID == SubPageID.Empty)
                    throw new Exception("未配置子页面");

                var subPageInfoes = _runtimeCallback.GetSubPageInfoes()
                     ?? throw new Exception("获取子页面信息失败");

                var subPageInfo = subPageInfoes.Find(o => o.SubPageID == subPageID)
                    ?? throw new Exception($"未找到ID为 {subPageID} 的子页面");

                var view = _runtimeCallback.GetSubPageView(subPageID)
                    ?? throw new Exception($"无法加载子页面视图: {subPageInfo.SubPageName}");
                _contentUpdater.UpdateWorkAreaContent(view, subPageID.ToGuid());
            }
            catch (Exception ex)
            {

                throw;
            }
            
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

            var subPageContainerRunTime = _runtimeCallback.GetSubPageContainerRunTime(containerInstanceID)
               ?? throw new Exception("子页面容器实例信息获取失败");

            _contentUpdater.UpdateWorkAreaContent(subPageContainerRunTime.View, containerInstanceID);
        }

        /// <summary>
        /// 获取框架页面自己的子页面ID列表
        /// </summary>
        /// <returns>当前页面的子页面ID列表</returns>
       private List<SubPageID> GetOwnerSubPageIDs()
        {
            if (_cfgInfo == null)
                throw new Exception("未配置子页面容器的子页面");
            List<SubPageID> subPageIDs = new List<SubPageID>();
            var subPageIDsGuid = _workAreaInfoes?.Where(o => o.WorkAreaKind == WorkAreaKind.SubPage).Select(p => p.SubID).ToList();
            if(subPageIDsGuid!=null)
            foreach (var subPageIDGuid in subPageIDsGuid)
            {
                subPageIDs.Add(new SubPageID(subPageIDGuid));
            }
            return subPageIDs;
        }
    }
}
