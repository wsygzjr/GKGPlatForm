using Griffins.CompUI.Rail.CompUI_Rail.PageType.InitCfgPage.RailInit.ViewModels;
using Griffins.CompUI.Rail.CompUI_Rail.PageType.InitCfgPage.RailInit.Views;
using GKG.MM;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.Rail.CompUI_Rail.PageType.InitCfgPage.RailInit
{
    internal class RailInitPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly RailInitCompUIView _view;
        private readonly RailInitCompUIViewModel _viewModel;
        private event EventHandler _afterModified;

        public RailInitPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new RailInitCompUIView();
            _viewModel = new RailInitCompUIViewModel(callBack);
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

        public void SetData(RailMachineModulesInitCfg data)
        {
            _viewModel.SetData(data);
        }

        public RailMachineModulesInitCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
