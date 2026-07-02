using GF_Gereric;
using Griffins.CompUI.GantryClean.CompUI_GantryClean.PageType.InitCfgPage.GantryCleanConfig;
using GKG.SubMM;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using Griffins.ImeIOT.Map;

namespace Griffins.CompUI.GantryClean.CompUI_GantryClean.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private IPageTypeRunTimeCompUIView gantryCleanConfigView;

        private GantryCleanSubMachineModulesInitCfg _data;

        protected override void _OnInit()
        {
            _data = new GantryCleanSubMachineModulesInitCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("InitCfgPage"); }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == InitCfgPageTypeConst.ViewID_GantryCleanConfig)
            {
                if (gantryCleanConfigView == null)
                {
                    gantryCleanConfigView = new GantryCleanConfigPageTypeRunTimeCompUIView(this.CallBack);
                    gantryCleanConfigView.AfterModified += OnAfterModified;
                    (gantryCleanConfigView as GantryCleanConfigPageTypeRunTimeCompUIView)?.SetData(_data?.CleanParameters ?? new CleanParameters());
                }
                return gantryCleanConfigView;
            }

            return null;
        }

        protected override void _SetData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            _data = JsonObjConvert.FromJSonBytes<GantryCleanSubMachineModulesInitCfg>(data) ?? new GantryCleanSubMachineModulesInitCfg();

            if (gantryCleanConfigView is GantryCleanConfigPageTypeRunTimeCompUIView gc)
            {
                gc.SetData(_data.CleanParameters);
            }
        }

        protected override byte[] _GetData()
        {
            if (gantryCleanConfigView is GantryCleanConfigPageTypeRunTimeCompUIView gc)
            {
                _data ??= new GantryCleanSubMachineModulesInitCfg();
                _data.CleanParameters = gc.GetData();
            }

            return JsonObjConvert.ToJSonBytes(_data ?? new GantryCleanSubMachineModulesInitCfg());
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            if (gantryCleanConfigView is GantryCleanConfigPageTypeRunTimeCompUIView gc)
            {
                _data ??= new GantryCleanSubMachineModulesInitCfg();
                _data.CleanParameters = gc.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            return null;
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = null;
            return true;
        }
    }
}
