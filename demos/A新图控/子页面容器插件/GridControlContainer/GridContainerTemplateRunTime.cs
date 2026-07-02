using Avalonia.Controls;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.Map.Page.UIContainer.GridContainer.Views;
using GKG.Map.Page.UIContainer.GridContainer.ViewModels;
using GKG.Map.Page.UIContainer.GridContainer.Models;
using System;

namespace GKG.Map.Page.UIContainer.GridContainer
{
    /// <summary>
    /// 网格子页面容器运行时接口实现
    /// (负责页面框架的初始化、运行时策略装配、子页面焦点联动等核心功能)
    /// </summary>
    public class GridContainerTemplateRunTime : ISubPageContainerRunTime
    {
        #region 私有字段

        private readonly PreviewViewModel _viewModel = new PreviewViewModel();
        private object _view = null!;

        #endregion

        #region ISubPageContainerRunTime 接口实现

        /// <summary>
        /// 显示界面实例
        /// </summary>
        object ISubPageContainerRunTime.View => _view;

        /// <summary>
        /// 运行时初始化
        /// </summary>
        void ISubPageContainerRunTime.Init(SubPageContainerCfgInfo data, ISubPageContainerRunTimeCallBack callBack)
        {
            ArgumentNullException.ThrowIfNull(callBack);

            ApplyConfiguration(data);

            _view = CreateRunTimeView(data);

            _viewModel.SetRuntimeCallback(callBack);
        }

        /// <summary>
        /// 设置指定的子页面为当前活动/聚焦子页面
        /// </summary>
        bool ISubPageContainerRunTime.SetCurSubPaage(SubPageID subPageID)
        {
            // 直接委托给视图模型及其策略去处理高亮/焦点联动
            return _viewModel.ActivateSubPage(subPageID);
        }

        #endregion

        #region 私有辅助方法

        /// <summary>
        /// 创建运行时视图 (支持样式扩展与兜底)
        /// </summary>
        private object CreateRunTimeView(SubPageContainerCfgInfo? cfgInfo)
        {
            // 1. 尝试从配置中解析出用户保存的样式 ID
            string pageStyleID = "";
            if (cfgInfo?.CfgInfo != null)
            {
                try
                {
                    var pageCfgInfo = new GridContainerTemplateCfgInfo();
                    pageCfgInfo.FromJsonBytes(cfgInfo.CfgInfo);
                    pageStyleID = pageCfgInfo.PageStyleID;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"运行时解析样式ID失败: {ex.Message}");
                }
            }

            // 2. 如果找到了样式 ID，去样式管理器里提取对应的 UI 控件
            if (!string.IsNullOrEmpty(pageStyleID))
            {
                var pageStyleInfo = PageStyleInfoMng.PageStyleInfoSource.Find(o => o.PageStyleID == pageStyleID);

                if (pageStyleInfo?.View is UserControl styleControl)
                {
                    styleControl.DataContext = _viewModel;
                    return styleControl;
                }
            }

            // 3. 兜底方案：如果没有样式或提取失败，返回最基础的默认网格视图
            return new PreviewView
            {
                DataContext = _viewModel
            };
        }

        /// <summary>
        /// 应用配置信息到视图模型
        /// </summary>
        private void ApplyConfiguration(SubPageContainerCfgInfo? cfgInfo)
        {
            if (cfgInfo == null) return;

            _viewModel.LoadConfiguration(cfgInfo.WorkAreaInfoes, cfgInfo.CfgInfo);
        }

        #endregion
    }
}