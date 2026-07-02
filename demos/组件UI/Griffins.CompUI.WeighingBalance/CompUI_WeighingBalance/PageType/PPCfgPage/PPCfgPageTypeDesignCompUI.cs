using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.PPCfgPage.WeighingBalancePP.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.PPCfgPage
{
    /// <summary>
    /// 配方配置页面类型设计组件UI
    /// </summary>
    internal class PPCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        /// <summary>
        /// 获取页面类型ID
        /// </summary>
        /// <returns>页面类型ID</returns>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.PPCfgPage; }

        /// <summary>
        /// 获取页面类型组件UI视图信息
        /// </summary>
        /// <returns>页面类型组件UI视图信息列表</returns>
        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = PPCfgPageTypeConst.ViewID_WeighingBalancePP, ViewName = "称重参数" },
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
                PPCfgPageTypeConst.ViewID_WeighingBalancePP => new WeighingBalancePPCompUIView(),
                _ => null,
            };
        }
    }
}
