using Avalonia.Controls;
using Griffins.Map.UI;
using GKG.Map.Page.UIContainer.TabControlContainer.ViewModels;
using GKG.Map.Page.UIContainer.TabControlContainer.Models;
using GKG.Map.Page.UIContainer.TabControlContainer.Views;

namespace GKG.Map.Page.UIContainer.TabControlContainer
{
    /// <summary>
    /// 选项卡控件子页面容器设计时预览实现（负责设计时预览功能和数据绑定）
    /// </summary>
    public class TabControlContainerTemplatePreview : ISubPageContainerPreview
    {
        private SubPageContainerCfgInfo? _cfgInfo;

        private PreviewViewModel? _viewModel;
        /// <summary>
        /// 预览视图模型（延迟初始化，确保非null）
        /// </summary>
        public PreviewViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = new PreviewViewModel();
                }
                return _viewModel;
            }
        }

        #region ISubPageContainerPreview接口实现

        void ISubPageContainerPreview.Init(SubPageContainerCfgInfo cfgInfo)
        {
            _cfgInfo = cfgInfo;
        }

        void ISubPageContainerPreview.SetSubPageContainerCfgInfo(SubPageContainerCfgInfo cfgInfo)
        {
            _cfgInfo = cfgInfo;
            RefreshViewModel();
        }

        object ISubPageContainerPreview.View
        {
            get
            {
                string pageStyleID = "";
                if(_cfgInfo!=null&& _cfgInfo.CfgInfo!=null)
                {
                    var pageCfgInfo = new TabControlContainerTemplateCfgInfo();
                    pageCfgInfo.FromJsonBytes(_cfgInfo.CfgInfo);
                    pageStyleID=pageCfgInfo.PageStyleID;
                }

                var pageStyleInfo = PageStyleInfoMng.PageStyleInfoSource.Find(o => o.PageStyleID == pageStyleID);
                if (pageStyleInfo != null)
                {
                    var userControl = pageStyleInfo.View;
                    if (userControl != null)
                    {
                        ((UserControl)userControl).DataContext = _viewModel;
                    }
                    else
                        return null!;
                }

                var defaultUserControl = new PreviewView();
                (defaultUserControl as UserControl).DataContext = _viewModel;
                return defaultUserControl;
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
                ViewModel.LoadConfiguration(_cfgInfo.WorkAreaInfoes,  _cfgInfo.CfgInfo);
                ViewModel.SetDesignTimeCommandStrategy();

            }
            catch (Exception ex)
            {
                // 可根据需要添加日志记录
                Console.WriteLine($"刷新预览视图模型失败: {ex.Message}");
            }
        }
       
    }
}
