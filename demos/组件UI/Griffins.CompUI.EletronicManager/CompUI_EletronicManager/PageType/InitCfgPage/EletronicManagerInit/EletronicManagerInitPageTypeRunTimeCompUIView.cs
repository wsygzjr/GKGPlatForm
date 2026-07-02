using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage.EletronicManagerInit.ViewModels;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage.EletronicManagerInit.Views;
using GKG.SubMM;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage.EletronicManagerInit
{
    internal class EletronicManagerInitPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly EletronicManagerInitCompUIView _view;
        private readonly EletronicManagerInitCompUIViewModel _viewModel;
        private event EventHandler _afterModified;

        public EletronicManagerInitPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new EletronicManagerInitCompUIView();
            _viewModel = new EletronicManagerInitCompUIViewModel(callBack);
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

        public void SetData(EletronicManagerSubMachineModulesInitCfg data)
        {
            _viewModel.SetData(data);
        }

        public EletronicManagerSubMachineModulesInitCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
