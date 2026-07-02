using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage.AdjustWidthRecipe.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage
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
                    ViewID = RecipeCfgPageTypeConst.ViewID_AdjustWidthRecipe,
                    ViewName = "调宽配方参数",
                },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID == RecipeCfgPageTypeConst.ViewID_AdjustWidthRecipe
                ? new AdjustWidthRecipeCompUIView()
                : null;
        }
    }
}
