using Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage.WorkStationInitConfig.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.InitCfgPage; }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_WorkStationInitConfig, ViewName = "轨道工位配置" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == InitCfgPageTypeConst.ViewID_WorkStationInitConfig)
            {
                return new WorkStationInitConfigCompUIView();
            }

            return null;
        }
    }
}
