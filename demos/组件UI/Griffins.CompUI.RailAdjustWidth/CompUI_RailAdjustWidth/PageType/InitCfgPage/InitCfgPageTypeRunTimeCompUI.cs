using GF_Gereric;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage.AdjustWidthInit;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private AdjustWidthInitPageTypeRunTimeCompUIView adjustWidthInitView;
        private RailAdjustWidthSubMachineModulesInitCfg data;

        protected override void _OnInit()
        {
            data = new RailAdjustWidthSubMachineModulesInitCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.InitCfgPage; }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != InitCfgPageTypeConst.ViewID_AdjustWidthInit)
            {
                return null;
            }

            if (adjustWidthInitView != null)
            {
                adjustWidthInitView.AfterModified -= OnAfterModified;
                adjustWidthInitView.Dispose();
            }

            adjustWidthInitView = new AdjustWidthInitPageTypeRunTimeCompUIView(CallBack);
            adjustWidthInitView.AfterModified += OnAfterModified;
            adjustWidthInitView.SetData(data ?? new RailAdjustWidthSubMachineModulesInitCfg());
            return adjustWidthInitView;
        }

        protected override void _SetData(byte[] rawData)
        {
            if (rawData == null)
            {
                return;
            }

            data = JsonObjConvert.FromJSonBytes<RailAdjustWidthSubMachineModulesInitCfg>(rawData)
                ?? new RailAdjustWidthSubMachineModulesInitCfg();
            adjustWidthInitView?.SetData(data);
        }

        protected override byte[] _GetData()
        {
            if (adjustWidthInitView != null)
            {
                data = adjustWidthInitView.GetData();
            }

            return JsonObjConvert.ToJSonBytes(data ?? new RailAdjustWidthSubMachineModulesInitCfg());
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            if (adjustWidthInitView != null)
            {
                data = adjustWidthInitView.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
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
