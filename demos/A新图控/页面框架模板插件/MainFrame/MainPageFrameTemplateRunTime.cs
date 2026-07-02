using Griffins.Map;
using Griffins.Map.UI;
using MainFrameView.Views;
using MainFrame.ViewModels;

namespace MainFrame
{
    /// <summary>
    /// 主页面框架模板运行时接口实现
    /// 负责页面框架的初始化、子页面切换等核心功能
    /// </summary>
    public class MainPageFrameTemplateRunTime : IPageFrameTemplateRunTime
    {
        private PreviewViewModel? _viewModel;

        /// <summary>
        /// 显示界面（从Control继承）
        /// </summary>
        public object View { get; private set; } =null!;

        /// <summary>
        /// 初始化页面框架
        /// </summary>
        /// <param name="cfgInfo">页面框架模板配置信息（可为null）</param>
        /// <param name="callBack">运行时回调接口（不可为null）</param>
        /// <exception cref="ArgumentNullException">当回调接口为null时抛出</exception>
        public void Init(PageFrameTemplateCfgInfo? cfgInfo, IPageFrameTemplateRunTimeCallBack callBack)
        {
            // 校验必要的回调接口
            if (callBack == null)
                throw new Exception("运行时回调接口不能为null");
              // 初始化视图模型并应用配置
              _viewModel = new PreviewViewModel();
			ApplyConfiguration(cfgInfo);
			// 传递回调接口给视图模型
			_viewModel.SetRuntimeCallback(callBack);
			// 初始化视图并绑定数据上下文
			View = new PreviewView
            {
                DataContext = _viewModel
            };
        }

        /// <summary>
        /// 设置指定的子页面为当前活动子页面
        /// </summary>
        /// <param name="subPageID">子页面实例ID（不可为null）</param>
        /// <returns>true:切换成功；false:子页面不存在或切换失败</returns>
        public bool SetCurSubPaage(SubPageID subPageID)
        {
            // 检查视图模型是否已初始化
            if (_viewModel == null)
                return false;
            // 处理子页面切换逻辑
            return _viewModel.ActivateSubPage(subPageID);
        }

        /// <summary>
        /// 应用配置信息到视图模型
        /// </summary>
        /// <param name="cfgInfo">配置信息实例</param>
        private void ApplyConfiguration(PageFrameTemplateCfgInfo? cfgInfo)
        {
            if (_viewModel == null)
                return;
            _viewModel.LoadConfiguration(cfgInfo?.WorkAreaInfoes,  cfgInfo?.CfgInfo,true);
        }
    }
}
