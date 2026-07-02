using GF_Gereric;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.CylinderPushRodInit;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.MotorPushRodInit;
using GKG;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using GKG.SubMM;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private readonly Guid _subMMObjID;
        private IPageTypeRunTimeCompUIView _initView;
        private MotorPushRodSubMachineModulesInitCfg _motorData;
        private CylinderPushRodSubMachineModulesInitCfg _cylinderData;
        private bool _isApplyingViewData;

        public InitCfgPageTypeRunTimeCompUI(Guid subMMObjID)
        {
            _subMMObjID = subMMObjID;
        }

        protected override void _OnInit()
        {
            _motorData = new MotorPushRodSubMachineModulesInitCfg();
            _cylinderData = new CylinderPushRodSubMachineModulesInitCfg();
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID && viewID == InitCfgPageTypeConst.ViewID_MotorPushRodInit)
            {
                if (_initView != null)
                {
                    _initView.AfterModified -= OnAfterModified;
                }

                _initView = new MotorPushRodInitPageTypeRunTimeCompUIView(this.CallBack);
                _initView.AfterModified += OnAfterModified;
                ApplyViewData(_motorData ?? new MotorPushRodSubMachineModulesInitCfg());
                return _initView;
            }

            if (_subMMObjID == PushRodSubMachineModulesConst.CylinderSubMMObjID && viewID == InitCfgPageTypeConst.ViewID_CylinderPushRodInit)
            {
                if (_initView != null)
                {
                    _initView.AfterModified -= OnAfterModified;
                }

                _initView = new CylinderPushRodInitPageTypeRunTimeCompUIView(this.CallBack);
                _initView.AfterModified += OnAfterModified;
                ApplyViewData(_cylinderData ?? new CylinderPushRodSubMachineModulesInitCfg());
                return _initView;
            }

            return null;
        }

        protected override void _SetData(byte[] data)
        {
            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID)
            {
                _motorData = data == null || data.Length == 0
                    ? new MotorPushRodSubMachineModulesInitCfg()
                    : JsonObjConvert.FromJSonBytes<MotorPushRodSubMachineModulesInitCfg>(data)
                        ?? new MotorPushRodSubMachineModulesInitCfg();
                ApplyViewData(_motorData);
            }
            else if (_subMMObjID == PushRodSubMachineModulesConst.CylinderSubMMObjID)
            {
                _cylinderData = data == null || data.Length == 0
                    ? new CylinderPushRodSubMachineModulesInitCfg()
                    : JsonObjConvert.FromJSonBytes<CylinderPushRodSubMachineModulesInitCfg>(data)
                        ?? new CylinderPushRodSubMachineModulesInitCfg();
                ApplyViewData(_cylinderData);
            }
        }

        protected override byte[] _GetData()
        {
            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID)
            {
                if (_initView is MotorPushRodInitPageTypeRunTimeCompUIView motorView)
                {
                    _motorData = motorView.GetData();
                }

                return JsonObjConvert.ToJSonBytes(_motorData ?? new MotorPushRodSubMachineModulesInitCfg());
            }

            if (_subMMObjID == PushRodSubMachineModulesConst.CylinderSubMMObjID)
            {
                if (_initView is CylinderPushRodInitPageTypeRunTimeCompUIView cylinderView)
                {
                    _cylinderData = cylinderView.GetData();
                }

                return JsonObjConvert.ToJSonBytes(_cylinderData ?? new CylinderPushRodSubMachineModulesInitCfg());
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

        private void OnAfterModified(object sender, EventArgs e)
        {
            if (_isApplyingViewData)
                return;

            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID && _initView is MotorPushRodInitPageTypeRunTimeCompUIView motorView)
            {
                _motorData = motorView.GetData();
            }
            else if (_subMMObjID == PushRodSubMachineModulesConst.CylinderSubMMObjID && _initView is CylinderPushRodInitPageTypeRunTimeCompUIView cylinderView)
            {
                _cylinderData = cylinderView.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
        }

        private void ApplyViewData(MotorPushRodSubMachineModulesInitCfg data)
        {
            if (_initView is not MotorPushRodInitPageTypeRunTimeCompUIView motorView)
                return;

            _isApplyingViewData = true;
            try
            {
                motorView.SetData(data);
            }
            finally
            {
                _isApplyingViewData = false;
            }
        }

        private void ApplyViewData(CylinderPushRodSubMachineModulesInitCfg data)
        {
            if (_initView is not CylinderPushRodInitPageTypeRunTimeCompUIView cylinderView)
                return;

            _isApplyingViewData = true;
            try
            {
                cylinderView.SetData(data);
            }
            finally
            {
                _isApplyingViewData = false;
            }
        }
    }
}
