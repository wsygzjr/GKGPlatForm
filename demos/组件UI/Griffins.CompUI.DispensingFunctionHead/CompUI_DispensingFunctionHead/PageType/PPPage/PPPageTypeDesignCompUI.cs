using GKG.SubMM.Dispenser;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage.DispensingFunctionHeadPP.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage
{
    /// <summary>
    /// 配方页面设计时CompUI
    /// </summary>
    internal class PPPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        private readonly Guid _subMMObjID;

        public PPPageTypeDesignCompUI(Guid subMMObjID)
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

            list.Add(new PageTypeCompUIViewInfo
            {
                ViewID = PPPageTypeConst.ViewID_DispensingFunctionHeadPP,
                ViewName = DispensingFunctionHeadSubMachineModulesConst.SubMMName + "配方配置"
            });

            return list;
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                PPPageTypeConst.ViewID_DispensingFunctionHeadPP => new DispensingFunctionHeadPPCompUIView(),
                _ => null,
            };
        }
    }
}
