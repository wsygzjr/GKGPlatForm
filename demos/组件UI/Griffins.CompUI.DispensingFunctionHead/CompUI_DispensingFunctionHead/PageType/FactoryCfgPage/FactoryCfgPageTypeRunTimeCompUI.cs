using GF_Gereric;
using GKG;
using GKG.SubMM.Dispenser;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.FactoryCfgPage.DispensingFunctionHeadFactory;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.FactoryCfgPage
{
    internal class FactoryCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private readonly Guid _subMMObjID;
        private DispensingFunctionHeadSubMachineModulesFactoryCfg _data;
        private IPageTypeRunTimeCompUIView _factoryView;

        private event EventHandler AfterDataModified;

        public FactoryCfgPageTypeRunTimeCompUI(Guid subMMObjID)
        {
            _subMMObjID = subMMObjID;
        }

        protected override void _OnInit()
        {
            _data = new DispensingFunctionHeadSubMachineModulesFactoryCfg();
        }

        private void AfterModified(object sender, EventArgs e)
        {
            if (_factoryView is DispensingFunctionHeadFactoryPageTypeRunTimeCompUIView view)
            {
                _data = view.GetData() ?? new DispensingFunctionHeadSubMachineModulesFactoryCfg();
            }

            AfterDataModified?.Invoke(sender, e);
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.FactoryCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == FactoryCfgPageTypeConst.ViewID_DispensingFunctionHeadFactory)
            {
                if (_factoryView != null)
                {
                    _factoryView.AfterModified -= AfterModified;
                }

                _factoryView = new DispensingFunctionHeadFactoryPageTypeRunTimeCompUIView(this.CallBack);
                _factoryView.AfterModified += AfterModified;
                (_factoryView as DispensingFunctionHeadFactoryPageTypeRunTimeCompUIView)?.SetData(
                    _data ?? new DispensingFunctionHeadSubMachineModulesFactoryCfg());
                return _factoryView;
            }

            return null;
        }

        protected override void _SetData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            _data = JsonObjConvert.FromJSonBytes<DispensingFunctionHeadSubMachineModulesFactoryCfg>(data)
                ?? new DispensingFunctionHeadSubMachineModulesFactoryCfg();
            if (_factoryView is DispensingFunctionHeadFactoryPageTypeRunTimeCompUIView view)
            {
                view.SetData(_data);
            }
        }

        protected override byte[] _GetData()
        {
            if (_factoryView is DispensingFunctionHeadFactoryPageTypeRunTimeCompUIView view)
            {
                _data = view.GetData() ?? new DispensingFunctionHeadSubMachineModulesFactoryCfg();
            }

            return JsonObjConvert.ToJSonBytes(_data ?? new DispensingFunctionHeadSubMachineModulesFactoryCfg());
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
