using Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.FactoryCfgPage.MeasureHeightFactory.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.FactoryCfgPage
{
    /// <summary>
    /// 工厂配置页面类型设计组件UI
    /// </summary>
    internal class FactoryCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        /// <summary>
        /// 获取页面类型ID
        /// </summary>
        /// <returns>页面类型ID</returns>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.FactoryCfgPage; }

        /// <summary>
        /// 获取页面类型组件UI视图信息
        /// </summary>
        /// <returns>页面类型组件UI视图信息列表</returns>
        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = FactoryCfgPageTypeConst.ViewID_MeasureHeightFactory, ViewName = "测高出厂配置" },
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
                FactoryCfgPageTypeConst.ViewID_MeasureHeightFactory => new MeasureHeightFactoryCompUIView(),
                _ => null,
            };
        }
    }
}
