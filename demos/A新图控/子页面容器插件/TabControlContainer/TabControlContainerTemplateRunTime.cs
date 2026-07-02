using Avalonia.Controls;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.Map.Page.UIContainer.TabControlContainer.Views;
using GKG.Map.Page.UIContainer.TabControlContainer.Models;
using GKG.Map.Page.UIContainer.TabControlContainer.ViewModels;


namespace GKG.Map.Page.UIContainer.TabControlContainer
{
    /// <summary>
    /// 选项卡控件子页面容器运行时接口实现
    /// 负责页面框架的初始化、子页面切换等核心功能
    /// </summary>
    public class TabControlContainerTemplateRunTime : ISubPageContainerRunTime
    {
        private PreviewViewModel? _viewModel;
        private SubPageContainerCfgInfo? _cfgInfo;

        #region ISubPageContainerRunTime接口实现

        object ISubPageContainerRunTime.View
        {
            get
            {
                string pageStyleID = "";
                if (_cfgInfo != null && _cfgInfo.CfgInfo != null)
                {
                    var pageCfgInfo = new TabControlContainerTemplateCfgInfo();
                    pageCfgInfo.FromJsonBytes(_cfgInfo.CfgInfo);
                    pageStyleID = pageCfgInfo.PageStyleID;
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

        void ISubPageContainerRunTime.Init(SubPageContainerCfgInfo? cfgInfo, ISubPageContainerRunTimeCallBack callBack)
        {
            // 校验必要的回调接口
            if (callBack == null)
                throw new Exception("运行时回调接口不能为null");

            // 初始化视图模型并应用配置
            _viewModel = new PreviewViewModel();
            _cfgInfo = cfgInfo;
            ApplyConfiguration(cfgInfo);

            // 传递回调接口给视图模型
            _viewModel.SetRuntimeCallback(callBack);
        }

        bool ISubPageContainerRunTime.SetCurSubPaage(SubPageID subPageID)
        {
            // 检查视图模型是否已初始化
            if (_viewModel == null)
                return false;

            // 委托视图模型处理子页面切换逻辑
            return _viewModel.ActivateSubPage(subPageID);
        }

        #endregion

        /// <summary>
        /// 应用配置信息到视图模型
        /// </summary>
        /// <param name="cfgInfo">配置信息实例</param>
        private void ApplyConfiguration(SubPageContainerCfgInfo? cfgInfo)
        {
            if (cfgInfo == null || _viewModel == null)
                return;
            _viewModel.LoadConfiguration(cfgInfo.WorkAreaInfoes, cfgInfo.CfgInfo);
        }
    }
}
