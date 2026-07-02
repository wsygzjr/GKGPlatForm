using Griffins.CompUI.Vision.CompUI.PageType.InitCfgPage;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.Vision;

namespace Griffins.CompUI.Vision.CompUI.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("InitCfgPage"); }
        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_Vision, ViewName = "视觉" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return null;

        }
    }
}
