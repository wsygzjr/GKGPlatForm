using GKG;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.CylinderPushRodFactory.ViewModels;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.CylinderPushRodFactory.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using GKG.SubMM;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.CylinderPushRodFactory
{
    internal class CylinderPushRodFactoryPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly CylinderPushRodFactoryCompUIView _view;
        private readonly CylinderPushRodFactoryCompUIViewModel _viewModel;

        private event EventHandler _afterModified;

        public CylinderPushRodFactoryPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new CylinderPushRodFactoryCompUIView();
            _viewModel = new CylinderPushRodFactoryCompUIViewModel(callBack);
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

        public void SetData(CylinderPushRodSubMachineModulesFactoryCfg data)
        {
            _viewModel.SetData(data);
        }

        public CylinderPushRodSubMachineModulesFactoryCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
