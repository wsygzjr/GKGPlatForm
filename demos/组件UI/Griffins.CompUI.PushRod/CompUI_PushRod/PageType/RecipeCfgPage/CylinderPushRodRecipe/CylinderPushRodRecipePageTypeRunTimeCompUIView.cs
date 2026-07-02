using GKG;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage.CylinderPushRodRecipe.ViewModels;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage.CylinderPushRodRecipe.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using GKG.SubMM;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage.CylinderPushRodRecipe
{
    internal class CylinderPushRodRecipePageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly CylinderPushRodRecipeCompUIView _view;
        private readonly CylinderPushRodRecipeCompUIViewModel _viewModel;

        private event EventHandler _afterModified;

        public CylinderPushRodRecipePageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new CylinderPushRodRecipeCompUIView();
            _viewModel = new CylinderPushRodRecipeCompUIViewModel(callBack);
            _view.DataContext = _viewModel;

            _viewModel.AfterModified += (s, e) => _afterModified?.Invoke(this, e);
        }

        public object View => _view;

        public OpMngCellID[] EditFuncMngCellIDs => null;

        public event EventHandler AfterModified
        {
            add => _afterModified += value;
            remove => _afterModified -= value;
        }

        public void SetReadOnly(bool readOnly)
        {
            _viewModel.ReadOnly = readOnly;
        }

        public void SetData(CylinderPushRodSubMachineModulesPPCfg data)
        {
            _viewModel.SetData(data);
        }

        public CylinderPushRodSubMachineModulesPPCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
