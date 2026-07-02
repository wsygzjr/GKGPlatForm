using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using Griffins.PF;
using Griffins.UI2.PropertyGrid;
using NonMainFrameView.Views;
using NonMainFrameViewModel.Models;
using NonMainFrameViewModel.ViewModels;
using System;

namespace NonMainFrame
{
    /// <summary>
    /// 非主页面框架模板设计时配置接口实现
    /// </summary>
    public class NonMainPageFrameTemplateCfg : IPageFrameTemplateCfg
    {
        private Window? _configWindow;
        private ConfigViewModel? _configViewModel;

        /// <summary>
        /// 页面框架模板信息修改事件
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 页面框架模板配置信息
        /// </summary>
        private PageFrameTemplateCfgInfo _cfgInfo = new PageFrameTemplateCfgInfo();
        public PageFrameTemplateCfgInfo CfgInfo
        {
            get => _cfgInfo;
            set
            {
                if (_cfgInfo != value)
                {
                    _cfgInfo = value ?? new PageFrameTemplateCfgInfo();
                    convertCfgInfoToViewModel();
                }
            }
        }

        /// <summary>
        /// 显示配置窗口
        /// </summary>
        /// <param name="owner">归属窗口</param>
        public void Show(object owner)
        {
            if (owner is not Window ownerWindow)
                throw new ArgumentException("所有者必须是Window类型", nameof(owner));

            // 清理之前的窗口实例
            cleanupWindows();

            // 创建属性窗口和配置窗口
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
        }

        /// <summary>
        /// 创建配置窗口
        /// </summary>
        private Window createConfigWindow()
        {
            var configWindow=  new ConfigWindow
            {
                Title = "插件配置",
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

            var mainPageCfgInfo = new NonMainPageFrameTemplateCfgInfo();

            if (CfgInfo.CfgInfo != null)
            {
                mainPageCfgInfo.FromJsonBytes(CfgInfo.CfgInfo);
            }
            else
            {
                // 初始化默认工作区
                CfgInfo.WorkAreaInfoes ??= new WorkAreaInfoList();
                if (CfgInfo.WorkAreaInfoes.Count == 0)
                {
                    CfgInfo.WorkAreaInfoes.Add(new WorkAreaInfo
                    {
                        WorkAreaID = Guid.NewGuid().ToString(),
                        WorkAreaName = "默认工作区",
                        WorkAreaKind = WorkAreaKind.Dynamic
                    });
                }
            }

            _configViewModel.CopyFrom(mainPageCfgInfo, CfgInfo.WorkAreaInfoes);
        }

        /// <summary>
        /// 从视图模型提取配置信息
        /// </summary>
        private void extractCfgInfoFromViewModel()
        {
            if (_configViewModel is null) return;

            var mainPageCfgInfo = new NonMainPageFrameTemplateCfgInfo();
            _configViewModel.Extract(mainPageCfgInfo, CfgInfo.WorkAreaInfoes);
            CfgInfo.CfgInfo = mainPageCfgInfo.ToJsonBytes();
        }

        /// <summary>
        /// 处理配置修改事件
        /// </summary>
        private void onConfigModified(object? sender, EventArgs e)
        {
            extractCfgInfoFromViewModel();
            AfterModified?.Invoke(this, EventArgs.Empty);
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

            //if (_propertyWindow != null)
            //{
            //    _propertyWindow.Close();
            //    _propertyWindow = null;
            //}

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
    }
}
