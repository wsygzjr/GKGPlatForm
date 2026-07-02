using GF_Gereric;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage.DispensingFunctionHeadInit;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG;
using GKG.SubMM.Dispenser;
using System;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private readonly Guid _subMMObjID;
        private DispensingFunctionHeadSubMachineModulesInitCfg _data;
        private IPageTypeRunTimeCompUIView _initView;
        private event EventHandler AfterDataModified;

        public InitCfgPageTypeRunTimeCompUI(Guid subMMObjID)
        {
            _subMMObjID = subMMObjID;
        }

        protected override void _OnInit()
        {
            _data = new DispensingFunctionHeadSubMachineModulesInitCfg();
        }

        private void AfterModified(object sender, EventArgs e)
        {
            if (_initView is DispensingFunctionHeadInitPageTypeRunTimeCompUIView view)
            {
                _data = view.GetData() ?? new DispensingFunctionHeadSubMachineModulesInitCfg();
            }

            AfterDataModified?.Invoke(sender, e);
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == InitCfgPageTypeConst.ViewID_DispensingFunctionHeadInit)
            {
                if (_initView != null)
                {
                    _initView.AfterModified -= AfterModified;
                }

                _initView = new DispensingFunctionHeadInitPageTypeRunTimeCompUIView(this.CallBack);
                _initView.AfterModified += AfterModified;
                (_initView as DispensingFunctionHeadInitPageTypeRunTimeCompUIView)?.SetData(
                    _data ?? new DispensingFunctionHeadSubMachineModulesInitCfg());
                return _initView;
            }

            return null;
        }

        protected override void _SetData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            _data = JsonObjConvert.FromJSonBytes<DispensingFunctionHeadSubMachineModulesInitCfg>(data)
                ?? new DispensingFunctionHeadSubMachineModulesInitCfg();
            if (_initView is DispensingFunctionHeadInitPageTypeRunTimeCompUIView view)
            {
                view.SetData(_data);
            }
        }

        protected override byte[] _GetData()
        {
            if (_initView is DispensingFunctionHeadInitPageTypeRunTimeCompUIView view)
            {
                _data = view.GetData() ?? new DispensingFunctionHeadSubMachineModulesInitCfg();
            }

            return JsonObjConvert.ToJSonBytes(_data ?? new DispensingFunctionHeadSubMachineModulesInitCfg());
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
