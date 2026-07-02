using Griffins.CompUI.Rail.CompUI_Rail.PageType.RecipeCfgPage.RailRecipe.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.Rail.CompUI_Rail.PageType.RecipeCfgPage
{
    internal class RecipeCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.PPCfgPage;
        }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList
            {
                new PageTypeCompUIViewInfo
                {
                    ViewID = RecipeCfgPageTypeConst.ViewID_RailRecipe,
                    ViewName = "单层轨道配方配置",
                },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                RecipeCfgPageTypeConst.ViewID_RailRecipe => new RailRecipeCompUIView(),
                _ => null,
            };
        }
    }
}
