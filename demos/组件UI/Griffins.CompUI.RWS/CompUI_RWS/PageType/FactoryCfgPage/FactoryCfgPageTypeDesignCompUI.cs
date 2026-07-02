using Griffins.CompUI.RWS.CompUI_RWS.PageType.FactoryCfgPage.WorkStationFactoryConfig.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.FactoryCfgPage
{
    internal class FactoryCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.FactoryCfgPage; }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList
            {
                new PageTypeCompUIViewInfo
                {
                    ViewID = FactoryCfgPageTypeConst.ViewID_WorkStationFactoryConfig,
                    ViewName = "轨道工位工厂配置",
                }
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID == FactoryCfgPageTypeConst.ViewID_WorkStationFactoryConfig
                ? new WorkStationFactoryConfigCompUIView()
                : null;
        }
    }
}
