using Avalonia.Controls;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.FactoryCfgPage.WorkStationFactoryConfig.ViewModels;
using Griffins.CompUI.RWS.CompUI_RWS.PageType.FactoryCfgPage.WorkStationFactoryConfig.Views;
using Griffins.Map.UI;
using Griffins.PF;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.FactoryCfgPage.WorkStationFactoryConfig
{
    internal class WorkStationFactoryConfigPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly WorkStationFactoryConfigCompUIView view;
        private readonly WorkStationFactoryConfigCompUIViewModel viewModel;
        private RailWorkStationSubMachineModulesFactoryCfg data;
        private event EventHandler afterModified;

        public WorkStationFactoryConfigPageTypeRunTimeCompUIView()
        {
            view = new WorkStationFactoryConfigCompUIView();
            viewModel = new WorkStationFactoryConfigCompUIViewModel();
            view.DataContext = viewModel;
            viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(viewModel.HasProximitySensor)
                    || e.PropertyName == nameof(viewModel.HasLeftSensor)
                    || e.PropertyName == nameof(viewModel.HasRightSensor)
                    || e.PropertyName == nameof(viewModel.HasLeftBlock)
                    || e.PropertyName == nameof(viewModel.HasRightBlock)
                    || e.PropertyName == nameof(viewModel.IsSupportLeftIn)
                    || e.PropertyName == nameof(viewModel.IsSupportLeftOut)
                    || e.PropertyName == nameof(viewModel.IsSupportRightIn)
                    || e.PropertyName == nameof(viewModel.IsSupportRightOut))
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

        public void SetData(RailWorkStationSubMachineModulesFactoryCfg data)
        {
            this.data = data ?? new RailWorkStationSubMachineModulesFactoryCfg();
            viewModel.SetData(this.data);
        }

        public RailWorkStationSubMachineModulesFactoryCfg GetData()
        {
            data ??= new RailWorkStationSubMachineModulesFactoryCfg();
            data.WorkStationEleConfigParams = viewModel.GetEleConfigData();
            data.WorkStationCapability = viewModel.GetCapabilityData();
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
