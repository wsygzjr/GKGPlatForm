using GF_Gereric;
using Griffins.CompUI.Rail.CompUI_Rail.PageType.RecipeCfgPage.RailRecipe;
using GKG.MM;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.Rail.CompUI_Rail.PageType.RecipeCfgPage
{
    internal class RecipeCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private IPageTypeRunTimeCompUIView _recipeView;
        private RailMachineModulesPPCfg _data;
        private bool _isApplyingViewData;

        protected override void _OnInit()
        {
            _data = new RailMachineModulesPPCfg();
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.PPCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != RecipeCfgPageTypeConst.ViewID_RailRecipe)
                return null;

            if (_recipeView != null)
                _recipeView.AfterModified -= OnAfterModified;

            _recipeView = new RailRecipePageTypeRunTimeCompUIView(CallBack);
            _recipeView.AfterModified += OnAfterModified;
            ApplyViewData(_data ?? new RailMachineModulesPPCfg());
            return _recipeView;
        }

        protected override void _SetData(byte[] data)
        {
            _data = data == null || data.Length == 0
                ? new RailMachineModulesPPCfg()
                : JsonObjConvert.FromJSonBytes<RailMachineModulesPPCfg>(data)
                    ?? new RailMachineModulesPPCfg();

            ApplyViewData(_data);
        }

        protected override byte[] _GetData()
        {
            if (_recipeView is RailRecipePageTypeRunTimeCompUIView recipeView)
                _data = recipeView.GetData();

            return JsonObjConvert.ToJSonBytes(_data ?? new RailMachineModulesPPCfg());
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
            if (_isApplyingViewData)
                return;

            if (_recipeView is RailRecipePageTypeRunTimeCompUIView recipeView)
                _data = recipeView.GetData();

            AfterDataModified?.Invoke(sender, e);
        }

        private void ApplyViewData(RailMachineModulesPPCfg data)
        {
            if (_recipeView is not RailRecipePageTypeRunTimeCompUIView recipeView)
                return;

            _isApplyingViewData = true;
            try
            {
                recipeView.SetData(data);
            }
            finally
            {
                _isApplyingViewData = false;
            }
        }
    }
}
