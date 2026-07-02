using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.RecipeCfgPage
{
    internal class RecipeCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.PPCfgPage; }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList
            {
                new PageTypeCompUIViewInfo
                {
                    ViewID = RecipeCfgPageTypeConst.ViewID_TransSpeed,
                    ViewName = "工位运输速度配置",
                }
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID == RecipeCfgPageTypeConst.ViewID_TransSpeed
                ? new Views.TransSpeedRecipeCompUIView()
                : null;
        }
    }
}
