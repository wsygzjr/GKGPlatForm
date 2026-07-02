using GF_Gereric;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.CylinderPushRodFactory;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.MotorPushRodFactory;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage
{
    internal class FactoryCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private readonly Guid _subMMObjID;
        private MotorPushRodSubMachineModulesFactoryCfg _motorData;
        private CylinderPushRodSubMachineModulesFactoryCfg _cylinderData;
        private IPageTypeRunTimeCompUIView _cylinderPushRodFactoryView;
        private IPageTypeRunTimeCompUIView _motorPushRodFactoryView;

        private event EventHandler AfterDataModified;

        public FactoryCfgPageTypeRunTimeCompUI(Guid subMMObjID)
        {
            _subMMObjID = subMMObjID;
        }

        protected override void _OnInit()
        {
            _motorData = new MotorPushRodSubMachineModulesFactoryCfg();
            _cylinderData = new CylinderPushRodSubMachineModulesFactoryCfg();
        }

        private void AfterModified(object sender, EventArgs e)
        {
            if (_motorPushRodFactoryView is MotorPushRodFactoryPageTypeRunTimeCompUIView motorView)
            {
                _motorData = motorView.GetData() ?? new MotorPushRodSubMachineModulesFactoryCfg();
            }

            if (_cylinderPushRodFactoryView is CylinderPushRodFactoryPageTypeRunTimeCompUIView cylinderView)
            {
                _cylinderData = cylinderView.GetData() ?? new CylinderPushRodSubMachineModulesFactoryCfg();
            }

            AfterDataModified?.Invoke(sender, e);
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return PageTypeID.Parse("FactoryCfgPage");
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID && viewID == FactoryCfgPageTypeConst.ViewID_MotorPushRodFactory)
            {
                if (_motorPushRodFactoryView != null)
                {
                    _motorPushRodFactoryView.AfterModified -= AfterModified;
                }

                _motorPushRodFactoryView = new MotorPushRodFactoryPageTypeRunTimeCompUIView(this.CallBack);
                _motorPushRodFactoryView.AfterModified += AfterModified;
                (_motorPushRodFactoryView as MotorPushRodFactoryPageTypeRunTimeCompUIView)?.SetData(
                    _motorData ?? new MotorPushRodSubMachineModulesFactoryCfg());
                return _motorPushRodFactoryView;
            }

            if (_subMMObjID == PushRodSubMachineModulesConst.CylinderSubMMObjID && viewID == FactoryCfgPageTypeConst.ViewID_CylinderPushRodFactory)
            {
                if (_cylinderPushRodFactoryView != null)
                {
                    _cylinderPushRodFactoryView.AfterModified -= AfterModified;
                }

                _cylinderPushRodFactoryView = new CylinderPushRodFactoryPageTypeRunTimeCompUIView(this.CallBack);
                _cylinderPushRodFactoryView.AfterModified += AfterModified;
                (_cylinderPushRodFactoryView as CylinderPushRodFactoryPageTypeRunTimeCompUIView)?.SetData(
                    _cylinderData ?? new CylinderPushRodSubMachineModulesFactoryCfg());
                return _cylinderPushRodFactoryView;
            }

            return null;
        }

        protected override void _SetData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID)
            {
                _motorData = JsonObjConvert.FromJSonBytes<MotorPushRodSubMachineModulesFactoryCfg>(data)
                    ?? new MotorPushRodSubMachineModulesFactoryCfg();
                if (_motorPushRodFactoryView is MotorPushRodFactoryPageTypeRunTimeCompUIView motorView)
                {
                    motorView.SetData(_motorData);
                }
            }
            else if (_subMMObjID == PushRodSubMachineModulesConst.CylinderSubMMObjID)
            {
                _cylinderData = JsonObjConvert.FromJSonBytes<CylinderPushRodSubMachineModulesFactoryCfg>(data)
                    ?? new CylinderPushRodSubMachineModulesFactoryCfg();
                if (_cylinderPushRodFactoryView is CylinderPushRodFactoryPageTypeRunTimeCompUIView cylinderView)
                {
                    cylinderView.SetData(_cylinderData);
                }
            }
        }

        protected override byte[] _GetData()
        {
            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID)
            {
                if (_motorPushRodFactoryView is MotorPushRodFactoryPageTypeRunTimeCompUIView motorView)
                {
                    _motorData = motorView.GetData() ?? new MotorPushRodSubMachineModulesFactoryCfg();
                }

                return JsonObjConvert.ToJSonBytes(_motorData ?? new MotorPushRodSubMachineModulesFactoryCfg());
            }

            if (_subMMObjID == PushRodSubMachineModulesConst.CylinderSubMMObjID)
            {
                if (_cylinderPushRodFactoryView is CylinderPushRodFactoryPageTypeRunTimeCompUIView cylinderView)
                {
                    _cylinderData = cylinderView.GetData() ?? new CylinderPushRodSubMachineModulesFactoryCfg();
                }

                return JsonObjConvert.ToJSonBytes(_cylinderData ?? new CylinderPushRodSubMachineModulesFactoryCfg());
            }

            return JsonObjConvert.ToJSonBytes(Array.Empty<byte>());
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            return null;
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = Array.Empty<string>();
            return true;
        }
    }
}
