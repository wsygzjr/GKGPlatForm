using Avalonia.Controls;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.RecipeCfgPage.ViewModels;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.RecipeCfgPage.Views;
using Griffins.Map.UI;
using Griffins.PF;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.RecipeCfgPage
{
    internal class TransSpeedRecipePageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly TransSpeedRecipeCompUIView view;
        private readonly TransSpeedRecipeCompUIViewModel viewModel;
        private RailWorkStationSubMachineModulesPPCfg data;
        private event EventHandler afterModified;

        public TransSpeedRecipePageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            viewModel = new TransSpeedRecipeCompUIViewModel(callBack);
            view = new TransSpeedRecipeCompUIView();
            view.DataContext = viewModel;
            viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(viewModel.TransSpeedGearID))
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

        public void SetData(RailWorkStationSubMachineModulesPPCfg data)
        {
            this.data = data ?? new RailWorkStationSubMachineModulesPPCfg();
            viewModel.SetData(this.data);
        }

        public void RefreshSpeedGearOptions()
        {
            viewModel.LoadSpeedGearOptions();
        }

        public RailWorkStationSubMachineModulesPPCfg GetData()
        {
            data ??= new RailWorkStationSubMachineModulesPPCfg();
            data.WorkStationTransSpeed = viewModel.GetData();
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
