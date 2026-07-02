using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.CylinderPushRodInit.ViewModels;
using Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.CylinderPushRodInit.Views;
using GKG;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using GKG.SubMM;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.InitCfgPage.CylinderPushRodInit
{
    internal class CylinderPushRodInitPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly CylinderPushRodInitCompUIView _view;
        private readonly CylinderPushRodInitCompUIViewModel _viewModel;
        private event EventHandler _afterModified;

        public CylinderPushRodInitPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new CylinderPushRodInitCompUIView();
            _viewModel = new CylinderPushRodInitCompUIViewModel(callBack);
            _view.DataContext = _viewModel;
            _viewModel.SetViewReference(_view);
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

        public void SetData(CylinderPushRodSubMachineModulesInitCfg data)
        {
            _viewModel.SetData(data);
        }

        public CylinderPushRodSubMachineModulesInitCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
