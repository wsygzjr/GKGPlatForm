using GF_Gereric;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage.MotorPushRodRecipe;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage
{
    internal class RecipeCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private readonly Guid _subMMObjID;
        private MotorPushRodSubMachineModulesPPCfg _motorData;
        private IPageTypeRunTimeCompUIView _motorPushRodView;

        private event EventHandler AfterDataModified;

        public RecipeCfgPageTypeRunTimeCompUI(Guid subMMObjID)
        {
            _subMMObjID = subMMObjID;
        }

        protected override void _OnInit()
        {
            _motorData = new MotorPushRodSubMachineModulesPPCfg();
        }

        private void AfterModified(object sender, EventArgs e)
        {
            if (_motorPushRodView is MotorPushRodRecipePageTypeRunTimeCompUIView motorView)
            {
                _motorData = motorView.GetData() ?? new MotorPushRodSubMachineModulesPPCfg();
            }

            AfterDataModified?.Invoke(sender, e);
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.PPCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID && viewID == RecipeCfgPageTypeConst.ViewID_MotorPushRodRecipe)
            {
                if (_motorPushRodView != null)
                {
                    (_motorPushRodView as MotorPushRodRecipePageTypeRunTimeCompUIView)?.Cleanup();
                    _motorPushRodView.AfterModified -= AfterModified;
                }

                _motorPushRodView = new MotorPushRodRecipePageTypeRunTimeCompUIView(this.CallBack);
                _motorPushRodView.AfterModified += AfterModified;
                (_motorPushRodView as MotorPushRodRecipePageTypeRunTimeCompUIView)?.SetData(
                    _motorData ?? new MotorPushRodSubMachineModulesPPCfg());
                return _motorPushRodView;
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
                _motorData = JsonObjConvert.FromJSonBytes<MotorPushRodSubMachineModulesPPCfg>(data)
                    ?? new MotorPushRodSubMachineModulesPPCfg();
                if (_motorPushRodView is MotorPushRodRecipePageTypeRunTimeCompUIView motorView)
                {
                    motorView.SetData(_motorData);
                }
            }
        }

        protected override byte[] _GetData()
        {
            if (_subMMObjID == PushRodSubMachineModulesConst.MotorSubMMObjID)
            {
                if (_motorPushRodView is MotorPushRodRecipePageTypeRunTimeCompUIView motorView)
                {
                    _motorData = motorView.GetData() ?? new MotorPushRodSubMachineModulesPPCfg();
                }

                return JsonObjConvert.ToJSonBytes(_motorData ?? new MotorPushRodSubMachineModulesPPCfg());
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
