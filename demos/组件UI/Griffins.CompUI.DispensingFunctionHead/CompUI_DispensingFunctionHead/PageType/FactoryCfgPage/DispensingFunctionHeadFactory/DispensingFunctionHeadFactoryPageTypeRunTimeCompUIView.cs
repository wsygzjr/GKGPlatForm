using GKG;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.FactoryCfgPage.DispensingFunctionHeadFactory.ViewModels;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.FactoryCfgPage.DispensingFunctionHeadFactory.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using GKG.SubMM.Dispenser;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.FactoryCfgPage.DispensingFunctionHeadFactory
{
    internal class DispensingFunctionHeadFactoryPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly DispensingFunctionHeadFactoryCompUIView _view;
        private readonly DispensingFunctionHeadFactoryCompUIViewModel _viewModel;

        private event EventHandler _afterModified;

        public DispensingFunctionHeadFactoryPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new DispensingFunctionHeadFactoryCompUIView();
            _viewModel = new DispensingFunctionHeadFactoryCompUIViewModel(callBack);
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

        public void SetData(DispensingFunctionHeadSubMachineModulesFactoryCfg data)
        {
            _viewModel.SetData(data);
        }

        public DispensingFunctionHeadSubMachineModulesFactoryCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
