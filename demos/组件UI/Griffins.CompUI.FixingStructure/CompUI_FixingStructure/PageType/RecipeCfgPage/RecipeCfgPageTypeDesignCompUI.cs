using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage.AxisFixRecipeConfig.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage
{
    /// <summary>
    /// 电机固定机构配方页设计态入口。
    /// </summary>
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
                    ViewID = RecipeCfgPageTypeConst.ViewID_AxisFixRecipeConfig,
                    ViewName = "电机固定机构配方配置",
                },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                RecipeCfgPageTypeConst.ViewID_AxisFixRecipeConfig => new AxisFixRecipeConfigCompUIView(),
                _ => null,
            };
        }
    }
}
