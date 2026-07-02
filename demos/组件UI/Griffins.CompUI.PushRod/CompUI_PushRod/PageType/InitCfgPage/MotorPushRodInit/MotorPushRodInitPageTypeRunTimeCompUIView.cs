using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.MotorPushRodInit.ViewModels;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.MotorPushRodInit.Views;
using GKG;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using GKG.SubMM;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.MotorPushRodInit
{
    internal class MotorPushRodInitPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly MotorPushRodInitCompUIView _view;
        private readonly MotorPushRodInitCompUIViewModel _viewModel;
        private event EventHandler _afterModified;

        public MotorPushRodInitPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new MotorPushRodInitCompUIView();
            _viewModel = new MotorPushRodInitCompUIViewModel(callBack);
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

        public void SetData(MotorPushRodSubMachineModulesInitCfg data)
        {
            _viewModel.SetData(data);
        }

        public MotorPushRodSubMachineModulesInitCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
