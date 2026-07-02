using GKG.SubMM;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage.MotorPushRodRecipe.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage
{
    internal class RecipeCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        private readonly Guid _subMMObjID;

        public RecipeCfgPageTypeDesignCompUI(Guid subMMObjID)
        {
            _subMMObjID = subMMObjID;
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.PPCfgPage;
        }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            var list = new PageTypeCompUIViewInfoList();
            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID)
            {
                list.Add(new PageTypeCompUIViewInfo
                {
                    ViewID = RecipeCfgPageTypeConst.ViewID_MotorPushRodRecipe,
                    ViewName = PushRodSubMachineModulesConst.MotorSubMMObjName + "配方配置"
                });
            }

            return list;
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                RecipeCfgPageTypeConst.ViewID_MotorPushRodRecipe => new MotorPushRodRecipeCompUIView(),
                _ => null,
            };
        }
    }
}
