using GKG.SubMM;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.CylinderPushRodFactory.Views;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.MotorPushRodFactory.Views;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage
{
    internal class FactoryCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        private static readonly PageTypeID FactoryCfgPageTypeID = PageTypeID.Parse("FactoryCfgPage");
        private readonly Guid _subMMObjID;

        public FactoryCfgPageTypeDesignCompUI(Guid subMMObjID)
        {
            _subMMObjID = subMMObjID;
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return FactoryCfgPageTypeID;
        }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            var list = new PageTypeCompUIViewInfoList();

            if (_subMMObjID == PushRodSubMachineModulesConst.CylinderSubMMObjID)
            {
                list.Add(new PageTypeCompUIViewInfo
                {
                    ViewID = FactoryCfgPageTypeConst.ViewID_CylinderPushRodFactory,
                    ViewName = PushRodSubMachineModulesConst.CylinderSubMMObjName + "出厂配置"
                });
            }

            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID)
            {
                list.Add(new PageTypeCompUIViewInfo
                {
                    ViewID = FactoryCfgPageTypeConst.ViewID_MotorPushRodFactory,
                    ViewName = PushRodSubMachineModulesConst.MotorSubMMObjName + "出厂配置"
                });
            }

            return list;
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                FactoryCfgPageTypeConst.ViewID_CylinderPushRodFactory => new CylinderPushRodFactoryCompUIView(),
                FactoryCfgPageTypeConst.ViewID_MotorPushRodFactory => new MotorPushRodFactoryCompUIView(),
                _ => null,
            };
        }
    }
}
