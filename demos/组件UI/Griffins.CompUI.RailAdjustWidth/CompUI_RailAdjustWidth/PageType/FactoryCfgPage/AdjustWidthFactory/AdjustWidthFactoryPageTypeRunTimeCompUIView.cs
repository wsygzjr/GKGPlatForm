using Avalonia.Controls;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.FactoryCfgPage.AdjustWidthFactory.ViewModels;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.FactoryCfgPage.AdjustWidthFactory.Views;
using Griffins.Map.UI;
using Griffins.PF;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.FactoryCfgPage.AdjustWidthFactory
{
    internal class AdjustWidthFactoryPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly AdjustWidthFactoryCompUIView view;
        private readonly AdjustWidthFactoryCompUIViewModel viewModel;
        private RailAdjustWidthSubMachineModulesFactoryCfg data;
        private event EventHandler afterModified;

        public AdjustWidthFactoryPageTypeRunTimeCompUIView()
        {
            view = new AdjustWidthFactoryCompUIView();
            viewModel = new AdjustWidthFactoryCompUIViewModel();
            view.DataContext = viewModel;
            viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(viewModel.FrontRailIsMovable)
                    || e.PropertyName == nameof(viewModel.BackRailIsMovable)
                    || e.PropertyName == nameof(viewModel.MaxWidthText)
                    || e.PropertyName == nameof(viewModel.MinWidthText))
                {
                    afterModified?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public object View
        {
            get
            {
                RemoveViewFromParent();
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

        public void SetData(RailAdjustWidthSubMachineModulesFactoryCfg data)
        {
            this.data = data ?? new RailAdjustWidthSubMachineModulesFactoryCfg();
            viewModel.SetData(this.data);
        }

        public RailAdjustWidthSubMachineModulesFactoryCfg GetData()
        {
            data = viewModel.GetData();
            return data;
        }

        private void RemoveViewFromParent()
        {
            if (view.Parent is Panel panelParent && panelParent.Children.Contains(view))
            {
                panelParent.Children.Remove(view);
            }
            else if (view.Parent is ContentControl contentParent && Equals(contentParent.Content, view))
            {
                contentParent.Content = null;
            }
        }
    }
}
