using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.Map.Page.UIContainer.GridContainer.ViewModels
{
    /// <summary>
    /// 设计时命令执行策略 (负责在后台配置或预览模式下，生成假数据/占位 UI)
    /// </summary>
    public class DesignTimeCommandStrategy : ICommandExecutionStrategy
    {
        private readonly IGridWorkAreaContext _context;


        public DesignTimeCommandStrategy(IGridWorkAreaContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region ICommandExecutionStrategy 接口实现

        void ICommandExecutionStrategy.LoadWorkAreaContent(GridWorkAreaModel workAreaItem)
        {
            switch (workAreaItem.WorkAreaKind)
            {
                case WorkAreaKind.SubPage:
                    workAreaItem.CachedContent = CreateSubPageMockView(new SubPageID(workAreaItem.SubID));
                    break;

                case WorkAreaKind.SubPageContainer:
                    workAreaItem.CachedContent = CreateContainerMockView(workAreaItem.SubID);
                    break;

                case WorkAreaKind.Dynamic:
                    if (workAreaItem.IsDefaultSubPage)
                        workAreaItem.CachedContent = CreateSubPageMockView(new SubPageID(workAreaItem.SubID));
                    else
                        workAreaItem.CachedContent = CreateContainerMockView(workAreaItem.SubID);
                    break;
            }
        }

        bool ICommandExecutionStrategy.ActivateSubPage(SubPageID subPageID)
        {
            var targetModel = _context.GetModelBySubID(subPageID.ToGuid());

            if (targetModel != null)
            {
                // 设计时预览：直接显示一段醒目的文字表示被激活
                targetModel.CachedContent = new TextBlock
                {
                    Text = $"🔔 模拟激活焦点：【{subPageID}】",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.DodgerBlue // 弄点颜色区分一下
                };

                return true;
            }

            return false;
        }

        #endregion

        #region 私有辅助方法 (生成占位视图)

        private object CreateSubPageMockView(SubPageID subPageID)
        {
            if (subPageID == SubPageID.Empty)
                return new TextBlock { Text = "⚠️ 未配置子页面", Foreground = Brushes.Orange };

            try
            {
                var subPageInfos = GlobleCallBack.GetSubPageInfoes()
                    ?? throw new Exception("获取子页面字典失败");

                var subPageInfo = subPageInfos.Find(o => o.SubPageID == subPageID)
                    ?? throw new Exception($"找不到 ID 为 {subPageID} 的页面");

                return new TextBlock
                {
                    Text = $"📄 [预览] 页面: {subPageInfo.SubPageName}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            catch (Exception ex)
            {
                return new TextBlock { Text = $"❌ {ex.Message}", Foreground = Brushes.Red };
            }
        }

        private object CreateContainerMockView(Guid containerInstanceID)
        {
            if (containerInstanceID == Guid.Empty)
                return new TextBlock { Text = "⚠️ 未配置子容器", Foreground = Brushes.Orange };

            try
            {
                var containerTypeInfos = GlobleCallBack.GetSubPageContainerInstInfoes()
                   ?? throw new Exception("获取容器字典失败");

                var containerTypeInfo = containerTypeInfos.Find(o => o.InstanceID == containerInstanceID)
                    ?? throw new Exception($"找不到 ID 为 {containerInstanceID} 的容器");

                return new TextBlock
                {
                    Text = $"📦 [预览] 容器: {containerTypeInfo.SubPageContainerName}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            catch (Exception ex)
            {
                return new TextBlock { Text = $"❌ {ex.Message}", Foreground = Brushes.Red };
            }
        }

        #endregion
    }
}