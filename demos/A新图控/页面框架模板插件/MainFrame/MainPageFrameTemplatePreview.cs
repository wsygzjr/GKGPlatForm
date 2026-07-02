using Griffins.Map;
using Griffins.Map.UI;
using Griffins.PF;
using MainFrameView.Views;
using MainFrame.Models;
using MainFrame.ViewModels;
using System;

namespace MainFrame
{
    /// <summary>
    /// 主页面框架模板设计时预览实现（负责设计时预览功能和数据绑定）
    /// </summary>
    public class MainPageFrameTemplatePreview : IPageFrameTemplatePreview
    {
        private PageFrameTemplateCfgInfo? _cfgInfo;
        private PreviewViewModel? _viewModel;

        /// <summary>
        /// 页面框架模板配置信息
        /// </summary>
        public PageFrameTemplateCfgInfo? CfgInfo
        {
            get => _cfgInfo;
            set
            {
                _cfgInfo = value;
                RefreshViewModel();
            }
        }

        /// <summary>
        /// 预览视图模型
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

        /// <summary>
        /// 设计时预览界面实例
        /// </summary>
        public object View => new PreviewView
        {
            DataContext = ViewModel
        };

        /// <summary>
        /// 刷新视图模型数据
        /// </summary>
        private void RefreshViewModel()
        {
            try
            {
                if (_cfgInfo == null)
                    return;
                ViewModel.LoadConfiguration(_cfgInfo.WorkAreaInfoes,  _cfgInfo.CfgInfo);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"刷新预览视图模型失败: {ex.Message}");
            }
        }
    }
}
