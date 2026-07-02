using GF_Gereric;
using GKG.SubMM.Dispenser;
using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage.WeighingBalance;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private IPageTypeRunTimeCompUIView weighingBalanceView;

        private WeighingBalanceSubMachineModulesInitCfg data;

        protected override void _OnInit()
        {
            data = new WeighingBalanceSubMachineModulesInitCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("InitCfgPage"); }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == InitCfgPageTypeConst.ViewID_WeighingBalance)
            {
                // 兜底：每次进入页面都重新创建视图，避免复用导致 DataGrid 首次渲染不刷新、必须点击才显示的问题
                if (weighingBalanceView != null)
                {
                    weighingBalanceView.AfterModified -= OnAfterModified;
                }

                weighingBalanceView = new WeighingBalancePageTypeRunTimeCompUIView(this.CallBack);
                weighingBalanceView.AfterModified += OnAfterModified;
                (weighingBalanceView as WeighingBalancePageTypeRunTimeCompUIView)?.SetData(data ?? new WeighingBalanceSubMachineModulesInitCfg());
                return weighingBalanceView;
            }

            return null;
        }

        protected override void _SetData(byte[] dataBytes)
        {
            data = dataBytes == null || dataBytes.Length == 0
                ? new WeighingBalanceSubMachineModulesInitCfg()
                : JsonObjConvert.FromJSonBytes<WeighingBalanceSubMachineModulesInitCfg>(dataBytes)
                    ?? new WeighingBalanceSubMachineModulesInitCfg();

            if (weighingBalanceView is WeighingBalancePageTypeRunTimeCompUIView view)
            {
                view.SetData(data);
            }
        }

        protected override byte[] _GetData()
        {
            if (weighingBalanceView is WeighingBalancePageTypeRunTimeCompUIView view)
            {
                data = view.GetData();
            }

            return JsonObjConvert.ToJSonBytes(data);
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            if (weighingBalanceView is WeighingBalancePageTypeRunTimeCompUIView view)
            {
                data = view.GetData();
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
