using GF_Gereric;
using Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.FactoryCfgPage;
using Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.InitCfgPage;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Threading.Tasks;

namespace Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead
{
    /// <summary>
    /// 测高功能头组件UI
    /// </summary>
    [CompUI("MeasureHeightFunctionHead", ImeIOTConst.CompType_SubMMStr)]
    public class MeasureHeightCompUI : CompUIBase
    {
        /// <summary>
        /// 获取组件名称
        /// </summary>
        /// <returns>组件名称</returns>
        protected override string _GetCompName() { return "测高功能头子机械模块"; }

        /// <summary>
        /// 获取页面类型设计组件UI
        /// </summary>
        /// <param name="pageTypeID">页面类型ID</param>
        /// <param name="guid">GUID</param>
        /// <returns>页面类型设计组件UI</returns>
        protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (pageTypeID == ImeIOTConst.FactoryCfgPage)
            {
                return new FactoryCfgPageTypeDesignCompUI();
            }
            else if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeDesignCompUI();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取页面类型运行时组件UI
        /// </summary>
        /// <param name="pageTypeID">页面类型ID</param>
        /// <param name="guid">GUID</param>
        /// <returns>页面类型运行时组件UI</returns>
        protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid guid)
        {
            if (pageTypeID == ImeIOTConst.FactoryCfgPage)
            {
                return new FactoryCfgPageTypeRunTimeCompUI();
            }
            else if (pageTypeID == ImeIOTConst.InitCfgPage)
            {
                return new InitCfgPageTypeRunTimeCompUI();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 空控制面板
        /// </summary>
        private class EmptyControlPanel : IControlPanel
        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="iControlPanelCallBack">控制面板回调</param>
            void IControlPanel.Init(IControlPanelCallBack iControlPanelCallBack)
            {
            }

            /// <summary>
            /// 获取控制面板视图信息
            /// </summary>
            /// <returns>控制面板视图信息列表</returns>
            ControlPanelViewInfoList IControlPanel.GetControlPanelViewInfos()
            {
                return new ControlPanelViewInfoList();
            }

            /// <summary>
            /// 显示控制面板
            /// </summary>
            /// <param name="controlPanelID">控制面板ID</param>
            /// <param name="owner">所有者</param>
            /// <returns>任务</returns>
            public Task ShowControlPanelAsync(string controlPanelID, object owner)
            {
                return Task.CompletedTask;
            }
        }
    }
}
