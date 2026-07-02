using GF_Gereric;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage.AdjustWidthRecipe;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage
{
    internal class RecipeCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private AdjustWidthRecipePageTypeRunTimeCompUIView adjustWidthRecipeView;
        private RailAdjustWidthSubMachineModulesPPCfg data;

        protected override void _OnInit()
        {
            data = new RailAdjustWidthSubMachineModulesPPCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.PPCfgPage; }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != RecipeCfgPageTypeConst.ViewID_AdjustWidthRecipe)
            {
                return null;
            }

            if (adjustWidthRecipeView != null)
            {
                adjustWidthRecipeView.AfterModified -= OnAfterModified;
            }

            adjustWidthRecipeView = new AdjustWidthRecipePageTypeRunTimeCompUIView(CallBack);
            adjustWidthRecipeView.AfterModified += OnAfterModified;
            adjustWidthRecipeView.SetData(data ?? new RailAdjustWidthSubMachineModulesPPCfg());
            return adjustWidthRecipeView;
        }

        protected override void _SetData(byte[] rawData)
        {
            data = rawData == null || rawData.Length == 0
                ? new RailAdjustWidthSubMachineModulesPPCfg()
                : JsonObjConvert.FromJSonBytes<RailAdjustWidthSubMachineModulesPPCfg>(rawData)
                    ?? new RailAdjustWidthSubMachineModulesPPCfg();

            adjustWidthRecipeView?.SetData(data);
        }

        protected override byte[] _GetData()
        {
            if (adjustWidthRecipeView != null)
            {
                data = adjustWidthRecipeView.GetData();
            }

            return JsonObjConvert.ToJSonBytes(data ?? new RailAdjustWidthSubMachineModulesPPCfg());
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
            if (adjustWidthRecipeView != null)
            {
                data = adjustWidthRecipeView.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
        }
    }
}
