using Avalonia.Controls;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage.AdjustWidthInit.ViewModels;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage.AdjustWidthInit.Views;
using Griffins.Map.UI;
using Griffins.PF;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.InitCfgPage.AdjustWidthInit
{
    internal class AdjustWidthInitPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView, IDisposable
    {
        private readonly AdjustWidthInitCompUIView view;
        private readonly AdjustWidthInitCompUIViewModel viewModel;
        private RailAdjustWidthSubMachineModulesInitCfg data;
        private event EventHandler afterModified;

        public AdjustWidthInitPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            view = new AdjustWidthInitCompUIView();
            viewModel = new AdjustWidthInitCompUIViewModel(false, callBack);
            view.DataContext = viewModel;
            viewModel.AfterModified += (_, __) => afterModified?.Invoke(this, EventArgs.Empty);
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

        public void SetData(RailAdjustWidthSubMachineModulesInitCfg data)
        {
            this.data = data ?? new RailAdjustWidthSubMachineModulesInitCfg();
            viewModel.SetData(this.data);
        }

        public RailAdjustWidthSubMachineModulesInitCfg GetData()
        {
            data = viewModel.GetData();
            return data;
        }

        public void Dispose()
        {
            viewModel.Dispose();
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
