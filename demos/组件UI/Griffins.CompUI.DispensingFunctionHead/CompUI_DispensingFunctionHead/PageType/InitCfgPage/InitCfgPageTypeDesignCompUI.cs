using GKG.SubMM.Dispenser;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage.DispensingFunctionHeadInit.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        private readonly Guid _subMMObjID;

        public InitCfgPageTypeDesignCompUI(Guid subMMObjID)
        {
            _subMMObjID = subMMObjID;
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            var list = new PageTypeCompUIViewInfoList();

            list.Add(new PageTypeCompUIViewInfo
            {
                ViewID = InitCfgPageTypeConst.ViewID_DispensingFunctionHeadInit,
                ViewName = DispensingFunctionHeadSubMachineModulesConst.SubMMName + "初始化配置"
            });

            return list;
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_DispensingFunctionHeadInit => new DispensingFunctionHeadInitCompUIView(),
                _ => null,
            };
        }
    }
}
