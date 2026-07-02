using Avalonia.Controls;
using Griffins.Map.UI;
using GKG.Map.Page.UIContainer.GridContainer.Views;
using GKG.Map.Page.UIContainer.GridContainer.ViewModels;
using GKG.Map.Page.UIContainer.GridContainer.Models;

namespace GKG.Map.Page.UIContainer.GridContainer
{
    /// <summary>
    /// 网格子页面容器设计时预览实现（负责设计时预览功能和数据绑定）
    /// </summary>
    public class GridContainerTemplatePreview : ISubPageContainerPreview
    {
        private SubPageContainerCfgInfo? _cfgInfo;
        private PreviewViewModel _viewModel = new PreviewViewModel();

        #region ISubPageContainerPreview接口实现

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cfgInfo">子页面容器配置信息</param>
        void ISubPageContainerPreview.Init(SubPageContainerCfgInfo cfgInfo)
        {
            _cfgInfo = cfgInfo;
        }

        /// <summary>
        /// 设置子页面容器配置信息
        /// </summary>
        /// <param name="cfgInfo">子页面容器配置信息</param>
        void ISubPageContainerPreview.SetSubPageContainerCfgInfo(SubPageContainerCfgInfo cfgInfo)
        {
            _cfgInfo = cfgInfo;
            RefreshViewModel();
        }

        /// <summary>
        /// 设计时预览界面实例（从Control继承）
        /// </summary>
        object ISubPageContainerPreview.View
        {
            get
            {
                // 1. 尝试从配置中解析出用户保存的样式 ID
                string pageStyleID = "";
                if (_cfgInfo?.CfgInfo != null)
                {
                    var pageCfgInfo = new GridContainerTemplateCfgInfo();
                    pageCfgInfo.FromJsonBytes(_cfgInfo.CfgInfo);
                    pageStyleID = pageCfgInfo.PageStyleID;
                }

                // 2. 如果找到了样式 ID，去样式插件管理器里提取对应的 UI 控件
                if (!string.IsNullOrEmpty(pageStyleID))
                {
                    var pageStyleInfo = PageStyleInfoMng.PageStyleInfoSource.Find(o => o.PageStyleID == pageStyleID);

                    if (pageStyleInfo?.View is UserControl styleControl)
                    {
                        styleControl.DataContext = _viewModel;
                        return styleControl;
                    }
                }

                // 3. 兜底方案：如果没配置样式，或者样式被删了找不到了，返回最基础的默认视图
                return new PreviewView
                {
                    DataContext = _viewModel
                };
            }
        }

        #endregion

        /// <summary>
        /// 刷新视图模型数据（从配置信息同步）
        /// </summary>
        private void RefreshViewModel()
        {
            try
            {
                // 检查必要条件
                if (_cfgInfo == null)
                    return;
                //加载配置信息
                _viewModel.LoadConfiguration(_cfgInfo.WorkAreaInfoes, _cfgInfo.CfgInfo);
                _viewModel.SetDesignTimeCommandStrategy();

            }
            catch (Exception ex)
            {
                // 可根据需要添加日志记录
                Console.WriteLine($"刷新预览视图模型失败: {ex.Message}");
            }
        }

    }
}
