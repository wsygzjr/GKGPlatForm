using Avalonia;
using Avalonia.Controls;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage.AdjustWidthRecipe.ViewModels;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage.AdjustWidthRecipe.Views;
using Griffins.Map.UI;
using Griffins.PF;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage.AdjustWidthRecipe
{
    internal class AdjustWidthRecipePageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly AdjustWidthRecipeCompUIViewModel viewModel;
        private RailAdjustWidthSubMachineModulesPPCfg data;
        private event EventHandler afterModified;

        public AdjustWidthRecipePageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            viewModel = new AdjustWidthRecipeCompUIViewModel(callBack);
            viewModel.AfterModified += (_, __) => afterModified?.Invoke(this, EventArgs.Empty);
        }

        public object View
        {
            get
            {
                var view = new AdjustWidthRecipeCompUIView
                {
                    DataContext = viewModel,
                };
                view.DetachedFromVisualTree += OnViewDetachedFromVisualTree;
                return view;
            }
        }

        public OpMngCellID[] EditFuncMngCellIDs => null;

        public event EventHandler AfterModified
        {
            add => afterModified += value;
            remove => afterModified -= value;
        }

        public void SetReadOnly(bool readOnly)
        {
            viewModel.ReadOnly = readOnly;
        }

        public void SetData(RailAdjustWidthSubMachineModulesPPCfg recipeData)
        {
            data = recipeData ?? new RailAdjustWidthSubMachineModulesPPCfg();
            viewModel.SetData(data);
        }

        public RailAdjustWidthSubMachineModulesPPCfg GetData()
        {
            data = viewModel.GetData();
            return data;
        }

        private void OnViewDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            viewModel.Cleanup();
        }
    }
}
