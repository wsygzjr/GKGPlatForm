using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage.DispensingFunctionHeadPP.ViewModels;
using Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage.DispensingFunctionHeadPP.Views;
using GKG.SubMM.Dispenser;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage.DispensingFunctionHeadPP
{
    /// <summary>
    /// 点胶机功能头配方页面运行时CompUIView
    /// </summary>
    internal class DispensingFunctionHeadPPPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly DispensingFunctionHeadPPCompUIView _view;
        private readonly DispensingFunctionHeadPPCompUIViewModel _viewModel;
        private event EventHandler _afterModified;

        public DispensingFunctionHeadPPPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new DispensingFunctionHeadPPCompUIView();
            _viewModel = new DispensingFunctionHeadPPCompUIViewModel(callBack);
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

        public void SetData(DispensingFunctionHeadSubMachineModulesPPCfg data)
        {
            _viewModel.SetData(data);
        }

        public DispensingFunctionHeadSubMachineModulesPPCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
