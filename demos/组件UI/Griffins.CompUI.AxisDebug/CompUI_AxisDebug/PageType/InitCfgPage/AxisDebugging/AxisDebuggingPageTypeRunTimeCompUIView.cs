using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.AxisDebugging.ViewModels;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.AxisDebugging.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.AxisDebugging
{
    internal class AxisDebuggingPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly AxisDebuggingCompUIView _view;
        private readonly AxisDebuggingCompUIViewModel _viewModel;

        private event EventHandler _afterModified;

        public AxisDebuggingPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new AxisDebuggingCompUIView();
            _viewModel = new AxisDebuggingCompUIViewModel(callBack);
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
    }
}
