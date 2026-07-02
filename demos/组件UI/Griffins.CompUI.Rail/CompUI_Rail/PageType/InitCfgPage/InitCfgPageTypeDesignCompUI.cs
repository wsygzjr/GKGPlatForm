using Griffins.CompUI.Rail.CompUI_Rail.PageType.InitCfgPage.RailInit.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.Rail.CompUI_Rail.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList
            {
                new PageTypeCompUIViewInfo
                {
                    ViewID = InitCfgPageTypeConst.ViewID_RailInit,
                    ViewName = "单层轨道初始化配置",
                },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_RailInit => new RailInitCompUIView(),
                _ => null,
            };
        }
    }
}
