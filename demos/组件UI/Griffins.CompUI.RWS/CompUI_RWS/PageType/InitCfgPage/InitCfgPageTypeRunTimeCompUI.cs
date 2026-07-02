using GF_Gereric;
using Griffins;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage.WorkStationInitConfig;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        

        private WorkStationInitConfigPageTypeRunTimeCompUIView workStationInitConfigView;
        private RailWorkStationSubMachineModulesInitCfg data;

        protected override void _OnInit()
        {
            data = new RailWorkStationSubMachineModulesInitCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.InitCfgPage; }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != InitCfgPageTypeConst.ViewID_WorkStationInitConfig)
            {
                return null;
            }

            if (workStationInitConfigView == null)
            {
                workStationInitConfigView = new WorkStationInitConfigPageTypeRunTimeCompUIView(CallBack);
                ((IPageTypeRunTimeCompUIView)workStationInitConfigView).AfterModified += OnAfterModified;
                workStationInitConfigView.SetData(data ?? new RailWorkStationSubMachineModulesInitCfg());
            }

            return workStationInitConfigView;
        }

        protected override void _SetData(byte[] rawData)
        {
            if (rawData == null)
            {
                return;
            }

            
            data = JsonObjConvert.FromJSonBytes<RailWorkStationSubMachineModulesInitCfg>(rawData);
            if (data == null)
            {
                var old = JsonObjConvert.FromJSonBytes<InitCfgPageTypeData>(rawData);
                data = old?.WorkStationInitConfigCompUIModel;
            }

            data ??= new RailWorkStationSubMachineModulesInitCfg();
            workStationInitConfigView?.SetData(data);
            
        }

        protected override byte[] _GetData()
        {
            if (workStationInitConfigView != null)
            {
                data = workStationInitConfigView.GetData() ?? new RailWorkStationSubMachineModulesInitCfg();
            }

            data ??= new RailWorkStationSubMachineModulesInitCfg();
            return JsonObjConvert.ToJSonBytes(data);
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            if (workStationInitConfigView != null)
            {
                data = workStationInitConfigView.GetData() ?? new RailWorkStationSubMachineModulesInitCfg();
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

        // 与 FactoryCfgPage 保持一致：直接对 RailWorkStationSubMachineModulesInitCfg 做序列化/反序列化。
        private sealed class InitCfgPageTypeData
        {
            public RailWorkStationSubMachineModulesInitCfg WorkStationInitConfigCompUIModel { get; set; }
        }
    }
}
