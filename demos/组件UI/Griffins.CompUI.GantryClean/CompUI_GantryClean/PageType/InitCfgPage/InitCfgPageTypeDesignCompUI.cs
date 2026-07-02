using Griffins.Map;
using Griffins.Map.UI;
using Griffins.CompUI.GantryClean.CompUI_GantryClean.PageType.InitCfgPage.GantryCleanConfig.Views;
using Griffins.ImeIOT.Map;

namespace Griffins.CompUI.GantryClean.CompUI_GantryClean.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("InitCfgPage"); }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_GantryCleanConfig, ViewName = "龙门清洗配置" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_GantryCleanConfig => new GantryCleanConfigCompUIView(),
                _ => null,
            };
        }
    }
}
