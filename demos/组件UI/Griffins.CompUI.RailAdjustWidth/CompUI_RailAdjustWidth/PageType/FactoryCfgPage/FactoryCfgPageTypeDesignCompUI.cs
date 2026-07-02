using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.FactoryCfgPage.AdjustWidthFactory.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.FactoryCfgPage
{
    internal class FactoryCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.FactoryCfgPage; }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = FactoryCfgPageTypeConst.ViewID_AdjustWidthFactory, ViewName = "调宽出厂参数" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                FactoryCfgPageTypeConst.ViewID_AdjustWidthFactory => new AdjustWidthFactoryCompUIView(),
                _ => null,
            };
        }
    }
}
