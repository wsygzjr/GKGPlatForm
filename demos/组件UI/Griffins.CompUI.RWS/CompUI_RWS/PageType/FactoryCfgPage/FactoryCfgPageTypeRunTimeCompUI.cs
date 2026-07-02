using GF_Gereric;
using Griffins.CompUI.RWS.CompUI_RWS;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.FactoryCfgPage.WorkStationFactoryConfig;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.FactoryCfgPage
{
    internal class FactoryCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private WorkStationFactoryConfigPageTypeRunTimeCompUIView workStationFactoryConfigView;
        private RailWorkStationSubMachineModulesFactoryCfg data;

        protected override void _OnInit()
        {
            data = new RailWorkStationSubMachineModulesFactoryCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.FactoryCfgPage; }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != FactoryCfgPageTypeConst.ViewID_WorkStationFactoryConfig)
            {
                return null;
            }

            if (workStationFactoryConfigView != null)
            {
                workStationFactoryConfigView.AfterModified -= OnAfterModified;
            }

            workStationFactoryConfigView = new WorkStationFactoryConfigPageTypeRunTimeCompUIView();
            workStationFactoryConfigView.AfterModified += OnAfterModified;
            workStationFactoryConfigView.SetData(data ?? new RailWorkStationSubMachineModulesFactoryCfg());
            return workStationFactoryConfigView;
        }

        protected override void _SetData(byte[] rawData)
        {
            if (rawData == null)
            {
                return;
            }

            data = JsonObjConvert.FromJSonBytes<RailWorkStationSubMachineModulesFactoryCfg>(rawData) ?? new RailWorkStationSubMachineModulesFactoryCfg();
            workStationFactoryConfigView?.SetData(data);
        }

        protected override byte[] _GetData()
        {
            if (workStationFactoryConfigView != null)
            {
                data = workStationFactoryConfigView.GetData();
            }

            data ??= new RailWorkStationSubMachineModulesFactoryCfg();
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
            if (workStationFactoryConfigView != null)
            {
                data = workStationFactoryConfigView.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
        }
    }
}
