using GF_Gereric;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.FactoryCfgPage.AdjustWidthFactory;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.FactoryCfgPage
{
    internal class FactoryCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private AdjustWidthFactoryPageTypeRunTimeCompUIView adjustWidthFactoryView;
        private RailAdjustWidthSubMachineModulesFactoryCfg data;

        protected override void _OnInit()
        {
            data = new RailAdjustWidthSubMachineModulesFactoryCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.FactoryCfgPage; }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != FactoryCfgPageTypeConst.ViewID_AdjustWidthFactory)
            {
                return null;
            }

            if (adjustWidthFactoryView != null)
            {
                adjustWidthFactoryView.AfterModified -= OnAfterModified;
            }

            adjustWidthFactoryView = new AdjustWidthFactoryPageTypeRunTimeCompUIView();
            adjustWidthFactoryView.AfterModified += OnAfterModified;
            adjustWidthFactoryView.SetData(data ?? new RailAdjustWidthSubMachineModulesFactoryCfg());
            return adjustWidthFactoryView;
        }

        protected override void _SetData(byte[] rawData)
        {
            if (rawData == null)
            {
                return;
            }

            data = JsonObjConvert.FromJSonBytes<RailAdjustWidthSubMachineModulesFactoryCfg>(rawData)
                ?? new RailAdjustWidthSubMachineModulesFactoryCfg();
            adjustWidthFactoryView?.SetData(data);
        }

        protected override byte[] _GetData()
        {
            if (adjustWidthFactoryView != null)
            {
                data = adjustWidthFactoryView.GetData();
            }

            data ??= new RailAdjustWidthSubMachineModulesFactoryCfg();
            return JsonObjConvert.ToJSonBytes(data);
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
            if (adjustWidthFactoryView != null)
            {
                data = adjustWidthFactoryView.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
        }
    }
}
