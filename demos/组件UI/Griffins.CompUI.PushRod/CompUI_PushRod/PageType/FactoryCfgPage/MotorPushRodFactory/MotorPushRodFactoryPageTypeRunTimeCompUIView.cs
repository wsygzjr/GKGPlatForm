using GKG;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.MotorPushRodFactory.ViewModels;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.MotorPushRodFactory.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using GKG.SubMM;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.MotorPushRodFactory
{
    internal class MotorPushRodFactoryPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly MotorPushRodFactoryCompUIView _view;
        private readonly MotorPushRodFactoryCompUIViewModel _viewModel;

        private event EventHandler _afterModified;

        public MotorPushRodFactoryPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new MotorPushRodFactoryCompUIView();
            _viewModel = new MotorPushRodFactoryCompUIViewModel(callBack);
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

        public void SetData(MotorPushRodSubMachineModulesFactoryCfg data)
        {
            _viewModel.SetData(data);
        }

        public MotorPushRodSubMachineModulesFactoryCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
