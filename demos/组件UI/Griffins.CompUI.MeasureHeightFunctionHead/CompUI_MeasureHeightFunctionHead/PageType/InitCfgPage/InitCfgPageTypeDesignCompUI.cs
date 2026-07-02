using Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.InitCfgPage.MeasureHeightInit.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.InitCfgPage
{
    /// <summary>
    /// 初始化配置页面类型设计组件UI
    /// </summary>
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        /// <summary>
        /// 获取页面类型ID
        /// </summary>
        /// <returns>页面类型ID</returns>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.InitCfgPage; }

        /// <summary>
        /// 获取页面类型组件UI视图信息
        /// </summary>
        /// <returns>页面类型组件UI视图信息列表</returns>
        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_MeasureHeightInit, ViewName = "测高初始化" },
            };
        }

        /// <summary>
        /// 获取页面类型组件UI视图
        /// </summary>
        /// <param name="viewID">视图ID</param>
        /// <returns>页面类型组件UI视图</returns>
        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_MeasureHeightInit => new MeasureHeightInitCompUIView(),
                _ => null,
            };
        }
    }
}
