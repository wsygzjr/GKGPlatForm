using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage.DispensingFunctionHeadInit.ViewModels;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage.DispensingFunctionHeadInit.Views;
using GKG;
using GKG.SubMM.Dispenser;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.InitCfgPage.DispensingFunctionHeadInit
{
    internal class DispensingFunctionHeadInitPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly DispensingFunctionHeadInitCompUIView _view;
        private readonly DispensingFunctionHeadInitCompUIViewModel _viewModel;
        private event EventHandler _afterModified;

        public DispensingFunctionHeadInitPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new DispensingFunctionHeadInitCompUIView();
            _viewModel = new DispensingFunctionHeadInitCompUIViewModel(callBack);
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

        public void SetData(DispensingFunctionHeadSubMachineModulesInitCfg data)
        {
            _viewModel.SetData(data);
        }

        public DispensingFunctionHeadSubMachineModulesInitCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
