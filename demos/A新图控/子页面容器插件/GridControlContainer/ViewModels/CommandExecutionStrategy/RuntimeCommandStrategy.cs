using Avalonia.Controls;
using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.Map.Page.UIContainer.GridContainer.ViewModels
{
    /// <summary>
    /// 运行时命令执行策略（负责调度宿主真实业务视图，支持嵌套容器联动）
    /// </summary>
    public class RuntimeCommandStrategy : ICommandExecutionStrategy
    {
        private readonly IGridWorkAreaContext _context;
        private readonly ISubPageContainerRunTimeCallBack _runtimeCallback;

        public RuntimeCommandStrategy(IGridWorkAreaContext context, ISubPageContainerRunTimeCallBack runtimeCallback)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _runtimeCallback = runtimeCallback ?? throw new ArgumentNullException(nameof(runtimeCallback));
        }

        #region ICommandExecutionStrategy 接口实现

        void ICommandExecutionStrategy.LoadWorkAreaContent(GridWorkAreaModel workAreaItem)
        {
            switch (workAreaItem.WorkAreaKind)
            {
                case WorkAreaKind.SubPage:
                    workAreaItem.CachedContent = LoadSubPageView(new SubPageID(workAreaItem.SubID));
                    break;

                case WorkAreaKind.SubPageContainer:
                    workAreaItem.CachedContent = LoadContainerView(workAreaItem.SubID);
                    break;

                case WorkAreaKind.Dynamic:
                    if (workAreaItem.IsDefaultSubPage)
                        workAreaItem.CachedContent = LoadSubPageView(new SubPageID(workAreaItem.SubID));
                    else
                        workAreaItem.CachedContent = LoadContainerView(workAreaItem.SubID);
                    break;
            }
        }

        bool ICommandExecutionStrategy.ActivateSubPage(SubPageID subPageID)
        {
            Guid targetGuid = subPageID.ToGuid();

            // 场景 1：要激活的页面直接就在咱们网格的某个格子里
            var targetModel = _context.GetModelBySubID(targetGuid);
            if (targetModel != null && targetModel.WorkAreaKind == WorkAreaKind.SubPage)
            {
                // 确保它已经被加载出来了
                if (targetModel.CachedContent == null)
                    targetModel.CachedContent = LoadSubPageView(subPageID);

                SetFocus(targetModel); // 高亮这个格子
                return true;
            }

            // 场景 2：要激活的页面藏在嵌套的子容器里
            var containerInfos = _runtimeCallback.GetSubPageContainerInstInfoes();
            if (containerInfos != null)
            {
                foreach (var containerInfo in containerInfos)
                {
                    // 判断这个容器是不是归咱们网格管的（是不是放在咱们的格子里）
                    var containerModel = _context.GetModelBySubID(containerInfo.InstanceID);
                    if (containerModel != null)
                    {
                        var childContainerRuntime = _runtimeCallback.GetSubPageContainerRunTime(containerInfo.InstanceID);
                        if (childContainerRuntime != null)
                        {
                            bool hasSubPage = childContainerRuntime.SetCurSubPaage(subPageID);
                            if (hasSubPage)
                            {
                                containerModel.CachedContent = childContainerRuntime.View;
                                SetFocus(containerModel);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        #region 私有方法

        private object LoadSubPageView(SubPageID subPageID)
        {
            try
            {
                if (subPageID == SubPageID.Empty)
                    return CreateErrorView("未配置子页面");

                var subPageInfos = _runtimeCallback.GetSubPageInfoes()
                     ?? throw new Exception("获取子页面信息失败");

                var subPageInfo = subPageInfos.Find(o => o.SubPageID == subPageID)
                    ?? throw new Exception($"未找到ID为 {subPageID} 的子页面");

                var view = _runtimeCallback.GetSubPageView(subPageID)
                    ?? throw new Exception($"无法加载子页面视图: {subPageInfo.SubPageName}");

                return view;
            }
            catch (Exception ex)
            {
                return CreateErrorView($"页面加载异常:\n{ex.Message}");
            }
        }

        private object LoadContainerView(Guid containerInstanceID)
        {
            try
            {
                if (containerInstanceID == Guid.Empty)
                    return CreateErrorView("未配置子页面容器");

                var subPageContainerRunTime = _runtimeCallback.GetSubPageContainerRunTime(containerInstanceID)
                   ?? throw new Exception("子页面容器实例获取失败");

                return subPageContainerRunTime.View;
            }
            catch (Exception ex)
            {
                return CreateErrorView($"容器加载异常:\n{ex.Message}");
            }
        }

        
        private void SetFocus(GridWorkAreaModel activeModel)
        {
            
        }

        /// <summary>
        /// 创建友好的错误回退视图
        /// </summary>
        private object CreateErrorView(string message)
        {
            return new TextBlock
            {
                Text = message,
                Foreground = Brushes.Red,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
        }

        #endregion
    }
}