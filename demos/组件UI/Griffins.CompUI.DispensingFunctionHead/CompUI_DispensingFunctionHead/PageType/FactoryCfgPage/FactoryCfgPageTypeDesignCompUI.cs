using GKG.SubMM.Dispenser;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.FactoryCfgPage.DispensingFunctionHeadFactory.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.FactoryCfgPage
{
    internal class FactoryCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
       
        private readonly Guid _subMMObjID;

        public FactoryCfgPageTypeDesignCompUI(Guid subMMObjID)
        {
            _subMMObjID = subMMObjID;
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.FactoryCfgPage;
        }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            var list = new PageTypeCompUIViewInfoList();

            list.Add(new PageTypeCompUIViewInfo
            {
                ViewID = FactoryCfgPageTypeConst.ViewID_DispensingFunctionHeadFactory,
                ViewName = DispensingFunctionHeadSubMachineModulesConst.SubMMName + "出厂配置"
            });

            return list;
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                FactoryCfgPageTypeConst.ViewID_DispensingFunctionHeadFactory => new DispensingFunctionHeadFactoryCompUIView(),
                _ => null,
            };
        }
    }
}
