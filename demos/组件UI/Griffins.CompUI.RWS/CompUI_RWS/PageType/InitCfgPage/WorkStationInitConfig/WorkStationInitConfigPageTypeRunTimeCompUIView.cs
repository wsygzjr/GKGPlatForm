using Avalonia.Controls;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage.WorkStationInitConfig.ViewModels;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage.WorkStationInitConfig.Views;
using Griffins.Map.UI;
using Griffins.PF;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.InitCfgPage.WorkStationInitConfig
{
    internal class WorkStationInitConfigPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private event EventHandler afterModified;

        private readonly WorkStationInitConfigCompUIView view;
        private readonly WorkStationInitConfigCompUIViewModel viewModel;
        private bool viewReferenceInitialized;

        object IPageTypeRunTimeCompUIView.View
        {
            get
            {
                RemoveViewFromParent();
                if (!viewReferenceInitialized)
                {
                    viewModel.SetViewReference(view);
                    viewReferenceInitialized = true;
                }

                return view;
            }
        }

        public WorkStationInitConfigPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            view = new WorkStationInitConfigCompUIView();
            viewModel = new WorkStationInitConfigCompUIViewModel(false, callBack);
            view.DataContext = viewModel;
            viewModel.AfterModified += (_, __) => afterModified?.Invoke(this, EventArgs.Empty);
        }

        void IPageTypeRunTimeCompUIView.SetReadOnly(bool readOnly)
        {
            viewModel.ReadOnly = readOnly;
        }

        public void SetData(RailWorkStationSubMachineModulesInitCfg railWorkStationInitParameters)
        {
            viewModel.SetData(railWorkStationInitParameters);
        }

        public RailWorkStationSubMachineModulesInitCfg GetData()
        {
            return viewModel.GetData();
        }

        event EventHandler IPageTypeRunTimeCompUIView.AfterModified
        {
            add => afterModified += value;
            remove => afterModified -= value;
        }

        OpMngCellID[] IPageTypeRunTimeCompUIView.EditFuncMngCellIDs => null;

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
