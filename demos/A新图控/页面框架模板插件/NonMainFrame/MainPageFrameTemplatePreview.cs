using Griffins.Map;
using Griffins.Map.UI;
using Griffins.PF;
using NonMainFrameView.Views;
using NonMainFrameViewModel.Models;
using NonMainFrameViewModel.ViewModels;
using System;

namespace NonMainFrame
{
    /// <summary>
    /// 非主页面框架模板设计时预览实现（负责设计时预览功能和数据绑定）
    /// </summary>
    public class NonMainPageFrameTemplatePreview : IPageFrameTemplatePreview
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

        /// <summary>
        /// 设计时预览界面实例（从Control继承）
        /// </summary>
        public object View => new PreviewView
        {
            DataContext = ViewModel
        };

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

            }
            catch (Exception ex)
            {
                // 可根据需要添加日志记录
                Console.WriteLine($"刷新预览视图模型失败: {ex.Message}");
            }
        }
    }
}
