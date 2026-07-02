using GKG.SubMM;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.CylinderPushRodInit.Views;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.MotorPushRodInit.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage
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

            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID)
            {
                list.Add(new PageTypeCompUIViewInfo
                {
                    ViewID = InitCfgPageTypeConst.ViewID_MotorPushRodInit,
                    ViewName = PushRodSubMachineModulesConst.MotorSubMMObjName + "初始化配置"
                });
            }
            else if (_subMMObjID == PushRodSubMachineModulesConst.CylinderSubMMObjID)
            {
                list.Add(new PageTypeCompUIViewInfo
                {
                    ViewID = InitCfgPageTypeConst.ViewID_CylinderPushRodInit,
                    ViewName = PushRodSubMachineModulesConst.CylinderSubMMObjName + "初始化配置"
                });
            }

            return list;
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_MotorPushRodInit => new MotorPushRodInitCompUIView(),
                InitCfgPageTypeConst.ViewID_CylinderPushRodInit => new CylinderPushRodInitCompUIView(),
                _ => null,
            };
        }
    }
}
