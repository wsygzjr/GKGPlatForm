using GF_Gereric;
using GKG.SubMM.Dispenser;
using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage.WeighingBalanceFactory;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage
{
    /// <summary>
    /// 出厂配置页面运行态组件。
    /// </summary>
    internal class FactoryCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private WeighingBalanceFactoryPageTypeRunTimeCompUIView factoryView;
        private WeighingBalanceSubMachineModulesFactoryCfg data;

        protected override void _OnInit()
        {
            data = new WeighingBalanceSubMachineModulesFactoryCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.FactoryCfgPage; }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != FactoryCfgPageTypeConst.ViewID_WeighingBalanceFactory)
            {
                return null;
            }

            if (factoryView != null)
            {
                factoryView.AfterModified -= OnAfterModified;
            }

            factoryView = new WeighingBalanceFactoryPageTypeRunTimeCompUIView();
            factoryView.AfterModified += OnAfterModified;
            factoryView.SetData(data ?? new WeighingBalanceSubMachineModulesFactoryCfg());
            return factoryView;
        }

        protected override void _SetData(byte[] rawData)
        {
            data = rawData == null || rawData.Length == 0
                ? new WeighingBalanceSubMachineModulesFactoryCfg()
                : JsonObjConvert.FromJSonBytes<WeighingBalanceSubMachineModulesFactoryCfg>(rawData)
                    ?? new WeighingBalanceSubMachineModulesFactoryCfg();

            factoryView?.SetData(data);
        }

        protected override byte[] _GetData()
        {
            if (factoryView != null)
            {
                data = factoryView.GetData();
            }

            return JsonObjConvert.ToJSonBytes(data ?? new WeighingBalanceSubMachineModulesFactoryCfg());
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
            if (factoryView != null)
            {
                data = factoryView.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
        }
    }
}
