using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using GKG.Map.Page.UIContainer.GridContainer.Models;
using GKG.Map.Page.UIContainer.GridContainer.ViewModels;
using GKG.Map.Page.UIContainer.GridContainer.Views;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using Griffins.UI2.PropertyGrid;

namespace GKG.Map.Page.UIContainer.GridContainer
{
    /// <summary>
    /// 网格子页面容器设计时配置接口实现
    /// </summary>
    public class GridContainerTemplateCfg : ISubPageContainerCfg
    {
        private Window? _configWindow;
        private ConfigViewModel? _configViewModel;

        private SubPageContainerCfgInfo _cfgInfo = new SubPageContainerCfgInfo();
        private EventHandler? _afterModified;

        #region ISubPageContainerCfg接口实现

        event EventHandler ISubPageContainerCfg.AfterModified
        {
            add => _afterModified += value;
            remove => _afterModified -= value;
        }

        SubPageContainerCfgInfo ISubPageContainerCfg.CfgInfo
        {
            get => _cfgInfo;
            set
            {
                if (_cfgInfo != value)
                {
                    _cfgInfo = value ?? new SubPageContainerCfgInfo();
                    convertCfgInfoToViewModel();
                }
            }
        }
        
        void ISubPageContainerCfg.Show(object owner)
        {
            if (owner is not Window ownerWindow)
                throw new ArgumentException("所有者必须是Window类型", nameof(owner));

            // 清理之前的窗口实例
            cleanupWindows();

            // 创建配置窗口
            _configWindow = createConfigWindow();

            // 初始化视图模型并同步数据
            _configViewModel = new ConfigViewModel();
            convertCfgInfoToViewModel();
            _configViewModel.SetViewReference(_configWindow);
            _configViewModel.AfterModified += onConfigModified;
            _configWindow.DataContext = _configViewModel;

            // 显示窗口并定位属性窗口
            _configWindow.ShowDialog(ownerWindow);

            //显示属性窗口
            StaticDataMng.SetViewReference(_configWindow);
            StaticDataMng.ShowPropertyWindow();
            _configViewModel.RegisterPropertyGridPropertyChanged();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 创建配置窗口
        /// </summary>
        private Window createConfigWindow()
        {
            var configWindow=  new ConfigWindow
            {
                Title = "动态网格配置",
                Width = 800,
                Height = 600,
            };
            configWindow.Closed += onConfigWindowClosed;
            return configWindow;
        }

        /// <summary>
        /// 将配置信息转换到视图模型
        /// </summary>
        private void convertCfgInfoToViewModel()
        {
            if (_configViewModel is null) return;

            var mainPageCfgInfo = new GridContainerTemplateCfgInfo();

            if (_cfgInfo.CfgInfo != null)
            {
                mainPageCfgInfo.FromJsonBytes(_cfgInfo.CfgInfo);
            }

            // 确保工作区信息列表不为null
            _cfgInfo.WorkAreaInfoes ??= new WorkAreaInfoList();

            _configViewModel.CopyFrom(mainPageCfgInfo, _cfgInfo.WorkAreaInfoes);
        }

        /// <summary>
        /// 从视图模型提取配置信息
        /// </summary>
        private void extractCfgInfoFromViewModel()
        {
            if (_configViewModel is null) return;

            var mainPageCfgInfo = new GridContainerTemplateCfgInfo();

            _configViewModel.Extract(mainPageCfgInfo, _cfgInfo.WorkAreaInfoes);
            _cfgInfo.CfgInfo = mainPageCfgInfo.ToJsonBytes();
        }

        /// <summary>
        /// 处理配置修改事件
        /// </summary>
        private void onConfigModified(object? sender, EventArgs e)
        {
            extractCfgInfoFromViewModel();
            _afterModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 清理窗口资源
        /// </summary>
        private void cleanupWindows()
        {
            if (_configWindow != null)
            {
                _configWindow.Closed -= onConfigWindowClosed;
                _configWindow = null;
            }

            if (_configViewModel != null)
            {
                _configViewModel.AfterModified -= onConfigModified;
                _configViewModel = null;
            }
        }

        /// <summary>
        /// 配置窗口关闭时清理资源
        /// </summary>
        private void onConfigWindowClosed(object? sender, EventArgs e)
        {
            cleanupWindows();
        }

        #endregion
    }
}
