using GKG;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage.MotorPushRodRecipe.ViewModels;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage.MotorPushRodRecipe.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using GKG.SubMM;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage.MotorPushRodRecipe
{
    internal class MotorPushRodRecipePageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly MotorPushRodRecipeCompUIView _view;
        private readonly MotorPushRodRecipeCompUIViewModel _viewModel;

        private event EventHandler _afterModified;

        public MotorPushRodRecipePageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new MotorPushRodRecipeCompUIView();
            _viewModel = new MotorPushRodRecipeCompUIViewModel(callBack);
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

        public void SetData(MotorPushRodSubMachineModulesPPCfg data)
        {
            _viewModel.SetData(data);
        }

        public MotorPushRodSubMachineModulesPPCfg GetData()
        {
            return _viewModel.GetData();
        }

        public void Cleanup()
        {
            _viewModel.Cleanup();
        }
    }
}
