using GF_Gereric;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.RecipeCfgPage
{
    internal class RecipeCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private RailWorkStationSubMachineModulesPPCfg data;
        private TransSpeedRecipePageTypeRunTimeCompUIView transSpeedView;

        protected override void _OnInit()
        {
            data = new RailWorkStationSubMachineModulesPPCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.PPCfgPage; }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != RecipeCfgPageTypeConst.ViewID_TransSpeed)
            {
                return null;
            }

            if (transSpeedView == null)
            {
                transSpeedView = new TransSpeedRecipePageTypeRunTimeCompUIView(CallBack);
                transSpeedView.AfterModified += OnAfterModified;
                transSpeedView.SetData(data ?? new RailWorkStationSubMachineModulesPPCfg());
            }
            else
            {
                transSpeedView.RefreshSpeedGearOptions();
            }

            return transSpeedView;
        }

        protected override void _SetData(byte[] rawData)
        {
            data = rawData == null || rawData.Length == 0
                ? new RailWorkStationSubMachineModulesPPCfg()
                : JsonObjConvert.FromJSonBytes<RailWorkStationSubMachineModulesPPCfg>(rawData) ?? new RailWorkStationSubMachineModulesPPCfg();

            transSpeedView?.SetData(data);
        }

        protected override byte[] _GetData()
        {
            if (transSpeedView != null)
            {
                data = transSpeedView.GetData();
            }

            return JsonObjConvert.ToJSonBytes(data ?? new RailWorkStationSubMachineModulesPPCfg());
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
            if (transSpeedView != null)
            {
                data = transSpeedView.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
        }
    }
}
