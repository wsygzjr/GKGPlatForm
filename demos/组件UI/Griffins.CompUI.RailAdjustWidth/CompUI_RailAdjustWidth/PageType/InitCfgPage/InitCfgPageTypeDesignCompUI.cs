using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage.AdjustWidthInit.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.InitCfgPage; }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_AdjustWidthInit, ViewName = "调宽初始化" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_AdjustWidthInit => new AdjustWidthInitCompUIView(),
                _ => null,
            };
        }
    }
}
