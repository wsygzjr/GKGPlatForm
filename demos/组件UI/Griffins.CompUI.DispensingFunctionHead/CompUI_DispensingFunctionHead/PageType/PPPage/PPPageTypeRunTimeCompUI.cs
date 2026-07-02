using GF_Gereric;
using GKG;
using GKG.SubMM.Dispenser;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage.DispensingFunctionHeadPP;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage
{
    /// <summary>
    /// 配方页面运行时CompUI
    /// </summary>
    internal class PPPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private readonly Guid _subMMObjID;
        private DispensingFunctionHeadSubMachineModulesPPCfg _data;
        private IPageTypeRunTimeCompUIView _ppView;

        private event EventHandler AfterDataModified;

        public PPPageTypeRunTimeCompUI(Guid subMMObjID)
        {
            _subMMObjID = subMMObjID;
        }

        protected override void _OnInit()
        {
            _data = new DispensingFunctionHeadSubMachineModulesPPCfg();
        }

        private void AfterModified(object sender, EventArgs e)
        {
            if (_ppView is DispensingFunctionHeadPPPageTypeRunTimeCompUIView view)
            {
                _data = view.GetData() ?? new DispensingFunctionHeadSubMachineModulesPPCfg();
            }

            AfterDataModified?.Invoke(sender, e);
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.PPCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == PPPageTypeConst.ViewID_DispensingFunctionHeadPP)
            {
                if (_ppView != null)
                {
                    _ppView.AfterModified -= AfterModified;
                }

                _ppView = new DispensingFunctionHeadPPPageTypeRunTimeCompUIView(this.CallBack);
                _ppView.AfterModified += AfterModified;
                (_ppView as DispensingFunctionHeadPPPageTypeRunTimeCompUIView)?.SetData(
                    _data ?? new DispensingFunctionHeadSubMachineModulesPPCfg());
                return _ppView;
            }

            return null;
        }

        protected override void _SetData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            _data = JsonObjConvert.FromJSonBytes<DispensingFunctionHeadSubMachineModulesPPCfg>(data)
                ?? new DispensingFunctionHeadSubMachineModulesPPCfg();
            if (_ppView is DispensingFunctionHeadPPPageTypeRunTimeCompUIView view)
            {
                view.SetData(_data);
            }
        }

        protected override byte[] _GetData()
        {
            if (_ppView is DispensingFunctionHeadPPPageTypeRunTimeCompUIView view)
            {
                _data = view.GetData() ?? new DispensingFunctionHeadSubMachineModulesPPCfg();
            }

            return JsonObjConvert.ToJSonBytes(_data ?? new DispensingFunctionHeadSubMachineModulesPPCfg());
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
