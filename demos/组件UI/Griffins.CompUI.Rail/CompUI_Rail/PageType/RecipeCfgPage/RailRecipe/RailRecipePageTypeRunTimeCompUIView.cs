using Griffins.CompUI.Rail.CompUI_Rail.PageType.RecipeCfgPage.RailRecipe.ViewModels;
using Griffins.CompUI.Rail.CompUI_Rail.PageType.RecipeCfgPage.RailRecipe.Views;
using GKG.MM;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.Rail.CompUI_Rail.PageType.RecipeCfgPage.RailRecipe
{
    internal class RailRecipePageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly RailRecipeCompUIView _view;
        private readonly RailRecipeCompUIViewModel _viewModel;
        private event EventHandler _afterModified;

        public RailRecipePageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new RailRecipeCompUIView();
            _viewModel = new RailRecipeCompUIViewModel();
            _view.DataContext = _viewModel;

            _viewModel.AfterModified += (_, e) => _afterModified?.Invoke(this, e);
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

        public void SetData(RailMachineModulesPPCfg data)
        {
            _viewModel.SetData(data);
        }

        public RailMachineModulesPPCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
